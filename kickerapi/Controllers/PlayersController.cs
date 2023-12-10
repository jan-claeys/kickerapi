using AutoMapper;
using ClassLibrary.Models;
using kickerapi.Dtos.Responses;
using kickerapi.Dtos.Responses.Player;
using kickerapi.QueryParameters;
using kickerapi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace kickerapi.Controllers
{
    [Route("[controller]")]
    [ApiController]
    [Authorize]
    public class PlayersController : Controller
    {
        private readonly IMapper _mapper;
        private readonly IPlayersService _playerService;
        private readonly ISecurityService _securityService;

        public PlayersController(IMapper mapper, IPlayersService playersService, ISecurityService securityService)
        {
            _mapper = mapper;
            _playerService = playersService;
            _securityService = securityService;
        }

        [HttpGet]
        [ProducesResponseType(typeof(List<PlayerDto>), StatusCodes.Status200OK)]
        public async Task<IStatusCodeActionResult> Get([FromQuery] PlayersParameters parameters)
        {
            var players = await _mapper.ProjectTo<PlayerDto>(
                _playerService.GetPlayers(parameters.Search)
                .Paging(parameters.PageNumber, parameters.PageSize))
                .ToListAsync();

            return Ok(players);
        }

        [HttpGet("current")]
        [ProducesResponseType(typeof(PlayerDto), StatusCodes.Status200OK)]
        public async Task<IStatusCodeActionResult> GetCurrent()
        {
			Player player = await _securityService.GetUserAsync(User);
            PlayerDto playerDto = _mapper.Map<PlayerDto>(player);

			return Ok(playerDto);
		}

        [HttpGet("ranking")]
        [ProducesResponseType(typeof(List<PlayerDto>), StatusCodes.Status200OK)]
        public async Task<IStatusCodeActionResult> GetRanking([FromQuery] PlayersRatingParameters parameters)
        {
            var players = await _mapper.ProjectTo<PlayerDto>(
                _playerService.GetPlayersRanking(parameters.OrderBy)
                .Paging(parameters.PageNumber, parameters.PageSize))
                .ToListAsync();

            return Ok(players);
        }
        
    }
}
