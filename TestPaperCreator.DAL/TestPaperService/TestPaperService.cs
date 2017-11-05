using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestPaperCreator.DAL.Utility;
using System.Data;

namespace TestPaperCreator.DAL.TestPaperService
{
    public class TestPaperService
    {
        #region 私有成员
        private static string conn = Conn.GetSqlServerConn("ALiConnection");
        #endregion

        #region 查询专业列表
        /// <summary>
        /// 查询专业列表
        /// </summary>
        /// <returns>专业列表</returns>
        public static List<MODEL.TestPaper.Condition> GetMajor()
        {
            List<MODEL.TestPaper.Condition> majorlist = new List<MODEL.TestPaper.Condition>();
            string sql = "select id, Value from Major where Flag = 1";
            DataSet ds = SqlHelper.ExecuteDataset(conn, CommandType.Text, sql);
            foreach(DataRow dr in ds.Tables[0].Rows)
            {
                MODEL.TestPaper.Condition major = new MODEL.TestPaper.Condition();
                major.id = (int)dr[0];
                major.value = (string)dr[1];
                majorlist.Add(major);
            }
            return majorlist;
        }
        #endregion

        #region 增删改筛选条件
        /// <summary>
        /// 增删筛选条件
        /// </summary>
        /// <param name="Condition">筛选条件(数据库表名，例如Course，Section)</param>
        /// <param name="Value">条件名称(显示名称，例如课程，章节)</param>
        /// <param name="Flag">1：生效，0：不生效</param>
        /// <returns>是否成功</returns>
        public static bool EditCondition(string Condition, string Value, int Flag)
        {
            string sql = "select Value from " + Condition + " where Value = '" + Value + "'";
            var result = SqlHelper.ExecuteScalar(conn, CommandType.Text, sql);
            if (result == null)
            {
                sql = "insert into " + Condition + " (Value, Flag) values ('" + Value + "', " + Flag + ")";
            }
            else
            {
                sql = "update " + Condition + " set Flag =" + Flag + "  where Value = '" + Value + "'";
            }
            try
            {
                SqlHelper.ExecuteNonQuery(conn, CommandType.Text, sql);
                if (Condition == "Major")
                {
                    if (Flag == 1)
                    {
                        sql = "select id from " + Condition + " where Value = '" + Value + "'";
                        int majorid = (int)SqlHelper.ExecuteScalar(conn, CommandType.Text, sql);
                        InsertMajor(majorid);
                    }
                    else
                    {
                        sql = "select id from " + Condition + " where Value = '" + Value + "'";
                        int majorid = (int)SqlHelper.ExecuteScalar(conn, CommandType.Text, sql);
                        DeleteMajor(majorid);
                    }
                }
            }
            catch
            {
                return false;
            }
            return true;
        }
        #endregion

        #region 插入专业
        public static void InsertMajor(int majorid)
        {
            string sql = "insert into Major_Question (MajorID,QuestionID,Weight) values ";
            string ssql = "select ID from Questions where Flag = 1";
            DataSet ds = SqlHelper.ExecuteDataset(conn, CommandType.Text, ssql);
            foreach(DataRow dr in ds.Tables[0].Rows)
            {
                sql += "("+ majorid+"," + dr[0].ToString()+",0),";
            }
            sql = sql.Substring(0, sql.Length - 1);
            SqlHelper.ExecuteNonQuery(conn, CommandType.Text, sql);
        }
        #endregion

        #region 删除专业
        public static void DeleteMajor(int majorid)
        {
            string sql = "delete from Major_Question where MajorID = " + majorid;
            SqlHelper.ExecuteNonQuery(conn, CommandType.Text, sql);
        }
        #endregion

        #region 查询课程
        public static List<MODEL.TestPaper.Condition> GetCourse(int flag)
        {
            List<MODEL.TestPaper.Condition> courselist = new List<MODEL.TestPaper.Condition>();
            string sql;
            if(flag == 1)
            {
                sql = "select ID, Value from Course where Flag = " + flag;
            }
            else
            {
                sql = "select ID, Value from Course";
            }
            DataSet ds = SqlHelper.ExecuteDataset(conn, CommandType.Text, sql);
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                MODEL.TestPaper.Condition course = new MODEL.TestPaper.Condition();
                course.id = (int)dr[0];
                course.value = (string)dr[1];
                courselist.Add(course);
            }
            return courselist;
        }
        #endregion

