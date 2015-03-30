using System;
using System.Collections.Generic;
using System.Linq;
using HalKit.Models;
using HalKit.Services;
using Xunit;

namespace HalKit.Tests.Services
{
    public class LinkResolverTests
    {
        private static LinkResolver CreateResolver()
        {
            return new LinkResolver();
        }

        public class TheResolveLinkMethod
        {
            private const string Level1Parameters = "var=value;hello=Hello World!";
            private const string Level2Parameters = "var=value;hello=Hello World!;path=/foo/bar";
            private const string Level3Parameters = "var=value;hello=Hello World!;empty=;path=/foo/bar;x=1024;y=768";

            [Theory]
            // Non-templated Examples
            [InlineData("http://host.com/path", false, null, "http://host.com/path")]
            [InlineData("http://host.com/path", false, "", "http://host.com/path")]
            [InlineData("http://host.com/path", false, "foo=foovalue;bar=barvalue", "http://host.com/path?foo=foovalue&bar=barvalue")]
            [InlineData("http://host.com/path?existing=true", false, "foo=foovalue", "http://host.com/path?existing=true&foo=foovalue")]
            [InlineData("http://host.com/path", false, "foo=foo value!", "http://host.com/path?foo=foo%20value%21")]

            // Spec Examples (See https://github.com/uri-templates/uritemplate-test/blob/master/spec-examples.json)
            // Level 1 Examples
            [InlineData("http://host.com/path/{var}", true, Level1Parameters, "http://host.com/path/value")]
            [InlineData("http://host.com/path/{hello}", true, Level1Parameters, "http://host.com/path/Hello%20World%21")]

            // Level 2 Examples
            [InlineData("http://host.com/path/{+var}", true, Level2Parameters, "http://host.com/path/value")]
            [InlineData("http://host.com/path/{+hello}", true, Level2Parameters, "http://host.com/path/Hello%20World!")]
            [InlineData("http://host.com/path{+path}/here", true, Level2Parameters, "http://host.com/path/foo/bar/here")]
            [InlineData("http://host.com/path/here?ref={+path}", true, Level2Parameters, "http://host.com/path/here?ref=/foo/bar")]

            // Level 3 Examples
            [InlineData("http://host.com/path/map?{x,y}", true, Level3Parameters, "http://host.com/path/map?1024,768")]
            [InlineData("http://host.com/path/{x,hello,y}", true, Level3Parameters, "http://host.com/path/1024,Hello%20World%21,768")]
            [InlineData("http://host.com/path/{+x,hello,y}", true, Level3Parameters, "http://host.com/path/1024,Hello%20World!,768")]
            [InlineData("http://host.com/path/{+path,x}/here", true, Level3Parameters, "http://host.com/path//foo/bar,1024/here")]
            [InlineData("http://host.com/path/{#x,hello,y}", true, Level3Parameters, "http://host.com/path/#1024,Hello%20World!,768")]
            [InlineData("http://host.com/path/{#path,x}/here", true, Level3Parameters, "http://host.com/path/#/foo/bar,1024/here")]
            [InlineData("http://host.com/path/X{.var}", true, Level3Parameters, "http://host.com/path/X.value")]
            [InlineData("http://host.com/path/X{.x,y}", true, Level3Parameters, "http://host.com/path/X.1024.768")]
            [InlineData("http://host.com/path{/var}", true, Level3Parameters, "http://host.com/path/value")]
            [InlineData("http://host.com/path{/var,x}/here", true, Level3Parameters, "http://host.com/path/value/1024/here")]
            [InlineData("http://host.com/path/{;x,y}", true, Level3Parameters, "http://host.com/path/;x=1024;y=768")]
            [InlineData("http://host.com/path/{;x,y,empty}", true, Level3Parameters, "http://host.com/path/;x=1024;y=768;empty")]
            [InlineData("http://host.com/path{?x,y}", true, Level3Parameters, "http://host.com/path?x=1024&y=768")]
            [InlineData("http://host.com/path{?x,y,empty}", true, Level3Parameters, "http://host.com/path?x=1024&y=768&empty=")]
            [InlineData("http://host.com/path?fixed=yes{&x}", true, Level3Parameters, "http://host.com/path?fixed=yes&x=1024")]
            [InlineData("http://host.com/path{&x,y,empty}", true, Level3Parameters, "http://host.com/path&x=1024&y=768&empty=")]
            public void ShouldResolveAppropriateUriForGivenLinkAndParameters(
                string href,
                bool isTemplated,
                string parametersText,
                string expectedUrl)
            {
                var expectedUri = new Uri(expectedUrl);
                var resolver = CreateResolver();

                var actualUri = resolver.ResolveLink(
                                    new Link {HRef = href, IsTemplated = isTemplated},
                                    GetParameters(parametersText));

                Assert.Equal(expectedUri, actualUri);
            }

            private IDictionary<string, string> GetParameters(string parametersText)
            {
                if (parametersText == null)
                {
                    return new Dictionary<string, string>();
                }

                return parametersText
                        .Split(new[] {';'}, StringSplitOptions.RemoveEmptyEntries)
                        .ToDictionary(kv => kv.Split('=')[0], kv => kv.Split('=')[1]);
            }
        }
    }
}
