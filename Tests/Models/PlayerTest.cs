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
    }
}
