using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace HalKit.Tests
{
    public class FakeDelegatingHandler : DelegatingHandler
    {
        private readonly HttpResponseMessage _resp;

        public FakeDelegatingHandler(HttpResponseMessage resp = null)
        {
            _resp = resp ?? new HttpResponseMessage();
        }

        protected override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            return Task.FromResult(_resp);
        }
    }
}

