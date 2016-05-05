using InfraStructure.DataBase;
using MongoDB.Driver;
using MongoDB.Driver.Builders;

namespace BlogSystem.Entity
{
    /// <summary>
    /// 专题文章
    /// </summary>
    public partial class TopicArticle : EntityBase
    {
        /// <summary>
        /// 专题是否收录某文章
        /// </summary>
        /// <param name="topicId"></param>
        /// <param name="articleId"></param>
        /// <returns></returns>
        public static bool IsExist(string topicId, string articleId)
        {
            IMongoQuery TopicQuery = Query.EQ(nameof(TopicID), topicId);
            IMongoQuery ArticleQuery = Query.EQ(nameof(ArticleID), articleId);
            return MongoDbRepository.GetRecordCount<TopicArticle>(Query.And(ArticleQuery, TopicQuery)) > 0;
        }
        /// <summary>
        /// 收录到主题
        /// </summary>
        /// <param name="topicId"></param>
        /// <param name="articleId"></param>
        /// <returns></returns>
        public static bool PutToTopic(string topicOwnerid, string articleId)
        {
            IMongoQuery OwnerQuery = Query.EQ(nameof(Topic.OwnerId), topicOwnerid);
            var topic = MongoDbRepository.GetFirstRec<Topic>(OwnerQuery);
            if (topic == null) return false;
            if (IsExist(topic.Sn, articleId)) return false;
            //是否能够被收录
            var article = Article.GetArticleBySn(articleId);
            if (!article.IsTopicable) return false;
            if (article.IsNeedTopicApproval)
            {
                var topicarticle = new TopicArticle()
                {
                    ArticleID = articleId,
                    TopicID = topic.Sn,
                    PublishStatus = ApproveStatus.Pending
                };
                InsertTopicArticle(topicarticle);
                var user = UserInfo.GetUserInfoBySn(topicOwnerid);
                var parm = "TopicOwnerId=" + topicOwnerid + "&ArticleId=" + articleId;
                var articleurl = "<a href = '/Article/Index?ArticleId=" + article.Sn + "'>" + article.Title + "</a>";
                var topicurl = "<a href = '/Author/TopicPage?accountid=" + topicOwnerid + "'>" + topic.Title + "</a>";
                SiteMessage.CreateYesNo(article.OwnerId, "[" + user.NickName + "]请求收录您的文章：[" + articleurl + "]到他的专题" + topicurl, "/Author/AcceptTopic?" + parm, "/Author/RefuseTopic?" + parm, topicOwnerid);
            }
            else
            {
                var topicarticle = new TopicArticle()
                {
                    ArticleID = articleId,
                    TopicID = topic.Sn,
                    PublishStatus = ApproveStatus.Accept
                };
                InsertTopicArticle(topicarticle);
            }
            return true;
        }
        /// <summary>
        /// 收录到主题
        /// </summary>
        /// <param name="topicId"></param>
        /// <param name="articleId"></param>
        /// <returns></returns>
        public static bool ChangeTopicStatus(string topicOwnerid, string articleId, bool Accept)
        {
            IMongoQuery OwnerQuery = Query.EQ(nameof(Topic.OwnerId), topicOwnerid);
            var topic = MongoDbRepository.GetFirstRec<Topic>(OwnerQuery);
            if (topic == null) return false;
            //改变状态，这里是非
            if (!IsExist(topic.Sn, articleId)) return false;
            //是否能够被收录
            var article = Article.GetArticleBySn(articleId);
            if (!article.IsTopicable) return false;
            IMongoQuery topicQuery = Query.EQ(nameof(TopicID), topic.Sn);
            IMongoQuery articleQuery = Query.EQ(nameof(ArticleID), articleId);
            var topicarticle = MongoDbRepository.GetFirstRec<TopicArticle>(Query.And(topicQuery, articleQuery));
            if (topicarticle == null) return false;
            topicarticle.PublishStatus = Accept ? ApproveStatus.Accept : ApproveStatus.Reject;
            UpdateTopicArticle(topicarticle);
            return true;
        }
        /// <summary>
        /// 删除专题文章
        /// </summary>
        /// <param name="topicid"></param>
        /// <param name="articleId"></param>
        public static void Remove(string topicid, string articleId)
        {
            IMongoQuery topicQuery = Query.EQ(nameof(TopicID), topicid);
            IMongoQuery articleQuery = Query.EQ(nameof(ArticleID), articleId);
            var topicarticle = MongoDbRepository.GetFirstRec<TopicArticle>(Query.And(topicQuery, articleQuery));
            if (topicarticle != null)
            {
                topicarticle.PublishStatus = ApproveStatus.None;
            }
            UpdateTopicArticle(topicarticle);
        }
        /// <summary>
        /// 删除专题文章
        /// </summary>
        /// <param name="articleId"></param>
        public static void RemoveArticle(string articleId)
        {
            IMongoQuery articleQuery = Query.EQ(nameof(ArticleID), articleId);
            var topicArticleList = MongoDbRepository.GetRecList<TopicArticle>(articleQuery);
            foreach (var topicarticle in topicArticleList)
            {
                topicarticle.PublishStatus = ApproveStatus.None;
                UpdateTopicArticle(topicarticle);
            }
        }
    }
}
