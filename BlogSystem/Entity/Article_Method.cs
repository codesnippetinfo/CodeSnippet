using BlogSystem.BussinessLogic;
using InfraStructure.DataBase;
using InfraStructure.Table;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using System;
using System.Collections.Generic;
using System.Configuration;

namespace BlogSystem.Entity
{
    public partial class Article : OwnerTable
    {

        #region Tag
        /// <summary>
        /// 分类列表（和具体技术无关）
        /// </summary>
        public static string[] CatalogItem = new string[]
        {
            "开发经验 - 开发运维中的经验",
            "功能效果 - 某个功能，某个效果的具体实现方法",
            "框架使用 - 前端后端框架的使用",
            "源码分析 - 某个框架，软件，系统的源代码层面的研究",
            "语言特性 - 某种语言的特性介绍",
            "设计模式 - 重构，模式，代码优化",
            "算法技巧 - 各种算法，机器学习，人工智能",
            "学习笔记 - 学习某个领域的基础知识的笔记",
            "理论知识 - 计算机底层知识的普及",
            "疑难杂症 - 对于某个错误异常的深入研究",
            "杂文趣事 - 面试总结，职场人生"
        };

        /// <summary>
        /// 获得指定标签的公开文章
        /// </summary>
        /// <param name="MultiTagString"></param>
        /// <returns></returns>
        public static List<Article> GetArticleListByTag(string MultiTagString)
        {
            return MongoDbRepository.GetRecList<Article>(Query.And(ArticleListManager.PublicArticleQuery, GetTagQuery(MultiTagString)));
        }

        /// <summary>
        /// 将多标签字符串转换为Mongo查询
        /// </summary>
        /// <param name="MultiTagString"></param>
        /// <returns></returns>
        public static IMongoQuery GetTagQuery(string MultiTagString)
        {
            IMongoQuery tagNameQuery = null;
            if (MultiTagString.Contains("|"))
            {
                var tagArray = MultiTagString.Split("|".ToCharArray());
                for (int i = 0; i < tagArray.Length; i++)
                {
                    if (tagNameQuery == null)
                    {
                        tagNameQuery = Query.EQ(nameof(TagName), tagArray[i]);
                    }
                    else
                    {
                        tagNameQuery = Query.And(tagNameQuery, Query.EQ(nameof(TagName), tagArray[i]));
                    }
                }
            }
            else
            {
                tagNameQuery = Query.EQ(nameof(TagName), MultiTagString);
            }
            return tagNameQuery;
        }

        #endregion

        #region Bussinese

        /// <summary>
        /// 接受首页申请
        /// </summary>
        /// <param name="ArticleId"></param>
        public static void Accept(string ArticleId)
        {
            var article = GetArticleBySn(ArticleId);
            article.PublishStatus = ApproveStatus.Accept;
            article.ConfirmDateTime = DateTime.Now;
            article.FirstPageMessage = "审核通过";
            UpdateArticle(article);
            ArticleListManager.RemoveArticleItemBody(article.Sn);
            var md = ArticleContent.GetMarkDown(ArticleId, RevisionType.First);
            md.Revision = RevisionType.Current;
            ArticleContent.InsertArticleContent(md, md.OwnerId);
            string SearchMethod = ConfigurationManager.AppSettings["SearchMethod"];
            if (SearchMethod.Equals("ElasticSearch") && !article.IsPrivate)
            {
                //非私有的进行索引
                SearchManager.Index(article);
            }
        }

        /// <summary>
        /// 拒绝首页申请
        /// </summary>
        /// <param name="ArticleId"></param>
        public static void Reject(string ArticleId, string strMessage)
        {
            var article = GetArticleBySn(ArticleId);
            article.PublishStatus = ApproveStatus.Reject;
            article.FirstPageMessage = strMessage;
            article.ConfirmDateTime = DateTime.Now;
            UpdateArticle(article);
            ArticleListManager.RemoveArticleItemBody(article.Sn);
        }

