using AutoMapper;
using ClassLibrary.Models;
using kickerapi;
using kickerapi.Controllers;
using kickerapi.Dtos.Requests.Match;
using kickerapi.Dtos.Responses.Match;
using kickerapi.QueryParameters;
using kickerapi.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using System.Security.Claims;
using static kickerapi.Dtos.Requests.Match.CreateMatchDto;
using Match = ClassLibrary.Models.Match;

namespace Tests.Controllers
{
    public class MatchesControllerTest : IDisposable
    {
        private readonly KickerContext _context;
        private readonly MatchesController _controller;
        private readonly Player _currentPlayer;

        public MatchesControllerTest(KickerContext context, IMapper _mapper)
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
            var team4 = new Team(player3, _currentPlayer, 0);
            var team5 = new Team(player1, player4, 0);
            var team6 = new Team(player2, _currentPlayer, 0);
            var team7 = new Team(player3, player4, 0);
            var team8 = new Team(player1, player2, 0);

            team1.Confirm();
            team2.Confirm();
            team3.Confirm();
            team4.Confirm();
            team6.Confirm();

            var match1 = new Match(team1, team2);
            var match2 = new Match(team3, team4);
            var match3 = new Match(team5, team6);
            var match4 = new Match(team7, team8);

            _context.Matches.Add(match1);
            _context.Matches.Add(match2);
            _context.Matches.Add(match3);
            _context.Matches.Add(match4);

            await _context.SaveChangesAsync();

            var response = await _controller.Get(new MatchParameters() { IsConfirmed = true});
            Assert.Equal(200, response.StatusCode);

            var okResult = response as OkObjectResult;
            var result = okResult?.Value;

            var matchDtos = Assert.IsType<List<MatchDto>>(result);
            Assert.Equal(2, matchDtos.Count);

            var team = Assert.IsType<MatchDto.TeamDto>(matchDtos[1].Team1);
            Assert.Equal(_currentPlayer.Id, team.Attacker.Id);

            response = await _controller.Get(new MatchParameters() { IsConfirmed = false });
            Assert.Equal(200, response.StatusCode);

            okResult = response as OkObjectResult;
            result = okResult?.Value;

            matchDtos = Assert.IsType<List<MatchDto>>(result);
            Assert.Equal(1, matchDtos?.Count);
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
