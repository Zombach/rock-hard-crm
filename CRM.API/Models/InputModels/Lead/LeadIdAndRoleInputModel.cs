using CRM.DAL.Enums;

namespace CRM.API.Models
{
    public class LeadIdAndRoleInputModel
    {
        public int Id { get; set; }
        public Role Role { get; set; }
    }
}
