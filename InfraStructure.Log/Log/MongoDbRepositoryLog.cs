using InfraStructure.DataBase;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;

namespace InfraStructure.Log
{
    public static class MongoDbRepositoryLog
    {
        /// <summary>
        ///     服务器
        /// </summary>
        private static MongoServer _innerServer;
        /// <summary>
        ///     链接字符串
        /// </summary>
        private static readonly string Connectionstring = @"mongodb://localhost:";

        /// <summary>
        ///     日志数据库
        /// </summary>
        public static MongoDatabase InnerLoggerDatabase;
        /// <summary>
        ///     日志数据库名称
        /// </summary>
        public const string LoggerDatabaseName = "Logger";
        /// <summary>
        ///     初始化MongoDB
        /// </summary>
        /// <param name="port"></param>
        /// <returns></returns>
        public static bool Init(string port = "28030")
        {
            try
            {
                if (_innerServer != null) return true;
                _innerServer = new MongoClient(Connectionstring + port).GetServer();
                _innerServer.Connect();
                InnerLoggerDatabase = _innerServer.GetDatabase(LoggerDatabaseName);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// 插入日志记录
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        public static string InsertLogRec<T>(T obj) where T : EntityBase
        {
            MongoCollection targetCollection = InnerLoggerDatabase.GetCollection(DateTime.Now.ToString("yyyyMMdd"));
            MongoDbRepository.FillBaseInfo(obj);
            obj.Sn = (targetCollection.Count() + 1).ToString(EntityBase.SnFormat);
            targetCollection.Insert(obj);
            return obj.Sn;
        }

        /// <summary>
        /// 插入异常和操作记录
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <param name="collectionName"></param>
        public static string InsertLogRec<T>(T obj, string collectionName) where T : EntityBase
        {
            MongoCollection targetCollection = InnerLoggerDatabase.GetCollection(collectionName);
            MongoDbRepository.FillBaseInfo(obj);
            obj.Sn = (targetCollection.Count() + 1).ToString(EntityBase.SnFormat);
            targetCollection.Insert(obj);
            return obj.Sn;
        }

        /// <summary>
        /// 查询异常和操作记录
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <param name="collectionName"></param>
        public static List<T> GetLogRec<T>() where T : EntityBase, new()
        {
            MongoCollection targetCollection = InnerLoggerDatabase.GetCollection(new T().GetCollectionName());
            var cursor = targetCollection.FindAs<T>(null);
            return cursor.AsEnumerable().ToList();
        }
    }
}
