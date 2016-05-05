using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using InfraStructure.Helper;
using InfraStructure.Table;
using InfraStructure.Utility;
using InfraStructure.Misc;

namespace InfraStructure.FilterSet
{
    public static class FilterHelper
    {
        /// <summary>
        /// 原则上，这个方法是不应该有的，
        /// View的东西这里和BL关联太紧密了
        /// </summary>
        /// <returns></returns>
        public static MvcHtmlString GetFilter(FilterItemBase filter, string ownerId)
        {
            var html = string.Empty;
            if (filter.GetType().Name == typeof(FilterItemList).Name)
            {
                var filterList = (FilterItemList)filter;
                if (filterList.AsEnum)
                {
                    var enumList = MasterWrapper.GenerateFromEnum(filter.FieldType);
                    return GetFilterItemListUi(filterList, enumList, filterList.Itemlist, filterList.IsOrMode);
                }
                //Master
                if (filterList.IsCatalog)
                {
                    var masterListWithCatalog = MasterWrapper.GenerateFromMaster(filter.FieldShortType(), ownerId, true);
                    masterListWithCatalog.Sort((x, y) => x.CatalogCode.CompareTo(y.CatalogCode));
                    var catalogList = MasterWrapper.GenerateFromMaster(filter.FieldShortType() + CatalogMasterTable.CatalogCollectionString, ownerId);
                    return GetFilterItemListCatalogUi(catalogList, filterList, masterListWithCatalog, filterList.Itemlist, filterList.IsOrMode);
                }
                var masterList = MasterWrapper.GenerateFromMaster(filter.FieldShortType(), ownerId);
                return GetFilterItemListUi(filterList, masterList, filterList.Itemlist, filterList.IsOrMode);
            }

            //FilterItemWithGradeList
            if (filter.GetType().Name == typeof(FilterItemWithGradeList).Name)
            {
                var filterGradeList = (FilterItemWithGradeList)filter;
                var masterList = MasterWrapper.GenerateFromMaster(filterGradeList.FieldShortType(), ownerId);
                return GetFilterItemWithGradeListUi((FilterItemWithGradeList)filter, masterList, filterGradeList.Itemlist, filterGradeList.IsOrMode);
            }

            //日期型 
            if (filter.GetType().Name == typeof(FilterItemDateFromTo).Name)
            {
                return GetFilterItemDateFromToUi((FilterItemDateFromTo)filter);
            }
            if (filter.GetType().Name == typeof(FilterItemDateInDays).Name)
            {
                return GetFilterItemWithInDaysUi((FilterItemDateInDays)filter);
            }
            //Boolean
            return filter.GetType().Name == typeof(FilterItemBoolean).Name ? GetFilterItemBooleanUi((FilterItemBoolean)filter) : new MvcHtmlString(html);
        }
        /// <summary>
        /// 获得详细
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="ownerId"></param>
        /// <returns></returns>
        public static MvcHtmlString GetDetails(FilterItemBase filter, string ownerId)
        {
            var html = string.Empty;
            html += "<dt>&nbsp;</dt>" + Environment.NewLine;
            html += "<dd></dd>" + Environment.NewLine;
            switch (filter.GetType().Name)
            {
                case "FilterItemList":
                    html += GetFilterItemListDetails(filter, ownerId);
                    break;
                case "FilterItemWithGradeList":
                    html += GetFilterItemWithGradeListDetails(filter, ownerId);
                    break;
                case "FilterItemDateInDays":
                    html += GetFilterItemDateInDaysDetails(filter, ownerId);
                    break;
                case "FilterItemDateFromTo":
                    html += GetFilterItemDateFromToDetails(filter, ownerId);
                    break;
                case "FilterItemBoolean":
                    html += GetFilterItemBooleanDetails(filter, ownerId);
                    break;
                default:
                    break;
            }
            return new MvcHtmlString(html);
        }

