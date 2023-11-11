using ClassLibrary.Models;
using kickerapi.Dtos;

namespace kickerapi.Services
{
    public interface IMatchesService : IServiceContext
    {
        public IQueryable<Match> GetMatches(Player player, bool isConfirmed, Position? playerPosition = null);
        public IQueryable<Match> GetMatchesWithPlayers(Player player, bool isConfirmed, Position? playerPosition = null);
        public Task<Match> GetMatchWithTeams(Team team);
        public void AddMatch(Match match);
        public void RemoveMatch(Match match);
    }
}
