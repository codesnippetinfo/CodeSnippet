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
        [BsonExtraElements]
        public Dictionary<string, object> CodeLineCnt;
        /// <summary>
        /// Links
        /// </summary>
        public List<string> Url;

        /// <summary>
        ///  总行数
        /// </summary>
        /// <returns></returns>
        public int TotalLineCnt()
        {
            int linecnt = LineCnt;
            if (CodeLineCnt != null) linecnt += CodeLineCnt.Sum((y) => { return (int)y.Value; });
            linecnt += ImageList.Count;
            return linecnt;
        }
    }
}
