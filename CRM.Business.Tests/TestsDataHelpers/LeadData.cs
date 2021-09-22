using System;
using System.Collections.Generic;
using CRM.DAL.Enums;
using CRM.DAL.Models;

namespace CRM.Business.Tests.TestsDataHelpers
{
    public static class LeadData
    {
        public static LeadDto GetLeadDto()
        {
            return new LeadDto
            {
                Id = 1,
                FirstName = "QQQ",
                LastName = "QQQ",
                Patronymic = "QQQ",
                RegistrationDate = DateTime.Now,
                Email = $"QQQ{DateTime.Now}@QQQ.qqq",
                PhoneNumber = "+7QQQqqqQQqq",
                Password = "QQQ",
                IsDeleted = false,
                BirthDate = DateTime.Now.AddYears(-18),
                Role = Role.Regular,
                City = CityData.GetCityDto(),
                Accounts = new List<AccountDto>
                {
                    new()
                    {
                        Id = 1,
                        LeadId = 1,
                        Currency = Currency.RUB,
                        CreatedOn = DateTime.Now.AddYears(-1),
                        IsDeleted = false
                    }
                }
            };
        }

        public static LeadDto GetLeadWithTwoAccountsDto()
        {
            return new LeadDto
            {
                Id = 1,
                FirstName = "QQQ",
                LastName = "QQQ",
                Patronymic = "QQQ",
                RegistrationDate = DateTime.Now,
                Email = $"QQQ{DateTime.Now}@QQQ.qqq",
                PhoneNumber = "+7QQQqqqQQqq",
                Password = "QQQ",
                IsDeleted = false,
                BirthDate = DateTime.Now.AddYears(-18),
                Role = Role.Regular,
                City = CityData.GetCityDto(),
                Accounts = new List<AccountDto>
                {
                    new()
                    {
                        Id = 1,
                        LeadId = 1,
                        Currency = Currency.RUB,
                        CreatedOn = DateTime.Now.AddYears(-1),
                        IsDeleted = false
                    },new()
                    {
                        Id = 2,
                        LeadId = 1,
                        Currency = Currency.USD,
                        CreatedOn = DateTime.Now.AddYears(-1),
                        IsDeleted = false
                    }
                }
            };
        }

        public static LeadDto GetAnotherLeadDto()
        {
            return new LeadDto
            {
                Id = 2,
                FirstName = "QQQ",
                LastName = "QQQ",
                Patronymic = "QQQ",
                RegistrationDate = DateTime.Now,
                Email = $"QQQ{DateTime.Now}@QQQ.qqq",
                PhoneNumber = "+7QQQqqqQQqq",
                Password = "QQQ",
                IsDeleted = false,
                BirthDate = DateTime.Now.AddYears(-18),
                Role = Role.Regular,
                City = CityData.GetCityDto(),
                Accounts = new List<AccountDto>
                {
                    new()
                    {
                        Id = 1,
                        LeadId = 1,
                        Currency = Currency.RUB,
                        CreatedOn = DateTime.Now.AddYears(-1),
                        IsDeleted = false
                    }
                }
            };
        }

        public static LeadDto GetInputLeadDto()
        {
            return new LeadDto
            {
                FirstName = "QQQ",
                LastName = "QQQ",
                Patronymic = "QQQ",
                Email = $"QQQ{DateTime.Now}@QQQ.qqq",
                PhoneNumber = "+7QQQqqqQQqq",
                Password = "QQQ",
                BirthDate = DateTime.Now.AddYears(-18),
                City = CityData.GetCityDto(),
                Role = Role.Regular,
            };
        }

        public static List<LeadDto> GetListLeadDto()
        {
            return new List<LeadDto>
            {
                new()
                {
                    Id = 1,
                    FirstName = "QQQ",
                    LastName = "QQQ",
                    Patronymic = "QQQ",
                    RegistrationDate = DateTime.Now,
                    Email = $"QQQ{DateTime.Now}@QQQ.qqq",
                    PhoneNumber = "+7QQQqqqQQqq",
                    Password = "QQQ",
                    IsDeleted = false,
                    BirthDate = DateTime.Now.AddYears(-18),
                    Role = Role.Regular,
                    City = CityData.GetCityDto(),
                    Accounts = new List<AccountDto>
                    {
                        new()
                        {
                            Id = 1,
                            LeadId = 1,
                            Currency = Currency.RUB,
                            CreatedOn = DateTime.Now.AddYears(-1),
                            IsDeleted = false
                        }
                    }
                },
                new()
                {
                    Id = 2,
                    FirstName = "WWW",
                    LastName = "WWW",
                    Patronymic = "WWW",
                    RegistrationDate = DateTime.Now,
                    Email = $"WWW{DateTime.Now}@WWW.www",
                    PhoneNumber = "+7WWWwwwWWww",
                    Password = "WWW",
                    IsDeleted = false,
                    BirthDate = DateTime.Now.AddYears(-18),
                    Role = Role.Regular,
                    City = CityData.GetCityDto(),
                    Accounts = new List<AccountDto>
                    {
                        new()
                        {
                            Id = 2,
                            LeadId = 2,
                            Currency = Currency.RUB,
                            CreatedOn = DateTime.Now.AddYears(-1),
                            IsDeleted = false
                        }
                    }
                }
            };
        }
    }
}