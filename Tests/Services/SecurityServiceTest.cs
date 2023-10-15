using kickerapi.Services;
using Microsoft.Extensions.Configuration;

namespace Tests.Services
{
    public class SecurityServiceTest
    {
        private readonly SecurityService _service;

        public SecurityServiceTest() {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.development.json")
                .Build();

            _service = new SecurityService(configuration);
        }

        [Fact]
        public void ItGenerateAToken()
        { 
           Assert.NotNull(_service.GenerateToken("test"));
           Assert.NotEmpty(_service.GenerateToken("test"));
        }
    }
}
