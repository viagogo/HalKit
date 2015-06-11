using HalKit.Json;
using HalKit.Models.Response;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using Xunit;

namespace HalKit.Tests.Json
{
    public class ResourceConverterTests
    {
        private const string FooResourceJson =
@"{
  ""normal_property"": {
    ""property_one"": ""one"",
    ""property_two"": [
      2,
      3
    ]
  },
  ""_links"": {
    ""self"": {
      ""href"": ""http://api.com/self"",
      ""title"": ""Self"",
      ""templated"": false
    },
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
      }
    ]
  },
  ""_embedded"": {
    ""docs:some_resource"": {
      ""message"": ""Expected embedded message""
    }
  }
}";
        public class FooResource : Resource
        {
            [JsonProperty(PropertyName = "normal_property")]
            public dynamic NormalProperty { get; set; }

            [Embedded("docs:some_resource")]
            public dynamic EmbeddedProperty { get; set; }

            [Embedded("docs:not_in_json")]
            public object NullEmbeddedProperty { get; set; }

            [Rel("docs:some_resource")]
            public Link LinkProperty { get; set; }

            [Rel("docs:not_in_json")]
            public Link NullLinkProperty { get; set; }

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
            public void ShouldDeserializeEmbeddedPropertyToNull_WhenTheRelIsNotInTheJson()
            {
                var converter = new ResourceConverter();

                var foo = JsonConvert.DeserializeObject<FooResource>(FooResourceJson, converter);

                Assert.Null(foo.NullEmbeddedProperty);
            }

            [Theory]
            [InlineData(@"{""normal_property"": {}}")]
            [InlineData(@"{""normal_property"": {}, ""_embedded"": {""e"": {}}}")]
            [InlineData(@"{""normal_property"": {}, ""_links"": {""l"": {}}}")]
            public void ShouldDeserializeObject_WhenJsonDoesNotReturnLinksOrEmbeddedProperties(string json)
            {
                var converter = new ResourceConverter();

                var foo = JsonConvert.DeserializeObject<FooResource>(json, converter);

                Assert.NotNull(foo.NormalProperty);
            }

            [Fact]
            public void ShouldDeserializeSelfLinkProperty()
            {
                var converter = new ResourceConverter();

                var foo = JsonConvert.DeserializeObject<FooResource>(FooResourceJson, converter);

                Assert.Equal("http://api.com/self", foo.SelfLink.HRef);
                Assert.Equal("Self", foo.SelfLink.Title);
                Assert.False(foo.SelfLink.IsTemplated);
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
            public void ShouldDeserializeLinkPropertyToNull_WhenTheRelIsNotInTheJson()
            {
                var converter = new ResourceConverter();

                var foo = JsonConvert.DeserializeObject<FooResource>(FooResourceJson, converter);

                Assert.Null(foo.NullLinkProperty);
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

        public class TheWriteJsonMethod
        {
            [Fact]
            public void ShouldSerializeResourceEmbeddedProperties()
            {
                // Deserialize and then serialize to get rid of all of the new lines
                var expectedJson = JsonConvert.SerializeObject(JsonConvert.DeserializeObject(FooResourceJson));
                var converter = new ResourceConverter();

                var actualJson = JsonConvert.SerializeObject(
                                    new FooResource
                                    {
                                        NormalProperty = new { property_one = "one", property_two = new[] { 2, 3 }},
                                        EmbeddedProperty = new { message = "Expected embedded message" },
                                        NullEmbeddedProperty = null,
                                        SelfLink = new Link { HRef = "http://api.com/self", Title = "Self", IsTemplated = false },
                                        LinkProperty = new Link { HRef = "http://api.com/resources/5", Title = "Some Resource", IsTemplated = true },
                                        NullLinkProperty = null,
                                        LinkArrayProperty = new[]
                                        {
                                            new Link {HRef = "http://api.com/bars/1", Title = "Bar 1", IsTemplated = false},
                                            new Link {HRef = "http://api.com/bars/2", Title = "Bar 2", IsTemplated = true},
                                        }
                                    },
                                    new JsonSerializerSettings
                                    {
                                        Converters = new[] {converter},
                                        Formatting = Formatting.None
                                    });

                Assert.Equal(expectedJson, actualJson);
            }
        }
    }
}
