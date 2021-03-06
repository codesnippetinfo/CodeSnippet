using InfraStructure.DataBase;
using System;

namespace BlogSystem.Entity
{
    /// <summary>
    /// 消息
    /// </summary>
    public partial class SiteMessage : EntityBase
    {
        #region "model"

        /// <summary>
        /// 消息发出者
        /// </summary>
        public string SenderID { get; set; }

        /// <summary>
        /// 消息接收者
        /// </summary>
        public string ReceiveID { get; set; }

        /// <summary>
        /// 消息文本
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// 消息类型
        /// </summary>
        public string MsgType { get; set; }

        /// <summary>
        /// 是否处理
        /// </summary>
        public bool IsHandle { get; set; }

        /// <summary>
        /// 确认URL
        /// </summary>
        public string AcceptURL { get; set; }

        /// <summary>
        /// 拒绝URL
        /// </summary>
        public string RefuseURL { get; set; }

        /// <summary>
        /// 处理结果
        /// </summary>
        public string HandleResult { get; set; }

        /// <summary>
        /// 数据集名称
        /// </summary>
        public override string GetCollectionName()
        {
            return "SiteMessage";
        }

        /// <summary>
        /// 数据集名称静态字段
        /// </summary>
        public static string CollectionName = "SiteMessage";


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
        public static string MvcTitle = "消息";

        /// <summary>
        /// 按照序列号查找消息
        /// </summary>
        /// <param name="Sn"></param>
        /// <returns>消息</returns>
        public static SiteMessage GetSiteMessageBySn(string Sn)
        {
            return MongoDbRepository.GetRecBySN<SiteMessage>(Sn);
        }

        /// <summary>
        /// 插入消息
        /// </summary>
        /// <param name="Newsitemessage"></param>
        /// <returns>序列号</returns>
        public static string InsertSiteMessage(SiteMessage NewSiteMessage)
        {
            return MongoDbRepository.InsertRec(NewSiteMessage);
        }

        /// <summary>
        /// 删除消息
        /// </summary>
        /// <param name="DropSiteMessage"></param>
        public static void DropSiteMessage(SiteMessage DropSiteMessage)
        {
            MongoDbRepository.DeleteRec(DropSiteMessage);
        }

        /// <summary>
        /// 修改消息
        /// </summary>
        /// <param name="OldSiteMessage"></param>
        public static void UpdateSiteMessage(SiteMessage OldSiteMessage)
        {
            MongoDbRepository.UpdateRec(OldSiteMessage);
        }

        #endregion
    }
}
