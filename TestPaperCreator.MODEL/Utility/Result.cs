using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestPaperCreator.MODEL.Utility
{
    /// <summary>
    /// 结果通知
    /// 作者：廖渝磊
    /// 时间：2017年3月31日01:48:47
    /// </summary>
    public class Result
    {
        /// <summary>
        /// 结果类型
        /// true：正确
        /// false：错误
        /// </summary>
        public bool result { get; set; }
        /// <summary>
        /// 错误原因
        /// </summary>
        public string error { get; set; }
    }
}
