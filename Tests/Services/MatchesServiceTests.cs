using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using kickerapi.Dtos;
using kickerapi.Services;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

using ClassLibrary.Models;
using kickerapi;
using Match = ClassLibrary.Models.Match;
using Moq.EntityFrameworkCore;

namespace Tests.Services
{
    public class MatchesServiceTests
    {
        private readonly Mock<KickerContext> _contextMock;
        private readonly MatchesService _matchesService;

        public MatchesServiceTests()
        {
            _contextMock = new Mock<KickerContext>();
            _matchesService = new MatchesService(_contextMock.Object);
        }

        [Fact]
        public void ItGetsMatchesFromPlayerNotConfirmed()
        {
            var player1 = new Player("test1", "test1@test.com");
            var player2 = new Player("test2", "test2@test.com");
            var player3 = new Player("test3", "test3@test.com");
            var player4 = new Player("test4", "test4@test.com");
            var player5 = new Player("test5", "test5@test.com");

            var team1 = new Team(player1, player2, 5);
            var team2 = new Team(player3, player4, 2);

            var match1 = new Match(team1, team2);

            var team3 = new Team(player1, player2, 5);
            var team4 = new Team(player3, player4, 2);
            team3.Confirm();
            team4.Confirm();

            var match2 = new Match(team3, team4);
            match2.UpdateRatings();
            
            var team5 = new Team(player1, player2, 5);
            var team6 = new Team(player3, player4, 2);

            var match3 = new Match(team5, team6);

            var team7 = new Team(player5, player2, 5);
            var team8 = new Team(player3, player4, 2);

            var match4 = new Match(team7, team8);

            var matches = new List<Match> { match1, match2, match3, match4};

            _contextMock.Setup(x => x.Matches).ReturnsDbSet(matches);

            var result = _matchesService.GetMatches(player1, false).ToList();
            Assert.Equal(2, result.Count);
        }

        [Fact]
        public void ItGetsMatchesFromPlayerConfirmed()
        {

            var player1 = new Player("test1", "test1@test.com");
            var player2 = new Player("test2", "test2@test.com");
            var player3 = new Player("test3", "test3@test.com");
            var player4 = new Player("test4", "test4@test.com");
            var player5 = new Player("test5", "test5@test.com");

            var team1 = new Team(player1, player2, 5);
            var team2 = new Team(player3, player4, 2);
            team1.Confirm();
            team2.Confirm();

            var match1 = new Match(team1, team2);
            match1.UpdateRatings();

            var team3 = new Team(player1, player2, 5);
            var team4 = new Team(player3, player4, 2);

            var match2 = new Match(team3, team4);

            var team5 = new Team(player1, player2, 5);
            var team6 = new Team(player3, player4, 2);
            team5.Confirm();
            team6.Confirm();

            var match3 = new Match(team5, team6);
            match3.UpdateRatings();

            var team7 = new Team(player5, player2, 5);
            var team8 = new Team(player3, player4, 2);
            team7.Confirm();
            team8.Confirm();

            var match4 = new Match(team7, team8);
            match4.UpdateRatings();

            var matches = new List<Match> { match1, match2, match3, match4 };

            _contextMock.Setup(x => x.Matches).ReturnsDbSet(matches);

            var result = _matchesService.GetMatches(player1, true).ToList();
            Assert.Equal(2, result.Count);
        }

        [Fact]
        public void ItGetsMatchesFromPlayerAttacker()
        {
            var player1 = new Player("test1", "test1@test.com");
            var player2 = new Player("test2", "test2@test.com");
            var player3 = new Player("test3", "test3@test.com");
            var player4 = new Player("test4", "test4@test.com");

            var team1 = new Team(player1, player2, 5);
            var team2 = new Team(player3, player4, 2);

            var match1 = new Match(team1, team2);

            var team3 = new Team(player2, player1, 5);
            var team4 = new Team(player3, player4, 2);

            var match2 = new Match(team3, team4);

            var team5 = new Team(player1, player2, 5);
            var team6 = new Team(player3, player4, 2);

            var match3 = new Match(team5, team6);


            var matches = new List<Match> { match1, match2, match3 };

            _contextMock.Setup(x => x.Matches).ReturnsDbSet(matches);

            var result = _matchesService.GetMatches(player1, false, Position.Attacker).ToList();
            Assert.Equal(2, result.Count);
        }

        [Fact]
        public void ItGetsMatchesFromPlayerDeffender()
        {
            var player1 = new Player("test1", "test1@test.com");
            var player2 = new Player("test2", "test2@test.com");
            var player3 = new Player("test3", "test3@test.com");
            var player4 = new Player("test4", "test4@test.com");

            var team1 = new Team(player1, player2, 5);
            var team2 = new Team(player3, player4, 2);

            var match1 = new Match(team1, team2);

            var team3 = new Team(player2, player1, 5);
            var team4 = new Team(player3, player4, 2);

            var match2 = new Match(team3, team4);

            var team5 = new Team(player1, player2, 5);
            var team6 = new Team(player3, player4, 2);

            var match3 = new Match(team5, team6);

            var matches = new List<Match> { match1, match2, match3 };

            _contextMock.Setup(x => x.Matches).ReturnsDbSet(matches);

            var result = _matchesService.GetMatches(player1, false, Position.Defender).ToList();
            Assert.Single(result);
        }
    }
}
