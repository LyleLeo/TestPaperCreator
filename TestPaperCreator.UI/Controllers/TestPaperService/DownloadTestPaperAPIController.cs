using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using TestPaperCreator.MODEL;
using System.IO;
using System.Web;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace TestPaperCreator.Controllers.TestPaperService
{
    public class DownloadTestPaperAPIController : ApiController
    {
        [HttpPost]
        [Route("api/DownloadTestPaperAPI/GetQuestionID/")]
        public List<MODEL.TestPaper.Question> GetQuestionID([FromBody]List<MODEL.TestPaper.Paper> paperlist)
        {
            string localPath = Path.Combine(HttpRuntime.AppDomainAppPath, "Upload");
            return BLL.OfficeHelper.WordDocumentMerger.GetPaperQuestionList(paperlist, localPath);
        }
        [HttpPost]
        public MODEL.TestPaper.Question GetOneQuestion(dynamic obj)
        {
            List<int> oldidlist = new List<int>();
            JArray jarray = obj.oldidlist;
            foreach(var item in jarray)
            {
                int i = Convert.ToInt32(item.ToString());
                oldidlist.Add(i);
                //oldidlist.Add(item.Value(int))
            }
            
            int oldquestionid = Convert.ToInt32(obj.oldquestionid);
            //MODEL.TestPaper.Paper paper = new MODEL.TestPaper.Paper();
            //MODEL.TestPaper.Property property = new MODEL.TestPaper.Property();
            //property = Newtonsoft.Json.JsonConvert.DeserializeObject<MODEL.TestPaper.Property>(Convert.ToString(obj.paper.Property));
            //paper.count = Newtonsoft.Json.JsonConvert.DeserializeObject<MODEL.TestPaper.Paper>(Convert.ToString(obj.paper.count));
            MODEL.TestPaper.Paper paper = Newtonsoft.Json.JsonConvert.DeserializeObject<MODEL.TestPaper.Paper>(Convert.ToString(obj.paper));
            return BLL.TestPaperService.TestPaperService.GetOneQuestion(paper, oldidlist);
        }
    }
}
