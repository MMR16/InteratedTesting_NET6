using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Server;

namespace Customer.Api.Tests.Integrations
{
    public class GithubApiServer :IDisposable
    {
        private WireMockServer _server;


        public void Start()
        {
            _server = WireMockServer.Start();
        }


        public void SetupUser(string userName)
        {
            _server.Given(Request.Create())
                .WithPath($"/Users/{userName}")
                .RespondWith(Response.Create());
              
        }

        public void Dispose()
        {
          _server.Stop();
          _server.Dispose();
        }
    }
}
