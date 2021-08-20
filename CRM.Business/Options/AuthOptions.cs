using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace CRM.Business.Options
{
    public class AuthOptions
    {
        public const string Issuer = "TheBestCrmEver";
        public const string Audience = "CrmClient";
        const string _key = "secret_key_for_token_qqq";
        public const int Lifetime = 1440;
        public const int WorkFactor = 13;

        public static SymmetricSecurityKey GetSymmetricSecurityKey()
        {
            return new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_key));
        }
    }
}