using InfraStructure.DataBase;
using InfraStructure.Table;

namespace BlogSystem.Entity
{
    /// <summary>
    /// 用户动作
    /// </summary>
    public partial class Focus : OwnerTable
    {
        #region "model"

        /// <summary>
        /// 被关注的用户编号
        /// </summary>
        public string AccountID { get; set; }

        /// <summary>
        /// 数据集名称
        /// </summary>
        public override string GetCollectionName()
        {
            return "Focus";
        }

        /// <summary>
        /// 数据集名称静态字段
        /// </summary>
        public static string CollectionName = "Focus";


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
        public static string MvcTitle = "用户动作";

        /// <summary>
        /// 按照序列号查找用户动作
        /// </summary>
        /// <param name="Sn"></param>
        /// <returns>用户动作</returns>
        public static Focus GetFocusBySn(string Sn)
        {
            return MongoDbRepository.GetRecBySN<Focus>(Sn);
        }

        /// <summary>
        /// 插入用户动作
        /// </summary>
        /// <param name="Newfocus"></param>
        /// <param name="OwnerId"></param>
        /// <returns>序列号</returns>
        public static string InsertFocus(Focus NewFocus, string OwnerId)
        {
            return OwnerTableOperator.InsertRec(NewFocus, OwnerId);
        }

        /// <summary>
        /// 删除用户动作
        /// </summary>
        /// <param name="DropFocus"></param>
        public static void DropFocus(Focus DropFocus)
        {
            MongoDbRepository.DeleteRec(DropFocus);
        }

        /// <summary>
        /// 修改用户动作
        /// </summary>
        /// <param name="OldFocus"></param>
        public static void UpdateFocus(Focus OldFocus)
        {
            MongoDbRepository.UpdateRec(OldFocus);
        }

        #endregion
    }
}
