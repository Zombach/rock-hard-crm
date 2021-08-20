using CRM.DAL.Enums;

namespace CRM.API.Models
{
    public class LeadSignInModel
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }
}