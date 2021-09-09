namespace CRM.API.Common
{
    public static class ValidationMessage
    {
        public const string FirstNameRequired = "FirstName must be provided";
        public const string LastNameRequired = "LastName must be provided";
        public const string PatronymicRequired = "Patronymic must be provided";
        public const string EmailRequired = "Email must be provided";
        public const string NameRequired = "Name must be provided";
        public const string AmountRequired = "Amount must be provided";
        public const string AccountRequired = "Account id must be provided";
        public const string RecipientAccountRequired = "Recipient account id must be provided";
        public const string CurrencyRequired = "Currency must be provided";
        public const string PasswordRequired = "Password must be provided";
        public const string CityIdRequired = "CityId must be provided";
        public const string BirthDateRequired = "BirthDate must be provided";
        public const string PhoneNumberRequired = "PhoneNumber must be provided";
        public const string DateToRequired = "Date must be provided";
        public const string DateFromRequired = "From Date must be provided";

        public const string WrongFormatAmount = "Amount must be integer from 1 to int.MaxValue";
        public const string WrongFormatAccount = "Account id must be integer from 1 to int.MaxValue";
        public const string WrongFormatCityId = "CityId must be integer from 1 to int.MaxValue";
        public const string WrongFormatLeadId = "LeadId must be integer from 1 to int.MaxValue";
        public const string WrongFormatRecipientAccount = "Recipient id must be integer from 1 to int.MaxValue";

        public const string WrongEmailFormat = "You've got to use the correct email format";

        public const string WrongFormatPassword = "Password must contain at least 8 characters";

        public const string WrongFormatDate = "Date must be format 'dd.MM.yyyy'";
        public const string WrongFormatDateWithHoursAndMinutes = "Date must be format 'dd.MM.yyyy HH:mm'";
    }
}