using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Text;

namespace TestPaperCreator.Controllers.TestPaperService
{
    public class ViewQuestionAPIController : ApiController
    {
        [HttpPost]
        public bool DeleteQuestion(dynamic obj)
        {
            return BLL.TestPaperService.TestPaperService.DeleteQuestion(Convert.ToInt32(obj.questionid));
        }
        
        public IHttpActionResult GetQuestionList()
        {
            StringBuilder sb = new StringBuilder(@"{'data':[");
            
            List<MODEL.TestPaper.Question> questionlist = BLL.TestPaperService.TestPaperService.GetQuestionList(0,0,0,0,null);
            foreach(MODEL.TestPaper.Question q in questionlist)
            {
                sb.Append("{'id':" + q.ID.ToString() + ",'Type':" + q.Type.ToString() + ",'TypeName':" + q.TypeName.ToString() + ",'Course':" + q.Course.ToString() + ",'CourseName':" + q.CourseName.ToString() + ",'Section':" + q.Section + ",'SectionName':" + q.SectionName.ToString() + ",'Difficulty':" + q.Difficulty.ToString() + ",'DifficultyName':" + q.DifficultyName.ToString() + ",'Content':" + q.Content.ToString() + "},");
            }
            sb.Append("]}");
            JsonResult jr = new JsonResult();
            jr.data = questionlist;
            return Json(jr);
        }
    }
    public class JsonResult
    {
        public List<MODEL.TestPaper.Question> data;
    }
}
