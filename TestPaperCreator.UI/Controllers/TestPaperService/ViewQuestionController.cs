using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using TestPaperCreator.MODEL.Membership;

namespace TestPaperCreator.Controllers.TestPaperService
{
    public class ViewQuestionController : Controller
    {
        // GET: ViewQuestion
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
            List<MODEL.TestPaper.Question> questionlist = BLL.TestPaperService.TestPaperService.GetQuestionList(0, 0, 0, 0, null);
            ViewBag.questionlist = questionlist;
            return View();
        }
        public ActionResult DeleteQuestion()
        {
            return View();
        }
        public ActionResult UpLoadProcess(string id, string name, string type, string lastModifiedDate, int? size,int questionid, int course, int questiontype, int section, int difficulty, HttpPostedFileBase file)
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
            question.ID = questionid;
            if (name.ToLower().EndsWith(".doc"))
            {
                BLL.OfficeHelper.WordDocumentMerger.ConvertDocToDocx(localPath + name);
                name = name.Replace("doc", "docx");
            }
            BLL.Utility.OpenXmlForOffice.SplitDocx(localPath, name, question);
            //清除问题内容中的问号
            //BLL.TestPaperService.TestPaperService.ClearQuestionMark("?");
            //BLL.TestPaperService.TestPaperService.ClearQuestionMark(" ");
            return Json(new
            {
                jsonrpc = "2.0",
                id = id,
                filePath = "/Upload/" + name
            });

        }
    }
}