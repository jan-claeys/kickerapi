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
    public class MatchesControllerTest : DatabaseTest
    {
        private readonly MatchesController _controller;
        private readonly Player _currentPlayer;

        public MatchesControllerTest(KickerContext context, IMapper _mapper): base(context)
        {
            _currentPlayer = new Player("test");
            var securityServiceMock = new Mock<ISecurityService>();
            securityServiceMock.Setup(x => x.GetUserAsync(It.IsAny<ClaimsPrincipal>()).Result).Returns(_currentPlayer);

            _controller = new MatchesController(_mapper, securityServiceMock.Object, new MatchesService(context), new PlayersService(context));
        }

        [Fact]
        public async void ItGetConfirmedMatchesFromCurrrentPlayer()
        {
            var player1 = new Player("test1");
            var player2 = new Player("test2");
            var player3 = new Player("test3");
            var player4 = new Player("test4");

            var team1 = new Team(_currentPlayer, player1, 0);
            var team2 = new Team(player2, player3, 0);

            var team3 = new Team(player1, player2, 0);
            var team4 = new Team(player3, player4, 0);

            team1.Confirm();
            team2.Confirm();
            team3.Confirm();
            team4.Confirm();

            var match1 = new Match(team1, team2);
            var match2 = new Match(team3, team4);

            match1.UpdateRatings();
            match2.UpdateRatings();

            await _context.Matches.AddAsync(match1);
            await _context.Matches.AddAsync(match2);
            await _context.SaveChangesAsync();


            var response = await _controller.Get(new MatchParameters() { IsConfirmed = true });
            Assert.Equal(200, response.StatusCode);

            var okResult = response as OkObjectResult;
            var result = okResult?.Value;
            
            var matchDtos = Assert.IsType<List<MatchDto>>(result);
            Assert.Equal(1, matchDtos?.Count);

            var team = Assert.IsType<MatchDto.TeamDto>(matchDtos?[0].Team1);
            Assert.Equal(_currentPlayer.Id, team?.Attacker?.Id);
        }

        [Fact]
        public async void ItGetNotConfirmedMatchesFromCurrrentPlayer()
        {
            var player1 = new Player("test1");
            var player2 = new Player("test2");
            var player3 = new Player("test3");

            var team1 = new Team(_currentPlayer, player1, 0);
            var team2 = new Team(player2, player3, 0);

            var team3 = new Team(_currentPlayer, player1, 0);
            var team4 = new Team(player2, player3, 0);

            team1.Confirm();
            team2.Confirm();
            team3.Confirm();

            var match1 = new Match(team1, team2);
            var match2 = new Match(team3, team4);

            match1.UpdateRatings();
            match2.UpdateRatings();

            await _context.Matches.AddAsync(match1);
            await _context.Matches.AddAsync(match2);
            await _context.SaveChangesAsync();

            var response = await _controller.Get(new MatchParameters() { IsConfirmed = false });
            Assert.Equal(200, response.StatusCode);

            var okResult = response as OkObjectResult;
            var result = okResult?.Value;

            var test = await _context.Matches.ToListAsync();

            var matchDtos = Assert.IsType<List<MatchDto>>(result);
            Assert.Equal(1, matchDtos?.Count);

            var team = Assert.IsType<MatchDto.TeamDto>(matchDtos?[0].Team1);
            Assert.Equal(_currentPlayer.Id, team?.Attacker?.Id);
        }

        [Fact]
        public async void ItCreateMatch()
        {
            var player1 = new Player("test1");
            var player2 = new Player("test2");
            var player3 = new Player("test3");

            await _context.Players.AddAsync(_currentPlayer);
            await _context.Players.AddAsync(player1);
            await _context.Players.AddAsync(player2);
            await _context.Players.AddAsync(player3);

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
            Assert.Equal(200, response.StatusCode);

            Assert.Equal(1, _context.Matches.Count());
            Assert.Equal(2, _context.Teams.Count());
        }

        [Fact]
        public async void ItReturnErrorIfPlayerNotExistCreateMatch()
        {
            var player1 = new Player("test1");
            var player2 = new Player("test2");
            var player3 = new Player("test3");
            var player4 = new Player("test4");

            await _context.Players.AddAsync(player1);
            await _context.Players.AddAsync(player2);
            await _context.Players.AddAsync(player3);
            await _context.Players.AddAsync(player4);
            await _context.SaveChangesAsync();

            var createMatchDto = new CreateMatchDto()
            {
                Team1 = new CreateTeamDto()
                {
                    AttackerId = "100",
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
            var player1 = new Player("test1");
            var player2 = new Player("test2");
            var player3 = new Player("test3");
            var player4 = new Player("test4");

            await _context.Players.AddAsync(player1);
            await _context.Players.AddAsync(player2);
            await _context.Players.AddAsync(player3);
            await _context.Players.AddAsync(player4);

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
    }
}
