using CRM.DAL.Models;
using System.Threading.Tasks;

namespace CRM.Business.Services
{
    public interface IEmailSenderService
    {
        
        Task EmailSenderAsync(LeadDto dto, string subject, string body, string base64Image);
        Task EmailSenderAsync(LeadDto dto, string subject, string body);
    }
}