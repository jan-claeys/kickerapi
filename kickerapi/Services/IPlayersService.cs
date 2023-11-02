using ClassLibrary.Models;

namespace kickerapi.Services
{
    public interface IPlayersService: IServiceContext
    {
        public IQueryable<Player> GetPlayer(string playerId);
        public IQueryable<Player> GetPlayers(string? search);
        public IQueryable<Player> GetPlayersRanking(string? orderBy);
        public int GetPlayersCount();
    }
}
