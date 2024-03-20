using Bogus;
using Customers.Api.Contracts.Requests;
using Customers.Api.Contracts.Responses;
using Customers.Api;
using Microsoft.AspNetCore.Mvc.Testing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using FluentAssertions;
using System.Net.Http;
using Microsoft.AspNetCore.Mvc;

namespace Customer.Api.test.intgtration.CustomerController
{
    public class GetCustomerControllerTests : IClassFixture<WebApplicationFactory<IApiMarker>>

    {
        private readonly HttpClient _client;

        private readonly Faker<CustomerRequest> _customerGenerator = new Faker<CustomerRequest>()
            .RuleFor(x => x.Email, faker => faker.Person.Email)
            .RuleFor(x => x.FullName, faker => faker.Person.FullName)
            .RuleFor(x => x.GitHubUsername, "MMR16")
            .RuleFor(x => x.DateOfBirth, faker => faker.Person.DateOfBirth.Date);

        private readonly List<Guid> _createdIds = new List<Guid>();
        public GetCustomerControllerTests(WebApplicationFactory<IApiMarker> apiFactory)
        {
            _client = apiFactory.CreateClient();

        }

        [Fact]
        public async Task Get_ReturnNotFound_CustomerNotExist()
        {
            //act
            HttpResponseMessage response = await _client.GetAsync($"customers/{Guid.NewGuid()}");

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
