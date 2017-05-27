using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using TestPaperCreator.MODEL;
using System.IO;
using System.Web;

namespace TestPaperCreator.Controllers.TestPaperService
{
    public class DownloadTestPaperAPIController : ApiController
    {
        [HttpPost]
        public List<MODEL.TestPaper.Question> GetQuestionID([FromBody]List<MODEL.TestPaper.Paper> paperlist)
        {
            //List<MODEL.TestPaper.Paper> paperlist = new List<MODEL.TestPaper.Paper>();
            //foreach(MODEL.TestPaper.Paper i in paperlist)
            //{
            //    MODEL.TestPaper.Paper paper = new MODEL.TestPaper.Paper();
            //    MODEL.TestPaper.PaperType papertype = new MODEL.TestPaper.PaperType();
            //    paper.count = i.count;
            //    papertype.course = i.papertype.course;
            //    papertype.questiontype = i.papertype.questiontype;
            //    papertype.section = i.papertype.section;
            //    papertype.difficulty = i.papertype.difficulty;
            //    paper.papertype = papertype;
            //    paperlist.Add(paper);
            //}
            
            string localPath = Path.Combine(HttpRuntime.AppDomainAppPath, "Upload");
            return BLL.OfficeHelper.WordDocumentMerger.GetPaperQuestionList(paperlist, localPath);
        }
    }
}
