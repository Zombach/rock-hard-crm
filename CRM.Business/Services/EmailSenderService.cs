using CRM.DAL.Models;
using MailExchange;
using MassTransit;
using System.Threading.Tasks;

namespace CRM.Business.Services
{
    public class EmailSenderService : IEmailSenderService
    {
        private readonly IPublishEndpoint _publishEndpoint;

        public EmailSenderService(IPublishEndpoint publishEndpoint)
        {
            _publishEndpoint = publishEndpoint;
        }

        public async Task EmailSenderAsync(LeadDto dto, string subject, string body)
        {
            var mailBody = $"{dto.LastName} {dto.FirstName} {body}";
            await _publishEndpoint.Publish<IMailExchangeModel>(new
            {
                Subject = subject,
                Body = mailBody,
                DisplayName = "Best CRM",
                MailAddresses = $"{dto.Email}",
                IsBodyHtml = false,
                Base64String = string.Empty
            });
        }
        public async Task EmailSenderAsync(LeadDto dto, string subject, string body, string base64Image)
        {
            await _publishEndpoint.Publish<IMailExchangeModel>(new
            {
                Subject = subject,
                Body = $"{dto.LastName} {dto.FirstName} {body}",
                DisplayName = "Best CRM",
                MailAddresses = $"{dto.Email}",
                Base64String = base64Image
            });
        }
    }
}