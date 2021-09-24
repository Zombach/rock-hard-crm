using CRM.API.Configuration.Middleware.ExceptionResponses;
using CRM.Business.Exceptions;
using CRM.Core;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Net;
using System.Threading.Tasks;

namespace CRM.API.Configuration.Middleware
{
    public class CustomExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private const string _jsonType = "application/json";
        private const string _messageAuthorization = "Authorization exception";
        private const string _messageValidation = "Validation exception";
        private const string _messageEntity = "Entity not found exception";
        private const string _messageInvalidArgument = "Invalid argument exception";
        private const string _messageUnknownError = "Unknown error";

        private const int _authorizationCode = 2000;
        private const int _validationCode = 1001;
        private const int _entityCode = 400;
        private const int _unknownErrorCode = 3000;
        private const int _invalidCode = 1002;

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

        private static Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            HttpStatusCode code;
            object error;
            switch (exception)
            {
                case ValidationException validationException:
                    code = HttpStatusCode.UnprocessableEntity;
                    error = new ValidationExceptionResponse(validationException)
                    {
                        Code = _validationCode,
                        Message = _messageValidation
                    };
                    Logger.Writer(exception.Message);
                    break;
                case InvalidArgumentException:
                    code = HttpStatusCode.BadRequest;
                    error = GenerateExceptionResponse(_invalidCode, _messageInvalidArgument, exception);
                    Logger.Writer(exception.Message);
                    break;
                case EntityNotFoundException _:
                    code = HttpStatusCode.NotFound;
                    error = GenerateExceptionResponse(_entityCode, _messageEntity, exception);
                    Logger.Writer(exception.Message);
                    break;
                case AuthorizationException _:
                    code = HttpStatusCode.Forbidden;
                    error = GenerateExceptionResponse(_authorizationCode, _messageAuthorization, exception);
                    Logger.Writer(exception.Message);
                    break;
                default:
                    code = HttpStatusCode.InternalServerError;
                    error = new
                    {
                        code = _unknownErrorCode,
                        message = _messageUnknownError,
                        description = exception.Message
                    };
                    Logger.Writer(exception.Message);
                    break;
            }

            var result = JsonConvert.SerializeObject(error);

            context.Response.ContentType = _jsonType;
            context.Response.StatusCode = (int)code;

            return context.Response.WriteAsync(result);
        }

        private static ExceptionResponse GenerateExceptionResponse(int code, string message, Exception ex)
        {
            return new()
            {
                Code = code,
                Message = message,
                Description = ex.Message
            };
        }
    }
}