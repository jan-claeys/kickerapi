using ClassLibrary.Models;
using kickerapi.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;

namespace Tests.Services
{
    public class SecurityServiceTest
    {
        private readonly SecurityService _service;

        public SecurityServiceTest(UserManager<Player> userManager) {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.development.json")
                .Build();

            _service = new SecurityService(configuration, userManager);
        }

        [Fact]
        public void ItGenerateAToken()
        {
           Player player = new Player("test");

           Assert.NotNull(_service.GenerateJwtToken(player));
        }
    }
}
