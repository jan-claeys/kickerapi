using ClassLibrary.Models;
using kickerapi.Dtos;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace kickerapi.Services
{
    public class PlayersService : ContextService, IPlayersService
    {
        public PlayersService(KickerContext context) : base(context)
        {
        }

        public async Task<Player> GetPlayer(string playerId)
        {
            return await _context.Players.FirstOrDefaultAsync(x => x.Id == playerId) ?? throw new NotFoundException("Player not found");
        }

        // Returns all players ordered by username ascending searched by username
        public IQueryable<Player> GetPlayers(string? search)
        {
            return _context.Players
                .WhereIf(x => x.UserName.Contains(search), search)
                .OrderBy(x => x.UserName);
        }

        // Returns all players ordered by rating descending, default by rating or by attackrating or defendrating
        public IQueryable<Player> GetPlayersRanking(Position? orderBy)
        {
            //switch does not need default case because orderBy is nullable and their only 2 positions
            Expression<Func<Player, int>> order = orderBy switch
            {
                null => x => x.Rating,
                Position.Attacker => x => x.AttackRating,
                Position.Defender => x => x.DefendRating
            };

            return _context.Players.OrderByDescending(order);
        }
    }
}
