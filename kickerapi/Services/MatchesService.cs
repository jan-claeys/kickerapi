
using ClassLibrary.Models;
using kickerapi.Dtos;
using Microsoft.EntityFrameworkCore;

namespace kickerapi.Services
{
    public class MatchesService : ContextService, IMatchesService
    {
        public MatchesService(KickerContext context) : base(context)
        {

        }

        // Returns all matches for a player (confirmed or unconfirmed) for a optional currentplayer position ordered by date ascending
        public IQueryable<Match> GetMatches(Player player, bool isConfirmed, Position? playerPosition = null)
        {
            return _context.Matches
                .WhereIf(x => x.Team1.Attacker == player || x.Team2.Attacker == player, playerPosition == Position.Attacker)
                .WhereIf(x => x.Team1.Defender == player || x.Team2.Defender == player, playerPosition == Position.Defender)
                .WhereIf(x => x.Team1.Attacker == player || x.Team1.Defender == player || x.Team2.Attacker == player || x.Team2.Defender == player, playerPosition == null)
                .WhereIf(x => x.Team1.IsConfirmed && x.Team2.IsConfirmed && x.IsCalculatedInRating, isConfirmed)
                .WhereIf(x => !x.Team1.IsConfirmed || !x.Team2.IsConfirmed || !x.IsCalculatedInRating, !isConfirmed)
                .OrderBy(x => x.Date);
        }

        //  Returns all matches for a player (confirmed or unconfirmed) ordered by date ascending with the 2 teams and their players for a playerPosition
        public IQueryable<Match> GetMatchesWithPlayers(Player player, bool isConfirmed, Position? playerPosition = null)
        {
            return GetMatches(player, isConfirmed, playerPosition)
                .Include(x => x.Team1.Attacker)
                .Include(x => x.Team1.Defender)
                .Include(x => x.Team2.Attacker)
                .Include(x => x.Team2.Defender);
        }

        // Returns the match from a team with the other teams
        public async Task<Match> GetMatchWithTeams(Team team)
        {
            return await _context.Matches.Where(x => x.Team1 == team || x.Team2 == team)
                .Include(x => x.Team1)
                .Include(x => x.Team2)
                .FirstOrDefaultAsync() ?? throw new Exception("Match not found");
        }

        public IQueryable<Match> GetMatchesToReview(Player player)
        {
            return GetMatchesWithPlayers(player, false)
                .Where(x => !x.IsCalculatedInRating)
                .Where(x => (x.Team1.Attacker == player || x.Team1.Defender == player) && !x.Team1.IsConfirmed
                || (x.Team2.Attacker == player || x.Team2.Defender == player) && !x.Team2.IsConfirmed);
        }

        public IQueryable<Match> GetMatchesUnderReview(Player player)
        {
            return GetMatchesWithPlayers(player, false)
                .Where(x => !x.IsCalculatedInRating)
                .Where(x => (x.Team1.Attacker == player || x.Team1.Defender == player) && x.Team1.IsConfirmed
                || (x.Team2.Attacker == player || x.Team2.Defender == player) && x.Team2.IsConfirmed);
        }

        public async void AddMatch(Match match)
        {
            await _context.AddAsync(match);
        }

        public void RemoveMatch(Match match)
        {
            _context.Remove(match);
        }
    }
}
