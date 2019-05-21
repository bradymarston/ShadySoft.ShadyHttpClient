using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace ShadySoft.ShadyHttpClient.Extensions
{
    public static class ShadyHttpClientServiceCollectionExtensions
    {
        public static IServiceCollection AddShadyHttpClient<TAuthorizationService>(this IServiceCollection services)
            where TAuthorizationService : class, IAuthorizationService
        {
            services.AddSingleton<IAuthorizationService>(svcs => svcs.GetService<TAuthorizationService>());
            return services.AddSingleton<ShadyHttpClient>();
        }
    }
}
