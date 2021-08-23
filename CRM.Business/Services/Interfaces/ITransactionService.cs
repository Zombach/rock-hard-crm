using CRM.Business.Models;

namespace CRM.Business.Services
{
    public interface ITransactionService
    {
        long AddTransaction(TransactionBusinessModel model);
    }
}