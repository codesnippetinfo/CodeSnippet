using System.Collections.Generic;
using InfraStructure.DataBase;
using InfraStructure.Helper;
using InfraStructure.Table;
using MongoDB.Driver;
using MongoDB.Driver.Builders;

namespace InfraStructure.FilterSet
{
    /// <summary>
    ///     过滤器中心
    /// </summary>
    public class FilterSetCenter : FilterSetBase
    {
        #region "model"

        /// <summary>
        ///     数据集名称
        /// </summary>
        public override string GetCollectionName()
        {
            return "FilterSetCenter";
        }

        /// <summary>
        ///     数据集名称静态字段
        /// </summary>
        public static string CollectionName = "FilterSetCenter";


        /// <summary>
        ///     数据主键前缀
        /// </summary>
        public override string GetPrefix()
        {
            return string.Empty;
        }

        /// <summary>
        ///     数据主键前缀静态字段
        /// </summary>
        public static string Prefix = string.Empty;

        /// <summary>
        ///     Mvc画面的标题
        /// </summary>
        public static string MvcTitle = "过滤器中心";

        /// <summary>
        /// 设定DisplayName
        /// </summary>
        public void SetDisplayName(string modelName)
        {
            var filterType = ExternalType.GetTypeByName(modelName);
            // 考虑到DisplayName和FieldName的关联性
            // 以及DisplayName可能会修改名称
            for (var i = 0; i < FilterItems.Count; i++)
            {
                FilterItems[i].DisplayName = CacheSystem.GetDisplayName(FilterItems[i].FieldName, filterType);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ownerId"></param>
        /// <param name="accountCode"></param>
        /// <returns></returns>
        public static List<MasterWrapper> GetFilterWrapperList<T>(string ownerId, string accountCode)
        {
            var filterMaster = new List<MasterWrapper>();
            var ownerIdQuery = OwnerTableOperator.OwnerIdQuery(ownerId);
            var accountIdQuery = OwnerTableExtend.AccountCodeQuery(accountCode);
            var modelNameQuery = Query.EQ("ModelName", typeof(T).FullName);
            var filterQuery = Query.And(ownerIdQuery, accountIdQuery, modelNameQuery);
            var filterSetList = MongoDbRepository.GetRecList<FilterSetCenter>(CollectionName, filterQuery);
            foreach (var filter in filterSetList)
            {
                filterMaster.Add(new MasterWrapper
                {
                    Code = filter.Code,
                    Rank = int.Parse(filter.Code),
                    Name = filter.Name,
                    Description = filter.Description
                });
            }
            return filterMaster;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="modelName"></param>
        /// <param name="ownerId"></param>
        /// <param name="accountCode"></param>
        /// <returns></returns>
        public static List<MasterWrapper> GetFilterWrapperList(string ownerId, string accountCode, string modelName)
        {
            var filterMaster = new List<MasterWrapper>();
            var ownerIdQuery = OwnerTableOperator.OwnerIdQuery(ownerId);
            var accountIdQuery = OwnerTableExtend.AccountCodeQuery(accountCode);
            var modelNameQuery = Query.EQ("ModelName", modelName);
            var filterQuery = Query.And(ownerIdQuery, accountIdQuery, modelNameQuery);
            var filterSetList = MongoDbRepository.GetRecList<FilterSetCenter>(CollectionName, filterQuery);
            foreach (var filter in filterSetList)
            {
                filterMaster.Add(new MasterWrapper
                {
                    Code = filter.Code,
                    Rank = int.Parse(filter.Code),
                    Name = filter.Name,
                    Description = filter.Description
                });
            }
            return filterMaster;
        }

        /// <summary>
        /// 获得查询(CompnayIDReady)
        /// </summary>
        /// <returns></returns>
        public IMongoQuery GetQuery()
        {
            IMongoQuery filterItemQuery = null;
            foreach (var item in FilterItems)
            {
                if (item.IsActive)
                {
                    switch (item.GetType().Name)
                    {
                        case "FilterItemList":
                            var filterItemList = (FilterItemList)item;
                            if (filterItemList.Itemlist.Count > 0)
                            {
                                if (filterItemQuery == null)
                                {
                                    filterItemQuery = filterItemList.GetQuery();
                                    continue;
                                }
                                if (JoinWithAnd)
                                {
                                    filterItemQuery = Query.And(filterItemQuery, filterItemList.GetQuery());
                                }
                                else
                                {
                                    filterItemQuery = Query.Or(filterItemQuery, filterItemList.GetQuery());
                                }
                            }
                            break;
                        case "FilterItemWithGradeList":
                            var filterItemWithGradeList = (FilterItemWithGradeList)item;
                            if (filterItemWithGradeList.Itemlist.Count > 0)
                            {
                                if (filterItemQuery == null)
                                {
                                    filterItemQuery = filterItemWithGradeList.GetQuery();
                                    continue;
                                }
                                if (JoinWithAnd)
                                {
                                    filterItemQuery = Query.And(filterItemQuery, filterItemWithGradeList.GetQuery());
                                }
                                else
                                {
                                    filterItemQuery = Query.Or(filterItemQuery, filterItemWithGradeList.GetQuery());
                                }
                            }
                            break;
                        default:
                            if (filterItemQuery == null)
                            {
                                filterItemQuery = item.GetQuery();
                                continue;
                            }
                            if (JoinWithAnd)
                            {
                                filterItemQuery = Query.And(filterItemQuery, item.GetQuery());
                            }
                            else
                            {
                                filterItemQuery = Query.Or(filterItemQuery, item.GetQuery());
                            }
                            break;
                    }

                }
            }
            return Query.And(filterItemQuery, OwnerTableOperator.OwnerIdQuery(OwnerId));
        }
        #endregion
    }
}