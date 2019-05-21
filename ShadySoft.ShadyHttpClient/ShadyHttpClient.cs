using Microsoft.JSInterop;
using ShadySoft.ShadyHttpClient.Exceptions;
using ShadySoft.ShadyHttpClient.Extensions;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ShadySoft.ShadyHttpClient
{
    /// <summary>
    /// Extension methods for working with JSON APIs.
    /// </summary>
    public class ShadyHttpClient
    {
        private readonly IAuthorizationService _authorization;
        private readonly HttpClient _httpClient;

        private const HttpCompletionOption defaultCompletionOption = HttpCompletionOption.ResponseContentRead;
        private const string accessTokenHeaderKey = "access-token";
        private const string userIdHeaderKey = "user-id";
        private const string isPersistentHeaderKey = "persist-login";
        private const string authorizationHeaderKey = "Authorization";
        private const string tokenType = "Shady";

        public ShadyHttpClient(IAuthorizationService authorization, HttpClient httpClient)
        {
            _authorization = authorization;
            _httpClient = httpClient;
        }

        /// <summary>
        /// Sends a GET request to the specified URI, and parses the JSON response body
        /// to create an object of the generic type.
        /// </summary>
        /// <typeparam name="T">A type into which the response body can be JSON-deserialized.</typeparam>
        /// <param name="requestUri">The URI that the request will be sent to.</param>
        /// <returns>The response parsed as an object of the generic type.</returns>
        public async Task<T> GetJsonAsync<T>(string requestUri)
        {
            var responseJson = await GetStringAsync(requestUri);
            return Json.Deserialize<T>(responseJson);
        }

        public Task<string> GetStringAsync(string requestUri) =>
            GetStringAsync(CreateUri(requestUri));

        public Task<string> GetStringAsync(Uri requestUri) =>
            GetStringAsyncCore(GetAsync(requestUri, HttpCompletionOption.ResponseHeadersRead));

        private async Task<string> GetStringAsyncCore(Task<HttpResponseMessage> getTask)
        {
            // Wait for the response message.
            using (HttpResponseMessage responseMessage = await getTask.ConfigureAwait(false))
            {
                // Make sure it completed successfully.
                responseMessage.ShadyEnsureSuccess();

                // Get the response content.
                HttpContent c = responseMessage.Content;
                if (c != null)
                {
                    return await c.ReadAsStringAsync();
                }

                // No content to return.
                return string.Empty;
            }
        }

        public Task<HttpResponseMessage> GetAsync(string requestUri)
        {
            return GetAsync(CreateUri(requestUri));
        }

        public Task<HttpResponseMessage> GetAsync(Uri requestUri)
        {
            return GetAsync(requestUri, defaultCompletionOption);
        }

        public Task<HttpResponseMessage> GetAsync(string requestUri, HttpCompletionOption completionOption)
        {
            return GetAsync(CreateUri(requestUri), completionOption);
        }

        public Task<HttpResponseMessage> GetAsync(Uri requestUri, HttpCompletionOption completionOption)
        {
            return GetAsync(requestUri, completionOption, CancellationToken.None);
        }

        public Task<HttpResponseMessage> GetAsync(string requestUri, CancellationToken cancellationToken)
        {
            return GetAsync(CreateUri(requestUri), cancellationToken);
        }

        public Task<HttpResponseMessage> GetAsync(Uri requestUri, CancellationToken cancellationToken)
        {
            return GetAsync(requestUri, defaultCompletionOption, cancellationToken);
        }

        public Task<HttpResponseMessage> GetAsync(string requestUri, HttpCompletionOption completionOption,
            CancellationToken cancellationToken)
        {
            return GetAsync(CreateUri(requestUri), completionOption, cancellationToken);
        }

        public Task<HttpResponseMessage> GetAsync(Uri requestUri, HttpCompletionOption completionOption,
            CancellationToken cancellationToken)
        {
            return SendAsync(new HttpRequestMessage(HttpMethod.Get, requestUri), completionOption, cancellationToken);
        }


        /// <summary>
        /// Sends a POST request to the specified URI, including the specified <paramref name="content"/>
        /// in JSON-encoded format, and parses the JSON response body to create an object of the generic type.
        /// </summary>
        /// <param name="requestUri">The URI that the request will be sent to.</param>
        /// <param name="content">Content for the request body. This will be JSON-encoded and sent as a string.</param>
        /// <returns>The response parsed as an object of the generic type.</returns>
        public Task PostJsonAsync(string requestUri, object content)
            => SendJsonAsync(HttpMethod.Post, requestUri, content);

        /// <summary>
        /// Sends a POST request to the specified URI, including the specified <paramref name="content"/>
        /// in JSON-encoded format, and parses the JSON response body to create an object of the generic type.
        /// </summary>
        /// <typeparam name="T">A type into which the response body can be JSON-deserialized.</typeparam>
        /// <param name="requestUri">The URI that the request will be sent to.</param>
        /// <param name="content">Content for the request body. This will be JSON-encoded and sent as a string.</param>
        /// <returns>The response parsed as an object of the generic type.</returns>
        public Task<T> PostJsonAsync<T>(string requestUri, object content)
            => SendJsonAsync<T>(HttpMethod.Post, requestUri, content);

        /// <summary>
        /// Sends a PUT request to the specified URI, including the specified <paramref name="content"/>
        /// in JSON-encoded format.
        /// </summary>
        /// <param name="requestUri">The URI that the request will be sent to.</param>
        /// <param name="content">Content for the request body. This will be JSON-encoded and sent as a string.</param>
        public Task PutJsonAsync(string requestUri, object content)
            => SendJsonAsync(HttpMethod.Put, requestUri, content);

        /// <summary>
        /// Sends a PUT request to the specified URI, including the specified <paramref name="content"/>
        /// in JSON-encoded format, and parses the JSON response body to create an object of the generic type.
        /// </summary>
        /// <typeparam name="T">A type into which the response body can be JSON-deserialized.</typeparam>
        /// <param name="requestUri">The URI that the request will be sent to.</param>
        /// <param name="content">Content for the request body. This will be JSON-encoded and sent as a string.</param>
        /// <returns>The response parsed as an object of the generic type.</returns>
        public Task<T> PutJsonAsync<T>(string requestUri, object content)
            => SendJsonAsync<T>(HttpMethod.Put, requestUri, content);

        /// <summary>
        /// Sends an HTTP request to the specified URI, including the specified <paramref name="content"/>
        /// in JSON-encoded format.
        /// </summary>
        /// <param name="method">The HTTP method.</param>
        /// <param name="requestUri">The URI that the request will be sent to.</param>
        /// <param name="content">Content for the request body. This will be JSON-encoded and sent as a string.</param>
        public Task SendJsonAsync(HttpMethod method, string requestUri, object content)
            => SendJsonAsync<IgnoreResponse>(method, requestUri, content);

        /// <summary>
        /// Sends an HTTP request to the specified URI, including the specified <paramref name="content"/>
        /// in JSON-encoded format, and parses the JSON response body to create an object of the generic type.
        /// </summary>
        /// <typeparam name="T">A type into which the response body can be JSON-deserialized.</typeparam>
        /// <param name="method">The HTTP method.</param>
        /// <param name="requestUri">The URI that the request will be sent to.</param>
        /// <param name="content">Content for the request body. This will be JSON-encoded and sent as a string.</param>
        /// <returns>The response parsed as an object of the generic type.</returns>
        public async Task<T> SendJsonAsync<T>(HttpMethod method, string requestUri, object content)
        {
            var requestJson = Json.Serialize(content);
            var response = await SendAsync(new HttpRequestMessage(method, requestUri)
            {
                Content = new StringContent(requestJson, Encoding.UTF8, "application/json")
            });

            // Make sure the call was successful before we
            // attempt to process the response content
            response.ShadyEnsureSuccess();

            if (typeof(T) == typeof(IgnoreResponse))
            {
                return default;
            }
            else
            {
                var responseJson = await response.Content.ReadAsStringAsync();
                return Json.Deserialize<T>(responseJson);
            }
        }

        public Task<HttpResponseMessage> SendAsync(HttpRequestMessage request)
        {
            return SendAsync(request, defaultCompletionOption, CancellationToken.None);
        }

        public Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            return SendAsync(request, defaultCompletionOption, cancellationToken);
        }

        public Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, HttpCompletionOption completionOption)

        {
            return SendAsync(request, completionOption, CancellationToken.None);
        }

        public async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, HttpCompletionOption completionOption,
            CancellationToken cancellationToken)
        {
            AddAuthorizationHeader(request.Headers);

            HttpResponseMessage response;
            try
            {
                response = await _httpClient.SendAsync(request, completionOption, cancellationToken);
            }
            catch (HttpRequestException e)
            {
                throw new ShadyHttpNoResponseException(request, e);
            }

            UpdateStoredUser(response.Headers);

            return response;
        }

        private void AddAuthorizationHeader(HttpRequestHeaders headers)
        {
            if (_authorization.IsLoggedIn)
                headers.Add(authorizationHeaderKey, $"{tokenType} {_authorization.AccessToken}");
        }

        private void UpdateStoredUser(HttpResponseHeaders headers)
        {
            if (headers.Contains(accessTokenHeaderKey) && headers.Contains(userIdHeaderKey) && headers.Contains(isPersistentHeaderKey))
            {
                var token = headers.GetValues(accessTokenHeaderKey).First();
                var userId = headers.GetValues(userIdHeaderKey).First();
                var isPersistent = headers.GetValues(isPersistentHeaderKey).First().ToLower() == "true";

                _authorization.SetUser(userId, token, isPersistent);
            }
        }

        private Uri CreateUri(string uri)
        {
            if (string.IsNullOrEmpty(uri))
            {
                return null;
            }
            return new Uri(uri, UriKind.RelativeOrAbsolute);
        }

        class IgnoreResponse { }
    }
}