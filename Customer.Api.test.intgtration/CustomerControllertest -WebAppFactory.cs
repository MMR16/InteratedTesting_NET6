using Customers.Api;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Testing;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Customer.Api.test.intgtration
{
    public class CustomerControllertestWebAppFactory : IClassFixture<WebApplicationFactory<IApiMarker>>
    {
        //arrange
        private readonly WebApplicationFactory<IApiMarker> _AppFactory;
        private readonly HttpClient _httpClient;
        public CustomerControllertestWebAppFactory(WebApplicationFactory<IApiMarker> appFactory)
        {
            _AppFactory = appFactory;
            _httpClient = _AppFactory.CreateClient();
        }
        //skip to ignore test
        [Fact]
        public async Task Get_ReturnNotFound_CustomerNotExist()
        {
            //act
            HttpResponseMessage response = await _httpClient.GetAsync($"customers/{Guid.NewGuid()}");

            //assert
              // response.StatusCode.Should().Be(HttpStatusCode.NotFound);
            // Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
           // var text = await response.Content.ReadAsStringAsync();
            var problem = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();
            problem.Title.Should().Be("Not Found");
            problem.Status.Should().Be(404);


        }

    }
}
