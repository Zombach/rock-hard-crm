using CRM.Business.Models;
using CRM.Business.Requests;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static CRM.Business.TransactionEndpoint;

namespace CRM.Business.Services
{
    public class TransactionService : ITransactionService
    {
        //private const string BaseEndpoint = "https://localhost:44386/";
        //private const string BaseEndpoint = ConigurationSettings
        //private string _endPoint;

        private readonly RestClient _client;
        private readonly RequestHelper _requestHelper;

        //public List<TransactionBusinessModel> GetTransactionsByAccountId(int id)
        //{
        //    _endPoint = string.Format(GetTransactionsByAccountIdEndpoint, id);
        //    var request = _requestHelper.CreateGetRequest(string.Format(GetTransactionsByAccountIdEndpoint, id));
        //    var response = _client.Execute<List<TransactionBusinessModel>>(request);
        //    return response.Data;
        //}

        public long AddTransaction(TransactionBusinessModel model)
        {
            var request = _requestHelper.CreatePostRequest(AddDepositEndpoint, model);
            var result = _client.Execute<long>(request);
            return result.Data;
        }
    }
}
