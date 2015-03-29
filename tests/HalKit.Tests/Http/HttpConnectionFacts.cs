using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using HalKit.Http;
using HalKit.Json;
using HalKit.Tests.Fakes;
using Moq;
using Xunit;

namespace HalKit.Tests.Http
{
    public class HttpConnectionFacts
    {
        private static HttpConnection CreateConnection(
            IEnumerable<DelegatingHandler> handlers = null,
            IJsonSerializer serializer = null,
            IHttpClientFactory httpFact = null,
            IHalKitConfiguration config = null,
            IApiResponseFactory responseFactory = null)
        {
            return new HttpConnection(
                handlers ?? new DelegatingHandler[] {},
                config ?? HalKitConfiguration.Default,
                httpFact ?? new FakeHttpClientFactory(),
                serializer ?? new Mock<IJsonSerializer>(MockBehavior.Loose).Object,
                responseFactory ?? new FakeApiResponseFactory());
        }

        public class TheConstructor
        {
            [Fact]
            public void ShouldPassTheGivenMiddlewareToTheHttpClientFactory()
            {
                var expectedHandlers = new[] { new FakeDelegatingHandler(), new FakeDelegatingHandler() };
                var mockFact = new Mock<IHttpClientFactory>(MockBehavior.Loose);
                mockFact.Setup(f => f.CreateClient(expectedHandlers)).Returns(new HttpClient()).Verifiable();

                CreateConnection(httpFact: mockFact.Object, handlers: expectedHandlers);

                mockFact.Verify();
            }
        }

        public class TheConfigurationProperties
        {
            [Fact]
            public void ShouldReturnTheGivenConfiguration()
            {
                var expectedConfig = new HalKitConfiguration();
                var connection = CreateConnection(config: expectedConfig);

                var actualConfig = connection.Configuration;

                Assert.Same(expectedConfig, actualConfig);
            }
        }

        public class TheSendRequestAsyncMethod
        {
            public static object[] HttpMethods =
            {
                new object[] {HttpMethod.Trace},
                new object[] {HttpMethod.Put},
                new object[] {HttpMethod.Post},
                new object[] {HttpMethod.Options},
                new object[] {HttpMethod.Head},
                new object[] {HttpMethod.Get},
                new object[] {HttpMethod.Delete}
            };

            [Fact]
            public async void ShouldSendAnHttpRequestMessageWithRequestUriSetToTheGivenUri()
            {
                var expectedUri = new Uri("https://foo.io");
                var mockHttp = new Mock<HttpClient>(MockBehavior.Loose);
                mockHttp.Setup(h => h.SendAsync(It.Is<HttpRequestMessage>(r => r.RequestUri == expectedUri),
                                                It.IsAny<CancellationToken>()))
                        .Returns(Task.FromResult(new HttpResponseMessage()))
                        .Verifiable();
                var conn = CreateConnection(httpFact: new FakeHttpClientFactory(http: mockHttp.Object));

                await conn.SendRequestAsync<string>(expectedUri, HttpMethod.Trace, null, null);

                mockHttp.Verify();
            }

            [Theory, MemberData("HttpMethods")]
            public async void ShouldSendAnHttpRequestMessageWithTheGivenHttpMethod(HttpMethod expectedMethod)
            {
                var mockHttp = new Mock<HttpClient>(MockBehavior.Loose);
                mockHttp.Setup(h => h.SendAsync(It.Is<HttpRequestMessage>(r => r.Method == expectedMethod),
                                                It.IsAny<CancellationToken>()))
                        .Returns(Task.FromResult(new HttpResponseMessage()))
                        .Verifiable();
                var conn = CreateConnection(httpFact: new FakeHttpClientFactory(http: mockHttp.Object));

                await conn.SendRequestAsync<string>(new Uri("https://api.vgg.io"), expectedMethod, null, null);

                mockHttp.Verify();
            }

