namespace CRM.Business.Constants
{
    public class TransactionEndpoint
    {
        public const string AddDepositEndpoint = "/api/Transaction/deposit";
        public const string AddWithdrawEndpoint = "/api/Transaction/withdraw";
        public const string AddTransferEndpoint = "/api/Transaction/transfer";
        public const string GetTransactionsByAccountIdEndpoint = "/api/Transaction/by-account/";
        public const string GetTransactionsByPeriodEndpoint = "/api/Transaction/by-period";
        public const string GetCurrentCurrenciesRatesEndpoint = "/api/Transaction/currency-rates";
    }
}