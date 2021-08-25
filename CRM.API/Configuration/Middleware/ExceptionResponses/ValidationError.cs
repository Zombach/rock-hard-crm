using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CRM.API.Configuration.Middleware.ExceptionResponses
{
    public class ValidationError
    {
        public int Code { get; set; }
        public string Field { get; set; }
        public string Message { get; set; }
    }
}
