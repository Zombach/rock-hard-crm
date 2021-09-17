using CRM.Business.IdentityInfo;
using CRM.Business.Models;
using CRM.DAL.Models;

namespace CRM.Business.Services
{
    public interface ITransactionService
    {
        CommissionFeeDto AddDeposit(TransactionBusinessModel model, LeadIdentityInfo leadInfo);
        CommissionFeeDto AddWithdraw(TransactionBusinessModel model, LeadIdentityInfo leadInfo);
        CommissionFeeDto AddTransfer(TransferBusinessModel model, LeadIdentityInfo leadInfo);
    }
}