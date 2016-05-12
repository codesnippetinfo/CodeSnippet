using BlogSystem.BussinessLogic;
using BlogSystem.DisplayEntity;
using BlogSystem.Entity;
using BlogSystem.TagSystem;
using CodeSnippet.Controllers;
using InfraStructure.DataBase;
using InfraStructure.Log;
using InfraStructure.Storage;
using System.Configuration;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.IO;

namespace CodeSnippet
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            //数据库准备
            MongoDbRepository.Init(new[] { "Bussiness" }, "Bussiness");
            //本地缩略图保存
            string Image = ConfigurationManager.AppSettings["Image"];
            string Thumbnail = ConfigurationManager.AppSettings["Thumbnail"];
            if (Image == "Mongo" || Thumbnail == "Mongo") MongoStorage.Init("28031");
            if (Image == "FileSystem" || Thumbnail == "FileSystem") FileSystemStorage.Init(Server.MapPath("/FileStorage/"), new string[] { "Image", "Thumbnail" });
            //日志
            MongoDbRepositoryLog.Init();
            //七牛存储
            QiniuStorage.Init(ConfigurationManager.AppSettings["QINIU:AK"], ConfigurationManager.AppSettings["QINIU:SK"],
                              ConfigurationManager.AppSettings["QINIU:BUCKET"], ConfigurationManager.AppSettings["QINIU:URLBASE"]);
            //GitHub
            GithubAccount.AppName = ConfigurationManager.AppSettings["GITHUB:AppName"];
            GithubAccount.ClientID = ConfigurationManager.AppSettings["GITHUB:ClientID"];
            GithubAccount.ClientSecret = ConfigurationManager.AppSettings["GITHUB:ClientSecret"];

            //QQ
            QQAccount.appID = ConfigurationManager.AppSettings["QQ:AppID"];
            QQAccount.appKey = ConfigurationManager.AppSettings["QQ:AppKey"];
            QQAccount.callback = ConfigurationManager.AppSettings["QQ:CallBack"];
            QQAccount.authorizeURL = ConfigurationManager.AppSettings["QQ:AuthorizeURL"];
            //设置缓存
            SetCache();

            //设置索引
            SetIndex();

            string SearchMethod = ConfigurationManager.AppSettings["SearchMethod"];
            switch (SearchMethod)
            {
                case ConstHelper.MongoTextSearch:
                    //设置Text索引用以检索(ElasticSearch)
                    MongoDbRepository.SetTextIndex(Article.CollectionName, nameof(Article.Title));
                    MongoDbRepository.SetTextIndex(ArticleContent.CollectionName, nameof(ArticleContent.Content));
                    MongoDbRepository.SetTextIndex(Comment.CollectionName, nameof(Comment.ContentMD));
                    break;
                case ConstHelper.ElasticSearch:
                    //ElasticSearch NEST的初始化
                    SearchManager.Init();
                    break;
            }
            //加载标签库
            var filename = Server.MapPath("/Content/Tag.xlsm");
            AdminController.TagFilename = filename;
            if (File.Exists(filename))
            {
                AdminController.InsertExcelTagInfo(new FileStream(filename, FileMode.Open));
            }
            TagUtility.Init();

            //PDF设定
            if (ConfigurationManager.AppSettings["DEBUGMODE"] == "true")
            {
                FileSystemController.BaseUrl = "http://localhost:60907";
            }
            else
            {
                FileSystemController.BaseUrl = ConfigurationManager.AppSettings["URLBASE"];
            }
            FileSystemController.PDFFolder = Server.MapPath("/FileStorage/PDF/");
            if (!Directory.Exists(FileSystemController.PDFFolder))
            {
                Directory.CreateDirectory(FileSystemController.PDFFolder);
            }
            //业务配置加载
            GetConfig();
            //新建临时文件夹
            var tempPath = Server.MapPath("/") + "/Temp/";
            if (!Directory.Exists(tempPath))
            {
                Directory.CreateDirectory(tempPath);
            }
        }
        /// <summary>
        /// 设置缓存
        /// </summary>
        private static void SetCache()
        {
            MongoDbRepository.DrapCollection(UserBody.CollectionName);
            MongoDbRepository.DrapCollection(UserItemBody.CollectionName);
            MongoDbRepository.DrapCollection(ArticleItemBody.CollectionName);
            MongoDbRepository.DrapCollection(ArticleBody.CollectionName);

            MongoDbRepository.SetCacheTime(ArticleBody.CollectionName, 15);
            MongoDbRepository.SetCacheTime(ArticleItemBody.CollectionName, 15);
            MongoDbRepository.SetCacheTime(UserBody.CollectionName, 15);
            MongoDbRepository.SetCacheTime(UserItemBody.CollectionName, 15);

            MongoDbRepository.SetIndex(UserBody.CollectionName, nameof(UserBody.GitInfo) + "." + MongoDbRepository.MongoKeyField);
            MongoDbRepository.SetIndex(UserBody.CollectionName, nameof(UserBody.QQInfo) + "." + MongoDbRepository.MongoKeyField);
            MongoDbRepository.SetIndex(UserItemBody.CollectionName, nameof(UserItemBody.UserInfo) + "." + MongoDbRepository.MongoKeyField);
            MongoDbRepository.SetIndex(ArticleItemBody.CollectionName, nameof(ArticleItemBody.ArticleInfo) + "." + MongoDbRepository.MongoKeyField);
            MongoDbRepository.SetIndex(ArticleItemBody.CollectionName, nameof(ArticleItemBody.AuthorInfo) + "." + MongoDbRepository.MongoKeyField);
            MongoDbRepository.SetIndex(ArticleBody.CollectionName, nameof(ArticleBody.ArticleInfo) + "." + MongoDbRepository.MongoKeyField);

        }

        /// <summary>
        /// 设置索引
        /// </summary>
        private static void SetIndex()
        {
            MongoDbRepository.SetIndex(Article.CollectionName, nameof(Article.OwnerId));
            MongoDbRepository.SetIndex(Collection.CollectionName, nameof(Collection.OwnerId));
            MongoDbRepository.SetIndex(Stock.CollectionName, nameof(Stock.OwnerId));
            MongoDbRepository.SetIndex(Stock.CollectionName, nameof(Stock.ArticleID));
            MongoDbRepository.SetIndex(Stock.CollectionName, nameof(Stock.AuthorID));
            MongoDbRepository.SetIndex(Focus.CollectionName, nameof(Focus.OwnerId));
            MongoDbRepository.SetIndex(Focus.CollectionName, nameof(Focus.AccountID));
            MongoDbRepository.SetIndex(Comment.CollectionName, nameof(Comment.ArticleID));
            MongoDbRepository.SetIndex(Comment.CollectionName, nameof(Comment.OwnerId));
            MongoDbRepository.SetIndex(Visitor.CollectionName, nameof(Visitor.ArticleID));
            MongoDbRepository.SetIndex(Topic.CollectionName, nameof(Topic.OwnerId));
            MongoDbRepository.SetIndex(TopicArticle.CollectionName, nameof(TopicArticle.TopicID));
        }

        /// <summary>
        /// 获得配置文件内容
        /// </summary>
        public static void GetConfig()
        {
            var config = SiteConfig.GetSiteConfigBySn("00000001");
            if (config == null)
            {
                MongoDbRepository.InsertRec(new SiteConfig());
            }
            else
            {
                ArticleListManager.SetTopArticle(config.TopArticleID);
            }
        }

    }
}
