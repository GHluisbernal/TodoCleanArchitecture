using Amazon.AspNetCore.Identity.Cognito;
using CleanArchitecture.Application.Common.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using System.Threading.Tasks;

namespace CleanArchitecture.Infrastructure.Identity
{
    public class CurrentUserService : ICurrentUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IAuthorizationService _authorizationService;

        public CurrentUserService(
            IHttpContextAccessor httpContextAccessor,
            IAuthorizationService authorizationService
        )
        {
            _httpContextAccessor = httpContextAccessor;
            _authorizationService = authorizationService;
        }

        public ClaimsPrincipal ClaimsPrincipal => _httpContextAccessor.HttpContext?.User;
        public bool IsInRole(string role) => ClaimsPrincipal.IsInRole(role);
        public Task<bool> AuthorizeAsync(AuthorizationPolicy policy) => Task.FromResult(
            _authorizationService.AuthorizeAsync(ClaimsPrincipal, policy)
                .ConfigureAwait(false)
                .GetAwaiter()
                .GetResult()
                .Succeeded
        );
        public Task<bool> AuthorizeAsync(string policyName) => Task.FromResult(
            _authorizationService.AuthorizeAsync(ClaimsPrincipal, policyName)
                .ConfigureAwait(false)
                .GetAwaiter()
                .GetResult()
                .Succeeded
        );
        
        public string UserId => _httpContextAccessor.HttpContext?.User?.FindFirstValue(CognitoAttribute.Sub.ToString());
        public string UserName => _httpContextAccessor.HttpContext?.User?.FindFirstValue(CognitoAttribute.UserName.ToString());
    }
}
