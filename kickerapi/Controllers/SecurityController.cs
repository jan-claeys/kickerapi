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
        private readonly string _domain = "@tillit.be";

        public SecurityController(ISecurityService service)
        {
            _service = service;
        }

        [AllowAnonymous]
        [HttpPost("Login")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IStatusCodeActionResult> Login([FromBody] LoginDto req)
        {
            var email = req.Email + _domain;
            var player = await _service.FindByEmailAsync(email);

            if (player == null || !await _service.CheckPasswordAsync(player, req.Password))
            {
                return Unauthorized("Invalid email or password");
            }

            var token = _service.GenerateJwtToken(player);

            return Ok(new
            {   
                id = player.Id,
                token = new JwtSecurityTokenHandler().WriteToken(token)
            });
        }

        [AllowAnonymous]
        [HttpPost("Register")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        public async Task<IStatusCodeActionResult> Register([FromBody] RegisterDto req)
        {
            var email = req.Email + _domain;
            var player = new Player(req.Name, email);

            var response = await _service.CreateAsync(player, req.Password);
            if (!response.Succeeded)
            {
                return UnprocessableEntity(response.Errors.First().Description);
            }

            return Ok();
        }
    }
}
