using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestPaperCreator.MODEL.TestPaper
{
    public class Question
    {
        public int ID { get; set; }
        public int Type { get; set; }
        public string TypeName { get; set; }
        public int Course { get; set; }
        public string CourseName { get; set; }
        public int Section { get; set; }
        public string SectionName { get; set; }
        public int Difficulty { get; set; }
        public string DifficultyName { get; set; }
        public string Content { get; set; }
    }
}
