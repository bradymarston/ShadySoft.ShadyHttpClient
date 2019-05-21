using System;
using System.Collections.Generic;
using System.Text;

namespace ShadySoft.ShadyHttpClient
{
    public interface IAuthorizationService
    {
        bool IsLoggedIn { get; }
        string AccessToken { get; }
        void SetUser(string userId, string accessToken, bool isPersistent);
    }
}
