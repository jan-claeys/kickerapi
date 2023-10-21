using AutoMapper;
using ClassLibrary.Models;
using kickerapi;
using kickerapi.Controllers;
using kickerapi.Dtos.Requests.Match;
using kickerapi.Dtos.Responses.Match;
using kickerapi.QueryParameters;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static kickerapi.Dtos.Requests.Match.CreateMatchDto;

namespace Tests.Controllers
{
    public class MatchControllerTest : IDisposable
    {
        private readonly KickerContext _context;
        private readonly MatchesController _controller;

        public MatchControllerTest(KickerContext context, IMapper _mapper, UserManager<Player> userManager)
        {
            _context = context;
            _controller = new MatchesController(_context, _mapper, userManager);
        }

        [Fact]
        public async void ItGetsMatches()
        {
            await _context.Database.OpenConnectionAsync();
            await _context.Database.EnsureCreatedAsync();

            var player1 = new Player("test1");
            var player2 = new Player("test2");
            var player3 = new Player("test3");
            var player4 = new Player("test4");

            var team1 = new Team(player1, player2, 0);
            var team2 = new Team(player3, player4, 0);

            var match1 = new Match(team1, team2);
            _context.Matches.Add(match1);
            var match2 = new Match(team2, team1);
            _context.Matches.Add(match2);

            await _context.SaveChangesAsync();

            var response = await _controller.Get(new PagingParameters());
            Assert.Equal(200, response.StatusCode);

            var okResult = response as OkObjectResult;
            var result = okResult?.Value;

            Assert.Equal(2, Assert.IsType<List<MatchDto>>(result).Count);
        }

        [Fact]
        public async void ItCreateMatch()
        {
            await _context.Database.OpenConnectionAsync();
            await _context.Database.EnsureCreatedAsync();

            var player1 = new Player("test1");
            var player2 = new Player("test2");
            var player3 = new Player("test3");
            var player4 = new Player("test4");

            _context.Players.Add(player1);
            _context.Players.Add(player2);
            _context.Players.Add(player3);
            _context.Players.Add(player4);
            await _context.SaveChangesAsync();

            var createMatchDto = new CreateMatchDto() { 
                Team1 = new CreateTeamDto()
                {
                    AttackerId = player1.Id,
                    DefenderId = player2.Id
                },

                Team2 = new CreateTeamDto()
                {
                    AttackerId = player3.Id,
                    DefenderId = player4.Id
                }
            };

            var response = await _controller.Post(createMatchDto);
            Assert.Equal(201, response.StatusCode);

            Assert.Equal(1, _context.Matches.Count());
            Assert.Equal(2, _context.Teams.Count());
        }

        [Fact]
        public async void ItReturnErrorIfPlayerNotExist()
        {
            await _context.Database.OpenConnectionAsync();
            await _context.Database.EnsureCreatedAsync();

            var player1 = new Player("test1");
            var player2 = new Player("test2");
            var player3 = new Player("test3");
            var player4 = new Player("test4");

            _context.Players.Add(player1);
            _context.Players.Add(player2);
            _context.Players.Add(player3);
            _context.Players.Add(player4);
            await _context.SaveChangesAsync();

            var createMatchDto = new CreateMatchDto()
            {
                Team1 = new CreateTeamDto()
                {
                    AttackerId = 100,
                    DefenderId = player2.Id
                },

                Team2 = new CreateTeamDto()
                {
                    AttackerId = player3.Id,
                    DefenderId = player4.Id
                }
            };

            var response = await _controller.Post(createMatchDto);
            Assert.Equal(400, response.StatusCode);
        }

        public void Dispose()
        {
             _context.Database.EnsureDeleted();
            _context.Dispose();
        }
    }
}
