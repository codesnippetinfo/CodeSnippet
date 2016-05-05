using BlogSystem.BussinessLogic;
using BlogSystem.DisplayEntity;
using InfraStructure.DataBase;
using InfraStructure.Table;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using System.Collections.Generic;

namespace BlogSystem.Entity
{
    public partial class Stock : OwnerTable
    {
        /// <summary>
        /// 是否收藏
        /// </summary>
        /// <param name="ownerId"></param>
        /// <param name="ArticleId"></param>
        /// <returns></returns>
        public static bool IsStocked(string ownerId, string ArticleId)
        {
            IMongoQuery OwnerIdQuery = Query.EQ(nameof(OwnerId), ownerId);
            IMongoQuery ArticleQuery = Query.EQ(nameof(ArticleID), ArticleId);
            var StockCnt = MongoDbRepository.GetRecordCount<Stock>(Query.And(OwnerIdQuery, ArticleQuery));
            if (StockCnt > 1)
            {
                InfraStructure.Log.ExceptionLog.Log("Stock Check Exception", ownerId, ArticleId, StockCnt.ToString());
            }
            return StockCnt != 0;
        }
        /// <summary>
        /// 文章的收藏数
        /// </summary>
        /// <param name="ArticleId"></param>
        /// <returns></returns>
        public static int GetStockCntByArticleId(string ArticleId)
        {
            return GetStockAccountByArticleId(ArticleId).Count;
        }
        /// <summary>
        /// 文章的收藏者
        /// </summary>
        /// <param name="ArticleId"></param>
        /// <returns></returns>
        public static List<UserInfo> GetStockAccountByArticleId(string ArticleId)
        {
            IMongoQuery ArticleQuery = Query.EQ(nameof(ArticleID), ArticleId);
            var stockList = MongoDbRepository.GetRecList<Stock>(ArticleQuery);
            var accountlist = new List<UserInfo>();
            foreach (Stock stock in stockList)
            {
                accountlist.Add(UserInfo.GetUserInfoBySn(stock.OwnerId));
            }
            return accountlist;
        }
        /// <summary>
        /// 获得某人收藏列表
        /// </summary>
        /// <param name="accountId"></param>
        /// <returns></returns>
        public static List<ArticleItemBody> GetStockByAccount(string accountId)
        {
            IMongoQuery AuthorQuery = Query.EQ(nameof(OwnerId), accountId);
            var StockList = MongoDbRepository.GetRecList<Stock>(AuthorQuery);
            var articleItembody = new List<ArticleItemBody>();
            foreach (var stock in StockList)
            {
                articleItembody.Add(ArticleListManager.GetArticleItemBodyById(stock.ArticleID));
            }
            return articleItembody;
        }

        /// <summary>
        /// 获得某人的总被收藏数
        /// </summary>
        /// <param name="accountId"></param>
        /// <returns></returns>
        public static int GetStockCntByAccount(string accountId)
        {
            IMongoQuery AuthorQuery = Query.EQ(nameof(AuthorID), accountId);
            var StockCnt = MongoDbRepository.GetRecordCount<Stock>(AuthorQuery);
            return StockCnt;
        }

        /// <summary>
        /// 收藏
        /// </summary>
        /// <param name="ownerId"></param>
        /// <param name="ArticleId"></param>
        /// <returns></returns>
        public static bool StockArticle(string ownerId, string ArticleId)
        {
            if (IsStocked(ownerId, ArticleId))
            {
                //已经收藏
                return false;
            }
            Stock stock = new Stock()
            {
                ArticleID = ArticleId,
                AuthorID = Article.GetArticleBySn(ArticleId).OwnerId
            };
            InsertStock(stock, ownerId);
            UserManager.RemoveUserItemBody(ownerId);
            UserManager.RemoveUserBody(ownerId);
            return true;
        }
        /// <summary>
        /// 取消收藏
        /// </summary>
        /// <param name="ownerId"></param>
        /// <param name="articleId"></param>
        /// <returns></returns>
        public static bool RemoveArticle(string ownerId, string articleId)
        {
            if (!IsStocked(ownerId, articleId))
            {
                return false;
            }
            IMongoQuery OwnerIdQuery = Query.EQ(nameof(OwnerId), ownerId);
            IMongoQuery ArticleQuery = Query.EQ(nameof(ArticleID), articleId);
            var stock = MongoDbRepository.GetFirstRec<Stock>(Query.And(OwnerIdQuery, ArticleQuery));
            DropStock(stock);
            return true;
        }

    }
}
