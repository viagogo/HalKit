using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using HalKit.Http;
using HalKit.Models;
using HalKit.Models.Response;
using HalKit.Services;
using HalKit.Tests.Fakes;
using Moq;
using Xunit;

namespace HalKit.Tests
{
    public class HalClientTests
    {
        private static HalClient CreateClient(IHttpConnection conn = null,
                                              IHalKitConfiguration config = null,
                                              ILinkResolver resolver = null)
        {
            return new HalClient(
                config ?? new HalKitConfiguration(new Uri("http://foo.api.io")),
                conn ?? new FakeHttpConnection(),
                resolver ?? new Mock<ILinkResolver>(MockBehavior.Loose).Object);
        }

        public class TheConfigurationProperty
        {
            [Fact]
            public void ShouldReturnTheGivenConfiguration()
            {
                var expectedConfig = new HalKitConfiguration(new Uri("http://foo.api.io"));
                var client = CreateClient(config: expectedConfig);

                var actualConfig = client.Configuration;

                Assert.Same(expectedConfig, actualConfig);
            }
        }

        public class TheHttpConnectionProperty
        {
            [Fact]
            public void ShouldReturnTheGivenConnection()
            {
                var expectedConnection = new FakeHttpConnection();
                var client = CreateClient(conn: expectedConnection);

                var actualConnection = client.HttpConnection;

                Assert.Same(expectedConnection, actualConnection);
            }
        }

        public class TheGetRootAsyncMethod
        {
            [Fact]
            public async void ShouldPassLinkWithRootEndpointAsItsHRefToTheLinkResolver()
            {
                var expectedHRef = "http://api.com/root";
                var mockResolver = new Mock<ILinkResolver>(MockBehavior.Loose);
                mockResolver.Setup(r => r.ResolveLink(It.Is<Link>(l => l.HRef == expectedHRef),
                                                      It.IsAny<IDictionary<string, string>>()))
                            .Returns(new Uri("http://uri.com"))
                            .Verifiable();
                var client = CreateClient(resolver: mockResolver.Object,
                                          config: new HalKitConfiguration(new Uri(expectedHRef)));

                await client.GetRootAsync(null, null);

                mockResolver.Verify();
            }

            [Fact]
            public async void ShouldPassGivenParametersToLinkResolver()
            {
                await VerifyGivenParametersArePassedToLinkResolver(
                    (client, parameters) => client.GetRootAsync(parameters, null));
            }

            [Fact]
            public async void ShouldSendRequestToUriReturnedByLinkResolver()
            {
                await VerifyUriReturnedByLinkResolverIsPassedToHttpConnection<RootResource>(
                    client => client.GetRootAsync(
                                null,
                                new Dictionary<string, IEnumerable<string>>{{"Foo", new[] {"Bar"}}}));
            }

            [Fact]
            public async void ShouldSendRequestWithHttpGetMethod()
            {
                await VerifyHttpMethodIsPassedToHttpConnection<RootResource>(
                    HttpMethod.Get,
                    client => client.GetRootAsync(null, null));
            }

            [Fact]
            public async void ShouldSendRequestWithNullBody()
            {
                await VerifyBodyIsPassedToHttpConnection<RootResource>(
                    null,
                    client => client.GetRootAsync(null, null));
            }

            [Fact]
            public async void ShouldSendRequestWithAcceptHeaderSetToHalJson()
            {
                await VerifyAcceptHalJsonHeaderIsPassedToHttpConnection<RootResource>(
                    client => client.GetRootAsync(null, null));
            }

            [Fact]
            public async void ShouldSendRequestWithGivenHeaderSetToHalJson()
            {
                await VerifyGivenHeaderIsPassedToHttpConnection<RootResource>(
                    (client, headers) => client.GetRootAsync(null, headers));
            }

