namespace CleanArchitecture.RestApi.Modules.Common
{
    using CleanArchitecture.Application.Common.Interfaces;
    using CleanArchitecture.Infrastructure.Identity;
    using FeatureFlags;
    using Microsoft.AspNetCore.Authentication;
    using Microsoft.AspNetCore.Authentication.JwtBearer;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.FeatureManagement;
    using Microsoft.IdentityModel.Tokens;

    /// <summary>
    ///     Authentication Extensions.
    /// </summary>
    public static class AuthenticationExtensions
    {
        /// <summary>
        ///     Add Authentication Extensions.
        /// </summary>
        public static IServiceCollection AddAuthentication(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            IFeatureManager featureManager = services
                .BuildServiceProvider()
                .GetRequiredService<IFeatureManager>();

            bool isEnabled = featureManager
                .IsEnabledAsync(nameof(CustomFeature.Authentication))
                .ConfigureAwait(false)
                .GetAwaiter()
                .GetResult();

            if (isEnabled)
            {
                services
                    .AddCognitoIdentity();
                services
                    .AddAuthentication(options =>
                    {
                        options.DefaultSignInScheme = JwtBearerDefaults.AuthenticationScheme;
                        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                    })
                    .AddJwtBearer(options =>
                    {
                        options.TokenValidationParameters = new TokenValidationParameters { ValidateAudience = false };
                        options.Authority = "https://cognito-idp.us-east-1.amazonaws.com/us-east-1_lBT5NNuCF";
                        options.RequireHttpsMetadata = true;
                    });
                services.AddScoped<ICurrentUserService, CurrentUserService>();
                services.AddScoped<IIdentityService, CognitoIdentityService>();

                services.AddAuthorization(options =>
                {
                    options.AddPolicy("CanPurge", policy => policy.RequireRole("Administrator"));
                });
            }
            else
            {
                services.AddAuthentication(x =>
                {
                    x.DefaultAuthenticateScheme = "Test";
                    x.DefaultChallengeScheme = "Test";
                }).AddScheme<AuthenticationSchemeOptions, TestAuthenticationHandler>(
                    "Test", options => { });
                services.AddHttpContextAccessor();
                services.AddScoped<ICurrentUserService, CurrentUserService>();
            }

            return services;
        }
    }
}
