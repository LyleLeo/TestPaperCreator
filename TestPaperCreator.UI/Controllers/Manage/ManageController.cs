﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TestPaperCreator.MODEL.Membership;

namespace TestPaperCreator.Controllers.Manage
{
    public class ManageController : Controller
    {
        // GET: Manage
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
            List<MODEL.TestPaper.Condition> courselist = BLL.TestPaperService.TestPaperService.GetCourse(1);
            List<MODEL.TestPaper.Condition> difficultylist = BLL.TestPaperService.TestPaperService.GetDifficulty(1);
            List<MODEL.TestPaper.Condition> sectionlist = BLL.TestPaperService.TestPaperService.GetSection(1);
            List<MODEL.TestPaper.Condition> questiontype = BLL.TestPaperService.TestPaperService.GetQuestionType(1);
            List<MODEL.TestPaper.Condition> majorlist = BLL.TestPaperService.TestPaperService.GetMajor();
            ViewBag.CourseList = courselist;
            ViewBag.DifficultyList = difficultylist;
            ViewBag.SectionList = sectionlist;
            ViewBag.QuestionTypeList = questiontype;
            ViewBag.Majorlist = majorlist;
            return View();
        }
        public ActionResult EditCondition()
        {
            return View();
        }
    }
}