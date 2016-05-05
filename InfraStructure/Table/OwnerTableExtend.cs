using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using InfraStructure.DataBase;
using InfraStructure.FilterSet;
using InfraStructure.Helper;
using InfraStructure.Utility;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using InfraStructure.Misc;

namespace InfraStructure.Table
{
    public static class OwnerTableExtend
    {
        /// <summary>
        ///     账号查询
        /// </summary>
        /// <remarks>CreateUserCodeQuery</remarks>
        [Obsolete]
        public static IMongoQuery AccountCodeQuery(string accountCode)
        {
            return Query.EQ(nameof(FilterSet.FilterSetBase.AccountCode), accountCode);
        }
        /// <summary>
        ///     获得指定AccountCode的指定表的数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collectionName"></param>
        /// <param name="ownerId"></param>
        /// <param name="accountCode"></param>
        /// <returns></returns>
        public static List<T> GetRecListByAccountCode<T>(string collectionName, string ownerId, string accountCode) where T : EntityBase
        {
            var query = Query.And(OwnerTableOperator.OwnerIdQuery(ownerId), AccountCodeQuery(accountCode));
            return MongoDbRepository.GetRecList<T>(collectionName, query);
        }

        /// <summary>
        ///     获得表示值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <param name="property"></param>
        /// <returns></returns>
        public static string GetDisplayValue<T>(T t, PropertyInfo property) where T : OwnerTable, new()
        {
            var displayValue = string.Empty;
            var filterAttrs = property.GetCustomAttributes(typeof(FilterItemAttribute), false);
            if (filterAttrs.Length == 1)
            {
                var filterAttr = (FilterItemAttribute)filterAttrs[0];
                switch (filterAttr.MetaStructType)
                {
                    case FilterItemAttribute.StructType.SingleMasterTable:
                        var masterCode = (string)property.GetValue(t);
                        var masterName = filterAttr.MetaType.Name;
                        displayValue = MasterTable.GetMasterName(masterName, masterCode, t.OwnerId);
                        break;
                    case FilterItemAttribute.StructType.MultiMasterTable:
                    case FilterItemAttribute.StructType.MultiCatalogMasterTable:
                        var masterCodeList = (List<string>)property.GetValue(t);
                        var masterListName = filterAttr.MetaType.Name;
                        displayValue = MasterTable.GetMasterNameList(masterListName, masterCodeList, t.OwnerId).GetJoinString(";");
                        break;
                    case FilterItemAttribute.StructType.MultiMasterTableWithGrade:
                        var masterGradeCodeList = (List<ItemWithGrade>)property.GetValue(t);
                        var masterGradeName = filterAttr.MetaType.Name;
                        displayValue = MasterTable.GetMasterNameGradeList(masterGradeName, masterGradeCodeList, t.OwnerId).GetJoinString(";");
                        break;
                    case FilterItemAttribute.StructType.Datetime:
                        displayValue = ((DateTime)property.GetValue(t)).ToString("yyyy-MM-dd");
                        break;
                    case FilterItemAttribute.StructType.Boolean:
                        displayValue = (bool)property.GetValue(t) ? "是" : "否";
                        break;
                    case FilterItemAttribute.StructType.SingleEnum:
                        displayValue = Enum.GetName(filterAttr.MetaType, property.GetValue(t));
                        break;
                    default:
                        displayValue = property.GetValue(t).ToString();
                        break;
                }
            }
            else
            {
                if (property.GetValue(t) != null)
                {
                    displayValue = property.GetValue(t).ToString();
                }
            }
            return displayValue;
        }
        /// <summary>
        ///     GetDisplayValue
        /// </summary>
        /// <param name="ownerId"></param>
        /// <param name="document"></param>
        /// <param name="property"></param>
        /// <returns></returns>
        public static string GetDisplayValue(string ownerId, BsonDocument document, PropertyInfo property)
        {
            var displayValue = string.Empty;
            var elementName = property.Name;
            var filterAttrs = property.GetCustomAttributes(typeof(FilterItemAttribute), false);
            if (document.GetValue(elementName) == BsonNull.Value)
            {
                return displayValue;
            }
            if (filterAttrs.Length == 1)
            {
                var filterAttr = (FilterItemAttribute)filterAttrs[0];
                switch (filterAttr.MetaStructType)
                {
                    case FilterItemAttribute.StructType.SingleMasterTable:
                        var masterCode = document.GetValue(elementName).ToString();
                        var masterName = filterAttr.MetaType.Name;
                        displayValue = MasterTable.GetMasterName(masterName, masterCode, ownerId);
                        break;
                    case FilterItemAttribute.StructType.MultiMasterTable:
                    case FilterItemAttribute.StructType.MultiCatalogMasterTable:
                        var bsonList = document.GetValue(elementName).AsBsonArray.ToList();
                        var masterCodeList = bsonList.Select(item => item.ToString()).ToList();
                        var masterListName = filterAttr.MetaType.Name;
                        displayValue = MasterTable.GetMasterNameList(masterListName, masterCodeList, ownerId).GetJoinString("<br />");
                        break;
                    case FilterItemAttribute.StructType.MultiMasterTableWithGrade:
                        bsonList = document.GetValue(elementName).AsBsonArray.ToList();
                        var masterGradeCodeList = new List<ItemWithGrade>();
                        foreach (var bsonValue in bsonList)
                        {
                            var item = (BsonDocument)bsonValue;
                            masterGradeCodeList.Add(new ItemWithGrade
                            {
                                Grade = (CommonGrade)item.GetValue(nameof(ItemWithGrade.Grade)).ToInt32(),
                                MasterCode = item.GetValue(nameof(ItemWithGrade.MasterCode)).ToString()
                            });
                        }
                        var masterGradeName = filterAttr.MetaType.Name;
                        displayValue = MasterTable.GetMasterNameGradeList(masterGradeName, masterGradeCodeList, ownerId).GetJoinString("<br />");
                        break;
                    case FilterItemAttribute.StructType.Datetime:
                        displayValue = (document.GetValue(elementName).ToLocalTime()).ToString("yyyy-MM-dd");
                        break;
                    case FilterItemAttribute.StructType.Boolean:
                        displayValue = (document.GetValue(elementName).AsBoolean) ? "是" : "否";
                        break;
                    case FilterItemAttribute.StructType.SingleEnum:
                        displayValue = Enum.GetName(filterAttr.MetaType, document.GetValue(elementName).ToInt32());
                        break;
                    default:
                        displayValue = document.GetValue(elementName).ToString();
                        break;
                }
            }
            else
            {
                displayValue = document.GetValue(elementName).ToString();
            }
            return displayValue;
        }
    }
}
