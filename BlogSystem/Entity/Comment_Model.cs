using InfraStructure.DataBase;
using InfraStructure.Table;

namespace BlogSystem.Entity
{
    /// <summary>
    /// 评论
    /// </summary>
    public partial class Comment : OwnerTable
    {
        #region "model"

        /// <summary>
        /// 被评论文章全局编号
        /// </summary>
        public string ArticleID { get; set; }

        /// <summary>
        /// 评论内容的MD
        /// </summary>
        public string ContentMD { get; set; }

        /// <summary>
        /// 评论内容的HTML
        /// </summary>
        public string ContentHTML { get; set; }

        /// <summary>
        /// 回复评论的全局编号
        /// </summary>
        public string ReplyCommentID { get; set; }

        /// <summary>
        /// 数据集名称
        /// </summary>
        public override string GetCollectionName()
        {
            return "Comment";
        }

        /// <summary>
        /// 数据集名称静态字段
        /// </summary>
        public static string CollectionName = "Comment";


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
        public static string MvcTitle = "评论";

        /// <summary>
        /// 按照序列号查找评论
        /// </summary>
        /// <param name="Sn"></param>
        /// <returns>评论</returns>
        public static Comment GetCommentBySn(string Sn)
        {
            return MongoDbRepository.GetRecBySN<Comment>(Sn);
        }

        /// <summary>
        /// 插入评论
        /// </summary>
        /// <param name="Newcomment"></param>
        /// <param name="OwnerId"></param>
        /// <returns>序列号</returns>
        public static string InsertComment(Comment NewComment, string OwnerId)
        {
            return OwnerTableOperator.InsertRec(NewComment, OwnerId);
        }

        /// <summary>
        /// 删除评论
        /// </summary>
        /// <param name="DropComment"></param>
        public static void DropComment(Comment DropComment)
        {
            MongoDbRepository.DeleteRec(DropComment);
        }

        /// <summary>
        /// 修改评论
        /// </summary>
        /// <param name="OldComment"></param>
        public static void UpdateComment(Comment OldComment)
        {
            MongoDbRepository.UpdateRec(OldComment);
        }

        #endregion
    }
}
