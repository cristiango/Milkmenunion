using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Shouldly;
using Xunit;

namespace Milkmenunion.Tests
{
    public class HttpPipelineTests: IAsyncLifetime
    {
        private TestSystem _testSystem;
        private HttpClient _client;

        [Fact]
        public async Task Can_get_basic_controller()
        {
            var result  =await _client.GetAsync("WeatherForecast");
            result.StatusCode.ShouldBe(HttpStatusCode.OK);
        }

        public async Task InitializeAsync()
        {
            _testSystem = await TestSystem.Create();
            _client = _testSystem.CreateClient();
        }

        public Task DisposeAsync()
        {
            _testSystem.Dispose();
            return Task.CompletedTask;
        }
    }
}
