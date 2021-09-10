namespace CleanArchitecture.RestApi.Modules.Common
{
    using System.Text.Json;
    using System.Text.Json.Serialization;
    using FeatureFlags;
    using FluentValidation.AspNetCore;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Formatters;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.FeatureManagement;

    /// <summary>
    ///     Custom Controller Extensions.
    /// </summary>
    public static class CustomControllersExtensions
    {
        /// <summary>
        ///     Add Custom Controller dependencies.
        /// </summary>
        public static IServiceCollection AddCustomControllers(this IServiceCollection services)
        {
            IFeatureManager featureManager = services
                .BuildServiceProvider()
                .GetRequiredService<IFeatureManager>();

            bool isErrorFilterEnabled = featureManager
                .IsEnabledAsync(nameof(CustomFeature.ErrorFilter))
                .ConfigureAwait(false)
                .GetAwaiter()
                .GetResult();

            services
                .AddControllersWithViews(options =>
                {
                    options.OutputFormatters.RemoveType<TextOutputFormatter>();
                    options.OutputFormatters.RemoveType<StreamOutputFormatter>();
                    options.RespectBrowserAcceptHeader = true;

                    if (isErrorFilterEnabled)
                    {
                        options.Filters.Add<ApiExceptionFilterAttribute>();
                    }
                })
                .AddFluentValidation(x => x.AutomaticValidationEnabled = false)
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.DictionaryKeyPolicy = JsonNamingPolicy.CamelCase;
                    options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
                    options.JsonSerializerOptions.Converters.Add(
                        new JsonStringEnumConverter(JsonNamingPolicy.CamelCase)
                    );
                })
                .AddControllersAsServices();
            
            // Customise default API behaviour
            services.Configure<ApiBehaviorOptions>(options =>
            {
                options.SuppressModelStateInvalidFilter = true;
            });

            return services;
        }
    }
}
