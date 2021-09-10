using System;
using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace CRM.API.Common
{
    [AttributeUsage(AttributeTargets.Property)]
    public class CustomDateFormatWithHoursAndMinutesAttribute : ValidationAttribute
    {
        private const string _dateFormat = "dd.MM.yyyy HH:mm";

        public override bool IsValid(object value)
        {
            CultureInfo cultureInfo = CultureInfo.CreateSpecificCulture("ru-RU");
            return value == null || DateTime.TryParseExact(value.ToString(), _dateFormat, cultureInfo, DateTimeStyles.None, out var date);
        }
    }
}