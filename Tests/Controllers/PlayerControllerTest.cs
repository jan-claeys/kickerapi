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

namespace Tests.Controllers
{
    public class PlayerControllerTest : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;


        public PlayerControllerTest(WebApplicationFactory<Program> application)
        {
            _client = application.CreateClient();
        }

        //[Fact]
        //public async void ItRegistersAPlayer()
        //{
        //    var payload = "{\"name\":\"test\",\"password\":\"test\"}";
        //    HttpContent content = new StringContent(payload, Encoding.UTF8, "application/json");
        //    var response = await _client.PostAsync("player/register", content);

        //    Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        //}

        [Fact]
        public async void ItRequiresNameAndPassword()
        {
            //this._client = new WebApplicationFactory<Program>().CreateClient();

            var payload = "{\"name\":\"fsfsf\"}";
            HttpContent content = new StringContent(payload, Encoding.UTF8, "application/json");
            var response = await _client.PostAsync("player/login", content);

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

            payload = "{\"password\":\"fsfsf\"}";
            content = new StringContent(payload, Encoding.UTF8, "application/json");
            response = await _client.PostAsync("player/login", content);

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        public void Dispose()
        {
            _client.Dispose();
        }
    }
}
