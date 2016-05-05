using System;
using System.Collections.Generic;
using InfraStructure.DataBase;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;

namespace InfraStructure.Table
{
    /// <summary>
    ///     业务相关数据库操作
    /// </summary>
    public static class OwnerTableOperator
    {
        #region "QueryCondition"
        /// <summary>
        /// 持有者查询
        /// </summary>
        /// <param name="ownerId"></param>
        /// <returns></returns>
        public static IMongoQuery OwnerIdQuery(string ownerId)
        {
            return Query.EQ(nameof(OwnerTable.OwnerId), ownerId);
        }

        /// <summary>
        ///     作者查询
        /// </summary>
        /// <param name="createUser"></param>
        /// <returns></returns>
        public static IMongoQuery CreateUserCodeQuery(string createUser)
        {
            return Query.EQ(nameof(EntityBase.CreateUser), createUser);
        }

        /// <summary>
        /// 编号查询
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public static IMongoQuery CodeQuery(string code)
        {
            return Query.EQ(nameof(OwnerTable.Code), code);
        }
        #endregion

        #region OwnerBaseQeury

        /// <summary>
        ///     获得指定OwnerId的指定表的数据数量
        /// </summary>
        /// <param name="collectionName"></param>
        /// <param name="ownerId"></param>
        /// <param name="includeDeleted"></param>
        /// <returns></returns>
        public static int GetCountByOwnerId(string collectionName, string ownerId, bool includeDeleted = false)
        {
            return MongoDbRepository.GetRecordCount(collectionName, OwnerIdQuery(ownerId), includeDeleted);
        }
        /// <summary>
        ///     获得指定AccountId的指定表的创建数据数量
        /// </summary>
        /// <param name="collectionName"></param>
        /// <param name="accountId"></param>
        /// <param name="includeDeleted"></param>
        /// <returns></returns>
        public static int GetCountByCreateUser(string collectionName, string accountId, bool includeDeleted = false)
        {
            return MongoDbRepository.GetRecordCount(collectionName, CreateUserCodeQuery(accountId), includeDeleted);
        }

