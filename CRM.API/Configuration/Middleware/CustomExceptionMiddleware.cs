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
        private const string jsonType = "application/json";
        private const string _messageAuthorization = "Authorization exception";
        private const string _messageValidation = "Validation exception";
        private const string _messageEntity = "Entity not found exception";
        private const int _authorizationCode = 1000;
        private const int _validationCode = 1001;
        private const int _entityCode = 1002;

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
