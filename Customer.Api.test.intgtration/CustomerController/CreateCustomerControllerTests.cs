using Bogus;
using Customers.Api;
using Customers.Api.Contracts.Requests;
using Customers.Api.Contracts.Responses;
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

namespace Customer.Api.test.intgtration.CustomerController
{
    [Collection("CustomerApi Collection")]
    public class CreateCustomerControllerTests //: IClassFixture<WebApplicationFactory<IApiMarker>>, IAsyncLifetime

    {
        private readonly HttpClient _client;

        private readonly Faker<CustomerRequest> _customerGenerator = new Faker<CustomerRequest>()
            .RuleFor(x => x.Email, faker => faker.Person.Email)
            .RuleFor(x => x.FullName, faker => faker.Person.FullName)
            .RuleFor(x => x.GitHubUsername, "MMR16")
            .RuleFor(x => x.DateOfBirth, faker => faker.Person.DateOfBirth.Date);

        private readonly List<Guid> _createdIds = new List<Guid>();
        public CreateCustomerControllerTests(WebApplicationFactory<IApiMarker> apiFactory)
        {
            _client = apiFactory.CreateClient();

        }


        [Fact]
        public async Task Create_CreatesUser_WhenDataIsValid()
        {
            // Arrange
            var customer = _customerGenerator.Generate();

            // Act
            var response = await _client.PostAsJsonAsync("customers", customer);

            // Assert
            var customerResponse = await response.Content.ReadFromJsonAsync<CustomerResponse>();
            customerResponse.Should().BeEquivalentTo(customer);
            response.StatusCode.Should().Be(HttpStatusCode.Created);
            _createdIds.Add(customerResponse!.Id);
        }

        public async Task DisposeAsync()
        {
            foreach (var item in _createdIds)
            {
                await _client.DeleteAsync($"Customers/{item}");
            }
        }

        public Task InitializeAsync()
        {
            return Task.CompletedTask;
        }
    }

}
