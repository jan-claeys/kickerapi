using AutoMapper;
using ClassLibrary.Models;
using kickerapi.Dtos.Responses.Player;
using kickerapi.QueryParameters;
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
        private readonly KickerContext _context;
        private readonly IMapper _mapper;

        public PlayersController(KickerContext context, IMapper mapper)
        {
            this._context = context;
            this._mapper = mapper;
        }

        [HttpGet]
        [Produces("application/json")]
        [ProducesResponseType(typeof(List<PlayerDto>), StatusCodes.Status200OK)]
        public async Task<IStatusCodeActionResult> Get([FromQuery] PlayersParameters parameters)
        {
            var players = await _mapper.ProjectTo<PlayerDto>(_context.Players
                .WhereIf(x => x.UserName.Contains(parameters.Search), parameters.Search)
                .OrderBy(x => x.UserName)
                .Skip((parameters.PageNumber - 1) * parameters.PageSize)
                .Take(parameters.PageSize))
                .ToListAsync();

            return Ok(players);
        }

        [HttpGet("ranking")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(List<PlayerDto>), StatusCodes.Status200OK)]
        public async Task<IStatusCodeActionResult> GetRanking([FromQuery] PlayersRatingParameters parameters)
        {
            Expression<Func<Player, int>> order = parameters.OrderBy switch
            {
                "Rating" => x => x.Rating,
                "AttackRating" => x => x.AttackRating,
                "DefendRating" => x => x.DefendRating,
                _ => x => x.Rating,
            };

            var players = await _mapper.ProjectTo<PlayerDto>(_context.Players
                .OrderByDescending(order)
                .Skip((parameters.PageNumber - 1) * parameters.PageSize)
                .Take(parameters.PageSize))
                .ToListAsync();

            return Ok(players);
        }

    }
}
