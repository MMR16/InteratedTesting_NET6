using Customers.Api;
using Microsoft.AspNetCore.Mvc.Testing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Customer.Api.test.intgtration
{
    [CollectionDefinition("CustomerApi Collection")]
    public class TestCollection :ICollectionFixture<WebApplicationFactory<IApiMarker>>
    {
    }
}
