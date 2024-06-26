﻿using Bogus;
using Customers.Api;
using Customers.Api.Contracts.Requests;
using Customers.Api.Contracts.Responses;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Testing;
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
    public class CreateCustomerControllerTests : IClassFixture<CustomerApiFactory>
    {
        private readonly CustomerApiFactory _apiFactory;
        private readonly HttpClient _client;

        private readonly Faker<CustomerRequest> _customerGenerator = new Faker<CustomerRequest>()
            .RuleFor(e => e.Email, faker => faker.Person.Email)
            .RuleFor(e => e.FullName, faker => faker.Person.FullName)
            .RuleFor(e => e.DateOfBirth, faker => faker.Person.DateOfBirth.Date)
            .RuleFor(e => e.GitHubUsername, faker => "MMR16");

        public CreateCustomerControllerTests(CustomerApiFactory factory)
        {
            _apiFactory = factory;
            _client = _apiFactory.CreateClient();
        }

        [Fact]
        public async Task CreateUserWhenDataIsValid()
        {
            // arrange
            var customer = _customerGenerator.Generate();

            // act
            var response = await _client.PostAsJsonAsync("Customers", customer);

            // assert
            var customerResponse = await response.Content.ReadFromJsonAsync<CustomerResponse>();
            customerResponse.Should().BeEquivalentTo(customer);
            response.StatusCode.Should().Be(HttpStatusCode.Created);
            response.Headers.Location.ToString().Should()
                .Be($"http://localhost/customers/{customerResponse}");

        }

        [Fact]
        public async Task Create_ReturnsValidationError_WhenUserNameIsInvalid()
        {
            // Arrage
            const string invalidEmail = "fffffffff";
           var customer=  _customerGenerator.Clone().RuleFor(e=>e.Email, invalidEmail);

            // Act 
            var response = await _client.PostAsJsonAsync("Customers", customer);
                //Assert

             response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            var error= await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();
            error!.Status.Should().Be(400);
            error.Title.Should().Be("One or more validation errors occurred.");
            error.Errors["Email"][0].Should().Be($"{invalidEmail} is not a valid email addressl");
        }

        [Fact]
        public async Task Create_ReturnsValidationError_WhenGitIsInvalid()
        {
            // Arrage
            const string invalidGitHub = "ffffffff2539f";
            var customer = _customerGenerator.Clone().RuleFor(e => e.GitHubUsername, invalidGitHub);

            // Act 
            var response = await _client.PostAsJsonAsync("Customers", customer);


            //Assert

            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            var error = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();
            error!.Status.Should().Be(400);
            error.Title.Should().Be("One or more validation errors occurred.");
            error.Errors["GitHubUsername"][0].Should().Be($"There is no GitHub user with username {invalidGitHub} ");
        }
       
        
        [Fact]
    public async Task Create_ReturnInternalServerErr_WhenServerTrottled()
        {
            // arrange
            var customer = _customerGenerator.Clone()
                .RuleFor(e=>e.GitHubUsername,CustomerApiFactory.ThrotteldUser).Generate();

            // act
            var response = await _client.PostAsJsonAsync("Customers", customer);

            //assert 
            response.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
        }
    }
}
