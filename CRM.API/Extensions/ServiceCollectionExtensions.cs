using CRM.Business.Options;
using CRM.Core;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace CRM.API.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static void AddAppConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddOptions<DatabaseSettings>()
                .Bind(configuration.GetSection(nameof(DatabaseSettings)))
                .ValidateDataAnnotations();
            services.AddOptions<AuthSettings>()
                .Bind(configuration.GetSection(nameof(AuthSettings)))
                .ValidateDataAnnotations();
            services.AddOptions<ConnectionUrl>()
               .Bind(configuration.GetSection(nameof(ConnectionUrl)))
               .ValidateDataAnnotations();
        }

        public static void AddBearerAuthentication(this IServiceCollection services)
        {
            var provider = services.BuildServiceProvider();
            var authOptions = provider.GetRequiredService<IAuthOptions>();
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.RequireHttpsMetadata = true;
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidIssuer = authOptions.Issuer,

                        ValidateAudience = true,
                        ValidAudience = authOptions.Audience,

                        ValidateLifetime = true,

                        IssuerSigningKey = authOptions.GetSymmetricSecurityKey(),
                        ValidateIssuerSigningKey = true
                    };
                });
        }
    }
}