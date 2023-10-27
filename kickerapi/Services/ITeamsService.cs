using ClassLibrary.Models;
using kickerapi.Migrations;

namespace kickerapi.Services
{
    public interface ITeamsService : IServiceContext
    {
        public IQueryable<Team> GetTeam(int teamId);
        public IQueryable<Team> GetTeamWithPlayers(int teamId);
    }
}
