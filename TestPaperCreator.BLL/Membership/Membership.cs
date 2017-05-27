using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestPaperCreator.DAL;
using TestPaperCreator.MODEL;

namespace TestPaperCreator.BLL.Membership
{
    /// <summary>
    /// 业务逻辑层用户相关
    /// 作者：廖渝磊
    /// 时间：2017年3月31日02:14:16
    /// </summary>
    public class Membership
    {
        /// <summary>
        /// 验证用户名是否存在
        /// </summary>
        /// <param name="username">用户名</param>
        /// <returns>true：不存在；false：存在</returns>
        public static bool VerifyUsernameExist(string username)
        {
            return DAL.Membership.Membership.VerifyUsernameExist(username);
        }
        /// <summary>
        /// 登录功能
        /// </summary>
        /// <param name="user">用户对象</param>
        /// <returns>true：成功；false：失败</returns>
        public static bool Login(MODEL.Membership.User user)
        {
            bool result = DAL.Membership.Membership.Login(user);
            return result;
        }
        /// <summary>
        /// 注册功能
        /// </summary>
        /// <param name="user">用户对象</param>
        /// <returns>true：成功；false：失败</returns>
        public static MODEL.Utility.Result Regist(MODEL.Membership.User user)
        {
            MODEL.Utility.Result result = DAL.Membership.Membership.Register(user);
            return result;
        }
    }
}
