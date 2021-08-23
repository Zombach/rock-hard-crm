namespace CRM.Business
{
    public class TransactionEndpoint
    {
        public const string GetTransactionsByAccountIdEndpoint = "/api/Transaction/by-account/{0}";
        public const string AddDepositEndpoint = "/api/Transaction/deposit";
        public const string AddWithdrawEndpoint = "/api/Transaction/withdraw";
        public const string AddTransferEndpoint = "/api/Transaction/transfer";
    }
}