        /// <summary>
        /// 获得某人的所有的文章
        /// </summary>
        /// <param name="ownerId"></param>
        /// <returns></returns>
        public static List<Article> GetListByOwnerId(string ownerId, bool IsOnlyFirstPage = true)
        {
            if (IsOnlyFirstPage)
            {
                return OwnerTableOperator.GetRecListByOwnerId<Article>(CollectionName, ownerId, ArticleListManager.FirstPageArticleQuery);
            }
            else
            {
                return OwnerTableOperator.GetRecListByOwnerId<Article>(CollectionName, ownerId);
            }
        }

        /// <summary>
        /// 获得指定文集的所有文章
        /// </summary>
        /// <param name="collectionId"></param>
        /// <param name="IsOnlyPublished"></param>
        /// <returns></returns>
        public static List<Article> GetArticleByColIdAndPublish(string collectionId, bool IsOnlyPublished)
        {
            IMongoQuery CollectionQuery = Query.EQ(nameof(CollectionID), collectionId); ;
            if (IsOnlyPublished)
            {
                CollectionQuery = Query.And(CollectionQuery, Query.EQ(nameof(Article.PublishStatus), ApproveStatus.Accept));
            }
            return MongoDbRepository.GetRecList<Article>(CollectionQuery);
        }

        /// <summary>
        /// 获得指定文集的所有文章
        /// </summary>
        /// <param name="collectionId"></param>
        /// <param name="IsOnlyFirstPage"></param>
        /// <returns></returns>
        public static List<Article> GetArticleByColIdAndFirstPage(string collectionId, bool IsOnlyFirstPage)
        {
            IMongoQuery CollectionQuery = Query.EQ(nameof(CollectionID), collectionId); ;
            if (IsOnlyFirstPage)
            {
                CollectionQuery = Query.And(ArticleListManager.FirstPageArticleQuery, CollectionQuery);
            }
            return MongoDbRepository.GetRecList<Article>(CollectionQuery);
        }

        /// <summary>
        /// 获得待审核文章列表
        /// </summary>
        /// <returns></returns>
        public static List<Article> GetPendingArticleList()
        {
            //只有申请首页，非文集才会有Pending状态
            IMongoQuery PendingQuery = Query.EQ(nameof(PublishStatus), ApproveStatus.Pending);
            return MongoDbRepository.GetRecList<Article>(PendingQuery);
        }

        /// <summary>
        /// 获得指定小时内首页发布文章的列表
        /// </summary>
        /// <returns></returns>
        public static List<Article> GetHoursFirstPageArticleList(int Hours = 48)
        {
            IMongoQuery ConfirmDateTimeQuery = Query.GT(nameof(ConfirmDateTime), DateTime.Now.AddHours(-Hours));
            var sortArgs = new Sort.SortArg[1];
            sortArgs[0] = new Sort.SortArg
            {
                FieldName = nameof(ConfirmDateTime),
                SortType = Sort.SortType.Descending,
                SortOrder = 1
            };
            Action<MongoCursor> setCursor = x => { x.SetSortOrder(Sort.GetSortBuilder(sortArgs)); };
            return MongoDbRepository.GetRecList<Article>(Query.And(ArticleListManager.FirstPageArticleQuery, ConfirmDateTimeQuery), setCursor);
        }

        /// <summary>
        /// 获得当前文章同一个Collection中的下一篇
        /// </summary>
        /// <param name="IsNext"></param>
        /// <returns></returns>
        public Article GetArticleInCollection(bool IsNext)
        {
            //寻找同一个Collection
            //Code大（小）于当前Code的记录
            //已经审核发布的
            //是否发布首页标志相同的
            //按照升（降）序排序，取第一条,
            IMongoQuery CollectionIDQuery = Query.EQ(nameof(CollectionID), CollectionID);
            IMongoQuery CodeQuery = IsNext ? Query.GT(nameof(Code), Code) : Query.LT(nameof(Code), Code);
            Sort.SortArg[] sortArgs = new Sort.SortArg[1];
            sortArgs[0] = new Sort.SortArg()
            {
                FieldName = nameof(Code),
                SortType = IsNext ? Sort.SortType.Ascending : Sort.SortType.Descending,
                SortOrder = 1
            };
            Action<MongoCursor> setCursor = x => { x.SetSortOrder(Sort.GetSortBuilder(sortArgs)); };
            //这里认为可以是所有公开的文章，通过首页文章，查看同文集的公开文章
            var Articlelist = MongoDbRepository.GetRecList<Article>(Query.And(CollectionIDQuery, CodeQuery, ArticleListManager.PublicArticleQuery), setCursor);
            if (Articlelist.Count > 0)
            {
                return Articlelist[0];
            }
            else
            {
                return null;
            }
        }
        #endregion

