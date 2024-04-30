using AutoMapper;
using ClassLibrary.Models;
using kickerapi;
using kickerapi.Controllers;
using kickerapi.Dtos;
using kickerapi.Dtos.Requests.Match;
using kickerapi.Dtos.Responses.Match;
using kickerapi.QueryParameters;
using kickerapi.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using System.Security.Claims;
using Match = ClassLibrary.Models.Match;

namespace Tests.Controllers
{
    public class MatchesControllerTest : DatabaseTest
    {
        private readonly MatchesController _controller;
        private readonly Player _currentPlayer;

        public MatchesControllerTest(KickerContext context, IMapper _mapper) : base(context)
        {
            _currentPlayer = new Player("test", "test@test.com");
            var securityServiceMock = new Mock<ISecurityService>();
            securityServiceMock.Setup(x => x.GetUserAsync(It.IsAny<ClaimsPrincipal>()).Result).Returns(_currentPlayer);

            _controller = new MatchesController(_mapper, securityServiceMock.Object, new MatchesService(context), new PlayersService(context));
        }

        [Fact]
        public async void ItGetConfirmedMatchesFromCurrrentPlayer()
        {
            var player1 = new Player("test1", "test1@test.com");
            var player2 = new Player("test2", "test2@test.com");
            var player3 = new Player("test3", "test3@test.com");
            var player4 = new Player("test4", "test4@test.com");

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

            var team = Assert.IsType<MatchDto.TeamDto>(matchDtos?[0].PlayerTeam);
            Assert.Equal(_currentPlayer.Id, team?.Attacker?.Id);
        }

        [Fact]
        public async void ItGetNotConfirmedMatchesFromCurrrentPlayer()
        {
            var player1 = new Player("test1", "test1@test.com");
            var player2 = new Player("test2", "test2@test.com");
            var player3 = new Player("test3", "test3@test.com");

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

            var team = Assert.IsType<MatchDto.TeamDto>(matchDtos?[0].PlayerTeam);
            Assert.Equal(_currentPlayer.Id, team?.Attacker?.Id);
        }

        [Fact]
        public async void ItCreateMatchWithPlayerAttacker()
        {
            var player1 = new Player("test1", "test1@test.com");
            var player2 = new Player("test2", "test2@test.com");
            var player3 = new Player("test3", "test3@test.com");

            await _context.Players.AddAsync(_currentPlayer);
            await _context.Players.AddAsync(player1);
            await _context.Players.AddAsync(player2);
            await _context.Players.AddAsync(player3);

            await _context.SaveChangesAsync();

            var createMatchDto = new CreateMatchDto()
            {
                PlayerPosition = Position.Attacker,
                AllyId = player1.Id,

                OpponentAttackerId = player2.Id,
                OpponentDefenderId = player3.Id
            };

            var response = await _controller.Post(createMatchDto);
            Assert.Equal(200, response.StatusCode);

            Assert.Equal(1, _context.Matches.Count());
            Assert.Equal(2, _context.Teams.Count());

            Assert.Equal(_currentPlayer.Id, _context.Teams.First().Attacker.Id);
        }

        [Fact]
        public async void ItConfirmsTeamPlayerWhoCreatesMath()
        {
            var player1 = new Player("test1", "test1@test.com");
            var player2 = new Player("test2", "test2@test.com");
            var player3 = new Player("test3", "test3@test.com");

            await _context.Players.AddAsync(_currentPlayer);
            await _context.Players.AddAsync(player1);
            await _context.Players.AddAsync(player2);
            await _context.Players.AddAsync(player3);

            await _context.SaveChangesAsync();

            var createMatchDto = new CreateMatchDto()
            {
                PlayerPosition = Position.Attacker,
                AllyId = player1.Id,

                OpponentAttackerId = player2.Id,
                OpponentDefenderId = player3.Id
            };

            var response = await _controller.Post(createMatchDto);
            Assert.Equal(200, response.StatusCode);

            var teamCurrentPlayer = _context.Teams.First(x => x.Attacker.Id == _currentPlayer.Id);

            Assert.True(teamCurrentPlayer.IsConfirmed);
        }


