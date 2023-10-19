using ClassLibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests.Models
{
    public class MatchTest
    {
        [Fact]
        public void ItSetsCollors()
        {
            Team team1 = new Team(new Player("test1"), new Player("test2"));
            Team team2 = new Team(new Player("test3"), new Player("test4"));

            Match match = new Match(team1, team2);

            Assert.Equal(Color.Black, match.Team1.Color);
            Assert.Equal(Color.Green, match.Team2.Color);

        }
    }
}
