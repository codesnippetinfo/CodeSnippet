using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Web.Mvc;
using InfraStructure.DataBase;
using InfraStructure.FilterSet;
using InfraStructure.Helper;
using InfraStructure.Table;
using MongoDB.Bson;

namespace InfraStructure.DataView
{
    public class DataViewSet : OwnerTable
    {
        #region Entity

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
        /// 
        /// </summary>
        public string ModelName { set; get; }

        /// <summary>
        ///     字段表示列表
        /// </summary>
        [DisplayName("表示器")]
        public List<DisplayField> DisplayList { get; set; }

        /// <summary>
        ///     过滤器
        /// </summary>
        [DisplayName("过滤器")]
        public string FilterCode { set; get; }

        /// <summary>
        ///     排序器
        /// </summary>
        [DisplayName("排序器")]
        public List<Sort.SortArg> SortArg { set; get; }

        /// <summary>
        ///     
        /// </summary>
        public static string MvcTitle = "视图中心";

        /// <summary>
        ///     
        /// </summary>
        public static string CollectionName = "DataViewSet";

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string GetCollectionName()
        {
            return "DataViewSet";
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string GetPrefix()
        {
            return string.Empty;
        }

        #endregion

        #region Method
        /// <summary>
        /// 表示器
        /// </summary>
        public class DisplayField
        {
            /// <summary>
            /// 字段名称
            /// </summary>
            public string FieldName;
            /// <summary>
            /// 自定义表示名称
            /// </summary>
            public string TitleName;
            /// <summary>
            /// 展示位置
            /// </summary>
            public int DisplayOrder;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="pageNo"></param>
        /// <returns></returns>
        public Pages GetPages(int pageNo)
        {
            var modelType = ExternalType.GetTypeByName(ModelName);
            if (FilterCode != MasterTable.UnKnownCode)
            {
                var filter = OwnerTableOperator.GetRecByCodeAtOwner<FilterSetCenter>(FilterSetCenter.CollectionName, OwnerId, FilterCode);
                var recordPages = new Pages(OwnerTableOperator.GetCountByOwnerId(modelType.Name, OwnerId, filter.GetQuery()));
                recordPages.CurrentPageNo = pageNo;
                return recordPages;
            }
            else
            {
                var recordPages = new Pages(OwnerTableOperator.GetCountByOwnerId(modelType.Name, OwnerId));
                recordPages.CurrentPageNo = pageNo;
                return recordPages;
            }
        }

        /// <summary>
        /// 获得数据的Title数组
        /// </summary>
        /// <returns></returns>
        public string[] GetTitles()
        {
            var title = new string[DisplayList.Count];
            var modelType = ExternalType.GetTypeByName(ModelName);
            for (var i = 0; i < DisplayList.Count; i++)
            {
                title[i] = string.IsNullOrEmpty(DisplayList[i].TitleName) ?
                    CacheSystem.GetDisplayName(DisplayList[i].FieldName, modelType) : DisplayList[i].TitleName;
            }
            return title;
        }

        /// <summary>
        ///     获得数据
        /// </summary>
        /// <param name="pageNo"></param>
        /// <returns></returns>
        public List<string[]> GetPageData(int pageNo)
        {
            var modelType = ExternalType.GetTypeByName(ModelName);
            var recordList = new List<BsonDocument>();
            if (FilterCode != MasterTable.UnKnownCode)
            {
                var filter = OwnerTableOperator.GetRecByCodeAtOwner<FilterSetCenter>(FilterSetCenter.CollectionName, OwnerId, FilterCode);
                var recordPages = new Pages(OwnerTableOperator.GetCountByOwnerId(modelType.Name, OwnerId, filter.GetQuery()));
                recordPages.CurrentPageNo = pageNo;
                recordList = OwnerTableOperator.GetRecListByOwnerIdWithPage(modelType.Name, OwnerId, SortArg.ToArray(), recordPages, filter.GetQuery());
            }
            else
            {
                var recordPages = new Pages(OwnerTableOperator.GetCountByOwnerId(modelType.Name, OwnerId));
                recordPages.CurrentPageNo = pageNo;
                recordList = OwnerTableOperator.GetRecListByOwnerIdWithPage(modelType.Name, OwnerId, SortArg.ToArray(), recordPages);
            }
            return GetValueList(modelType, recordList);
        }

        /// <summary>
        /// 数据组装函数
        /// </summary>
        /// <param name="modelType"></param>
        /// <param name="recordList"></param>
        /// <returns></returns>
        public List<string[]> GetValueList(Type modelType, List<BsonDocument> recordList)
        {
            var valueList = new List<string[]>();
            //获得Propery字典
            var propertyDic = new Dictionary<string, PropertyInfo>();
            foreach (var prop in modelType.GetProperties())
            {
                propertyDic.Add(prop.Name, prop);
            }
            foreach (var doc in recordList)
            {
                var valueArray = new string[DisplayList.Count];
                for (var i = 0; i < DisplayList.Count; i++)
                {
                    if (propertyDic.ContainsKey(DisplayList[i].FieldName))
                    {
                        valueArray[i] = OwnerTableExtend.GetDisplayValue(OwnerId, doc, propertyDic[DisplayList[i].FieldName]);
                    }
                    else
                    {
                        valueArray[i] = "N/A";
                    }
                }
                valueList.Add(valueArray);
            }
            return valueList;
        }

        #endregion
    }
}
