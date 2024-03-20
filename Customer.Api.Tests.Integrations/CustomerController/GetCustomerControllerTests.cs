using Bogus;
using Customers.Api.Contracts.Requests;
using Customers.Api.Contracts.Responses;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Customer.Api.Tests.Integrations.CustomerController
{
    public class GetCustomerControllerTests : IClassFixture<CustomerApiFactory>
    {
        private readonly HttpClient _client;

        private readonly Faker<CustomerRequest> _customerGenerator = new Faker<CustomerRequest>()
            .RuleFor(e => e.Email, faker => faker.Person.Email)
            .RuleFor(e => e.FullName, faker => faker.Person.FullName)
            .RuleFor(e => e.DateOfBirth, faker => faker.Person.DateOfBirth.Date)
            .RuleFor(e => e.GitHubUsername, faker => "MMR16");

        public GetCustomerControllerTests(CustomerApiFactory factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task Get_Return_WhenCustomerExists()
        {
            // arrange
            var customer = _customerGenerator.Generate();
            var customerResponse =await _client.PostAsJsonAsync("Customers",customer);
            var createdCustomer= await customerResponse.Content.ReadFromJsonAsync<CustomerResponse>();

            // act 
            var response = await _client.GetAsync($"Customers/{createdCustomer!.Id}");


            //assert 
            var retrievedCustomer= await response.Content.ReadFromJsonAsync<CustomerResponse>();
            retrievedCustomer.Should().BeEquivalentTo(createdCustomer);
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }



        [Fact]
        public async Task Get_ReturnNotFound_WhenCustomerExists()
        {
        
            // act 
            var response = await _client.GetAsync($"Customers/{Guid.NewGuid()}");


            //assert 
           
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }
    }
}
