using System;
using System.Collections.Generic;
using System.Linq;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;

namespace InfraStructure.DataBase
{
    /// <summary>
    ///     Summary description for MongoDBRepository
    /// </summary>
    public static partial class MongoDbRepository
    {

        #region Generic

        /// <summary>
        ///     系统用户
        /// </summary>
        public const string UserSystem = "SYSTEM";

        /// <summary>
        ///     服务器
        /// </summary>
        private static MongoServer _innerServer;

        /// <summary>
        ///     链接字符串
        /// </summary>
        private static readonly string Connectionstring = @"mongodb://localhost:";

        /// <summary>
        ///     仅查询未删除的记录
        /// </summary>
        private static readonly IMongoQuery NotDeletequery = Query.EQ(nameof(EntityBase.IsDel), false);

        /// <summary>
        ///     更新时候的缓存更新的注入操作
        /// </summary>
        public static Action<EntityBase> UpdateCache;

        /// <summary>
        ///     数据库列表
        /// </summary>
        public static Dictionary<string, MongoDatabase> DatabaseList = new Dictionary<string, MongoDatabase>();

        /// <summary>
        ///     默认数据库名称
        /// </summary>
        private static string _defaultDatabaseName = string.Empty;
        /// <summary>
        ///     默认数据库
        /// </summary>
        public static MongoDatabase InnerDefaultDatabase;

        /// <summary>
        ///     MongoKeyField
        /// </summary>
        public const string MongoKeyField = "_id";

