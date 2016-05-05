using InfraStructure.DataBase;
using InfraStructure.Table;
using System.Collections.Generic;

namespace InfraStructure.Log
{
    public class ExceptionLog : OwnerTable
    {
        /// <summary>
        /// 用户
        /// </summary>
        public string User { set; get; }
        /// <summary>
        /// 控制器
        /// </summary>
        public string Controller { set; get; }
        /// <summary>
        /// 行为
        /// </summary>
        public string Action { set; get; }
        /// <summary>
        /// 备注
        /// </summary>
        public string SourceTrace { set; get; }

        /// <summary>
        ///     数据集名称
        /// </summary>
        public override string GetCollectionName()
        {
            return "ExceptionLog";
        }

        /// <summary>
        ///     数据集名称静态字段
        /// </summary>
        public static string CollectionName = "ExceptionLog";


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
        public static string MvcTitle = "异常日志";

        /// <summary>
        ///     日志
        /// </summary>
        /// <param name="user"></param>
        /// <param name="action"></param>
        /// <param name="controller"></param>
        /// <param name="sourceTrace"></param>
        public static void Log(string user, string action, string controller, string sourceTrace)
        {
            var info = new ExceptionLog
            {
                User = user,
                Action = action,
                Controller = controller,
                SourceTrace = sourceTrace
            };
            MongoDbRepositoryLog.InsertLogRec(info, CollectionName);
        }
        /// <summary>
        /// 获得LOG
        /// </summary>
        /// <returns></returns>
        public static List<ExceptionLog> GetLog()
        {
            return MongoDbRepositoryLog.GetLogRec<ExceptionLog>();
        }

    }

}