using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using TestPaperCreator.MODEL.Membership;

namespace TestPaperCreator.Controllers.TestPaperService
{
    public class UploadQuestionsController : Controller
    {
        public ActionResult Index()
        {
            if (Session["user"] != null)
            {
                string sessions = Session["user"].ToString();
                User user = new User();
                user = BLL.Utility.Utility.DeSerialize<User>(user, sessions);
                bool result = BLL.Membership.Membership.Login(user);
                if (result == false)
                {
                    ViewBag.IsLogin = true;
                    Response.Redirect("/Membership/Login");
                }
                else
                {
                    ViewBag.IsLogin = true;
                    ViewBag.Username = user.username;
                }
            }
            else
            {
                if (Request.Cookies["UserInfo"] != null)
                {
                    User user = new User();
                    user = BLL.Utility.Utility.DeSerialize<User>(user, Request.Cookies["UserInfo"].Value.ToString());
                    string username = user.username;
                    string password = user.password;
                    ViewBag.UserName = username;
                    bool result = BLL.Membership.Membership.Login(user);
                    if (result == false)
                    {
                        ViewBag.IsLogin = false;
                        Response.Redirect("/Membership/login");
                    }
                    else
                    {
                        ViewBag.IsLogin = true;
                        ViewBag.Username = username;
                    }
                }
                else
                {
                    Response.Redirect("/Membership/login");
                }
            }
            List<MODEL.TestPaper.Condition> courselist = BLL.TestPaperService.TestPaperService.GetCourse();
            List<MODEL.TestPaper.Condition> difficultylist = BLL.TestPaperService.TestPaperService.GetDifficulty();
            List<MODEL.TestPaper.Condition> sectionlist = BLL.TestPaperService.TestPaperService.GetSection();
            List<MODEL.TestPaper.Condition> questiontype = BLL.TestPaperService.TestPaperService.GetQuestionType();
            ViewBag.CourseList = courselist;
            ViewBag.DifficultyList = difficultylist;
            ViewBag.SectionList = sectionlist;
            ViewBag.QuestionTypeList = questiontype;
            //MODEL.TestPaper.PaperType papertype = new MODEL.TestPaper.PaperType();
            //papertype.course = 1;
            //papertype.difficulty = 1;
            //papertype.questiontype = 1;
            //papertype.section = 1;
            //MODEL.TestPaper.Paper paper = new MODEL.TestPaper.Paper();
            //paper.count = 5;
            //paper.papertype = papertype;
            //List<MODEL.TestPaper.Paper> paperlist = new List<MODEL.TestPaper.Paper>();
            //paperlist.Add(paper);
            //BLL.OfficeHelper.WordDocumentMerger.getaquestion(paperlist);
            //DateTime beforDT = System.DateTime.Now;
            //string tempDoc = @"/Upload/test/temp.docx";
            //string strCopyFolder = @"/Upload/test/new";
            //string outDoc = @"/Upload/test/out.docx";
            //BLL.OfficeHelper.WordDocumentMerger wordmeger = new BLL.OfficeHelper.WordDocumentMerger();
            //wordmeger.InsertMerge(tempDoc, strCopyFolder, outDoc);
            //DateTime afterDT = System.DateTime.Now;
            //TimeSpan ts = afterDT.Subtract(beforDT);
            //ViewBag.totaltime = ts.TotalSeconds;
            //BLL.OfficeHelper.WordDocumentMerger wordmerger = new BLL.OfficeHelper.WordDocumentMerger();
            //string[] temp = wordmerger.SplitWord(@"D:\TeamFoundationServer\TestPaperCreator\TestPaperCreator/Upload/test/new/01.docx");
            //ViewBag.temp = temp;
            //ViewBag.length = temp.Length;
            return View();
        }
        public ActionResult UpLoadProcess(string id, string name, string type, string lastModifiedDate, int size, int course, int questiontype, int section, int difficulty, HttpPostedFileBase file)
        {
            string filePathName = string.Empty;

            string localPath = Path.Combine(HttpRuntime.AppDomainAppPath, "Upload");
            string path = "\\" + course.ToString() + "\\" + section.ToString() + "\\" + questiontype.ToString() + "\\" + difficulty.ToString() + "\\";
            localPath += path;
            if (Request.Files.Count == 0)
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