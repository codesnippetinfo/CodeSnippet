using MongoDB.Bson.Serialization.Attributes;
using System.Collections.Generic;
using System.Linq;

namespace BlogSystem.BussinessLogic
{
    /// <summary>
    /// 统计结果结构
    /// </summary>
    public class AnlyzeResult
    {
        /// <summary>
        /// 行数（去除代码，图片等）
        /// </summary>
        public int LineCnt;
        /// <summary>
        /// 图片路径数组
        /// </summary>
        public List<string> ImageList;
        /// <summary>
        /// 语言VS行数
        /// </summary>
        public List<struCodeLine> CodeLineCnt;
        /// <summary>
        /// Links
        /// </summary>
        public List<string> Url;
        /// <summary>
        /// CodeLine
        /// </summary>
        public class struCodeLine
        {
            /// <summary>
            /// 语言
            /// </summary>
            public string Language;
            /// <summary>
            /// 行数
            /// </summary>
            public int Count;
        }

        /// <summary>
        /// 是否包含语言
        /// </summary>
        public bool IsContainLanguage(string lan)
        {
            foreach (var item in CodeLineCnt)
            {
                if (item.Language.Equals(lan)) return true;
            }
            return false;
        }

        /// <summary>
        ///  总行数
        /// </summary>
        /// <returns></returns>
        public int TotalLineCnt()
        {
            int linecnt = LineCnt;
            if (CodeLineCnt != null) linecnt += CodeLineCnt.Sum((y) => { return (int)y.Count; });
            linecnt += ImageList.Count;
            return linecnt;
        }
    }
}
