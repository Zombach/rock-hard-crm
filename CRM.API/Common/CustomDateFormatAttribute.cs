﻿using System;
using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace CRM.API.Common
{
    [AttributeUsage(AttributeTargets.Property)]
    public class CustomDateFormatAttribute : ValidationAttribute
    {
        private const string _dateFormat = "dd.MM.yyyy";

        public override bool IsValid(object value)
        {
            var cultureInfo = CultureInfo.CreateSpecificCulture("ru-RU");
            return value == null || DateTime.TryParseExact(value.ToString(), _dateFormat, cultureInfo, DateTimeStyles.None, out _);
        }
    }
}