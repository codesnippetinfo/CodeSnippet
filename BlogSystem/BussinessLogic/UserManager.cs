using BlogSystem.DisplayEntity;
using BlogSystem.Entity;
using InfraStructure.DataBase;
using InfraStructure.Table;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using System.Collections.Generic;

namespace BlogSystem.BussinessLogic
{
    public static class UserManager
    {

        public static UserItemBody GetUserItemBody(string accountId)
        {
            IMongoQuery x = Query.EQ(nameof(UserItemBody.UserInfo) + "." + MongoDbRepository.MongoKeyField, accountId);
            var cache = MongoDbRepository.GetFirstCacheRec<UserItemBody>(x);
            if (cache != null)
            {
                return cache;
            }
            var u = new UserItemBody();
            u.UserInfo = UserInfo.GetUserInfoBySn(accountId);
            u.ArticleCnt = Article.GetListByOwnerId(accountId).Count;
            u.StockCnt = Stock.GetStockCntByAccount(accountId);
            u.FocusCnt = Focus.GetFoucsCnt(accountId);
            u.FollwersCnt = Focus.GetFollowCnt(accountId);
            UserItemBody.InsertUserItemBody(u);
            return u;
        }
        /// <summary>
        /// 删除缓存
        /// </summary>
        /// <param name="accountId"></param>
        public static void RemoveUserItemBody(string accountId)
        {
            IMongoQuery x = Query.EQ(nameof(UserItemBody.UserInfo) + "." + MongoDbRepository.MongoKeyField, accountId);
            var cache = MongoDbRepository.GetFirstCacheRec<UserItemBody>(x);
            if (cache != null)
            {
                MongoDbRepository.DeleteRecPhysical<UserItemBody>(cache);
            }
        }
        /// <summary>
        /// 删除缓存
        /// </summary>
        /// <param name="accountId"></param>
        public static void RemoveUserBody(string accountId)
        {
            IMongoQuery x = Query.EQ(nameof(UserBody.UserInfo) + "." + MongoDbRepository.MongoKeyField, accountId);
            var cache = MongoDbRepository.GetFirstCacheRec<UserBody>(x);
            if (cache != null)
            {
                MongoDbRepository.DeleteRecPhysical<UserBody>(cache);
            }
        }

        /// <summary>
        /// 获得用户BODY
        /// </summary>
        /// <param name="accountId"></param>
        /// <returns></returns>
        public static UserBody GetUserBody(string accountId)
        {
            IMongoQuery x = Query.EQ(nameof(UserBody.AccountId), accountId);
            var cache = MongoDbRepository.GetFirstCacheRec<UserBody>(x);
            if (cache != null)
            {
                return cache;
            }
            UserBody u = new UserBody();
            u.AccountId = accountId;
            u.UserInfo = UserInfo.GetUserInfoBySn(accountId);

            switch (u.UserInfo.RegisterMethod)
            {
                case GithubAccount.Github:
                    u.GitInfo = GithubAccount.GetGithubAccountBySn(u.UserInfo.RegisterAccountID);
                    break;
                case QQAccount.QQ:
                    u.QQInfo = QQAccount.GetQQAccountBySn(u.UserInfo.RegisterAccountID);
                    break;
                default:
                    break;
            }

            //获得用户文章列表
            u.ArticleList = new List<ArticleItemBody>();
            var alist = Article.GetListByOwnerId(accountId);
            foreach (var item in alist)
            {
                u.ArticleList.Add(ArticleListManager.GetArticleItemBodyById(item.Sn));
            }
            //关注的人
            u.FocusList = new List<UserInfo>();
            var focuslist = Focus.GetFocus(accountId);
            foreach (var item in focuslist)
            {
                u.FocusList.Add(UserInfo.GetUserInfoBySn(item));
            }
            //跟随的人
            u.FollowList = new List<UserInfo>();
            var followlist = Focus.GetFollow(accountId);
            foreach (var item in followlist)
            {
                u.FollowList.Add(UserInfo.GetUserInfoBySn(item));
            }
            //收藏
            u.StockList = new List<ArticleItemBody>();
            var slist = OwnerTableOperator.GetRecListByOwnerId<Stock>(accountId);
            foreach (var item in slist)
            {
                u.StockList.Add(ArticleListManager.GetArticleItemBodyById(item.ArticleID));
            }
            UserBody.InsertUserBody(u);
            return u;
        }
    }
}
