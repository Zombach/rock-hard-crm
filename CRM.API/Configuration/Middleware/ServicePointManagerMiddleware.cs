using Microsoft.AspNetCore.Http;
using System.Net;
using System.Threading.Tasks;

namespace CRM.API.Configuration.Middleware
{
    // You may need to install the Microsoft.AspNetCore.Http.Abstractions package into your project
    public class ServicePointManagerMiddleware
    {
        private readonly RequestDelegate _next;

        public ServicePointManagerMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public Task Invoke(HttpContext httpContext)
        {
            ServicePointManager.ServerCertificateValidationCallback += (sender, cert, chain, sslPolicyErrors) => true;
            return _next(httpContext);
        }
    }
}