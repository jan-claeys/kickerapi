using ClassLibrary.Models;
using kickerapi;
using kickerapi.Controllers;
using kickerapi.Services;
using Microsoft.EntityFrameworkCore;
using Moq;
using System.Security.Claims;
using Match = ClassLibrary.Models.Match;

namespace Tests.Controllers
{
    public class TeamsControllerTest : DatabaseTest
    {
        private readonly TeamsController _controller;
        private readonly Player _currentPlayer;

        public TeamsControllerTest(KickerContext context) : base(context)
        {
            _currentPlayer = new Player("test");
            var securityServiceMock = new Mock<ISecurityService>();
            securityServiceMock.Setup(x => x.GetUserAsync(It.IsAny<ClaimsPrincipal>()).Result).Returns(_currentPlayer);

            _controller = new TeamsController(securityServiceMock.Object, new MatchesService(context), new TeamsService(context));
        }

        [Fact]
        public async void ItConfirmsTeam()
        {
            var player1 = new Player("test1");

            var team1 = new Team(_currentPlayer, player1, 0);

            await _context.Teams.AddAsync(team1);
            await _context.SaveChangesAsync();

            Assert.False(team1.IsConfirmed);

            var result = await _controller.Confirm(team1.Id);
            Assert.Equal(200, result.StatusCode);

            Assert.True(team1.IsConfirmed);
        }

        [Fact]
        public async void ItThrowsErrorIfTeamNotExistConfirm()
        {
            var result = await _controller.Confirm(1);
            Assert.Equal(404, result.StatusCode);
        }

        [Fact]
        public async void ItThrowsErrorIfPlayerNotInTeamConfirm()
        {
            var player1 = new Player("test1");
            var player2 = new Player("test2");

            var team1 = new Team(player1, player2, 0);

            await _context.Teams.AddAsync(team1);
            await _context.SaveChangesAsync();

            var result = await _controller.Confirm(team1.Id);
            Assert.Equal(403, result.StatusCode);
        }


        [Fact]
        public async void ItUpdateAllRatingsMatchesConfirmed()
        {
            var player1 = new Player("test1");
            var player2 = new Player("test2");
            var player3 = new Player("test3");

            var team1 = new Team(_currentPlayer, player1, 0);
            var team2 = new Team(player2, player3, 0);
            var team3 = new Team(_currentPlayer, player1, 0);
            var team4 = new Team(player2, player3, 0);
            var team5 = new Team(_currentPlayer, player1, 0);
            var team6 = new Team(player2, player3, 0);

            var match1 = new Match(team1, team2);
            var match2 = new Match(team3, team4);
            var match3 = new Match(team5, team6);

            await _context.Matches.AddAsync(match1);
            await _context.Matches.AddAsync(match2);
            await _context.Matches.AddAsync(match3);

            team1.Confirm();
            team2.Confirm();
            team3.Confirm();
            team4.Confirm();
            team5.Confirm();

            await _context.SaveChangesAsync();

            Assert.Null(team1.AttackerRatingChange);
            Assert.Null(team1.DefenderRatingChange);
            Assert.Null(team2.AttackerRatingChange);
            Assert.Null(team2.DefenderRatingChange);
            Assert.Null(team3.AttackerRatingChange);
            Assert.Null(team3.DefenderRatingChange);
            Assert.Null(team4.AttackerRatingChange);
            Assert.Null(team4.DefenderRatingChange);
            Assert.Null(team5.AttackerRatingChange);
            Assert.Null(team5.DefenderRatingChange);
            Assert.Null(team6.AttackerRatingChange);
            Assert.Null(team6.DefenderRatingChange);

            Assert.False(match1.IsCalculatedInRating);
            Assert.False(match2.IsCalculatedInRating);
            Assert.False(match3.IsCalculatedInRating);

            var result = await _controller.Confirm(team1.Id);

            Assert.Equal(200, result.StatusCode);

            Assert.NotNull(team1.AttackerRatingChange);
            Assert.NotNull(team1.DefenderRatingChange);
            Assert.NotNull(team2.AttackerRatingChange);
            Assert.NotNull(team2.DefenderRatingChange);
            Assert.NotNull(team3.AttackerRatingChange);
            Assert.NotNull(team3.DefenderRatingChange);
            Assert.NotNull(team4.AttackerRatingChange);
            Assert.NotNull(team4.DefenderRatingChange);
            Assert.Null(team5.AttackerRatingChange);
            Assert.Null(team5.DefenderRatingChange);
            Assert.Null(team6.AttackerRatingChange);
            Assert.Null(team6.DefenderRatingChange);

            Assert.True(match1.IsCalculatedInRating);
            Assert.True(match2.IsCalculatedInRating);
            Assert.False(match3.IsCalculatedInRating);
        }

        [Fact]
        public async void ItDeleteTeamsAndMatchIfDeny()
        {
            var player1 = new Player("test1");
            var player2 = new Player("test2");
            var player3 = new Player("test3");

            var team1 = new Team(_currentPlayer, player1, 0);
            var team2 = new Team(player2, player3, 0);
            var team3 = new Team(_currentPlayer, player1, 0);
            var team4 = new Team(player2, player3, 0);

            var match1 = new Match(team1, team2);
            var match2 = new Match(team3, team4);

            await _context.AddAsync(match1);
            await _context.AddAsync(match2);
            await _context.SaveChangesAsync();

            Assert.Equal(4, await _context.Players.CountAsync());
            Assert.Equal(4, await _context.Teams.CountAsync());
            Assert.Equal(2, await _context.Matches.CountAsync());

            var result = await _controller.Deny(team1.Id);
            Assert.Equal(200, result.StatusCode);

            Assert.Equal(4, await _context.Players.CountAsync());
            Assert.Equal(2, await _context.Teams.CountAsync());
            Assert.Equal(1, await _context.Matches.CountAsync());
        }

        [Fact]
        public async void ItThrowsErrorIfTeamNotExistDeny()
        {
            var result = await _controller.Deny(1);
            Assert.Equal(404, result.StatusCode);
        }

        [Fact]
        public async void ItThrowsErrorIfPlayerNotInTeamDeny()
        {
            var player1 = new Player("test1");   
            var player2 = new Player("test2");

            var team1 = new Team(player1, player2, 0);

            await _context.Teams.AddAsync(team1);
            await _context.SaveChangesAsync();

            var result = await _controller.Deny(team1.Id);
            Assert.Equal(403, result.StatusCode);
        }
    }
}
