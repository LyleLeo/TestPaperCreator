using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Runtime.Serialization;
using System.Net.Mail;
using System.Net;

namespace TestPaperCreator.BLL.Utility
{
    /// <summary>
    /// 业务逻辑层公共类
    /// 作者：廖渝磊
    /// 时间：2017年3月31日02:31:35
    /// </summary>
    public class Utility
    {
        #region MD5加密
        /// <summary>
        /// MD5加密
        /// </summary>
        /// <param name="pre">加密前字符</param>
        /// <returns>加密后字符</returns>
        public static string getMD5(string pre)
        {
            byte[] result = Encoding.Default.GetBytes(pre);
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] output = md5.ComputeHash(result);
            string next = BitConverter.ToString(output).Replace("-", "");
            return next;
        }
        #endregion

        #region 序列化与反序列化
        /// <summary>
        /// 序列化
        /// </summary>
        /// <param name="obj">对象</param>
        /// <returns>返回序列化之后的字符串</returns>
        public static string Serialize<T>(T obj)
        {
            BinaryFormatter ser = new BinaryFormatter();
            MemoryStream mStream = new MemoryStream();
            ser.Serialize(mStream, obj);
            byte[] buf = mStream.ToArray();
            mStream.Close();
            return Convert.ToBase64String(buf);
            //return buf.ToString();
        }
        /// <summary>
        /// 反序列化
        /// </summary>
        /// <typeparam name="T">用户给定的类</typeparam>
        /// <param name="obj">用户给定类的对象</param>
        /// <param name="str">序列化的字符串</param>
        /// <returns>返回反序列化后的对象</returns>
        public static T DeSerialize<T>(T obj, string str)
        {
            obj = default(T);
            IFormatter formatter = new BinaryFormatter();
            byte[] buffer = Convert.FromBase64String(str);
            MemoryStream mStream = new MemoryStream(buffer);
            obj = (T)formatter.Deserialize(mStream);
            mStream.Flush();
            mStream.Close();
            return obj;
        }
        #endregion

        #region 发送邮件
        /// <summary>
        /// 发送邮件的方法
        /// </summary>
        /// <param name="useremailaddress">目标邮箱的地址</param>
        /// <param name="emailcontent">邮件内容</param>
        /// <param name="fileaddress">文件地址</param>
        /// <returns>返回结果</returns>
        public static MODEL.Utility.Result SendMail(string useremailaddress, string emailcontent, string fileaddress)
        {
            string myAddress = "593789199@qq.com";
            MailMessage myMail = new MailMessage();
            myMail.From = new MailAddress(myAddress, "廖渝磊的邮箱");
            myMail.To.Add(new MailAddress(useremailaddress));
            myMail.Subject = "廖渝磊的网站示例，邮箱验证信息";
            //myMail.SubjectEncoding = Encoding.UTF8;
            myMail.Body = emailcontent;
            //myMail.BodyEncoding = Encoding.UTF8;
            myMail.IsBodyHtml = true;
            Attachment att = new Attachment(fileaddress);
            myMail.Attachments.Add(att);
            SmtpClient smtp = new SmtpClient();
            smtp.Host = "smtp.qq.com";
            smtp.EnableSsl = true;
            smtp.Credentials = new NetworkCredential("593789199", "ellpxmxsiyizbfbb");
            MODEL.Utility.Result result = new MODEL.Utility.Result();
            try
            {
                smtp.Send(myMail);
                result.result = true;
                return result;
            }
            catch (Exception exception)
            {
                result.result = false;
                result.error = exception.Message;
                return result;
            }
        }
        #endregion

    }
    class EmailEntity
    {
        public bool getreturn { get; set; }
        public string results { get; set; }
    }
}
