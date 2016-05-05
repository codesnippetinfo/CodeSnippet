using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using InfraStructure.DataBase;
using InfraStructure.FilterSet;
using InfraStructure.Helper;
using InfraStructure.Table;

namespace InfraStructure.Chart
{
    public abstract class ChartSetBase : OwnerTable
    {
        /// <summary>
        ///     名称
        /// </summary>
        [DisplayName("名称")]
        [Required]
        public string Name { get; set; }

        /// <summary>
        ///     描述
        /// </summary>
        [DisplayName("描述")]
        public string Description { get; set; }

        /// <summary>
        /// 用户编号
        /// (可以考虑使用CreateUser字段代替)
        /// </summary>
        [HiddenInput]
        public string AccountCode { get; set; }

        /// <summary>
        /// 图表统计对象的过滤器
        /// </summary>
        /// <remarks>
        /// 如果为 000000 的话表示不用过滤
        /// </remarks>
        [DisplayName("过滤器")]
        public string FilterCode { set; get; }

        /// <summary>
        /// 图表的标题文字
        /// </summary>
        [DisplayName("标题")]
        public string Title { set; get; }

        /// <summary>
        /// 图表的类型
        /// </summary>
        [DisplayName("图表类型")]
        [UIHint("Enum")]
        public ChartType ChartType { set; get; }

        /// <summary>
        /// 排序
        /// </summary>
        [DisplayName("排序")]
        [UIHint("Enum")]
        public Sort.SortType SortArg { set; get; }


        /// <summary>
        ///     数据集
        /// </summary>
        [DisplayName("模型名称")]
        public string ModelName { get; set; }

        /// <summary>
        /// 统计关注字段
        /// </summary>
        [DisplayName("统计字段")]
        public string FieldName { set; get; }

        /// <summary>
        /// 
        /// </summary>
        [DisplayName("顶部记录数")]
        public int TopCount { set; get; }

        /// <summary>
        /// 宽度
        /// </summary>
        [DisplayName("宽度")]
        public int Width { set; get; }

        /// <summary>
        /// 高度
        /// </summary>
        [DisplayName("高度")]
        public int Height { set; get; }

        /// <summary>
        /// 附加表格
        /// </summary>
        [DisplayName("附加表格")]
        public bool WithTable { set; get; }

        /// <summary>
        /// 包括未知
        /// </summary>
        [DisplayName("包括未知")]
        public bool ContainsUnKnown { set; get; }

        /// <summary>
        /// 包括零
        /// </summary>
        [DisplayName("包括零")]
        public bool ContainsZero { set; get; }

        /// <summary>
        ///     获得分组可用的字段列表
        /// </summary>
        /// <param name="modelName"></param>
        /// <returns></returns>
        public static Dictionary<string, string> GetAvalibleChartField(string modelName) 
        {
            var chartFields = new Dictionary<string, string>();
            foreach (var property in ExternalType.GetTypeByName(modelName).GetProperties())
            {
                var filterAttrs = property.GetCustomAttributes(typeof(FilterItemAttribute), false);
                if (filterAttrs.Length == 0)
                {
                    continue;
                }
                var filterAttr = (FilterItemAttribute)filterAttrs[0];
                if (filterAttr != null)
                {
                    switch (filterAttr.MetaStructType)
                    {
                        case FilterItemAttribute.StructType.SingleMasterTable:
                        case FilterItemAttribute.StructType.SingleEnum:
                        case FilterItemAttribute.StructType.SingleStringMaster:
                            chartFields.Add(property.Name, CacheSystem.GetDisplayName(property.Name, ExternalType.GetTypeByName(modelName)));
                            break;
                        default:
                            break;
                    }
                }
            }
            return chartFields;
        }
    }
}
