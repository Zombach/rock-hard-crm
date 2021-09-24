using CRM.Business.Models;
using Google.Authenticator;
using System;

namespace CRM.Business.Services
{
    public class TwoFactorAuthenticatorService : ITwoFactorAuthenticatorService
    {
        TwoFactorAuthenticator tfa = new TwoFactorAuthenticator();

        public TwoFactorAuthenticatorModel GetTwoFactorAuthenticatorKey()
        {
            string key = Guid.NewGuid().ToString().Replace("-", "").Substring(0, 10);
            
            return new TwoFactorAuthenticatorModel() { Key = key, Tfa = tfa };
        }

        public bool ValidateTwoFactorPIN(string key, string pinCode)
        {
            return tfa.ValidateTwoFactorPIN(key, pinCode);
        }
    }
}
