using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Office.Interop.Word;
using System.Reflection;
using System.IO;
using TestPaperCreator.BLL.Utility;
using System.Linq;

namespace TestPaperCreator.BLL.OfficeHelper
{
    ///
    /// Word文档合并类
    ///
    public class WordDocumentMerger
    {
        private ApplicationClass objApp = null;
        private Document objDocLast = null;
        private Document objDocBeforeLast = null;
        public WordDocumentMerger()
        {
            objApp = new ApplicationClass();
        }
        #region 打开文件
        private void Open(string tempDoc)
        {
            object objTempDoc = tempDoc;
            object objMissing = Missing.Value;

            objDocLast = objApp.Documents.Open(
            ref objTempDoc, //FileName
            ref objMissing, //ConfirmVersions
            ref objMissing, //ReadOnly
            ref objMissing, //AddToRecentFiles
            ref objMissing, //PasswordDocument
            ref objMissing, //Password Template
            ref objMissing, //Revert
            ref objMissing, //WritePasswordDocument
            ref objMissing, //WritePasswordTemplate
            ref objMissing, //Format
            ref objMissing, //Enconding
            ref objMissing, //Visible
            ref objMissing, //OpenAndRepair
            ref objMissing, //DocumentDirection
            ref objMissing, //NoEncodingDialog
            ref objMissing //XMLTransform
            );

            objDocLast.Activate();
        }
        #endregion

        #region 保存文件到输出模板
        private void SaveAs(string outDoc)
        {
            object objMissing = System.Reflection.Missing.Value;
            object objOutDoc = outDoc;
            objDocLast.SaveAs(
            ref objOutDoc, //FileName
            ref objMissing, //FileFormat
            ref objMissing, //LockComments
            ref objMissing, //PassWord
            ref objMissing, //AddToRecentFiles
            ref objMissing, //WritePassword
            ref objMissing, //ReadOnlyRecommended
            ref objMissing, //EmbedTrueTypeFonts
            ref objMissing, //SaveNativePictureFormat
            ref objMissing, //SaveFormsData
            ref objMissing, //SaveAsAOCELetter,
            ref objMissing, //Encoding
            ref objMissing, //InsertLineBreaks
            ref objMissing, //AllowSubstitutions
            ref objMissing, //LineEnding
            ref objMissing //AddBiDiMarks
            );
        }
        #endregion

        #region 循环合并多个文件（复制合并重复的文件）
        ///
        /// 循环合并多个文件（复制合并重复的文件）
        ///
        /// 模板文件
        /// 需要合并的文件
        /// 合并后的输出文件
        public void CopyMerge(string tempDoc, string[] arrCopies, string outDoc)
        {
            object objMissing = Missing.Value;
            object objFalse = false;
            object objTarget = WdMergeTarget.wdMergeTargetSelected;
            object objUseFormatFrom = WdUseFormattingFrom.wdFormattingFromSelected;
            try
            {
                //打开模板文件
                Open(tempDoc);
                foreach (string strCopy in arrCopies)
                {
                    objDocLast.Merge(
                    strCopy, //FileName
                    ref objTarget, //MergeTarget
                    ref objMissing, //DetectFormatChanges
                    ref objUseFormatFrom, //UseFormattingFrom
                    ref objMissing //AddToRecentFiles
                    );
                    objDocBeforeLast = objDocLast;
                    objDocLast = objApp.ActiveDocument;
                    if (objDocBeforeLast != null)
                    {
                        objDocBeforeLast.Close(
                        ref objFalse, //SaveChanges
                        ref objMissing, //OriginalFormat
                        ref objMissing //RouteDocument
                        );
                    }
                }
                //保存到输出文件
                SaveAs(outDoc);
                foreach (Document objDocument in objApp.Documents)
                {
                    objDocument.Close(
                    ref objFalse, //SaveChanges
                    ref objMissing, //OriginalFormat
                    ref objMissing //RouteDocument
                    );
                }
            }
            finally
            {
                objApp.Quit(
                ref objMissing, //SaveChanges
                ref objMissing, //OriginalFormat
                ref objMissing //RoutDocument
                );
                objApp = null;
            }
        }
        ///
        /// 循环合并多个文件（复制合并重复的文件）
        ///
        /// 模板文件
        /// 需要合并的文件
        /// 合并后的输出文件
        public void CopyMerge(string tempDoc, string strCopyFolder, string outDoc)
        {
            string[] arrFiles = Directory.GetFiles(strCopyFolder);
            CopyMerge(tempDoc, arrFiles, outDoc);
        }
        #endregion

