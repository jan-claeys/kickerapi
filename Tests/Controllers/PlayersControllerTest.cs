using AutoMapper;
using ClassLibrary.Models;
using kickerapi;
using kickerapi.Controllers;
using kickerapi.Dtos.Responses.Player;
using kickerapi.QueryParameters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Tests.Controllers
{
    public class PlayersControllerTest : IDisposable
    {
        private readonly KickerContext _context;
        private readonly PlayersController _controller;

        public PlayersControllerTest(KickerContext context, IMapper _mapper)
        {
            _context = context;
            _controller = new PlayersController(_context, _mapper);
        }

        [Fact]
        public async void ItGetsPlayersByRating()
        {
            await _context.Database.OpenConnectionAsync();
            await _context.Database.EnsureCreatedAsync();

            var player1 = new Player("test1");
            player1.SetAttackRating(5);
            player1.SetDefendRating(20);
            _context.Players.Add(player1);

            var player2 = new Player("test2");
            player2.SetAttackRating(10);
            player2.SetDefendRating(10);
            _context.Players.Add(player2);

            await _context.SaveChangesAsync();

            var response = await _controller.Get(new PlayersParameters());
            Assert.Equal(200, response.StatusCode);

            var okResult = response as OkObjectResult;
            var result = okResult?.Value;

            Assert.Equal("test1", Assert.IsType<List<PlayerDto>>(result)[0].Name);
            Assert.Equal("test2", Assert.IsType<List<PlayerDto>>(result)[1].Name);

            response = await _controller.Get(new PlayersParameters(){ 
                OrderBy = "Rating"
            });
            Assert.Equal(200, response.StatusCode);

            okResult = response as OkObjectResult;
            result = okResult?.Value;

            Assert.Equal("test1", Assert.IsType<List<PlayerDto>>(result)[0].Name);
            Assert.Equal("test2", Assert.IsType<List<PlayerDto>>(result)[1].Name);
        }

        [Fact]
        public async void ItGetsPlayersByAttackRating()
        {
            await _context.Database.OpenConnectionAsync();
            await _context.Database.EnsureCreatedAsync();

            var player1 = new Player("test1");
            player1.SetAttackRating(5);
            player1.SetDefendRating(20);
            _context.Players.Add(player1);

            var player2 = new Player("test2");
            player2.SetAttackRating(10);
            player2.SetDefendRating(10);
            _context.Players.Add(player2);

            await _context.SaveChangesAsync();

            var response = await _controller.Get(new PlayersParameters()
            {
                OrderBy= "AttackRating"
            });
            Assert.Equal(200, response.StatusCode);

            var okResult = response as OkObjectResult;
            var result = okResult?.Value;

            Assert.Equal("test2", Assert.IsType<List<PlayerDto>>(result)[0].Name);
            Assert.Equal("test1", Assert.IsType<List<PlayerDto>>(result)[1].Name);
        }

        [Fact]
        public async void ItGetsPlayersByDefendRating()
        {
            await _context.Database.OpenConnectionAsync();
            await _context.Database.EnsureCreatedAsync();

            var player1 = new Player("test1");
            player1.SetAttackRating(5);
            player1.SetDefendRating(20);
            _context.Players.Add(player1);

            var player2 = new Player("test2");
            player2.SetAttackRating(10);
            player2.SetDefendRating(10);
            _context.Players.Add(player2);

            await _context.SaveChangesAsync();

            var response = await _controller.Get(new PlayersParameters()
            {
                OrderBy = "DefendRating"
            });
            Assert.Equal(200, response.StatusCode);

            var okResult = response as OkObjectResult;
            var result = okResult?.Value;

            Assert.Equal("test1", Assert.IsType<List<PlayerDto>>(result)[0].Name);
            Assert.Equal("test2", Assert.IsType<List<PlayerDto>>(result)[1].Name);
        }

        public void Dispose()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }
    }
}
