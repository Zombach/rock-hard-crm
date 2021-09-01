using CRM.API.Configuration.Middleware;
using CRM.API.Extensions;
using CRM.Business.Configuration;
using CRM.Business.Options;
using CRM.Business.Services;
using CRM.Core;
using CRM.DAL.Repositories;
using DevEdu.Business.ValidationHelpers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NSwag.Generation.Processors.Security;

namespace CRM.API
{
    public class Startup
    {
        private const string _pathToEnvironment = "ASPNETCORE_ENVIRONMENT";

        public Startup(IConfiguration configuration)
        {
            var currentEnvironment = configuration.GetValue<string>(_pathToEnvironment);
            var builder = new ConfigurationBuilder().AddJsonFile($"appsettings.{currentEnvironment}.json");

            Configuration = builder.Build();
            Configuration.SetEnvironmentVariableForConfiguration();
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddAppConfiguration(Configuration);
            services.AddScoped<IAuthOptions, AuthOptions>();
            services.AddBearerAuthentication();
            services.AddAutoMapper(typeof(Startup), typeof(BusinessProfile));

            services.AddScoped<IAccountRepository, AccountRepository>();
            services.AddScoped<ILeadRepository, LeadRepository>();
            services.AddScoped<ICityRepository, CityRepository>();

            services.AddScoped<ILeadService, LeadService>();
            services.AddScoped<IAccountService, AccountService>();
            services.AddScoped<ICityService, CityService>();
            services.AddScoped<IAuthenticationService, AuthenticationService>();
            services.AddScoped<ITransactionService, TransactionService>();

            services.AddScoped<ILeadValidationHelper, LeadValidationHelper>();
            services.AddScoped<IAccountValidationHelper, AccountValidationHelper>();

            services.AddControllersWithViews();

            services.AddControllers();
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

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseOpenApi();
                app.UseSwaggerUi3();
            }
            app.UseMiddleware<CustomExceptionMiddleware>();
            app.UseDefaultFiles();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            app.UseHttpsRedirection();

            app.UseRouting();
        }
    }
}