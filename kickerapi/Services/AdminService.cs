using ClassLibrary.Models;
using kickerapi;
using kickerapi.Services;

namespace Api.Services
{
    public class AdminService : ContextService, IAdminService
    {

        public AdminService(KickerContext context) :  base(context)
        {
        }

        public IQueryable<Match> GetMatchesToReview()
        {
            return _context.Matches.Where(m => m.IsCalculatedInRating == false).OrderBy(x=>x.Date);
        }
    }
}
