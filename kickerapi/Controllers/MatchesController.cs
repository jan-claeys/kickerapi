using AutoMapper;
using ClassLibrary.Models;
using kickerapi.Dtos.Requests.Match;
using kickerapi.Dtos.Responses.Match;
using kickerapi.QueryParameters;
using kickerapi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace kickerapi.Controllers
{
    [Route("[controller]")]
    [ApiController]
    [Authorize]
    public class MatchesController : ControllerBase
    {
        private readonly KickerContext _context;
        private readonly IMapper _mapper;
        private readonly UserManager<Player> _userManager;

        public MatchesController(KickerContext context, IMapper mapper, UserManager<Player> userManager)
        {
            this._context = context;
            this._mapper = mapper;
            this._userManager = userManager;
        }

        [HttpGet]
        [Produces("application/json")]
        [ProducesResponseType(typeof(List<MatchDto>), StatusCodes.Status200OK)]
        public async Task<IStatusCodeActionResult> Get([FromQuery] PagingParameters parameters)
        {
            var player = await _userManager.GetUserAsync(User);

            var matches = await _mapper.ProjectTo<MatchDto>(_context.Matches
                .Where(x => x.Team1.Attacker.Id == player.Id || x.Team1.Deffender.Id == player.Id || x.Team2.Attacker.Id == player.Id || x.Team2.Deffender.Id == player.Id)
                .OrderByDescending(x => x.Date)
                .Skip((parameters.PageNumber - 1) * parameters.PageSize)
                .Take(parameters.PageSize))
                .ToListAsync();

            return Ok(matches);
        }

        [HttpPost]
        [ProducesResponseType(typeof(MatchDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IStatusCodeActionResult> Post([FromBody] CreateMatchDto req)
        {
            var attackerTeam1 = await _context.Players.FindAsync(req.Team1.AttackerId);
            var defenderTeam1 = await _context.Players.FindAsync(req.Team1.DefenderId);

            var attackerTeam2 = await _context.Players.FindAsync(req.Team2.AttackerId);
            var defenderTeam2 = await _context.Players.FindAsync(req.Team2.DefenderId);

            if (attackerTeam1 == null || defenderTeam1 == null || attackerTeam2 == null || defenderTeam2 == null)
                return BadRequest("Player not found");

            var team = new Team(attackerTeam1, defenderTeam1, req.Team1.Score);
            var team2 = new Team(attackerTeam2, defenderTeam2, req.Team2.Score);

            var match = new Match(team, team2);

            await _context.Matches.AddAsync(match);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(Get), match);
        }
    }
}
