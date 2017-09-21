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
            foreach (var item in jarray)
            {
                int i = Convert.ToInt32(item.ToString());
                oldidlist.Add(i);
            }

            int oldquestionid = Convert.ToInt32(obj.oldquestionid);
            MODEL.TestPaper.Paper paper = Newtonsoft.Json.JsonConvert.DeserializeObject<MODEL.TestPaper.Paper>(Convert.ToString(obj.paper));
            return BLL.TestPaperService.TestPaperService.GetOneQuestion(paper, oldidlist);
        }
        [HttpPost]
        [Route("api/DownloadTestPaperAPI/Generate/")]
        public string Generate(dynamic obj)
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
            string paperhead = rootpath + @"\Upload\OUT\TestPaperHead.dotx";
            string paperbody = rootpath + @"\Upload\OUT\TestPaperBody.dotx";
            string strCopyFolder = rootpath + @"\Upload\OUT\";
            BLL.Utility.OpenXmlForOffice.CreatePaper(paperhead, paperbody, strCopyFolder, type, property);
            string resultname = Path.GetFileName(Directory.GetFiles(rootpath + @"\Upload\OUT\final\")[0]);
            string finalfile = rootpath + @"\Upload\OUT\final\" + resultname;
            if(!Directory.Exists(rootpath + @"\Upload\Results\" + DateTime.Now.Year + DateTime.Now.Month + DateTime.Now.Day))
            {
                Directory.CreateDirectory(rootpath + @"\Upload\Results\" + DateTime.Now.Year + DateTime.Now.Month + DateTime.Now.Day);
            }
            string a = rootpath + @"\Upload\Results\" + DateTime.Now.Year + DateTime.Now.Month + DateTime.Now.Day + @"\" + resultname;
            File.Copy(finalfile, a, true);
            return resultname;
        }
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
    }

    class DaTi
    {
        string tihao { get; set; }
        MODEL.TestPaper.SingleDaTi singledati { get; set; }
    }

}
