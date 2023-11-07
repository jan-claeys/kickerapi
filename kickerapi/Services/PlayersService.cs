﻿using ClassLibrary.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace kickerapi.Services
{
    public class PlayersService : ContextService,  IPlayersService
    {
        public PlayersService(KickerContext context) : base(context)
        {
        }

        public async Task<Player> GetPlayer(string playerId)
        {
            return await _context.Players.FirstOrDefaultAsync(x => x.Id == playerId)?? throw new Exception("Player does noet exist");
        }

        // Returns all players ordered by username ascending searched by username
        public IQueryable<Player> GetPlayers(string? search)
        {
            return _context.Players
                .WhereIf(x => x.UserName.Contains(search), search)
                .OrderBy(x => x.UserName);
        }

        // Returns all players ordered by rating descending, default by rating or by attackrating or defendrating
        public IQueryable<Player> GetPlayersRanking(string? orderBy)
        {
            Expression<Func<Player, int>> order = orderBy switch
            {
                "Rating" => x => x.Rating,
                "AttackRating" => x => x.AttackRating,
                "DefendRating" => x => x.DefendRating,
                _ => x => x.Rating,
            };

            return _context.Players.OrderByDescending(order);
        }
    }
}