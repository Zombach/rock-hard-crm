using CRM.DAL.Models;
using System.Threading.Tasks;

namespace CRM.Business.Services
{
    public interface IAuthenticationService
    {
        public string HashPassword(string pass, byte[] salt = default);
        Task<string> SignIn(LeadDto dto);
        public byte[] GetSalt();
        public bool Verify(string hashedPassword, string leadPassword);
    }
}