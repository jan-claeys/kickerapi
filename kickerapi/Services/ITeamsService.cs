﻿using ClassLibrary.Models;

namespace kickerapi.Services
{
    public interface ITeamsService : IServiceContext
    {
        public Task<Team> GetTeamWithPlayers(int teamId);
        public void RemoveTeam(Team team);
    }
}
