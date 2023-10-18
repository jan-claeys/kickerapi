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
    [ApiController]
    [Route("[controller]")]
    public class PlayerController : Controller
    {
        private readonly KickerContext _context;
        private readonly SecurityService _service;

        public PlayerController(KickerContext context, SecurityService service)
        {
            this._context = context;
            this._service = service;
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IStatusCodeActionResult> Login([FromBody] LoginDto req)
        {
            var player = await _context.Players.FirstOrDefaultAsync(p => p.Name == req.Name);

            if (player != null && _service.VerifyPassword(req.Password, player.Password))
            {
                var token = _service.GenerateJwtToken(player.Name);
                return Ok(token);
            }

            return Unauthorized();
        }

        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<IStatusCodeActionResult> Register([FromBody] RegisterDto req)
        {
            var player = await _context.Players.FirstOrDefaultAsync(p => p.Name == req.Name);

            if (player != null)
            {
                return BadRequest("Player already exists");
            }

            var newPlayer = new Player(req.Name, _service.HashPassword(req.Password));
            await _context.Players.AddAsync(newPlayer);
            await _context.SaveChangesAsync();

            var token = _service.GenerateJwtToken(newPlayer.Name);
            return Ok(token);
        }
    }
}
