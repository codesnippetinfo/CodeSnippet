namespace InfraStructure.DataBase
{
    public static class SqlGererator
    {
        //使用Sql缓存进行操作
        public static string GetInsertRecSql<T>(T obj, string createUser = MongoDbRepository.UserSystem) where T : EntityBase
        {
            var sql = string.Empty;
            var typeName = typeof(T).FullName;
            //先从缓存中取得Sql
            var sqlKey = typeName + "-Insert";

            return sql;
        }
    }
}