        #region 查询难度
        public static List<MODEL.TestPaper.Condition> GetDifficulty(int flag)
        {
            List<MODEL.TestPaper.Condition> difficultylist = new List<MODEL.TestPaper.Condition>();
            string sql;
            if(flag == 1)
            {
                sql = "select ID, Value from Difficulty where Flag = " + flag;
            }
            else
            {
                sql = "select ID, Value from Difficulty";
            }
            DataSet ds = SqlHelper.ExecuteDataset(conn, CommandType.Text, sql);
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                MODEL.TestPaper.Condition difficulty = new MODEL.TestPaper.Condition();
                difficulty.id = (int)dr[0];
                difficulty.value = (string)dr[1];
                difficultylist.Add(difficulty);
            }
            return difficultylist;
        }
        #endregion

        #region 查询章节
        public static List<MODEL.TestPaper.Condition> GetSection(int flag)
        {
            List<MODEL.TestPaper.Condition> sectionlist = new List<MODEL.TestPaper.Condition>();
            string sql;
            if(flag == 1)
            {
                sql = "select ID, Value from Section where Flag = " + flag;
            }
            else
            {
                sql = "select ID, Value from Section";
            }
            DataSet ds = SqlHelper.ExecuteDataset(conn, CommandType.Text, sql);
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                MODEL.TestPaper.Condition section = new MODEL.TestPaper.Condition();
                section.id = (int)dr[0];
                section.value = (string)dr[1];
                sectionlist.Add(section);
            }
            return sectionlist;
        }
        #endregion

        #region 查询题型
        public static List<MODEL.TestPaper.Condition> GetQuestionType(int flag)
        {
            List<MODEL.TestPaper.Condition> typelist = new List<MODEL.TestPaper.Condition>();
            string sql;
            if(flag == 1)
            {
                sql = "select ID, Value from Type where Flag = " + flag;
            }
            else
            {
                sql = "select ID, Value from Type";
            }
            DataSet ds = SqlHelper.ExecuteDataset(conn, CommandType.Text, sql);
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                MODEL.TestPaper.Condition type = new MODEL.TestPaper.Condition();
                type.id = (int)dr[0];
                type.value = (string)dr[1];
                typelist.Add(type);
            }
            return typelist;
        }
        #endregion

        #region 获取试题ID最大值
        /// <summary>
        /// 获取试题ID最大值
        /// </summary>
        /// <returns>ID</returns>
        public static int GetMaxQuestionID()
        {
            string sql = "SELECT MAX(ID) as maxid FROM Questions";
            var maxid = SqlHelper.ExecuteScalar(conn, CommandType.Text, sql);
            //int maxid = SqlHelper.ExecuteNonQuery(conn, System.Data.CommandType.Text, sql);
            return maxid == DBNull.Value ? 0 : (int)maxid;
        }
        #endregion

        #region 插入试题
        /// <summary>
        /// 插入试题
        /// </summary>
        /// <param name="course">课程</param>
        /// <param name="questiontype">题型</param>
        /// <param name="section">章节</param>
        /// <param name="difficulty">难易度</param>
        /// <returns>成功或失败</returns>
        public static void InsertQuestion(int course, int questiontype, int section, int difficulty, string content,int questionid)
        {
            content = content.Replace("'", " ");
            content = content.Replace('"', ' ');
            string sql = "insert into Questions (Type, Course, Section, Difficulty, Weight, Time, Flag,Content) values (" + questiontype + "," + course + "," + section + "," + difficulty + ",0,'" + DateTime.Now.ToShortDateString() + "',1,'"+content.ToString()+"')";
            try
            {
                SqlHelper.ExecuteNonQuery(conn, CommandType.Text, sql);
            }
            catch (Exception e)
            {
                throw e;
            }
            sql = "select id from Major where Flag = 1 ";
            DataSet ds = SqlHelper.ExecuteDataset(conn, CommandType.Text, sql);
            List<int> majoridlist = new List<int>();
            foreach(DataRow dr in ds.Tables[0].Rows)
            {
                majoridlist.Add((int)dr[0]);
            }
            foreach(int i in majoridlist)
            {
                sql = "insert into Major_Question values (" + i + "," + questionid + ",0)";
                SqlHelper.ExecuteNonQuery(conn, CommandType.Text, sql);
            }
        }
        #endregion
        
        #region 根据条件获取试题ID集合
        /// <summary>
        /// 根据条件获取试题ID集合
        /// </summary>
        /// <param name="paperlist">试题条件</param>
        /// <returns>试题ID集合</returns>
        public static IDictionary<int,List<int>> GetQuestionID(List<MODEL.TestPaper.Paper> paperlist)
        {
            //试卷字典，key代表题型ID，value代表
            IDictionary<int, List<int>> paperDic = new Dictionary<int, List<int>>();
            
            foreach (MODEL.TestPaper.Paper paper in paperlist)
            {
                List<int> b = new List<int>();//每个题型的最终结果集合
                int course = paper.paperproperty.course;
                int section = paper.paperproperty.section;
                int difficulty = paper.paperproperty.difficulty;
                int questiontype = paper.paperproperty.questiontype;
                int major = paper.paperproperty.major;
                int count = paper.count*2;
                //string sql = "select ID from Questions where Course = " + course + " and Section =" + section + " and Difficulty =" + difficulty + " and Type = " + questiontype + " and Flag=1";
                ////string sql = "select ID form Questions where Course = " + course + " and Section =" + section + " and Difficulty =" + difficulty + " and Type = " + questiontype + "  and Flag=1";
                //DataSet results = SqlHelper.ExecuteDataset(conn, CommandType.Text, sql);
                ////results.Tables[0].Rows
                //List<int> a = new List<int>();//符合筛选条件的所有集合（已筛选课程，章节，难度，题型，未考虑权重）
                //foreach (DataRow dr in results.Tables[0].Rows)
                //{
                //    a.Add((int)dr[0]);
                //}
                List<int> avaliableID = new List<int>();//考虑权重后可选的ID集合
                Random rd = new Random();
                string sql = "select min(Weight) from Major_Question where MajorID = " + major + " and QuestionID in (select ID from Questions where Course = " + course + " and Section =" + section + " and Difficulty =" + difficulty + " and Type = " + questiontype + " and Flag=1) ";
                int minweight = (int)SqlHelper.ExecuteScalar(conn, CommandType.Text, sql);
                do
                {
                    sql = "select [QuestionID],[Weight] from Major_Question where QuestionID IN (SELECT id from [Questions]  where Course = " + course + " and Section = " + section + " and Difficulty = " + difficulty + " and Type = " + questiontype + " and Flag=1) and MajorID = " + major + " and weight = " + minweight;
                    DataSet ds = SqlHelper.ExecuteDataset(conn, CommandType.Text, sql);
                    foreach(DataRow dr in ds.Tables[0].Rows)
                    {
                        avaliableID.Add((int)dr[0]);
                    }
                    minweight++;
                } while (avaliableID.Count < count);
                
                //do
                //{
                //    foreach (int i in a)
                //    {
                //        sql = "select Weight from Major_Question where QuestionID = " + i + " and MajorID = " + major;
                //        //sql = "insert into Major_Question (MajorID, QuestionID, Weight) values (1, " + i + ", 0)";
                //        int? result = (int)SqlHelper.ExecuteScalar(conn, CommandType.Text, sql);
                //        if (result <= minweight)
                //        {
                //            avaliableID.Add(i);
                //        }
                //    }
                //    foreach(int i in avaliableID)
                //    {
                //        a.Remove(i);
                //    }
                //    minweight++;
                //} while (avaliableID.Count< paper.count);
                //List<int> SatisfactoryID = GetID(avaliableID, paper.count, (int)minweight,a,major);

                List<int> c = new List<int>();
                for (int i = 0; i < count;)
                {
                    int r = rd.Next(0, avaliableID.Count);
                    if (c.Contains(r))
                    {
                        continue;
                    }
                    b.Add(avaliableID[r]);
                    c.Add(r);
                    i++;
                }
                paperDic.Add(questiontype, b);
                paperDic[questiontype] = b;
            }
            return paperDic;
        }
        #endregion

        #region 确认试卷，增加权重
        public static string Confirm(int majorid,List<int> questionlist)
        {
            int length = questionlist.Count;
            string sql = "";
            try
            {
                foreach (int i in questionlist)
                {
                    sql = "update Major_Question set weight = (select weight from Major_Question where MajorID = " + majorid + " and QuestionID = " + i + ")+1 where MajorID = " + majorid + " and QuestionID = " + i;
                    SqlHelper.ExecuteNonQuery(conn, CommandType.Text, sql);
                }
                return "success";
            }
            catch(Exception e)
            {
                return e.ToString();
            }
            
            //psql = psql.Substring(0, psql.Length - 1);
            //string sql = "update Major_Question set weight == weight+1 where QuestionID in (" + psql + ") and MajorID = " + majorid;
            //try
            //{
            //    SqlHelper.ExecuteScalar(sql, CommandType.Text, sql);
            //    return "success";
            //}
            //catch (Exception e)
            //{
            //    return e.ToString();
            //}
        }
        #endregion

        #region 更换题目
        public static MODEL.TestPaper.Question GetOneQuestion(MODEL.TestPaper.Paper paper,List<int> oldidlist)
        {
            int course = paper.paperproperty.course;
            int section = paper.paperproperty.section;
            int questiontype = paper.paperproperty.questiontype;
            int difficulty = paper.paperproperty.difficulty;
            string sql = "select QuestionID from Major_Question where Questionid in (select id from Questions where Course = " + course + " and Section = " + section + " and Difficulty = " + section + " and Type = " + questiontype + ") and Weight in(select min(Weight) from Major_Question where Questionid in (select id from Questions where Course = " + course + " and Section = " + section + " and Difficulty = " + difficulty + " and Type = " + questiontype + "))";
            //string sql = "select ID from Questions where Course = " + course + " and Section =" + section + " and Difficulty =" + difficulty + " and Type = " + questiontype + " and Weight in(select MIN(Weight) from Questions)  and Flag=1";
            DataSet results = SqlHelper.ExecuteDataset(conn, CommandType.Text, sql);
            List<int> a = new List<int>();
            foreach (DataRow dr in results.Tables[0].Rows)
            {
                a.Add((int)dr[0]);
            }
            Random rd = new Random();
            int result = rd.Next(1, a.Count);
            while (oldidlist.Contains(a[result]))
            {
                result = rd.Next(0, a.Count);
            }
            return GetAQuestionByID(a[result]);
        }
        #endregion

        #region 根据条件查询试题
        /// <summary>
        /// 根据条件查询试题
        /// </summary>
        /// <param name="course">课程ID</param>
        /// <param name="section">章节ID</param>
        /// <param name="difficulty">难度ID</param>
        /// <param name="questiontype">题型ID</param>
        /// <param name="keyword">搜索关键字</param>
        /// <returns>试题列表</returns>
        public static List<MODEL.TestPaper.Question> GetQuestionList(int course, int section, int difficulty, int questiontype, string keyword)
        {
            string sql = "SELECT questions.ID, questions.Type, questiontype.Value, questions.Course, course.Value, questions.Section, section.Value, questions.Difficulty, difficulty.Value, questions.Content FROM Questions questions, Difficulty difficulty, Course course, Section section, Type questiontype WHERE questions.Flag = 1 AND course.Flag = 1 AND questiontype.Flag = 1 AND[Section].[Flag] = 1 AND[Difficulty].[Flag] = 1 AND[Questions].[Type] = questiontype.[ID] AND[Questions].[Course] = [Course].[ID] AND[Questions].[Section] = [Section].[ID] AND[Questions].[Difficulty] = [Difficulty].[ID]";
            if (course != 0)
            {
                sql += "and Course = " + course;
            }
            if (section != 0)
            {
                sql += "and Section = " + section;
            }
            if (difficulty != 0)
            {
                sql += "and Difficulty = " + difficulty;
            }
            if (questiontype != 0)
            {
                sql += "and Type = " + questiontype;
            }
            if (keyword != null)
            {
                sql += "and Content like %'" + keyword + "'%";
            }
            List<MODEL.TestPaper.Question> questionlist = new List<MODEL.TestPaper.Question>();

            DataSet result = SqlHelper.ExecuteDataset(conn, CommandType.Text, sql);
            foreach (DataRow dr in result.Tables[0].Rows)
            {
                MODEL.TestPaper.Question question = new MODEL.TestPaper.Question();
                question.ID = (int)dr[0];
                question.Type = (int)dr[1];
                question.TypeName = (string)dr[2];
                question.Course = (int)dr[3];
                question.CourseName = (string)dr[4];
                question.Section = (int)dr[5];
                question.SectionName = (string)dr[6];
                question.Difficulty = (int)dr[7];
                question.DifficultyName = (string)dr[8];
                if (dr[9] != System.DBNull.Value)
                {
                    question.Content = (string)dr[9];
                }
                else
                {
                    question.Content = "";
                }
                questionlist.Add(question);
            }
            return questionlist;
        }
        #endregion

        #region 删除试题
        public static bool DeleteQuestion(int questionid)
        {
            string sql = "update Questions set Flag = 0 where id = " + questionid;
            try
            {
                SqlHelper.ExecuteNonQuery(conn, CommandType.Text, sql);
                sql = "delete from Major_Question where QuestionID = " + questionid;
                SqlHelper.ExecuteNonQuery(conn, CommandType.Text, sql);
                return true;
            }
            catch
            {
                return true;
            }
        }
        #endregion

        #region 根据ID获取题目
        public static MODEL.TestPaper.Question GetAQuestionByID(int id)
        {
            MODEL.TestPaper.Question question = new MODEL.TestPaper.Question();
            string sql = "select questions.ID,questions.Type, questiontype.Value ,questions.Course, course.Value,questions.Section, section.Value,questions.Difficulty, difficulty.Value, questions.Content from Questions questions, Difficulty difficulty, Course course, Section section, Type questiontype where questions.Type = questiontype.ID and questions.Course = course.ID and questions.Section = section.ID and questions.Difficulty = difficulty.ID and questions.ID =" + id;
            DataSet result = SqlHelper.ExecuteDataset(conn, CommandType.Text, sql);
            foreach (DataRow dr in result.Tables[0].Rows)
            {
                question.ID = (int)dr[0];
                question.Type = (int)dr[1];
                question.TypeName = (string)dr[2];
                question.Course = (int)dr[3];
                question.CourseName = (string)dr[4];
                question.Section = (int)dr[5];
                question.SectionName = (string)dr[6];
                question.Difficulty = (int)dr[7];
                question.DifficultyName = (string)dr[8];
                if (dr[9] != DBNull.Value)
                {
                    question.Content = (string)dr[9];
                }
                else
                {
                    question.Content = "";
                }
            }
            return question;
        }
        #endregion

        #region 根据ID获取答案
        public static MODEL.TestPaper.Question GetAnswerByID(int id)
        {
            MODEL.TestPaper.Question answer = new MODEL.TestPaper.Question();
            string sql = "select questions.ID,questions.Type, questiontype.Value ,questions.Course, course.Value,questions.Section, section.Value,questions.Difficulty, difficulty.Value, questions.Content from Questions questions, Difficulty difficulty, Course course, Section section, Type questiontype where questions.Type = questiontype.ID and questions.Course = course.ID and questions.Section = section.ID and questions.Difficulty = difficulty.ID and questions.ID =" + id;
            DataSet result = SqlHelper.ExecuteDataset(conn, CommandType.Text, sql);
            foreach (DataRow dr in result.Tables[0].Rows)
            {
                answer.ID = (int)dr[0];
                answer.Type = (int)dr[1];
                answer.TypeName = (string)dr[2];
                answer.Course = (int)dr[3];
                answer.CourseName = (string)dr[4];
                answer.Section = (int)dr[5];
                answer.SectionName = (string)dr[6];
                answer.Difficulty = (int)dr[7];
                answer.DifficultyName = (string)dr[8];
                if (dr[9] != DBNull.Value)
                {
                    answer.Content = (string)dr[9];
                }
                else
                {
                    answer.Content = "";
                }
            }
            return answer;
        }
        #endregion

        #region 清除题目中的某个字符
        public static void ClearQuestionMark(string tobecleared)
        {
            string sql = "update Questions set Content=replace( Content, '" + tobecleared + "', '') where Content like '%" + tobecleared.ToString() + "%'";
            SqlHelper.ExecuteNonQuery(conn, CommandType.Text, sql);
        }
        #endregion

        #region 根据条件ID获取条件名称
        /// <summary>
        /// 根据条件ID获取条件名称
        /// 例：输入1，Course，就能获取课程表中ID为1的课程的名称
        /// </summary>
        /// <param name="typeid">条件ID</param>
        /// <param name="ConditionName">条件名称</param>
        /// <returns>条件名称</returns>
        public static string GetConditionNameByConditionID(int typeid,string ConditionName)
        {
            string sql = "select Value from " + ConditionName + " where ID = " + typeid;
            var typename = SqlHelper.ExecuteScalar(conn, CommandType.Text, sql);
            return typename == DBNull.Value ? "" : typename.ToString();
        }
        #endregion

        #region 自动填表
        public static IDictionary<string,string> GetProperty()
        {
            Dictionary<string, string> property = new Dictionary<string, string>();
            string sql = "select schoolname,collegename,schoolyear,term,testtype,volume,length,testmethod,grade,class from Property where id = 1";
            DataSet result = SqlHelper.ExecuteDataset(conn, CommandType.Text, sql);
            foreach(DataRow dr in result.Tables[0].Rows)
            {
                property["schoolname"] = dr[0].ToString();
                property["collegename"] = dr[1].ToString();
                property["schoolyear"] = dr[2].ToString();
                property["term"] = dr[3].ToString();
                property["testtype"] = dr[4].ToString();
                property["volume"] = dr[5].ToString();
                property["length"] = dr[6].ToString();
                property["testmethod"] = dr[7].ToString();
                property["grade"] = dr[8].ToString();
                property["class"] = dr[9].ToString();
            }
            return property;
        }
        #endregion

        #region 补齐空缺的M-Q值
        public static void magic()
        {
            string sql = "select id from questions where flag = 1";
            List<int> questionidlist = new List<int>();
            DataSet result = SqlHelper.ExecuteDataset(conn, CommandType.Text, sql);
            foreach(DataRow dr in result.Tables[0].Rows)
            {
                questionidlist.Add((int)dr[0]);
            }
            List<int> majoridlist = new List<int>();
            sql = "select id from major where flag = 1";
            result = SqlHelper.ExecuteDataset(conn, CommandType.Text, sql);
            foreach (DataRow dr in result.Tables[0].Rows)
            {
                majoridlist.Add((int)dr[0]);
            }
            sql = "select max(id) from Major_Question";
            int maxid = 1;
            var maxidinDB = SqlHelper.ExecuteScalar(conn, CommandType.Text, sql);
            if(maxidinDB != DBNull.Value)
            {
                maxid = (int)maxidinDB;
            }
            sql = "insert into Major_Question values ";
            foreach(var i in majoridlist)
            {
                foreach(var j in questionidlist)
                {
                    sql += "(" + i + "," + j + ",0),";
                    maxid++;
                }
            }
            sql = sql.Remove(sql.Length - 1,1);
        }
        #endregion

        #region 修改试题
        /// <summary>
        /// 修改试题
        /// </summary>
        /// <param name="questionid">试题ID</param>
        /// <param name="condition">修改字段和值的列表</param>
        public static void UpdateQuestion(int questionid,List<string> condition)
        {
            string sql = "update Questions set " + condition[0] + " = '" + condition[1] + "' where id = " + questionid;
            SqlHelper.ExecuteNonQuery(conn, CommandType.Text, sql);
        }
        #endregion

        public static Dictionary<int, string> GetMaxCount(int course, int questiontype, int section, int difficulty)
        {
            string sql = "select count(id) from questions where course = " + course + "and section = " + section + "and type = " + questiontype + "and difficulty = " + difficulty + " and flag = 1";
            int count = (int)SqlHelper.ExecuteScalar(conn, CommandType.Text, sql);
            Dictionary<int, string> result = new Dictionary<int, string>();
            result.Add(5, count.ToString());
            string coursename = GetConditionNameByConditionID(course, "Course");
            string difficultyname = GetConditionNameByConditionID(difficulty, "Difficulty");
            string sectionname = GetConditionNameByConditionID(section, "Section");
            string questiontypename = GetConditionNameByConditionID(questiontype, "Type");
            result.Add(1, coursename);
            result.Add(2, questiontypename);
            result.Add(3, sectionname);
            result.Add(4, difficultyname);
            return result;
        }
    }
}