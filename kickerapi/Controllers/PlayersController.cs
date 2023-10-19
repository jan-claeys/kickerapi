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
