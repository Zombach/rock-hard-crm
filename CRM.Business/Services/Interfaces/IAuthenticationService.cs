using CRM.DAL.Models;

namespace CRM.Business.Services
{
    public interface IAuthenticationService
    {
        public string HashPassword(string pass, byte[] salt = default);
        string SignIn(LeadDto dto);
        public byte[] GetSalt();
        public bool Verify(string hashedPassword, string userPassword);
    }
}