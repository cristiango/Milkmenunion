using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MilkmenUnion;
using MilkmenUnion.Storage;
using Milkmenunion.Tests.Infra;

namespace Milkmenunion.Tests
{
    public class TestSystem : IDisposable
    {
        private readonly TestServer _testServer;

        private readonly Uri MilkmenUnionUri = new Uri("http://milkmenunion.org");

        private TestSystem()
        {
            var webHostBuilder = new WebHostBuilder()
                .UseStartup<Startup>()
                .ConfigureLogging(x => x.AddDebug().AddConsole())
                .ConfigureServices(services =>
                    {
                        services.AddSingleton(new DbContextOptionsBuilder<CompanyDbContext>()
                            .UseInMemoryDatabase("InMemoryDb").Options);
                    });

            _testServer = new TestServer(webHostBuilder);
            using (var scope = _testServer.Host.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                var context = services.GetRequiredService<CompanyDbContext>();
                context.SeedWithTestData();
            }
        }

        public static async Task<TestSystem> Create()
        {
            var testSystem = new TestSystem();
            await testSystem.WaitForPipelineReady();
            return testSystem;
        }

        public HttpClient CreateClient()
        {
            return _testServer.CreateClient();
        }

        private async Task WaitForPipelineReady()
        {
            var client = _testServer.CreateClient();
            var response = await client.GetAsync(MilkmenUnionUri);
            var i = 0;
            while (response.StatusCode == HttpStatusCode.ServiceUnavailable)
            {
                await Task.Delay(50);
                i++;
                if (i > 200)
                {
                    throw new TimeoutException("Timed out waiting for server to boot.");
                }

                ;
                response = await client.GetAsync(MilkmenUnionUri.CreateRelative("health"));
            }

            if (response.StatusCode == HttpStatusCode.InternalServerError)
            {
                throw new TimeoutException("Server failed to boot.");
            }
        }

        public void Dispose()
        {
            _testServer.Dispose();
        }
    }
}