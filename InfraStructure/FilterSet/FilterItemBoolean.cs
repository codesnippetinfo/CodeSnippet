using MongoDB.Driver;
using MongoDB.Driver.Builders;

namespace InfraStructure.FilterSet
{
    /// <summary>
    /// 布尔型的过滤器
    /// </summary>
    public class FilterItemBoolean : FilterItemBase
    {
        /// <summary>
        /// 是否
        /// </summary>
        public bool YesOrNo;
        public FilterItemBoolean(string fieldName)
        {
            FieldName = fieldName;
            IsActive = false;
        }
        /// <summary>
        /// 获得Query
        /// </summary>
        /// <returns></returns>
        public override IMongoQuery GetQuery()
        {
            return Query.EQ(FieldName, YesOrNo);
        }
    }
}
