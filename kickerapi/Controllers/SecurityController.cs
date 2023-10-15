using kickerapi.Dtos.Security;
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
    public class SecurityController : Controller
    {
        private readonly KickerContext _context;
        private readonly SecurityService _service;

        public SecurityController(KickerContext context, SecurityService service)
        {
            this._context = context;
            this._service = service;
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<IResult> Login(Login req)
        {
            var player = await _context.Players.FirstOrDefaultAsync(p => p.Name == req.UserName && p.Password == req.Password);

            if (player != null)
            {
                var token = _service.GenerateToken(player.Name);
                return Results.Ok(token);
            }

            return Results.Unauthorized();
        }
    }
}
