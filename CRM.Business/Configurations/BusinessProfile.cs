using AutoMapper;
using CRM.Business.Models;
using CRM.DAL.Models;

namespace CRM.Business.Configuration
{
    public class BusinessProfile : Profile
    {
        public BusinessProfile()
        {
            CreateMappingFromDto();
        }

        private void CreateMappingFromDto()
        {
            CreateMap<AccountDto, AccountBusinessModel>();
        }
    }
}