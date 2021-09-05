using CRM.Business.IdentityInfo;
using CRM.Business.Models;

namespace CRM.Business.Services
{
    public interface ITransactionService
    {
        long AddDeposit(TransactionBusinessModel model, LeadIdentityInfo leadInfo);
        long AddWithdraw(TransactionBusinessModel model, LeadIdentityInfo leadInfo);
        string AddTransfer(TransferBusinessModel model, LeadIdentityInfo leadInfo);
    }
}