        [Fact]
        public async void ItCreateMatchWithPlayerDefender()
        {
            var player1 = new Player("test1", "test1@test.com");
            var player2 = new Player("test2", "test2@test.com");
            var player3 = new Player("test3", "test3@test.com");

            await _context.Players.AddAsync(_currentPlayer);
            await _context.Players.AddAsync(player1);
            await _context.Players.AddAsync(player2);
            await _context.Players.AddAsync(player3);

            await _context.SaveChangesAsync();

            var createMatchDto = new CreateMatchDto()
            {
                PlayerPosition = Position.Defender,
                AllyId = player1.Id,

                OpponentAttackerId = player2.Id,
                OpponentDefenderId = player3.Id
            };

            var response = await _controller.Post(createMatchDto);
            Assert.Equal(200, response.StatusCode);

            Assert.Equal(1, _context.Matches.Count());
            Assert.Equal(2, _context.Teams.Count());

            Assert.Equal(_currentPlayer.Id, _context.Teams.First().Defender.Id);
        }

        [Fact]
        public async void ItReturnsErrorIfPlayerDoesNotExist()
        {
            var player1 = new Player("test1", "test1@test.com");
            var player2 = new Player("test2", "test2@test.com");
            var player3 = new Player("test3", "test3@test.com");

            await _context.Players.AddAsync(_currentPlayer);
            await _context.Players.AddAsync(player1);
            await _context.Players.AddAsync(player2);
            await _context.Players.AddAsync(player3);

            await _context.SaveChangesAsync();

            var createMatchDto = new CreateMatchDto()
            {
                PlayerPosition = Position.Defender,
                AllyId = "fakeid",

                OpponentAttackerId = player2.Id,
                OpponentDefenderId = player3.Id
            };

            var response = await _controller.Post(createMatchDto);
            Assert.Equal(404, response.StatusCode);

            Assert.Equal(0, _context.Matches.Count());
            Assert.Equal(0, _context.Teams.Count());
        }

        [Fact]
        public async void ItReturnplayerTeamIsTeamCurrentPlayer()
        {
            var player1 = new Player("test1", "test1@test.com");
            var player2 = new Player("test2", "test2@test.com");
            var player3 = new Player("test3", "test3@test.com");

            var team1 = new Team(player2, player1, 0);
            var team2 = new Team(_currentPlayer, player3, 0);

            var match1 = new Match(team1, team2);

            team1.Confirm();
            team2.Confirm();
            match1.UpdateRatings();

            await _context.Matches.AddAsync(match1);
            await _context.SaveChangesAsync();

            var response = await _controller.Get(new MatchParameters() { IsConfirmed = true });
            Assert.Equal(200, response.StatusCode);

            var okResult = response as OkObjectResult;
            var result = okResult?.Value;
            var matchDtos = Assert.IsType<List<MatchDto>>(result);
            Assert.Equal(_currentPlayer.Id, matchDtos[0].PlayerTeam?.Attacker?.Id);
        }

        [Fact]
        public async void ItReturnOpponentTeamIsTeamOpponentPlayers()
        {
            var player1 = new Player("test1", "test1@test.com");
            var player2 = new Player("test2", "test2@test.com");
            var player3 = new Player("test3", "test3@test.com");

            var team1 = new Team(_currentPlayer, player1, 0);
            var team2 = new Team(player2, player3, 0);

            var match1 = new Match(team1, team2);

            team1.Confirm();
            team2.Confirm();
            match1.UpdateRatings();

            await _context.Matches.AddAsync(match1);
            await _context.SaveChangesAsync();

            var response = await _controller.Get(new MatchParameters() { IsConfirmed = true });
            Assert.Equal(200, response.StatusCode);

            var okResult = response as OkObjectResult;
            var result = okResult?.Value;
            var matchDtos = Assert.IsType<List<MatchDto>>(result);
            Assert.Equal(player2.Id, matchDtos[0].OpponentTeam?.Attacker?.Id);
        }

