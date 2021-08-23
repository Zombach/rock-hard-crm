using CRM.Business.Exceptions;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

using System.Linq;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;

namespace CRM.API.Middleware
{
    public class CustomExceptionMiddleware
    {
        private readonly RequestDelegate _next;

        public CustomExceptionMiddleware(RequestDelegate next) => _next = next;

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception exception)
            {
                await HandleExceptionAsync(context, exception);
            }
        }

        private Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            HttpStatusCode code;
            object error = new { error = exception.Message };
            switch (exception)
            {
                case ValidationException _:
                case InvalidArgumentException _:
                    code = HttpStatusCode.BadRequest; // 400
                    break;
                case EntityNotFoundException _:
                    code = HttpStatusCode.NotFound; //404
                    break;
                case AuthorizationException _:
                    code = HttpStatusCode.Forbidden; //403
                    break;
                default:
                    code = HttpStatusCode.InternalServerError;
                    break;
            }
            var result = JsonConvert.SerializeObject(error);

            context.Response.ContentType = jsonType;
            context.Response.StatusCode = (int)code;
            
            return context.Response.WriteAsync(result);
        }
    }
}
