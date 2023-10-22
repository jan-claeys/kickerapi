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
        private readonly IMatchService _matchService;

        public MatchesController(KickerContext context, IMapper mapper, ISecurityService securityService, IMatchService matchService)
        {
            _context = context;
            _mapper = mapper;
            _securityService = securityService;
            _matchService = matchService;
        }

        [HttpGet]
        [Produces("application/json")]
        [ProducesResponseType(typeof(List<MatchDto>), StatusCodes.Status200OK)]
        public async Task<IStatusCodeActionResult> Get([FromQuery] MatchParameters parameters)
        {
            var player = await _securityService.GetUserAsync(User);

            var matches = await _mapper.ProjectTo<MatchDto>(_matchService.GetMatches(player, parameters.IsConfirmed)
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

                var attackerTeam1 = await _context.Players.FindAsync(req.Team1.AttackerId) ?? throw new Exception("One or more players are not existing");
                var defenderTeam1 = await _context.Players.FindAsync(req.Team1.DefenderId) ?? throw new Exception("One or more players are not existing");

                var attackerTeam2 = await _context.Players.FindAsync(req.Team2.AttackerId) ?? throw new Exception("One or more players are not existing");
                var defenderTeam2 = await _context.Players.FindAsync(req.Team2.DefenderId) ?? throw new Exception("One or more players are not existing");

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
