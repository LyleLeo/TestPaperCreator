using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Web;
using System.Web.Http;

namespace TestPaperCreator.Controllers.TestPaperService
{
    public class DownloadTestPaperAPIController : ApiController
    {
        #region 获取最大试题数
        [HttpPost]
        [Route("api/DownloadTestPaperAPI/DeleteMaxCount/")]
        public Dictionary<int, string> DeleteMaxCount(dynamic obj)
        {
            int course = Convert.ToInt32(obj[0]);
            int questiontype = Convert.ToInt32(obj[1]);
            int section = Convert.ToInt32(obj[2]);
            int difficulty = Convert.ToInt32(obj[3]);
            return BLL.TestPaperService.TestPaperService.GetMaxCount(course, questiontype, section, difficulty);
            //List<int> list = new List<int>();
            //return list;
        }
        #endregion

        #region 获取符合条件的试题
        [HttpPost]
        [Route("api/DownloadTestPaperAPI/GetQuestionID/")]
        public List<MODEL.TestPaper.Question> GetQuestionID([FromBody]List<MODEL.TestPaper.Paper> paperlist)
        {
            string localPath = Path.Combine(HttpRuntime.AppDomainAppPath, "Upload");
            return BLL.OfficeHelper.WordDocumentMerger.GetPaperQuestionList(paperlist, localPath);
        }
        #endregion

        #region 确认添加权重
        [HttpPost]
        [Route("api/DownloadTestPaperAPI/Confirm/")]
        public string Confirm(dynamic obj)
        {
            List<int> questionlist = new List<int>();
            JArray jarray = obj.questionlist;
            foreach (var item in jarray)
            {
                int i = Convert.ToInt32(item.ToString());
                questionlist.Add(i);
            }
            int majorid = obj.majorname;
            string result = BLL.TestPaperService.TestPaperService.Confirm(majorid, questionlist);
            return "success";
        }
        #endregion

        #region 换题
        [HttpPost]
        public MODEL.TestPaper.Question GetOneQuestion(dynamic obj)
        {
            List<int> oldidlist = new List<int>();
            JArray jarray = obj.oldidlist;
            foreach (var item in jarray)
            {
                int i = Convert.ToInt32(item.ToString());
                oldidlist.Add(i);
            }

            int oldquestionid = Convert.ToInt32(obj.oldquestionid);
            MODEL.TestPaper.Paper paper = Newtonsoft.Json.JsonConvert.DeserializeObject<MODEL.TestPaper.Paper>(Convert.ToString(obj.paper));
            return BLL.TestPaperService.TestPaperService.GetOneQuestion(paper, oldidlist);
        }
        #endregion

