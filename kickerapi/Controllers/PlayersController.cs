using AutoMapper;
using ClassLibrary.Models;
using kickerapi.Dtos.Player;
using kickerapi.QueryParameters;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.EntityFrameworkCore;
using System.Linq;
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
            Expression<Func<Player, int>> order = parameters.OrderBy switch
            {
                "Rating" => x => x.Rating,
                "AttackRating" => x => x.AttackRating,
                "DeffendRating" => x => x.DeffendRating,
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