            [Fact]
            public async void ShouldReturnRootResourceReturnedByHttpConnection()
            {
                await VerifyBodyOfApiResponseIsReturned(
                    client => client.GetRootAsync(null, null));
            }
        }

        public class TheGetAsyncMethod
        {
            [Fact]
            public async void ShouldPassGivenLinkToTheLinkResolver()
            {
                await VerifyGivenLinkIsPassedToLinkResolver(
                    (client, link) => client.GetAsync<Resource>(link, null, null));
            }

            [Fact]
            public async void ShouldPassGivenParametersToLinkResolver()
            {
                await VerifyGivenParametersArePassedToLinkResolver(
                    (client, parameters) => client.GetAsync<Resource>(new Link(), parameters, null));
            }

            [Fact]
            public async void ShouldSendRequestToUriReturnedByLinkResolver()
            {
                await VerifyUriReturnedByLinkResolverIsPassedToHttpConnection<Resource>(
                    client => client.GetAsync<Resource>(
                                new Link(),
                                null,
                                new Dictionary<string, IEnumerable<string>> { { "Foo", new[] { "Bar" } } }));
            }

            [Fact]
            public async void ShouldSendRequestWithHttpGetMethod()
            {
                await VerifyHttpMethodIsPassedToHttpConnection<Resource>(
                    HttpMethod.Get,
                    client => client.GetAsync<Resource>(new Link(), null, null));
            }

            [Fact]
            public async void ShouldSendRequestWithNullBody()
            {
                await VerifyBodyIsPassedToHttpConnection<Resource>(
                    null,
                    client => client.GetAsync<Resource>(new Link(), null, null));
            }

            [Fact]
            public async void ShouldSendRequestWithAcceptHeaderSetToHalJson()
            {
                await VerifyAcceptHalJsonHeaderIsPassedToHttpConnection<Resource>(
                    client => client.GetAsync<Resource>(new Link(), null, null));
            }

            [Fact]
            public async void ShouldSendRequestWithGivenHeaderSetToHalJson()
            {
                await VerifyGivenHeaderIsPassedToHttpConnection<Resource>(
                    (client, headers) => client.GetAsync<Resource>(new Link(), null, headers));
            }

            [Fact]
            public async void ShouldReturnBodyOfResponseReturnedByHttpConnection()
            {
                await VerifyBodyOfApiResponseIsReturned(
                    client => client.GetAsync<Resource>(new Link(), null, null));
            }
        }

        public class ThePostAsyncMethod
        {
            [Fact]
            public async void ShouldPassGivenLinkToTheLinkResolver()
            {
                await VerifyGivenLinkIsPassedToLinkResolver(
                    (client, link) => client.PostAsync<Resource>(link, new object(), null, null));
            }

            [Fact]
            public async void ShouldPassGivenParametersToLinkResolver()
            {
                await VerifyGivenParametersArePassedToLinkResolver(
                    (client, parameters) => client.PostAsync<Resource>(new Link(), new object(), parameters, null));
            }

            [Fact]
            public async void ShouldSendRequestToUriReturnedByLinkResolver()
            {
                await VerifyUriReturnedByLinkResolverIsPassedToHttpConnection<Resource>(
                    client => client.PostAsync<Resource>(
                                new Link(),
                                new object(),
                                null,
                                new Dictionary<string, IEnumerable<string>> { { "Foo", new[] { "Bar" } } }));
            }

            [Fact]
            public async void ShouldSendRequestWithHttpPostMethod()
            {
                await VerifyHttpMethodIsPassedToHttpConnection<Resource>(
                    HttpMethod.Post,
                    client => client.PostAsync<Resource>(new Link(), new object(), null, null));
            }

            [Fact]
            public async void ShouldSendRequestWithNullBody()
            {
                var expectedBody = new object();
                await VerifyBodyIsPassedToHttpConnection<Resource>(
                    expectedBody,
                    client => client.PostAsync<Resource>(new Link(), expectedBody, null, null));
            }

