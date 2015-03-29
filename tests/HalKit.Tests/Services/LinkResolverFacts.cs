using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HalKit.Models;
using HalKit.Services;
using Xunit;

namespace HalKit.Tests.Services
{
    public class LinkResolverFacts
    {
        private static LinkResolver CreateResolver()
        {
            return new LinkResolver();
        }

        public class TheResolveLinkMethod
        {
            public static readonly object[] NullAndEmptyParameters =
            {
                new object[] {null},
                new object[] {new Dictionary<string, string>()}
            };

            [Theory]
            [MemberData("NullAndEmptyParameters")]
            public void ShouldReturnsUriWithUnchangedLinkHRef_WhenParametersIsNullOrEmpty(
                Dictionary<string, string> parameters)
            {
                var expectedUri = new Uri("https://host.com");
                var resolver = CreateResolver();

                var actualUri = resolver.ResolveLink(
                                    new Link { HRef = expectedUri.OriginalString },
                                    parameters);

                Assert.Equal(expectedUri, actualUri);
            }
        }
    }
}
