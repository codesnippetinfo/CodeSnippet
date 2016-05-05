using System.Collections.Generic;
using System.ComponentModel;
using InfraStructure.DataBase;
using MongoDB.Driver.Builders;

namespace InfraStructure.Table
{
    /// <summary>
    /// 带有目录的辅助表
    /// </summary>
    public abstract class CatalogMasterTable : MasterTable
    {
        /// <summary>
        ///     分类
        /// </summary>
        [DisplayName("分类")]
        public string CatalogCode { get; set; }

        /// <summary>
        /// 目录数据集名称后缀
        /// </summary>
        /// <remarks>
        /// 这里推荐（非强制）如果数据集为 A ，
        /// 则记录它的目录的数据集为 A + Catalog
        /// </remarks>
        public const string CatalogCollectionString = "Catalog";

        /// <summary>
        /// 获得Catalog名称
        /// </summary>
        /// <returns></returns>
        public abstract string GetCatalogCollectionName();
        /// <summary>
        /// 获得Catalog字典
        /// CatalogCode VS CatalogItem List
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ownerId"></param>
        /// <returns></returns>
        public static Dictionary<string, List<T>> GetCatalogItem<T>(string ownerId) where T : CatalogMasterTable, new()
        {
            var catalogList = new Dictionary<string, List<T>>();
            foreach (var catalogItem in GetActiveMasterRec<MasterTable>(new T().GetCatalogCollectionName(), ownerId))
            {
                catalogList.Add(catalogItem.Code, GetListByCatalogCode<T>(ownerId, catalogItem.Code));
            }
            return catalogList;
        }
        /// <summary>
        /// 获得Catalog字典
        /// CatalogCode VS CatalogItem List
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static Dictionary<string, List<T>> GetCatalogItem<T>() where T : CatalogMasterTable, new()
        {
            return GetCatalogItem<T>(DefaultOwnerId);
        }
        /// <summary>
        ///     获得指定分类的所有技能(启用状态)
        /// </summary>
        /// <param name="ownerId"></param>
        /// <param name="catalogCode"></param>
        /// <returns></returns>
        public static List<T> GetListByCatalogCode<T>(string ownerId, string catalogCode, bool isActive = true) where T : MasterTable, new()
        {
            var queryOwner = OwnerTableOperator.OwnerIdQuery(ownerId);
            var queryCatalogCode = Query.EQ("CatalogCode", catalogCode);
            var query = Query.And(queryOwner, queryCatalogCode);
            if (isActive)
            {
                query = Query.And(query, Query.EQ("IsActive", true));
            }
            return MongoDbRepository.GetRecList<T>(query);
        }
        /// <summary>
        ///     获得指定分类的所有技能(启用状态)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="catalogCode"></param>
        /// <returns></returns>
        public static List<T> GetListByCatalogCode<T>(string catalogCode, bool isActive = true) where T : MasterTable, new()
        {
            return GetListByCatalogCode<T>(DefaultOwnerId, catalogCode, isActive);
        }
    }
}
