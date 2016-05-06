using BlogSystem.BussinessLogic;
using InfraStructure.DataBase;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using System;
using System.Collections.Generic;
namespace BlogSystem.Entity
{
    public partial class UserInfo : EntityBase
    {
        /// <summary>
        /// 获得用户昵称
        /// </summary>
        /// <param name="accountId"></param>
        /// <returns></returns>
        public static string GetUserNickNameByAccountId(string accountId)
        {
            var u = GetUserInfoBySn(accountId);
            if (u != null) return u.NickName;
            return string.Empty;
        }

        /// <summary>
        /// 除去管理员和拉黑用户之外的所有人
        /// </summary>
        public static IMongoQuery NormalUserQuery
        {
            get
            {
                var NormalQuery = Query.EQ(nameof(Privilege), UserType.Normal);
                var AuthorQuery = Query.EQ(nameof(Privilege), UserType.Author);
                var EditorQuery = Query.EQ(nameof(Privilege), UserType.Editor);
                return Query.Or(NormalQuery, AuthorQuery, EditorQuery);
            }
        }
        
        /// <summary>
        /// 普通和作者的人数
        /// </summary>
        /// <returns></returns>
        public static int GetNormalUserCnt()
        {
            return MongoDbRepository.GetRecordCount<UserInfo>(NormalUserQuery);
        }

        /// <summary>
        /// 普通和作者的一览表
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public static List<UserInfo> GetNormalUserInfo(Pages p)
        {
            var sortArgs = new Sort.SortArg[1];
            sortArgs[0] = new Sort.SortArg
            {
                FieldName = MongoDbRepository.MongoKeyField,
                SortType = Sort.SortType.Descending,
                SortOrder = 1
            };
            Action<MongoCursor> setCursor = x => { x.SetSkip(p.SkipCount()).SetLimit(p.PageItemCount).SetSortOrder(Sort.GetSortBuilder(sortArgs)); };
            return MongoDbRepository.GetRecList<UserInfo>(NormalUserQuery, setCursor);
        }

        /// <summary>
        /// 权限变更
        /// </summary>
        /// <param name="accountId"></param>
        /// <param name="group"></param>
        public static void ChangePrivilege(string accountId, UserType group)
        {
            var u = GetUserInfoBySn(accountId);
            u.Privilege = group;
            UpdateUserInfo(u);
            UserManager.RemoveUserBody(accountId);
            UserManager.RemoveUserItemBody(accountId);
        }


        #region Circle
        /// <summary>
        /// 是否加入
        /// </summary>
        /// <param name="tagName"></param>
        /// <param name="accountId"></param>
        /// <returns></returns>
        public static bool IsJoined(string tagName, string accountId)
        {
            IMongoQuery TagNameQuery = Query.EQ(nameof(TagList), tagName);
            IMongoQuery AccountIdQuery = Query.EQ(MongoDbRepository.MongoKeyField, accountId);
            var JoinCnt = MongoDbRepository.GetRecordCount<UserInfo>(Query.And(TagNameQuery, AccountIdQuery));
            if (JoinCnt > 1)
            {
                InfraStructure.Log.ExceptionLog.Log("Join Check Exception", accountId, accountId, JoinCnt.ToString());
            }
            return JoinCnt != 0;
        }

        /// <summary>
        /// 获得该TagName加入的人的ID列表
        /// </summary>
        /// <param name="ownerId"></param>
        /// <returns></returns>
        public static List<UserInfo> GetJoins(string tagName)
        {
            IMongoQuery TagNameQuery = Query.EQ(nameof(TagList), tagName);
            return MongoDbRepository.GetRecList<UserInfo>(TagNameQuery);
        }

        /// <summary>
        /// 获得该加入圈子的人数
        /// </summary>
        /// <param name="ownerId"></param>
        /// <returns></returns>
        public static int GetJoinsCnt(string tagName)
        {
            IMongoQuery TagNameQuery = Query.EQ(nameof(TagList), tagName);
            return MongoDbRepository.GetRecordCount<UserInfo>(TagNameQuery);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tagName"></param>
        /// <param name="accountId"></param>
        /// <returns></returns>
        public static bool JoinTag(string tagName, string accountId)
        {
            if (IsJoined(tagName, accountId))
            {
                //已经加入
                return false;
            }
            IMongoQuery AccountIDQuery = Query.EQ(MongoDbRepository.MongoKeyField, accountId);
            var u = MongoDbRepository.GetFirstRec<UserInfo>(AccountIDQuery);
            u.TagList.Add(tagName);
            UpdateUserInfo(u);
            return true;
        }
        #endregion
    }
}
