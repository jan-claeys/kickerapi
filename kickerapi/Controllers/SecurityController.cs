using ClassLibrary.Models;
using kickerapi.Dtos.Requests.Security;
using kickerapi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using System.IdentityModel.Tokens.Jwt;

namespace kickerapi.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class SecurityController : Controller
    {
        private readonly ISecurityService _service;

        public SecurityController(ISecurityService service)
        {
            _service = service;
        }

        [AllowAnonymous]
        [HttpPost("Login")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IStatusCodeActionResult> Login([FromBody] LoginDto req)
        {
            var player = await _service.FindByNameAsync(req.Name);

            if (player == null || !await _service.CheckPasswordAsync(player, req.Password))
            {
                return Unauthorized("Invalid username or password");
            }

            var token = _service.GenerateJwtToken(player);

            return Ok(new
            {
                token = new JwtSecurityTokenHandler().WriteToken(token)
            });
        }

        [AllowAnonymous]
        [HttpPost("Register")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IStatusCodeActionResult> Register([FromBody] RegisterDto req)
        {
            var userExists = await _service.FindByNameAsync(req.Name);
            if (userExists != null)
                return BadRequest("Player already exists");

            var player = new Player(req.Name)
            {
                SecurityStamp = Guid.NewGuid().ToString(),
            };

            var response = await _service.CreateAsync(player, req.Password);
            if (!response.Succeeded)
            {
                return BadRequest(response.Errors);
            }

            return Ok();
        }
    }
}
