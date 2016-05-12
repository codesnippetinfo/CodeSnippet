using BlogSystem.BussinessLogic;
using BlogSystem.Entity;
using InfraStructure.DataBase;
using MongoDB.Bson;
using Nest;
using System;
using System.Collections.Generic;
using System.IO;

namespace BlogDataSimulator
{
    public static class GenerateCnblogsDate
    {
        #region 模拟加载博客园标题
        /// <summary>
        /// 标题列表
        /// </summary>
        public static List<string> titles = new List<string>();
        /// <summary>
        /// 用户字典
        /// </summary>
        public static Dictionary<string, string> userdic = new Dictionary<string, string>();
        /// <summary>
        /// 用户文集字典
        /// </summary>
        public static Dictionary<string, string> userColdic = new Dictionary<string, string>();

        /// <summary>
        /// 模拟加载博客园标题
        /// </summary>
        /// <param name="cnblogFilename"></param>
        /// <param name="strMDContent"></param>
        /// <param name="strHTMLContent"></param>
        /// <param name="LimitCnt"></param>
        /// <param name="client"></param>
        public static void InsertCnblogs(string cnblogFilename, string strMDContent, string strHTMLContent, int LimitCnt, ElasticClient client, bool IsArticleRandom)
        {
            Random r = new Random();
            titles.Clear();
            userdic.Clear();
            userColdic.Clear();
            StreamReader FileReader = new StreamReader(cnblogFilename);
            string Line = string.Empty;
            int startlength = "the article title is :".Length;
            int endlength = " - 博客园".Length;
            MongoDbRepository.DrapCollection(Article.CollectionName);
            MongoDbRepository.DrapCollection(Collection.CollectionName);
            MongoDbRepository.DrapCollection(GithubAccount.CollectionName);
            MongoDbRepository.DrapCollection(QQAccount.CollectionName);
            MongoDbRepository.DrapCollection(UserInfo.CollectionName);
            MongoDbRepository.DrapCollection(ArticleContent.CollectionName);
            MongoDbRepository.DrapCollection(SiteConfig.CollectionName);


            var PublishStatusTypeValues = Enum.GetValues(typeof(ApproveStatus));
            var ArticleLevelValues = Enum.GetValues(typeof(ArticleLevel));

            int GetCnt = 0;
            int LineCnt = 0;
            while (!FileReader.EndOfStream)
            {
                Line = FileReader.ReadLine();
                LineCnt++;
                if (Line.StartsWith("the article title is :用户登录 - 博客园")) continue;
                if (!Line.StartsWith("the article title is :"))
                {
                    System.Diagnostics.Debug.WriteLine(Line);
                    continue;
                }
                if (!Line.EndsWith(" - 博客园"))
                {
                    System.Diagnostics.Debug.WriteLine(Line);
                    continue;
                }
                try
                {
                    Line = Line.Substring(startlength, Line.Length - startlength - endlength);
                    int pos = Line.LastIndexOf(" - ");
                    string title = Line.Substring(0, pos);
                    string user = Line.Substring(pos + 3);
                    if (!userdic.ContainsKey(user))
                    {
                        var userinfo = new UserInfo();
                        int qqOrGit = r.Next(100);
                        string accountId = string.Empty;
                        QQAccount qqaccount = new QQAccount();
                        GithubAccount gitaccount = new GithubAccount();

                        if (qqOrGit % 2 == 0)
                        {
                            //Github帐号
                            gitaccount = new GithubAccount()
                            {
                                Avatar_url = "https://avatars.githubusercontent.com/u/897796?v=3",
                                Login = user,
                                Name = user,
                                Email = "mynightelfplayer@hotmail.com",
                                Location = "Shanghai,China",
                                Blog = "http://www.mywechatapp.com",
                                Company = "Shanghai Chuwa software co.ltd",
                                Followers = 50,
                                Following = 2,
                            };

                            accountId = MongoDbRepository.InsertRec(gitaccount);

                            userinfo = new UserInfo()
                            {
                                RegisterAccountID = accountId,
                                Privilege = UserType.Normal,
                                RegisterMethod = GithubAccount.Github,
                                TopicList = new List<string>(),
                                TagList = new List<string>(),
                                NickName = gitaccount.Name,
                                Avatar_url = gitaccount.Avatar_url,
                                ContainTag = string.Empty,
                                AntiTag = string.Empty,
                                Catalog = new List<string>(),
                                Level = new List<ArticleLevel>()
                            };
                        }
                        else
                        {
                            //QQ
                            qqaccount = new QQAccount()
                            {
                                figureurl = "https://avatars.githubusercontent.com/u/19196306?v=3",
                                gender = "男",
                                nickname = user,
                                OpenID = "1234567890"
                            };

                            accountId = MongoDbRepository.InsertRec(qqaccount);

                            userinfo = new UserInfo()
                            {
                                RegisterAccountID = accountId,
                                Privilege = UserType.Normal,
                                RegisterMethod = QQAccount.QQ,
                                TopicList = new List<string>(),
                                TagList = new List<string>(),
                                NickName = qqaccount.nickname,
                                Avatar_url = qqaccount.figureurl,
                                ContainTag = string.Empty,
                                AntiTag = string.Empty,
                                Catalog = new List<string>(),
                                Level = new List<ArticleLevel>()
                            };
                        }

                        var x = r.Next(100);
                        if (x % 10 == 0)
                        {
                            userinfo.Privilege = UserType.Author;
                        }
                        else
                        {
                            if (x == 51)
                            {
                                userinfo.Privilege = UserType.Editor;
                            }
                        }
                        var userId = MongoDbRepository.InsertRec(userinfo);
                        if (userId == 1.ToString(EntityBase.SnFormat))
                        {
                            userinfo.Privilege = UserType.Admin;
                            UserInfo.UpdateUserInfo(userinfo);
                        }
                        if (userId == 2.ToString(EntityBase.SnFormat))
                        {
                            userinfo.Privilege = UserType.Editor;
                            UserInfo.UpdateUserInfo(userinfo);
                        }

                        if (qqOrGit % 2 == 0)
                        {
                            MongoDbRepository.UpdateRec(gitaccount, nameof(GithubAccount.UserInfoID), (BsonString)userId);
                        }
                        else
                        {
                            MongoDbRepository.UpdateRec(qqaccount, nameof(QQAccount.UserInfoID), (BsonString)userId);
                        }
                        userdic.Add(user, userId);

                        //默认文集
                        Collection collection = new Collection()
                        {
                            Title = user + " 的文集",
                            Description = user + " 的文集",
                            IsSerie = (r.Next(100) % 2 == 1)
                        };
                        var CollectionId = Collection.InsertCollection(collection, userId);
                        userColdic.Add(user, CollectionId);
                    }

                    if (!titles.Contains(title))
                    {
                        string ownerid = userdic[user];
                        string collecId = userColdic[user];
                        Article article = new Article()
                        {
                            Title = title,
                            IsFirstPage = IsArticleRandom ? (r.Next(0, 100) % 2 == 1) : true,
                            IsPrivate = IsArticleRandom ? (r.Next(0, 100) % 2 == 1) : false,
                            PublishStatus = IsArticleRandom ? (ApproveStatus)(PublishStatusTypeValues.GetValue(r.Next(0, 100) % PublishStatusTypeValues.Length)) : ApproveStatus.Accept,
                            IsCloseComment = (r.Next(0, 100) % 2 == 1),
                            IsOriginal = (r.Next(0, 100) % 2 == 1),
                            CollectionID = collecId,
                            PublishDateTime = DateTime.Now.AddMinutes(r.NextDouble() * -10000),
                            Level = (ArticleLevel)(ArticleLevelValues.GetValue(r.Next(0, 100) % ArticleLevelValues.Length)),
                            Catalog = Article.CatalogItem[r.Next(0, 100) % Article.CatalogItem.Length].Substring(0, 4)
                        };
                        if (article.IsPrivate)
                        {
                            article.IsFirstPage = false;
                            article.PublishStatus = ApproveStatus.NotNeed;
                        }
                        else
                        {
                            article.IsTopicable = (r.Next(0, 100) % 2 == 1);
                            article.IsNeedTopicApproval = (r.Next(0, 100) % 2 == 1);
                        }
                        if (article.IsFirstPage)
                        {
                            article.IsOriginal = true;
                            article.IsCloseComment = false;
                        }
                        article.ConfirmDateTime = article.PublishDateTime.AddSeconds(r.Next(1000, 7200));
                        string ArticleId = Article.InsertArticle(article, ownerid);
                        if (client != null && (!article.IsPrivate) && (article.PublishStatus == ApproveStatus.Accept)) client.Index(article);
                        //插入MarkDown文档(保证首页只读,则只需要SaveMarkDownVersion：Current)
                        ArticleContent.SaveMarkDownVersion(ArticleId, strMDContent, ownerid, RevisionType.Current);
                        ArticleContent.SaveMarkDownVersion(ArticleId, strMDContent, ownerid, RevisionType.First);
                        ArticleContent.SaveMarkDownVersion(ArticleId, strMDContent, ownerid, RevisionType.Draft);
                        ArticleContent.SaveHTMLVersion(ArticleId, strHTMLContent, ownerid);
                        titles.Add(title);
                        GetCnt++;
                        if (GetCnt == LimitCnt) break;
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine("Dulipt:" + Line);

                    }
                }
                catch (Exception)
                {
                    System.Diagnostics.Debug.WriteLine("Error:" + Line);
                }

            }
            FileReader.Close();
            System.Diagnostics.Debug.WriteLine("LineCnt:" + LineCnt);

        }

