using System.Collections.Generic;
using InfraStructure.Helper;
using MongoDB.Driver;
using MongoDB.Driver.Builders;

namespace InfraStructure.FilterSet
{
    public class FilterItemWithGradeList : FilterItemBase
    {
        public FilterItemWithGradeList(string fieldName)
        {
            FieldName = fieldName;
            Itemlist = new List<ItemWithGrade>();
            IsActive = false;
            JoinWithAnd = false;
        }

        /// <summary>
        ///     是否用 And 连接
        ///     True：And False：Or
        /// </summary>
        public bool JoinWithAnd { set; get; }
        /// <summary>
        ///     过滤项目
        /// </summary>
        public List<ItemWithGrade> Itemlist { set; get; }

        /// <summary>
        ///     是否为多选项目
        ///     True：原来该项目为多选项目
        ///     False:原来该项目为单选项目，这里 JoinWithAnd 必须为 False
        /// </summary>
        public bool IsOrMode { set; get; }

        public List<string> StringItemList()
        {
            var l = new List<string>();
            foreach (var item in Itemlist)
            {
                l.Add(item.MasterCode);
            }
            return l;
        }

        public override IMongoQuery GetQuery()
        {
            IMongoQuery filterItemQuery;
            var queries = new IMongoQuery[Itemlist.Count];
            for (var i = 0; i < Itemlist.Count; i++)
            {
                var l = Itemlist[i];
                queries[i] = Query.ElemMatch(FieldName, Query.And(Query.EQ("MasterCode", l.MasterCode), Query.GTE("Grade", l.Grade)));
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
