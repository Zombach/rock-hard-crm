using CRM.Business.Models;
using Google.Authenticator;

namespace CRM.Business.Tests.TestsDataHelpers
{
   public static class TwoFactorAuthenticatorModelData
   {
       public static TwoFactorAuthenticatorModel GeTwoFactorAuthenticatorModel() => new()
       {
           Key = "thynmutrfew324567kuytnrb",
           Tfa = new TwoFactorAuthenticator()
           {
               
           }
       };
   }
}
