namespace CRM.Business.Constants
{
    public static class ServiceMessages
    {
        public const string EntityNotFoundMessage = "{0} with id = {1} was not found";
        public const string LeadHasNoAccessMessageToAccount = "The lead with id = {0} does not have access to this account";
        public const string LeadHasNoAccessMessageToLead = "The lead with id = {0} does not have access to this lead";
        public const string LeadHasNoAccessMessageByRole = "The lead with id = {0} does not have vip status";
        public const string LeadHasThisCurrencyMessage = "The lead with id = {0} already has an account with that currency";
    }
}