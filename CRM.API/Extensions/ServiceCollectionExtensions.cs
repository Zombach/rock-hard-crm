using System.Text.Json.Serialization;
using CRM.API.Configuration.Middleware.ExceptionResponses;
using CRM.Business.Options;
using CRM.Business.Services;
using CRM.Core;
using CRM.DAL.Repositories;
using DevEdu.Business.ValidationHelpers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using NSwag.Generation.Processors.Security;

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
            services.AddOptions<CommissionSettings>()
                .Bind(configuration.GetSection(nameof(CommissionSettings)))
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

        public static IServiceCollection AddRepositories(this IServiceCollection services)
        {
            services.AddScoped<IAccountRepository, AccountRepository>();
            services.AddScoped<ILeadRepository, LeadRepository>();
            services.AddScoped<ICityRepository, CityRepository>();

            return services;
        }

        public static IServiceCollection AddCustomServices(this IServiceCollection services)
        {
            services.AddScoped<ILeadService, LeadService>();
            services.AddScoped<IAccountService, AccountService>();
            services.AddScoped<ICityService, CityService>();
            services.AddScoped<IAuthenticationService, AuthenticationService>();
            services.AddScoped<ITransactionService, TransactionService>();

            return services;
        }

        public static IServiceCollection AddValidationHelpers(this IServiceCollection services)
        {
            services.AddScoped<ILeadValidationHelper, LeadValidationHelper>();
            services.AddScoped<IAccountValidationHelper, AccountValidationHelper>();

            return services;
        }

        public static void AddSwagger(this IServiceCollection services)
        {
            services.AddSwaggerDocument(document =>
            {
                document.DocumentName = "CRM";
                document.Title = "CRM API";
                document.Version = "v1";
                document.Description = "An interface for CRM.";

                document.DocumentProcessors.Add(
                    new SecurityDefinitionAppender("JWT token", new NSwag.OpenApiSecurityScheme
                    {
                        Type = NSwag.OpenApiSecuritySchemeType.ApiKey,
                        Name = "Authorization",
                        Description = "Copy 'Bearer ' + valid JWT token into field",
                        In = NSwag.OpenApiSecurityApiKeyLocation.Header
                    }));
                document.OperationProcessors.Add(new OperationSecurityScopeProcessor("JWT token"));
            });
        }

        public static void AddValidationExceptionResponse(this IServiceCollection services)
        {
            services
                .AddMvc()
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                })

                .ConfigureApiBehaviorOptions(options =>
                {
                    options.InvalidModelStateResponseFactory = context =>
                    {
                        var exc = new ValidationExceptionResponse(context.ModelState);
                        return new UnprocessableEntityObjectResult(exc);
                    };
                });
        }
    }
}