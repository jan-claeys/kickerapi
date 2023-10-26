using ClassLibrary.Models;

namespace kickerapi.Services
{
    public interface IMatchService
    {
        public IQueryable<Match> GetMatches(Player player, bool isConfirmed);
        public IQueryable<Match> GetMatchesWithPlayers(Player player, bool isConfirmed);
    }
}
