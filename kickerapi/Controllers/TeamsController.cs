﻿using kickerapi.Services;
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
        private readonly KickerContext _context;
        private readonly ISecurityService _securityService;
        private readonly IMatchesService _matchService;
        private readonly ITeamsService _teamsService;

        public TeamsController(KickerContext context, ISecurityService securityService, IMatchesService matchService, ITeamsService teamsService)
        {
            _context = context;
            _securityService = securityService;
            _matchService = matchService;
            _teamsService = teamsService;
        }

        [HttpPut("confirm/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IStatusCodeActionResult> Confirm([FromRoute]int id)
        {
            try
            {
                var player = await _securityService.GetUserAsync(User);
                var team = await _teamsService.GetTeamWithPlayers(id).FirstOrDefaultAsync() ?? throw new Exception("Team not found");

                if (team.Attacker.Id != player.Id && team.Defender.Id != player.Id)
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

        [HttpDelete("deny/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IStatusCodeActionResult> Deny(int id)
        {
            try
            {
                var player = await _securityService.GetUserAsync(User);
                var team = await _teamsService.GetTeamWithPlayers(id).FirstOrDefaultAsync() ?? throw new Exception("Team not found");

                if (team.Attacker.Id != player.Id && team.Defender.Id != player.Id)
                    throw new Exception("You are not allowed to deny this team");

                var match = await _context.Matches.FirstOrDefaultAsync(x=>x.Team1.Id == team.Id || x.Team2.Id == team.Id) ?? throw new Exception("Match not found");

                var team1 = match.Team1;
                var team2 = match.Team2;

                _context.Teams.Remove(team1);
                _context.Teams.Remove(team2);
                _context.Matches.Remove(match);

                await _context.SaveChangesAsync();

                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
