using CRM.Core;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace CRM.Business.Options
{
    public class AuthOptions : IAuthOptions
    {
        //public const string Issuer = "TheBestCrmEver";
        //public const string Audience = "CrmClient";
        //const string _key = "secret_key_for_token_qqq";
        //public const int Lifetime = 1440;
        public const int WorkFactor = 13;

        private readonly string _key;   // key for encoding last part of the token

        public string Issuer => "TheBestCrmEver"; // for example auth.myserver.com
        public string Audience => "CrmClient"; // for example myserver.com
        public int Lifetime { get; set; } // 5 minutes

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