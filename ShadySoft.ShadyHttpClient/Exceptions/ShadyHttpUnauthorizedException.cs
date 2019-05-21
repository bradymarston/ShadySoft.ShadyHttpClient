using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace ShadySoft.ShadyHttpClient.Exceptions
{
    public class ShadyHttpUnauthorizedException : ShadyHttpRequestException
    {
        public ShadyHttpUnauthorizedException(HttpResponseMessage response)
            : base(response)
        { }
        public ShadyHttpUnauthorizedException(HttpResponseMessage response, Exception inner)
            : base(response, inner)
        { }
    }
}
