using System;

namespace InfraStructure.DataBase
{
    /// <summary>
    ///     数据库帮助
    /// </summary>
    public interface IDbHelper
    {
        /// <summary>
        /// 插入实体
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        string InsertRec<T>(T obj) where T : EntityBase;
    }
    /// <summary>
    ///     MONGO
    /// </summary>
    public class MongoDbHelper : IDbHelper
    {
        public string InsertRec<T>(T obj) where T : EntityBase
        {
            return MongoDbRepository.InsertRec(obj);
        }
    }
    /// <summary>
    ///     MySQL
    /// </summary>
    public class MySqlDbHelper : IDbHelper
    {
        public string InsertRec<T>(T obj) where T : EntityBase
        {
            throw new NotImplementedException();
        }
    }
}