        #endregion

        /// <summary>
        /// 模拟收藏
        /// </summary>
        /// <param name="MinPerArticle"></param>
        /// <param name="MaxPerArticle"></param>
        public static void SimulateStock(int MinPerArticle = 20, int MaxPerArticle = 20)
        {
            var users = MongoDbRepository.GetRecList<GithubAccount>();
            int usercnt = users.Count;
            //遍历所有文章,每篇文章有1-Max个人Stock
            MongoDbRepository.DrapCollection(Stock.CollectionName);
            MongoDbRepository.SetIndex(Stock.CollectionName, nameof(Stock.OwnerId));
            var articles = MongoDbRepository.GetRecList<Article>(ArticleListManager.FirstPageArticleQuery);
            foreach (var article in articles)
            {
                Random r = new Random();
                int x = r.Next(MinPerArticle, MaxPerArticle + 1);
                //if (x == 0) x = r.Next(20);
                System.Diagnostics.Debug.WriteLine(article.Sn + ":" + x);
                for (int i = 0; i < x; i++)
                {
                    var uid = r.Next(1, usercnt + 1);
                    if (uid.ToString(EntityBase.SnFormat) != article.OwnerId)
                    {
                        Stock.StockArticle(uid.ToString(EntityBase.SnFormat), article.Sn);
                    }
                }
            }
        }

