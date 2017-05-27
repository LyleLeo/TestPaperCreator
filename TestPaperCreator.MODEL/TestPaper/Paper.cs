using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestPaperCreator.MODEL.TestPaper
{
    public class Paper
    {
        //条件
        public Property paperproperty { get; set; }
        //数量
        public int count { get; set; }
    }
    public class Property
    {
        public int questiontype { get; set; }
        public int course { get; set; }
        public int section { get; set; }
        public int difficulty { get; set; }
    }
}