            [Fact]
            public async void ShouldSendRequestWithAcceptHeaderSetToHalJson()
            {
                await VerifyAcceptHalJsonHeaderIsPassedToHttpConnection<Resource>(
                    client => client.PostAsync<Resource>(new Link(), new object(), null, null));
            }

            [Fact]
            public async void ShouldSendRequestWithGivenHeaderSetToHalJson()
            {
                await VerifyGivenHeaderIsPassedToHttpConnection<Resource>(
                    (client, headers) => client.PostAsync<Resource>(
                        new Link(),
                        new object(),
                        null,
                        headers));
            }

            [Fact]
            public async void ShouldReturnBodyOfResponseReturnedByHttpConnection()
            {
                await VerifyBodyOfApiResponseIsReturned(
                    client => client.PostAsync<Resource>(new Link(), new object(), null, null));
            }
        }

        public class ThePutAsyncMethod
        {
            [Fact]
            public async void ShouldPassGivenLinkToTheLinkResolver()
            {
                await VerifyGivenLinkIsPassedToLinkResolver(
                    (client, link) => client.PutAsync<Resource>(link, new object(), null, null));
            }

            [Fact]
            public async void ShouldPassGivenParametersToLinkResolver()
            {
                await VerifyGivenParametersArePassedToLinkResolver(
                    (client, parameters) => client.PutAsync<Resource>(new Link(), new object(), parameters, null));
            }

            [Fact]
            public async void ShouldSendRequestToUriReturnedByLinkResolver()
            {
                await VerifyUriReturnedByLinkResolverIsPassedToHttpConnection<Resource>(
                    client => client.PutAsync<Resource>(
                                new Link(),
                                new object(),
                                null,
                                new Dictionary<string, IEnumerable<string>> { { "Foo", new[] { "Bar" } } }));
            }

            [Fact]
            public async void ShouldSendRequestWithHttpPutMethod()
            {
                await VerifyHttpMethodIsPassedToHttpConnection<Resource>(
                    HttpMethod.Put,
                    client => client.PutAsync<Resource>(new Link(), new object(), null, null));
            }

            [Fact]
            public async void ShouldSendRequestWithNullBody()
            {
                var expectedBody = new object();
                await VerifyBodyIsPassedToHttpConnection<Resource>(
                    expectedBody,
                    client => client.PutAsync<Resource>(new Link(), expectedBody, null, null));
            }

            [Fact]
            public async void ShouldSendRequestWithAcceptHeaderSetToHalJson()
            {
                await VerifyAcceptHalJsonHeaderIsPassedToHttpConnection<Resource>(
                    client => client.PutAsync<Resource>(new Link(), new object(), null, null));
            }

            [Fact]
            public async void ShouldSendRequestWithGivenHeaderSetToHalJson()
            {
                await VerifyGivenHeaderIsPassedToHttpConnection<Resource>(
                    (client, headers) => client.PutAsync<Resource>(
                        new Link(),
                        new object(),
                        null,
                        headers));
            }

            [Fact]
            public async void ShouldReturnBodyOfResponseReturnedByHttpConnection()
            {
                await VerifyBodyOfApiResponseIsReturned(
                    client => client.PutAsync<Resource>(new Link(), new object(), null, null));
            }
        }

        public class ThePatchAsyncMethod
        {
            [Fact]
            public async void ShouldPassGivenLinkToTheLinkResolver()
            {
                await VerifyGivenLinkIsPassedToLinkResolver(
                    (client, link) => client.PatchAsync<Resource>(link, new object(), null, null));
            }

            [Fact]
            public async void ShouldPassGivenParametersToLinkResolver()
            {
                await VerifyGivenParametersArePassedToLinkResolver(
                    (client, parameters) => client.PatchAsync<Resource>(new Link(), new object(), parameters, null));
            }

