using System;
using System.Collections.Generic;
using System.ComponentModel;
using InfraStructure.DataBase;
using InfraStructure.Helper;
using MongoDB.Driver.Builders;

namespace InfraStructure.Table
{
    /// <summary>
    ///     MasterTable的包装
    /// </summary>
    public class MasterWrapper : MasterTable
    {
        /// <summary>
        /// 数据集名称
        /// </summary>
        public string CollectionName = string.Empty;

        /// <summary>
        ///     分类
        /// </summary>
        [DisplayName("分类")]
        public string CatalogCode { get; set; }

        /// <summary>
        ///     表示顺序
        /// </summary>
        [DisplayName("表示顺序")]
        public int SortRank { get; set; }

        /// <summary>
        ///     背景颜色
        /// </summary>
        [DisplayName("背景颜色")]
        public WarningType BgColor { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string GetCollectionName()
        {
            return CollectionName;
        }

        /// <summary>
        /// 获得SN前缀
        /// </summary>
        /// <returns></returns>
        public override string GetPrefix()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 保存用辅助方法
        /// [BsonIgnore]的补充
        /// </summary>
        /// <param name="m"></param>
        /// <returns></returns>
        public static void CompleteCatalog(string collectionName, MasterWrapper m)
        {
            var query = Query.EQ(MongoDbRepository.MongoKeyField, m.Sn);
            var bsondoc = MongoDbRepository.GetFirstRec(collectionName, query);
            bsondoc.Add(nameof(CatalogCode), m.CatalogCode);
            MongoDbRepository.UpdateDocument(bsondoc, collectionName);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="stringDic"></param>
        /// <returns></returns>
        public static List<MasterWrapper> GenerateFromStringDic(Dictionary<string, string> stringDic)
        {
            var t = new List<MasterWrapper>();
            foreach (var itemName in stringDic)
            {
                t.Add(new MasterWrapper
                {
                    Name = itemName.Value,
                    Description = itemName.Value,
                    Code = itemName.Key
                });
            }
            return t;
        }

        /// <summary>
        /// 将扩展Master转换为Master
        /// </summary>
        /// <param name="master"></param>
        /// <returns></returns>
        public static List<MasterWrapper> GenerateFromMaster(string collectionName, string ownerId, bool hasCatalogCode = false)
        {
            var list = OwnerTableOperator.GetRecListByOwnerId(collectionName, ownerId);
            var t = new List<MasterWrapper>();
            foreach (var item in list)
            {
                t.Add(new MasterWrapper
                {
                    Code = item.GetValue(nameof(Code)).ToString(),
                    Name = item.GetValue(nameof(Name)).ToString(),
                    Description = item.GetValue(nameof(Description)).ToString(),
                    Rank = item.GetValue(nameof(Rank)).ToInt32(),
                    CatalogCode = (hasCatalogCode ? item.GetValue(nameof(CatalogCode)).ToString() : string.Empty)
                });
            }
            return t;
        }

        /// <summary>
        /// 枚举转MasterWrapper（泛型）
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static List<MasterWrapper> GenerateFromEnum<T>()
        {
            var t = new List<MasterWrapper>();
            foreach (var itemName in Enum.GetNames(typeof(T)))
            {
                t.Add(new MasterWrapper
                {
                    Name = itemName,
                    Description = itemName,
                    Rank = Enum.Parse(typeof(T), itemName).GetHashCode(),
                    Code = Enum.Parse(typeof(T), itemName).GetHashCode().ToString()
                });
            }
            return t;
        }
        /// <summary>
        /// 将枚举装换为MasterWrapper
        /// </summary>
        /// <param name="enumTypeName"></param>
        /// <returns></returns>
        public static List<MasterWrapper> GenerateFromEnum(string enumTypeName)
        {
            var t = new List<MasterWrapper>();
            var enumtype = Type.GetType(enumTypeName) ?? ExternalType.GetEnumByName(enumTypeName);
            foreach (var itemName in Enum.GetNames(enumtype))
            {
                t.Add(new MasterWrapper
                {
                    Name = itemName,
                    Description = itemName,
                    Rank = Enum.Parse(enumtype, itemName).GetHashCode(),
                    Code = Enum.Parse(enumtype, itemName).GetHashCode().ToString()
                });
            }
            return t;
        }
    }
}
