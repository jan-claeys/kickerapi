using ClassLibrary.Models;
using kickerapi.Dtos;

namespace kickerapi.Services
{
    public interface IPlayersService: IServiceContext
    {
        public Task<Player> GetPlayer(string playerId);
        public IQueryable<Player> GetPlayers(string? search);
        public IQueryable<Player> GetPlayersRanking(Position? orderBy);
    }
}
