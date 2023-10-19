using AutoMapper;
using AutoMapper.QueryableExtensions;
using ClassLibrary.Models;
using kickerapi.Dtos.Player;
using kickerapi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace kickerapi.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class PlayersController : Controller
    {
        private readonly KickerContext _context;
        private readonly SecurityService _securityService;
        private readonly IMapper _mapper;

        public PlayersController(KickerContext context, SecurityService securityService, IMapper mapper)
        {
            this._context = context;
            this._securityService = securityService;
            this._mapper = mapper;
        }

        [Authorize]
        [HttpGet]
        public async Task<IStatusCodeActionResult> Get()
        {
            var players = await _context.Players.ToListAsync();
            return Ok(_mapper.Map<List<PlayerDto>>(players));
        }

        [AllowAnonymous]
        [HttpPost("Login")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IStatusCodeActionResult> Login([FromBody] LoginDto req)
        {
            var player = await _context.Players.FirstOrDefaultAsync(p => p.Name == req.Name);

            if (player != null && _securityService.VerifyPassword(req.Password, player.Password))
            {
                var token = _securityService.GenerateJwtToken(player.Name);
                return Ok(new TokenDto
                {
                    Token = token
                });
            }

            return Unauthorized();
        }

        [AllowAnonymous]
        [HttpPost("Register")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IStatusCodeActionResult> Register([FromBody] RegisterDto req)
        {
            var player = await _context.Players.FirstOrDefaultAsync(p => p.Name == req.Name);

            if (player != null)
            {
                return BadRequest("Player already exists");
            }

            var newPlayer = new Player(req.Name, _securityService.HashPassword(req.Password));
            await _context.Players.AddAsync(newPlayer);
            await _context.SaveChangesAsync();

            var token = _securityService.GenerateJwtToken(newPlayer.Name);
            return Ok(new TokenDto
            {
                Token = token
            });
        }
    }
}
