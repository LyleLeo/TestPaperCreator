using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace TestPaperCreator.Controllers.Manage
{
    public class ManageAPIController : ApiController
    {
        [HttpPost]
        public bool EditCondition(dynamic obj)
        {
            string condition = obj.Condition;
            string value = obj.Value;
            int flag = obj.Flag;
            return BLL.TestPaperService.TestPaperService.EditCondition(condition, value, flag);
        }
    }
}
