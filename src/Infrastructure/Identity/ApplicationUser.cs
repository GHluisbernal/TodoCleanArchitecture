using System.Collections.Generic;
using Amazon.CognitoIdentityProvider;
using Amazon.Extensions.CognitoAuthentication;
using Microsoft.AspNetCore.Identity;

namespace CleanArchitecture.Infrastructure.Identity
{
    public class ApplicationUser : CognitoUser
    {
        public ApplicationUser(string userName): base(userName, null, null, null, username: userName)
        {
            
        }
        public ApplicationUser(string userID, string clientID, CognitoUserPool pool, IAmazonCognitoIdentityProvider provider, string clientSecret = null, string status = null, string username = null, Dictionary<string, string> attributes = null) : base(userID, clientID, pool, provider, clientSecret, status, username, attributes)
        {
        }
    }
}
