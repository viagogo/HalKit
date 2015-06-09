using HalKit.Json;
using HalKit.Models.Response;
using Newtonsoft.Json;
using System.Collections.Generic;
using Xunit;

namespace HalKit.Tests.Json
{
    public class ResourceConverterTests
    {
        private const string FooResourceJson =
@"{
  ""_links"": {
    ""curies"": [
      {""name"": ""docs"",""href"": ""http://foo.com/rels/{rel}""}
    ],
    ""docs:some_resource"": {
      ""href"": ""http://api.com/resources/5"",
      ""title"": ""Some Resource"",
      ""templated"": true
    },
    ""docs:bars"": [
        {
          ""href"": ""http://api.com/bars/1"",
          ""title"": ""Bar 1"",
          ""templated"": false
        },
        {
          ""href"": ""http://api.com/bars/2"",
          ""title"": ""Bar 2"",
          ""templated"": true
        },
    ]
  },
  ""_embedded"": {
    ""docs:some_resource"": {""message"": ""Expected embedded message""}
  }
}";
        public class FooResource : Resource
        {
            [Embedded("docs:some_resource")]
            public dynamic EmbeddedProperty { get; set; }

            [Rel("docs:some_resource")]
            public Link LinkProperty { get; set; }

            [Rel("docs:bars")]
            public IList<Link> LinkArrayProperty { get; set; }
        }

        public class TheCanReadProperty
        {
            [Fact]
            public void ShouldReturnTrue()
            {
                var converter = new ResourceConverter();

                Assert.True(converter.CanRead);
            }
        }
        
        public class TheCanWriteProperty
        {
            [Fact]
            public void ShouldReturnTrue()
            {
                var converter = new ResourceConverter();

                Assert.True(converter.CanWrite);
            }
        }

        public class TheCanConvertMethod
        {
            [Fact]
            public void ShouldReturnTrue_WhenGivenTypeIsDerivedFromResource()
            {
                var converter = new ResourceConverter();

                var result = converter.CanConvert(typeof(FooResource));

                Assert.True(result);
            }

            [Fact]
            public void ShouldReturnFalse_WhenGivenTypeIsNotDerivedFromResource()
            {
                var converter = new ResourceConverter();

                var result = converter.CanConvert(typeof(Link));

                Assert.False(result);
            }
        }

        public class TheReadJsonMethod
        {
            [Fact]
            public void ShouldDeserializeEmbeddedProperty()
            {
                var converter = new ResourceConverter();

                var foo = JsonConvert.DeserializeObject<FooResource>(FooResourceJson, converter);

                Assert.Equal("Expected embedded message", (string)foo.EmbeddedProperty.message);
            }

            [Fact]
            public void ShouldDeserializeLinkProperty()
            {
                var converter = new ResourceConverter();

                var foo = JsonConvert.DeserializeObject<FooResource>(FooResourceJson, converter);

                Assert.Equal("http://api.com/resources/5", foo.LinkProperty.HRef);
                Assert.Equal("Some Resource", foo.LinkProperty.Title);
                Assert.True(foo.LinkProperty.IsTemplated);
            }

            [Fact]
            public void ShouldDeserializeLinksArrayProperty()
            {
                var converter = new ResourceConverter();

                var foo = JsonConvert.DeserializeObject<FooResource>(FooResourceJson, converter);

                Assert.Equal(2, foo.LinkArrayProperty.Count);
                Assert.Equal("http://api.com/bars/1", foo.LinkArrayProperty[0].HRef);
                Assert.Equal("Bar 1", foo.LinkArrayProperty[0].Title);
                Assert.False(foo.LinkArrayProperty[0].IsTemplated);
                Assert.Equal("http://api.com/bars/2", foo.LinkArrayProperty[1].HRef);
                Assert.Equal("Bar 2", foo.LinkArrayProperty[1].Title);
                Assert.True(foo.LinkArrayProperty[1].IsTemplated);
            }
        }
    }
}
