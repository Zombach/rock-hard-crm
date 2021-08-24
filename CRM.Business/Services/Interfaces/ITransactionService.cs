using CRM.Business.Models;

namespace CRM.Business.Services
{
    public interface ITransactionService
    {
        long AddDeposit(int accountId, TransactionBusinessModel model);
        long AddWithdraw(int accountId, TransactionBusinessModel model);
        string AddTransfer(int accountId, int recipientId, TransferBusinessModel model);
    }
}