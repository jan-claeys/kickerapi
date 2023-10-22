using ClassLibrary.Models;
using kickerapi;
using kickerapi.Controllers;
using kickerapi.Services;
using Microsoft.EntityFrameworkCore;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Tests.Controllers
{
    public class TeamsControllerTest : DbTest
    {
        private readonly TeamsController _controller;
        private readonly Player _currentPlayer;

        public TeamsControllerTest(KickerContext context): base(context)
        {
            _currentPlayer = new Player("test");
            var securityServiceMock = new Mock<ISecurityService>();
            securityServiceMock.Setup(x => x.GetUserAsync(It.IsAny<ClaimsPrincipal>()).Result).Returns(_currentPlayer);

            _controller = new TeamsController(context, securityServiceMock.Object, new MatchService(context));
        }

        //[Fact]
        //public async void ItConfirmsTeam()
        //{
        //    await _context.Database.OpenConnection();
        //    await _context.Database.EnsureCreatedAsync();
        //}

    }
}
