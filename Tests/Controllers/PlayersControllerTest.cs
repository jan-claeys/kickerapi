using AutoMapper;
using ClassLibrary.Models;
using kickerapi;
using kickerapi.Controllers;
using kickerapi.Dtos.Responses.Player;
using kickerapi.QueryParameters;
using Microsoft.AspNetCore.Mvc;

namespace Tests.Controllers
{
    public class PlayersControllerTest : DatabaseTest
    {
        private readonly PlayersController _controller;

        public PlayersControllerTest(KickerContext context, IMapper _mapper) : base(context)
        {
            _controller = new PlayersController(_context, _mapper);
        }

        [Fact]
        public async void ItGetsPlayersByRating()
        {
            var player1 = new Player("test1");
            player1.SetAttackRating(5);
            player1.SetDefendRating(20);
            await _context.Players.AddAsync(player1);

            var player2 = new Player("test2");
            player2.SetAttackRating(10);
            player2.SetDefendRating(10);
            await _context.Players.AddAsync(player2);

            await _context.SaveChangesAsync();

            var response = await _controller.GetRanking(new PlayersRatingParameters());
            Assert.Equal(200, response.StatusCode);

            var okResult = response as OkObjectResult;
            var result = okResult?.Value;

            Assert.Equal("test1", Assert.IsType<List<PlayerDto>>(result)[0].Name);
            Assert.Equal("test2", Assert.IsType<List<PlayerDto>>(result)[1].Name);

            response = await _controller.GetRanking(new PlayersRatingParameters()
            {
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
            var player1 = new Player("test1");
            player1.SetAttackRating(5);
            player1.SetDefendRating(20);
            await _context.Players.AddAsync(player1);

            var player2 = new Player("test2");
            player2.SetAttackRating(10);
            player2.SetDefendRating(10);
            await _context.Players.AddAsync(player2);

            await _context.SaveChangesAsync();

            var response = await _controller.GetRanking(new PlayersRatingParameters()
            {
                OrderBy = "AttackRating"
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
            var player1 = new Player("test1");
            player1.SetAttackRating(5);
            player1.SetDefendRating(20);
            await _context.Players.AddAsync(player1);

            var player2 = new Player("test2");
            player2.SetAttackRating(10);
            player2.SetDefendRating(10);
            await _context.Players.AddAsync(player2);

            await _context.SaveChangesAsync();

            var response = await _controller.GetRanking(new PlayersRatingParameters()
            {
                OrderBy = "DefendRating"
            });
            Assert.Equal(200, response.StatusCode);

            var okResult = response as OkObjectResult;
            var result = okResult?.Value;

            Assert.Equal("test1", Assert.IsType<List<PlayerDto>>(result)[0].Name);
            Assert.Equal("test2", Assert.IsType<List<PlayerDto>>(result)[1].Name);
        }

        [Fact]
        public async void ItGetsPlayersByName()
        {
            var player3 = new Player("cindy");
            await _context.Players.AddAsync(player3);

            var player2 = new Player("brian");
            await _context.Players.AddAsync(player2);

            var player1 = new Player("aaron");
            await _context.Players.AddAsync(player1);

            await _context.SaveChangesAsync();

            var response = await _controller.Get(new PlayersParameters());

            Assert.Equal(200, response.StatusCode);
            var okResult = response as OkObjectResult;

            var result = okResult?.Value;
            var players = Assert.IsType<List<PlayerDto>>(result);

            Assert.Equal("aaron", players[0].Name);
            Assert.Equal("brian", players[1].Name);
            Assert.Equal("cindy", players[2].Name);

            response = await _controller.Get(new PlayersParameters()
            {
                Search = "ria"
            });

            Assert.Equal(200, response.StatusCode);
            okResult = response as OkObjectResult;

            result = okResult?.Value;
            players = Assert.IsType<List<PlayerDto>>(result);

            Assert.Equal(1, players?.Count);
            Assert.Equal("brian", players?[0].Name);
        }
    }
}
