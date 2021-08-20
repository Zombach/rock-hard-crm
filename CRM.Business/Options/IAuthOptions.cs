using Microsoft.IdentityModel.Tokens;

namespace CRM.Business.Options
{
    public interface IAuthOptions
    {
        int Lifetime { get; set; }
        string Issuer { get; }
        string Audience { get; }

        SymmetricSecurityKey GetSymmetricSecurityKey();
    }
}