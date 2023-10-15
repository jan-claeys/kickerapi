using kickerapi;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests
{
    internal class SharedDatabaseFixture: IDisposable
    {
        private readonly SqliteConnection connection;
        public SharedDatabaseFixture()
        {
            this.connection = new SqliteConnection("DataSource=:memory:");
            this.connection.Open();
        }
        public void Dispose() => this.connection.Dispose();
        public KickerContext CreateContext()
        {
            var result = new KickerContext(new DbContextOptionsBuilder<KickerContext>()
                .UseSqlite(this.connection)
                .Options);
            result.Database.EnsureCreated();
            return result;
        }
    }
}


