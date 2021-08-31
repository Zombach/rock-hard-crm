using AutoMapper;
using CRM.API.Models;
using CRM.Business.Models;
using CRM.Business.FilterModels;
using CRM.DAL.Models;
using System;
using System.Globalization;

namespace CRM.API.Configuration
{
    public class MapperProfile : Profile
    {
        private const string _dateFormat = "dd.MM.yyyy";

        public MapperProfile()
        {
            CreateMappingToDto();
            CreateMappingFromDto();
            CreateMappingToBusiness();
            CreateMappingFromBusiness();
        }

        private void CreateMappingToDto()
        {
            CreateMap<CityInputModel, CityDto>();
            CreateMap<AccountInputModel, AccountDto>();
            CreateMap<LeadSignInModel, LeadDto>();
            CreateMap<LeadInputModel, LeadDto>()
                .ForMember(dest => dest.City, opt => opt.MapFrom(src => new CityDto { Id = src.CityId }));
            CreateMap<LeadUpdateInputModel, LeadDto>();
            //.ForMember(dest => dest.City, opt => opt.MapFrom(src => new CityDto { Id = src.CityId }));
            CreateMap<LeadFilterInputModel, LeadFilterModel>()
                .ForMember(dest => dest.SearchType, opt => opt.MapFrom(src => src.SearchType != null ? src.SearchType : (int)SearchType.StartsWith))
                .ForMember(dest => dest.BirthDateFrom, opt => opt.MapFrom(src => DateTime.ParseExact(src.BirthDateFrom, _dateFormat, CultureInfo.InvariantCulture)))
                .ForMember(dest => dest.BirthDateTo, opt => opt.MapFrom(src => DateTime.ParseExact(src.BirthDateTo, _dateFormat, CultureInfo.InvariantCulture)));
        }

        private void CreateMappingFromDto()
        {
            CreateMap<CityDto, CityOutputModel>();
            CreateMap<AccountDto, AccountOutputModel>()
                .ForMember(dest => dest.CreatedOn, opt => opt.MapFrom(src => src.CreatedOn.ToString(_dateFormat)));
            CreateMap<LeadDto, LeadOutputModel>()
                .ForMember(dest => dest.RegistrationDate, opt => opt.MapFrom(src => src.RegistrationDate.ToString(_dateFormat)))
                .ForMember(dest => dest.BirthDate, opt => opt.MapFrom(src => src.BirthDate.ToString(_dateFormat)));
        }

        private void CreateMappingToBusiness()
        {
            CreateMap<TransactionInputModel, TransactionBusinessModel>();
            CreateMap<TransactionInputModel, TransferBusinessModel>();
        }

        private void CreateMappingFromBusiness()
        {
            CreateMap<TransactionBusinessModel, TransactionInputModel>();
        }
    }
}