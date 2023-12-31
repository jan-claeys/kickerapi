﻿using ClassLibrary.Models;

namespace Tests.Models
{
    public class TeamTest
    {
        [Fact]
        public void ItCalculatesRating()
        {
            var player1 = new Player("test1", "test1@test.com");
            var player2 = new Player("test2", "test2@test.com");
            player1.SetAttackRating(1500);
            player2.SetDefendRating(2001);

            var team = new Team(player1, player2, 0);

            Assert.Equal(1750, team.Rating());
        }

        [Fact]
        public void ItRisesRatingByWin()
        {
            var player1 = new Player("test1", "test1@test.com");
            var player2 = new Player("test2", "test2@test.com");

            var oldRatingPlayer1 = 1500;
            var oldRatingPlayer2 = 2001;

            player1.SetAttackRating(oldRatingPlayer1);
            player2.SetDefendRating(oldRatingPlayer2);

            var team = new Team(player1, player2, 0);
            var oldRating = team.Rating();

            team.UpdateRatings(1, 0.5, true);

            Assert.True(team.AttackerRatingChange >= 0 && team.AttackerRatingChange <= 32) ;
            Assert.True(team.DefenderRatingChange >= 0 && team.DefenderRatingChange <= 32);

            Assert.True(oldRatingPlayer1 <= player1.AttackRating);
            Assert.True(oldRatingPlayer2 <= player2.DefendRating);
            Assert.True(oldRating <= team.Rating());
        }

        [Fact]
        public void ItDropsRatingByLose()
        {
            var player1 = new Player("test1", "test1@test.com");
            var player2 = new Player("test2", "test2@test.com");

            var oldRatingPlayer1 = 1500;
            var oldRatingPlayer2 = 2001;

            player1.SetAttackRating(oldRatingPlayer1);
            player2.SetDefendRating(oldRatingPlayer2);

            var team = new Team(player1, player2, 0);
            var oldRating = team.Rating();

            team.UpdateRatings(0, 0.5, false);

            Assert.True(team.AttackerRatingChange <= 0 && team.AttackerRatingChange >= -32);
            Assert.True(team.DefenderRatingChange <= 0 && team.DefenderRatingChange >= -32);

            Assert.True(oldRatingPlayer1 >= player1.AttackRating);
            Assert.True(oldRatingPlayer2 >= player2.DefendRating);
            Assert.True(oldRating >= team.Rating());
        }

        [Fact]
        public void ItDoesNotChangeOtherRating()
        {
            var player1 = new Player("test1", "test1@test.com");
            var player2 = new Player("test2", "test2@test.com");

            var oldRatingPlayer1 = 1500;
            var oldRatingPlayer2 = 2001;

            player1.SetDefendRating(oldRatingPlayer1);
            player2.SetAttackRating(oldRatingPlayer2);

            var team = new Team(player1, player2, 0);
            var oldRating = team.Rating();

            team.UpdateRatings(1, 0.5, true);

            Assert.Equal(oldRatingPlayer1, player1.DefendRating);
            Assert.Equal(oldRatingPlayer2, player2.AttackRating);
            Assert.True(oldRating <= team.Rating());
        }
    }
}
