using ClassLibrary.Models;

namespace kickerapi.Services
{
    public interface IMatchesService : IServiceContext
    {
        public IQueryable<Match> GetMatches(Player player, bool isConfirmed);
        public IQueryable<Match> GetMatchesWithPlayers(Player player, bool isConfirmed);
        public IQueryable<Match> GetMatchWithTeams(Team team);
        public void RemoveMatch(Match match);
    }
}
