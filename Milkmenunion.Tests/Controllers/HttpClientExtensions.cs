using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Milkmenunion.Tests.Controllers
{
    public static class HttpClientExtensions
    {
        public static async Task<TResponse> GetMessage<TResponse>(
            this HttpClient client,
            string uri,
            CancellationToken cancellationToken = default)
        {
            var httpResponse = await client.GetAsync(uri, cancellationToken);
            if (httpResponse.StatusCode == HttpStatusCode.NotFound)
            {
                return default;
            }

            httpResponse.EnsureSuccessStatusCode();
            var responseJson = await httpResponse.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<TResponse>(responseJson);
        }
    }
}