            [Fact]
            public async void ShouldSendRequestToUriReturnedByLinkResolver()
            {
                await VerifyUriReturnedByLinkResolverIsPassedToHttpConnection<Resource>(
                    client => client.PatchAsync<Resource>(
                                new Link(),
                                new object(),
                                null,
                                new Dictionary<string, IEnumerable<string>> { { "Foo", new[] { "Bar" } } }));
            }

            [Fact]
            public async void ShouldSendRequestWithHttpPatchMethod()
            {
                await VerifyHttpMethodIsPassedToHttpConnection<Resource>(
                    new HttpMethod("Patch"),
                    client => client.PatchAsync<Resource>(new Link(), new object(), null, null));
            }

            [Fact]
            public async void ShouldSendRequestWithNullBody()
            {
                var expectedBody = new object();
                await VerifyBodyIsPassedToHttpConnection<Resource>(
                    expectedBody,
                    client => client.PatchAsync<Resource>(new Link(), expectedBody, null, null));
            }

            [Fact]
            public async void ShouldSendRequestWithAcceptHeaderSetToHalJson()
            {
                await VerifyAcceptHalJsonHeaderIsPassedToHttpConnection<Resource>(
                    client => client.PatchAsync<Resource>(new Link(), new object(), null, null));
            }

            [Fact]
            public async void ShouldSendRequestWithGivenHeaderSetToHalJson()
            {
                await VerifyGivenHeaderIsPassedToHttpConnection<Resource>(
                    (client, headers) => client.PatchAsync<Resource>(
                        new Link(),
                        new object(),
                        null,
                        headers));
            }

            [Fact]
            public async void ShouldReturnBodyOfResponseReturnedByHttpConnection()
            {
                await VerifyBodyOfApiResponseIsReturned(
                    client => client.PatchAsync<Resource>(new Link(), new object(), null, null));
            }
        }

        public class TheDeleteAsyncMethod
        {
            [Fact]
            public async void ShouldPassGivenLinkToTheLinkResolver()
            {
                await VerifyGivenLinkIsPassedToLinkResolver(
                    (client, link) => client.DeleteAsync(link, null, null));
            }

            [Fact]
            public async void ShouldPassGivenParametersToLinkResolver()
            {
                await VerifyGivenParametersArePassedToLinkResolver(
                    (client, parameters) => client.DeleteAsync(new Link(), parameters, null));
            }

            [Fact]
            public async void ShouldSendRequestToUriReturnedByLinkResolver()
            {
                await VerifyUriReturnedByLinkResolverIsPassedToHttpConnection<object>(
                    client => client.DeleteAsync(
                                new Link(),
                                null,
                                new Dictionary<string, IEnumerable<string>> { { "Foo", new[] { "Bar" } } }));
            }

            [Fact]
            public async void ShouldSendRequestWithHttpDeleteMethod()
            {
                await VerifyHttpMethodIsPassedToHttpConnection<object>(
                    HttpMethod.Delete,
                    client => client.DeleteAsync(new Link(), null, null));
            }

            [Fact]
            public async void ShouldSendRequestWithNullBody()
            {
                await VerifyBodyIsPassedToHttpConnection<object>(
                    null,
                    client => client.DeleteAsync(new Link(), null, null));
            }

            [Fact]
            public async void ShouldSendRequestWithAcceptHeaderSetToHalJson()
            {
                await VerifyAcceptHalJsonHeaderIsPassedToHttpConnection<object>(
                    client => client.DeleteAsync(new Link(), null, null));
            }

            [Fact]
            public async void ShouldSendRequestWithGivenHeaderSetToHalJson()
            {
                await VerifyGivenHeaderIsPassedToHttpConnection<object>(
                    (client, headers) => client.DeleteAsync(new Link(), null, headers));
            }

