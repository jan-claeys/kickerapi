using Microsoft.AspNetCore.Mvc.Testing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using kickerapi;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http.Json;
using kickerapi.Controllers;
using kickerapi.Dtos.Player;
using Microsoft.EntityFrameworkCore;
using kickerapi.Services;
using Moq;
using ClassLibrary.Models;
using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using Microsoft.AspNetCore.Identity;

namespace Tests.Controllers
{
    public class PlayerControllerTest: IDisposable
    {
        private readonly PlayersController _controller;
        private readonly KickerContext _context;
        private readonly SecurityService _securityService;
        private readonly UserManager<Player> _userManager;

        public PlayerControllerTest(KickerContext context,SecurityService securityService, IMapper mapper, UserManager<Player> userManager)
        {
            _context = context;
            _securityService = securityService;
            _controller = new PlayersController(_context, _securityService, mapper, userManager);
            _userManager = userManager;
        }

        [Fact]
        public async void ItRegistersAPlayer()
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
            var response = await _controller.Register(payload);

             payload = new RegisterDto
            {
                Name = "test",
                Password = "Test1*"
             };
            response = await _controller.Register(payload);
            Assert.Equal(400, response.StatusCode);
        }

        [Fact]
        public async void ItDoNotRegisterPlayerWeakPassword()
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

            Player player = new Player("test");
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
        public async void ItDoNotLoginPlayerWrongPassword()
        {
            await _context.Database.OpenConnectionAsync();
            await _context.Database.EnsureCreatedAsync();

            Player player = new Player("test");
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
        public async void ItDoNotLoginPlayerWithoutAccount()
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
