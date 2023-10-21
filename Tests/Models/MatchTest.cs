using ClassLibrary.Models;

namespace Tests.Models
{
    public class MatchTest
    {
        [Fact]
        public void ItSetsCollors()
        {
            var team1 = new Team(new Player("test1"), new Player("test2"), 0);
            var team2 = new Team(new Player("test3"), new Player("test4"), 0);

            var match = new Match(team1, team2);

            Assert.Equal(Color.Black, match.Team1.Color);
            Assert.Equal(Color.Green, match.Team2.Color);
        }


        [Fact]
        public void ItSetsDate()
        {
            var team1 = new Team(new Player("test1"), new Player("test2"), 0);
            var team2 = new Team(new Player("test3"), new Player("test4"), 0);

            var match = new Match(team1, team2);

            Assert.Equal(DateTime.Now.Date, match.Date.Date);
        }
    }
}
