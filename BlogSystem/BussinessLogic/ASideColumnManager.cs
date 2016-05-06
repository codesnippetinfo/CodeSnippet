using BlogSystem.DisplayEntity;
using BlogSystem.Entity;
using BlogSystem.TagSystem;
using InfraStructure.DataBase;
using InfraStructure.Misc;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BlogSystem.BussinessLogic
{
    public static class ASideColumnManager
    {
        /// <summary>
        /// 获得Rank
        /// </summary>
        /// <param name="rankContain"></param>
        /// <param name="ItemCount"></param>
        /// <param name="TransformDisplay"></param>
        /// <returns></returns>
        private static List<GenericItem> GetGenericItemListByRankContain(RankContain rankContain, int ItemCount, Func<string, string> TransformDisplay = null)
        {
            var DetailItem = new List<GenericItem>();
            for (int i = 0; i < Math.Min(ItemCount, rankContain.RankList.Count); i++)
            {
                DetailItem.Add(new GenericItem()
                {
                    Display = TransformDisplay == null ? rankContain.RankList[i].Key : TransformDisplay(rankContain.RankList[i].Key),
                    Cnt = rankContain.RankList[i].Count,
                    KeyId = rankContain.RankList[i].Key
                });
            }
            return DetailItem;
        }
        /// <summary>
        /// 统计个人的兴趣爱好
        /// </summary>
        public static AsideColumnBody TagHobby(string accountId, int TagCnt)
        {
            var TagScore = new Dictionary<string, int>();
            //阅读量
            var visitors = Visitor.GetArticleByAccountId(accountId);
            foreach (var article in visitors)
            {
                foreach (var tag in article.TagName)
                {
                    if (TagScore.ContainsKey(tag))
                    {
                        TagScore[tag] += 1;
                    }
                    else
                    {
                        TagScore.Add(tag, 1);
                    }
                }
            }
            //评论
            IMongoQuery AuthorQuery = Query.EQ(nameof(Comment.OwnerId), accountId);
            var comments = MongoDbRepository.GetRecList<Comment>(AuthorQuery);
            foreach (var comment in comments)
            {
                var article = Article.GetArticleBySn(comment.ArticleID);
                foreach (var tag in article.TagName)
                {
                    if (TagScore.ContainsKey(tag))
                    {
                        TagScore[tag] += 3;
                    }
                    else
                    {
                        TagScore.Add(tag, 3);
                    }
                }
            }

            //收藏
            AuthorQuery = Query.EQ(nameof(Stock.OwnerId), accountId);
            var Stocks = MongoDbRepository.GetRecList<Stock>(AuthorQuery);

            foreach (var stock in Stocks)
            {
                var article = Article.GetArticleBySn(stock.ArticleID);
                foreach (var tag in article.TagName)
                {
                    if (TagScore.ContainsKey(tag))
                    {
                        TagScore[tag] += 5;
                    }
                    else
                    {
                        TagScore.Add(tag, 5);
                    }
                }
            }

            //文章
            AuthorQuery = Query.EQ(nameof(Article.OwnerId), accountId);
            var articles = MongoDbRepository.GetRecList<Article>(Query.And(AuthorQuery,ArticleListManager.FirstPageArticleQuery));
            foreach (var article in articles)
            {
                foreach (var tag in article.TagName)
                {
                    if (TagScore.ContainsKey(tag))
                    {
                        TagScore[tag] += 10;
                    }
                    else
                    {
                        TagScore.Add(tag, 10);
                    }
                }
            }

            var titlelist = new AsideColumnBody()
            {
                Title = "标签关注度",
                DetailItem = GetGenericItemListByRankContain(new RankContain(TagScore), TagCnt),
                HrefBase = "/Home/TagList?PageNo=1&TagName="
            };
            return titlelist;
        }
        /// <summary>
        /// 最热门标签
        /// </summary>
        /// <param name="TagCnt"></param>
        /// <returns></returns>
        public static AsideColumnBody MostHotTag(int TagCnt)
        {
            var titlelist = new AsideColumnBody()
            {
                Title = "最热门标签",
                DetailItem = GetGenericItemListByRankContain(TagUtility.TagRankContain, TagCnt),
                HrefBase = "/Home/TagList?PageNo=1&TagName="
            };
            return titlelist;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="TagCnt"></param>
        /// <returns></returns>
        public static AsideColumnBody MostHotTag_TwoCombo(int TagCnt)
        {
            var titlelist = new AsideColumnBody()
            {
                HrefBase = "/Home/TagList?PageNo=1&TagName=",
                Title = "最热门标签组合",
                DetailItem = GetGenericItemListByRankContain(TagUtility.TagTwoComboRankContain, TagCnt, (x) => { return x.Replace("|", " + "); }),
            };
            return titlelist;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static AsideColumnBody HotCircle(int CircleCnt)
        {
            var DistinctResult = MongoDbRepository.Distinct(UserInfo.CollectionName, nameof(UserInfo.TopicList));
            var groupCntResult = new Dictionary<string, int>();
            foreach (string item in DistinctResult)
            {
                groupCntResult.Add(item, UserInfo.GetJoinsCnt(item));
            }
            var titlelist = new AsideColumnBody()
            {
                Title = "热门圈子",
                DetailItem = GetGenericItemListByRankContain(new RankContain(groupCntResult), CircleCnt),
                HrefBase = "/Home/TagList?PageNo=1&TagName="
            };
            return titlelist;
        }

        /// <summary>
        /// 泛用标签
        /// </summary>
        /// <param name="RankTagDic"></param>
        /// <param name="TagCntDic"></param>
        /// <returns></returns>
        public static AsideColumnBody CreateTagRankAside(RankContain rankContain, int TagCnt)
        {
            var titlelist = new AsideColumnBody()
            {
                Title = "标签一览",
                DetailItem = GetGenericItemListByRankContain(rankContain, TagCnt),
                HrefBase = "/Home/TagList?PageNo=1&TagName="
            };
            return titlelist;
        }

        /// <summary>
        /// 最多文章作者
        /// </summary>
        /// <param name="AuthorCnt"></param>
        /// <returns></returns>
        public static AsideColumnBody MostArticleAuthor(int AuthorCnt)
        {
            var groupCntResult = MongoDbRepository.GroupCount(Article.CollectionName, nameof(Article.OwnerId), ArticleListManager.FirstPageArticleQuery);
            var titlelist = new AsideColumnBody()
            {
                Title = "最多文章作者",
                DetailItem = GetGenericItemListByRankContain(new RankContain(groupCntResult), AuthorCnt, UserInfo.GetUserNickNameByAccountId),
                HrefBase = "/Author/Index?AccountId="
            };
            return titlelist;
        }

        /// <summary>
        /// 该标签的作者列表（文章数排序）
        /// </summary>
        /// <param name="TopCnt"></param>
        /// <returns></returns>
        public static AsideColumnBody GetMostAuthorByTag(string tagName, int AuthorCnt = 10)
        {
            IMongoQuery TagNameQuery = Article.GetTagQuery(tagName);
            var groupCntResult = MongoDbRepository.GroupCount(Article.CollectionName, nameof(Article.OwnerId), Query.And(ArticleListManager.FirstPageArticleQuery, TagNameQuery));
            var titlelist = new AsideColumnBody()
            {
                Title = "最多文章作者",
                DetailItem = GetGenericItemListByRankContain(new RankContain(groupCntResult), AuthorCnt, UserInfo.GetUserNickNameByAccountId),
                HrefBase = "/Author/Index?AccountId="
            };
            return titlelist;
        }
        /// <summary>
        /// 获得指定时间内评分最高的文章
        /// </summary>
        /// <param name="ArticleCnt"></param>
        /// <param name="Hours"></param>
        /// <returns></returns>
        public static AsideColumnBody HotArticle(int ArticleCnt, int Hours)
        {
            //评分规则在ArticleListManager编写
            var articles = Article.GetHoursFirstPageArticleList(Hours);
            var articleBodys = new List<ArticleBody>();
            foreach (var art in articles)
            {
                articleBodys.Add(ArticleListManager.GetArticleBodyById(art.Sn));
            }
            var firstpage = articleBodys.OrderByDescending((x) => { return x.Score; });
            var titlelist = new AsideColumnBody()
            {
                Title = Hours + "小时热门文章",
                DetailItem = new List<GenericItem>(),
                HrefBase = "/ArticlePage/Index?ArticleId="
            };
            int xCnt = 0;
            foreach (var item in firstpage)
            {
                titlelist.DetailItem.Add(new GenericItem()
                {
                    Display = item.ArticleInfo.Title,
                    Cnt = item.Score,
                    KeyId = item.ArticleInfo.Sn
                });
                xCnt++;
                if (xCnt == ArticleCnt) break;
            }
            return titlelist;
        }

        /// <summary>
        /// 最新发布文章
        /// </summary>
        /// <returns></returns>
        public static AsideColumnBody LastArticle(int ArticleCnt)
        {
            var sortArgs = new Sort.SortArg[1];
            sortArgs[0] = new Sort.SortArg
            {
                FieldName = nameof(Article.PublishDateTime),
                SortType = Sort.SortType.Descending,
                SortOrder = 1
            };
            Action<MongoCursor> setCursor = x => { x.SetLimit(ArticleCnt).SetSortOrder(Sort.GetSortBuilder(sortArgs)); };
            var firstpage = MongoDbRepository.GetRecList<Article>(ArticleListManager.FirstPageArticleQuery, setCursor);
            var titlelist = new AsideColumnBody()
            {
                Title = "最新发布文章",
                DetailItem = new List<GenericItem>(),
                HrefBase = "/ArticlePage/Index?ArticleId="
            };
            foreach (var item in firstpage)
            {
                titlelist.DetailItem.Add(new GenericItem()
                {
                    Display = item.Title,
                    Cnt = item.GetStockCnt(),
                    KeyId = item.Sn
                });
            }
            return titlelist;
        }
    }
}
