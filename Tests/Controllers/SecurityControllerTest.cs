using kickerapi;
using kickerapi.Controllers;
using Microsoft.EntityFrameworkCore;
using kickerapi.Services;
using ClassLibrary.Models;
using Microsoft.AspNetCore.Identity;
using kickerapi.Dtos.Requests.Security;
using Microsoft.Extensions.Configuration;

namespace Tests.Controllers
{
    public class SecurityControllerTest : IDisposable
    {
        private readonly SecurityController _controller;
        private readonly KickerContext _context;
        private readonly UserManager<Player> _userManager;

        public SecurityControllerTest(KickerContext context, UserManager<Player> userManager)
        {
            var configuration = new ConfigurationBuilder()
              .SetBasePath(Directory.GetCurrentDirectory())
              .AddJsonFile("appsettings.development.json")
              .Build();

            _context = context;
            _controller = new SecurityController(new SecurityService(configuration,userManager));
            _userManager = userManager;
        }

        [Fact]
        public async void ItRegisterPlayer()
        {
            await _context.Database.OpenConnectionAsync();
            await _context.Database.EnsureCreatedAsync();

            var payload = new RegisterDto
            {
                Name = "test",
                Password = "Test1*"
            };
            var response = await _controller.Register(payload);
            Assert.Equal(200, response.StatusCode);

            var player = await _context.Players.FirstOrDefaultAsync(p => p.UserName == payload.Name);
            Assert.NotNull(player);
        }

        [Fact]
        public async void ItDoNotRegisterPlayerWithSameName()
        {
            await _context.Database.OpenConnectionAsync();
            await _context.Database.EnsureCreatedAsync();

            var payload = new RegisterDto
            {
                Name = "test",
                Password = "Test1*"
            };
            await _controller.Register(payload);

            payload = new RegisterDto
            {
                Name = "test",
                Password = "Test1*"
            };
            var response = await _controller.Register(payload);
            Assert.Equal(400, response.StatusCode);
        }

        [Fact]
        public async void ItNotRegisterPlayerWeakPassword()
        {
            await _context.Database.OpenConnectionAsync();
            await _context.Database.EnsureCreatedAsync();

            var payload = new RegisterDto
            {
                Name = "test",
                Password = "test"
            };
            var response = await _controller.Register(payload);

            Assert.Equal(400, response.StatusCode);
        }

        [Fact]
        public async void ItLoginPlayer()
        {
            await _context.Database.OpenConnectionAsync();
            await _context.Database.EnsureCreatedAsync();

            var player = new Player("test");
            await _userManager.CreateAsync(player, "Test1*");

            var payload = new LoginDto
            {
                Name = "test",
                Password = "Test1*"
            };

            var response = await _controller.Login(payload);
            Assert.Equal(200, response.StatusCode);
        }

        [Fact]
        public async void ItNotLoginPlayerWrongPassword()
        {
            await _context.Database.OpenConnectionAsync();
            await _context.Database.EnsureCreatedAsync();

            var player = new Player("test");
            await _userManager.CreateAsync(player, "test");

            var payload = new LoginDto
            {
                Name = "test",
                Password = "test1"
            };

            var response = await _controller.Login(payload);
            Assert.Equal(401, response.StatusCode);
        }

        [Fact]
        public async void ItNotLoginPlayerWithoutAccount()
        {

            await _context.Database.OpenConnectionAsync();
            await _context.Database.EnsureCreatedAsync();

            var payload = new LoginDto
            {
                Name = "test",
                Password = "test"
            };

            var response = await _controller.Login(payload);
            Assert.Equal(401, response.StatusCode);
        }

        public void Dispose()
        {
            _context.Database.CloseConnectionAsync();
            _context.Dispose();
        }
    }
}
