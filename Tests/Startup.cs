using kickerapi;
using kickerapi.Services;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            var configuration = new ConfigurationBuilder()
               .SetBasePath(Directory.GetCurrentDirectory())
               .AddJsonFile("appsettings.development.json")
               .Build();

            services.AddTransient<SecurityService>(x=> new SecurityService(configuration));

            services.AddTransient<KickerContext>(x => new KickerContext(new DbContextOptionsBuilder<KickerContext>()
                               .UseSqlite("DataSource=file::memory:?cache=shared")
                                              .Options));
        }
    }
}
