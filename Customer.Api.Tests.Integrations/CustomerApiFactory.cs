using Customers.Api;
using Customers.Api.Database;
using Docker.DotNet.Models;
using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Configurations;
using DotNet.Testcontainers.Containers;
using FluentAssertions.Common;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Net.Http.Headers;
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

        private readonly GithubApiServer _gitHubApiServer = new();
        public const string ThrotteldUser = "throttle";

        public async Task InitializeAsync()
        {
            _gitHubApiServer.Start();
            _gitHubApiServer.SetupUser("MMR16");
            _gitHubApiServer.SetupThrottledUser(ThrotteldUser);

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
                services.AddHttpClient("GitHub", httpClient =>
                {
                    httpClient.BaseAddress = new Uri(_gitHubApiServer.Url);
                    httpClient.DefaultRequestHeaders.Add(
                        HeaderNames.Accept, "application/vnd.github.v3+json");
                    httpClient.DefaultRequestHeaders.Add(
                        HeaderNames.UserAgent, $"Course-{Environment.MachineName}");
                });
                services.RemoveAll(typeof(IHostedService));


                // to test database using EF Core 
                // remove the dbcontext that's registerd
                //then we register new one
                services.RemoveAll(typeof(AppDbContext));
                services.AddSingleton<IDbConnectionFactory>(_ =>
                 new NpgsqlConnectionFactory(_dbContainer.ConnectionString));

                // we never use InMemory Database
                //services.AddDbContext<AppDbContext>(
                //    OptionsBuilder => OptionsBuilder.UseInMemoryDatabase()
                //    ); 

            });
        }

        public new async Task DisposeAsync()
        {
            _gitHubApiServer?.Dispose();
            await _dbContainer.DisposeAsync();

        }
    }

    public class AppDbContext : DbContext
    {

    }
}
