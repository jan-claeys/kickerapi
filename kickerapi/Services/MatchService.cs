
using ClassLibrary.Models;
using kickerapi.QueryParameters;
using Microsoft.EntityFrameworkCore;
using System.Numerics;

namespace kickerapi.Services
{
    public class MatchService : IMatchService
    {
        private readonly KickerContext _context;

        public MatchService(KickerContext context)
        {
            _context = context;
        }

        public IQueryable<Match> GetMatches(Player player, bool isConfirmed)
        {
            return _context.Matches
                .Where(x => x.Team1.Attacker.Id == player.Id || x.Team1.Defender.Id == player.Id || x.Team2.Attacker.Id == player.Id || x.Team2.Defender.Id == player.Id)
                .WhereIf(x => x.Team1.IsConfirmed && x.Team2.IsConfirmed && x.IsCalculatedInRating, isConfirmed)
                .WhereIf(x => !x.Team1.IsConfirmed || !x.Team2.IsConfirmed || !x.IsCalculatedInRating, !isConfirmed)
                .OrderByDescending(x => x.Date);
        }
    }
}
