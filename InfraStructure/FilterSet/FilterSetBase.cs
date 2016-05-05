using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Web.Mvc;
using InfraStructure.Helper;
using InfraStructure.Table;

namespace InfraStructure.FilterSet
{
    public abstract class FilterSetBase : OwnerTable
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
        ///     数据集
        /// </summary>
        [DisplayName("模型名称")]
        public string ModelName { get; set; }

        /// <summary>
        /// 过滤器
        /// </summary>
        public List<FilterItemBase> FilterItems { get; set; }

        /// <summary>
        ///     是否用 And 连接
        ///     True：And False：Or
        /// </summary>
        [DisplayName("并且")]
        public bool JoinWithAnd { set; get; }

        /// <summary>
        /// 根据类型获得可用的FilterItem列表
        /// </summary>
        /// <returns></returns>
        public List<FilterItemBase> GetAvalibleFilterItems() 
        {
            var filterItems = new List<FilterItemBase>();
            foreach (var property in ExternalType.GetTypeByName(ModelName).GetProperties())
            {
                var filterAttrs = property.GetCustomAttributes(typeof(FilterItemAttribute), false);
                if (filterAttrs.Length == 0)
                {
                    continue;
                }
                var filterAttr = (FilterItemAttribute)filterAttrs[0];
                if (filterAttr != null)
                {
                    FilterItemBase filterItem = null;
                    switch (filterAttr.MetaStructType)
                    {
                        case FilterItemAttribute.StructType.SingleMasterTable:
                            //JoinWithAnd 默认为False
                            filterItem = new FilterItemList(property.Name) { IsOrMode = true };
                            InsertFilter(filterItems, property, filterAttr, filterItem);
                            break;
                        case FilterItemAttribute.StructType.MultiMasterTable:
                            filterItem = new FilterItemList(property.Name) { IsOrMode = false };
                            InsertFilter(filterItems, property, filterAttr, filterItem);
                            break;
                        case FilterItemAttribute.StructType.SingleEnum:
                            filterItem = new FilterItemList(property.Name) { AsEnum = true, IsOrMode = true };
                            InsertFilter(filterItems, property, filterAttr, filterItem);
                            break;
                        case FilterItemAttribute.StructType.MultiEnum:
                            filterItem = new FilterItemList(property.Name) { AsEnum = true, IsOrMode = false };
                            InsertFilter(filterItems, property, filterAttr, filterItem);
                            break;
                        case FilterItemAttribute.StructType.SingleCatalogMasterTable:
                            filterItem = new FilterItemList(property.Name) { IsCatalog = true, IsOrMode = true };
                            InsertFilter(filterItems, property, filterAttr, filterItem);
                            break;
                        case FilterItemAttribute.StructType.MultiCatalogMasterTable:
                            filterItem = new FilterItemList(property.Name) { IsCatalog = true, IsOrMode = false };
                            InsertFilter(filterItems, property, filterAttr, filterItem);
                            break;
                        case FilterItemAttribute.StructType.SingleMasterTableWithGrade:
                            filterItem = new FilterItemWithGradeList(property.Name) { IsOrMode = true };
                            InsertFilter(filterItems, property, filterAttr, filterItem);
                            break;
                        case FilterItemAttribute.StructType.MultiMasterTableWithGrade:
                            filterItem = new FilterItemWithGradeList(property.Name) { IsOrMode = false };
                            InsertFilter(filterItems, property, filterAttr, filterItem);
                            break;
                        case FilterItemAttribute.StructType.Datetime:
                            filterItem = new FilterItemDateFromTo(property.Name);
                            InsertFilter(filterItems, property, filterAttr, filterItem);
                            FilterItemBase filterItem2 = new FilterItemDateInDays(property.Name);
                            InsertFilter(filterItems, property, filterAttr, filterItem2);
                            break;
                        case FilterItemAttribute.StructType.Boolean:
                            filterItem = new FilterItemBoolean(property.Name);
                            InsertFilter(filterItems, property, filterAttr, filterItem);
                            break;
                        default:
                            break;
                    }
                }
            }
            return filterItems;
        }

        /// <summary>
        /// 插入到过滤器列表
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="filterItems"></param>
        /// <param name="property"></param>
        /// <param name="filterAttr"></param>
        /// <param name="fieldItem"></param>
        private static void InsertFilter(List<FilterItemBase> filterItems,
                                            PropertyInfo property,
                                            FilterItemAttribute filterAttr,
                                            FilterItemBase fieldItem) 
        {
            //DisplayName 这里设计成为一个动态属性，使用前赋值
            fieldItem.IsActive = false;
            if (filterAttr.MetaType != null)
            {
                fieldItem.FieldType = filterAttr.MetaType.FullName;
            }
            filterItems.Add(fieldItem);
        }
    }
}
