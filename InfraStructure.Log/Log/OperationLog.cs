using System;
using System.Collections.Generic;
using System.ComponentModel;
using InfraStructure.DataBase;
using InfraStructure.Table;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;

namespace InfraStructure.Log
{
    public class OperationLog : OwnerTable
    {
        /// <summary>
        /// 用户
        /// </summary>
        [DisplayName("用户名")]
        public string User { set; get; }
        /// <summary>
        /// 行为
        /// </summary>
        [DisplayName("操作对象")]
        public string Target { set; get; }
        /// <summary>
        /// 行为
        /// </summary>
        [DisplayName("行为")]
        public string Action { set; get; }
        /// <summary>
        /// 备注
        /// </summary>
        [DisplayName("备注")]
        public string Comment { set; get; }

        /// <summary>
        ///     数据集名称
        /// </summary>
        public override string GetCollectionName()
        {
            return "OperationLog";
        }

        /// <summary>
        ///     数据集名称静态字段
        /// </summary>
        public static string CollectionName = "OperationLog";

        /// <summary>
        ///     数据主键前缀
        /// </summary>
        public override string GetPrefix()
        {
            return string.Empty;
        }

        /// <summary>
        ///     数据主键前缀静态字段
        /// </summary>
        public static string Prefix = string.Empty;

        /// <summary>
        ///     Mvc画面的标题
        /// </summary>
        [BsonIgnore]
        public static string MvcTitle = "操作日志";

        public static void Log(string ownerId, string user, string action, string target, string comment)
        {
            var info = new OperationLog
            {
                User = user,
                Action = action,
                Target = target,
                Comment = comment
            };
            info.OwnerId = ownerId;
            //1.OwnerTable.GetCode 默认是Bussinsee数据库
            //2.Code可能太多，超过99999
            //info.Code = OwnerTable.GetCode(info);
            MongoDbRepositoryLog.InsertLogRec(info, CollectionName);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="CollectionName"></param>
        /// <param name="ownerId"></param>
        /// <returns></returns>
        public static List<OperationLog> GetLogRecListByOwnerId(string ownerId)
        {
            var sortArgs = new Sort.SortArg[1];
            sortArgs[0] = new Sort.SortArg
            {
                FieldName = "CreateDateTime",
                SortType = Sort.SortType.Descending,
                SortOrder = 1
            };
            Action<MongoCursor> setCursor = x => { x.SetLimit(50).SetSortOrder(Sort.GetSortBuilder(sortArgs)); };
            return MongoDbRepository.GetRecList<OperationLog>(OwnerTableOperator.OwnerIdQuery(ownerId), setCursor, MongoDbRepositoryLog.LoggerDatabaseName);
        }
    }

}