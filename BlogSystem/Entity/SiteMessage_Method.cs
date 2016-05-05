using InfraStructure.DataBase;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using System.Collections.Generic;

namespace BlogSystem.Entity
{
    /// <summary>
    /// 消息
    /// </summary>
    public partial class SiteMessage : EntityBase
    {
        /// <summary>
        /// 通知类型 
        /// </summary>
        public const string NotifyType = "Notify";
        /// <summary>
        /// 是否类型
        /// </summary>
        public const string YesNoType = "YesNo";
        /// <summary>
        /// 管理者
        /// </summary>
        public const string AdminId = "00000001";

        /// <summary>
        /// 创建提醒消息
        /// </summary>
        /// <param name="receiveid"></param>
        /// <param name="content"></param>
        /// <param name=""></param>
        public static void CreateNotify(string receiveid, string content, string senderid = AdminId)
        {
            var msg = new SiteMessage()
            {
                Content = content,
                IsHandle = false,
                MsgType = NotifyType,
                ReceiveID = receiveid,
                SenderID = senderid
            };
            InsertSiteMessage(msg);
        }

        /// <summary>
        /// 创建接受拒绝消息
        /// </summary>
        /// <param name="receiveid"></param>
        /// <param name="content"></param>
        /// <param name="YesUrl"></param>
        /// <param name="NoUrl"></param>
        /// <param name="senderid"></param>
        public static void CreateYesNo(string receiveid, string content, string YesUrl, string NoUrl, string senderid = AdminId)
        {
            var msg = new SiteMessage()
            {
                Content = content,
                IsHandle = false,
                MsgType = YesNoType,
                ReceiveID = receiveid,
                SenderID = senderid,
                AcceptURL = YesUrl,
                RefuseURL = NoUrl
            };
            InsertSiteMessage(msg);
        }

        /// <summary>
        /// 关闭消息
        /// </summary>
        /// <param name="messageId"></param>
        /// <param name="ReceiveId"></param>
        public static bool CloseMessage(string messageId, string receiveid,string result = "已处理")
        {
            //防止恶意关闭
            IMongoQuery msgidQuery = Query.EQ(MongoDbRepository.MongoKeyField, messageId);
            IMongoQuery receiveQuery = Query.EQ(nameof(ReceiveID), receiveid);
            var msg = MongoDbRepository.GetFirstRec<SiteMessage>(Query.And(msgidQuery, receiveQuery));
            if (msg == null) return false;
            msg.IsHandle = true;
            msg.HandleResult = result;
            UpdateSiteMessage(msg);
            return true;
        }
        /// <summary>
        /// 获得所有消息
        /// </summary>
        /// <param name="receiveid"></param>
        /// <returns></returns>
        public static List<SiteMessage> GetMessage(string receiveid)
        {
            IMongoQuery receiveQuery = Query.EQ(nameof(ReceiveID), receiveid);
            var msg = MongoDbRepository.GetRecList<SiteMessage>(receiveQuery);
            return msg;
        }
        /// <summary>
        /// 获得所有未处理消息
        /// </summary>
        /// <param name="receiveid"></param>
        /// <returns></returns>
        public static List<SiteMessage> GetUnHandleMessage(string receiveid)
        {
            IMongoQuery receiveQuery = Query.EQ(nameof(ReceiveID), receiveid);
            IMongoQuery UnhandleQuery = Query.EQ(nameof(IsHandle), false);
            var msg = MongoDbRepository.GetRecList<SiteMessage>(Query.And(UnhandleQuery, receiveQuery));
            return msg;
        }
    }
}
