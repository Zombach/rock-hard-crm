using CRM.Business.Models;

namespace CRM.Business.Services
{
    public interface ITwoFactorAuthenticatorService
    {
        TwoFactorAuthenticatorModel GetTwoFactorAuthenticatorKey();
        bool ValidateTwoFactorPIN(string key, string pinCode);
    }
}