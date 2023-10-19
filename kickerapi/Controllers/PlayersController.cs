using AutoMapper;
using AutoMapper.QueryableExtensions;
using Azure;
using ClassLibrary.Models;
using kickerapi.Dtos.Player;
using kickerapi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
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
        private readonly UserManager<Player> _userManager;

        public PlayersController(KickerContext context, SecurityService securityService, IMapper mapper, UserManager<Player> userManager)
        {
            this._context = context;
            this._securityService = securityService;
            this._mapper = mapper;
            this._userManager = userManager;
        }

        [Authorize]
        [HttpGet]
        public async Task<IStatusCodeActionResult> Get()
        {
            var players = await _mapper.ProjectTo<PlayerDto>(_context.Players).ToListAsync();
            return Ok(players);
        }

        [AllowAnonymous]
        [HttpPost("Login")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IStatusCodeActionResult> Login([FromBody] LoginDto req)
        {
            var player = await _userManager.FindByNameAsync(req.Name);

            if(player == null || !await _userManager.CheckPasswordAsync(player, req.Password))
            {
                return Unauthorized("Invalid username or password");
            }

            var token = _securityService.GenerateJwtToken(player);

            return Ok(new
            {
                token = new JwtSecurityTokenHandler().WriteToken(token)
            });
        }

        [AllowAnonymous]
        [HttpPost("Register")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IStatusCodeActionResult> Register([FromBody] RegisterDto req)
        {
            var userExists = await _userManager.FindByNameAsync(req.Name);
            if (userExists != null)
                return BadRequest("Player already exists");

            var player = new Player(req.Name)
            {
                SecurityStamp = Guid.NewGuid().ToString(),
            };

            var response = await _userManager.CreateAsync(player, req.Password);
            if (!response.Succeeded)
            {
                return BadRequest(response.Errors);
            }

            return Ok();
        }
    }
}
