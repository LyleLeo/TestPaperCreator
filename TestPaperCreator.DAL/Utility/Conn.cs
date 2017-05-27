using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestPaperCreator.DAL.Utility
{
    /// <summary>
    /// 数据库连接相关类
    /// 作者：廖渝磊
    /// 时间：2017年3月31日02:03:01
    /// </summary>
    internal static class Conn
    {
        /// <summary>
        /// 获取链接字符串
        /// </summary>
        /// <param name="hostname">主机商</param>
        /// <returns>链接字符串</returns>
        internal static string GetSqlServerConn(string hostname)
        {
            return System.Configuration.ConfigurationManager.ConnectionStrings[hostname].ToString();
        }
    }
}
