﻿using ClassLibrary.Models;
using kickerapi;
using kickerapi.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

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

            services.AddDbContext<KickerContext>(options =>
            {
                options.UseSqlite("DataSource=:memory:");
                options.EnableSensitiveDataLogging();
            }); 

            //Identity
            services.AddIdentity<Player, IdentityRole>()
                .AddEntityFrameworkStores<KickerContext>()
                .AddDefaultTokenProviders();

            services.AddTransient<SecurityService>();

            services.AddAutoMapper(typeof(MappingProfiles));
        }
    }
}
