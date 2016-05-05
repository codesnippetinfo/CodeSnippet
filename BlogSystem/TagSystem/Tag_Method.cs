using System.Collections.Generic;
using BlogSystem.Entity;
using InfraStructure.DataBase;

namespace BlogSystem.TagSystem
{
    public partial class Tag : EntityBase
    {
        /// <summary>
        ///     返回所有标签
        /// </summary>
        /// <returns>官方标签</returns>
        public static List<Tag> GetAllTags()
        {
            return MongoDbRepository.GetRecList<Tag>();
        }

        /// <summary>
        ///     获得某人的TagList
        /// </summary>
        public static Dictionary<string, int> GetTagListByOwnerId(string OwnerId)
        {
            var ArticleList = Article.GetListByOwnerId(OwnerId);
            var TagCnt = new Dictionary<string, int>();
            foreach (var article in ArticleList)
            {
                var tags = TagUtility.getTagsFromTitle(article.Title);
                foreach (var tag in tags)
                {
                    if (TagCnt.ContainsKey(tag))
                    {
                        TagCnt[tag]++;
                    }
                    else
                    {
                        TagCnt.Add(tag, 1);
                    }
                }
            }
            return TagCnt;
        }

        /// <summary>
        ///     处理用户标签（获得未定义的新标签）
        /// </summary>
        /// <param name="TagList"></param>
        public static List<string> GetCustomTag(string TagList)
        {
            var NewTag = new List<string>();
            if (string.IsNullOrEmpty(TagList)) return NewTag;
            //使用分号切割
            var tags = TagList.Split(";".ToCharArray());
            foreach (var tag in tags)
            {
                //去掉版本（如果有）
                var pureTag = tag.Split(":".ToCharArray())[0];
                //新标签
                NewTag.Add(pureTag);
            }
            return NewTag;
        }

    }
}