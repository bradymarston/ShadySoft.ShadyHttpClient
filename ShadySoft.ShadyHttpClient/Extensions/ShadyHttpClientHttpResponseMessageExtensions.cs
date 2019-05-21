using ShadySoft.ShadyHttpClient.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace ShadySoft.ShadyHttpClient.Extensions
{
    public static class ShadyHttpClientHttpResponseMessageExtensions
    {
        public static void ShadyEnsureSuccess(this HttpResponseMessage response)
        {
            if (!response.IsSuccessStatusCode)
                switch (response.StatusCode)
                {
                    case HttpStatusCode.Unauthorized: throw new ShadyHttpUnauthorizedException(response);
                    case HttpStatusCode.NotFound: throw new ShadyHttpNotFoundException(response);
                    default: throw new ShadyHttpRequestException(response);
                }
                
        }
    }
}
