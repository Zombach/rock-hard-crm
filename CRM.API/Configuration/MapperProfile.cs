using AutoMapper;
using CRM.API.Models;
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
        }

        private void CreateMappingToDto()
        {
            CreateMap<CityInputModel, CityDto>();
            CreateMap<AccountInputModel, AccountDto>();
            CreateMap<LeadInputModel, LeadDto>();
            CreateMap<LeadUpdateInputModel, LeadDto>();
        }

        private void CreateMappingFromDto()
        {
            CreateMap<CityDto, CityOutputModel>();
            CreateMap<AccountDto, AccountOutputModel>()
                .ForMember(dest => dest.CreatedOn, opt => opt.MapFrom(src => src.CreatedOn.ToString(_dateFormat)));
            CreateMap<LeadDto, LeadOutputModel>()
                .ForMember(dest => dest.RegistrationDate, opt => opt.MapFrom(src => src.RegistrationDate.ToString(_dateFormat)));
            CreateMap<LeadDto, LeadInfoOutputModel>()
                .ForMember(dest => dest.RegistrationDate, opt => opt.MapFrom(src => src.RegistrationDate.ToString(_dateFormat)));
        }
    }
}