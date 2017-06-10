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
using DocumentFormat.OpenXml.Vml.Office;

using OpenXmlPowerTools;

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
                        try
                        {
                            OfficeHelper.WordDocumentMerger.ConvertDocxToHtml(file + maxid.ToString() + ".docx",file+maxid.ToString()+".html");
                        }
                        catch (Exception e)
                        {
                            throw e;
                        }
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
                        try
                        {
                            CreateDocx(file, maxid.ToString() + "_answer");
                        }
                        catch (Exception e)
                        {
                            throw e;
                        }
                        documentisend = false;
                    }
                    if (p.InnerText == "@")
                    {
                        documentisend = true;
                        flag++;
                        OfficeHelper.WordDocumentMerger.ConvertDocxToHtml(file + maxid.ToString() + "_answer.docx",file+maxid.ToString()+"_answer.html");
                        maxid++;
                        continue;
                    }
                    OpenAndAddParagraphToWordDocument(file + maxid.ToString() + "_answer.docx", p, wordprocessingDocument);
                }
            }
            wordprocessingDocument.Close();
        }
        #endregion

        #region 打开一个文档，添加一道小题
        private static void OpenAndAddParagraphToWordDocument(string filepath, List<Paragraph> paragraphlist, WordprocessingDocument wordprocessingDocument, string typeName)
        {

        }
        #endregion

        #region 将大题合并到试卷
        public static void MergeDatiToPaper(string paperhead, string strCopyFolder)
        {
            string name = Guid.NewGuid().ToString();
            #region 复制大题模板并转换为docx格式
            string sourceFile = paperhead;
            if (!Directory.Exists(System.IO.Path.GetDirectoryName(paperhead) + @"\final\"))
            {
                Directory.CreateDirectory(System.IO.Path.GetDirectoryName(paperhead) + @"\final\");
            }
            string paperbody_copy = System.IO.Path.GetDirectoryName(paperhead) + @"\final\" + name + ".docx";
            bool isrewrite = true; // true=覆盖已存在的同名文件,false则反之
            File.Copy(sourceFile, paperbody_copy, isrewrite);
            WordprocessingDocument wpd = WordprocessingDocument.Open(paperbody_copy, true);
            wpd.ChangeDocumentType(WordprocessingDocumentType.Document);
            wpd.Close();
            #endregion
            string[] datis = Directory.GetFiles(strCopyFolder, "*.docx");
            WordprocessingDocument paperheadobj = WordprocessingDocument.Open(paperbody_copy, true);
            foreach (string dati in datis)
            {
                WordprocessingDocument datiobj = WordprocessingDocument.Open(dati, true);
                foreach (OpenXmlElement oxe in datiobj.MainDocumentPart.Document.Body.ChildElements)
                {
                    foreach (OpenXmlElement b in oxe.Descendants<BookmarkStart>())
                    {
                        b.Remove();
                    }
                    foreach (OpenXmlElement b in oxe.Descendants<BookmarkEnd>())
                    {
                        b.Remove();
                    }
                    //Paragraph refparagraph = paperbodyobj.MainDocumentPart.RootElement.Descendants<Paragraph>().Last();
                    foreach (EmbeddedObject embedobj in oxe.Descendants<EmbeddedObject>())
                    {

                        foreach (OleObject oleobject in embedobj.Descendants<OleObject>())
                        {
                            string rid = oleobject.Id;
                            IdPartPair relationship = datiobj.MainDocumentPart.Parts.Single(i => i.RelationshipId == rid);
                            paperheadobj.MainDocumentPart.AddPart(relationship.OpenXmlPart, rid);
                        }
                        foreach (ImageData imgdata in embedobj.Descendants<ImageData>())
                        {
                            string rid = imgdata.RelationshipId;
                            IdPartPair imgrel = datiobj.MainDocumentPart.Parts.Single(i => i.RelationshipId == rid);
                            paperheadobj.MainDocumentPart.AddPart(imgrel.OpenXmlPart, rid);
                        }
                    }
                    paperheadobj.MainDocumentPart.Document.Body.AppendChild(oxe.CloneNode(true));
                }
                datiobj.Close();
            }
            paperheadobj.Close();
        }
        #endregion

        #region 插入大题
        public static void InsertDaTi(string paperhead, string paperbody, string copyfiles, Dictionary<int, MODEL.TestPaper.SingleDaTi> type)
        {
            WordprocessingDocument paperheadobj = WordprocessingDocument.Open(paperhead, true);
            List<IdPartPair> plist = paperheadobj.MainDocumentPart.Parts.ToList();
            int maxrid = 0;
            foreach (IdPartPair ipp in plist)
            {
                string rid = ipp.RelationshipId;
                rid = rid.Replace("rId", "*").Split('*')[1];
                if (Convert.ToInt32(rid) > maxrid)
                    maxrid = Convert.ToInt32(rid) + 1;
            }
            int maxshapeid = 1025;
            string shapeid = "";
            if (paperheadobj.MainDocumentPart.RootElement.Descendants<Shape>().Where(i => i.Id.ToString().Split('_').Count() > 1).Count() != 0)
            {
                shapeid = paperheadobj.MainDocumentPart.RootElement.Descendants<Shape>().Last().Id.ToString();
                shapeid = shapeid.Replace("_x0000_i", "*");
                maxshapeid = Convert.ToInt32(shapeid.Split('*')[1]) + 1;
            }
            int maxshapetype = 75;
            string shapetype = "";
            if (paperheadobj.MainDocumentPart.RootElement.Descendants<Shapetype>().Count() != 0)
            {
                shapetype = paperheadobj.MainDocumentPart.RootElement.Descendants<Shapetype>().Last().Id;
                shapetype = shapetype.Replace("_x0000_t", "*");
                maxshapetype = Convert.ToInt32(shapetype.Split('*')[1]) + 1;
            }
            paperheadobj.Close();
            MODEL.TestPaper.EmbedIDCounter eic = new MODEL.TestPaper.EmbedIDCounter();
            eic.MaxrId = maxrid;
            eic.MaxshapeId = maxshapeid;
            eic.MaxshapeType = maxshapetype;
            foreach (int tihao in type.Keys)
            {
                //复制大题模板
                #region 复制大题模板并转换为docx格式
                string sourceFile = paperbody;
                string paperbody_copy = System.IO.Path.GetDirectoryName(paperbody) + @"\" + tihao.ToString() + ".docx";
                bool isrewrite = true; // true=覆盖已存在的同名文件,false则反之
                File.Copy(sourceFile, paperbody_copy, isrewrite);
                WordprocessingDocument wpd = WordprocessingDocument.Open(paperbody_copy, true);
                wpd.ChangeDocumentType(WordprocessingDocumentType.Document);
                wpd.Close();
                #endregion
                int tixing = type[tihao].Type;
                int count = type[tihao].Count;
                int score = type[tihao].Score;
                int total_count = count * score;
                //获取题型中文名
                string tixingmingzi = "选择题";
                string[] xiaotifiles = Directory.GetFiles(copyfiles + @"\" + tihao.ToString());
                //WordprocessingDocument paperbodyobj = WordprocessingDocument.Open(paperbody_copy, true);

                eic = InsertXiaoTi(tixing, count, score, total_count, tixingmingzi, tihao, xiaotifiles, paperbody_copy, eic);
            }
        }
        #endregion

        #region 插入小题
        public static MODEL.TestPaper.EmbedIDCounter InsertXiaoTi(int tixing, int count, int score, int total_count, string tixingmingzi, int tihao, string[] xiaotifiles, string paperbody_copy, MODEL.TestPaper.EmbedIDCounter eic)
        {
            int maxrid = eic.MaxrId;
            int maxshapeid = eic.MaxshapeId;
            int maxshapetype = eic.MaxshapeType;

            #region 设置大题题干
            WordprocessingDocument paperbodyobjset = WordprocessingDocument.Open(paperbody_copy, true);
            BookmarkStart bms_tihao = paperbodyobjset.MainDocumentPart.RootElement.Descendants<BookmarkStart>().Single(i => i.Name == "QuestionNumber");
            bms_tihao.Parent.Descendants<Run>().First().Descendants<Text>().First().Text = ListItemTextGetter_zh_CN.GetListItemText("大写", tihao, "chineseCounting");
            BookmarkStart bms_tixing = paperbodyobjset.MainDocumentPart.RootElement.Descendants<BookmarkStart>().Single(i => i.Name == "TypeName");
            bms_tixing.NextSibling<Run>().Descendants<Text>().First().Text = tixingmingzi;
            BookmarkStart bms_count = GetBookmarkStartByName(paperbodyobjset, "Count");
            bms_count.NextSibling<Run>().Descendants<Text>().First().Text = count.ToString();
            BookmarkStart bms_score = GetBookmarkStartByName(paperbodyobjset, "Score");
            bms_score.NextSibling<Run>().Descendants<Text>().First().Text = score.ToString();
            BookmarkStart bms_total_score = GetBookmarkStartByName(paperbodyobjset, "TotalScore");
            string total_score = (score * count).ToString();
            bms_total_score.NextSibling<Run>().Descendants<Text>().First().Text = total_score;
            paperbodyobjset.Close();
            #endregion
            foreach (string file in xiaotifiles)
            {
                WordprocessingDocument paperbodyobj = WordprocessingDocument.Open(paperbody_copy, true);
                WordprocessingDocument xiaotiobj = WordprocessingDocument.Open(file, false);
                List<Paragraph> xiaotiparagraphlist = xiaotiobj.MainDocumentPart.RootElement.Descendants<Paragraph>().ToList();
                //加入题号
                
                string number = System.IO.Path.GetFileName(file).Replace("*",".docx");
                number = number.Split('*')[0];
                Run run1 = new Run();
                RunProperties runProperties1 = new RunProperties();
                RunFonts runFonts1 = new RunFonts() { Hint = FontTypeHintValues.EastAsia, Ascii = "宋体", HighAnsi = "宋体" };
                runProperties1.Append(runFonts1);
                Text text1 = new Text();
                text1.Text = number+". ";
                run1.Append(runProperties1);
                run1.Append(text1);
                xiaotiobj.MainDocumentPart.RootElement.Descendants<Paragraph>().First().InsertBefore(run1, xiaotiobj.MainDocumentPart.RootElement.Descendants<Paragraph>().First().ChildElements.First<Run>());
                //xiaotiobj.MainDocumentPart.RootElement.First<Paragraph>().InsertBefore(run1, xiaotiobj.MainDocumentPart.Document.Body.First<Paragraph>().First<Run>)
                foreach (Paragraph p in xiaotiparagraphlist)
                {
                    if (p.Descendants<Shapetype>().Count() != 0)
                    {
                        foreach (Shapetype st in p.Descendants<Shapetype>())
                        {
                            foreach (Shape sp in p.Descendants<Shape>().Where(i => i.Type == "#" + st.Id))
                            {
                                sp.Type = "#_x0000_t" + (maxshapetype + 1).ToString();
                            }
                            st.Id = "_x0000_t" + (maxshapetype + 1).ToString();
                            st.OptionalNumber = maxshapetype + 1;
                            maxshapetype++;
                        }
                    }
                    foreach (OpenXmlElement b in p.Descendants<BookmarkStart>())
                    {
                        b.Remove();
                    }
                    foreach (OpenXmlElement b in p.Descendants<BookmarkEnd>())
                    {
                        b.Remove();
                    }
                    Paragraph refparagraph = paperbodyobj.MainDocumentPart.RootElement.Descendants<Paragraph>().Last();
                    foreach (EmbeddedObject embedobj in p.Descendants<EmbeddedObject>())
                    {
                        foreach (OleObject oleobject in embedobj.Descendants<OleObject>())
                        {
                            string rid = oleobject.Id;
                            p.Descendants<OleObject>().First(i => i.Id == rid).Parent.Descendants<Shape>().First().Id = "_x0000_i" + (maxshapeid + 1).ToString();
                            p.Descendants<OleObject>().First(i => i.Id == rid).ShapeId = "_x0000_i" + (maxshapeid + 1).ToString();
                            p.Descendants<OleObject>().First(i => i.Id == rid).Id = "rId" + (maxrid + 1).ToString();
                            IdPartPair relationship = xiaotiobj.MainDocumentPart.Parts.Single(i => i.RelationshipId == rid);
                            paperbodyobj.MainDocumentPart.AddPart(relationship.OpenXmlPart, "rId" + (maxrid + 1).ToString());
                            maxshapeid++;
                            maxrid++;
                        }
                        foreach (ImageData imgdata in embedobj.Descendants<ImageData>())
                        {
                            string rid = imgdata.RelationshipId;
                            IdPartPair imgrel = xiaotiobj.MainDocumentPart.Parts.Single(i => i.RelationshipId == rid);
                            string newrid = "rId" + (maxrid + 1).ToString();
                            p.Descendants<ImageData>().First(i => i.RelationshipId == rid).RelationshipId = "rId" + (maxrid + 1).ToString();
                            paperbodyobj.MainDocumentPart.AddPart(imgrel.OpenXmlPart, "rId" + (maxrid + 1).ToString());
                            maxrid++;
                        }
                    }
                    paperbodyobj.MainDocumentPart.RootElement.GetFirstChild<Body>().AppendChild(p.CloneNode(true));
                    //paperbodyobj.MainDocumentPart.Document.Body.InsertAfter<Paragraph>((Paragraph)p.CloneNode(true), refparagraph);
                }
                //paperbodyobj.MainDocumentPart.DocumentSettingsPart.FeedData(xiaotiobj.MainDocumentPart.DocumentSettingsPart.GetStream());
                xiaotiobj.Close();
                paperbodyobj.Close();
            }
            eic.MaxrId = maxrid;
            eic.MaxshapeId = maxshapeid;
            eic.MaxshapeType = maxshapetype;
            return eic;
        }
        #endregion

        #region 同过书签名称获取对象
        public static BookmarkStart GetBookmarkStartByName(WordprocessingDocument obj, string name)
        {
            return obj.MainDocumentPart.RootElement.Descendants<BookmarkStart>().Single(i => i.Name == name);
        }
        #endregion

        #region 合并WORD文档
        /// <summary>
        /// 组成试卷的流程：
        /// 1、首先把一个题型的各个小题组合成一个大题(例：15个选择题组合成一个选择大题)
        /// 2、其次把大题依次插入到试卷头下面
        /// 3、为了不影响大题秩序，题型临时文件夹名字按题号命名
        /// </summary>
        /// <param name="head">试卷头</param>
        /// <param name="body">试卷大题</param>
        /// <param name="outfile">输出文档</param>
        /// <param name="strCopyFolder">待插入题的文件夹</param>
        /// <param name="type">大题字典，key为题号，value为题型</param>
        public static void CreatePaper(string paperhead, string paperbody, string strCopyFolder, Dictionary<int, MODEL.TestPaper.SingleDaTi> type)
        {
            //将小题组合成大题，按题号命名，放在OUT文件夹内
            InsertDaTi(paperhead, paperbody, strCopyFolder, type);
            //将大题合并成试卷
            MergeDatiToPaper(paperhead, strCopyFolder);
        }
        #endregion

    }
    
}