        #region 生成试卷
        [HttpPost]
        [Route("api/DownloadTestPaperAPI/Generate/")]
        public List<string> Generate(dynamic obj)
        {
            string rootpath = HttpContext.Current.Request.MapPath("/");
            JArray questionlist = obj.questionlist;
            Dictionary<int, int> questions = new Dictionary<int, int>();
            foreach (var q in questionlist)
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
            JObject propertyobj = obj.property;
            MODEL.TestPaper.PaperProperty property = new MODEL.TestPaper.PaperProperty();
            property.schoolname = propertyobj["schoolname"].ToString();
            property.collegename = propertyobj["collegename"].ToString();
            property.majorname = propertyobj["majorname"].ToString();
            property.term = propertyobj["term"].ToString();
            property.testtype = propertyobj["testtype"].ToString();
            property.course = propertyobj["course"].ToString();
            property.volume = propertyobj["volume"].ToString();
            property.length = propertyobj["length"].ToString();
            property.testmethod = propertyobj["testmethod"].ToString();
            property.schoolyear = propertyobj["schoolyear"].ToString();
            property.grade = propertyobj["grade"].ToString();
            property.classnumber = propertyobj["classnumber"].ToString();
            property.total_count = propertyobj["tihao"].ToString();
            property.total_score = propertyobj["total_score"].ToString();
            BLL.TestPaperService.TestPaperService.CopyFiles(questions, rootpath, type);
            string paperheadA = rootpath + @"\Upload\OUT\A\TestPaperHead.dotx";
            string paperbodyA = rootpath + @"\Upload\OUT\A\TestPaperBody.dotx";
            string paperheadB = rootpath + @"\Upload\OUT\B\TestPaperHead.dotx";
            string paperbodyB = rootpath + @"\Upload\OUT\B\TestPaperBody.dotx";
            string strCopyFolderA = rootpath + @"\Upload\OUT\A\";
            string strCopyFolderB = rootpath + @"\Upload\OUT\B\";
            property.volume = "A";
            bool isanswer = false;
            BLL.Utility.OpenXmlForOffice.CreatePaper(paperheadA, paperbodyA, strCopyFolderA, type, property, isanswer);
            property.volume = "B";
            BLL.Utility.OpenXmlForOffice.CreatePaper(paperheadB, paperbodyB, strCopyFolderB, type, property, isanswer);
            property.volume = "A";
            isanswer = true;
            BLL.Utility.OpenXmlForOffice.CreateAnswer(paperheadA, paperbodyA, strCopyFolderA, type, property, isanswer);
            property.volume = "B";
            BLL.Utility.OpenXmlForOffice.CreateAnswer(paperheadB, paperbodyB, strCopyFolderB, type, property, isanswer);
            string[] volumeA = Directory.GetFiles(rootpath + @"\Upload\OUT\A\final\");
            string[] volumeB = Directory.GetFiles(rootpath + @"\Upload\OUT\B\final\");
            string resultnameA = volumeA[0];
            string resultnameAanswer = volumeA[1];
            string resultnameB = volumeB[0];
            string resultnameBanswer = volumeB[1];
            string finalfileA = Path.GetFileName(rootpath + @"\Upload\OUT\A\final\" + resultnameA);
            string finalfileAanswer = Path.GetFileName(rootpath + @"\Upload\OUT\A\final\" + resultnameAanswer);
            string finalfileB = Path.GetFileName(rootpath + @"\Upload\OUT\B\final\" + resultnameB);
            string finalfileBanswer = Path.GetFileName(rootpath + @"\Upload\OUT\B\final\" + resultnameBanswer);
            if (!Directory.Exists(rootpath + @"\Upload\Results\" + DateTime.Now.Year + DateTime.Now.Month + DateTime.Now.Day))
            {
                Directory.CreateDirectory(rootpath + @"\Upload\Results\" + DateTime.Now.Year + DateTime.Now.Month + DateTime.Now.Day);
            }
            string a = rootpath + @"\Upload\Results\" + DateTime.Now.Year + DateTime.Now.Month + DateTime.Now.Day + @"\" + finalfileA;
            string aa = rootpath + @"\Upload\Results\" + DateTime.Now.Year + DateTime.Now.Month + DateTime.Now.Day + @"\" + finalfileAanswer;
            string b = rootpath + @"\Upload\Results\" + DateTime.Now.Year + DateTime.Now.Month + DateTime.Now.Day + @"\" + finalfileB;
            string ba = rootpath + @"\Upload\Results\" + DateTime.Now.Year + DateTime.Now.Month + DateTime.Now.Day + @"\" + finalfileBanswer;
            File.Copy(resultnameA, a, true);
            File.Copy(resultnameAanswer, aa, true);
            File.Copy(resultnameB, b, true);
            File.Copy(resultnameBanswer, ba, true);
            List<string> filelist = new List<string>();
            filelist.Add(finalfileA);
            filelist.Add(finalfileAanswer);
            filelist.Add(finalfileB);
            filelist.Add(finalfileBanswer);
            return filelist;
        }
        #endregion

        #region 删除临时文件
        [HttpPost]
        [Route("api/DownloadTestPaperAPI/DeleteTemp/")]
        public string DeleteTemp()
        {
            string srcPath = HttpContext.Current.Request.MapPath("/") + @"\Upload\OUT\";
            if (Directory.Exists(srcPath))
            {
                try
                {
                    DirectoryInfo dir = new DirectoryInfo(srcPath);
                    FileSystemInfo[] fileinfo = dir.GetFileSystemInfos();  //返回目录中所有文件和子目录
                    foreach (FileSystemInfo i in fileinfo)
                    {
                        if (i is DirectoryInfo)            //判断是否文件夹
                        {
                            DirectoryInfo subdir = new DirectoryInfo(i.FullName);
                            subdir.Delete(true);          //删除子目录和文件
                        }
                        else
                        {
                            File.Delete(i.FullName);      //删除指定文件
                        }
                    }
                }
                catch (Exception e)
                {
                    return e.ToString();
                }
                Directory.Delete(srcPath);
            }

            return "success";
        }
        #endregion

        
    }

    class DaTi
    {
        string tihao { get; set; }
        MODEL.TestPaper.SingleDaTi singledati { get; set; }
    }
    public class ConditionList
    {
        int course { get; set; }
        int questiontype { get; set; }
        int section { get; set; }
        int difficulty { get; set; }
    }
}
