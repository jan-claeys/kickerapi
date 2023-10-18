﻿using Microsoft.AspNetCore.Mvc.Testing;
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

namespace Tests.Controllers
{
    public class PlayerControllerTest: IDisposable
    {
        private readonly PlayerController _controller;
        private readonly KickerContext _context;

        public PlayerControllerTest(SecurityService securityService)
        {
            _context = new KickerContext(new DbContextOptionsBuilder<KickerContext>()
                               .UseSqlite("DataSource=file::memory:?cache=shared")
                                              .Options);
          
            _controller = new PlayerController(_context, securityService);
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

            var player  = await _context.Players.FirstOrDefaultAsync(p => p.Name == payload.Name);
            Assert.NotNull(player);
        }

        public void Dispose()
        {
            _context.Database.CloseConnectionAsync();
        }
    }
}