        /// <summary>
        /// 模拟关注
        /// </summary>
        /// <param name="MinPerAccount"></param>
        /// <param name="MaxPerAccount"></param>
        public static void SimulateFocus(int MinPerAccount = 20, int MaxPerAccount = 20)
        {
            MongoDbRepository.DrapCollection(Focus.CollectionName);
            MongoDbRepository.SetIndex(Focus.CollectionName, nameof(Focus.OwnerId));
            var users = MongoDbRepository.GetRecList<GithubAccount>();
            int usercnt = users.Count;
            foreach (var u in users)
            {
                Random r = new Random();
                int x = r.Next(MinPerAccount, MaxPerAccount + 1);
                //if (x == 0) x = r.Next(50);
                System.Diagnostics.Debug.WriteLine(u.Sn + ":" + x);
                for (int i = 0; i < x; i++)
                {
                    var uid = r.Next(1, usercnt + 1);
                    if (uid.ToString(EntityBase.SnFormat) != u.Sn)
                    {
                        Focus.FocusAccount(u.Sn, uid.ToString(EntityBase.SnFormat));
                    }
                }
            }
        }

        /// <summary>
        /// 模拟评论
        /// </summary>
        /// <param name="MinPerArticle"></param>
        /// <param name="MaxPerArticle"></param>
        public static void SimulateComment(string strComment, string strHTML, int MinPerArticle = 0, int MaxPerArticle = 100)
        {
            var users = MongoDbRepository.GetRecList<GithubAccount>();
            int usercnt = users.Count;
            //遍历所有文章,每篇文章有1-Max个人Stock
            MongoDbRepository.DrapCollection(Comment.CollectionName);
            MongoDbRepository.SetIndex(Comment.CollectionName, nameof(Stock.OwnerId));
            var articles = MongoDbRepository.GetRecList<Article>(ArticleListManager.FirstPageArticleQuery);
            foreach (var article in articles)
            {
                Random r = new Random();
                int x = r.Next(MinPerArticle, MaxPerArticle + 1);
                //if (x == 0) x = r.Next(20);
                System.Diagnostics.Debug.WriteLine(article.Sn + ":" + x);
                for (int i = 0; i < x; i++)
                {
                    var uid = r.Next(1, usercnt + 1);
                    var newComment = new Comment();
                    newComment.OwnerId = uid.ToString(EntityBase.SnFormat);
                    newComment.ContentMD = strComment;
                    newComment.ContentHTML = strHTML;
                    newComment.ArticleID = article.Sn;
                    var replyCommentId = Comment.InsertComment(newComment, newComment.OwnerId);
                    if (r.Next(1, 101) % 5 == 0)
                    {
                        var Replyuid = r.Next(1, usercnt + 1);
                        var ReplynewComment = new Comment();
                        ReplynewComment.OwnerId = Replyuid.ToString(EntityBase.SnFormat);
                        ReplynewComment.ContentMD = strComment;
                        ReplynewComment.ContentHTML = strHTML;
                        ReplynewComment.ArticleID = article.Sn;
                        ReplynewComment.ReplyCommentID = replyCommentId;
                        Comment.InsertComment(newComment, newComment.OwnerId);
                    }
                }
            }
        }

