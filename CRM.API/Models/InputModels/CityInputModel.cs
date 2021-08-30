using System.ComponentModel.DataAnnotations;
using static CRM.API.Common.ValidationMessage;

namespace CRM.API.Models
{
    public class CityInputModel
    {
        [Required(ErrorMessage = NameRequired)]
        public string Name { get; set; }
    }
}