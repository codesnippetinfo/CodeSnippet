using InfraStructure.DataBase;
using InfraStructure.Table;

namespace BlogSystem.Entity
{
    /// <summary>
    /// 文章MarkDown内容
    /// </summary>
    public partial class ArticleContent : OwnerTable
    {
        #region "model"

        /// <summary>
        /// 文章全局编号
        /// </summary>
        public string ArticleID { get; set; }

        /// <summary>
        /// 版本
        /// </summary>
        public RevisionType Revision { get; set; }

        /// <summary>
        /// 内容
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// HTML内容
        /// </summary>
        public string ContentType { get; set; }

        /// <summary>
        /// 数据集名称
        /// </summary>
        public override string GetCollectionName()
        {
            return "ArticleContent";
        }

        /// <summary>
        /// 数据集名称静态字段
        /// </summary>
        public static string CollectionName = "ArticleContent";


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
        public static string MvcTitle = "文章MarkDown内容";

        /// <summary>
        /// 按照序列号查找文章MarkDown内容
        /// </summary>
        /// <param name="Sn"></param>
        /// <returns>文章MarkDown内容</returns>
        public static ArticleContent GetArticleContentBySn(string Sn)
        {
            return MongoDbRepository.GetRecBySN<ArticleContent>(Sn);
        }

        /// <summary>
        /// 插入文章MarkDown内容
        /// </summary>
        /// <param name="NewArticleContent"></param>
        /// <param name="OwnerId"></param>
        /// <returns>序列号</returns>
        public static string InsertArticleContent(ArticleContent NewArticleContent, string OwnerId)
        {
            return OwnerTableOperator.InsertRec(NewArticleContent, OwnerId);
        }

        /// <summary>
        /// 删除文章MarkDown内容
        /// </summary>
        /// <param name="DropArticleContent"></param>
        public static void DropArticleContent(ArticleContent DropArticleContent)
        {
            MongoDbRepository.DeleteRec(DropArticleContent);
        }

        /// <summary>
        /// 修改文章MarkDown内容
        /// </summary>
        /// <param name="OldArticleContent"></param>
        public static void UpdateArticleContent(ArticleContent OldArticleContent)
        {
            MongoDbRepository.UpdateRec(OldArticleContent);
        }

        #endregion
    }
}
