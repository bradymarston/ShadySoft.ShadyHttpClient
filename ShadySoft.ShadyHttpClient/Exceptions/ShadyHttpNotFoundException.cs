using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace ShadySoft.ShadyHttpClient.Exceptions
{
    public class ShadyHttpNotFoundException : ShadyHttpRequestException
    {
        public ShadyHttpNotFoundException(HttpResponseMessage response)
            : base(response)
        { }
        public ShadyHttpNotFoundException(HttpResponseMessage response, Exception inner)
            : base(response, inner)
        { }
    }
}