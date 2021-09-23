using CRM.API.Configuration.Middleware.ExceptionResponses;
using CRM.Business.Options;
using CRM.Business.Services;
using CRM.Business.ValidationHelpers;
using CRM.Core;
using CRM.DAL.Repositories;
using MassTransit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using NSwag.Generation.Processors.Security;
using System.Text.Json.Serialization;

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
            services.AddOptions<ConnectionSettings>()
               .Bind(configuration.GetSection(nameof(ConnectionSettings)))
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
            services.AddScoped<ICommissionFeeRepository, CommissionFeeRepository>();

            return services;
        }

        public static IServiceCollection AddCustomServices(this IServiceCollection services)
        {
            services.AddScoped<ILeadService, LeadService>();
            services.AddScoped<IAccountService, AccountService>();
            services.AddScoped<ICityService, CityService>();
            services.AddScoped<IAuthenticationService, AuthenticationService>();
            services.AddScoped<ITransactionService, TransactionService>();
            services.AddScoped<ICommissionFeeService, CommissionFeeService>();
            services.AddScoped<IEmailSenderService, EmailSenderService>();
            services.AddScoped<ITwoFactorAuthenticatorService, TwoFactorAuthenticatorService>();

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
                        Logger.Writer($"{exc.Message} {exc.Code} {exc.Errors}");
                        return new UnprocessableEntityObjectResult(exc);
                    };
                });
        }

        public static IServiceCollection EmailSender(this IServiceCollection services)
        {
            services.AddMassTransit(x =>
            {
                x.UsingRabbitMq((context, cfg) => cfg.Host("80.78.240.16", "/", h =>
                {
                    cfg.OverrideDefaultBusEndpointQueueName("queue-mail");
                    h.Username("nafanya");
                    h.Password("qwe!23");
                }));
            });
            services.AddMassTransitHostedService();
            return services;
        }
    }
}