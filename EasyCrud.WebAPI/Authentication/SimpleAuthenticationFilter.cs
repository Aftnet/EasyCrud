using System;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http.Filters;
using EasyCrud.WebAPI.Configuration;

namespace EasyCrud.WebAPI.Authentication
{
    public class SimpleAuthenticationFilter : IAuthenticationFilter
    {
        private const string InvalidTokenError = "Invalid token";

        public bool AllowMultiple { get; private set; }
        protected readonly string AuthTokenConfigKey;
        protected string AuthToken;

        public SimpleAuthenticationFilter(string authTokenConfigKey)
        {
            AuthTokenConfigKey = authTokenConfigKey;
            AllowMultiple = true;
        }

        public Task AuthenticateAsync(HttpAuthenticationContext context, CancellationToken cancellationToken)
        {
            if(AuthToken == null)
            {
                AuthToken = GetReferenceAuthToken();
            }

            var request = context.Request;
            var authorization = request.Headers.Authorization;
            var failureResult = new AuthenticationFailureResult(InvalidTokenError, request);
            if (authorization == null)
            {
                context.ErrorResult = failureResult;
                return Task.FromResult(true);
            }
            if (!String.Equals(authorization.Parameter, AuthToken) && !String.Equals(authorization.Scheme, AuthToken))
            {
                context.ErrorResult = failureResult;
            }
            return Task.FromResult(true);
        }

        public Task ChallengeAsync(HttpAuthenticationChallengeContext context, CancellationToken cancellationToken)
        {
            return Task.FromResult(0);
        }

        protected string GetReferenceAuthToken()
        {
            var configManager = new ConfigManager();
            var output = configManager.GetParameter<string>(AuthTokenConfigKey);
            return output;
        }
    }
}
