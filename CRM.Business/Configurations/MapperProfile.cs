using AutoMapper;
using CRM.Business.Models;
using CRM.DAL.Models;

namespace CRM.Configuration
{
    public class MapperProfile : Profile
    {
        public MapperProfile()
        {
            CreateMappingFromDto();
        }

        private void CreateMappingFromDto()
        {
            CreateMap<AccountDto, AccountBusinessModel>();
        }
    }
}