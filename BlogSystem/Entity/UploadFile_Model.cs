using InfraStructure.DataBase;
using InfraStructure.Table;

namespace BlogSystem.Entity
{
    /// <summary>
    /// 上传文件
    /// </summary>
    public partial class UploadFile : OwnerTable
    {
        #region "model"

        /// <summary>
        /// 文件名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 文件大小
        /// </summary>
        public int Size { get; set; }

        /// <summary>
        /// 缩略文件大小
        /// </summary>
        public int SmallFileSize { get; set; }

        /// <summary>
        /// 文章全局编号
        /// </summary>
        public string ArticleID { get; set; }

        /// <summary>
        /// 数据集名称
        /// </summary>
        public override string GetCollectionName()
        {
            return "UploadFile";
        }

        /// <summary>
        /// 数据集名称静态字段
        /// </summary>
        public static string CollectionName = "UploadFile";


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
        public static string MvcTitle = "上传文件";

        /// <summary>
        /// 按照序列号查找上传文件
        /// </summary>
        /// <param name="Sn"></param>
        /// <returns>上传文件</returns>
        public static UploadFile GetUploadFileBySn(string Sn)
        {
            return MongoDbRepository.GetRecBySN<UploadFile>(Sn);
        }

        /// <summary>
        /// 插入上传文件
        /// </summary>
        /// <param name="Newuploadfile"></param>
        /// <param name="OwnerId"></param>
        /// <returns>序列号</returns>
        public static string InsertUploadFile(UploadFile NewUploadFile, string OwnerId)
        {
            return OwnerTableOperator.InsertRec(NewUploadFile, OwnerId);
        }

        /// <summary>
        /// 删除上传文件
        /// </summary>
        /// <param name="DropUploadFile"></param>
        public static void DropUploadFile(UploadFile DropUploadFile)
        {
            MongoDbRepository.DeleteRec(DropUploadFile);
        }

        /// <summary>
        /// 修改上传文件
        /// </summary>
        /// <param name="OldUploadFile"></param>
        public static void UpdateUploadFile(UploadFile OldUploadFile)
        {
            MongoDbRepository.UpdateRec(OldUploadFile);
        }

        #endregion
    }
}
