using ClassLibrary.Models;
using Microsoft.EntityFrameworkCore;

namespace kickerapi.Services
{
    public class TeamsService : ContextService, ITeamsService
    {
        public TeamsService(KickerContext context) : base(context)
        {
        }

        public IQueryable<Team> GetTeam(int teamId)
        {
            return _context.Teams.Where(x => x.Id == teamId);
        }

        public IQueryable<Team> GetTeamWithPlayers(int teamId)
        {
            return GetTeam(teamId).Include(x => x.Attacker)
                .Include(x => x.Defender);
        }
    }
}
