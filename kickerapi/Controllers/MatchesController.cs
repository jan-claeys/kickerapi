using AutoMapper;
using ClassLibrary.Models;
using kickerapi.Dtos.Requests.Match;
using kickerapi.Dtos.Responses.Match;
using kickerapi.QueryParameters;
using kickerapi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace kickerapi.Controllers
{
    [Route("[controller]")]
    [ApiController]
    [Authorize]
    public class MatchesController : ControllerBase
    {
        private readonly KickerContext _context;
        private readonly IMapper _mapper;
        private readonly ISecurityService _securityService;

        public MatchesController(KickerContext context, IMapper mapper, ISecurityService securityService)
        {
            this._context = context;
            this._mapper = mapper;
            this._securityService = securityService;
        }

        [HttpGet]
        [Produces("application/json")]
        [ProducesResponseType(typeof(List<MatchDto>), StatusCodes.Status200OK)]
        public async Task<IStatusCodeActionResult> Get([FromQuery] MatchParameters parameters)
        {
            var player = await _securityService.GetUserAsync(User);

            var allMatches = await _context.Matches.ToListAsync();

            var matches = await _mapper.ProjectTo<MatchDto>(_context.Matches
                .Where(x => x.Team1.Attacker.Id == player.Id || x.Team1.Defender.Id == player.Id || x.Team2.Attacker.Id == player.Id || x.Team2.Defender.Id == player.Id)
                .WhereIf(x=> x.Team1.IsConfirmed && x.Team2.IsConfirmed,parameters.IsConfirmed)
                .WhereIf(x => !x.Team1.IsConfirmed || !x.Team2.IsConfirmed, !parameters.IsConfirmed)
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
            try
            {
                var player = await _securityService.GetUserAsync(User);

                var attackerTeam1 = await _context.Players.FindAsync(req.Team1.AttackerId);
                var defenderTeam1 = await _context.Players.FindAsync(req.Team1.DefenderId);

                var attackerTeam2 = await _context.Players.FindAsync(req.Team2.AttackerId);
                var defenderTeam2 = await _context.Players.FindAsync(req.Team2.DefenderId);

                if (attackerTeam1 == null || defenderTeam1 == null || attackerTeam2 == null || defenderTeam2 == null)
                    throw new Exception("One or more players are not existing");

                if (attackerTeam1.Id != player.Id && defenderTeam1.Id != player.Id)
                    throw new Exception("You are not allowed to create a match with this players");

                var team1 = new Team(attackerTeam1, defenderTeam1, req.Team1.Score);
                var team2 = new Team(attackerTeam2, defenderTeam2, req.Team2.Score);

                var match = new Match(team1, team2);

                await _context.Matches.AddAsync(match);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(Get), match);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
