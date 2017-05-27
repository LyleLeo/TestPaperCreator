using System;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Wordprocessing;
using DocumentFormat.OpenXml.Packaging;
using System.IO;
using System.Linq;
using DocumentFormat.OpenXml.Vml;
using System.Text;
using System.Runtime.Serialization.Formatters.Binary;
using System.Collections.Generic;


namespace TestPaperCreator.BLL.Utility
{
    public class OpenXmlForOffice
    {
        #region 创建空docx文件
        /// <summary>
        /// 创建空docx文件
        /// </summary>
        /// <param name="filepath">文件路径</param>
        /// <param name="filename">文件名称（不带后缀，默认docx）</param>
        private static void CreateDocx(string filepath, string filename)
        {
            //检查目录是否存在，没有则创建
            if (!Directory.Exists(filepath))
            {
                Directory.CreateDirectory(filepath);
            }
            try
            {
                using (WordprocessingDocument doc = WordprocessingDocument.Create(filepath + filename + ".docx", WordprocessingDocumentType.Document))
                {
                    MainDocumentPart mainPart = doc.AddMainDocumentPart();
                    mainPart.Document = new Document();
                    Body body = mainPart.Document.AppendChild(new Body());
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        #endregion

        #region 查询附件ID
        private static string GetEmbedObjectID(DocumentFormat.OpenXml.Vml.Office.OleObject t)
        {
            return t.Id;
        }
        #endregion

        #region 查询图片ID
        private static string GetImageID(ImageData i)
        {

            return i.RelationshipId;
        }
        #endregion

        #region 打开一个文档，并在指定位置添加段落（重载）
        /// <summary>
        /// 打开一个文档，并在指定位置添加段落（重载）
        /// </summary>
        /// <param name="filepath">目标文档路径</param>
        /// <param name="paragraph">待插入的段落</param>
        /// <param name="wordprocessingDocument">源文档对象</param>
        /// <param name="refParagraph">定位段落，待插入的段落会插入它后面</param>
        private static void OpenAndAddParagraphToWordDocument(string filepath, Paragraph paragraph, WordprocessingDocument wordprocessingDocument, Paragraph refParagraph)
        {
            //打开目标文件
            WordprocessingDocument distwordprocessingDocument = WordprocessingDocument.Open(filepath, true);
            //遍历附件内容
            foreach (DocumentFormat.OpenXml.Vml.Office.OleObject obj in paragraph.Descendants<DocumentFormat.OpenXml.Vml.Office.OleObject>().ToList())
            {
                string rid = GetEmbedObjectID(obj);
                IdPartPair ip = wordprocessingDocument.MainDocumentPart.Parts.Single(p => p.RelationshipId == rid);
                EmbeddedObjectPart eop = wordprocessingDocument.MainDocumentPart.EmbeddedObjectParts.Single(q => q.Uri == ip.OpenXmlPart.Uri);
                distwordprocessingDocument.MainDocumentPart.AddPart(ip.OpenXmlPart, rid);

            }
            //遍历图片内容
            foreach (ImageData imagedata in paragraph.Descendants<ImageData>())
            {
                string rid = GetImageID(imagedata);
                IdPartPair ip = wordprocessingDocument.MainDocumentPart.Parts.Single(p => p.RelationshipId == rid);
                distwordprocessingDocument.MainDocumentPart.AddPart(ip.OpenXmlPart, rid);

            }
            // Assign a reference to the existing document body.
            Body body = distwordprocessingDocument.MainDocumentPart.Document.Body;
            // Add new text.
            body.InsertAfter(paragraph.CloneNode(true), refParagraph);
            // Close the handle explicitly.
            distwordprocessingDocument.Close();
        }
        #endregion

        #region 打开文档并添加一个段落
        /// <summary>
        /// 打开文档并添加一个段落
        /// </summary>
        /// <param name="filepath">文档路径</param>
        /// <param name="paragraph">待插入的段落</param>
        /// <param name="wordprocessingDocument">源文件对象</param>
        private static void OpenAndAddParagraphToWordDocument(string filepath, Paragraph paragraph, WordprocessingDocument wordprocessingDocument)
        {
            //打开目标文件
            WordprocessingDocument distwordprocessingDocument = WordprocessingDocument.Open(filepath, true);
            //遍历附件内容
            foreach (DocumentFormat.OpenXml.Vml.Office.OleObject obj in paragraph.Descendants<DocumentFormat.OpenXml.Vml.Office.OleObject>().ToList())
            {
                string rid = GetEmbedObjectID(obj);
                IdPartPair ip = wordprocessingDocument.MainDocumentPart.Parts.Single(p => p.RelationshipId == rid);
                EmbeddedObjectPart eop = wordprocessingDocument.MainDocumentPart.EmbeddedObjectParts.Single(q => q.Uri == ip.OpenXmlPart.Uri);
                distwordprocessingDocument.MainDocumentPart.AddPart(ip.OpenXmlPart, rid);

            }
            //遍历图片内容
            foreach (ImageData imagedata in paragraph.Descendants<ImageData>())
            {
                string rid = GetImageID(imagedata);
                IdPartPair ip = wordprocessingDocument.MainDocumentPart.Parts.Single(p => p.RelationshipId == rid);
                distwordprocessingDocument.MainDocumentPart.AddPart(ip.OpenXmlPart, rid);

            }
            //摄氏度后面的字符跟的太近了，所以遇到摄氏度的时候就要增加两个空格调远一点
            foreach (Run r in paragraph.Descendants<Run>())
            {
                if (r.InnerText.Contains("℃"))
                {
                    Run run1 = new Run();

                    RunProperties runProperties1 = new RunProperties();
                    RunFonts runFonts1 = new RunFonts() { Hint = FontTypeHintValues.EastAsia };

                    runProperties1.Append(runFonts1);
                    Text text1 = new Text() { Space = SpaceProcessingModeValues.Preserve };
                    text1.Text = "  ";
                    run1.Append(runProperties1);
                    run1.Append(text1);
                    paragraph.InsertAfter(run1.CloneNode(true), r);
                }
            }
            // Assign a reference to the existing document body.
            Body body = distwordprocessingDocument.MainDocumentPart.Document.Body;
            // Add new text.
            body.AppendChild(paragraph.CloneNode(true));
            // Close the handle explicitly.
            distwordprocessingDocument.Close();
        }
        #endregion

        #region 深拷贝
        public static T DeepCopy<T>(T obj)
        {
            object retval;
            using (MemoryStream ms = new MemoryStream())
            {
                BinaryFormatter bf = new BinaryFormatter();
                //序列化成流
                bf.Serialize(ms, obj);
                ms.Seek(0, SeekOrigin.Begin);
                //反序列化成对象
                retval = bf.Deserialize(ms);
                ms.Close();
            }
            return (T)retval;
        }
        #endregion

        #region 分割上传的WORD文档
        /// <summary>
        /// 分割上传的WORD文档
        /// </summary>
        /// <param name="file">文件路径,不含文件名</param>
        /// <param name="question">问题实体</param>
        /// <param name="maxid">当前数据库中题目最大ID</param>
        public static void SplitDocx(string file, string name, MODEL.TestPaper.Question question)
        {
            //获取数据库中最大ID
            int maxid = DAL.TestPaperService.TestPaperService.GetMaxQuestionID();
            WordprocessingDocument wordprocessingDocument = WordprocessingDocument.Open(file + name, true);
            Body body = wordprocessingDocument.MainDocumentPart.Document.Body;
            int flag = 0;//标记当前段落是问题还是答案，偶数为问题，奇数为答案，从偶数开始。
            bool documentisend = true;//标记一个文档是否插入完成
            maxid += 1;//从最大ID+1开始新增题目
            StringBuilder sb = new StringBuilder();//内容容器
            foreach (Paragraph p in body.Descendants<Paragraph>().ToList())
            {
                if (flag % 2 == 0)
                {
                    if (documentisend)
                    {
                        CreateDocx(file, maxid.ToString());
                        documentisend = false;
                    }
                    if (p.InnerText == "@")
                    {
                        documentisend = true;
                        flag++;
                        OfficeHelper.WordDocumentMerger.ConvertDocxToHtml(file + maxid.ToString() + ".docx");
                        DAL.TestPaperService.TestPaperService.InsertQuestion(question.Course, question.Type, question.Section, question.Difficulty, sb.ToString().Trim());
                        sb.Length = 0;//清空stringbuilder
                        continue;
                    }
                    OpenAndAddParagraphToWordDocument(file + maxid.ToString() + ".docx", p, wordprocessingDocument);
                    sb.Append(p.InnerText);
                }
                else
                {
                    if (documentisend)
                    {
                        CreateDocx(file, maxid.ToString() + "_answer");
                        documentisend = false;
                    }
                    if (p.InnerText == "@")
                    {
                        documentisend = true;
                        flag++;
                        OfficeHelper.WordDocumentMerger.ConvertDocxToHtml(file + maxid.ToString() + "_answer.docx");
                        maxid++;
                        continue;
                    }
                    OpenAndAddParagraphToWordDocument(file + maxid.ToString() + "_answer.docx", p, wordprocessingDocument);
                }
            }
            wordprocessingDocument.Close();
        }
        #endregion

        #region 合并WORD文档
        public static void CreatePaper(string templet, string outfile, string strCopyFolder, int questiontype)
        {
            string[] arrFiles = Directory.GetFiles(strCopyFolder);
            foreach (string file in arrFiles)
            {
                //题目文件
                WordprocessingDocument wordprocessingDocument = WordprocessingDocument.Open(templet, true);
                IDictionary<string, BookmarkStart> bookmarkDic = new Dictionary<String, BookmarkStart>();
                foreach (BookmarkStart bookmarkstart in wordprocessingDocument.MainDocumentPart.RootElement.Descendants<BookmarkStart>())
                {
                    bookmarkDic[bookmarkstart.Name] = bookmarkstart;
                }
                wordprocessingDocument.Close();
                wordprocessingDocument = WordprocessingDocument.Open(file, true);
                string questiontypename;
                if (questiontype == 1)
                {
                    questiontypename = "ChoiceQ";
                }
                else
                {
                    questiontypename = "ChoiceQ";
                }
                if (bookmarkDic.Keys.Contains(questiontypename))
                {
                    BookmarkStart bookmarkstart = bookmarkDic[questiontypename];
                    Paragraph refParagraph = (Paragraph)bookmarkstart.Parent;
                    foreach (Paragraph p in wordprocessingDocument.MainDocumentPart.Document.Descendants<Paragraph>())
                    {
                        OpenAndAddParagraphToWordDocument(templet, p, wordprocessingDocument, refParagraph);
                    }
                }

            }
        }
        #endregion
    }
}