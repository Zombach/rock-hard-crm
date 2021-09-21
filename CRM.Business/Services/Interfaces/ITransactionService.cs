using CRM.Business.IdentityInfo;
using CRM.Business.Models;
using CRM.DAL.Models;
using System.Threading.Tasks;

namespace CRM.Business.Services
{
    public interface ITransactionService
    {
        Task<CommissionFeeDto> AddDepositAsync(TransactionBusinessModel model, LeadIdentityInfo leadInfo);
        Task<CommissionFeeDto> AddWithdrawAsync(TransactionBusinessModel model, LeadIdentityInfo leadInfo);
        Task<CommissionFeeDto> AddTransferAsync(TransferBusinessModel model, LeadIdentityInfo leadInfo);
        Task CheckTransferAndSendEmailAsync(TransferBusinessModel model, LeadIdentityInfo leadInfo);
        Task CheckTransactionAndSendEmailAsync(TransactionBusinessModel model, LeadIdentityInfo leadInfo);
        Task<CommissionFeeDto> ContinueTransaction(LeadIdentityInfo leadInfo);
    }
}