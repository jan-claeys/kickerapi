using ClassLibrary.Models;

namespace kickerapi.Services
{
    public interface IPlayersService: IServiceContext
    {
        public IQueryable<Player> GetPlayer(string playerId);
    }
}
