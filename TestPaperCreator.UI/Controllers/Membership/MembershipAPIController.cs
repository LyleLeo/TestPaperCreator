using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using TestPaperCreator.MODEL.Membership;
using System.Web.SessionState;
using System.Web;
using TestPaperCreator.BLL;

namespace TestPaperCreator.Controllers.Membership
{
    public class MembershipAPIController : ApiController
    {
        [Route("api/MembershipAPI/Register")]
        [HttpPost]
        public TestPaperCreator.MODEL.Utility.Result Register(User user)
        {
            TestPaperCreator.MODEL.Utility.Result result = BLL.Membership.Membership.Regist(user);
            return result;
        }
        [HttpPost]
        public bool Login([FromBody]User user)
        {
            bool result = TestPaperCreator.BLL.Membership.Membership.Login(user);
            if (result == true)
            {
                var rememberme = user.rememberme;
                HttpSessionState session = HttpContext.Current.Session;
                session.Add("user", BLL.Utility.Utility.Serialize<User>(user));
                //session["user"] = TestPaperCreator.BLL.Utility.Utility.Serialize<User>(user);
                if (user.rememberme == 1)
                {
                    HttpCookie cookie = new HttpCookie("UserInfo");
                    cookie.Value = TestPaperCreator.BLL.Utility.Utility.Serialize<User>(user);
                    cookie.Expires = DateTime.Now.AddDays(666);
                    HttpContext.Current.Response.Cookies.Add(cookie);
                }
            }
            return result;
        }
    }
}
