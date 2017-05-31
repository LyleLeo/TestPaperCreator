using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestPaperCreator.BLL.TestPaperService
{
    public class TestPaperService
    {
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
        public static MODEL.TestPaper.Question GetOneQuestion(MODEL.TestPaper.Paper paper,List<int> oldidlist)
        {
            return DAL.TestPaperService.TestPaperService.GetOneQuestion(paper, oldidlist);
        }
    }
}
