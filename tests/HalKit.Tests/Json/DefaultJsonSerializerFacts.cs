using HalKit.Json;
using HalKit.Models;
using HalKit.Resources;
using Xunit;

namespace HalKit.Tests.Json
{
    public class DefaultJsonSerializerFacts
    {
        private const string FooResourceJson =
@"{
  ""_links"": {
    ""curies"": [
      {""name"": ""docs"",""href"": ""http://foo.com/rels/{rel}""}
    ],
    ""docs:some_resource"": {
      ""href"": ""http://api.com/resources/5"",
      ""title"": ""Some Resource""
    }
  },
  ""_embedded"": {
    ""docs:some_resource"": {""message"": ""Expected embedded message""}
  }
}";
        public class FooResource : Resource
        {
            [Embedded("docs:some_resource")]
            public dynamic EmbeddedProperty { get; set; }
        }

        public class TheDeserializeAsyncMethod
        {
            [Fact]
            public async void ShouldDeserializeEmbeddedProperty()
            {
                var serializer = new DefaultJsonSerializer();

                var foo = await serializer.DeserializeAsync<FooResource>(FooResourceJson);

                Assert.Equal("Expected embedded message", (string)foo.EmbeddedProperty.message);
            }

            [Fact]
            public async void ShouldDeserializeLinnks()
            {
                var serializer = new DefaultJsonSerializer();

                var foo = await serializer.DeserializeAsync<FooResource>(FooResourceJson);

                Assert.Equal("http://api.com/resources/5", foo.Links["docs:some_resource"].HRef);
                Assert.Equal("Some Resource", foo.Links["docs:some_resource"].Title);
            }
        }
    }
}
