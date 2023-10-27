using ClassLibrary.Models;

namespace kickerapi.Services
{
    public class PlayersService : ContextService,  IPlayersService
    {
        public PlayersService(KickerContext context) : base(context)
        {
        }

        public IQueryable<Player> GetPlayer(string playerId)
        {
            return _context.Players.Where(x => x.Id == playerId);
        }
    }
}
