using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Word = Microsoft.Office.Interop.Word;

namespace TestPaperCreator.BLL.Utility
{
    public class WordHelper
    {
        #region 私有成员

        private Word.ApplicationClass _wordApplication; //word程序变量
        private Word.Document _wordDocument;           //word文档变量
        object missing = System.Reflection.Missing.Value;//调用COM库，实例化对象时的缺省参数

        #endregion

        #region  公开属性

        /// <summary>
        /// ApplciationClass
        /// </summary>
        public Word.ApplicationClass WordApplication
        {
            get
            {
                return _wordApplication;
            }
        }

        /// <summary>
        /// Document
        /// </summary>
        public Word.Document WordDocument
        {
            get
            {
                return _wordDocument;
            }
        }

        #endregion

        #region  构造函数

        public WordHelper()
        {
            _wordApplication = new Word.ApplicationClass();
        }

        public WordHelper(Word.ApplicationClass wordApplication)
        {
            _wordApplication = wordApplication;
        }

        #endregion

        #region 基本操作（新建、打开、保存、关闭）

        /// <summary>
        /// 新建并打开一个文档（默认缺省值）
        /// </summary>
        public void CreateAndActive()
        {
            _wordDocument = CreateOneDocument(missing, missing, missing, missing);
            _wordDocument.Activate();
        }

