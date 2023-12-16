using ClassLibrary.Models;
using Microsoft.EntityFrameworkCore;

namespace kickerapi.Services
{
    public class TeamsService : ContextService, ITeamsService
    {
        public TeamsService(KickerContext context) : base(context)
        {
        }

        public async Task<Team> GetTeamWithPlayers(int teamId)
        {
            return await _context.Teams.Where(x => x.Id == teamId).Include(x => x.Attacker)
                .Include(x => x.Defender).FirstOrDefaultAsync() ?? throw new NotFoundException("Team not found");
        }

        public void RemoveTeam(Team team)
        {
            _context.Remove(team);
        }
    }
}
