using ClassLibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests.Models
{
    public class PlayerTest
    {
        [Fact]
        public void ItCalculatesRating()
        {
            Player player = new Player("test", "test");
            player.AttackRating = 10;
            player.DeffendRating = 20;

            Assert.Equal(15, player.Rating());
        }
    }
}
