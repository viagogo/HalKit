using System;
using System.Net.Http;
using Moq;
using System.Net;
using Xunit;

namespace HalKit.Tests
{
    public class HttpClientFactoryTests
    {
        private static HttpClientFactory CreateFactory(HttpClientHandler clientHndl = null)
        {
            return new HttpClientFactory(
                clientHndl ?? new Mock<HttpClientHandler>(MockBehavior.Loose).Object);
        }

        public class Constructor
        {
            [Fact]
            public void ShouldSetTheClientHandlerToSupportAutomaticGzipAndDeflateCompression_WhenItSupportsAutoDecompression()
            {
                var expectedDecompressionMethods = DecompressionMethods.GZip | DecompressionMethods.Deflate;
                var mockClientHandler = new Mock<HttpClientHandler>(MockBehavior.Loose);
                mockClientHandler.Setup(h => h.SupportsAutomaticDecompression).Returns(true);

                CreateFactory(clientHndl: mockClientHandler.Object);

                Assert.Equal(expectedDecompressionMethods, mockClientHandler.Object.AutomaticDecompression);
            }

            [Fact]
            public void ShouldNotSetTheClientHandlerAutomaticDecompressionToNone_WhenItDoesntSupportAutoDecompression()
            {
                var expectedDecompressionMethods = DecompressionMethods.None;
                var mockClientHandler = new Mock<HttpClientHandler>(MockBehavior.Loose);
                mockClientHandler.Setup(h => h.SupportsAutomaticDecompression).Returns(false);

                CreateFactory(clientHndl: mockClientHandler.Object);

                Assert.Equal(expectedDecompressionMethods, mockClientHandler.Object.AutomaticDecompression);
            }
        }
    }
}

