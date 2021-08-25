using CRM.API.Configuration.Middleware.ExceptionResponses;
using CRM.Business.Exceptions;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Net;
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
        private const string _messageInvalidArgument = "Invalid argument exception";
        private const string _messageUnknownError = "Unknown error";

        private const int _authorizationCode = 1000;
        private const int _validationCode = 1001;
        private const int _entityCode = 1002;
        private const int _unknownErrorCode = 1003;
        private const int _invalidCode = 1004;


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
            object error;
            switch (exception)
            {
                case ValidationException validationException:
                    code = HttpStatusCode.BadRequest;
                    error = new ValidationExceptionResponse(validationException)
                    {
                        Code = _validationCode,
                        Message = _messageValidation
                    };
                    break;
                case InvalidArgumentException :
                    code = HttpStatusCode.BadRequest; 
                    error = GenerateExceptionResponse(_invalidCode, _messageInvalidArgument, exception);
                    break;
                case EntityNotFoundException _:
                    code = HttpStatusCode.NotFound; 
                    error = GenerateExceptionResponse(_entityCode, _messageEntity, exception);
                    break;
                case AuthorizationException _:
                    code = HttpStatusCode.Forbidden; 
                    error = GenerateExceptionResponse(_authorizationCode, _messageAuthorization, exception);
                    break;
                default:
                    code = HttpStatusCode.InternalServerError;
                    error = new
                    {
                        code = _unknownErrorCode,
                        message = _messageUnknownError,
                        description = exception.Message
                    };
                    break;
            }
            var result = JsonConvert.SerializeObject(error);

            context.Response.ContentType = jsonType;
            context.Response.StatusCode = (int)code;
            
            return context.Response.WriteAsync(result);
        }

        private ExceptionResponse GenerateExceptionResponse(int code, string message, Exception ex)
        {
            ExceptionResponse result = new ExceptionResponse()
            {
                Code = code,
                Message = message,
                Description = ex.Message
            };
            return result;
        }
    }
}
