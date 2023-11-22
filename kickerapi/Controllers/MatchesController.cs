using AutoMapper;
using ClassLibrary.Models;
using kickerapi.Dtos;
using kickerapi.Dtos.Requests.Match;
using kickerapi.Dtos.Responses;
using kickerapi.Dtos.Responses.Match;
using kickerapi.QueryParameters;
using kickerapi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.EntityFrameworkCore;
using static kickerapi.Dtos.Responses.Match.MatchDto;

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
            Player player = await _securityService.GetUserAsync(User);

            var matches = await _matchService.GetMatchesWithPlayers(player, parameters.IsConfirmed, parameters.PlayerPosition)
                .OrderByDescending(x => x.Date)
                .Paging(parameters.PageNumber, parameters.PageSize).ToListAsync();

            var res = _mapper.Map<List<MatchDto>>(matches, options => options.Items["currentPlayer"] = player);
            return Ok(res);
        }

        [HttpGet("toreview")]
        [ProducesResponseType(typeof(List<MatchDto>), StatusCodes.Status200OK)]
        public async Task<IStatusCodeActionResult> ToReview([FromQuery] PagingParameters parameters)
        {
            Player player = await _securityService.GetUserAsync(User);

            var matches = await _matchService.GetMatchesToReview(player)
                .OrderByDescending(x => x.Date)
                .Paging(parameters.PageNumber, parameters.PageSize).ToListAsync();

            var res = _mapper.Map<List<MatchDto>>(matches, options => options.Items["currentPlayer"] = player);
            return Ok(res);
        }

        [HttpGet("underreview")]
        [ProducesResponseType(typeof(List<MatchDto>), StatusCodes.Status200OK)]
        public async Task<IStatusCodeActionResult> UnderReview([FromQuery] PagingParameters parameters)
        {
            Player player = await _securityService.GetUserAsync(User);

            var matches = await _matchService.GetMatchesUnderReview(player)
                .OrderByDescending(x => x.Date)
                .Paging(parameters.PageNumber, parameters.PageSize).ToListAsync();

            var res = _mapper.Map<List<MatchDto>>(matches, options => options.Items["currentPlayer"] = player);
            return Ok(res);
        }

        [HttpPost]
        [ProducesResponseType(typeof(MatchDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IStatusCodeActionResult> Post([FromBody] CreateMatchDto req)
        {
            try
            {
                Player player = await _securityService.GetUserAsync(User);
                Player attackerTeam1;
                Player defenderTeam1;

                if (req.PlayerPosition == Position.Attacker)
                {
                    attackerTeam1 = player;
                    defenderTeam1 = await _playerService.GetPlayer(req.AllyId);
                }
                else
                {
                    defenderTeam1 = player;
                    attackerTeam1 = await _playerService.GetPlayer(req.AllyId);
                }

                Player attackerTeam2 = await _playerService.GetPlayer(req.OpponentAttackerId);
                Player defenderTeam2 = await _playerService.GetPlayer(req.OpponentDefenderId);

                var team1 = new Team(attackerTeam1, defenderTeam1, req.PlayerScore);
                var team2 = new Team(attackerTeam2, defenderTeam2, req.OpponentScore);

                //confirm team of player who created match
                team1.Confirm();

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
