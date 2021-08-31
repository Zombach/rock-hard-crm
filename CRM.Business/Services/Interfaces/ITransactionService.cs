using CRM.Business.Models;

namespace CRM.Business.Services
{
    public interface ITransactionService
    {
        long AddDeposit(TransactionBusinessModel model);
        long AddWithdraw(TransactionBusinessModel model);
        string AddTransfer(TransferBusinessModel model);
    }
}