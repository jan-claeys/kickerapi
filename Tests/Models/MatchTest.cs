using ClassLibrary.Models;

namespace Tests.Models
{
    public class MatchTest
    {
        [Fact]
        public void ItSetsDate()
        {
            var team1 = new Team(new Player("test1") { Id = 1}, new Player("test2") { Id = 2}, 0);
            var team2 = new Team(new Player("test3") { Id = 3}, new Player("test4") { Id = 4}, 0);

            var match = new Match(team1, team2);

            Assert.Equal(DateTime.Now.Date, match.Date.Date);
        }

        [Fact]
        public void ItThrowsExeptionPlayersNotUnique()
        {
            var team1 = new Team(new Player("test1") { Id = 1}, new Player("test2") { Id = 2}, 1);
            var team2 = new Team(new Player("test1") { Id = 2}, new Player("test4") { Id = 4}, 3);
            Assert.Throws<Exception>(() => new Match(team1, team2));

            team1 = new Team(new Player("test1") { Id = 1 }, new Player("test2") { Id = 1 }, 1);
            team2 = new Team(new Player("test1") { Id = 2 }, new Player("test4") { Id = 4 }, 3);
            Assert.Throws<Exception>(() => new Match(team1, team2));
        }
    }
}