        /// <summary>
        ///     初始化MongoDB
        /// </summary>
        /// <param name="dbList">除去Logger以外</param>
        /// <param name="defaultDbName"></param>
        /// <param name="port"></param>
        /// <returns></returns>
        public static bool Init(string[] dbList, string defaultDbName, string port = "28030")
        {
            try
            {
                _innerServer = new MongoClient(Connectionstring + port).GetServer();
                _innerServer.Connect();
                for (var i = 0; i < dbList.Length; i++)
                {
                    DatabaseList.Add(dbList[i], _innerServer.GetDatabase(dbList[i]));
                }
                //DatabaseList.Add(LoggerDatabaseName, _innerServer.GetDatabase(LoggerDatabaseName));
                _defaultDatabaseName = defaultDbName;
                //InnerLoggerDatabase = GetDatabaseByType(LoggerDatabaseName);
                InnerDefaultDatabase = GetDatabaseByType(defaultDbName);
                //http://mongodb.github.io/mongo-csharp-driver/1.10/serialization/
                var pack = new ConventionPack();
                pack.Add(new IgnoreExtraElementsConvention(true));
                pack.Add(new IgnoreIfNullConvention(true));
                ConventionRegistry.Register("CustomElementsConvention", pack, t => { return true; });
                //DateTime Localize    
                BsonSerializer.RegisterSerializer(typeof(DateTime), new DateTimeSerializer(DateTimeKind.Local));
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// 获得数据库
        /// </summary>
        /// <param name="database"></param>
        /// <returns></returns>
        public static MongoDatabase GetDatabaseByType(string database)
        {
            MongoDatabase innerDb = null;
            if (DatabaseList.ContainsKey(database))
            {
                innerDb = DatabaseList[database];
            }
            return innerDb;
        }

        /// <summary>
        /// 数据集存在检查
        /// </summary>
        /// <param name="collectionName"></param>
        /// <param name="database"></param>
        /// <returns></returns>
        public static bool ExistCollection(string collectionName, string database = "")
        {
            if (string.IsNullOrEmpty(database)) database = _defaultDatabaseName;
            return GetDatabaseByType(database).CollectionExists(collectionName);
        }
        /// <summary>
        /// 删除数据集
        /// </summary>
        /// <param name="collectionName"></param>
        /// <param name="database"></param>
        public static void DrapCollection(string collectionName, string database = "")
        {
            if (string.IsNullOrEmpty(database)) database = _defaultDatabaseName;
            if (ExistCollection(collectionName, database))
            {
                GetDatabaseByType(database).DropCollection(collectionName);
            }
        }
        /// <summary>
        ///     设置索引
        /// </summary>
        /// <param name="collectionName"></param>
        /// <param name="FieldName"></param>
        /// <param name="database"></param>
        public static void SetIndex(string collectionName, string FieldName, string database = "")
        {
            if (string.IsNullOrEmpty(database)) database = _defaultDatabaseName;
            MongoCollection col = GetDatabaseByType(database).GetCollection(collectionName);
            if (col.IndexExistsByName(FieldName))
            {
                return;
            }
            var option = new IndexOptionsBuilder();
            option.SetName(FieldName);
            var indexkeys = new IndexKeysBuilder();
            indexkeys.Ascending(new string[] { FieldName });
            col.CreateIndex(indexkeys, option);

        }

        /// <summary>
        ///     设置Text索引
        /// </summary>
        /// <param name="collectionName"></param>
        /// <param name="FieldName"></param>
        /// <param name="database"></param>
        public static void SetTextIndex(string collectionName, string FieldName, string database = "")
        {
            if (string.IsNullOrEmpty(database)) database = _defaultDatabaseName;
            MongoCollection col = GetDatabaseByType(database).GetCollection(collectionName);
            if (col.IndexExistsByName(FieldName))
            {
                return;
            }
            var option = new IndexOptionsBuilder();
            option.SetName(FieldName);
            var indexkeys = new IndexKeysBuilder();
            indexkeys.Text(new string[] { FieldName });
            col.CreateIndex(indexkeys, option);
        }

        /// <summary>
        ///     设定数据缓存时间(以创建时间为基础)
        /// </summary>
        /// <param name="collectionName"></param>
        /// <param name="ExpiresMinute"></param>
        /// <param name="database"></param>
        public static void SetCacheTime(string collectionName, int ExpiresMinute, string database = "")
        {
            if (string.IsNullOrEmpty(database)) database = _defaultDatabaseName;
            MongoCollection col = GetDatabaseByType(database).GetCollection(collectionName);
            if (col.IndexExistsByName("Cache"))
            {
                col.DropIndexByName("Cache");
            }
            var option = new IndexOptionsBuilder();
            option.SetTimeToLive(new TimeSpan(0, ExpiresMinute, 0));
            option.SetName("Cache");
            var indexkeys = new IndexKeysBuilder();
            indexkeys.Ascending(new string[] { nameof(EntityBase.CreateDateTime) });
            col.CreateIndex(indexkeys, option);
        }

        /// <summary>
        ///     填充事件资料
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="createUser"></param>
        public static void FillBaseInfo(EntityBase obj, string createUser = UserSystem)
        {
            obj.CreateDateTime = DateTime.Now;
            obj.UpdateDateTime = DateTime.Now;
            obj.CreateUser = createUser;
            obj.UpdateUser = createUser;
            obj.IsDel = false;
        }
        /// <summary>
        ///     填充事件资料
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="createUser"></param>
        private static void FillBaseInfo(CacheEntityBase obj, string createUser = UserSystem)
        {
            obj.CreateDateTime = DateTime.Now;
            obj.UpdateDateTime = DateTime.Now;
            obj.CreateUser = createUser;
            obj.UpdateUser = createUser;
            obj.IsDel = false;
        }
        #endregion

        #region Schema

        /// <summary>
        /// 移除字段，备份为CollectionName_Legacy
        /// </summary>
        /// <param name="collectionName"></param>
        /// <param name="fieldName"></param>
        /// <param name="isBackUp">是否留存备份</param>
        public static void RemoveField(string collectionName, string fieldName, bool isBackUp = true)
        {
            MongoCollection targetCollection = InnerDefaultDatabase.GetCollection(collectionName);
            var docList = targetCollection.FindAllAs<BsonDocument>().ToList();
            foreach (var item in docList)
            {
                item.Remove(fieldName);
            }
            var legacy = collectionName + "_Legacy_" + DateTime.Now.ToString("yyyyMMddHHmmss");
            InnerDefaultDatabase.RenameCollection(collectionName, legacy);
            targetCollection = InnerDefaultDatabase.GetCollection(collectionName);
            targetCollection.InsertBatch(docList);
            if (!isBackUp)
            {
                InnerDefaultDatabase.DropCollection(legacy);
            }
        }

        /// <summary>
        /// 修改字段，备份为CollectionName_Legacy
        /// </summary>
        /// <param name="collectionName">数据集名称</param>
        /// <param name="oldFieldName">旧的字段名称</param>
        /// <param name="newFieldName">新的字段名称</param>
        /// <param name="isBackUp">是否留存备份</param>
        public static void ChangeField(string collectionName, string oldFieldName, string newFieldName, bool isBackUp = true)
        {
            MongoCollection targetCollection = InnerDefaultDatabase.GetCollection(collectionName);
            var docList = targetCollection.FindAllAs<BsonDocument>().ToList();
            foreach (var item in docList)
            {
                var value = item.GetValue(oldFieldName);
                item.Remove(oldFieldName);
                item.Add(new BsonElement(newFieldName, value));
            }
            var legacy = collectionName + "_Legacy_" + DateTime.Now.ToString("yyyyMMddHHmmss");
            InnerDefaultDatabase.RenameCollection(collectionName, legacy);
            targetCollection = InnerDefaultDatabase.GetCollection(collectionName);
            targetCollection.InsertBatch(docList);
            if (!isBackUp)
            {
                InnerDefaultDatabase.DropCollection(legacy);
            }
        }

        /// <summary>
        /// 新增一个元素
        /// </summary>
        /// <param name="collectionName">数据集名称</param>
        /// <param name="newFieldName">新元素名称</param>
        /// <param name="getValue">获得该元素值的方法</param>
        /// <param name="isBackUp">是否留存备份</param>
        public static void AddField(string collectionName, string newFieldName, Func<BsonDocument, BsonValue> getValue, bool isBackUp = true)
        {
            MongoCollection targetCollection = InnerDefaultDatabase.GetCollection(collectionName);
            var docList = targetCollection.FindAllAs<BsonDocument>().ToList();
            foreach (var item in docList)
            {
                item.Add(new BsonElement(newFieldName, getValue(item)));
            }
            var legacy = collectionName + "_Legacy_" + DateTime.Now.ToString("yyyyMMddHHmmss");
            InnerDefaultDatabase.RenameCollection(collectionName, legacy);
            targetCollection = InnerDefaultDatabase.GetCollection(collectionName);
            targetCollection.InsertBatch(docList);
            if (!isBackUp)
            {
                InnerDefaultDatabase.DropCollection(legacy);
            }
        }

        #endregion

        #region Insert

        /// <summary>
        /// 线程控制
        /// </summary>
        private static readonly object objLock = new object();

        /// <summary>
        ///     插入记录
        /// </summary>
        /// <typeparam name="T">继承与EntityBase的类</typeparam>
        /// <param name="obj"></param>
        /// <param name="createUser"></param>
        public static string InsertRec<T>(T obj, string createUser = UserSystem) where T : EntityBase
        {
            MongoCollection targetCollection = InnerDefaultDatabase.GetCollection(obj.GetCollectionName());
            FillBaseInfo(obj, createUser);
            lock (objLock)
            {
                obj.Sn = obj.GetPrefix() + (targetCollection.Count() + 1).ToString(EntityBase.SnFormat);
                targetCollection.Insert(obj);
            }
            return obj.Sn;
        }
        /// <summary>
        ///     插入记录
        /// </summary>
        /// <typeparam name="T">继承与EntityBase的类</typeparam>
        /// <param name="obj"></param>
        /// <param name="createUser"></param>
        public static ObjectId InsertCacheRec<T>(T obj, string createUser = UserSystem) where T : CacheEntityBase
        {
            MongoCollection targetCollection = InnerDefaultDatabase.GetCollection(obj.GetCollectionName());
            FillBaseInfo(obj, createUser);
            lock (objLock)
            {
                targetCollection.Insert(obj);
            }
            return obj.Sn;
        }
        /// <summary>
        ///     插入记录
        /// </summary>
        /// <typeparam name="T">继承与EntityBase的类</typeparam>
        /// <param name="obj"></param>
        /// <param name="createUser"></param>
        public static ObjectId InsertCacheRec<T>(string CollectionName,T obj, string createUser = UserSystem) where T : CacheEntityBase
        {
            MongoCollection targetCollection = InnerDefaultDatabase.GetCollection(CollectionName);
            FillBaseInfo(obj, createUser);
            lock (objLock)
            {
                targetCollection.Insert(obj);
            }
            return obj.Sn;
        }
        /// <summary>
        ///     插入一些记录
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <param name="createUser"></param>
        /// <returns></returns>
        public static List<string> InsertRecList<T>(List<T> obj, string createUser = UserSystem) where T : EntityBase
        {
            MongoCollection targetCollection = InnerDefaultDatabase.GetCollection(obj[0].GetCollectionName());
            string strPrefix = obj[0].GetPrefix();
            var SnList = new List<string>();
            //如果同时发生这个操作,在异步的时候会发生主键重复的问题
            foreach (var item in obj)
            {
                FillBaseInfo(item, createUser);
                lock (objLock)
                {
                    item.Sn = strPrefix + (targetCollection.Count() + 1).ToString(EntityBase.SnFormat);
                    targetCollection.Insert(item);
                }
                SnList.Add(item.Sn);
            }
            return SnList;
        }

        /// <summary>
        ///     插入记录
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <param name="databaseType"></param>
        /// <param name="createUser"></param>
        public static string InsertRecByDatabase<T>(T obj, string databaseType, string createUser = UserSystem) where T : EntityBase
        {
            var innerDb = GetDatabaseByType(databaseType);
            MongoCollection targetCollection = innerDb.GetCollection(obj.GetCollectionName());
            FillBaseInfo(obj, createUser);
            obj.Sn = obj.GetPrefix() + (targetCollection.Count() + 1).ToString(EntityBase.SnFormat);
            targetCollection.Insert(obj);
            return obj.Sn;
        }

        /// <summary>
        /// 插入记录
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <param name="collectionName"></param>
        /// <param name="createUser"></param>
        public static string InsertRec<T>(T obj, string collectionName, string createUser = UserSystem) where T : EntityBase
        {
            MongoCollection targetCollection = InnerDefaultDatabase.GetCollection(collectionName);
            FillBaseInfo(obj, createUser);
            obj.Sn = (targetCollection.Count() + 1).ToString(EntityBase.SnFormat);
            targetCollection.Insert(obj);
            return obj.Sn;
        }


        #endregion

        #region Update

        /// <summary>
        ///     更新记录
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <param name="updateUser"></param>
        public static void UpdateRec<T>(T obj, string updateUser = UserSystem) where T : EntityBase
        {
            MongoCollection targetCollection = InnerDefaultDatabase.GetCollection(obj.GetCollectionName());
            obj.UpdateDateTime = DateTime.Now;
            obj.UpdateUser = updateUser;
            //Remove Old
            targetCollection.Remove(Query.EQ(MongoKeyField, obj.Sn), WriteConcern.Acknowledged);
            //Insert New
            targetCollection.Insert(obj);
            //UpdateCache If Needed
            if (UpdateCache != null) UpdateCache(obj);
        }

        /// <summary>
        /// 更新数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <param name="collectionName"></param>
        /// <param name="updateUser"></param>
        public static void UpdateRec<T>(T obj, string collectionName, string updateUser = UserSystem) where T : EntityBase
        {
            MongoCollection targetCollection = InnerDefaultDatabase.GetCollection(collectionName);
            obj.UpdateDateTime = DateTime.Now;
            obj.UpdateUser = updateUser;
            //Remove Old
            targetCollection.Remove(Query.EQ(MongoKeyField, obj.Sn), WriteConcern.Acknowledged);
            //Insert New
            targetCollection.Insert(obj);
            //UpdateCache If Needed
            if (UpdateCache != null) UpdateCache(obj);
        }

        /// <summary>
        /// 更新文档
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="collectionName"></param>
        /// <param name="updateUser"></param>
        public static void UpdateDocument(BsonDocument obj, string collectionName, string updateUser = UserSystem)
        {
            MongoCollection targetCollection = InnerDefaultDatabase.GetCollection(collectionName);
            //Remove Old
            targetCollection.Remove(Query.EQ(MongoKeyField, obj.GetElement(MongoKeyField).Value), WriteConcern.Acknowledged);
            //Insert New
            targetCollection.Insert(obj);
        }

        /// <summary>
        ///     更新记录
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="query"></param>
        /// <param name="Field"></param>
        /// <param name="value"></param>
        public static void UpdateRec<T>(T obj, string Field, BsonValue value) where T : EntityBase, new()
        {
            MongoCollection targetCollection = InnerDefaultDatabase.GetCollection(new T().GetCollectionName());
            var update = new UpdateBuilder();
            update.Set(Field, value);
            update.Set(nameof(EntityBase.UpdateDateTime), DateTime.Now);
            update.Set(nameof(EntityBase.UpdateUser), UserSystem);
            targetCollection.Update(Query.EQ(MongoKeyField, obj.Sn), update);
        }
        #endregion

        #region Delete

        /// <summary>
        ///     将所有符合条件的记录逻辑删除
        /// </summary>
        /// <param name="collectionName"></param>
        /// <param name="query"></param>
        public static void DeleteRecByCondition(string collectionName, IMongoQuery query,
            string deleteUser = UserSystem)
        {
            MongoCollection targetCollection = InnerDefaultDatabase.GetCollection(collectionName);
            var update = new UpdateBuilder();
            update.Set(nameof(EntityBase.IsDel), true);
            update.Set(nameof(EntityBase.UpdateDateTime), DateTime.Now);
            update.Set(nameof(EntityBase.UpdateUser), UserSystem);
            targetCollection.Update(query, update, UpdateFlags.Multi);
        }

        /// <summary>
        ///     逻辑删除一条记录
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public static void DeleteRec<T>(T obj, string deleteUser = UserSystem) where T : EntityBase
        {
            var query = Query.EQ(MongoKeyField, obj.Sn);
            DeleteRecByCondition(obj.GetCollectionName(), query, deleteUser);
        }

        /// <summary>
        ///     逻辑删除指定记录
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <param name="collectionName"></param>
        /// <param name="deleteUser"></param>
        public static void DeleteRec<T>(T obj, string collectionName, string deleteUser = UserSystem) where T : EntityBase
        {
            var query = Query.EQ(MongoKeyField, obj.Sn);
            DeleteRecByCondition(collectionName, query, deleteUser);
        }

        /// <summary>
        ///     逻辑删除指定记录
        /// </summary>
        /// <param name="sn"></param>
        /// <param name="collectionName"></param>
        /// <param name="deleteUser"></param>
        [Obsolete]
        public static void DeleteRec<T>(string sn, string collectionName, string deleteUser = UserSystem)
            where T : EntityBase
        {
            var query = Query.EQ(MongoKeyField, sn);
            DeleteRecByCondition(collectionName, query, deleteUser);
        }

        /// <summary>
        ///     逻辑删除指定记录
        /// </summary>
        /// <param name="id"></param>
        /// <param name="CollectionName"></param>
        /// <param name="deleteUser"></param>
        public static void DeleteRec<T>(string sn, string deleteUser = UserSystem) where T : EntityBase, new()
        {
            var query = Query.EQ(MongoKeyField, sn);
            DeleteRecByCondition(new T().GetCollectionName(), query, deleteUser);
        }
        /// <summary>
        ///     物理删除数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        public static void DeleteRecPhysical<T>(T obj) where T : CacheEntityBase, new()
        {
            MongoCollection targetCollection = InnerDefaultDatabase.GetCollection((new T()).GetCollectionName());
            var query = Query.EQ(MongoKeyField, obj.Sn);
            targetCollection.Remove(query);
        }

        /// <summary>
        ///     物理删除一个数据集中所有记录
        /// </summary>
        /// <param name="collectionName"></param>
        public static void DeleteAllRecPhysical(string collectionName)
        {
            MongoCollection targetCollection = InnerDefaultDatabase.GetCollection(collectionName);
            targetCollection.RemoveAll();
        }

        /// <summary>
        ///     逻辑恢复记录
        /// </summary>
        /// <param name="collectionName"></param>
        /// <param name="query"></param>
        [Obsolete]
        public static void RecoverRec(string collectionName, IMongoQuery query)
        {
            MongoCollection targetCollection = InnerDefaultDatabase.GetCollection(collectionName);
            var update = new UpdateBuilder();
            update.Set(nameof(EntityBase.IsDel), false);
            update.Set(nameof(EntityBase.UpdateDateTime), DateTime.Now);
            update.Set(nameof(EntityBase.UpdateUser), UserSystem);
            targetCollection.Update(query, update);
        }

        /// <summary>
        ///     逻辑恢复记录
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="query"></param>
        public static void RecoverRec<T>(IMongoQuery query) where T : EntityBase, new()
        {
            MongoCollection targetCollection = InnerDefaultDatabase.GetCollection(new T().GetCollectionName());
            var update = new UpdateBuilder();
            update.Set(nameof(EntityBase.IsDel), false);
            update.Set(nameof(EntityBase.UpdateDateTime), DateTime.Now);
            update.Set(nameof(EntityBase.UpdateUser), UserSystem);
            targetCollection.Update(query, update);
        }
        #endregion

        #region Query
        /// <summary>
        ///     获得逻辑删除记录
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="query"></param>
        /// <returns></returns>
        public static List<T> GetDeleteRecList<T>(IMongoQuery query) where T : EntityBase, new()
        {
            MongoCollection targetCollection = InnerDefaultDatabase.GetCollection(new T().GetCollectionName());
            var cursor = targetCollection.FindAs<T>(Query.And(Query.EQ(nameof(EntityBase.IsDel), true), query));
            return cursor.ToList();
        }

        /// <summary>
        ///     查询一条记录
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collectionName"></param>
        /// <param name="sn"></param>
        /// <returns></returns>
        [Obsolete]
        public static T GetRecBySN<T>(string collectionName, string sn) where T : EntityBase
        {
            MongoCollection targetCollection = InnerDefaultDatabase.GetCollection(collectionName);
            var query = Query.EQ(MongoKeyField, sn);
            query = Query.And(query, NotDeletequery);
            var rec = (T)targetCollection.FindOneAs(typeof(T), query);
            return rec;
        }
        /// <summary>
        ///     查询一条记录
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sn"></param>
        /// <returns></returns>
        public static T GetRecBySN<T>(string sn) where T : EntityBase, new()
        {
            MongoCollection targetCollection = InnerDefaultDatabase.GetCollection(new T().GetCollectionName());
            var query = Query.EQ(MongoKeyField, sn);
            query = Query.And(query, NotDeletequery);
            var rec = (T)targetCollection.FindOneAs(typeof(T), query);
            return rec;
        }

        /// <summary>
        ///     查询一条记录（泛型版）
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        [Obsolete]
        public static T GetFirstRec<T>(string collectionName, IMongoQuery query) where T : EntityBase
        {
            MongoCollection targetCollection = InnerDefaultDatabase.GetCollection(collectionName);
            query = Query.And(query, NotDeletequery);
            return (T)targetCollection.FindOneAs(typeof(T), query);
        }
        /// <summary>
        ///     查询一条记录（泛型版）
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="query"></param>
        /// <returns></returns>
        public static T GetFirstRec<T>(IMongoQuery query) where T : EntityBase, new()
        {
            MongoCollection targetCollection = InnerDefaultDatabase.GetCollection(new T().GetCollectionName());
            query = Query.And(query, NotDeletequery);
            return (T)targetCollection.FindOneAs(typeof(T), query);
        }
        /// <summary>
        ///     查询一条记录（泛型版）
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="query"></param>
        /// <returns></returns>
        public static T GetFirstCacheRec<T>(IMongoQuery query) where T : CacheEntityBase, new()
        {
            MongoCollection targetCollection = InnerDefaultDatabase.GetCollection(new T().GetCollectionName());
            query = Query.And(query, NotDeletequery);
            return (T)targetCollection.FindOneAs(typeof(T), query);
        }
        /// <summary>
        ///     查询一条记录
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static BsonDocument GetFirstRec(string collectionName, IMongoQuery query)
        {
            MongoCollection targetCollection = InnerDefaultDatabase.GetCollection(collectionName);
            query = Query.And(query, NotDeletequery);
            return targetCollection.FindOneAs<BsonDocument>(query);
        }

        /// <summary>
        ///     获得指定条件的数据集
        /// </summary>
        /// <typeparam name="T">泛型</typeparam>
        /// <param name="collectionName">数据集名称</param>
        /// <param name="query">检索条件（Nullable）</param>
        /// <param name="setCursor">游标限制</param>
        /// <returns></returns>
        [Obsolete]
        public static List<T> GetRecList<T>(string collectionName, IMongoQuery query = null,
            Action<MongoCursor> setCursor = null, string databaseType = "")
        {
            if (string.IsNullOrEmpty(databaseType)) databaseType = _defaultDatabaseName;
            var innerDb = GetDatabaseByType(databaseType);
            MongoCollection targetCollection = innerDb.GetCollection(collectionName);
            query = query == null ? NotDeletequery : Query.And(query, NotDeletequery);
            var cursor = targetCollection.FindAs<T>(query);
            setCursor?.Invoke(cursor);
            return cursor.AsEnumerable().ToList();
        }

        /// <summary>
        ///     获得指定条件的数据集
        /// </summary>
        /// <typeparam name="T">泛型</typeparam>
        /// <param name="CollectionName">数据集名称</param>
        /// <param name="query">检索条件（Nullable）</param>
        /// <param name="setCursor">游标限制</param>
        /// <returns></returns>
        public static List<T> GetRecList<T>(IMongoQuery query = null, Action<MongoCursor> setCursor = null,
                                            string databaseType = "") where T : EntityBase, new()
        {
            if (string.IsNullOrEmpty(databaseType)) databaseType = _defaultDatabaseName;
            var innerDb = GetDatabaseByType(databaseType);
            MongoCollection targetCollection = innerDb.GetCollection(new T().GetCollectionName());
            query = query == null ? NotDeletequery : Query.And(query, NotDeletequery);
            var cursor = targetCollection.FindAs<T>(query);
            setCursor?.Invoke(cursor);
            return cursor.AsEnumerable().ToList();
        }


        /// <summary>
        ///     获得指定条件的数据集
        /// </summary>
        /// <typeparam name="T">泛型</typeparam>
        /// <param name="CollectionName">数据集名称</param>
        /// <param name="query">检索条件（Nullable）</param>
        /// <param name="setCursor">游标限制</param>
        /// <returns></returns>
        public static List<T> GetCacheRecList<T>(IMongoQuery query = null, Action<MongoCursor> setCursor = null,
                                            string databaseType = "") where T : CacheEntityBase, new()
        {
            if (string.IsNullOrEmpty(databaseType)) databaseType = _defaultDatabaseName;
            var innerDb = GetDatabaseByType(databaseType);
            MongoCollection targetCollection = innerDb.GetCollection(new T().GetCollectionName());
            query = query == null ? NotDeletequery : Query.And(query, NotDeletequery);
            var cursor = targetCollection.FindAs<T>(query);
            setCursor?.Invoke(cursor);
            return cursor.AsEnumerable().ToList();
        }

        /// <summary>
        ///     获得指定条件的数据集
        /// </summary>
        /// <param name="collectionName"></param>
        /// <param name="query"></param>
        /// <param name="databaseType"></param>
        /// <returns></returns>
        public static List<BsonDocument> GetRecList(string collectionName,
                                                    IMongoQuery query = null, string databaseType = "")
        {
            if (string.IsNullOrEmpty(databaseType)) databaseType = _defaultDatabaseName;
            var innerDb = GetDatabaseByType(databaseType);
            MongoCollection targetCollection = innerDb.GetCollection(collectionName);
            query = query == null ? NotDeletequery : Query.And(query, NotDeletequery);
            var cursor = targetCollection.FindAs<BsonDocument>(query);
            return cursor.ToList();
        }
        /// <summary>
        ///     获得件数
        /// </summary>
        /// <param name="collectionName"></param>
        /// <returns></returns>
        public static int GetRecordCount<T>(IMongoQuery query, bool includeDeleted = false) where T : EntityBase, new()
        {
            if (query == null) return GetRecordCount(new T().GetCollectionName(), includeDeleted);
            MongoCollection targetCollection = InnerDefaultDatabase.GetCollection(new T().GetCollectionName());
            if (!includeDeleted) query = Query.And(query, NotDeletequery);
            return (int)targetCollection.Count(query);
        }
        /// <summary>
        ///     获得件数
        /// </summary>
        /// <param name="collectionName"></param>
        /// <returns></returns>
        public static int GetRecordCount(string collectionName, IMongoQuery query, bool includeDeleted = false)
        {
            if (query == null) return GetRecordCount(collectionName, includeDeleted);
            MongoCollection targetCollection = InnerDefaultDatabase.GetCollection(collectionName);
            if (!includeDeleted) query = Query.And(query, NotDeletequery);
            return (int)targetCollection.Count(query);
        }

        /// <summary>
        ///     获得件数
        /// </summary>
        /// <param name="collectionName"></param>
        /// <returns></returns>
        public static int GetRecordCount(string collectionName, bool includeDeleted = false)
        {
            MongoCollection targetCollection = InnerDefaultDatabase.GetCollection(collectionName);
            if (includeDeleted)
            {
                return (int)targetCollection.Count();
            }
            return (int)targetCollection.Count(NotDeletequery);
        }

        #endregion

        #region Aggregate

        /// <summary>
        /// 全文检索
        /// </summary>
        /// <param name="collectionName"></param>
        /// <param name="key"></param>
        /// <param name="caseSensitive"></param>
        /// <param name="limit"></param>
        /// <returns></returns>
        public static List<BsonDocument> SearchText(string collectionName, string key, bool caseSensitive, int limit, IMongoQuery query = null)
        {
            //检索关键字
            var textSearchOption = new TextSearchOptions();
            textSearchOption.CaseSensitive = caseSensitive;
            var textSearchQuery = Query.Text(key, textSearchOption);
            if (query != null)
            {
                textSearchQuery = Query.And(textSearchQuery, query);
            }
            MongoCollection col = GetDatabaseByType(_defaultDatabaseName).GetCollection(collectionName);
            var result = col.FindAs<BsonDocument>(textSearchQuery);
            var resultDocumentList = result.SetLimit(limit).ToList();
            return resultDocumentList;
        }

        /// <summary>
        /// Distinct
        /// </summary>
        /// <param name="collectionName"></param>
        /// <param name="FieldName"></param>
        /// <param name="query"></param>
        /// <returns></returns>
        public static List<string> Distinct(string collectionName, string FieldName, IMongoQuery query = null)
        {
            MongoCollection col = GetDatabaseByType(_defaultDatabaseName).GetCollection(collectionName);
            var DistinctResult = col.Distinct(FieldName, query);
            var result = new List<string>();
            foreach (BsonValue item in DistinctResult)
            {
                result.Add(item.AsString);
            }
            return result;
        }


        /// <summary>
        /// GroupByCount
        /// </summary>
        /// <param name="collectionName"></param>
        /// <param name="FieldName"></param>
        /// <returns></returns>
        public static Dictionary<string, int> GroupCount(string collectionName, string FieldName, IMongoQuery query = null)
        {
            MongoCollection col = GetDatabaseByType(_defaultDatabaseName).GetCollection(collectionName);
            GroupArgs g = new GroupArgs();
            var groupdoc = new GroupByDocument();
            groupdoc.Add(FieldName, true);
            g.KeyFields = groupdoc;
            g.ReduceFunction = new BsonJavaScript("function(obj,prev){ prev.count++;}");
            g.Initial = new BsonDocument().Add("count", 0);
            if (query != null)
            {
                g.Query = query;
            }
            var GroupResult = col.Group(g);
            var result = new Dictionary<string, int>();
            foreach (BsonDocument item in GroupResult)
            {
                result.Add(item.GetElement(FieldName).Value.ToString(), (int)item.GetElement("count").Value.AsDouble);
            }
            return result;
        }
        #endregion
    }
}