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
    public class SecurityControllerTest : DatabaseTest
    {
        private readonly SecurityController _controller;
        private readonly UserManager<Player> _userManager;

        public SecurityControllerTest(KickerContext context, UserManager<Player> userManager): base(context)
        {
            var configuration = new ConfigurationBuilder()
              .SetBasePath(Directory.GetCurrentDirectory())
              .AddJsonFile("appsettings.development.json")
              .Build();

            _controller = new SecurityController(new SecurityService(configuration,userManager));
            _userManager = userManager;
        }

        [Fact]
        public async void ItRegisterPlayer()
        {
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
            var payload = new RegisterDto
            {
                Name = "test",
                Password = "Test1*",
                Email = "test",
            };
            await _controller.Register(payload);

            payload = new RegisterDto
            {
                Name = "test",
                Password = "Test1*",
                Email = "test",
            };
            var response = await _controller.Register(payload);
            Assert.Equal(422, response.StatusCode);
        }

        [Fact]
        public async void ItNotRegisterPlayerWeakPassword()
        {
            var payload = new RegisterDto
            {
                Name = "test",
                Password = "test",
                Email = "test",
            };
            var response = await _controller.Register(payload);

            Assert.Equal(422, response.StatusCode);
        }

        [Fact]
        public async void ItLoginPlayer()
        {
            var player = new Player("test", "test@tillit.be");
            await _userManager.CreateAsync(player, "Test1*");

            var payload = new LoginDto
            {
                Email = "test",
                Password = "Test1*"
            };

            var response = await _controller.Login(payload);
            Assert.Equal(200, response.StatusCode);
        }

        [Fact]
        public async void ItNotLoginPlayerWrongPassword()
        {
            var player = new Player("test", "test@tillit.be");
            await _userManager.CreateAsync(player, "test");

            var payload = new LoginDto
            {
                Email = "test",
                Password = "test1"
            };

            var response = await _controller.Login(payload);
            Assert.Equal(401, response.StatusCode);
        }

        [Fact]
        public async void ItNotLoginPlayerWithoutAccount()
        {
            var payload = new LoginDto
            {
                Email = "test",
                Password = "test"
            };

            var response = await _controller.Login(payload);
            Assert.Equal(401, response.StatusCode);
        }
    }
}
