using ClassLibrary.Models;
using kickerapi.Dtos.Player;
using kickerapi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace kickerapi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PlayerController : Controller
    {
        private readonly KickerContext _context;
        private readonly SecurityService _securityService;
        private readonly PlayerService _playerService;

        public PlayerController(KickerContext context, SecurityService service, PlayerService playerService)
        {
            this._context = context;
            this._securityService = service;
            this._playerService = playerService;
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IResult> Login([FromBody] Login req)
        {
            var player = await _context.Players.FirstOrDefaultAsync(p => p.Name == req.Name);

            if (player != null && _securityService.VerifyPassword(req.Password, player.Password))
            {
                var token = _securityService.GenerateJwtToken(player.Name);
                return Results.Ok(token);
            }

            return Results.Unauthorized();
        }

        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<IResult> Register([FromBody] Register req)
        {
            var player = await _context.Players.FirstOrDefaultAsync(p => p.Name == req.Name);

            if (player != null)
            {
                return Results.BadRequest("Player already exists");
            }

            var newPlayer = new Player(req.Name, _securityService.HashPassword(req.Password));
            await _context.Players.AddAsync(newPlayer);
            await _context.SaveChangesAsync();

            var token = _securityService.GenerateJwtToken(newPlayer.Name);
            return Results.Ok(token);
        }
    }
}
