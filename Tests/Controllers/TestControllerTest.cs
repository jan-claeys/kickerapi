using Api.Controllers;
using Microsoft.AspNetCore.Mvc;

namespace Tests.Controllers
{
    public class TestControllerTest
    {
        private readonly TestController _controller;
        public TestControllerTest()
        {
            _controller = new TestController();
        }

        [Fact]
        public async void ItReturnsItWorks()
        {
            var result = await _controller.Get();
            var okResult = result as OkObjectResult;

            Assert.NotNull(okResult);
            Assert.Equal("It works", okResult.Value);
        }
    }
}
