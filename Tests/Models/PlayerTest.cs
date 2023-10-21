using ClassLibrary.Models;

namespace Tests.Models
{
    public class PlayerTest
    {
        [Fact]
        public void ItCalculatesRating()
        {
            Player player = new Player("test");
            player.SetAttackRating(10);
            player.SetDefendRating(20);

            Assert.Equal(10, player.AttackRating);
            Assert.Equal(20, player.DefendRating);
            Assert.Equal(15, player.Rating);
        }

        [Fact]
        public void ItCalculatesHigherRatingByWin()
        {
            Random random = new Random();
            foreach(var i in Enumerable.Range(0, 1000))
            {
                Assert.True(Player.CalcualteRating(i, random.NextDouble(), random.NextDouble(), true) >= i);
            }
        }

        [Fact]
        public void ItCalculatesLowerRatingByLose()
        {
            Random random = new Random();
            foreach (var i in Enumerable.Range(0, 1000))
            {
                Assert.True(Player.CalcualteRating(i, random.NextDouble(), random.NextDouble(), false) <= i);
            }
        }

        [Fact]
        public void ItRaisesRatingByWin()
        {
            Player player = new Player("test");
            player.SetAttackRating(10);
            player.SetDefendRating(20);

            player.SetAttackRating(1, 0.5, true);

            Assert.True(player.AttackRating > 10 );
            Assert.True(player.DefendRating == 20);
            Assert.True(player.Rating > 15);
        }

        [Fact]
        public void ItDropsRatingByLose()
        {
            Player player = new Player("test");
            player.SetAttackRating(10);
            player.SetDefendRating(20);

            player.SetAttackRating(0, 0.5, false);

            Assert.True(player.AttackRating < 10);
            Assert.True(player.DefendRating == 20);
            Assert.True(player.Rating < 15);
        }
    }
}
