using InfraStructure.DataBase;
using InfraStructure.Table;
using MongoDB.Bson.Serialization.Attributes;

namespace InfraStructure.Log
{
    public class InfoLog : OwnerTable
    {
        /// <summary>
        /// 用户
        /// </summary>
        public string User { set; get; }
        /// <summary>
        /// 行为
        /// </summary>
        public string Controller { set; get; }
        /// <summary>
        /// 行为
        /// </summary>
        public string Action { set; get; }
        /// <summary>
        /// 备注
        /// </summary>
        public string Comment { set; get; }

        /// <summary>
        ///     数据集名称
        /// </summary>
        public override string GetCollectionName()
        {
            return "LogInfo";
        }

        /// <summary>
        ///     数据集名称静态字段
        /// </summary>
        public static string CollectionName = "LogInfo";


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
        public static string MvcTitle = "信息日志";

        public static void Log(string user, string action, string controller, string comment = "")
        {
            var info = new InfoLog
            {
                User = user,
                Action = action,
                Controller = controller,
                Comment = comment
            };
            MongoDbRepositoryLog.InsertLogRec(info);
        }
    }
}