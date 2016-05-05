using BlogSystem.BussinessLogic;
using InfraStructure.DataBase;
using InfraStructure.Table;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using System.Collections.Generic;

namespace BlogSystem.Entity
{
    /// <summary>
    /// 专题
    /// </summary>
    public partial class Topic : OwnerTable
    {
        /// <summary>
        /// 是否存在专题
        /// </summary>
        /// <param name="accountid"></param>
        /// <returns></returns>
        public static bool IsExistTopic(string accountid)
        {
            IMongoQuery TopicQuery = Query.EQ(nameof(OwnerId), accountid);
            return MongoDbRepository.GetRecordCount<Topic>(TopicQuery) > 0;
        }
        
        /// <summary>
        /// 是否关注专题
        /// </summary>
        /// <param name="accountid"></param>
        /// <param name="topicid"></param>
        /// <returns></returns>
        public static bool IsFoucs(string accountid,string topicid)
        {
            var user = UserInfo.GetUserInfoBySn(accountid);
            return user.TopicList.Contains(topicid);
        }
        /// <summary>
        /// 关注专题
        /// </summary>
        /// <param name="accountid"></param>
        /// <param name="topicid"></param>
        /// <returns></returns>
        public static bool FocusTopic(string accountid, string topicid)
        {
            var user = UserInfo.GetUserInfoBySn(accountid);
            if (user.TopicList.Contains(topicid))
            {
                return false;
            }
            user.TopicList.Add(topicid);
            UserInfo.UpdateUserInfo(user);
            UserManager.RemoveUserBody(accountid);
            UserManager.RemoveUserItemBody(accountid);
            return true;
        }

        /// <summary>
        /// 获得专题
        /// </summary>
        /// <param name="accountid"></param>
        /// <returns></returns>
        public static Topic GetTopicByAccountId(string accountid)
        {
            IMongoQuery TopicQuery = Query.EQ(nameof(OwnerId), accountid);
            return MongoDbRepository.GetFirstRec<Topic>(TopicQuery); 
        }
        /// <summary>
        /// 获得所有专题
        /// </summary>
        /// <returns></returns>
        public static List<Topic> getAllTopic()
        {
            return MongoDbRepository.GetRecList<Topic>();
        }

    }
}
