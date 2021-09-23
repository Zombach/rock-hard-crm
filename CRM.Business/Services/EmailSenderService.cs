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
            await _publishEndpoint.Publish<IMailExchangeModel>(new
            {
                Subject = subject,
                Body = $"{dto.LastName} {dto.FirstName} {body}",
                DisplayName = "Best CRM",
                MailAddresses = $"{dto.Email}"
            });
        }
    }
}