        /// <summary>
        /// 打开指定文件
        /// </summary>
        /// <param name="FileName">文件名（包含路径）</param>
        /// <param name="IsReadOnly">打开后是否只读</param>
        /// <param name="IsVisibleWin">打开后是否可视</param>
        /// <returns>打开是否成功</returns>
        public bool OpenAndActive(string FileName, bool IsReadOnly, bool IsVisibleWin)
        {
            if (string.IsNullOrEmpty(FileName))
            {
                return false;
            }
            try
            {
                _wordDocument = OpenOneDocument(FileName, missing, IsReadOnly, missing, missing, missing, missing, missing, missing, missing, missing, IsVisibleWin, missing, missing, missing, missing);
                _wordDocument.Activate();
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 关闭
        /// Closes the specified document or documents.
        /// </summary>
        public void Close()
        {
            if (_wordDocument != null)
            {
                _wordDocument.Close(ref missing, ref missing, ref missing);
                _wordApplication.Application.Quit(ref missing, ref missing, ref missing);
            }
        }

        /// <summary>
        /// 保存
        /// </summary>
        public void Save()
        {
            if (_wordDocument == null)
            {
                _wordDocument = _wordApplication.ActiveDocument;
            }
            _wordDocument.Save();
        }

        /// <summary>
        /// 保存为...
        /// </summary>
        /// <param name="FileName">文件名（包含路径）</param>
        public void SaveAs(string FileName)
        {
            if (_wordDocument == null)
            {
                _wordDocument = _wordApplication.ActiveDocument;
            }
            object objFileName = FileName;

            _wordDocument.SaveAs(ref objFileName, ref missing, ref missing, ref missing, ref missing, ref missing, ref missing,
                ref missing, ref missing, ref missing, ref missing, ref missing, ref missing, ref missing, ref missing, ref missing);
        }

        /// <summary>
        /// 新建一个Document
        /// </summary>
        /// <param name="template">Optional Object. The name of the template to be used for the new document. If this argument is omitted, the Normal template is used.</param>
        /// <param name="newTemplate">Optional Object. True to open the document as a template. The default value is False.</param>
        /// <param name="documentType">Optional Object. Can be one of the following WdNewDocumentType constants: wdNewBlankDocument, wdNewEmailMessage, wdNewFrameset, or wdNewWebPage. The default constant is wdNewBlankDocument.</param>
        /// <param name="visible">Optional Object. True to open the document in a visible window. If this value is False, Microsoft Word opens the document but sets the Visible property of the document window to False. The default value is True.</param> 
        public Word.Document CreateOneDocument(object template, object newTemplate, object documentType, object visible)
        {
            return _wordApplication.Documents.Add(ref template, ref newTemplate, ref documentType, ref visible);
        }

        /// <summary>
        /// 打开一个已有文档
        /// </summary>
        /// <param name="FileName"></param>
        /// <param name="ConfirmConversions"></param>
        /// <param name="ReadOnly"></param>
        /// <param name="AddToRecentFiles"></param>
        /// <param name="PasswordDocument"></param>
        /// <param name="PasswordTemplate"></param>
        /// <param name="Revert"></param>
        /// <param name="WritePasswordDocument"></param>
        /// <param name="WritePasswordTemplate"></param>
        /// <param name="Format"></param>
        /// <param name="Encoding"></param>
        /// <param name="Visible"></param>
        /// <param name="OpenAndRepair"></param>
        /// <param name="DocumentDirection"></param>
        /// <param name="NoEncodingDialog"></param>
        /// <param name="XMLTransform"></param>
        /// <returns></returns>
        public Word.Document OpenOneDocument(object FileName, object ConfirmConversions, object ReadOnly,
            object AddToRecentFiles, object PasswordDocument, object PasswordTemplate, object Revert,
            object WritePasswordDocument, object WritePasswordTemplate, object Format, object Encoding,
            object Visible, object OpenAndRepair, object DocumentDirection, object NoEncodingDialog, object XMLTransform)
        {
            try
            {
                return _wordApplication.Documents.Open(ref FileName, ref ConfirmConversions, ref ReadOnly, ref AddToRecentFiles,
                    ref PasswordDocument, ref PasswordTemplate, ref Revert, ref WritePasswordDocument, ref WritePasswordTemplate,
                    ref Format, ref Encoding, ref Visible, ref OpenAndRepair, ref DocumentDirection, ref NoEncodingDialog, ref XMLTransform);
            }
            catch
            {
                return null;
            }
        }

        #endregion

        #region 移动光标位置

        /// <summary>
        /// 光标移动到指定书签位置，书签不存在时不移动
        /// </summary>
        /// <param name="bookMarkName"></param>
        /// <returns></returns>
        public bool GoToBookMark(string bookMarkName)
        {
            //是否存在书签
            if (_wordDocument.Bookmarks.Exists(bookMarkName))
            {
                object what = Word.WdGoToItem.wdGoToBookmark;
                object name = bookMarkName;
                GoTo(what, missing, missing, name);
                return true;
            }
            return false;
        }

        /// <summary>
        /// 移动光标
        /// Moves the insertion point to the character position immediately preceding the specified item.
        /// </summary>
        /// <param name="what">Optional Object. The kind of item to which the selection is moved. Can be one of the WdGoToItem constants.</param>
        /// <param name="which">Optional Object. The item to which the selection is moved. Can be one of the WdGoToDirection constants. </param>
        /// <param name="count">Optional Object. The number of the item in the document. The default value is 1.</param>
        /// <param name="name">Optional Object. If the What argument is wdGoToBookmark, wdGoToComment, wdGoToField, or wdGoToObject, this argument specifies a name.</param>
        public void GoTo(object what, object which, object count, object name)
        {
            _wordApplication.Selection.GoTo(ref what, ref which, ref count, ref name);
        }

        /// <summary>
        /// 向右移动一个字符
        /// </summary>
        public void MoveRight()
        {
            MoveRight(1);
        }

        /// <summary>
        /// 向右移动N个字符
        /// </summary>
        /// <param name="num"></param>
        public void MoveRight(int num)
        {
            object unit = Word.WdUnits.wdCharacter;
            object count = num;
            MoveRight(unit, count, missing);
        }

        /// <summary>
        /// 向下移动一个字符
        /// </summary>
        public void MoveDown()
        {
            MoveDown(1);
        }

        /// <summary>
        /// 向下移动N个字符
        /// </summary>
        /// <param name="num"></param>
        public void MoveDown(int num)
        {
            object unit = Word.WdUnits.wdCharacter;
            object count = num;
            MoveDown(unit, count, missing);
        }
        /// <summary>
        /// 光标上移 
        /// Moves the selection up and returns the number of units it's been moved.
        /// </summary>
        /// <param name="unit">Optional Object. The unit by which to move the selection. Can be one of the following WdUnits constants: wdLine, wdParagraph, wdWindow or wdScreen etc. The default value is wdLine.</param>
        /// <param name="count">Optional Object. The number of units the selection is to be moved. The default value is 1.</param>
        /// <param name="extend">Optional Object. Can be either wdMove or wdExtend. If wdMove is used, the selection is collapsed to the end point and moved up. If wdExtend is used, the selection is extended up. The default value is wdMove.</param>
        /// <returns></returns>
        public int MoveUp(object unit, object count, object extend)
        {
            return _wordApplication.Selection.MoveUp(ref unit, ref count, ref extend);
        }

        /// <summary>
        /// 光标下移 
        /// Moves the selection down and returns the number of units it's been moved.
        /// 参数说明详见MoveUp
        /// </summary>
        public int MoveDown(object unit, object count, object extend)
        {
            return _wordApplication.Selection.MoveDown(ref unit, ref count, ref extend);
        }

        /// <summary>
        /// 光标左移 
        /// Moves the selection to the left and returns the number of units it's been moved.
        /// 参数说明详见MoveUp
        /// </summary>
        public int MoveLeft(object unit, object count, object extend)
        {
            return _wordApplication.Selection.MoveLeft(ref unit, ref count, ref extend);
        }

        /// <summary>
        /// 光标右移 
        /// Moves the selection to the left and returns the number of units it's been moved.
        /// 参数说明详见MoveUp
        /// </summary>
        public int MoveRight(object unit, object count, object extend)
        {
            return _wordApplication.Selection.MoveRight(ref unit, ref count, ref extend);
        }

        #endregion

        #region 查找、替换

        /// <summary>
        /// 替换书签内容
        /// </summary>
        /// <param name="bookMarkName">书签名</param>
        /// <param name="text">替换后的内容</param>
        public void ReplaceBookMark(string bookMarkName, string text)
        {
            bool isExist = GoToBookMark(bookMarkName);
            if (isExist)
            {
                InsertText(text);
            }
        }

        /// <summary>
        /// 替换
        /// </summary>
        /// <param name="oldText">待替换的文本</param>
        /// <param name="newText">替换后的文本</param>
        /// <param name="replaceType">All:替换所有、None:不替换、FirstOne:替换第一个</param>
        /// <param name="isCaseSensitive">大小写是否敏感</param>
        /// <returns></returns>
        public bool Replace(string oldText, string newText, string replaceType, bool isCaseSensitive)
        {
            if (_wordDocument == null)
            {
                _wordDocument = _wordApplication.ActiveDocument;

            }
            object findText = oldText;
            object replaceWith = newText;
            object wdReplace;
            object matchCase = isCaseSensitive;
            switch (replaceType)
            {
                case "All":
                    wdReplace = Microsoft.Office.Interop.Word.WdReplace.wdReplaceAll;
                    break;
                case "None":
                    wdReplace = Microsoft.Office.Interop.Word.WdReplace.wdReplaceNone;
                    break;
                case "FirstOne":
                    wdReplace = Microsoft.Office.Interop.Word.WdReplace.wdReplaceOne;
                    break;
                default:
                    wdReplace = Microsoft.Office.Interop.Word.WdReplace.wdReplaceOne;
                    break;
            }
            _wordDocument.Content.Find.ClearFormatting();//移除Find的搜索文本和段落格式设置

            return _wordDocument.Content.Find.Execute(ref findText, ref matchCase, ref missing, ref missing,
                  ref missing, ref missing, ref missing, ref missing, ref missing, ref replaceWith,
                  ref wdReplace, ref missing, ref missing, ref missing, ref missing);
        }

        #endregion

        #region 插入、删除操作

        /// <summary>
        /// 插入文本 Inserts the specified text.
        /// </summary>
        /// <param name="text"></param>
        public void InsertText(string text)
        {
            _wordApplication.Selection.TypeText(text);
        }

        /// <summary>
        /// Enter(换行) Inserts a new, blank paragraph.
        /// </summary>
        public void InsertLineBreak()
        {
            _wordApplication.Selection.TypeParagraph();
        }
        /// <summary>
        /// 插入图片（图片大小自适应）
        /// </summary>
        /// <param name="fileName">图片名（包含路径）</param>
        public void InsertPic(string fileName)
        {
            object range = _wordApplication.Selection.Range;
            InsertPic(fileName, missing, missing, range);
        }

        /// <summary>
        /// 插入图片
        /// </summary>
        /// <param name="fileName">图片名（包含路径）</param>
        /// <param name="width">设置宽度</param>
        /// <param name="height">设置高度</param>
        public void InsertPic(string fileName, float width, float height)
        {
            object range = _wordApplication.Selection.Range;
            InsertPic(fileName, missing, missing, range, width, height);
        }

        /// <summary>
        /// 插入图片（带标题）
        /// </summary>
        /// <param name="fileName">图片名（包含路径）</param>
        /// <param name="width">设置宽度</param>
        /// <param name="height">设置高度<</param>
        /// <param name="caption">标题或备注文字</param>
        public void InsertPic(string fileName, float width, float height, string caption)
        {
            object range = _wordApplication.Selection.Range;
            InsertPic(fileName, missing, missing, range, width, height, caption);
        }

        /// <summary>
        /// 插入图片（带标题）
        /// </summary>
        public void InsertPic(string FileName, object LinkToFile, object SaveWithDocument, object Range, float Width, float Height, string caption)
        {
            _wordApplication.Selection.InlineShapes.AddPicture(FileName, ref LinkToFile, ref SaveWithDocument, ref Range).Select();
            if (Width > 0)
            {
                _wordApplication.Selection.InlineShapes[1].Width = Width;
            }
            if (Height > 0)
            {
                _wordApplication.Selection.InlineShapes[1].Height = Height;
            }

            object Label = Word.WdCaptionLabelID.wdCaptionFigure;
            object Title = caption;
            object TitleAutoText = "";
            object Position = Word.WdCaptionPosition.wdCaptionPositionBelow;
            object ExcludeLabel = true;
            _wordApplication.Selection.InsertCaption(ref Label, ref Title, ref TitleAutoText, ref Position, ref ExcludeLabel);
            MoveRight();
        }

        /// <summary>
        /// Adds a picture to a document.
        /// </summary>
        /// <param name="FileName">Required String. The path and file name of the picture.</param>
        /// <param name="LinkToFile">Optional Object. True to link the picture to the file from which it was created. False to make the picture an independent copy of the file. The default value is False.</param>
        /// <param name="SaveWithDocument">Optional Object. True to save the linked picture with the document. The default value is False.</param>
        /// <param name="Range">Optional Object. The location where the picture will be placed in the text. If the range isn't collapsed, the picture replaces the range; otherwise, the picture is inserted. If this argument is omitted, the picture is placed automatically.</param>
        /// <param name="Width">Sets the width of the specified object, in points. </param>
        /// <param name="Height">Sets the height of the specified inline shape. </param>
        public void InsertPic(string FileName, object LinkToFile, object SaveWithDocument, object Range, float Width, float Height)
        {
            _wordApplication.Selection.InlineShapes.AddPicture(FileName, ref LinkToFile, ref SaveWithDocument, ref Range).Select();
            _wordApplication.Selection.InlineShapes[1].Width = Width;
            _wordApplication.Selection.InlineShapes[1].Height = Height;
            MoveRight();
        }

        /// <summary>
        /// Adds a picture to a document.
        /// </summary>
        /// <param name="FileName">Required String. The path and file name of the picture.</param>
        /// <param name="LinkToFile">Optional Object. True to link the picture to the file from which it was created. False to make the picture an independent copy of the file. The default value is False.</param>
        /// <param name="SaveWithDocument">Optional Object. True to save the linked picture with the document. The default value is False.</param>
        /// <param name="Range">Optional Object. The location where the picture will be placed in the text. If the range isn't collapsed, the picture replaces the range; otherwise, the picture is inserted. If this argument is omitted, the picture is placed automatically.</param>
        public void InsertPic(string FileName, object LinkToFile, object SaveWithDocument, object Range)
        {
            _wordApplication.Selection.InlineShapes.AddPicture(FileName, ref LinkToFile, ref SaveWithDocument, ref Range);
        }

        /// <summary>
        /// 插入书签
        /// 如过存在同名书签，则先删除再插入
        /// </summary>
        /// <param name="bookMarkName">书签名</param>
        public void InsertBookMark(string bookMarkName)
        {
            //存在则先删除
            if (_wordDocument.Bookmarks.Exists(bookMarkName))
            {
                DeleteBookMark(bookMarkName);
            }
            object range = _wordApplication.Selection.Range;
            _wordDocument.Bookmarks.Add(bookMarkName, ref range);

        }

        /// <summary>
        /// 删除书签
        /// </summary>
        /// <param name="bookMarkName">书签名</param>
        public void DeleteBookMark(string bookMarkName)
        {
            if (_wordDocument.Bookmarks.Exists(bookMarkName))
            {
                var bookMarks = _wordDocument.Bookmarks;
                for (int i = 1; i <= bookMarks.Count; i++)
                {
                    object index = i;
                    var bookMark = bookMarks.get_Item(ref index);
                    if (bookMark.Name == bookMarkName)
                    {
                        bookMark.Delete();
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// 删除所有书签
        /// </summary>
        public void DeleteAllBookMark()
        {
            for (; _wordDocument.Bookmarks.Count > 0;)
            {
                object index = _wordDocument.Bookmarks.Count;
                var bookmark = _wordDocument.Bookmarks.get_Item(ref index);
                bookmark.Delete();
            }
        }
        #endregion

        #region 对表格（Table）操作

        /// <summary>
        /// 新增自定表格
        /// </summary>
        /// <param name="NumRows">行数</param>
        /// <param name="NumColumns">列数</param>
        /// <returns></returns>
        public Word.Table AddTable(int NumRows, int NumColumns)
        {
            return AddTable(_wordApplication.Selection.Range, NumRows, NumColumns, missing, missing);
        }

        /// <summary>
        /// 新增自定义表格
        /// </summary>
        /// <param name="NumRows">行数</param>
        /// <param name="NumColumns">列数</param>
        /// <param name="AutoFitBehavior">自适应方式</param>
        /// <returns></returns>
        public Word.Table AddTable(int NumRows, int NumColumns, Word.WdAutoFitBehavior AutoFitBehavior)
        {
            return AddTable(_wordApplication.Selection.Range, NumRows, NumColumns, missing, AutoFitBehavior);
        }

        /// <summary>
        /// 新增自定义表格：Returns a Table object that represents a new, blank table added to a document.
        /// </summary>
        /// <param name="Range">Required Range object. The range where you want the table to appear. The table replaces the range, if the range isn't collapsed.</param>
        /// <param name="NumRows">Required Integer. The number of rows you want to include in the table.</param>
        /// <param name="NumColumns">Required Integer. The number of columns you want to include in the table.</param>
        /// <param name="DefaultTableBehavior">Optional Object. Sets a value that specifies whether Word automatically resizes cells in tables to fit the cells’ contents (AutoFit). Can be either of the following constants: wdWord8TableBehavior (AutoFit disabled) or wdWord9TableBehavior (AutoFit enabled). The default constant is wdWord8TableBehavior.</param>
        /// <param name="AutoFitBehavior">Optional Object. Sets the AutoFit rules for how Microsoft Word sizes tables. Can be one of the following WdAutoFitBehavior constants: wdAutoFitContent, wdAutoFitFixed, or wdAutoFitWindow. If DefaultTableBehavior is set to wdWord8TableBehavior, this argument is ignored.</param>
        /// <returns></returns>
        public Word.Table AddTable(Word.Range Range, int NumRows, int NumColumns, object DefaultTableBehavior, object AutoFitBehavior)
        {
            if (_wordDocument == null)
            {
                _wordDocument = _wordApplication.ActiveDocument;
            }
            return _wordDocument.Tables.Add(Range, NumRows, NumColumns, ref DefaultTableBehavior, ref AutoFitBehavior);
        }

        /// <summary>
        /// 在表尾插入行
        /// </summary>
        /// <param name="table"></param>
        /// <returns></returns>
        public Word.Row AddRow(Word.Table table)
        {
            return AddRow(table, missing);
        }

        /// <summary>
        /// 插入行到指定列前
        /// 如object beforeRow = table.Rows[1];
        /// </summary>
        /// <param name="table"></param>
        /// <param name="beforeRow">Optional Object. A Row object that represents the row that will appear immediately below the new row.</param>
        /// <returns></returns>
        ///
        public Word.Row AddRow(Word.Table table, object beforeRow)
        {
            return table.Rows.Add(ref beforeRow);
        }
        /// <summary>
        /// 在当前位置插入指定数量行
        /// Inserts the specified number of new rows above the row that contains the selection.
        /// </summary>
        /// <param name="numRows"></param>
        public void InsertRows(int numRows)
        {
            object NumRows = numRows;
            object wdCollapseStart = Word.WdCollapseDirection.wdCollapseStart;
            _wordApplication.Selection.InsertRows(ref NumRows);
            _wordApplication.Selection.Collapse(ref wdCollapseStart);
        }


        /// <summary>
        /// 向上移动一个单元格
        /// </summary>
        public void MoveToUpCell()
        {
            MoveToUpCell(1);
        }

        /// <summary>
        /// 向上移动N个单元格
        /// </summary>
        /// <param name="num"></param>
        public void MoveToUpCell(int num)
        {
            object unit = Word.WdUnits.wdCell;
            MoveUp(unit, num, missing);
        }

        /// <summary>
        /// 向下移动一个单元格
        /// </summary>
        public void MoveToDownCell()
        {
            MoveToDownCell(1);
        }

        /// <summary>
        /// 向下移动N个单元格
        /// </summary>
        public void MoveToDownCell(int num)
        {
            //  object unit = Word.WdUnits.wdCell;
            MoveDown(missing, num, missing);
        }

        /// <summary>
        /// 向右移动一个单元格
        /// </summary>
        public void MoveToRightCell()
        {
            MoveToRightCell(1);
        }

        /// <summary>
        /// 向右移动N个单元格
        /// </summary>
        public void MoveToRightCell(int num)
        {
            object unit = Word.WdUnits.wdCell;
            MoveRight(unit, num, missing);
        }

        /// <summary>
        /// 向左移动一个单元格
        /// </summary>
        public void MoveToLeftCell()
        {
            MoveToLeftCell(1);
        }

        /// <summary>
        /// 向左移动N个单元格
        /// </summary>
        public void MoveToLeftCell(int num)
        {
            object unit = Word.WdUnits.wdCell;
            MoveLeft(unit, num, missing);
        }

        /// <summary>
        /// 向上移动1个单元格，并加入选中（Section）
        /// </summary>
        public void MoveExtendToUpCell()
        {
            MoveExtendToUpCell(1);
        }

        /// <summary>
        /// 向上移动N个单元格，并加入选中（Section）
        /// </summary>
        /// <param name="num"></param>
        public void MoveExtendToUpCell(int num)
        {
            object extend = Word.WdMovementType.wdExtend;
            MoveUp(missing, num, extend);
        }

        /// <summary>
        /// 向下移动1个单元格，并加入选中（Section）
        /// </summary>
        public void MoveExtendToDownCell()
        {
            MoveExtendToDownCell(1);
        }

        /// <summary>
        /// 向下移动N个单元格，并加入选中（Section）
        /// </summary>
        /// <param name="num"></param>
        public void MoveExtendToDownCell(int num)
        {
            object extend = Word.WdMovementType.wdExtend;
            MoveDown(missing, num, extend);
        }

        #endregion
    }
}
