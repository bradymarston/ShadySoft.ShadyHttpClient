using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace ShadySoft.ShadyHttpClient.Exceptions
{
    public class ShadyHttpNoResponseException : Exception
    {
        public Uri RequestUri { get; private set; }
        private const string message = "Did not get a response from the server.";

        public ShadyHttpNoResponseException(HttpRequestMessage request)
            : base(message)
        {
            FillFields(request);
        }

        public ShadyHttpNoResponseException(HttpRequestMessage request, Exception inner)
            : base(message, inner)
        {
            FillFields(request);
        }

        private void FillFields(HttpRequestMessage request)
        {
            RequestUri = request.RequestUri;
        }
    }
}
