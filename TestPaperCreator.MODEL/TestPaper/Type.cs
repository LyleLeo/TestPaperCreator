using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestPaperCreator.MODEL.TestPaper
{
    public class SingleDaTi
    {
        public int Type { get; set; }
        public int Count { get; set; }
        public int Score { get; set; }
    }
    public class EmbedIDCounter
    {
        public int MaxrId { get; set; }
        public int MaxshapeId { get; set; }
        public int MaxshapeType { get; set; }
    }
}
