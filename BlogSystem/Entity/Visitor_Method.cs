using InfraStructure.DataBase;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using System.Collections.Generic;
using System.Linq;

namespace BlogSystem.Entity
{
    public partial class Visitor : CacheEntityBase
    {
        /// <summary>
        /// 是否为爬虫
        /// </summary>
        public bool IsSpider
        {
            get
            {
                if (UserAgent == null) return false;
                if (UserAgent.Contains("http://www.baidu.com/")) return true;
                if (UserAgent.Contains("http://www.haosou.com")) return true;
                if (UserAgent.Contains("http://www.google.com/")) return true;
                if (UserAgent.Contains("http://www.majestic12.co.uk/")) return true;
                if (UserAgent.Contains("http://www.sogou.com/")) return true;
                if (UserAgent.Contains("http://www.so.com/")) return true;
                if (UserAgent.Contains("http://www.soso.com/")) return true;
                if (UserAgent.Contains("http://OpenLinkProfiler.org/")) return true;
                if (UserAgent.Contains("360Spider")) return true;
                if (UserAgent.Contains("Baiduspider")) return true;
                if (UserAgent.Contains("Googlebot")) return true;
                if (UserAgent.Contains("Sosospider")) return true;
                if (UserAgent.Contains("sogou spider")) return true;
                return false;
            }
        }
        /// <summary>
        /// 插入爬虫
        /// </summary>
        /// <param name="NewVisitor"></param>
        public static void InsertSpider(Visitor NewVisitor)
        {
            MongoDbRepository.InsertCacheRec("Spider", NewVisitor);
        }
        /// <summary>
        /// 获得访客数
        /// </summary>
        /// <param name="articleId"></param>
        /// <returns></returns>
        public static int GetReadCntByArticleId(string articleId)
        {
            IMongoQuery x1 = Query.EQ(nameof(ArticleID), articleId);
            var visitorList = MongoDbRepository.GetCacheRecList<Visitor>(x1);
            //结果去除各种爬虫的影响
            IEnumerable<Visitor> DistinctList = visitorList
              .GroupBy(visitor => visitor.UserHostAddress)
              .Select(group => group.First());
            return DistinctList.Count();
        }
        /// <summary>
        /// 获得某个账号的观看文章列表
        /// </summary>
        /// <param name="accountId"></param>
        /// <returns></returns>
        public static List<Article> GetArticleByAccountId(string accountId)
        {
            IMongoQuery x1 = Query.EQ(nameof(UserInfoId), accountId);
            var visitorList = MongoDbRepository.GetCacheRecList<Visitor>(x1);
            IEnumerable<Visitor> filteredList = visitorList
              .GroupBy(visitor => visitor.ArticleID)
              .Select(group => group.First());
            var articleList = new List<Article>();
            foreach (var item in filteredList)
            {
                articleList.Add(Article.GetArticleBySn(item.ArticleID));
            }
            return articleList;
        }

    }
}