            [Fact]
            public async void ShouldReturnResponseReturnedByHttpConnection()
            {
                var expectedResponse = new ApiResponse<object>();
                var mockConnection = new Mock<IHttpConnection>(MockBehavior.Loose);
                mockConnection.Setup(c => c.SendRequestAsync<object>(
                                            It.IsAny<Uri>(),
                                            It.IsAny<HttpMethod>(),
                                            It.IsAny<object>(),
                                            It.IsAny<IDictionary<string, IEnumerable<string>>>()))
                             .Returns(Task.FromResult<IApiResponse<object>>(expectedResponse))
                             .Verifiable();
                var client = CreateClient(conn: mockConnection.Object);

                var actualResponse = await client.DeleteAsync(new Link(), null, null);

                Assert.Same(expectedResponse, actualResponse);
            }
        }

        private static async Task VerifyGivenParametersArePassedToLinkResolver<T>(
            Func<HalClient, IDictionary<string, string>, Task<T>> clientAction)
        {
            var expectedParams = new Dictionary<string, string>();
            var mockResolver = new Mock<ILinkResolver>(MockBehavior.Loose);
            mockResolver.Setup(r => r.ResolveLink(It.IsAny<Link>(), expectedParams))
                        .Returns(new Uri("http://host.com/path"))
                        .Verifiable();
            var client = CreateClient(resolver: mockResolver.Object);

            await clientAction(client, expectedParams);

            mockResolver.Verify();
        }

        private static async Task VerifyGivenLinkIsPassedToLinkResolver(
            Func<HalClient, Link, Task> clientAction)
        {
            var expectedLink = new Link();
            var mockResolver = new Mock<ILinkResolver>(MockBehavior.Loose);
            mockResolver.Setup(r => r.ResolveLink(expectedLink, It.IsAny<IDictionary<string, string>>()))
                        .Returns(new Uri("http://host.com/path"))
                        .Verifiable();
            var client = CreateClient(resolver: mockResolver.Object);

            await clientAction(client, expectedLink);

            mockResolver.Verify();
        }

        private static async Task VerifyUriReturnedByLinkResolverIsPassedToHttpConnection<T>(
            Func<HalClient, Task> clientAction)
        {
            var expectedUri = new Uri("http://host.com/path");
            var mockResolver = new Mock<ILinkResolver>(MockBehavior.Loose);
            var mockConnection = new Mock<IHttpConnection>(MockBehavior.Loose);
            mockResolver.Setup(r => r.ResolveLink(It.IsAny<Link>(), It.IsAny<IDictionary<string, string>>()))
                        .Returns(expectedUri);
            mockConnection.Setup(c => c.SendRequestAsync<T>(
                                        expectedUri,
                                        It.IsAny<HttpMethod>(),
                                        It.IsAny<object>(),
                                        It.IsAny<IDictionary<string, IEnumerable<string>>>()))
                         .Returns(Task.FromResult<IApiResponse<T>>(new ApiResponse<T>()))
                         .Verifiable();
            var client = CreateClient(resolver: mockResolver.Object, conn: mockConnection.Object);

            await clientAction(client);

            mockConnection.Verify();
        }

        private static async Task VerifyHttpMethodIsPassedToHttpConnection<T>(
            HttpMethod expectedMethod,
            Func<HalClient, Task> clientAction)
        {
            var mockConnection = new Mock<IHttpConnection>(MockBehavior.Loose);
            mockConnection.Setup(c => c.SendRequestAsync<T>(
                                        It.IsAny<Uri>(),
                                        expectedMethod,
                                        It.IsAny<object>(),
                                        It.IsAny<IDictionary<string, IEnumerable<string>>>()))
                         .Returns(Task.FromResult<IApiResponse<T>>(new ApiResponse<T>()))
                         .Verifiable();
            var client = CreateClient(conn: mockConnection.Object);

            await clientAction(client);

            mockConnection.Verify();
        }

