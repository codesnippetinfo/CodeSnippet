using InfraStructure.DataBase;
using BlogSystem.Entity;

namespace BlogSystem.DisplayEntity
{
    /// <summary>
    /// 文章列表综合体
    /// </summary>
    public partial class ArticleItemBody : CacheEntityBase
    {
        #region "model"

        /// <summary>
        /// 作者信息
        /// </summary>
        public UserInfo AuthorInfo { get; set; }

        /// <summary>
        /// 文章信息
        /// </summary>
        public Article ArticleInfo { get; set; }

        /// <summary>
        /// 阅读数
        /// </summary>
        public int ReadCnt { get; set; }

        /// <summary>
        /// 评论数
        /// </summary>
        public int CommentCnt { get; set; }

        /// <summary>
        /// 评论人数
        /// </summary>
        public int CommentAccountCnt { get; set; }

        /// <summary>
        /// 收藏数
        /// </summary>
        public int StockCnt { get; set; }

        /// <summary>
        /// 文章评分
        /// </summary>
        public int Score { get; set; }


        /// <summary>
        /// 数据集名称
        /// </summary>
        public override string GetCollectionName()
        {
            return "ArticleItemBody";
        }

        /// <summary>
        /// 数据集名称静态字段
        /// </summary>
        public static string CollectionName = "ArticleItemBody";


        /// <summary>
        /// 数据主键前缀
        /// </summary>
        public override string GetPrefix()
        {
            return string.Empty;
        }

        /// <summary>
        /// 数据主键前缀静态字段
        /// </summary>
        public static string Prefix = string.Empty;

        /// <summary>
        /// Mvc画面的标题
        /// </summary>
        public static string MvcTitle = "文章列表综合体";

        /// <summary>
        /// 插入文章列表综合体
        /// </summary>
        /// <param name="NewArticleItemBody">文章列表综合体</param>
        public static void InsertArticleItemBody(ArticleItemBody NewArticleItemBody)
        {
            MongoDbRepository.InsertCacheRec(NewArticleItemBody);
        }

        #endregion
    }
}
