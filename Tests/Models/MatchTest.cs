﻿using ClassLibrary.Models;

namespace Tests.Models
{
    public class MatchTest
    {
        [Fact]
        public void ItSetsDate()
        {
            var team1 = new Team(new Player("test1"), new Player("test2"), 0);
            var team2 = new Team(new Player("test3"), new Player("test4"), 0);

            var match = new Match(team1, team2);

            Assert.Equal(DateTime.Now.Date, match.Date.Date);
        }

        [Fact]
        public void ItThrowsExeptionPlayersNotUnique()
        {
            var team1 = new Team(new Player("test1") { Id = "1"}, new Player("test2") { Id = "2"}, 1);
            var team2 = new Team(new Player("test1") { Id = "2"}, new Player("test4") { Id = "4"}, 3);
            Assert.Throws<Exception>(() => new Match(team1, team2));

            team1 = new Team(new Player("test1") { Id = "1" }, new Player("test2") { Id = "1" }, 1);
            team2 = new Team(new Player("test1") { Id = "2" }, new Player("test4") { Id = "4" }, 3);
            Assert.Throws<Exception>(() => new Match(team1, team2));
        }

        [Fact]
        public void ItRisesRatingByWin()
        {
            var player1 = new Player("test1");
            var player2 = new Player("test2");

            var oldRatingPlayer1 = 1500;
            var oldRatingPlayer2 = 2001;

            player1.SetAttackRating(oldRatingPlayer1);
            player2.SetDefendRating(oldRatingPlayer2);

            var team1 = new Team(player1, player2, 11);
            var team2 = new Team(new Player("test3"), new Player("test4"), 5);

            team1.Confirm();
            team2.Confirm();

            var oldRatingTeam1 = team1.Rating();

            var match = new Match(team1, team2);
            match.UpdateRatings();

            Assert.True(oldRatingPlayer1 < player1.AttackRating);
            Assert.True(oldRatingPlayer2 < player2.DefendRating);
            Assert.True(oldRatingTeam1 < team1.Rating());
        }

        [Fact]
        public void ItDropsRatingByLose()
        {
            var player1 = new Player("test1");
            var player2 = new Player("test2");

            var oldRatingPlayer1 = 3000;
            var oldRatingPlayer2 = 3000;

            player1.SetAttackRating(oldRatingPlayer1);
            player2.SetDefendRating(oldRatingPlayer2);

            var team1 = new Team(player1, player2, 5);
            var team2 = new Team(new Player("test3"), new Player("test4"), 11);
            team1.Confirm();
            team2.Confirm();

            var oldRatingTeam1 = team1.Rating();

            var match = new Match(team1, team2);
            match.UpdateRatings();

            Assert.True(oldRatingPlayer1 > player1.AttackRating);
            Assert.True(oldRatingPlayer2 > player2.DefendRating);
            Assert.True(oldRatingTeam1 > team1.Rating());
        }

        [Fact]
        public void ItDoesNotUpdateRatingIfMatchIsNotConfirmed()
        {
            var team1 = new Team(new Player("test1"), new Player("test2"), 11);
            var team2 = new Team(new Player("test3"), new Player("test4"), 5);

            var match = new Match(team1, team2);
            Assert.False(match.UpdateRatings());
        }

        [Fact]
        public void ItDoesNotUpdateRatingIfRatingAlreadyUpdated()
        {
            var team1 = new Team(new Player("test1"), new Player("test2"), 11);
            var team2 = new Team(new Player("test3"), new Player("test4"), 5);

            team1.Confirm();
            team2.Confirm();

            var match = new Match(team1, team2);

            Assert.True(match.UpdateRatings());
            Assert.False(match.UpdateRatings());
        }
    }
}
