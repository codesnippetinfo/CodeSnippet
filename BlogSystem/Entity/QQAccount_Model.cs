using InfraStructure.DataBase;
using System;

namespace BlogSystem.Entity
{
    /// <summary>
    /// QQ帐户信息
    /// </summary>
    public partial class QQAccount : EntityBase
    {
        #region "model"

        /// <summary>
        /// 用户昵称
        /// </summary>
        public string nickname { get; set; }

        /// <summary>
        /// 性别
        /// </summary>
        public string gender { get; set; }

        /// <summary>
        /// 头像
        /// </summary>
        public string figureurl { get; set; }

        /// <summary>
        /// OpenId
        /// </summary>
        public string OpenID { get; set; }

        /// <summary>
        /// 用户ID
        /// </summary>
        public string UserInfoID { get; set; }

        /// <summary>
        /// 最后登陆时间
        /// </summary>
        public DateTime LastAccess { get; set; }

        /// <summary>
        /// 数据集名称
        /// </summary>
        public override string GetCollectionName()
        {
            return "QQAccount";
        }

        /// <summary>
        /// 数据集名称静态字段
        /// </summary>
        public static string CollectionName = "QQAccount";


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
        public static string MvcTitle = "QQ帐户信息";

        /// <summary>
        /// 按照序列号查找QQ帐户信息
        /// </summary>
        /// <param name="Sn"></param>
        /// <returns>QQ帐户信息</returns>
        public static QQAccount GetQQAccountBySn(string Sn)
        {
            return MongoDbRepository.GetRecBySN<QQAccount>(Sn);
        }

        /// <summary>
        /// 插入QQ帐户信息
        /// </summary>
        /// <param name="Newqqaccount"></param>
        /// <returns>序列号</returns>
        public static string InsertQQAccount(QQAccount NewQQAccount)
        {
            return MongoDbRepository.InsertRec(NewQQAccount);
        }

        /// <summary>
        /// 删除QQ帐户信息
        /// </summary>
        /// <param name="DropQQAccount"></param>
        public static void DropQQAccount(QQAccount DropQQAccount)
        {
            MongoDbRepository.DeleteRec(DropQQAccount);
        }

        /// <summary>
        /// 修改QQ帐户信息
        /// </summary>
        /// <param name="OldQQAccount"></param>
        public static void UpdateQQAccount(QQAccount OldQQAccount)
        {
            MongoDbRepository.UpdateRec(OldQQAccount);
        }

        #endregion
    }
}
