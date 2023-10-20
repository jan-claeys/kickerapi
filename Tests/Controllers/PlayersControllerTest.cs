using AutoMapper;
using ClassLibrary.Models;
using kickerapi;
using kickerapi.Controllers;
using kickerapi.Dtos.Player;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Tests.Controllers
{
    public class PlayersControllerTest
    {
        private readonly KickerContext _context;
        private readonly PlayersController _controller;

        public PlayersControllerTest(KickerContext context, IMapper _mapper)
        {
            _context = context;
            _controller = new PlayersController(_context, _mapper);
        }

        [Fact]
        public async void ItGetsPlayers()
        {
            await _context.Database.OpenConnectionAsync();
            await _context.Database.EnsureCreatedAsync();

            _context.Players.Add(new Player("test1"));
            _context.Players.Add(new Player("test2"));
            await _context.SaveChangesAsync();

            var response = await _controller.Get();
            Assert.Equal(200, response.StatusCode);

            var okResult = response as OkObjectResult;
            var result = okResult?.Value;

            Assert.Equal("test1", Assert.IsType<List<PlayerDto>>(result)[0].Name);
            Assert.Equal("test2", Assert.IsType<List<PlayerDto>>(result)[1].Name);
        }
    }
}
