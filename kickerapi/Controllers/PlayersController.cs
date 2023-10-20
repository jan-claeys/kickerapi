using AutoMapper;
using kickerapi.Dtos.Player;
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
        public async Task<IStatusCodeActionResult> Get()
        {
            var players = await _mapper.ProjectTo<PlayerDto>(_context.Players).ToListAsync();
            return Ok(players);
        }

    }
}
