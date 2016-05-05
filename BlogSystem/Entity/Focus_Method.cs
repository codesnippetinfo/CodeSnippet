using BlogSystem.BussinessLogic;
using InfraStructure.DataBase;
using InfraStructure.Table;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using System.Collections.Generic;

namespace BlogSystem.Entity
{
    /// <summary>
    /// 用户动作
    /// </summary>
    public partial class Focus : OwnerTable
    {
        /// <summary>
        /// 是否关注
        /// </summary>
        /// <param name="ownerId"></param>
        /// <param name="ArticleId"></param>
        /// <returns></returns>
        public static bool IsFocused(string ownerId, string accountId)
        {
            IMongoQuery OwnerIdQuery = Query.EQ(nameof(OwnerId), ownerId);
            IMongoQuery ArticleQuery = Query.EQ(nameof(AccountID), accountId);
            var FocusCnt = MongoDbRepository.GetRecordCount<Focus>(Query.And(OwnerIdQuery, ArticleQuery));
            if (FocusCnt > 1)
            {
                InfraStructure.Log.ExceptionLog.Log("Focus Check Exception", ownerId, accountId, FocusCnt.ToString());
            }
            return FocusCnt != 0;
        }

        /// <summary>
        /// 获得该帐号关注的人的ID列表
        /// </summary>
        /// <param name="ownerId"></param>
        /// <returns></returns>
        public static List<string> GetFocus(string ownerId)
        {
            IMongoQuery OwnerIdQuery = Query.EQ(nameof(OwnerId), ownerId);
            var x = MongoDbRepository.GetRecList<Focus>(OwnerIdQuery);
            List<string> idList = new List<string>();
            foreach (var item in x)
            {
                idList.Add(item.AccountID);
            }
            return idList;
        }
        /// <summary>
        /// 获得关注该帐号的人的ID列表
        /// </summary>
        /// <param name="AccountID"></param>
        /// <returns></returns>
        public static List<string> GetFollow(string AccountID)
        {
            IMongoQuery OwnerIdQuery = Query.EQ(nameof(Focus.AccountID), AccountID);
            var x = MongoDbRepository.GetRecList<Focus>(OwnerIdQuery);
            List<string> idList = new List<string>();
            foreach (var item in x)
            {
                idList.Add(item.OwnerId);
            }
            return idList;
        }


        /// <summary>
        /// 获得该帐号关注的人数
        /// </summary>
        /// <param name="ownerId"></param>
        /// <returns></returns>
        public static int GetFoucsCnt(string ownerId)
        {
            IMongoQuery OwnerIdQuery = Query.EQ(nameof(OwnerId), ownerId);
            return MongoDbRepository.GetRecordCount<Focus>(OwnerIdQuery);
        }

        /// <summary>
        /// 获得关注该帐号的人数
        /// </summary>
        /// <param name="AccountID"></param>
        /// <returns></returns>
        public static int GetFollowCnt(string AccountID)
        {
            IMongoQuery OwnerIdQuery = Query.EQ(nameof(Focus.AccountID), AccountID);
            return MongoDbRepository.GetRecordCount<Focus>(OwnerIdQuery);
        }


        /// <summary>
        /// 关注 
        /// </summary>
        /// <param name="ownerId">关注者</param>
        /// <param name="ArticleId">被关注方</param>
        /// <returns></returns>
        public static bool FocusAccount(string ownerId, string accountId)
        {
            if (IsFocused(ownerId, accountId))
            {
                //已经关注 
                return false;
            }
            Focus focus = new Focus()
            {
                AccountID = accountId
            };
            InsertFocus(focus, ownerId);
            UserManager.RemoveUserItemBody(ownerId);
            UserManager.RemoveUserItemBody(accountId);
            UserManager.RemoveUserBody(ownerId);
            UserManager.RemoveUserBody(accountId);
            return true;
        }

    }
}
