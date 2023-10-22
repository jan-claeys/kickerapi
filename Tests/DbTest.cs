using kickerapi;
using Microsoft.EntityFrameworkCore;

namespace Tests
{
    public class DbTest : IDisposable
    {
        protected readonly KickerContext _context;

        public DbTest(KickerContext context)
        {
            _context = context;
            _context.Database.OpenConnectionAsync();
            _context.Database.EnsureCreatedAsync();
        }

        public void Dispose()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }
    }
}
