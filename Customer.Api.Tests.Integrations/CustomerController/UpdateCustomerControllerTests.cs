using Bogus;
using Customers.Api.Contracts.Requests;
using Customers.Api.Contracts.Responses;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using FluentAssertions;
using WireMock.ResponseBuilders;

namespace Customer.Api.Tests.Integrations.CustomerController
{
    public class UpdateCustomerControllerTests : IClassFixture<CustomerApiFactory>
    {
        private readonly CustomerApiFactory _apiFactory;
        private readonly HttpClient _client;

        private readonly Faker<CustomerRequest> _customerGenerator = new Faker<CustomerRequest>()
            .RuleFor(e => e.Email, faker => faker.Person.Email)
            .RuleFor(e => e.FullName, faker => faker.Person.FullName)
            .RuleFor(e => e.DateOfBirth, faker => faker.Person.DateOfBirth.Date)
            .RuleFor(e => e.GitHubUsername, faker => "MMR16");

        public UpdateCustomerControllerTests(CustomerApiFactory factory)
        {
            _apiFactory = factory;
            _client = _apiFactory.CreateClient();
        }

        [Fact]
        public async Task Update_UpdatesUser_WhenDataIsVa1id()
        {
            // arrange
            var customer = _customerGenerator.Generate();
            var CreatedResponse = await _client.PostAsJsonAsync("Customers", customer);
            var createdCustomer = await CreatedResponse.Content.ReadFromJsonAsync<CustomerResponse>();

            customer = _customerGenerator.Generate();
            // act
            var response = await _client.PutAsJsonAsync($"Customers/{createdCustomer!.Id}", customer);

            // assert
            var customerResponse = await response.Content.ReadFromJsonAsync<CustomerResponse>();
            customerResponse.Should().BeEquivalentTo(customer);
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            response.Headers.Location.ToString().Should()
                .Be($"http://localhost/customers/{customerResponse}");

        }

        [Fact]
        public async Task Update_ReturnsVa1idationError_WhenEmai11sInva1id()
        {
            // Arrage
            var customer = _customerGenerator.Generate();
            var CreatedResponse = await _client.PostAsJsonAsync("Customers", customer);
            var createdCustomer = await CreatedResponse.Content.ReadFromJsonAsync<CustomerResponse>();

            const string invalidEmail = "fffffffff";
            customer = _customerGenerator.Clone().RuleFor(e => e.Email, invalidEmail);

            // Act 
            var response = await _client.PutAsJsonAsync($"Customers/{createdCustomer!.Id}", customer);
          
            //Assert

            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            var error = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();
            error!.Status.Should().Be(400);
            error.Title.Should().Be("One or more validation errors occurred.");
            error.Errors["Email"][0].Should().Be($"{invalidEmail} is not a valid email addressl");
        }

        [Fact]
        public async Task Update_ReturnsVa1idationError_WhenGitHubUserDoestNotExist()
        {
            // Arrage
            var customer = _customerGenerator.Generate();
            var CreatedResponse = await _client.PostAsJsonAsync("Customers", customer);
            var createdCustomer = await CreatedResponse.Content.ReadFromJsonAsync<CustomerResponse>();

            // Act 
            var response = await _client.PutAsJsonAsync($"Customers/{createdCustomer!.Id}", customer);

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            var error = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();
            error!.Status.Should().Be(400);
            error.Title.Should().Be("One or more validation errors occurred.");
        }
    }
}
