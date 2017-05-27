using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestPaperCreator.MODEL.Membership
{
    /// <summary>
    /// 用户类
    /// 作者：廖渝磊
    /// 时间：2017年3月31日01:24:54
    /// </summary>
    [Serializable]
    public class User
    {
        //用户名
        public string username { get; set; }
        //密码
        public string password { get; set; }
        /// <summary>
        /// 用户类型
        /// 1：教师
        /// 2：学生
        /// </summary>
        public int type { get; set; }
        public int rememberme { get; set; }
    }
}
