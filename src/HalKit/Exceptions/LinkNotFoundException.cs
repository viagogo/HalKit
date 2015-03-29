using System;

namespace HalKit.Exceptions
{
    public class LinkNotFoundException : Exception
    {
        public LinkNotFoundException(string rel)
            : base(string.Format("Resource has no link with rel '{0}'", rel))
        {
            
        }
    }
}
