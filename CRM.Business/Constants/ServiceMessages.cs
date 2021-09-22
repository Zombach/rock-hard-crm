namespace CRM.Business.Constants
{
    public static class ServiceMessages
    {
        public const string EntityNotFoundMessage = "{0} with id = {1} was not found";
        public const string LeadHasNoAccessMessageToAccount = "The lead with id = {0} does not have access to account with id {1}";
        public const string LeadHasNoAccessMessageToLead = "The lead with id = {0} does not have access to this lead";
        public const string LeadHasNoAccessMessageByRole = "The lead with id = {0} does not have vip status";
        public const string LeadHasThisCurrencyMessage = "The lead with id = {0} already has an account with that currency";
        public const string EntityNotFound = "Entity Not Found";
        public const string WrongPassword = "WrongPassword";
        public const string DoesNotHaveEnoughMoney = "There is not enough money on the account with id = {0}, the balance is {1}";
        public const string DoesNotHaveTransactions = "There is not transactions on the account with id = {0}";
        public const string DuplicateTransactions = "This is duplicate transaction on the account with id = {0}";
        public const string IncompleteTransfer = "Not vip can transfer only the entire amount at once";
        public const string NoAdminRights = "admin access only";
    }
}