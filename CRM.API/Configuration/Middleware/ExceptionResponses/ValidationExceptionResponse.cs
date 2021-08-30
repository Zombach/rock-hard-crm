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

        private const int _validationCode = 1001;
        private const int _validationStatusCode = 422;
        private const string _messageValidation = "Validation exception";

        public ValidationExceptionResponse(ValidationException exception)
        {
            Errors = new List<ValidationError>
            {
                new() 
                {
                    Code = _validationStatusCode,
                    Field = exception.FieldOfError,
                    Message = exception.Message

                }
            };
        }

        public ValidationExceptionResponse(ModelStateDictionary modelState)
        {
            Code = _validationCode;
            Message = _messageValidation;
            Errors = new List<ValidationError>();
            foreach (var state in modelState)
            {
                Errors.Add(new ValidationError
                {
                    Code = _validationStatusCode,
                    Field = state.Key,
                    Message = state.Value.Errors[0].ErrorMessage
                });
            }
        }
    }
}