        #region 循环合并多个文件（插入合并文件）
        ///
        /// 循环合并多个文件（插入合并文件）
        ///
        /// 模板文件
        /// 需要合并的文件
        /// 合并后的输出文件
        public void InsertMerge(string tempDoc, string[] arrCopies, string outDoc)
        {
            object objMissing = Missing.Value;
            object objFalse = false;
            object confirmConversion = false;
            object link = false;
            object attachment = false;
            try
            {
                //打开模板文件
                Open(tempDoc);
                foreach (string strCopy in arrCopies)
                {
                    objApp.Selection.InsertFile(
                    strCopy,
                    ref objMissing,
                    ref confirmConversion,
                    ref link,
                    ref attachment
                    );
                }
                //保存到输出文件
                SaveAs(outDoc);
                foreach (Document objDocument in objApp.Documents)
                {
                    objDocument.Close(
                    ref objFalse, //SaveChanges
                    ref objMissing, //OriginalFormat
                    ref objMissing //RouteDocument
                    );
                }
            }
            finally
            {
                objApp.Quit(
                ref objMissing, //SaveChanges
                ref objMissing, //OriginalFormat
                ref objMissing //RoutDocument
                );
                objApp = null;
            }
        }
        ///
        /// 循环合并多个文件（插入合并文件）
        ///
        /// 模板文件
        /// 需要合并的文件
        /// 合并后的输出文件
        public void InsertMerge(string tempDoc, string strCopyFolder, string outDoc)
        {
            string[] arrFiles = Directory.GetFiles(strCopyFolder);
            InsertMerge(tempDoc, arrFiles, outDoc);
        }
        #endregion

        #region 拆分文档
        public string[] SplitWord(string filepath, string filename, MODEL.TestPaper.Question question)
        {
            string temp;
            try
            {
                Application app = new Application();
                Document doc = null;
                object unknow = Type.Missing;
                app.Visible = false;
                object file = Path.Combine(filepath, filename);
                doc = app.Documents.Open(ref file,
                    ref unknow, ref unknow, ref unknow, ref unknow,
                    ref unknow, ref unknow, ref unknow, ref unknow,
                    ref unknow, ref unknow, ref unknow, ref unknow,
                    ref unknow, ref unknow, ref unknow);
                temp = doc.Content.Text;
                temp = temp.Replace("\r@\r", "@");
                string[] arrays = temp.Split('@');
                //temp.Replace(@"\r@\r", "*");
                //string[] arrays = temp.Split('*');
                int id = DAL.TestPaperService.TestPaperService.GetMaxQuestionID();
                if (id == -1)
                    id = 1;
                else
                    id += 1;
                for (int i = 0; i < arrays.Length - 1; i++)
                {
                    if (i % 2 == 0)
                    {
                        WordHelper wh = new WordHelper();
                        wh.CreateAndActive();
                        wh.InsertText(arrays[i]);
                        wh.SaveAs(filepath + id.ToString() + ".docx");
                        wh.Close();
                        DAL.TestPaperService.TestPaperService.InsertQuestion(question.Course, question.Type, question.Section, question.Difficulty, arrays[i].Trim());
                    }
                    else
                    {
                        WordHelper wh = new WordHelper();
                        wh.CreateAndActive();
                        wh.InsertText(arrays[i]);
                        wh.SaveAs(filepath + id.ToString() + "_answer.docx");
                        wh.Close();
                        id++;
                    }
                    //for(int j=1;j<=2;j++)
                    //{
                    //    WordHelper wh = new WordHelper();
                    //    wh.CreateAndActive();
                    //    wh.InsertText(arrays[i]);
                    //    wh.SaveAs(filepath + j.ToString());
                    //    wh.Close();
                    //}
                }
                //foreach(string i in arrays)
                //{
                //    for(int j=1;j<=2;j++)
                //    {
                //        WordHelper wh = new WordHelper();
                //        wh.CreateAndActive();
                //        wh.InsertText(i);
                //        wh.SaveAs(filepath + j.ToString());
                //        wh.Close();
                //    }
                //    //CreateWordFile(filepath, i);
                //}
                doc.Close();
                app.Quit();
                return arrays;

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return new string[] { "error" };
            }

        }
        #endregion

        #region 生成word文档
        public string CreateWordFile(string filepath, string content)
        {
            string message = "";
            try
            {
                Application app = new Application();              //声明一个wordAPP对象  
                Document doc = null;
                object Nothing = Missing.Value;                       //COM调用时用于占位  
                object format = WdSaveFormat.wdFormatDocument; //Word文档的保存格式  
                object path = filepath;
                doc = app.Documents.Add(ref Nothing, ref Nothing,
                    ref Nothing, ref Nothing);
                //向文档中写入内容  
                doc.Paragraphs.Last.Range.Text = content;
                //保存文档  
                doc.SaveAs(ref path, ref format, ref Nothing, ref Nothing,
                    ref Nothing, ref Nothing, ref Nothing, ref Nothing, ref Nothing,
                    ref Nothing, ref Nothing, ref Nothing, ref Nothing);
                //关闭文档  
                doc.Close(ref Nothing, ref Nothing, ref Nothing);  //关闭worddoc文档对象  
                app.Quit(ref Nothing, ref Nothing, ref Nothing);   //关闭wordApp组对象
            }
            catch
            {
                message = "文件导出异常！";
            }
            return message;
        }

