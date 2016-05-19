using BlogSystem.DisplayEntity;
using BlogSystem.Entity;
using InfraStructure.DataBase;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BlogSystem.BussinessLogic
{
    /// <summary>
    /// 获得各种文章列表
    /// </summary>
    public static class ArticleListManager
    {
        /// <summary>
        /// 置顶文章id
        /// </summary>
        private static string topArticleId = string.Empty;
        /// <summary>
        ///  获得每日推荐文章
        /// </summary>
        public static ArticleItemBody GetTopArticle()
        {
            if (string.IsNullOrEmpty(topArticleId)) return null;
            return GetArticleItemBodyById(topArticleId);
        }
        /// <summary>
        /// 设定每日推荐文章
        /// </summary>
        public static void SetTopArticle(string ArticleId)
        {
            topArticleId = ArticleId;
        }
        /// <summary>
        /// 查询器
        /// </summary>
        public struct ArticleQueryFilter
        {
            public bool? isFirstPage;
            public bool? isPrivate;
            public ApproveStatus[] statuslist;
            public string ownid;
            public string collectionid;

            //过滤
            public ArticleLevel[] Levelist;
            public string[] Cataloglist;
            //包含标签
            public string[] ContainTag;
            //过滤标签
            public string[] AntiTag;
        }
        /// <summary>
        /// 获得文章列表查询条件
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        public static IMongoQuery GetQueryByArticelFilter(ArticleQueryFilter filter)
        {
            IMongoQuery basicQuery = null;
            if (filter.isFirstPage != null)
            {
                if (basicQuery != null)
                {
                    basicQuery = Query.And(basicQuery, Query.EQ(nameof(Article.IsFirstPage), filter.isFirstPage));
                }
                else
                {
                    basicQuery = Query.EQ(nameof(Article.IsFirstPage), filter.isFirstPage);
                }
            }
            if (filter.isPrivate != null)
            {
                if (basicQuery != null)
                {
                    basicQuery = Query.And(basicQuery, Query.EQ(nameof(Article.IsPrivate), filter.isPrivate));
                }
                else
                {
                    basicQuery = Query.EQ(nameof(Article.IsPrivate), filter.isPrivate);
                }
            }
            if (filter.statuslist != null)
            {
                IMongoQuery statusQuery = null;
                foreach (var status in filter.statuslist)
                {
                    if (statusQuery != null)
                    {
                        statusQuery = Query.Or(statusQuery, Query.EQ(nameof(Article.PublishStatus), status));
                    }
                    else
                    {
                        statusQuery = Query.EQ(nameof(Article.PublishStatus), status);
                    }
                }
                if (basicQuery != null)
                {
                    basicQuery = Query.And(basicQuery, statusQuery);
                }
                else
                {
                    basicQuery = statusQuery;
                }
            }
            //Owner
            if (!string.IsNullOrEmpty(filter.ownid))
            {
                if (basicQuery != null)
                {
                    basicQuery = Query.And(basicQuery, Query.EQ(nameof(Article.OwnerId), filter.ownid));
                }
                else
                {
                    basicQuery = Query.EQ(nameof(Article.OwnerId), filter.ownid);
                }
            }
            if (!string.IsNullOrEmpty(filter.collectionid))
            {
                if (basicQuery != null)
                {
                    basicQuery = Query.And(basicQuery, Query.EQ(nameof(Article.CollectionID), filter.collectionid));
                }
                else
                {
                    basicQuery = Query.EQ(nameof(Article.CollectionID), filter.collectionid);
                }
            }


            IMongoQuery userQuery = null;
            //难度
            if (filter.Levelist != null)
            {
                IMongoQuery levelQuery = null;
                foreach (var level in filter.Levelist)
                {
                    if (levelQuery != null)
                    {
                        levelQuery = Query.Or(levelQuery, Query.EQ(nameof(Article.Level), level));
                    }
                    else
                    {
                        levelQuery = Query.EQ(nameof(Article.Level), level);
                    }
                }
                if (userQuery != null)
                {
                    userQuery = Query.And(userQuery, levelQuery);
                }
                else
                {
                    userQuery = levelQuery;
                }
            }
            //分类
            if (filter.Cataloglist != null)
            {
                IMongoQuery CatalogQuery = null;
                foreach (var catalog in filter.Cataloglist)
                {
                    if (CatalogQuery != null)
                    {
                        CatalogQuery = Query.Or(CatalogQuery, Query.EQ(nameof(Article.Catalog), catalog));
                    }
                    else
                    {
                        CatalogQuery = Query.EQ(nameof(Article.Catalog), catalog);
                    }
                }
                if (userQuery != null)
                {
                    userQuery = Query.And(userQuery, CatalogQuery);
                }
                else
                {
                    userQuery = CatalogQuery;
                }
            }

            //不包含，使用AND连接
            if (filter.AntiTag != null)
            {
                IMongoQuery AntiTagQuery = null;
                foreach (var tag in filter.AntiTag)
                {
                    if (AntiTagQuery != null)
                    {
                        //过滤标签用AND
                        AntiTagQuery = Query.And(AntiTagQuery, Query.NE(nameof(Article.TagName), tag));
                    }
                    else
                    {
                        AntiTagQuery = Query.NE(nameof(Article.TagName), tag);
                    }
                }
                if (userQuery != null)
                {
                    userQuery = Query.And(userQuery, AntiTagQuery);
                }
                else
                {
                    userQuery = AntiTagQuery;
                }
            }


            //注意：这个条件必须写在最后
            //注意：这个条件必须写在最后
            //注意：这个条件必须写在最后
            //包含，使用OR连接
            if (filter.ContainTag != null)
            {
                IMongoQuery ContainTagQuery = null;
                foreach (var tag in filter.ContainTag)
                {
                    if (ContainTagQuery != null)
                    {
                        //过滤标签用AND
                        ContainTagQuery = Query.Or(ContainTagQuery, Query.EQ(nameof(Article.TagName), tag));
                    }
                    else
                    {
                        ContainTagQuery = Query.EQ(nameof(Article.TagName), tag);
                    }
                }
                if (userQuery != null)
                {
                    userQuery = Query.Or(userQuery, ContainTagQuery);
                }
                else
                {
                    userQuery = ContainTagQuery;
                }
            }

            return userQuery == null ? basicQuery : Query.And(basicQuery, userQuery);
        }

        /// <summary>
        /// 首页文章查询
        /// </summary>
        public static IMongoQuery FirstPageArticleQuery
        {
            get
            {
                return GetQueryByArticelFilter(FirstPageArticleQueryFileter);
            }
        }
        /// <summary>
        /// 首页文章查询
        /// </summary>
        public static ArticleQueryFilter FirstPageArticleQueryFileter
        {
            get
            {
                ArticleQueryFilter aqb = new ArticleQueryFilter()
                {
                    isFirstPage = true,
                    statuslist = new ApproveStatus[] { ApproveStatus.Accept }
                };
                return aqb;
            }
        }

        /// <summary>
        /// 所有公开的文章
        /// </summary>
        public static IMongoQuery PublicArticleQuery
        {
            get
            {
                return GetQueryByArticelFilter(PublicArticleQueryFilter);
            }
        }
        /// <summary>
        /// 所有公开的文章
        /// </summary>
        public static ArticleQueryFilter PublicArticleQueryFilter
        {
            get
            {
                ArticleQueryFilter aqb = new ArticleQueryFilter()
                {
                    isPrivate = false,
                    statuslist = new ApproveStatus[] { ApproveStatus.Accept, ApproveStatus.NotNeed }
                };
                return aqb;
            }
        }

        /// <summary>
        /// 获得所有文章的数量
        /// </summary>
        /// <returns></returns>
        public static int GetArticleCnt(ArticleQueryFilter? filter)
        {
            if (filter != null)
            {
                return MongoDbRepository.GetRecordCount<Article>(GetQueryByArticelFilter(filter.Value));
            }
            else
            {
                return MongoDbRepository.GetRecordCount<Article>(null);
            }
        }
        /// <summary>
        /// 获得文章
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        public static List<Article> GetArticles(ArticleQueryFilter? filter)
        {
            if (filter != null)
            {
                return MongoDbRepository.GetRecList<Article>(GetQueryByArticelFilter(filter.Value));
            }
            else
            {
                return MongoDbRepository.GetRecList<Article>();
            }
        }

        /// <summary>
        /// 删除缓存
        /// </summary>
        /// <param name="accountId"></param>
        public static void RemoveArticleItemBody(string articleId)
        {
            IMongoQuery x = Query.EQ(nameof(ArticleItemBody.ArticleInfo) + "." + MongoDbRepository.MongoKeyField, articleId);
            var cache = MongoDbRepository.GetFirstCacheRec<ArticleItemBody>(x);
            if (cache != null)
            {
                MongoDbRepository.DeleteRecPhysical<ArticleItemBody>(cache);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static List<string> GetListForArticleId()
        {
            var firstpage = MongoDbRepository.GetRecList<Article>(FirstPageArticleQuery);
            return firstpage.Select(x => x.Sn).ToList();
        }

        /// <summary>
        /// 获得首页表示的文章列表
        /// 按照确认时间排序
        /// </summary>
        /// <param name="p">分页器</param>
        /// <param name="isFirstPage">仅首页</param>
        /// <param name="AddtionalCondition">附加条件</param>
        /// <returns></returns>
        public static List<ArticleItemBody> GetPublicListForArticleByPage(Pages p, ArticleQueryFilter filter)
        {
            //注意这里必须要控制取出的数量，当然现在可以不考虑优化问题
            //所有文章中首页审核通过的，限制数量4000篇（20 * 20）
            //取出后进行缓存
            var sortArgs = new Sort.SortArg[1];
            sortArgs[0] = new Sort.SortArg
            {
                FieldName = nameof(Article.ConfirmDateTime),
                SortType = Sort.SortType.Descending,
                SortOrder = 1
            };
            //修改后的文章再发布，则发布日期是最新的，这里使用首页审核日期作为排序日期。
            Action<MongoCursor> setCursor = x => { x.SetSkip(p.SkipCount()).SetLimit(p.PageItemCount).SetSortOrder(Sort.GetSortBuilder(sortArgs)); };
            var firstpage = new List<Article>();
            firstpage = MongoDbRepository.GetRecList<Article>(GetQueryByArticelFilter(filter), setCursor);
            var titlelist = new List<ArticleItemBody>();
            foreach (var item in firstpage)
            {
                titlelist.Add(GetArticleItemBodyById(item.Sn));
            }
            return titlelist;
        }
        /// <summary>
        /// 获得专题的列表
        /// </summary>
        /// <param name="accountid"></param>
        /// <returns></returns>
        public static List<ArticleItemBody> GetTopicArticleList(string accountid, bool IsForPublic)
        {
            var topic = Topic.GetTopicByAccountId(accountid);
            if (topic == null) return new List<ArticleItemBody>();
            var topicId = topic.Sn;
            IMongoQuery topicQuery = Query.EQ(nameof(TopicArticle.TopicID), topicId);
            IMongoQuery statusQuery = Query.Or(Query.EQ(nameof(TopicArticle.PublishStatus), ApproveStatus.Accept), Query.EQ(nameof(TopicArticle.PublishStatus), ApproveStatus.NotNeed));
            var articleIdList = MongoDbRepository.GetRecList<TopicArticle>(IsForPublic ? Query.And(topicQuery, statusQuery) : topicQuery);
            var articleitembodyList = new List<ArticleItemBody>();
            foreach (var item in articleIdList)
            {
                var x = GetArticleItemBodyById(item.ArticleID);
                x.Score = item.PublishStatus.GetHashCode();
                articleitembodyList.Add(x);
            }
            return articleitembodyList;
        }

        /// <summary>
        /// 文章列表转文章体列表
        /// </summary>
        /// <param name="articleList"></param>
        /// <returns></returns>
        public static List<ArticleItemBody> GetArticleItemBodyListByArticleList(List<Article> articleList)
        {
            var articlebodys = new List<ArticleItemBody>();
            foreach (var item in articleList)
            {
                articlebodys.Add(GetArticleItemBodyById(item.Sn));
            }
            return articlebodys;
        }
        /// <summary>
        /// 文章列表转文章体列表
        /// </summary>
        /// <param name="articleList"></param>
        /// <returns></returns>
        public static List<ArticleBody> GetArticleBodyListByArticleList(List<Article> articleList)
        {
            var articlebodys = new List<ArticleBody>();
            foreach (var item in articleList)
            {
                articlebodys.Add(GetArticleBodyById(item.Sn));
            }
            return articlebodys;
        }

        /// <summary>
        /// 根据文章序列号获得表示用文章信息组合
        /// </summary>
        /// <returns></returns>
        public static ArticleBody GetArticleBodyById(string ArticleId)
        {
            IMongoQuery x = Query.EQ(nameof(ArticleBody.ArticleInfo) + "." + MongoDbRepository.MongoKeyField, ArticleId);
            var cache = MongoDbRepository.GetFirstCacheRec<ArticleBody>(x);
            if (cache != null)
            {
                return cache;
            }
            var articlebody = new ArticleBody();
            Article Article = Article.GetArticleBySn(ArticleId);
            UserInfo account = UserInfo.GetUserInfoBySn(Article.OwnerId);
            articlebody.ArticleInfo = Article;
            articlebody.AuthorInfo = account;
            //作者
            articlebody.FocusCnt = Focus.GetFoucsCnt(account.Sn);
            articlebody.FollowCnt = Focus.GetFollowCnt(account.Sn);
            articlebody.AuthorStockCnt = Stock.GetStockCntByAccount(account.Sn);
            //文章
            articlebody.CollectionTitle = Collection.GetCollectionBySn(Article.CollectionID).Title;
            articlebody.CommentCnt = Article.GetCommentCnt();
            articlebody.CommentAccountCnt = Article.GetCommentAccountCnt();
            articlebody.ReadCnt = Article.GetReadCnt();
            //关联文章
            articlebody.NextArticleInCollection = Article.GetArticleInCollection(true);
            articlebody.PreviousArticleInCollection = Article.GetArticleInCollection(false);
            //收藏者列表
            articlebody.StockAccountList = Stock.GetStockAccountByArticleId(ArticleId);
            articlebody.StockCnt = articlebody.StockAccountList.Count;
            //文章综合评分
            if (articlebody.ArticleInfo.IsPrivate)
            {
                articlebody.Score = 0;
            }
            else
            {
                articlebody.Score = Article.EvaluateScore(articlebody.ReadCnt, articlebody.CommentCnt, articlebody.CommentAccountCnt, articlebody.StockCnt);
            }
            articlebody.MarkDownAnlyze = MarkDownAnlyzer.Anlyze(ArticleContent.GetMarkDownString(ArticleId, RevisionType.Current));
            ArticleBody.InsertArticleBody(articlebody);
            return articlebody;
        }


        /// <summary>
        /// 根据文章序列号获得表示用(列表)文章信息组合
        /// </summary>
        /// <returns></returns>
        public static ArticleItemBody GetArticleItemBodyById(string ArticleId)
        {
            IMongoQuery x = Query.EQ(nameof(ArticleItemBody.ArticleInfo) + "." + MongoDbRepository.MongoKeyField, ArticleId);
            var cache = MongoDbRepository.GetFirstCacheRec<ArticleItemBody>(x);
            if (cache != null)
            {
                return cache;
            }
            var articleitembody = new ArticleItemBody();
            Article article = Article.GetArticleBySn(ArticleId);
            UserInfo account = UserInfo.GetUserInfoBySn(article.OwnerId);
            articleitembody.ArticleInfo = article;
            articleitembody.AuthorInfo = account;
            articleitembody.CommentCnt = article.GetCommentCnt();
            articleitembody.CommentAccountCnt = article.GetCommentAccountCnt();
            articleitembody.ReadCnt = article.GetReadCnt();
            articleitembody.StockCnt = article.GetStockCnt();
            //文章综合评分
            if (articleitembody.ArticleInfo.IsPrivate)
            {
                articleitembody.Score = 0;
            }
            else
            {
                articleitembody.Score = article.EvaluateScore(articleitembody.ReadCnt, articleitembody.CommentCnt, articleitembody.CommentAccountCnt, articleitembody.StockCnt);
            }
            ArticleItemBody.InsertArticleItemBody(articleitembody);
            return articleitembody;
        }

        /// <summary>
        /// 获得评论列表
        /// </summary>
        /// <param name="ArticleId"></param>
        /// <returns></returns>
        public static List<CommentBody> GetCommentBodyList(string ArticleId)
        {
            var commentBodyList = new List<CommentBody>();
            var commentlist = Comment.GetCommentListByArticleId(ArticleId);
            foreach (var comment in commentlist)
            {
                var commentbody = new CommentBody();
                commentbody.CommentInfo = comment;
                commentbody.AuthorInfo = UserInfo.GetUserInfoBySn(comment.OwnerId);
                commentbody.HasSubComment = (string.IsNullOrEmpty(comment.ReplyCommentID) && Comment.HasSubComment(comment.Sn));
                commentBodyList.Add(commentbody);
            }
            return commentBodyList;
        }

    }
}
