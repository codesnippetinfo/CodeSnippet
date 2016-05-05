using System;
using System.Linq;
using MongoDB.Driver.Builders;

namespace InfraStructure.DataBase
{
    [Serializable]
    public class Sort
    {
        [Serializable]
        public enum SortType
        {
            /// <summary>
            /// 无
            /// </summary>
            ///[EnumDisplayName("无")]
            None = 0,
            /// <summary>
            /// 升序
            /// </summary>
            ///[EnumDisplayName("升序")]
            Ascending = 1,
            /// <summary>
            /// 降序
            /// </summary>
            ///[EnumDisplayName("降序")]
            Descending = -1
        }

        [Serializable]
        /// <summary>
        /// Sort用参数类
        /// MongoDB不能反序列化结构，只能反序列化类
        /// </summary>
        public class SortArg
        {
            /// <summary>
            /// 字段名称
            /// </summary>
            public string FieldName;
            /// <summary>
            /// Ascending ：1
            /// Descending ：-1
            /// </summary>
            public SortType SortType;
            /// <summary>
            /// 排序顺位
            /// </summary>
            public int SortOrder;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sortArgs"></param>
        /// <returns></returns>
        public static SortByBuilder GetSortBuilder(SortArg[] sortArgs)
        {
            var sortArgList = sortArgs.ToList();
            //根据SortOrder排序
            sortArgList.Sort((x, y) => { return x.SortOrder - y.SortOrder; });
            var sort = new SortByBuilder();
            for (var i = 0; i < sortArgList.Count; i++)
            {
                //可以省略排序优先度
                //if (SortArgList[i].SortOrder == 0) continue;
                sort = sortArgList[i].SortType == SortType.Ascending ? sort.Ascending(sortArgList[i].FieldName) : sort.Descending(sortArgList[i].FieldName);
            }
            return sort;
        }
    }
}
