using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web;
using System.IO;

namespace TestPaperCreator.Controllers.TestPaperService
{
    public class UploadQuestionsAPIController : ApiController
    {
        [HttpPost]
        public IHttpActionResult UpLoadProcess(string id, string name, string type, string lastModifiedDate, int size, int course, int questiontype, int section, int difficulty, HttpPostedFileBase file)
        {
            string filePathName = string.Empty;

            string localPath = Path.Combine(HttpRuntime.AppDomainAppPath, "Upload");
            string path = "\\" + course.ToString() + "\\" + section.ToString() + "\\" + questiontype.ToString() + "\\" + difficulty.ToString() + "\\";
            localPath += path;
            if (HttpContext.Current.Request.Files.Count == 0)
            {
                return Json(new { jsonrpc = 2.0, error = new { code = 102, message = "保存失败" }, id = "id" });
            }
            string ex = Path.GetExtension(file.FileName);
            //filePathName = Guid.NewGuid().ToString("N") + ex;
            if (!Directory.Exists(localPath))
            {
                Directory.CreateDirectory(localPath);
            }
            file.SaveAs(Path.Combine(localPath, name));
            MODEL.TestPaper.Question question = new MODEL.TestPaper.Question();
            question.Type = questiontype;
            question.Section = section;
            question.Difficulty = difficulty;
            question.Course = course;
            if (name.ToLower().EndsWith(".doc"))
            {
                BLL.OfficeHelper.WordDocumentMerger.ConvertDocToDocx(localPath + name);
                name = name.Replace("doc", "docx");
            }
            BLL.Utility.OpenXmlForOffice.SplitDocx(localPath, name, question);
            //清除问题内容中的问号
            BLL.TestPaperService.TestPaperService.ClearQuestionMark("?");
            BLL.TestPaperService.TestPaperService.ClearQuestionMark(" ");
            return Json(new
            {
                jsonrpc = "2.0",
                id = id,
                filePath = "/Upload/" + name
            });
        }
    }
}
