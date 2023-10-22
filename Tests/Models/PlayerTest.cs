using ClassLibrary.Models;
using Xunit.Abstractions;

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
        public void ItCalculatesPositiveRatingChangeByWin()
        {
            Random random = new Random();
            foreach(var i in Enumerable.Range(0, 1000))
            {
                var ratingChange = Player.CalcualteRatingChange(random.NextDouble(), random.NextDouble(), true);
                Assert.True(ratingChange >= 0 && ratingChange <= 32);
            }
        }

        [Fact]
        public void ItCalculatesNegativeRatingChangeByLose()
        {
            Random random = new Random();
            foreach(int i in Enumerable.Range(0, 1000)) {
                var ratingChange = Player.CalcualteRatingChange(random.NextDouble(), random.NextDouble(), false);
                Assert.True(ratingChange <= 0 && ratingChange >= -32);
                Console.WriteLine(ratingChange);
            }
        }

        [Fact]
        public void ItRaisesRatingByWin()
        {
            Player player = new Player("test");
            player.SetAttackRating(10);
            player.SetDefendRating(20);

            var ratingChange = player.UpdateAttackRating(1, 0.5, true);

            Assert.True(ratingChange >= 0 && ratingChange <= 32);
            Assert.True(player.AttackRating > 10 );
            Assert.True(player.DefendRating == 20);
            Assert.True(player.Rating >= 15);
        }

        [Fact]
        public void ItDropsRatingByLose()
        {
            Player player = new Player("test");
            player.SetAttackRating(10);
            player.SetDefendRating(20);

            var ratingChange = player.UpdateAttackRating(0, 0.5, false);
            Assert.True(ratingChange <= 0 && ratingChange >= -32);

            Assert.True(player.AttackRating <= 10);
            Assert.True(player.DefendRating == 20);
            Assert.True(player.Rating <= 15);
        }
    }
}
