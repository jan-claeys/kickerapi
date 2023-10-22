
using ClassLibrary.Models;

namespace kickerapi.Services
{
    public class MatchService : IMatchService
    {
        private readonly KickerContext _context;

        public MatchService(KickerContext context)
        {
            _context = context;
        }

        //returns all matches for a player (confirmed and unconfirmed) ordered by date ascending
        public IQueryable<Match> GetMatches(Player player, bool isConfirmed)
        {
            return _context.Matches
                .Where(x => x.Team1.Attacker.Id == player.Id || x.Team1.Defender.Id == player.Id || x.Team2.Attacker.Id == player.Id || x.Team2.Defender.Id == player.Id)
                .WhereIf(x => x.Team1.IsConfirmed && x.Team2.IsConfirmed && x.IsCalculatedInRating, isConfirmed)
                .WhereIf(x => !x.Team1.IsConfirmed || !x.Team2.IsConfirmed || !x.IsCalculatedInRating, !isConfirmed)
                .OrderBy(x => x.Date);
        }
    }
}
