using Amazon.AspNetCore.Identity.Cognito;
using Amazon.Extensions.CognitoAuthentication;
using CleanArchitecture.Application.Common.Interfaces;
using CleanArchitecture.Application.Common.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;

namespace CleanArchitecture.Infrastructure.Identity
{
    public class CognitoIdentityService : IIdentityService
    {
        private readonly CognitoUserManager<CognitoUser> _userManager;
        private readonly CognitoUserClaimsPrincipalFactory<CognitoUser> _userClaimsPrincipalFactory;
        private readonly IAuthorizationService _authorizationService;
        private readonly CognitoUserPool _userPool;

        public CognitoIdentityService(
            UserManager<CognitoUser> userManager,
            IUserClaimsPrincipalFactory<CognitoUser> userClaimsPrincipalFactory,
            IAuthorizationService authorizationService,
            CognitoUserPool userPool)
        {
            _userManager = userManager as CognitoUserManager<CognitoUser>;
            _userClaimsPrincipalFactory = userClaimsPrincipalFactory as CognitoUserClaimsPrincipalFactory<CognitoUser>;
            _authorizationService = authorizationService;
            _userPool = userPool;
        }

        public async Task<string> GetUserNameAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);

            return user.Username;
        }

        public async Task<(Result Result, string UserId)> CreateUserAsync(string userName, string password)
        {
            var user = new CognitoUser(null, null, null, null, username: userName);

            var result = await _userManager.CreateAsync(user, password);

            return (result.ToApplicationResult(), user.UserID);
        }

        public async Task<bool> IsInRoleAsync(string userId, string role)
        {
            var user = await _userManager.FindByIdAsync(userId);
            return await _userManager.IsInRoleAsync(user, role);
        }

        public async Task<bool> AuthorizeAsync(string userId, string policyName)
        {
            var user = await _userManager.FindByIdAsync(userId);
            var principal = await _userClaimsPrincipalFactory.CreateAsync(user);
            var result = await _authorizationService.AuthorizeAsync(principal, policyName);

            return result.Succeeded;
        }

        public async Task<Result> DeleteUserAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return Result.Success();
            
            return await DeleteUserAsync(user);
        }

        public async Task<Result> DeleteUserAsync(CognitoUser user)
        {
            var result = await _userManager.DeleteAsync(user);

            return result.ToApplicationResult();
        }
    }
}
