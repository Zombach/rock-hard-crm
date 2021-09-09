using AutoMapper;
using CRM.API.Models;
using CRM.Business.Models;
using CRM.DAL.Models;

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
            CreateMap<LeadInputModel, LeadDto>()
                .ForMember(dest => dest.City, opt => opt.MapFrom(src => new CityDto { Id = src.CityId }));
            CreateMap<LeadUpdateInputModel, LeadDto>();
        }

        private void CreateMappingFromDto()
        {
            CreateMap<CityDto, CityOutputModel>();
            CreateMap<CommissionFeeDto, CommissionFeeOutputModel>();
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
    }
}