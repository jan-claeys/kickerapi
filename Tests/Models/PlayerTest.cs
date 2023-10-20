using ClassLibrary.Models;

namespace Tests.Models
{
    public class PlayerTest
    {
        [Fact]
        public void ItCalculatesRating()
        {
            Player player = new Player("test");
            player.AttackRating = 10;
            player.DeffendRating = 20;

            Assert.Equal(15, player.Rating());
        }
    }
}