        /// <summary>
        /// 获得指定OwnerId的指定表的数据数量
        /// </summary>
        /// <param name="collectionName"></param>
        /// <param name="ownerId"></param>
        /// <param name="query"></param>
        /// <param name="includeDeleted"></param>
        /// <returns></returns>
        public static int GetCountByOwnerId(string collectionName, string ownerId, IMongoQuery query, bool includeDeleted = false)
        {
            query = Query.And(OwnerIdQuery(ownerId), query);
            return MongoDbRepository.GetRecordCount(collectionName, query, includeDeleted);
        }
        /// <summary>
        /// 获得指定OwnerId的指定表的数据(改进版本)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ownerId"></param>
        /// <returns></returns>
        public static List<T> GetRecListByOwnerId<T>(string ownerId) where T : EntityBase, new()
        {
            return MongoDbRepository.GetRecList<T>(new T().GetCollectionName(), OwnerIdQuery(ownerId));
        }
        /// <summary>
        ///     获得指定OwnerId的指定表的数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collectionName"></param>
        /// <param name="ownerId"></param>
        /// <returns></returns>
        public static List<T> GetRecListByOwnerId<T>(string collectionName, string ownerId) where T : EntityBase
        {
            return MongoDbRepository.GetRecList<T>(collectionName, OwnerIdQuery(ownerId));
        }
        /// <summary>
        /// 获得指定OwnerId的指定表的BSONDOCUMENT
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collectionName"></param>
        /// <param name="ownerId"></param>
        /// <returns></returns>
        public static List<BsonDocument> GetRecListByOwnerId(string collectionName, string ownerId)
        {
            return MongoDbRepository.GetRecList(collectionName, OwnerIdQuery(ownerId));
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collectionName"></param>
        /// <param name="ownerId"></param>
        /// <param name="query"></param>
        /// <returns></returns>
        public static List<T> GetRecListByOwnerId<T>(string collectionName, string ownerId, IMongoQuery query) where T : EntityBase
        {
            query = Query.And(OwnerIdQuery(ownerId), query);
            return MongoDbRepository.GetRecList<T>(collectionName, query);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="collectionName"></param>
        /// <param name="ownerId"></param>
        /// <param name="sortArgs"></param>
        /// <param name="query"></param>
        /// <returns></returns>
        public static List<BsonDocument> GetRecListByOwnerId(string collectionName, string ownerId, Sort.SortArg[] sortArgs, IMongoQuery query)
        {
            query = Query.And(query, OwnerIdQuery(ownerId));
            Action<MongoCursor> setting = x => { x.SetSortOrder(Sort.GetSortBuilder(sortArgs)); };
            var list = MongoDbRepository.GetRecList<BsonDocument>(collectionName, query, setting);
            return list;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="collectionName"></param>
        /// <param name="ownerId"></param>
        /// <param name="sortArgs"></param>
        /// <returns></returns>
        public static List<BsonDocument> GetRecListByOwnerId(string collectionName, string ownerId, Sort.SortArg[] sortArgs)
        {
            Action<MongoCursor> setting = x => { x.SetSortOrder(Sort.GetSortBuilder(sortArgs)); };
            var list = MongoDbRepository.GetRecList<BsonDocument>(collectionName, OwnerIdQuery(ownerId), setting);
            return list;
        }
        /// <summary>
        ///     获得指定默认Owner的指定表的数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static List<T> GetRecListForDefaultOwner<T>() where T : EntityBase, new()
        {
            return MongoDbRepository.GetRecList<T>(OwnerIdQuery(OwnerTable.DefaultOwnerId));
        }
        /// <summary>
        ///     获得指定默认Owner的指定表的数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="query"></param>
        /// <returns></returns>
        public static List<T> GetRecListForDefaultOwner<T>(IMongoQuery query) where T : EntityBase, new()
        {
            if (query == null)
            {
                return GetRecListForDefaultOwner<T>();
            }
            return MongoDbRepository.GetRecList<T>(Query.And(query, OwnerIdQuery(OwnerTable.DefaultOwnerId)));
        }
        /// <summary>
        ///     获得指定作者的对象集合
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="CollectionName"></param>
        /// <param name="createUser"></param>
        /// <returns></returns>
        public static List<T> GetRecListByCreateUserAccountCode<T>(string createUser) where T : EntityBase, new()
        {
            return MongoDbRepository.GetRecList<T>(new T().GetCollectionName(), CreateUserCodeQuery(createUser));
        }
        /// <summary>
        /// 分页用
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collectionName"></param>
        /// <param name="ownerId"></param>
        /// <param name="sort"></param>
        /// <param name="currentPage"></param>
        /// <returns></returns>
        public static List<T> GetRecListByOwnerIdWithPage<T>(string collectionName, string ownerId, Sort.SortArg[] sortArgs, Pages currentPage) where T : EntityBase
        {
            //排序 限制数据条数 这里可以无视顺序？只是设定阶段？
            Action<MongoCursor> setting = x => { x.SetSortOrder(Sort.GetSortBuilder(sortArgs)).SetLimit(currentPage.PageItemCount).SetSkip(currentPage.SkipCount()); };
            var list = MongoDbRepository.GetRecList<T>(collectionName, OwnerIdQuery(ownerId), setting);
            return list;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collectionName"></param>
        /// <param name="ownerId"></param>
        /// <param name="sortArgs"></param>
        /// <param name="currentPage"></param>
        /// <param name="query"></param>
        /// <returns></returns>
        public static List<T> GetRecListByOwnerIdWithPage<T>(string collectionName, string ownerId, Sort.SortArg[] sortArgs, Pages currentPage, IMongoQuery query) where T : EntityBase
        {
            //排序 限制数据条数 这里可以无视顺序？只是设定阶段？
            query = Query.And(query, OwnerIdQuery(ownerId));
            Action<MongoCursor> setting = x => { x.SetSortOrder(Sort.GetSortBuilder(sortArgs)).SetLimit(currentPage.PageItemCount).SetSkip(currentPage.SkipCount()); };
            var list = MongoDbRepository.GetRecList<T>(collectionName, query, setting);
            return list;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="collectionName"></param>
        /// <param name="ownerId"></param>
        /// <param name="sortArgs"></param>
        /// <param name="currentPage"></param>
        /// <returns></returns>
        public static List<BsonDocument> GetRecListByOwnerIdWithPage(string collectionName, string ownerId, Sort.SortArg[] sortArgs, Pages currentPage)
        {
            //排序 限制数据条数 这里可以无视顺序？只是设定阶段？
            Action<MongoCursor> setting = x => { x.SetSortOrder(Sort.GetSortBuilder(sortArgs)).SetLimit(currentPage.PageItemCount).SetSkip(currentPage.SkipCount()); };
            var list = MongoDbRepository.GetRecList<BsonDocument>(collectionName, OwnerIdQuery(ownerId), setting);
            return list;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="collectionName"></param>
        /// <param name="ownerId"></param>
        /// <param name="sortArgs"></param>
        /// <param name="currentPage"></param>
        /// <param name="query"></param>
        /// <returns></returns>
        public static List<BsonDocument> GetRecListByOwnerIdWithPage(string collectionName, string ownerId, Sort.SortArg[] sortArgs, Pages currentPage, IMongoQuery query)
        {
            //排序 限制数据条数 这里可以无视顺序？只是设定阶段？
            query = Query.And(query, OwnerIdQuery(ownerId));
            Action<MongoCursor> setting = x =>
            {
                x.SetSortOrder(Sort.GetSortBuilder(sortArgs))
                 .SetLimit(currentPage.PageItemCount)
                 .SetSkip(currentPage.SkipCount());
            };
            var list = MongoDbRepository.GetRecList<BsonDocument>(collectionName, query, setting);
            return list;
        }




        /// <summary>
        ///     获得指定OwnerId,Code的记录
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collectionName"></param>
        /// <param name="ownerId"></param>
        /// <returns></returns>
        public static T GetRecByCodeAtOwner<T>(string collectionName, string ownerId, string code)
            where T : OwnerTable
        {
            var query = Query.And(OwnerIdQuery(ownerId), CodeQuery(code));
            return MongoDbRepository.GetFirstRec<T>(collectionName, query);
        }

        /// <summary>
        /// 获得指定OwnerId,Code的记录(改进版本)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ownerId"></param>
        /// <param name="code"></param>
        /// <returns></returns>
        public static T GetRecByCodeAtOwner<T>(string ownerId, string code)
           where T : OwnerTable, new()
        {
            var query = Query.And(OwnerIdQuery(ownerId), CodeQuery(code));
            return MongoDbRepository.GetFirstRec<T>(new T().GetCollectionName(), query);
        }

        /// <summary>
        /// 获得指定OwnerId,Code的记录(改进版本)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="code"></param>
        /// <returns></returns>
        public static T GetRecByCodeAtOwner<T>(string code)
           where T : OwnerTable, new()
        {
            var query = Query.And(OwnerIdQuery(OwnerTable.DefaultOwnerId), CodeQuery(code));
            return MongoDbRepository.GetFirstRec<T>(new T().GetCollectionName(), query);
        }


        /// <summary>
        /// 通过组织和编号获得
        /// </summary>
        /// <param name="collectionName"></param>
        /// <param name="ownerId"></param>
        /// <param name="code"></param>
        /// <returns></returns>
        public static BsonDocument GetRecByCodeAtOwner(string collectionName, string ownerId, string code)
        {
            var query = Query.And(OwnerIdQuery(ownerId), CodeQuery(code));
            return MongoDbRepository.GetFirstRec(collectionName, query);
        }
        /// <summary>
        ///     获得删除记录
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ownerId"></param>
        /// <returns></returns>
        public static List<T> GetDeleteRec<T>(string ownerId) where T : EntityBase, new()
        {
            return MongoDbRepository.GetDeleteRecList<T>(OwnerIdQuery(ownerId));
        }
        #endregion

        #region "Insert Recover"

        /// <summary>
        /// 直接插入一个OwnerTable对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string InsertRecForDefaultOwner<T>(T obj) where T : OwnerTable
        {
            return InsertRec(obj, OwnerTable.DefaultOwnerId);
        }
        /// <summary>
        /// 直接插入一个OwnerTable对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <param name="OwnerId"></param>
        /// <returns></returns>
        public static string InsertRec<T>(T obj, string OwnerId) where T : OwnerTable
        {
            obj.OwnerId = OwnerId;
            obj.Code = OwnerTable.GetNewCodeByOwnerId(obj);
            return MongoDbRepository.InsertRec(obj);
        }

        /// <summary>
        /// 恢复数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="code"></param>
        /// <param name="ownerId"></param>
        public static void RecoverRec<T>(string code, string ownerId) where T : EntityBase, new()
        {
            MongoDbRepository.RecoverRec(new T().GetCollectionName(), Query.And(OwnerIdQuery(ownerId), CodeQuery(code)));
        }
        /// <summary>
        /// 恢复数据
        /// </summary>
        /// <param name="collectionName"></param>
        /// <param name="code"></param>
        /// <param name="ownerId"></param>
        public static void RecoverRec(string collectionName, string code, string ownerId)
        {
            MongoDbRepository.RecoverRec(collectionName, Query.And(OwnerIdQuery(ownerId), CodeQuery(code)));
        }

        #endregion
    }
}