using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace ShadySoft.ShadyHttpClient.Exceptions
{
    public class ShadyHttpRequestException : Exception
    {
        public HttpStatusCode StatusCode { get; private set; }

        public ShadyHttpRequestException(HttpResponseMessage response)
            : base(GenerateMessage(response))
        {
            FillFields(response);
        }

        public ShadyHttpRequestException(HttpResponseMessage response, string message)
            : base(message)
        {
            FillFields(response);
        }

        public ShadyHttpRequestException(HttpResponseMessage response, string message, Exception inner)
            : base(message, inner)
        {
            FillFields(response);
        }

        public ShadyHttpRequestException(HttpResponseMessage response, Exception inner)
            : base(GenerateMessage(response), inner)
        {
            FillFields(response);

        }

        private static string GenerateMessage(HttpResponseMessage response)
        {
            return $"Error getting values: Response status code does not indicate success: {(int)response.StatusCode} ({response.StatusCode})";
        }

        private void FillFields(HttpResponseMessage response)
        {
            StatusCode = response.StatusCode;
        }
    }
}