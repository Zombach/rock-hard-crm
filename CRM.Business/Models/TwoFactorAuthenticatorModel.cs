using Google.Authenticator;

namespace CRM.Business.Models
{
    public class TwoFactorAuthenticatorModel
    {
        public string Key { get; set; }
        public TwoFactorAuthenticator Tfa { get; set; }
    }
}
