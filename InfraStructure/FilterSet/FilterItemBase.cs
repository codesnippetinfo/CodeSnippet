using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;

namespace InfraStructure.FilterSet
{
    /// <summary>
    ///     过滤项目
    /// </summary>
    /// <remarks>
    /// 为了能够正确的序列化，
    /// 这里需要注册一下抽象父类的具体子类
    /// </remarks>
    [BsonKnownTypes(typeof(FilterItemBoolean),
                    typeof(FilterItemDateFromTo),
                    typeof(FilterItemDateInDays),
                    typeof(FilterItemList),
                    typeof(FilterItemWithGradeList))]
    public abstract class FilterItemBase
    {
        /// <summary>
        ///     字段表示名称
        /// </summary>
        /// <remarks>SetDisplayName@FilterSetCenter</remarks>
        [BsonIgnore]
        public string DisplayName { set; get; }
        /// <summary>
        ///     字段查询名称
        /// </summary>
        public string FieldName { get; set; }
        /// <summary>
        ///     不限
        /// </summary>
        public bool IsActive { set; get; }
        /// <summary>
        ///     字段元数据类型
        /// </summary>
        public string FieldType { set; get; }

   
        /// <summary>
        /// 获得数据集名称
        /// </summary>
        /// <returns></returns>
        public string FieldShortType()
        {
            var fieldTypeArray = FieldType.Split(".".ToCharArray());
            return fieldTypeArray[fieldTypeArray.Length - 1];
        }
        /// <summary>
        ///     查询
        /// </summary>
        /// <returns></returns>
        public abstract IMongoQuery GetQuery();
    }
}
