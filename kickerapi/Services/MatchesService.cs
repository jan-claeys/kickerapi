﻿
using ClassLibrary.Models;
using Microsoft.EntityFrameworkCore;

namespace kickerapi.Services
{
    public class MatchesService : ContextService, IMatchesService
    {
        public MatchesService(KickerContext context) : base(context)
        {
           
        }

        //returns all matches for a player (confirmed or unconfirmed) ordered by date ascending
        public IQueryable<Match> GetMatches(Player player, bool isConfirmed)
        {
            return _context.Matches
                .Where(x => x.Team1.Attacker.Id == player.Id || x.Team1.Defender.Id == player.Id || x.Team2.Attacker.Id == player.Id || x.Team2.Defender.Id == player.Id)
                .WhereIf(x => x.Team1.IsConfirmed && x.Team2.IsConfirmed && x.IsCalculatedInRating, isConfirmed)
                .WhereIf(x => !x.Team1.IsConfirmed || !x.Team2.IsConfirmed || !x.IsCalculatedInRating, !isConfirmed)
                .OrderBy(x => x.Date);
        }

        public IQueryable<Match> GetMatchesWithPlayers(Player player, bool isConfirmed)
        {
            return GetMatches(player, isConfirmed)
                .Include(x => x.Team1.Attacker)
                .Include(x => x.Team1.Defender)
                .Include(x => x.Team2.Attacker)
                .Include(x => x.Team2.Defender);
        }

        public IQueryable<Match> GetMatchWithTeams(Team team)
        {
            return _context.Matches.Where(x => x.Team1.Id == team.Id || x.Team2.Id == team.Id)
                .Include(x=>x.Team1)
                .Include(x=>x.Team2);
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