        /// <summary>
        /// 模拟专题
        /// </summary>
        public static void SimulateTopic()
        {
            MongoDbRepository.DrapCollection(Topic.CollectionName);
            MongoDbRepository.DrapCollection(TopicArticle.CollectionName);
            MongoDbRepository.SetIndex(Topic.CollectionName, nameof(Topic.OwnerId));
            MongoDbRepository.SetIndex(TopicArticle.CollectionName, nameof(TopicArticle.TopicID));
            Topic[] TopicList = new Topic[]
            {
                    new Topic(){Title = "Android知识" },
                    new Topic(){Title = "玩转Mac"},
                    new Topic(){Title = "iOS Development"},
                    new Topic(){Title = "产品经理"},
                    new Topic(){Title = "iOS移动开发"},
                    new Topic(){Title = "GitHub上有趣的资源"},
                    new Topic(){Title = "Linux学习|Gentoo/Arch/FreeBSD"},
                    new Topic(){Title = "Pythoner集中营" },
                    new Topic(){Title ="Java技术文章"},
                    new Topic(){
                        Title ="无法投稿",
                        IsPostable = false
                    },
                    new Topic(){
                        Title ="可以投稿(无需审核)",
                        IsPostable = true
                    },
                    new Topic(){
                        Title ="可以投稿(需要审核)",
                        IsPostable = true,
                        IsNeedApproval = true},
            };

            var TagList = new string[]
            {
                ".NET",
                "Java",
                "Javascript",
                "Windows",
                "AngularJS",
                "Android",
                "C#",
                "ASP.NET",
                "Python",
                "iOS",
            };

            for (int i = 0; i < TopicList.Length; i++)
            {
                TopicList[i].Description = TopicList[i].Title;
                TopicList[i].CustomTagList = TopicList[i].Title;
                TopicList[i].OwnerId = (i + 1).ToString("D8");
                MongoDbRepository.InsertRec<Topic>(TopicList[i]);
            }

            var users = MongoDbRepository.GetRecList<UserInfo>();
            Random r = new Random();
            foreach (var u in users)
            {
                //所有用户加入1 - 3个专题
                var c = new List<string>();
                for (int i = 0; i < 3; i++)
                {
                    var TagNo = r.Next(0, TopicList.Length);
                    if (!c.Contains(TopicList[TagNo].Sn))
                    {
                        c.Add(TopicList[TagNo].Sn);
                    }
                }
                MongoDbRepository.UpdateRec(u, nameof(UserInfo.TopicList), new BsonArray(c));
                //所有用户关注1 - 3个标签
                c.Clear();
                for (int i = 0; i < 3; i++)
                {
                    var TagNo = r.Next(0, TagList.Length);
                    if (!c.Contains(TagList[TagNo]))
                    {
                        c.Add(TagList[TagNo]);
                    }
                }
                MongoDbRepository.UpdateRec(u, nameof(UserInfo.TagList), new BsonArray(c));
            }
        }

        /// <summary>
        /// 模拟收藏
        /// </summary>
        /// <param name="MinPerArticle"></param>
        /// <param name="MaxPerArticle"></param>
        public static void SimulateVisitor(int MinVisitArticle = 50, int MaxVisitArticle = 200)
        {
            var users = MongoDbRepository.GetRecList<GithubAccount>();
            int usercnt = users.Count;
            //遍历所有文章,每篇文章有1-Max个人Stock
            MongoDbRepository.DrapCollection(Visitor.CollectionName);
            MongoDbRepository.SetIndex(Visitor.CollectionName, nameof(Visitor.ArticleID));
            var articles = MongoDbRepository.GetRecList<Article>(ArticleListManager.FirstPageArticleQuery);
            int ip = 1;
            foreach (var article in articles)
            {
                Random r = new Random();
                int x = r.Next(MinVisitArticle, MaxVisitArticle + 1);
                System.Diagnostics.Debug.WriteLine(article.Sn + ":" + x);
                for (int i = 0; i < x; i++)
                {
                    var uid = r.Next(1, usercnt + 1);
                    var VisitorRec = new Visitor()
                    {
                        ArticleID = article.Sn,
                        UserInfoId = uid.ToString(EntityBase.SnFormat),
                        UserHostAddress = ip.ToString(),
                    };
                    if (r.Next(100) % 2 == 1)
                    {
                        VisitorRec.UserAgent = "Spider";
                        Visitor.InsertSpider(VisitorRec);
                    }
                    else
                    {
                        VisitorRec.UserAgent = "Webkit";
                        Visitor.InsertVisitor(VisitorRec);
                    }
                    ip++;
                }
            }
        }

    }
}
