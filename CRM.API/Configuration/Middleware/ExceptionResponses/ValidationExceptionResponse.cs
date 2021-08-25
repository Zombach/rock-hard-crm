using CRM.Business.Exceptions;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Collections.Generic;
namespace CRM.API.Configuration.Middleware.ExceptionResponses
{
    public class ValidationExceptionResponse
    {
        public int Code { get; set; }
        public string Message { get; set; }
        public List<ValidationError> Errors { get; set; }

        private const int ValidationCode = 1001;
        private const string MessageValidation = "Validation exception";

        public ValidationExceptionResponse(ValidationException exception)
        {
            Errors = new List<ValidationError>
            {
                new ValidationError {Code = 422, Field = exception.FieldOfError, Message = exception.Message}
            };
        }
        public ValidationExceptionResponse(ModelStateDictionary modelState)
        {
            Code = ValidationCode;
            Message = MessageValidation;
            Errors = new List<ValidationError>();
            foreach (var state in modelState)
            {
                Errors.Add(new ValidationError
                {
                    Code = 422,
                    Field = state.Key,
                    Message = state.Value.Errors[0].ErrorMessage
                });
            }
        }
    }
}
