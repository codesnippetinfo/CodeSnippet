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
    }
}