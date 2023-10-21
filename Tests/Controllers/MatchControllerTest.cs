using AutoMapper;
using ClassLibrary.Models;
using kickerapi;
using kickerapi.Controllers;
using kickerapi.Dtos.Requests.Match;
using kickerapi.Dtos.Responses.Match;
using kickerapi.QueryParameters;
using kickerapi.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using System.Security.Claims;
using static kickerapi.Dtos.Requests.Match.CreateMatchDto;
using Match = ClassLibrary.Models.Match;

namespace Tests.Controllers
{
    public class MatchControllerTest : IDisposable
    {
        private readonly KickerContext _context;
        private readonly MatchesController _controller;
        private readonly Player _currentPlayer;

        public MatchControllerTest(KickerContext context, IMapper _mapper)
        {
            _currentPlayer = new Player("test");
            _context = context;
            var securityServiceMock = new Mock<ISecurityService>();
            securityServiceMock.Setup(x => x.GetUserAsync(It.IsAny<ClaimsPrincipal>()).Result).Returns(_currentPlayer);

            _controller = new MatchesController(_context, _mapper, securityServiceMock.Object);
        }

        [Fact]
        public async void ItGetsMatchesFromPlayer()
        {
            await _context.Database.OpenConnectionAsync();
            await _context.Database.EnsureCreatedAsync();

            _currentPlayer.Id = 1;

            var player1 = new Player("test1") { Id = 2 };
            var player2 = new Player("test2") { Id = 3 };
            var player3 = new Player("test3") { Id = 4 };
            var player4 = new Player("test4") { Id = 5 };

            var team1 = new Team(_currentPlayer, player2, 0);
            var team2 = new Team(player3, player4, 0);
            var team3 = new Team(player1, player2, 0);

            var match1 = new Match(team1, team2);
            var match2 = new Match(team2, team1);
            var match3 = new Match(team3, team2);
            _context.Matches.Add(match1);
            _context.Matches.Add(match2);
            _context.Matches.Add(match3);

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

            _context.Players.Add(_currentPlayer);
            _context.Players.Add(player1);
            _context.Players.Add(player2);
            _context.Players.Add(player3);

            await _context.SaveChangesAsync();

            var createMatchDto = new CreateMatchDto()
            {
                Team1 = new CreateTeamDto()
                {
                    AttackerId = _currentPlayer.Id,
                    DefenderId = player1.Id
                },

                Team2 = new CreateTeamDto()
                {
                    AttackerId = player2.Id,
                    DefenderId = player3.Id
                }
            };

            var response = await _controller.Post(createMatchDto);
            Assert.Equal(201, response.StatusCode);

            Assert.Equal(1, _context.Matches.Count());
            Assert.Equal(2, _context.Teams.Count());
        }

        [Fact]
        public async void ItReturnErrorIfPlayerNotExistCreateMatch()
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

        [Fact]
        public async void ItReturnErrorIfCurrentPlayerNotInTeam1()
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
            Assert.Equal(400, response.StatusCode);
        }

        public void Dispose()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }
    }
}
