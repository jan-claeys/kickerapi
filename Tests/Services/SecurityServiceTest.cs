using kickerapi.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests.Services
{
    public class SecurityServiceTest : BaseTest
    {
        private readonly SecurityService _service;

        public SecurityServiceTest(SecurityService service) {
            _service = service;
        }

        [Fact]
        public void ItGenerateAToken()
        { 
           Assert.NotNull(_service.GenerateToken("test"));
           Assert.NotEmpty(_service.GenerateToken("test"));
        }
    }
}
