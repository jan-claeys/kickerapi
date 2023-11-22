using kickerapi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.EntityFrameworkCore;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace kickerapi.Controllers
{
    [Route("[controller]")]
    [ApiController]
    [Authorize]
    public class TeamsController : ControllerBase
    {
        private readonly ISecurityService _securityService;
        private readonly IMatchesService _matchService;
        private readonly ITeamsService _teamsService;

        public TeamsController(ISecurityService securityService, IMatchesService matchService, ITeamsService teamsService)
        {
            _securityService = securityService;
            _matchService = matchService;
            _teamsService = teamsService;
        }

        [HttpPut("{id}/confirm")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IStatusCodeActionResult> Confirm([FromRoute] int id)
        {
            try
            {
                var player = await _securityService.GetUserAsync(User);
                var team = await _teamsService.GetTeamWithPlayers(id);

                if (team.Attacker != player && team.Defender != player)
                    throw new Exception("You are not allowed to confirm this team");

                team.Confirm();

                var matchesToUpdate = await _matchService.GetMatchesWithPlayers(player, false)
                    .ToListAsync();

                foreach (var match in matchesToUpdate)
                {
                    var isUpdated = match.UpdateRatings();
                    if (!isUpdated)
                        break;
                }

                await _teamsService.SaveChangesAsync();

                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{id}/deny")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IStatusCodeActionResult> Deny([FromRoute] int id)
        {
            try
            {
                var player = await _securityService.GetUserAsync(User);
                var team = await _teamsService.GetTeamWithPlayers(id);

                if (team.Attacker != player && team.Defender != player)
                    throw new Exception("You are not allowed to deny this team");

                var match = await _matchService.GetMatchWithTeams(team);

                var team1 = match.Team1;
                var team2 = match.Team2;

                _teamsService.RemoveTeam(team1);
                _teamsService.RemoveTeam(team2);
                _matchService.RemoveMatch(match);

                await _teamsService.SaveChangesAsync();

                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
