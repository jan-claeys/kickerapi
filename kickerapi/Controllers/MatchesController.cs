﻿using AutoMapper;
using ClassLibrary.Models;
using kickerapi.Dtos.Requests.Match;
using kickerapi.Dtos.Responses;
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
        private readonly IMapper _mapper;
        private readonly ISecurityService _securityService;
        private readonly IMatchesService _matchService;
        private readonly IPlayersService _playerService;

        public MatchesController(IMapper mapper, ISecurityService securityService, IMatchesService matchService, IPlayersService playerService)
        {
            _mapper = mapper;
            _securityService = securityService;
            _matchService = matchService;
            _playerService = playerService;
        }

        [HttpGet]
        [ProducesResponseType(typeof(List<MatchDto>), StatusCodes.Status200OK)]
        public async Task<IStatusCodeActionResult> Get([FromQuery] MatchParameters parameters)
        {
            var player = await _securityService.GetUserAsync(User);

            var matches = await _mapper.ProjectTo<MatchDto>(_matchService.GetMatches(player, parameters.IsConfirmed)
                .OrderByDescending(x => x.Date)
                .Paging(parameters.PageNumber, parameters.PageSize))
                .ToListAsync();

            return Ok(matches);
        }

        [HttpPost]
        [ProducesResponseType(typeof(MatchDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IStatusCodeActionResult> Post([FromBody] CreateMatchDto req)
        {
            try
            {
                var player = await _securityService.GetUserAsync(User);

                var attackerTeam1 = await _playerService.GetPlayer(req.Team1.AttackerId).FirstOrDefaultAsync() ?? throw new Exception("One or more players are not existing");
                var defenderTeam1 = await _playerService.GetPlayer(req.Team1.DefenderId).FirstOrDefaultAsync() ?? throw new Exception("One or more players are not existing");

                var attackerTeam2 = await _playerService.GetPlayer(req.Team2.AttackerId).FirstOrDefaultAsync() ?? throw new Exception("One or more players are not existing");
                var defenderTeam2 = await _playerService.GetPlayer(req.Team2.DefenderId).FirstOrDefaultAsync() ?? throw new Exception("One or more players are not existing");

                if (attackerTeam1.Id != player.Id && defenderTeam1.Id != player.Id)
                    throw new Exception("You are not allowed to create a match with this players");

                var team1 = new Team(attackerTeam1, defenderTeam1, req.Team1.Score);
                var team2 = new Team(attackerTeam2, defenderTeam2, req.Team2.Score);

                var match = new Match(team1, team2);

                _matchService.AddMatch(match);
                await _matchService.SaveChangesAsync();

                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