        #region FilterItemList
        /// <summary>
        /// GetFilterItemListUI
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="filter"></param>
        /// <param name="list"></param>
        /// <param name="selectKey"></param>
        /// <param name="isOrMode"></param>
        /// <returns></returns>
        public static MvcHtmlString GetFilterItemListUi<T>(FilterItemList filter, List<T> list, List<string> selectKey,
            bool isOrMode = false) where T : MasterTable
        {
            var html = string.Empty;
            html += HtmlUiHelper.GetCheckBox("启用", "IsActive." + filter.FieldName, filter.IsActive, false);
            if (!isOrMode)
            {
                html += HtmlUiHelper.GetCheckBox("并且（同时满足条件）", "JoinWithAnd." + filter.FieldName, filter.JoinWithAnd);
            }
            else
            {
                html += "<br />" + Environment.NewLine;
            }
            html += "<br />" + Environment.NewLine;
            html += HtmlUiHelper.GetMultiValueUi(filter.FieldName, list, selectKey);
            return new MvcHtmlString(html);
        }
        /// <summary>
        /// 获得一个带有目录的列表型过滤器的UI界面
        /// </summary>
        /// <param name="catalogList">一个目录的MasterTable</param>
        /// <param name="filter">过滤器</param>
        /// <param name="list">列表项目一览</param>
        /// <param name="selectKey">已经选择项目一览</param>
        /// <param name="isOrMode">是否为Or模式（对于单选项目）</param>
        /// <returns></returns>
        public static MvcHtmlString GetFilterItemListCatalogUi(List<MasterWrapper> catalogList, FilterItemList filter,
            List<MasterWrapper> list, List<string> selectKey, bool isOrMode = false) 
        {
            var html = string.Empty;
            html += HtmlUiHelper.GetCheckBox("启用", "IsActive." + filter.FieldName, filter.IsActive, false);
            if (!isOrMode)
            {
                html += HtmlUiHelper.GetCheckBox("并且（同时满足条件）", "JoinWithAnd." + filter.FieldName, filter.JoinWithAnd);
            }
            else
            {
                html += "<br />" + Environment.NewLine;
            }
            html += "<br />" + Environment.NewLine;

            foreach (var catalog in catalogList)
            {
                html += HtmlUiHelper.GetPanel(catalog.Code, catalog.Name + "[点击展开]",
                        HtmlUiHelper.GetMultiValueUi(filter.FieldName, 
                                                     list.Where(x => x.CatalogCode == catalog.Code).ToList(),
                                                     selectKey).ToString());
            }
            return new MvcHtmlString(html);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="collection"></param>
        public static void GetFilterItemListValueFromUi(FilterItemList filter, FormCollection collection)
        {
            filter.IsActive = collection.AllKeys.Contains("IsActive." + filter.FieldName);
            filter.JoinWithAnd = collection.AllKeys.Contains("JoinWithAnd." + filter.FieldName);
            filter.Itemlist = HtmlUiHelper.GetMultiValueFromUi(filter.FieldName, collection);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="ownerId"></param>
        /// <returns></returns>
        private static string GetFilterItemListDetails(FilterItemBase filter, string ownerId)
        {
            var html = string.Empty;
            var filterItemList = (FilterItemList)filter;
            html += "<dt>" + filter.DisplayName + "</dt>" + Environment.NewLine;
            html += "<dd></dd>" + Environment.NewLine;
            html += "<dt>列表</dt>" + Environment.NewLine;
            var listName = MasterTable.GetMasterNameList(filterItemList.FieldShortType(), filterItemList.Itemlist, ownerId);
            html += "<dd>" + MvcHtmlString.Create(listName.GetJoinString("<br />")) + "</dd>" + Environment.NewLine;
            html += "<dt>连接</dt>" + Environment.NewLine;
            html += "<dd>" + (filterItemList.JoinWithAnd ? "并且" : "或者") + "</dd>" + Environment.NewLine;
            return html;
        }

        #endregion

        #region FilterItemWithGradeList
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="filter"></param>
        /// <param name="list"></param>
        /// <param name="selectKey"></param>
        /// <param name="isOrMode"></param>
        /// <returns></returns>
        public static MvcHtmlString GetFilterItemWithGradeListUi<T>(FilterItemWithGradeList filter, List<T> list, List<ItemWithGrade> selectKey, bool isOrMode = false) where T : MasterTable
        {
            var html = string.Empty;
            html += HtmlUiHelper.GetCheckBox("启用", "IsActive." + filter.FieldName, filter.IsActive, false);
            if (!isOrMode)
            {
                html += HtmlUiHelper.GetCheckBox("并且（同时满足条件）", "JoinWithAnd." + filter.FieldName, filter.JoinWithAnd);
            }
            else
            {
                html += "<br />" + Environment.NewLine;
            }
            html += "<br />" + Environment.NewLine;
            html += "<h6>结果中将包括至少为指定等级的数据</h6>" + Environment.NewLine;
            html += HtmlUiHelper.GetMultiValueWithGradeUi(filter.FieldName, list, selectKey);
            return new MvcHtmlString(html);
        }


        public static void GetFilterItemWithGradeListValueFromUi(FilterItemWithGradeList filter, FormCollection collection)
        {
            filter.IsActive = collection.AllKeys.Contains("IsActive." + filter.FieldName);
            filter.JoinWithAnd = collection.AllKeys.Contains("JoinWithAnd." + filter.FieldName);
            filter.Itemlist = HtmlUiHelper.GetMultiValueWithGradeFromUi(filter.FieldName, collection);
        }

        private static string GetFilterItemWithGradeListDetails(FilterItemBase filter, string ownerId)
        {
            var html = string.Empty;
            var filterItemList = (FilterItemWithGradeList)filter;
            html += "<dt>" + filter.DisplayName + "</dt>" + Environment.NewLine;
            html += "<dd></dd>" + Environment.NewLine;
            html += "<dt>列表</dt>" + Environment.NewLine;
            var listName = MasterTable.GetMasterNameList(filterItemList.FieldShortType(), filterItemList.Itemlist, ownerId);
            html += "<dd>" + MvcHtmlString.Create(listName.GetJoinString("<br />")) + "</dd>" + Environment.NewLine;
            html += "<dt>连接</dt>" + Environment.NewLine;
            html += "<dd>" + (filterItemList.JoinWithAnd ? "并且" : "或者") + "</dd>" + Environment.NewLine;
            return html;
        }

        #endregion

        #region FilterItemDateInDays
        /// <summary>
        /// 
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        public static MvcHtmlString GetFilterItemWithInDaysUi(FilterItemDateInDays filter)
        {
            var html = string.Empty;
            html += HtmlUiHelper.GetCheckBox("启用", "IsActive." + filter.FieldName, filter.IsActive, false);
            html += "<br />" + Environment.NewLine;
            html += "<br />" + Environment.NewLine;
            html += "距离天数：" + Environment.NewLine;
            html += HtmlUiHelper.GetInput(filter.FieldName + ".Days", "number", filter.Days.ToString()) + Environment.NewLine;
            html += "<br />" + Environment.NewLine;
            html += "基准日期：" + Environment.NewLine;
            html += HtmlUiHelper.GetDatePicker(filter.FieldName + ".BaseDate", filter.BaseDate) + Environment.NewLine;
            return new MvcHtmlString(html);
        }

        public static void GetFilterItemDateInDaysValueFromUi(FilterItemDateInDays filter, FormCollection collection)
        {
            filter.IsActive = collection.AllKeys.Contains("IsActive." + filter.FieldName);
            filter.BaseDate = DateTime.Parse(collection[filter.FieldName + ".BaseDate"]);
            filter.Days = collection.AllKeys.Contains(filter.FieldName + ".Days") ? int.Parse(collection[filter.FieldName + ".Days"]) : 0;
        }

        private static string GetFilterItemDateInDaysDetails(FilterItemBase filter, string ownerId)
        {
            var html = string.Empty;
            var filterItemDateInDays = (FilterItemDateInDays)filter;
            html += "<dt>" + filter.DisplayName + "</dt>" + Environment.NewLine;
            html += "<dd></dd>" + Environment.NewLine;
            html += "<dt>基准日</dt>" + Environment.NewLine;
            html += "<dd>" + filterItemDateInDays.BaseDate + "</dd>" + Environment.NewLine;
            html += "<dt>天数范围</dt>" + Environment.NewLine;
            html += "<dd>" + filterItemDateInDays.Days + "</dd>" + Environment.NewLine;
            return html;
        }

        #endregion

        #region FilterItemDateFromTo

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="Days"></param>
        /// <returns></returns>
        public static MvcHtmlString GetFilterItemDateFromToUi(FilterItemDateFromTo filter)
        {

            if (filter.From == DateTime.MinValue) filter.From = DateTime.Now;
            if (filter.To == DateTime.MaxValue) filter.To = DateTime.Now;

            var html = string.Empty;
            html += HtmlUiHelper.GetCheckBox("启用", "IsActive." + filter.FieldName, filter.IsActive);
            html += "<br />" + Environment.NewLine;
            html += "开始日期：";
            html += HtmlUiHelper.GetDatePicker(filter.FieldName + ".From", filter.From);
            html += "终了日期：";
            html += HtmlUiHelper.GetDatePicker(filter.FieldName + ".To", filter.To);
            return new MvcHtmlString(html);
        }

        public static void GetFilterItemDateFromToValueFromUi(FilterItemDateFromTo filter, FormCollection collection)
        {
            filter.IsActive = collection.AllKeys.Contains("IsActive." + filter.FieldName);
            filter.From = DateTime.Parse(collection[filter.FieldName + ".From"]);
            filter.To = DateTime.Parse(collection[filter.FieldName + ".To"]);
        }

        private static string GetFilterItemDateFromToDetails(FilterItemBase filter, string ownerId)
        {
            var html = string.Empty;
            var filterItemDateFromTo = (FilterItemDateFromTo)filter;
            html += "<dt>" + filter.DisplayName + "</dt>" + Environment.NewLine;
            html += "<dd></dd>" + Environment.NewLine;
            html += "<dt>开始日</dt>" + Environment.NewLine;
            html += "<dd>" + filterItemDateFromTo.From + "</dd>" + Environment.NewLine;
            html += "<dt>结束日</dt>" + Environment.NewLine;
            html += "<dd>" + filterItemDateFromTo.To + "</dd>" + Environment.NewLine;
            return html;
        }

        #endregion

        #region FilterItemBoolean
        /// <summary>
        /// 
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        public static MvcHtmlString GetFilterItemBooleanUi(FilterItemBoolean filter)
        {
            var html = string.Empty;
            html += HtmlUiHelper.GetCheckBox("启用", "IsActive." + filter.FieldName, filter.IsActive);
            html += HtmlUiHelper.GetCheckBox("是/否", "YesOrNo." + filter.FieldName, filter.YesOrNo);
            return new MvcHtmlString(html);
        }

        public static void GetFilterItemBooleanValueFromUi(FilterItemBoolean filter, FormCollection collection)
        {
            filter.IsActive = collection.AllKeys.Contains("IsActive." + filter.FieldName);
            filter.YesOrNo = collection.AllKeys.Contains("YesOrNo." + filter.FieldName);
        }
        private static string GetFilterItemBooleanDetails(FilterItemBase filter, string ownerId)
        {
            var html = string.Empty;
            var filterBoolean = (FilterItemBoolean)filter;
            html += "<dt>" + filter.DisplayName + "</dt>" + Environment.NewLine;
            html += "<dd>" + (filterBoolean.YesOrNo ? "是" : "否") + "</dd>" + Environment.NewLine;
            return html;
        }

        #endregion
    }
}