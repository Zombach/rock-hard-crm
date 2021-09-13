using CRM.API.Common;
using CRM.Business.Exceptions;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Collections.Generic;
using System.Linq;
using CRM.Core;

namespace CRM.API.Configuration.Middleware.ExceptionResponses
{
    public class ValidationExceptionResponse
    {
        public int Code { get; set; }
        public string Message { get; set; }
        public List<ValidationError> Errors { get; set; }

        private const int ValidationCode = 1001;
        private const int ValidationStatusCode = 422;
        private const string MessageValidation = "Validation exception";

        public ValidationExceptionResponse(ValidationException exception)
        {
            Errors = new List<ValidationError>
            {
                new() {Code = ValidationStatusCode, Field = exception.FieldOfError, Message = exception.Message}
            };
        }

        public ValidationExceptionResponse(ModelStateDictionary modelState)
        {
            Code = ValidationCode;
            Message = MessageValidation;
            Errors = new List<ValidationError>();
            foreach (var state in modelState.Where(state => state.Value.Errors.Count != 0))
            {
                Errors.Add(new ValidationError
                {
                    Code = GetValidationCode(state.Value.Errors[0].ErrorMessage),
                    Field = state.Key,
                    Message = state.Value.Errors[0].ErrorMessage
                });
            }
        }

        private static int GetValidationCode(string exception)
        {
            return exception switch
            {
                ValidationMessage.FirstNameRequired => 1001,
                ValidationMessage.LastNameRequired => 1002,
                ValidationMessage.PatronymicRequired => 1003,
                ValidationMessage.EmailRequired => 1004,
                ValidationMessage.WrongEmailFormat => 1005,
                ValidationMessage.NameRequired => 1006,
                ValidationMessage.AmountRequired => 1007,
                ValidationMessage.AccountRequired => 1008,
                ValidationMessage.RecipientAccountRequired => 1009,
                ValidationMessage.CurrencyRequired => 1010,
                ValidationMessage.PasswordRequired => 1011,
                ValidationMessage.WrongFormatPassword => 1012,
                ValidationMessage.CityIdRequired => 1013,
                ValidationMessage.WrongFormatCityId => 1014,
                ValidationMessage.BirthDateRequired => 1015,
                ValidationMessage.WrongFormatDate => 1016,
                ValidationMessage.PhoneNumberRequired => 1017,
                ValidationMessage.WrongFormatRecipientAccount => 1018,
                ValidationMessage.WrongFormatAmount => 1019,
                ValidationMessage.WrongFormatAccount => 1020,
                ValidationMessage.DateToRequired => 1021,
                ValidationMessage.DateFromRequired => 1022,
                _ => 1500
            };
        }
    }
}