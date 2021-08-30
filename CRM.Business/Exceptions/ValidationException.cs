using System;

namespace CRM.Business.Exceptions
{
    public class ValidationException : Exception
    {
        public string FieldOfError { get; set; }
        public ValidationException(string fieldOfError, string message) : base(message)
        {
            FieldOfError = fieldOfError;
        }
    }
}