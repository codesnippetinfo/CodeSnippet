using InfraStructure.DataBase;

namespace BlogSystem.Entity
{
    /// <summary>
    /// PDF下载
    /// </summary>
    public partial class PDFDownload : CacheEntityBase
    {
        #region "model"

        /// <summary>
        /// 文章全局编号
        /// </summary>
        public string ArticleID { get; set; }

        /// <summary>
        /// UserInfoId
        /// </summary>
        public string UserInfoId { get; set; }

        /// <summary>
        /// 数据集名称
        /// </summary>
        public override string GetCollectionName()
        {
            return "PDFDownload";
        }

        /// <summary>
        /// 数据集名称静态字段
        /// </summary>
        public static string CollectionName = "PDFDownload";


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
        public static string MvcTitle = "PDF下载";

        /// <summary>
        /// 插入PDF下载
        /// </summary>
        /// <param name="NewPDFDownload">PDF下载</param>
        public static void InsertPDFDownload(PDFDownload NewPDFDownload)
        {
            MongoDbRepository.InsertCacheRec(NewPDFDownload);
        }

        #endregion
    }
}
