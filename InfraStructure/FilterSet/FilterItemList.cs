using System.Collections.Generic;
using MongoDB.Driver;
using MongoDB.Driver.Builders;

namespace InfraStructure.FilterSet
{
    public class FilterItemList : FilterItemBase
    {
        public FilterItemList(string fieldName)
        {
            FieldName = fieldName;
            Itemlist = new List<string>();
            IsActive = false;
            JoinWithAnd = false;
        }

        /// <summary>
        ///     是否用 And 连接
        ///     True：And False：Or
        /// </summary>
        public bool JoinWithAnd { set; get; }
        /// <summary>
        ///     是否为枚举集合
        /// </summary>
        public bool AsEnum { set; get; }

        /// <summary>
        ///     是否为带有目录的CatalogMasterTable
        /// </summary>
        public bool IsCatalog { set; get; }

        /// <summary>
        ///     过滤项目
        /// </summary>
        public List<string> Itemlist { set; get; }
        /// <summary>
        ///     是否为多选项目
        ///     True：原来该项目为多选项目
        ///     False:原来该项目为单选项目，这里 JoinWithAnd 必须为 False
        /// </summary>
        public bool IsOrMode { set; get; }
        /// <summary>
        ///     获得查询
        /// </summary>
        /// <returns></returns>
        public override IMongoQuery GetQuery()
        {
            IMongoQuery filterItemQuery;
            var queries = new IMongoQuery[Itemlist.Count];
            for (var i = 0; i < Itemlist.Count; i++)
            {
                if (AsEnum)
                {
                    //枚举的时候，这里必须转化为数字
                    queries[i] = Query.EQ(FieldName, int.Parse(Itemlist[i]));
                }
                else
                {
                    queries[i] = Query.EQ(FieldName, Itemlist[i]);
                }
            }
            if (JoinWithAnd)
            {
                filterItemQuery = Query.And(queries);
            }
            else
            {
                filterItemQuery = Query.Or(queries);
            }
            return filterItemQuery;
        }

    }
}
