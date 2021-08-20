using CRM.DAL.Models;

namespace CRM.Business.Services
{
    public interface IAuthenticationService
    {
        string SignIn(LeadDto dto);
    }
}