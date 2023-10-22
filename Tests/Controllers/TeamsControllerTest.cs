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
    public class TeamsControllerTest : DbTest
    {
        private readonly TeamsController _controller;
        private readonly Player _currentPlayer;

        public TeamsControllerTest(KickerContext context) : base(context)
        {
            _currentPlayer = new Player("test");
            var securityServiceMock = new Mock<ISecurityService>();
            securityServiceMock.Setup(x => x.GetUserAsync(It.IsAny<ClaimsPrincipal>()).Result).Returns(_currentPlayer);

            _controller = new TeamsController(context, securityServiceMock.Object, new MatchService(context));
        }

        [Fact]
        public async void ItConfirmsTeam()
        {
            _currentPlayer.Id = 1;
            var player1 = new Player("test1") { Id = 2 };

            var team1 = new Team(_currentPlayer, player1, 0);

            _context.Teams.Add(team1);
            _context.SaveChanges();

            Assert.False(team1.IsConfirmed);

            var result = await _controller.Confirm(team1.Id);
            Assert.Equal(200, result.StatusCode);

            Assert.True(team1.IsConfirmed);
        }

        [Fact]
        public async void ItThrowsErrorIfTeamNotExistConfirme()
        {
            var result = await _controller.Confirm(1);
            Assert.Equal(400, result.StatusCode);
        }

        [Fact]
        public async void ItThrowsErrorIfPlayerNotInTeamConfirme()
        {
            var player1 = new Player("test1") { Id = 1 };
            var player2 = new Player("test2") { Id = 2 };

            var team1 = new Team(player1, player2, 0);

            _context.Teams.Add(team1);
            _context.SaveChanges();

            var result = await _controller.Confirm(team1.Id);
            Assert.Equal(400, result.StatusCode);
        }


        [Fact]
        public async void ItUpdateAllRatingsMatchesConfirmed()
        {
            _currentPlayer.Id = 1;
            var player1 = new Player("test1") { Id = 2 };
            var player2 = new Player("test2") { Id = 3 };
            var player3 = new Player("test3") { Id = 4 };

            var team1 = new Team(_currentPlayer, player1, 0);
            var team2 = new Team(player2, player3, 0);
            var team3 = new Team(_currentPlayer, player1, 0);
            var team4 = new Team(player2, player3, 0);
            var team5 = new Team(_currentPlayer, player1, 0);
            var team6 = new Team(player2, player3, 0);

            var match1 = new Match(team1, team2);
            var match2 = new Match(team3, team4);
            var match3 = new Match(team5, team6);

            _context.Matches.Add(match1);
            _context.Matches.Add(match2);
            _context.Matches.Add(match3);

            team1.Confirm();
            team2.Confirm();
            team3.Confirm();
            team4.Confirm();
            team5.Confirm();

            _context.SaveChanges();

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
            _currentPlayer.Id = 1;
            var player1 = new Player("test1") { Id = 2 };
            var player2 = new Player("test2") { Id = 3 };
            var player3 = new Player("test3") { Id = 4 };

            var team1 = new Team(_currentPlayer, player1, 0);
            var team2 = new Team(player2, player3, 0);
            var team3 = new Team(_currentPlayer, player1, 0);
            var team4 = new Team(player2, player3, 0);

            var match1 = new Match(team1, team2);
            var match2 = new Match(team3, team4);

            _context.Add(match1);
            _context.Add(match2);
            _context.SaveChanges();

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
            Assert.Equal(400, result.StatusCode);
        }

        [Fact]
        public async void ItThrowsErrorIfPlayerNotInTeamDeny()
        {
            var player1 = new Player("test1") { Id = 1 };
            var player2 = new Player("test2") { Id = 2 };

            var team1 = new Team(player1, player2, 0);

            _context.Teams.Add(team1);
            _context.SaveChanges();

            var result = await _controller.Deny(team1.Id);
            Assert.Equal(400, result.StatusCode);
        }
    }
}
