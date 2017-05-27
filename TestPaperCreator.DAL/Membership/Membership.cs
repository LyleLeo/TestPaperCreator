using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestPaperCreator.MODEL;

namespace TestPaperCreator.DAL.Membership
{
    /// <summary>
    /// 数据链路层，用户相关方法
    /// </summary>
    public class Membership
    {
        #region 用户注册
        /// <summary>
        /// 用户注册
        /// </summary>
        /// <param name="user">注册用户填写的内容</param>
        /// <returns>true：成功；false：失败</returns>
        public static MODEL.Utility.Result Register(MODEL.Membership.User user)
        {
            string conn = Utility.Conn.GetSqlServerConn("ALiConnection");
            string username = user.username;
            string password = user.password;
            int usertype = user.type;
            string sql = "insert into [dbo].[LocalUser] (UserName, UserType) values ('" + username + "', '" + usertype + "')";
            MODEL.Utility.Result result = new MODEL.Utility.Result();
            try
            {
                Utility.SqlHelper.ExecuteNonQuery(conn, System.Data.CommandType.Text, sql);
                sql = "select ID from [dbo].[LocalUser] where username = '" + username + "'";
                var results = Utility.SqlHelper.ExecuteScalar(conn, System.Data.CommandType.Text, sql);
                sql = "insert into [dbo].[LocalAuth] (UserID, Password, Login) values (" + results + ", '" + password + "', 0)";
                Utility.SqlHelper.ExecuteNonQuery(conn, System.Data.CommandType.Text, sql);
                result.result = true;
                return result;
            }
            catch (Exception e)
            {
                result.result = false;
                result.error = e.Message;
            }

            return result;
        }
        #endregion

        #region 验证用户名是否存在
        /// <summary>
        /// 验证用户名是否存在
        /// </summary>
        /// <param name="username">用户名</param>
        /// <returns>true:不存在；false:已存在</returns>
        public static bool VerifyUsernameExist(string username)
        {
            string conn = Utility.Conn.GetSqlServerConn("ALiConnection");
            string sql = "select * from [dbo].[LocalUser] where username = '" + username + "'";
            var result = Utility.SqlHelper.ExecuteScalar(conn, System.Data.CommandType.Text, sql);
            return result == null ? true : false;
        }
        #endregion

        #region 验证注册邮箱是否存在
        /// <summary>
        /// 验证注册邮箱是否存在
        /// </summary>
        /// <param name="email">用户的邮箱地址</param>
        /// <returns>不存在：true；存在：false</returns>
        public static bool VerifyEmailExist(string email)
        {
            string conn = Utility.Conn.GetSqlServerConn("ALiConnection");
            string sql = "select * from [dbo].[LocalUser] where Email = '" + email + "'";
            var result = Utility.SqlHelper.ExecuteScalar(conn, System.Data.CommandType.Text, sql);
            return result == null ? true : false;
        }
        #endregion

        #region 用户登录
        /// <summary>
        /// 用户登录
        /// </summary>
        /// <param name="user">用户属性</param>
        /// <returns>true:登录成功;false:登录失败</returns>
        public static bool Login(MODEL.Membership.User user)
        {
            string conn = Utility.Conn.GetSqlServerConn("ALiConnection");
            int UserID;
            UserID = GetUserIdByUserName(user.username);
            if (UserID == -1)
            {
                return false;
            }
            string sql = "select * from [dbo].[LocalAuth] where [UserID] = " + UserID + " and [Password] = '" + user.password.ToString() + "'";
            var result = Utility.SqlHelper.ExecuteScalar(conn, System.Data.CommandType.Text, sql);
            if (result != null)
            {
                AddLoginTimes(UserID);
            }
            return result == null ? false : true;
        }
        #endregion

        #region 根据用户名获取用户ID
        /// <summary>
        /// 根据用户名获取用户ID
        /// </summary>
        /// <param name="username">用户名</param>
        /// <returns>用户ID</returns>
        private static int GetUserIdByUserName(string username)
        {
            string conn = Utility.Conn.GetSqlServerConn("ALiConnection");
            string sql = "select [ID] from [dbo].[LocalUser] where username = '" + username + "'";
            var result = Utility.SqlHelper.ExecuteScalar(conn, System.Data.CommandType.Text, sql);
            return result == null ? -1 : (int)result;
        }
        #endregion

        #region 根据Email获取用户ID
        /// <summary>
        /// 根据Email获取用户ID
        /// </summary>
        /// <param name="username">用户输入的用户名参数</param>
        /// <returns>用户ID</returns>
        private static int GetUserIdByEmail(string username)
        {
            string conn = Utility.Conn.GetSqlServerConn("ALiConnection");
            string sql = "select [ID] from [dbo].[LocalUser] where Email = '" + username + "'";
            var result = Utility.SqlHelper.ExecuteScalar(conn, System.Data.CommandType.Text, sql);
            return result == null ? -1 : (int)result;
        }
        #endregion

        #region 记录用户的登录次数
        /// <summary>
        /// 记录用户的登录次数
        /// </summary>
        /// <param name="UserID">用户ID</param>
        /// <returns>是否成功</returns>
        private static bool AddLoginTimes(int UserID)
        {
            int logintimes;
            string conn = Utility.Conn.GetSqlServerConn("ALiConnection");
            string sql = "select [Login] from [dbo].[LocalUser] where [ID] = " + UserID;
            var result = Utility.SqlHelper.ExecuteScalar(conn, System.Data.CommandType.Text, sql);
            if (Convert.ToInt32(result) == 0)
            {
                logintimes = 1;
            }
            else
            {
                logintimes = Convert.ToInt32(result) + 1;
            }
            sql = "update [dbo].[LocalUser] set [Login] = " + logintimes + " where [ID] = " + UserID;
            try
            {
                Utility.SqlHelper.ExecuteNonQuery(conn, System.Data.CommandType.Text, sql);
                return true;
            }
            catch
            {
                return false;
            }
        }
        #endregion
    }
}