        private static async Task VerifyBodyIsPassedToHttpConnection<T>(
            object expectedBody,
            Func<HalClient, Task> clientAction)
        {
            var mockConnection = new Mock<IHttpConnection>(MockBehavior.Loose);
            mockConnection.Setup(c => c.SendRequestAsync<T>(
                                        It.IsAny<Uri>(),
                                        It.IsAny<HttpMethod>(),
                                        expectedBody,
                                        It.IsAny<IDictionary<string, IEnumerable<string>>>()))
                         .Returns(Task.FromResult<IApiResponse<T>>(new ApiResponse<T>()))
                         .Verifiable();
            var client = CreateClient(conn: mockConnection.Object);

            await clientAction(client);

            mockConnection.Verify();
        }

        private static async Task VerifyAcceptHalJsonHeaderIsPassedToHttpConnection<T>(
            Func<HalClient, Task> clientAction)
        {
            var expectedAcceptHeader = "application/hal+json";
            IDictionary<string, IEnumerable<string>> actualHeaders = null;
            var mockConnection = new Mock<IHttpConnection>(MockBehavior.Loose);
            mockConnection.Setup(c => c.SendRequestAsync<T>(
                                        It.IsAny<Uri>(),
                                        It.IsAny<HttpMethod>(),
                                        It.IsAny<object>(),
                                        It.IsAny<IDictionary<string, IEnumerable<string>>>()))
                         .Callback((Uri uri, HttpMethod method, object body, IDictionary<string, IEnumerable<string>> headers) =>
                            actualHeaders = headers)
                         .Returns(Task.FromResult<IApiResponse<T>>(new ApiResponse<T>()));
            var client = CreateClient(conn: mockConnection.Object);

            await clientAction(client);

            Assert.Equal(expectedAcceptHeader, actualHeaders["Accept"].Single());
        }

        private static async Task VerifyGivenHeaderIsPassedToHttpConnection<T>(
            Func<HalClient, IDictionary<string, IEnumerable<string>>, Task> clientAction)
        {
            var expectedHeaderKey = "Custom Header";
            var expectedHeaderValue = new[] { "Custom Value 1", "Custom Value 2" };
            IDictionary<string, IEnumerable<string>> actualHeaders = null;
            var mockConnection = new Mock<IHttpConnection>(MockBehavior.Loose);
            mockConnection.Setup(c => c.SendRequestAsync<T>(
                                        It.IsAny<Uri>(),
                                        It.IsAny<HttpMethod>(),
                                        It.IsAny<object>(),
                                        It.IsAny<IDictionary<string, IEnumerable<string>>>()))
                         .Callback((Uri uri, HttpMethod method, object body, IDictionary<string, IEnumerable<string>> headers) =>
                            actualHeaders = headers)
                         .Returns(Task.FromResult<IApiResponse<T>>(new ApiResponse<T>()));
            var client = CreateClient(conn: mockConnection.Object);

            await clientAction(
                client,
                new Dictionary<string, IEnumerable<string>>
                    {
                        {expectedHeaderKey, expectedHeaderValue}
                    });

            Assert.Same(expectedHeaderValue, actualHeaders[expectedHeaderKey]);
        }

        private static async Task VerifyBodyOfApiResponseIsReturned<T>(
            Func<HalClient, Task<T>> clientAction)
            where T : new()
        {
            var expectedResponse = new T();
            var mockConnection = new Mock<IHttpConnection>(MockBehavior.Loose);
            mockConnection.Setup(c => c.SendRequestAsync<T>(
                                        It.IsAny<Uri>(),
                                        It.IsAny<HttpMethod>(),
                                        It.IsAny<object>(),
                                        It.IsAny<IDictionary<string, IEnumerable<string>>>()))
                         .Returns(Task.FromResult<IApiResponse<T>>(new ApiResponse<T> { BodyAsObject = expectedResponse }));
            var client = CreateClient(conn: mockConnection.Object);

            var actualResponse = await clientAction(client);

            Assert.Same(expectedResponse, actualResponse);
        }
    }
}
