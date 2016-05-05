using System;
using MongoDB.Bson.Serialization.Attributes;

namespace InfraStructure.DataBase
{
    /// <summary>
    ///     实体
    ///     没有物理删除,所以自增SN是安全的
    /// </summary>
    public abstract class EntityBase
    {
        /// <summary>
        ///     创建时间
        /// </summary>
        [BsonDateTimeOptions(Kind = DateTimeKind.Local)] public DateTime CreateDateTime;

        /// <summary>
        ///     创建者
        ///     [这里可以是用户名，亦可是账号]
        /// </summary>
        public string CreateUser;

        /// <summary>
        ///     删除标记
        /// </summary>
        public bool IsDel;

        /// <summary>
        ///     序列号
        /// </summary>
        [BsonId] public string Sn;

        /// <summary>
        ///     更新时间
        /// </summary>
        [BsonDateTimeOptions(Kind = DateTimeKind.Local)] public DateTime UpdateDateTime;

        /// <summary>
        ///     更新者
        /// </summary>
        public string UpdateUser;

        /// <summary>
        ///     获得表名称
        /// </summary>
        /// <returns></returns>
        public abstract string GetCollectionName();

        /// <summary>
        ///     获得主键前缀
        /// </summary>
        /// <returns></returns>
        public abstract string GetPrefix();

        /// <summary>
        /// 序列号格式
        /// </summary>
        public const string SnFormat = "D8";

    }
}