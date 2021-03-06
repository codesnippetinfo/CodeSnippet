using BlogSystem.Entity;
using InfraStructure.DataBase;

namespace BlogSystem.DisplayEntity
{
    /// <summary>
    /// 文章表示综合体
    /// </summary>
    public partial class CommentBody : EntityBase
    {
        #region "model"

        /// <summary>
        /// 作者信息
        /// </summary>
        public UserInfo AuthorInfo { get; set; }

        /// <summary>
        /// 评论信息
        /// </summary>
        public Comment CommentInfo { get; set; }

        /// <summary>
        /// 评论顺序
        /// </summary>
        public int Order { get; set; }

        /// <summary>
        /// 存在子评论
        /// </summary>
        public bool HasSubComment { get; set; }

        /// <summary>
        /// 数据集名称
        /// </summary>
        public override string GetCollectionName()
        {
            return "CommentBody";
        }

        /// <summary>
        /// 数据集名称静态字段
        /// </summary>
        public static string CollectionName = "CommentBody";


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
        public static string MvcTitle = "文章表示综合体";

        /// <summary>
        /// 按照序列号查找文章表示综合体
        /// </summary>
        /// <param name="Sn"></param>
        /// <returns>文章表示综合体</returns>
        public static CommentBody GetCommentBodyBySn(string Sn)
        {
            return MongoDbRepository.GetRecBySN<CommentBody>(Sn);
        }

        /// <summary>
        /// 插入文章表示综合体
        /// </summary>
        /// <param name="Newcommentbody"></param>
        /// <returns>序列号</returns>
        public static string InsertCommentBody(CommentBody NewCommentBody)
        {
            return MongoDbRepository.InsertRec(NewCommentBody);
        }

        /// <summary>
        /// 删除文章表示综合体
        /// </summary>
        /// <param name="DropCommentBody"></param>
        public static void DropCommentBody(CommentBody DropCommentBody)
        {
            MongoDbRepository.DeleteRec(DropCommentBody);
        }

        /// <summary>
        /// 修改文章表示综合体
        /// </summary>
        /// <param name="OldCommentBody"></param>
        public static void UpdateCommentBody(CommentBody OldCommentBody)
        {
            MongoDbRepository.UpdateRec(OldCommentBody);
        }

        #endregion
    }
}
