using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests
{
    public static class DbContextExtensions
    {
        public static async Task<int> SaveChangesAsyncTest(this DbContext dbContext)
        { 
            var affectedRows = await dbContext.SaveChangesAsync();
            dbContext.ChangeTracker.Entries().Where(e => e.Entity != null).ToList().ForEach(e => e.State = EntityState.Detached);
            return affectedRows;
        }
    }
}
