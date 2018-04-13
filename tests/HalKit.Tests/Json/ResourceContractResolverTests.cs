using HalKit.Json;
using HalKit.Models.Response;
using Newtonsoft.Json;
using System.Collections.Generic;
using Xunit;

namespace HalKit.Tests.Json
{
    public class ResourceContractResolverTests
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
    ],
    ""docs:some_resource"": {
      ""href"": ""http://api.com/resources/5"",
      ""title"": ""Some Resource"",
      ""templated"": true
    }
  },
  ""_embedded"": {
    ""docs:some_resource"": {
      ""message"": ""Expected embedded message""
    },
    ""docs:foo_2"":{""normal_property"": {
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
    ],
    ""docs:some_resource"": {
      ""href"": ""http://api.com/resources/5"",
      ""title"": ""Some Resource"",
      ""templated"": true
    }
  },
  ""_embedded"": {
    ""docs:some_resource"": {
      ""message"": ""Expected embedded message""
    }
  }
  }}
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
            
            [Embedded("docs:foo_2")]
            public Foo2Resource Foo2Resource { get; set; }
        }

        public class Foo2Resource : Resource
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

        private static JsonSerializerSettings CreateSettingsWithContractResolver(Formatting formatting = Formatting.None)
        {
            var settings = new JsonSerializerSettings {Formatting = formatting};
            settings.ContractResolver = new ResourceContractResolver(settings);

            return settings;
        }
        public class TheDeserializeObjectMethod
        {
            [Fact]
            public void ShouldDeserializeEmbeddedProperty()
            {
                var settingsWithResolver = CreateSettingsWithContractResolver();

                var foo = JsonConvert.DeserializeObject<FooResource>(FooResourceJson, settingsWithResolver);

                Assert.Equal("Expected embedded message", (string)foo.EmbeddedProperty.message);
            }

            [Fact]
            public void ShouldDeserializeEmbeddedPropertyToNull_WhenTheRelIsNotInTheJson()
            {
                var settingsWithResolver = CreateSettingsWithContractResolver();

                var foo = JsonConvert.DeserializeObject<FooResource>(FooResourceJson, settingsWithResolver);

                Assert.Null(foo.NullEmbeddedProperty);
            }

            [Theory]
            [InlineData(@"{""normal_property"": {}}")]
            [InlineData(@"{""normal_property"": {}, ""_embedded"": {""e"": {}}}")]
            [InlineData(@"{""normal_property"": {}, ""_links"": {""l"": {}}}")]
            public void ShouldDeserializeObject_WhenJsonDoesNotReturnLinksOrEmbeddedProperties(string json)
            {
                var settingsWithResolver = CreateSettingsWithContractResolver();

                var foo = JsonConvert.DeserializeObject<FooResource>(json, settingsWithResolver);

                Assert.NotNull(foo.NormalProperty);
            }

            [Fact]
            public void ShouldDeserializeSelfLinkProperty()
            {
                var settingsWithResolver = CreateSettingsWithContractResolver();

                var foo = JsonConvert.DeserializeObject<FooResource>(FooResourceJson, settingsWithResolver);

                Assert.Equal("http://api.com/self", foo.SelfLink.HRef);
                Assert.Equal("Self", foo.SelfLink.Title);
                Assert.False(foo.SelfLink.IsTemplated);
            }

            [Fact]
            public void ShouldDeserializeLinkProperty()
            {
                var settingsWithResolver = CreateSettingsWithContractResolver();

                var foo = JsonConvert.DeserializeObject<FooResource>(FooResourceJson, settingsWithResolver);

                Assert.Equal("http://api.com/resources/5", foo.LinkProperty.HRef);
                Assert.Equal("Some Resource", foo.LinkProperty.Title);
                Assert.True(foo.LinkProperty.IsTemplated);
            }

            [Fact]
            public void ShouldDeserializeLinkPropertyToNull_WhenTheRelIsNotInTheJson()
            {
                var settingsWithResolver = CreateSettingsWithContractResolver();

                var foo = JsonConvert.DeserializeObject<FooResource>(FooResourceJson, settingsWithResolver);

                Assert.Null(foo.NullLinkProperty);
            }

            [Fact]
            public void ShouldDeserializeLinksArrayProperty()
            {
                var settingsWithResolver = CreateSettingsWithContractResolver();

                var foo = JsonConvert.DeserializeObject<FooResource>(FooResourceJson, settingsWithResolver);

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
                var settingsWithResolver = CreateSettingsWithContractResolver();
                // Deserialize and then serialize to get rid of all of the new lines
                var expectedJson = JsonConvert.SerializeObject(JsonConvert.DeserializeObject(FooResourceJson, settingsWithResolver));

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
                                    settingsWithResolver);

                Assert.Equal(expectedJson, actualJson);
            }
        }
    }
}
