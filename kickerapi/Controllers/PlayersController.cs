using AutoMapper;
using ClassLibrary.Models;
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
        private readonly IPlayersService _service;

        public PlayersController(IMapper mapper, IPlayersService playersService)
        {
            _mapper = mapper;
            _service = playersService;
        }

        [HttpGet]
        [ProducesResponseType(typeof(List<PlayerDto>), StatusCodes.Status200OK)]
        public async Task<IStatusCodeActionResult> Get([FromQuery] PlayersParameters parameters)
        {
            var players = await _mapper.ProjectTo<PlayerDto>(
                _service.GetPlayers(parameters.Search)
                .Paging(parameters.PageNumber, parameters.PageSize))
                .ToListAsync();

            return Ok(players);
        }

        [HttpGet("ranking")]
        [ProducesResponseType(typeof(List<PlayerDto>), StatusCodes.Status200OK)]
        public async Task<IStatusCodeActionResult> GetRanking([FromQuery] PlayersRatingParameters parameters)
        {
            var players = await _mapper.ProjectTo<PlayerDto>(
                _service.GetPlayersRanking(parameters.OrderBy)
                .Paging(parameters.PageNumber, parameters.PageSize))
                .ToListAsync();

            return Ok(players);
        }

    }
}
