using CRM.Core;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace CRM.Business.Options
{
    public class AuthOptions : IAuthOptions
    {
        private readonly string _key;

        public string Issuer => "TheBestCrmEver";
        public string Audience => "CrmClient";
        public int Lifetime { get; set; }

        public AuthOptions(IOptions<AuthSettings> options)
        {
            _key = options.Value.KeyForToken;
            Lifetime = options.Value.TokenLifeTime;
        }

        public SymmetricSecurityKey GetSymmetricSecurityKey()
        {
            return new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_key));
        }
    }
}