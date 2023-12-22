using ClassLibrary.Exceptions;
using ClassLibrary.Models;

namespace Tests.Models
{
    public class MatchTest
    {
        [Fact]
        public void ItSetsDate()
        {
            var team1 = new Team(new Player("test1", "test1@test.com"), new Player("test2", "test2@test.com"), 0);
            var team2 = new Team(new Player("test3", "test3@test.com"), new Player("test4", "test4@test.com"), 0);

            var match = new Match(team1, team2);

            Assert.Equal(DateTime.Now.Date, match.Date.Date);
        }

        [Fact]
        public void ItThrowsExeptionPlayersNotUnique()
        {
            var team1 = new Team(new Player("test1", "test1@test.com") { Id = "1"}, new Player("test2", "test2@test.com") { Id = "2"}, 1);
            var team2 = new Team(new Player("test1", "test1@test.com") { Id = "2"}, new Player("test4", "test4@test.com") { Id = "4"}, 3);
            Assert.Throws<DuplicatePlayerException>(() => new Match(team1, team2));

            team1 = new Team(new Player("test1", "test1@test.com") { Id = "1" }, new Player("test2", "test2@test.com") { Id = "1" }, 1);
            team2 = new Team(new Player("test1", "test1@test.com") { Id = "2" }, new Player("test4", "test4@test.com") { Id = "4" }, 3);
            Assert.Throws<DuplicatePlayerException>(() => new Match(team1, team2));
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

            var team1 = new Team(player1, player2, 11);
            var team2 = new Team(new Player("test3", "test3@test.com"), new Player("test4", "test4@test.com"), 5);

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
            var player1 = new Player("test1", "test1@test.com");
            var player2 = new Player("test2", "test2@test.com");

            var oldRatingPlayer1 = 3000;
            var oldRatingPlayer2 = 3000;

            player1.SetAttackRating(oldRatingPlayer1);
            player2.SetDefendRating(oldRatingPlayer2);

            var team1 = new Team(player1, player2, 5);
            var team2 = new Team(new Player("test3", "test3@test.com"), new Player("test4", "test4@test.com"), 11);
            team1.Confirm();
            team2.Confirm();

            var oldRatingTeam1 = team1.Rating();

            var match1 = new Match(team1, team2);
            match1.UpdateRatings();

            Assert.True(oldRatingPlayer1 > player1.AttackRating);
            Assert.True(oldRatingPlayer2 > player2.DefendRating);
            Assert.True(oldRatingTeam1 > team1.Rating());

           
        }
        [Fact]
        public void ItRisesRatingMoreByBigWin()
        {
            Team team1 = new Team(new Player("test1", "test1@test.com"), new Player("test2", "test2@test.com"), 5);
            Team team2 = new Team(new Player("test3", "test3@test.com"), new Player("test4", "test4@test.com"), 11);
            team1.Confirm();
            team2.Confirm();

            var match1 = new Match(team1, team2);
            match1.UpdateRatings();

            Team team3 = new Team(new Player("test5", "test5@test.com"), new Player("test6", "test6@test.com"), 2);
            Team team4 = new Team(new Player("test7", "test7@test.com"), new Player("test8", "test8@test.com"), 11);
            team3.Confirm();
            team4.Confirm();

            var match2 = new Match(team3, team4);
            match2.UpdateRatings();

            Assert.True(team2.DefenderRatingChange < team4.DefenderRatingChange);
            Assert.True(team2.AttackerRatingChange < team4.AttackerRatingChange);
        }


        [Fact]
        public void ItDropsRatingMoreByBigLose()
        {
            Team team1 = new Team(new Player("test1", "test1@test.com"), new Player("test2", "test2@test.com"), 5);
            Team team2 = new Team(new Player("test3", "test3@test.com"), new Player("test4", "test4@test.com"), 11);
            team1.Confirm();
            team2.Confirm();
            
            var match1 = new Match(team1, team2);
            match1.UpdateRatings();

            Team team3 = new Team(new Player("test5", "test5@test.com"), new Player("test6", "test6@test.com"), 2);
            Team team4 = new Team(new Player("test7", "test7@test.com"), new Player("test8", "test8@test.com"), 11);
            team3.Confirm();
            team4.Confirm();

            var match2 = new Match(team3, team4);
            match2.UpdateRatings();

            Assert.True(team3.DefenderRatingChange < team1.DefenderRatingChange);
            Assert.True(team3.AttackerRatingChange < team1.AttackerRatingChange);
        }

        [Fact]
        public void ItDoesNotChangeRatingIfDraw()
        {
            var player1 = new Player("test1", "test1@test.com");
            var player2 = new Player("test2", "test2@test.com");

            var oldRatingPlayer1 = 3000;
            var oldRatingPlayer2 = 3000;

            player1.SetAttackRating(oldRatingPlayer1);
            player2.SetDefendRating(oldRatingPlayer2);

            var team1 = new Team(player1, player2, 0);
            var team2 = new Team(new Player("test3", "test3@test.com"), new Player("test4", "test4@test.com"), 0);
            team1.Confirm();
            team2.Confirm();
            var match1 = new Match(team1, team2);
            match1.UpdateRatings();

            var team3 = new Team(player1, player2, 5);
            var team4 = new Team(new Player("test3", "test3@test.com"), new Player("test4", "test4@test.com"), 5);
            team3.Confirm();
            team4.Confirm();
            var match2 = new Match(team3, team4);
            match2.UpdateRatings();

            Assert.Equal(oldRatingPlayer1, player1.AttackRating);
            Assert.Equal(oldRatingPlayer2, player2.DefendRating);
            Assert.Equal(oldRatingPlayer1, player1.AttackRating);
            Assert.Equal(oldRatingPlayer2, player2.DefendRating);
        }

        [Fact]
        public void ItDoesNotUpdateRatingIfMatchIsNotConfirmed()
        {
            var team1 = new Team(new Player("test1", "test1@test.com"), new Player("test2", "test2@test.com"), 11);
            var team2 = new Team(new Player("test3", "test3@test.com"), new Player("test4", "test4@test.com"), 5);

            var match = new Match(team1, team2);
            Assert.False(match.UpdateRatings());
        }

        [Fact]
        public void ItDoesNotUpdateRatingIfRatingAlreadyUpdated()
        {
            var team1 = new Team(new Player("test1", "test1@test.com"), new Player("test2", "test2@test.com"), 11);
            var team2 = new Team(new Player("test3", "test3@test.com"), new Player("test4", "test4@test.com"), 5);

            team1.Confirm();
            team2.Confirm();

            var match = new Match(team1, team2);

            Assert.True(match.UpdateRatings());
            Assert.False(match.UpdateRatings());
        }
    }
}