        /// <summary>
        /// 发布
        /// </summary>
        /// <param name="ArticleId"></param>
        public static void Publish(string ArticleId, string MarkDownContent, string HTMLContent)
        {
            Article article = GetArticleBySn(ArticleId);
            if (article.IsFirstPage)
            {
                //首页的处理
                article.PublishDateTime = DateTime.Now;
                switch (article.PublishStatus)
                {
                    case ApproveStatus.None:
                        //未发布
                        article.PublishStatus = ApproveStatus.Pending;
                        UpdateArticle(article);
                        ArticleListManager.RemoveArticleItemBody(article.Sn);
                        ArticleContent.SaveMarkDownVersion(ArticleId, MarkDownContent, article.OwnerId, RevisionType.First);
                        switch (UserInfo.GetUserInfoBySn(article.OwnerId).Privilege)
                        {
                            case UserType.Admin:
                            case UserType.Editor:
                            case UserType.Author:
                                //管理员，编辑，特约作者直接获得批准
                                Accept(ArticleId);
                                break;
                            default:
                                break;
                        }
                        break;
                    case ApproveStatus.Pending:
                        //第一次发布，还没有获得批准的
                        ArticleContent.SaveMarkDownVersion(ArticleId, MarkDownContent, article.OwnerId, RevisionType.First);
                        break;
                    case ApproveStatus.Accept:
                        //已经发布的
                        ArticleContent.SaveMarkDownVersion(ArticleId, MarkDownContent, article.OwnerId, RevisionType.Current);
                        break;
                    case ApproveStatus.Reject:
                        //拒绝的，变成待审核状态
                        article.PublishStatus = ApproveStatus.Pending;
                        UpdateArticle(article);
                        ArticleListManager.RemoveArticleItemBody(article.Sn);
                        ArticleContent.SaveMarkDownVersion(ArticleId, MarkDownContent, article.OwnerId, RevisionType.First);
                        break;
                    default:
                        break;
                }
            }
        }

        /// <summary>
        /// 检索
        /// </summary>
        /// <param name="KeyWord"></param>
        /// <returns></returns>
        public static List<Article> Search(string KeyWord)
        {
            var Articles = new List<Article>();
            var searchResult = MongoDbRepository.SearchText(CollectionName, KeyWord, false, 50, ArticleListManager.PublicArticleQuery);
            foreach (var item in searchResult)
            {
                Articles.Add(BsonSerializer.Deserialize<Article>(item));
            }
            return Articles;
        }

        #region 统计

        /// <summary>
        /// 评论数
        /// </summary>
        /// <returns></returns>
        internal int GetCommentCnt()
        {
            return Comment.GetCommentCntByArticleId(Sn);
        }

        /// <summary>
        /// 评论人数
        /// </summary>
        /// <returns></returns>
        internal int GetCommentAccountCnt()
        {
            return Comment.GetCommentCntByArticleIdDistinct(Sn);
        }

        /// <summary>
        /// 阅读人数
        /// </summary>
        /// <returns></returns>
        internal int GetReadCnt()
        {
            return Visitor.GetReadCntByArticleId(Sn);
        }

        /// <summary>
        /// 收藏人数
        /// </summary>
        /// <returns></returns>
        internal int GetStockCnt()
        {
            return Stock.GetStockCntByArticleId(Sn);
        }
        /// <summary>
        /// 文章综合评分
        /// </summary>
        /// <param name="ReadCnt">阅读数</param>
        /// <param name="CommentCnt">评论数</param>
        /// <param name="CommentAccountCnt">评论人数</param>
        /// <param name="StockCnt">收藏数</param>
        /// <returns></returns>
        public int EvaluateScore(int ReadCnt, int CommentCnt, int CommentAccountCnt, int StockCnt)
        {
            return (ReadCnt / 20) +
                   (CommentCnt * 1) +
                   (CommentAccountCnt * 2) +
                   (StockCnt * 5);
        }
        #endregion

    }
}
