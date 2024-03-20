using Customers.Api;
using Customers.Api.Database;
using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Configurations;
using DotNet.Testcontainers.Containers;
using FluentAssertions.Common;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Customer.Api.Tests.Integrations
{
    public class CustomerApiFactory : WebApplicationFactory<IApiMarker>, IAsyncLifetime
    {
        //private readonly TestcontainersContainer _dbContainer =
        //    new TestcontainersBuilder<TestcontainersContainer>()
        //    .WithImage("postgres:latest")
        //    .WithEnvironment(" POSTGRES_USER", "course")
        //    .WithEnvironment("POSTGRES_PASSWORD", "changeme")
        //    .WithEnvironment("POSTGRES_DB", "mydb")
        //    .WithPortBinding(5555, 5432)
        //    .WithWaitStrategy(Wait.ForUnixContainer().UntilPortIsAvailable(5432))
        //    .Build();

        private readonly TestcontainerDatabase _dbContainer =
             new TestcontainersBuilder<PostgreSqlTestcontainer>()
                 .WithDatabase(new PostgreSqlTestcontainerConfiguration
                 {
                     Database = "db",
                     Username = "course",
                     Password = "changeme"
                 }).Build();

        public async Task InitializeAsync()
        {
            await _dbContainer.StartAsync();
        }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureLogging(logging =>
            {
                logging.ClearProviders();
            });

            builder.ConfigureTestServices(services =>
            {
                services.RemoveAll(typeof(IDbConnectionFactory));
                services.AddSingleton<IDbConnectionFactory>(_ =>
                    new NpgsqlConnectionFactory(_dbContainer.ConnectionString));

            });
        }

        public new async Task DisposeAsync()
        {
            await _dbContainer.DisposeAsync();

        }
    }
}
