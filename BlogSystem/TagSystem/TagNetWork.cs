using System;
using System.Collections.Generic;

namespace BlogSystem.TagSystem
{
    /// <summary>
    /// Tag网络
    /// </summary>
    public class TagNetWork
    {
        /// <summary>
        /// TAG号码
        /// </summary>
        string TagID { get; set; }
        /// <summary>
        /// 相关TAG号码
        /// </summary>
        string RelationTagID { get; set; }
        /// <summary>
        /// 关系号码
        /// </summary>
        string RelationShipID { get; set; }

        /// <summary>
        /// 获得该TAG的关联TAG(从该TAG开始的)
        /// </summary>
        /// <param name="TagID"></param>
        /// <returns></returns>
        public List<string> GetRelationTagIDFrom(string TagID)
        {
            return new List<string>();
        }

        /// <summary>
        /// 获得该TAG的关联TAG（到该TAG的）
        /// </summary>
        /// <param name="TagID"></param>
        /// <returns></returns>
        public List<string> GetRelationTagIDTo(string TagID)
        {
            return new List<string>();
        }

        /// <summary>
        /// 获得Tag列表
        /// </summary>
        public static void LoadTagList()
        {
            throw new NotImplementedException();            
        }

        /// <summary>
        /// 从文本中提取存在的Tag
        /// </summary>
        /// <param name="strContent"></param>
        public static void GetTagListFromContent(string strContent)
        {
            throw new NotImplementedException();
        }

    }
}
