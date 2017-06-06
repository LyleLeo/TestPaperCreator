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
        [HttpPost]
        [Route("api/DownloadTestPaperAPI/Generate/")]
        public string Generate(dynamic obj)
        {
            string rootpath = System.Web.HttpContext.Current.Request.MapPath("/");
            JArray questionlist = obj.questionlist;
            Dictionary<int, int> questions = new Dictionary<int, int>();
            foreach(var q in questionlist)
            {
                questions.Add(Convert.ToInt32(q["xiaotitihao"]), Convert.ToInt32(q["tihaoid"]));
            }

            JArray typelist = obj.typelist;
            Dictionary<int, MODEL.TestPaper.SingleDaTi> type = new Dictionary<int, MODEL.TestPaper.SingleDaTi>();
            foreach (var t in typelist)
            {
                MODEL.TestPaper.SingleDaTi dati = new MODEL.TestPaper.SingleDaTi();
                dati.Type = Convert.ToInt32(t["singledati"]["type"]);
                dati.Count = Convert.ToInt32(t["singledati"]["count"]);
                dati.Score = Convert.ToInt32(t["singledati"]["score"]);
                type.Add(Convert.ToInt32(t["tihao"]), dati);
            }
            BLL.TestPaperService.TestPaperService.CopyFiles(questions, rootpath, type);
            string paperhead = rootpath + @"\Upload\OUT\TestPaperHead.dotx";
            string paperbody = rootpath + @"\Upload\OUT\TestPaperBody.dotx";
            string strCopyFolder = rootpath + @"\Upload\OUT\";
            BLL.Utility.OpenXmlForOffice.CreatePaper(paperhead, paperbody, strCopyFolder, type);
            string resultname = Path.GetFileName(Directory.GetFiles(rootpath + @"\Upload\OUT\final\")[0]);
            return resultname;
        }
    }
    class DaTi
    {
        string tihao { get; set; }
        MODEL.TestPaper.SingleDaTi singledati { get; set; }
    }
}