        [Fact]
        public async void ItReturnsMatchesToReview()
        {
            var player1 = new Player("test1", "test1@test.com");
            var player2 = new Player("test2", "test2@test.com");
            var player3 = new Player("test3", "test3@test.com");

            var team1 = new Team(_currentPlayer, player1, 0);
            var team2 = new Team(player2, player3, 0);
            team2.Confirm();

            var match1 = new Match(team1, team2);

            var team3 = new Team(_currentPlayer, player1, 0);
            var team4 = new Team(player2, player3, 0);
            team3.Confirm();
            team4.Confirm();

            var match2 = new Match(team3, team4);

            var team5 = new Team(player2, player1, 0);
            var team6 = new Team(player3, _currentPlayer, 0);
            team5.Confirm();

            var match3 = new Match(team5, team6);

            await _context.Matches.AddRangeAsync(match1, match2, match3);
            await _context.SaveChangesAsync();

            var response = await _controller.ToReview(new MatchParameters());
            Assert.Equal(200, response.StatusCode);

            var okResult = response as OkObjectResult;
            var result = okResult?.Value;
            var matchDtos = Assert.IsType<List<MatchDto>>(result);
            Assert.Equal(2, matchDtos.Count);
        }

        [Fact]
        public async void ItReturnsMatchesToReviewCount()
        {
            var player1 = new Player("test1", "test1@test.com");
            var player2 = new Player("test2", "test2@test.com");
            var player3 = new Player("test3", "test3@test.com");

            var team1 = new Team(_currentPlayer, player1, 0);
            var team2 = new Team(player2, player3, 0);
            team2.Confirm();

            var match1 = new Match(team1, team2);

            var team3 = new Team(_currentPlayer, player1, 0);
            var team4 = new Team(player2, player3, 0);
            team3.Confirm();
            team4.Confirm();

            var match2 = new Match(team3, team4);

            var team5 = new Team(player2, player1, 0);
            var team6 = new Team(player3, _currentPlayer, 0);
            team5.Confirm();

            var match3 = new Match(team5, team6);

            await _context.Matches.AddRangeAsync(match1, match2, match3);
            await _context.SaveChangesAsync();

            var response = await _controller.ToReviewCount();
            Assert.Equal(200, response.StatusCode);

            var okResult = response as OkObjectResult;
            var result = okResult?.Value;
            var matchCount = Assert.IsType<int>(result);
            Assert.Equal(2, matchCount);
        }

        [Fact]
        public async void ItReturnsMatchesUnderReview()
        {
            var player1 = new Player("test1", "test1@test.com");
            var player2 = new Player("test2", "test2@test.com");
            var player3 = new Player("test3", "test3@test.com");

            var team1 = new Team(_currentPlayer, player1, 0);
            var team2 = new Team(player2, player3, 0);
            team1.Confirm();

            var match1 = new Match(team1, team2);

            var team3 = new Team(_currentPlayer, player1, 0);
            var team4 = new Team(player2, player3, 0);
            team3.Confirm();
            team4.Confirm();

            var match2 = new Match(team3, team4);
            match2.UpdateRatings();

            var team5 = new Team(player2, player1, 0);
            var team6 = new Team(player3, _currentPlayer, 0);
            team6.Confirm();

            var match3 = new Match(team5, team6);

            await _context.Matches.AddRangeAsync(match1, match2, match3);
            await _context.SaveChangesAsync();

            var response = await _controller.UnderReview(new MatchParameters());
            Assert.Equal(200, response.StatusCode);

            var okResult = response as OkObjectResult;
            var result = okResult?.Value;
            var matchDtos = Assert.IsType<List<MatchDto>>(result);
            Assert.Equal(2, matchDtos.Count);
        }


        [Fact]
        public async void ItReturnsErrorIfPlayerIsTwiceInTeam()
        {
            var player1 = new Player("test1", "test1@test.com");
            var player2 = new Player("test2", "test2@test.com");

            await _context.Players.AddAsync(_currentPlayer);
            await _context.Players.AddAsync(player1);
            await _context.Players.AddAsync(player2);

            await _context.SaveChangesAsync();

            var createMatchDto = new CreateMatchDto()
            {
                PlayerPosition = Position.Attacker,
                AllyId = player1.Id,

                OpponentAttackerId = player2.Id,
                OpponentDefenderId = player1.Id
            };

            var response = await _controller.Post(createMatchDto);
            Assert.Equal(422, response.StatusCode);

            Assert.Equal(0, _context.Matches.Count());
            Assert.Equal(0, _context.Teams.Count());
        }
    }
}
