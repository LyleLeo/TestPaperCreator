using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;

namespace TestPaperCreator.BLL.TestPaperService
{
    public class TestPaperService
    {
        public static IDictionary<string,string> GetProperty()
        {
            return DAL.TestPaperService.TestPaperService.GetProperty();
        }
        public static List<MODEL.TestPaper.Condition> GetCourse()
        {
            return DAL.TestPaperService.TestPaperService.GetCourse();
        }
        public static List<MODEL.TestPaper.Condition> GetDifficulty()
        {
            return DAL.TestPaperService.TestPaperService.GetDifficulty();
        }
        public static List<MODEL.TestPaper.Condition> GetSection()
        {
            return DAL.TestPaperService.TestPaperService.GetSection();
        }
        public static List<MODEL.TestPaper.Condition> GetQuestionType()
        {
            return DAL.TestPaperService.TestPaperService.GetQuestionType();
        }

        public static List<MODEL.TestPaper.Question> GetQuestionList(int course, int section, int difficulty, int questiontype, string keyword)
        {
            return DAL.TestPaperService.TestPaperService.GetQuestionList(course, section, difficulty, questiontype, keyword);
        }
        public static bool DeleteQuestion(int questionid)
        {
            return DAL.TestPaperService.TestPaperService.DeleteQuestion(questionid);
        }
        public static bool EditCondition(string Condition, string Value, int Flag)
        {
            return DAL.TestPaperService.TestPaperService.EditCondition(Condition, Value, Flag);
        }
        public static void ClearQuestionMark(string tobecleared)
        {
            DAL.TestPaperService.TestPaperService.ClearQuestionMark(tobecleared);
        }
        public static MODEL.TestPaper.Question GetOneQuestion(MODEL.TestPaper.Paper paper, List<int> oldidlist)
        {
            return DAL.TestPaperService.TestPaperService.GetOneQuestion(paper, oldidlist);
        }
        public static List<MODEL.TestPaper.Condition> GetMajor()
        {
            return DAL.TestPaperService.TestPaperService.GetMajor();
        }
        public static void CopyFiles(Dictionary<int,int> questions,string rootpath, Dictionary<int, MODEL.TestPaper.SingleDaTi> type)
        {
            if (!Directory.Exists(rootpath + @"\Upload\OUT\A\"))
            {
                Directory.CreateDirectory(rootpath + @"\Upload\OUT\A\");
            }
            if (!Directory.Exists(rootpath + @"\Upload\OUT\B\"))
            {
                Directory.CreateDirectory(rootpath + @"\Upload\OUT\B\");
            }
            string[] templates = Directory.GetFiles(rootpath + @"\Files\", "*.dotx");
            foreach(string template in templates)
            {
                string filename = Path.GetFileName(template);
                File.Copy(template, rootpath+@"\Upload\OUT\A\"+filename, true);
                File.Copy(template, rootpath + @"\Upload\OUT\B\" + filename, true);
            }
            foreach (int datitihao in type.Keys)
            {
                foreach (int tihao in questions.Keys)
                {
                    if(tihao<=questions.Keys.Count()/2)
                    {
                        MODEL.TestPaper.Question question = DAL.TestPaperService.TestPaperService.GetAQuestionByID(questions[tihao]);
                        string course = question.Course.ToString();
                        string section = question.Section.ToString();
                        string questiontype = question.Type.ToString();
                        string difficulty = question.Difficulty.ToString();
                        if (questiontype == type[datitihao].Type.ToString())
                        {
                            string filepath = rootpath + @"\Upload\" + course + @"\" + section + @"\" + questiontype + @"\" + difficulty + @"\" + questions[tihao].ToString() + ".docx";
                            if (!Directory.Exists(rootpath + @"\Upload\OUT\A\" + datitihao.ToString()))
                            {
                                Directory.CreateDirectory(rootpath + @"\Upload\OUT\A\" + datitihao.ToString());
                            }
                            string distpath = rootpath + @"\Upload\OUT\A\" + datitihao.ToString() + @"\" + tihao.ToString() + ".docx";
                            File.Copy(filepath, distpath, true);
                        }
                    }
                    else
                    {
                        MODEL.TestPaper.Question question = DAL.TestPaperService.TestPaperService.GetAQuestionByID(questions[tihao]);
                        string course = question.Course.ToString();
                        string section = question.Section.ToString();
                        string questiontype = question.Type.ToString();
                        string difficulty = question.Difficulty.ToString();
                        if (questiontype == type[datitihao].Type.ToString())
                        {
                            string filepath = rootpath + @"\Upload\" + course + @"\" + section + @"\" + questiontype + @"\" + difficulty + @"\" + questions[tihao].ToString() + ".docx";
                            if (!Directory.Exists(rootpath + @"\Upload\OUT\B\" + datitihao.ToString()))
                            {
                                Directory.CreateDirectory(rootpath + @"\Upload\OUT\B\" + datitihao.ToString());
                            }
                            string distpath = rootpath + @"\Upload\OUT\B\" + datitihao.ToString() + @"\" + (tihao-questions.Keys.Count()/2).ToString() + ".docx";
                            File.Copy(filepath, distpath, true);
                        }
                    }
                    
                }
            }
        }
    }
}
