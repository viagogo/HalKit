using System;
using System.Collections.Generic;
using System.Linq;
using HalKit.Exceptions;
using HalKit.Models;
using Xunit;

namespace HalKit.Tests.Models
{
    public class LinkCollectionFacts
    {
        private static LinkCollection CreateCollection(
            IEnumerable<Link> links = null,
            IEnumerable<CurieLink> curies = null)
        {
            return new LinkCollection(links ?? new Link[] {}, curies ?? new CurieLink[] {});
        }

        public class TheConstructor
        {
            [Fact]
            public void ShouldThrowInvalidArgumentException_WhenAnyOfTheGivenLinksDoNotHaveARel()
            {
                var links = new[] {new Link {Rel = null}, new Link {Rel = "A"}};

                Assert.Throws<ArgumentException>(() => CreateCollection(links: links));
            }
        }

        public class TheGetEnumeratorMethod
        {
            [Fact]
            public void ShouldReturnAnEnumeratorForTheGivenLinks()
            {
                var expectedLinks = new[] {new Link {Rel = "B"}, new Link {Rel = "A"}, new Link {Rel = "B"}};
                var collection = CreateCollection(links: expectedLinks);

                var actualLinks = collection.ToList();

                Assert.Equal(expectedLinks.Length, actualLinks.Count);
                Assert.True(expectedLinks.All(actualLinks.Contains));
            }
        }

        public class TheTryGetLinkMethod
        {
            [Fact]
            public void ShouldReturnTrue_WhenTheCollectionContainsALinkWithTheGivenRel()
            {
                var collection = CreateCollection(links: new[] {new Link {Rel = "B"}, new Link {Rel = "A"}, new Link {Rel = "B"}});

                Link link;
                var result = collection.TryGetLink("A", out link);

                Assert.True(result);
            }

            [Fact]
            public void ShouldReturnFalse_WhenTheCollectionDoesNotContainALinkWithTheGivenRel()
            {
                var collection = CreateCollection(links: new[] { new Link { Rel = "B" }, new Link { Rel = "A" }, new Link { Rel = "B" } });

                Link link;
                var result = collection.TryGetLink("C", out link);

                Assert.False(result);
            }

            [Fact]
            public void ShouldOutputFirstLinkWithSpecifiedRel()
            {
                var expectedLink = new Link { Rel = "A" };
                var collection = CreateCollection(links: new[]
                                                         {
                                                             expectedLink,
                                                             new Link {Rel = "B"},
                                                             new Link {Rel = expectedLink.Rel}
                                                         });

                Link actualLink;
                collection.TryGetLink("A", out actualLink);

                Assert.Same(expectedLink, actualLink);
            }
        }

        public class TheTryGetLinksMethod
        {
            [Fact]
            public void ShouldReturnTrue_WhenTheCollectionContainsLinksWithTheGivenRel()
            {
                var collection = CreateCollection(links: new[] { new Link { Rel = "B" }, new Link { Rel = "A" }, new Link { Rel = "B" } });

                IEnumerable<Link> links;
                var result = collection.TryGetLink("A", out links);

                Assert.True(result);
            }

            [Fact]
            public void ShouldReturnFalse_WhenTheCollectionDoesNotContainALinkWithTheGivenRel()
            {
                var collection = CreateCollection(links: new[] { new Link { Rel = "B" }, new Link { Rel = "A" }, new Link { Rel = "B" } });

                IEnumerable<Link> links;
                var result = collection.TryGetLink("C", out links);

                Assert.False(result);
            }

            [Fact]
            public void ShouldOutputTheLinksWithSpecifiedRel()
            {
                var expectedLinks = new[] {new Link {Rel = "A"}, new Link {Rel = "A"}};
                var collection = CreateCollection(links: new[]
                                                         {
                                                             expectedLinks[0],
                                                             new Link {Rel = "B"},
                                                             expectedLinks[1]
                                                         });

                IEnumerable<Link> actualLinks;
                collection.TryGetLink("A", out actualLinks);

                Assert.Equal(expectedLinks, actualLinks);
            }
        }

        public class TheArrayIndexer
        {
            [Fact]
            public void ShouldReturnTheFirstLinkWithTheSpecifiedRel()
            {
                var expectedLink = new Link {Rel = "A"};
                var collection = CreateCollection(links: new[]
                                                         {
                                                             expectedLink,
                                                             new Link {Rel = "B"},
                                                             new Link {Rel = expectedLink.Rel}
                                                         });

                var actualLink = collection["A"];

                Assert.Same(expectedLink, actualLink);
            }

            [Fact]
            public void ShouldThrowLinkNotFoundException_WhenTheCollectionDoesNotContainALinkWithTheGivenRel()
            {
                var collection = CreateCollection(links: new[] { new Link { Rel = "B" }, new Link { Rel = "A" }, new Link { Rel = "B" } });

                Assert.Throws<LinkNotFoundException>(() => collection["C"]);
            }
        }

        public class TheCuriesProperty
        {
            [Fact]
            public void ShouldReturnTheCuriesGivenToTheConstructor()
            {
                var expectedCuries = new[] {new CurieLink(), new CurieLink()};
                var collection = CreateCollection(curies: expectedCuries);

                var actualCuries = collection.Curies;

                Assert.Equal(expectedCuries, actualCuries);
            }
        }
    }
}
