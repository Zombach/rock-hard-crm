namespace MailExchange
{
    public interface IMailExchangeModel
    {
        public string MailTo { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
    }
}