using ClassLibrary.Models;

namespace kickerapi.Services
{
    public interface IMatchService
    {
        public IQueryable<Match> GetMatches(Player player, bool isConfirmed);
    }
}
