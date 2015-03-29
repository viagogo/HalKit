using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;

namespace HalKit.Http
{
    public class HttpClientFactory : IHttpClientFactory
    {
        private readonly HttpClientHandler _clientHandler;

        public HttpClientFactory()
            : this(new HttpClientHandler())
        {
        }

        public HttpClientFactory(HttpClientHandler clientHandler)
        {
            Requires.ArgumentNotNull(clientHandler, "clientHandler");

            if (clientHandler.SupportsAutomaticDecompression)
            {
                clientHandler.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
            }
            _clientHandler = clientHandler;
        }

        public HttpClient CreateClient(IEnumerable<DelegatingHandler> handlers)
        {
            HttpMessageHandler pipeline = _clientHandler;
            foreach (var handler in handlers.Reverse())
            {
                if (handler == null)
                {
                    throw new ArgumentException("Handlers contains null handler", "handlers");
                }

                handler.InnerHandler = pipeline;
                pipeline = handler;
            }

            return new HttpClient(pipeline);
        }
    }
}

