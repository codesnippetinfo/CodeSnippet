using InfraStructure.DataBase;
using InfraStructure.Table;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using System.Collections.Generic;
using System.Linq;

namespace BlogSystem.Entity
{
    /// <summary>
    /// 评论
    /// </summary>
    public partial class Comment : OwnerTable
    {
        /// <summary>
        /// 获得评论数
        /// </summary>
        /// <param name="ArticleId"></param>
        /// <returns></returns>
        public static int GetCommentCntByArticleId(string ArticleId)
        {
            IMongoQuery AccountQuery = Query.EQ(nameof(ArticleID), ArticleId);
            return MongoDbRepository.GetRecordCount<Comment>(AccountQuery);
        }
        /// <summary>
        /// 获得评论数（同用户不累加）
        /// </summary>
        /// <param name="ArticleId"></param>
        /// <returns></returns>
        public static int GetCommentCntByArticleIdDistinct(string ArticleId)
        {
            IMongoQuery AccountQuery = Query.EQ(nameof(ArticleID), ArticleId);
            var commentlist = MongoDbRepository.GetRecList<Comment>(AccountQuery);
            IEnumerable<Comment> filteredList = commentlist
              .GroupBy(comment => comment.OwnerId)
              .Select(group => group.First());
            return filteredList.Count();
        }
        /// <summary>
        /// 获得评论
        /// </summary>
        /// <param name="ArticleId"></param>
        /// <returns></returns>
        public static List<Comment> GetCommentListByArticleId(string ArticleId)
        {
            IMongoQuery AccountQuery = Query.EQ(nameof(ArticleID), ArticleId);
            var commentlist = MongoDbRepository.GetRecList<Comment>(AccountQuery);
            commentlist.Sort((x, y) => { return x.CreateDateTime.CompareTo(y.CreateDateTime); });
            return commentlist;
        }
        /// <summary>
        /// 某个评论是否有子评论
        /// </summary>
        /// <param name="MainCommentId"></param>
        /// <returns></returns>
        public static bool HasSubComment(string MainCommentId)
        {
            IMongoQuery ReplyCommentQuery = Query.EQ(nameof(ReplyCommentID), MainCommentId);
            return MongoDbRepository.GetRecordCount<Comment>(ReplyCommentQuery) > 0;
        }
    }
}