            [Fact]
            public async void ShouldSendAnHttpRequestMessageWithTheGivenHttpHeaders()
            {
                var expectedHeaderKey = "HeaderKey";
                var expectedHeaderValues = new[] {"Foo", "Bar"};
                IEnumerable<string> actualHeaderValues = null;
                var mockHttp = new Mock<HttpClient>(MockBehavior.Loose);
                mockHttp.Setup(h => h.SendAsync(It.IsAny<HttpRequestMessage>(), It.IsAny<CancellationToken>()))
                        .Callback((HttpRequestMessage request, CancellationToken token) =>
                            actualHeaderValues = request.Headers.GetValues(expectedHeaderKey))
                        .Returns(Task.FromResult(new HttpResponseMessage()));
                var conn = CreateConnection(httpFact: new FakeHttpClientFactory(http: mockHttp.Object));

                await conn.SendRequestAsync<string>(
                    new Uri("https://api.io"),
                    HttpMethod.Put,
                    null,
                    new Dictionary<string, IEnumerable<string>>
                    {
                        {expectedHeaderKey, expectedHeaderValues}
                    });

                Assert.Equal(expectedHeaderValues, actualHeaderValues);
            }

            [Fact]
            public async void ShouldSendAnHttpRequestMessageWithNullContent_WhenHttpMethodIsGet()
            {
                var mockHttp = new Mock<HttpClient>(MockBehavior.Loose);
                mockHttp.Setup(h => h.SendAsync(It.Is<HttpRequestMessage>(r => r.Content == null),
                                                It.IsAny<CancellationToken>()))
                        .Returns(Task.FromResult(new HttpResponseMessage()))
                        .Verifiable();
                var conn = CreateConnection(httpFact: new FakeHttpClientFactory(http: mockHttp.Object));

                await conn.SendRequestAsync<string>(
                    new Uri("https://api.io"),
                    HttpMethod.Get,
                    "body",
                    new Dictionary<string, IEnumerable<string>>
                    {
                        {"Content-Type", new[] {"application/json"}}
                    });

                mockHttp.Verify();
            }

            [Fact]
            public async void ShouldSendAnHttpRequestMessageWithTheGivenHttpContent_WhenBodyIsHttpContent()
            {
                var expectedContent = new ByteArrayContent(new byte[] { });
                HttpContent actualContent = null;
                var mockHttp = new Mock<HttpClient>(MockBehavior.Loose);
                mockHttp.Setup(h => h.SendAsync(It.IsAny<HttpRequestMessage>(), It.IsAny<CancellationToken>()))
                        .Callback((HttpRequestMessage request, CancellationToken token) =>
                            actualContent = request.Content)
                        .Returns(Task.FromResult(new HttpResponseMessage()));
                var conn = CreateConnection(httpFact: new FakeHttpClientFactory(http: mockHttp.Object));

                await conn.SendRequestAsync<string>(
                    new Uri("https://api.io"),
                    HttpMethod.Put,
                    expectedContent,
                    new Dictionary<string, IEnumerable<string>>
                    {
                        {"Content-Type", new[] {"application/json"}}
                    });

                Assert.Same(expectedContent, actualContent);
            }

            [Fact]
            public async void ShouldPassTheHttpResponseMessageToTheApiResponseFactory()
            {
                var expectedResponse = new HttpResponseMessage();
                var mockHttp = new Mock<HttpClient>(MockBehavior.Loose);
                mockHttp.Setup(h => h.SendAsync(It.IsAny<HttpRequestMessage>(), It.IsAny<CancellationToken>()))
                        .Returns(Task.FromResult(expectedResponse));
                var mockRespFact = new Mock<IApiResponseFactory>(MockBehavior.Loose);
                mockRespFact.Setup(r => r.CreateApiResponseAsync<object>(expectedResponse))
                            .Returns(Task.FromResult<IApiResponse<object>>(new ApiResponse<object>()))
                            .Verifiable();
                var conn = CreateConnection(httpFact: new FakeHttpClientFactory(http: mockHttp.Object),
                                            responseFactory: mockRespFact.Object);

                await conn.SendRequestAsync<object>(
                    new Uri("https://api.vgg.io"),
                    HttpMethod.Get,
                    null,
                    null);

                mockRespFact.Verify();
            }

            [Fact]
            public async void SendRequestAsync_ShouldReturnTheApiResponseReturnedByTheResponseFactory()
            {
                var expectedApiResponse = new ApiResponse<object>();
                var conn = CreateConnection(responseFactory: new FakeApiResponseFactory(resp: expectedApiResponse));

                var actualApiResponse = await conn.SendRequestAsync<object>(
                                            new Uri("https://api.vgg.io"),
                                            HttpMethod.Get,
                                            null,
                                            null);

                Assert.Same(expectedApiResponse, actualApiResponse);
            }
        }
    }
}
