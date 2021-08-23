using RestSharp;

namespace CRM.Business.Requests
{
    public class RequestHelper
    {
        public IRestRequest CreateRequest(Method httpMethod, string endPoint)
        {
            return new RestRequest(endPoint, httpMethod);
        }
    }
}