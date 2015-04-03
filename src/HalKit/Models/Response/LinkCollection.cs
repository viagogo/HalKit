using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using HalKit.Exceptions;

namespace HalKit.Models.Response
{
    public class LinkCollection : IEnumerable<Link>
    {
        private readonly IEnumerable<CurieLink> _curies;
        private readonly Dictionary<string, List<Link>> _links;

        public LinkCollection(IEnumerable<Link> links, IEnumerable<CurieLink> curies)
        {
            Requires.ArgumentNotNull(links, "links");
            Requires.ArgumentNotNull(curies, "curies");

            _links = new Dictionary<string, List<Link>>();
            foreach (var link in links)
            {
                if (string.IsNullOrEmpty(link.Rel))
                {
                    throw new ArgumentException("All links must have a link-relation");
                }

                List<Link> listOfLinksForRel;
                if (!_links.TryGetValue(link.Rel, out listOfLinksForRel))
                {
                    listOfLinksForRel = new List<Link>();
                    _links.Add(link.Rel, listOfLinksForRel);
                }

                listOfLinksForRel.Add(link);
            }

            _curies = curies;
        }

        public IEnumerable<CurieLink> Curies
        {
            get { return _curies; }
        }

        public bool TryGetLink(string rel, out IEnumerable<Link> links)
        {
            Requires.ArgumentNotNull(rel, "rel");

            List<Link> linksList;
            if (!_links.TryGetValue(rel, out linksList))
            {
                links = null;
                return false;
            }

            links = linksList;
            return true;
        }

        public bool TryGetLink(string rel, out Link link)
        {
            IEnumerable<Link> links;
            if (!TryGetLink(rel, out links))
            {
                link = null;
                return false;
            }

            link = links.First();
            return true;
        }

        public Link this[string rel]
        {
            get
            {
                Link link;
                if (!TryGetLink(rel, out link))
                {
                    throw new LinkNotFoundException(rel);
                }

                return link;
            }
        }

        public IEnumerator<Link> GetEnumerator()
        {
            return _links.SelectMany(kv => kv.Value).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
