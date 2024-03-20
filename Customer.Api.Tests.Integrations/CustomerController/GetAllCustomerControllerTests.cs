using Bogus;
using Customers.Api.Contracts.Requests;
using Customers.Api.Contracts.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using FluentAssertions;

namespace Customer.Api.Tests.Integrations.CustomerController
{
    public class GetAllCustomerControllerTests : IClassFixture<CustomerApiFactory>
    {
        private readonly HttpClient _client;

        private readonly Faker<CustomerRequest> _customerGenerator = new Faker<CustomerRequest>()
            .RuleFor(e => e.Email, faker => faker.Person.Email)
            .RuleFor(e => e.FullName, faker => faker.Person.FullName)
            .RuleFor(e => e.DateOfBirth, faker => faker.Person.DateOfBirth.Date)
            .RuleFor(e => e.GitHubUsername, faker => "MMR16");

        public GetAllCustomerControllerTests(CustomerApiFactory factory)
        {
            _client = factory.CreateClient();
        }
        [Fact]
        public async Task GetAll_ReturnAllCustomers_WhenCustomerExist()
        {
            // arrange
            var customer = _customerGenerator.Generate();
            var customerResponse = await _client.PostAsJsonAsync("Customers", customer);
            var createdCustomer = await customerResponse.Content.ReadFromJsonAsync<CustomerResponse>();

            // act 
            var response = await _client.GetAsync($"Customers");


            //assert 
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var customersResponse = await response.Content.ReadFromJsonAsync<GetAllCustomersResponse>();
            customersResponse.Customers.Single().Should().BeEquivalentTo(createdCustomer);

            //cleanUp
            await _client.DeleteAsync($"Customers/{createdCustomer.Id}");
        }


        [Fact]
        public async Task GetAll_ReturnEmptyResult_WhenNoCustomerExist()
        {
         
            // act 
            var response = await _client.GetAsync($"Customers");


            //assert 
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var customersResponse = await response.Content.ReadFromJsonAsync<GetAllCustomersResponse>();
            customersResponse.Customers.Should().BeEmpty();

        }
    }

}


