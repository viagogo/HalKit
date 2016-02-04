using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace HalKit.Http
{
    public interface IApiRequest
    {
        Uri Uri { get; set; }
        HttpMethod Method { get; set; }
        IDictionary<string, IEnumerable<string>> Headers { get; set; }
        object Body { get; set; }
    }
}
