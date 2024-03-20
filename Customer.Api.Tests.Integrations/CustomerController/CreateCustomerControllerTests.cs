using Customers.Api;
using Microsoft.AspNetCore.Mvc.Testing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Customer.Api.Tests.Integrations.CustomerController
{
    public class CreateCustomerControllerTests :IClassFixture<CustomerApiFactory>
    {
        private readonly CustomerApiFactory _factory;

        public CreateCustomerControllerTests(CustomerApiFactory factory)
        {
            _factory = factory;
        }

        [Fact]
        public async Task Test()
        {
            await Task.Delay(5000);
        }
    }
}
