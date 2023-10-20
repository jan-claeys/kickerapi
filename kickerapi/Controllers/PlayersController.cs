using AutoMapper;
using kickerapi.Dtos.Player;
using kickerapi.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.EntityFrameworkCore;

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
        [ProducesResponseType(typeof(List<PlayerDto>),StatusCodes.Status200OK)]
        public async Task<IStatusCodeActionResult> Get([FromQuery]PagingParameters parameters)
        {
            var players = await _mapper.ProjectTo<PlayerDto>(_context.Players)
                .Skip((parameters.PageNumber - 1) * parameters.PageSize)
                .Take(parameters.PageSize)
                .ToListAsync();

            return Ok(players);
        }

    }
}
