using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using HalKit.Http;
using Moq;

namespace HalKit.Tests.Fakes
{
    public class FakeHttpClientFactory : IHttpClientFactory
    {
        private readonly HttpClient _httpClient;

        public FakeHttpClientFactory(HttpClient http = null)
        {
            var mockClient = new Mock<HttpClient>(MockBehavior.Loose);
            mockClient.Setup(h => h.SendAsync(It.IsAny<HttpRequestMessage>(), It.IsAny<CancellationToken>()))
                      .Returns(Task.FromResult(new HttpResponseMessage()));
            _httpClient = http ?? mockClient.Object;
        }

        public HttpClient CreateClient(IEnumerable<DelegatingHandler> handlers)
        {
            return _httpClient;
        }
    }
}
