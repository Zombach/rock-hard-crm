using CRM.Business.IdentityInfo;
using CRM.Business.Models;

namespace CRM.Business.Services
{
    public interface ITransactionService
    {
        TransactionBusinessModel AddDeposit(TransactionBusinessModel model, LeadIdentityInfo leadInfo);
        TransactionBusinessModel AddWithdraw(TransactionBusinessModel model, LeadIdentityInfo leadInfo);
        TransferBusinessModel AddTransfer(TransferBusinessModel model, LeadIdentityInfo leadInfo);
    }
}