using AutoMapper;
using CRM.API.Models;
using CRM.Business.Models;
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
        }

        private void CreateMappingToDto()
        {
            CreateMap<CityInputModel, CityDto>();
            CreateMap<TimeBasedAcquisitionInputModel, TimeBasedAcquisitionDto>();
            CreateMap<TimeBasedAcquisitionSearchingInputModes, TimeBasedAcquisitionDto>();
            CreateMap<AccountInputModel, AccountDto>();
            CreateMap<LeadSignInModel, LeadDto>();
            CreateMap<LeadUpdateInputModel, LeadDto>()
                .ForMember(dest => dest.BirthDate, opt => opt.MapFrom(src => DateTime.ParseExact(src.BirthDate, _dateFormat, CultureInfo.InvariantCulture)))
                .ForMember(dest => dest.City, opt => opt.MapFrom(src => new CityDto { Id = src.CityId }));
            CreateMap<LeadInputModel, LeadDto>()
                .ForMember(dest => dest.BirthDate, opt => opt.MapFrom(src => DateTime.ParseExact(src.BirthDate, _dateFormat, CultureInfo.InvariantCulture)))
                .ForMember(dest => dest.City, opt => opt.MapFrom(src => new CityDto { Id = src.CityId }));
            CreateMap<LeadFiltersInputModel, LeadFiltersDto>()
                .ForMember(dest => dest.BirthDateFrom, opt => opt.MapFrom(src => DateTime.ParseExact(src.BirthDateFrom, _dateFormat, CultureInfo.InvariantCulture)))
                .ForMember(dest => dest.BirthDateTo, opt => opt.MapFrom(src => DateTime.ParseExact(src.BirthDateTo, _dateFormat, CultureInfo.InvariantCulture)));
            CreateMap<LeadIdAndRoleInputModel, LeadDto>();
        }

        private void CreateMappingFromDto()
        {
            CreateMap<CityDto, CityOutputModel>();
            CreateMap<CommissionFeeDto, CommissionFeeOutputModel>();
            CreateMap<CommissionFeeDto, CommissionFeeShortOutputModel>();
            CreateMap<AccountDto, AccountOutputModel>()
                .ForMember(dest => dest.CreatedOn, opt => opt.MapFrom(src => src.CreatedOn.ToString(_dateFormat)));
            CreateMap<LeadDto, LeadOutputModel>()
                .ForMember(dest => dest.RegistrationDate, opt => opt.MapFrom(src => src.RegistrationDate.ToString(_dateFormat)))
                .ForMember(dest => dest.BirthDate, opt => opt.MapFrom(src => src.BirthDate.ToString(_dateFormat)));
            CreateMap<LeadDto, LeadByFiltersOutputModel>()
                .ForMember(dest => dest.BirthDate, opt => opt.MapFrom(src => src.BirthDate.ToString(_dateFormat)));
            CreateMap<LeadDto, LeadByBatchesOutputModel>()
                .ForMember(dest => dest.BirthDate, opt => opt.MapFrom(src => src.BirthDate.ToString(_dateFormat)));
            CreateMap<TransactionBusinessModel, TransactionOutputModel>();
        }

        private void CreateMappingToBusiness()
        {
            CreateMap<TransactionInputModel, TransactionBusinessModel>();
            CreateMap<TransferInputModel, TransferBusinessModel>();
            CreateMap<TimeBasedAcquisitionInputModel, TimeBasedAcquisitionBusinessModel>();
        }
    }
}