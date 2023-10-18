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

namespace Tests.Controllers
{
    public class PlayerControllerTest: IDisposable
    {
        private readonly PlayerController _controller;
        private readonly KickerContext _context;
        private readonly SecurityService _securityService;

        public PlayerControllerTest(KickerContext context,SecurityService securityService)
        {
            _context = context;
            _securityService = securityService;
            _controller = new PlayerController(_context, _securityService);
        }

        [Fact]
        public async void ItRegistersAPlayer()
        {
            await _context.Database.OpenConnectionAsync();
            await _context.Database.EnsureCreatedAsync();

            var payload = new RegisterDto
            {
                Name = "test",
                Password = "test"
            };
            var response = await _controller.Register(payload);
            Assert.Equal(200, response.StatusCode);

            var player = await _context.Players.FirstOrDefaultAsync(p => p.Name == payload.Name);
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
                Password = "test"
            };
            var response = await _controller.Register(payload);

             payload = new RegisterDto
            {
                Name = "test",
                Password = "test"
            };
            response = await _controller.Register(payload);
            Assert.Equal(400, response.StatusCode);
        }

        [Fact]
        public async void ItLoginPlayer()
        {
            await _context.Database.OpenConnectionAsync();
            await _context.Database.EnsureCreatedAsync();

            Player player = new Player("test", _securityService.HashPassword("test"));
            _context.Players.Add(player);
            await _context.SaveChangesAsync();

            var payload = new LoginDto
            {
                Name = "test",
                Password = "test"
            };

            var response = await _controller.Login(payload);
            Assert.Equal(200, response.StatusCode);
        }

        [Fact]
        public async void ItDoNotLoginPlayerWrongPassword()
        {
            await _context.Database.OpenConnectionAsync();
            await _context.Database.EnsureCreatedAsync();

            Player player = new Player("test", _securityService.HashPassword("test"));
            _context.Players.Add(player);
            await _context.SaveChangesAsync();

            var payload = new LoginDto
            {
                Name = "test",
                Password = "test1"
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