        #endregion

        public static List<MODEL.TestPaper.Question> GetPaperQuestionList(List<MODEL.TestPaper.Paper> paperlist, string localpath)
        {
            if (Directory.Exists(localpath + "\\Output\\Questions\\"))
            {
                DelectDir(localpath + "\\Output\\Questions\\");
            }
            //判断文件是不是存在
            if (File.Exists(localpath + "\\Output\\out.docx"))
            {
                //如果存在则删除
                File.Delete(localpath + "\\Output\\out.docx");
            }
            //获取试题列表字典
            IDictionary<int,List<int>> typequestionidDic = DAL.TestPaperService.TestPaperService.GetQuestionID(paperlist);
            //把问题复制到Output文件夹
            List<MODEL.TestPaper.Question> questionlist = new List<MODEL.TestPaper.Question>();
            foreach (int questiontypeid in typequestionidDic.Keys)
            {
                foreach(int id in typequestionidDic[questiontypeid])
                {
                    MODEL.TestPaper.Question question = DAL.TestPaperService.TestPaperService.GetAQuestionByID(id);
                    questionlist.Add(question);
                    //string sourceurl = localpath + "\\" + question.Course.ToString() + "\\" + question.Section.ToString() + "\\" + question.Type.ToString() + "\\" + question.Difficulty.ToString() + "\\";
                    //string destinationurl = localpath + "\\Output\\Questions\\" + question.Type.ToString() + "\\";
                    //string sourceFile = sourceurl + question.ID.ToString() + ".docx";
                    //string destinationFile = localpath + "\\Output\\Questions\\" + question.Type.ToString() + "\\" + question.ID.ToString() + ".docx";
                    //if (!Directory.Exists(destinationurl))
                    //{
                    //    Directory.CreateDirectory(destinationurl);
                    //}
                    //bool isrewrite = true; // true=覆盖已存在的同名文件,false则反之
                    //File.Copy(sourceFile, destinationFile, isrewrite);
                }   
            }
            //string templatefile = localpath + "\\Output\\templet.dotx";
            //string outfile = localpath + "\\Output\\out.dotx";
            //File.Copy(templatefile, outfile, true);//复制模板文件
            //string tempDoc = localpath + "\\Output\\templet.dotx";
            //string strCopyFolder = localpath + "\\Output\\Questions\\";
            //string outDoc = localpath + "\\Output\\out.docx";
            //foreach(int typeid in typequestionidDic.Keys)
            //{
            //    OpenXmlForOffice.CreatePaper(tempDoc, outDoc, strCopyFolder + typeid.ToString() + "\\", typeid);
            //}
            
            //WordDocumentMerger wordmeger = new WordDocumentMerger();
            //wordmeger.InsertMerge(tempDoc, strCopyFolder, outDoc);
            return questionlist;
        }
        public static void DelectDir(string srcPath)
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
                throw e;
            }
        }
        /// <summary>
        /// 将.doc文件转换为.docx文件（word2013），请确认传入doc文件
        /// </summary>
        /// <param name="path">文件路径（包含文件）</param>
        public static void ConvertDocToDocx(string path)
        {
            Application word = new Application();
            var sourceFile = new FileInfo(path);
            var document = word.Documents.Open(sourceFile.FullName);
            string newFileName = sourceFile.FullName.Replace(".doc", ".docx");
            document.SaveAs2(newFileName, WdSaveFormat.wdFormatXMLDocument,
                             CompatibilityMode: WdCompatibilityMode.wdWord2013);
            word.ActiveDocument.Close();
            word.Quit();
            File.Delete(path);
        }
        public static void ConvertDocxToHtml(string path)
        {
            Application word = new Application();
            var sourceFile = new FileInfo(path);
            var document = word.Documents.Open(sourceFile.FullName);
            string newFileName = sourceFile.FullName.Replace(".docx", ".html");
            string abspath = Path.GetDirectoryName(newFileName);
            if(!Directory.Exists(abspath + "\\files\\"))
            {
                Directory.CreateDirectory(abspath + "\\files\\");
            }
            newFileName = abspath + "\\files\\" + Path.GetFileName(newFileName);
            document.SaveAs2(newFileName, WdSaveFormat.wdFormatHTML);
            word.ActiveDocument.Close();
            word.Quit();
            //File.Delete(path);
        }
    }
}