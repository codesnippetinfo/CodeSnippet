using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using InfraStructure.Table;
using System;
using InfraStructure.DataBase;

namespace InfraStructure.Helper
{
    /// <summary>
    /// Cache System
    /// </summary>
    public static class CacheSystem
    {
        /// <summary>
        /// 系统启动时候，载入所有的Master
        /// </summary>
        public static void Init()
        {
            foreach (var type in ExternalType.ExternalTypeDictionary.Values)
            {
                if (type.Name.StartsWith(MasterTable.MasterPrefix))
                {
                    var masterList = MongoDbRepository.GetRecList(type.Name);
                    foreach (var item in masterList)
                    {
                        //OwnerId + "." + new T().GetCollectionName() + "." + MasterName
                        var nameKey = item.GetValue(nameof(OwnerTable.OwnerId)) + "." + type.Name + "." + item.GetValue(nameof(MasterTable.Name));
                        var codeKey = item.GetValue(nameof(OwnerTable.OwnerId)) + "." + type.Name + "." + item.GetValue(nameof(OwnerTable.Code));
                        MasterCodeCacheDic.Add(nameKey, item.GetValue(nameof(OwnerTable.Code)).ToString());
                        MasterNameCacheDic.Add(codeKey, item.GetValue(nameof(MasterTable.Name)).ToString());
                    }
                }
                else
                {
                    var displayNameProperty = type.GetCustomAttributes(typeof(DisplayNameAttribute), false).FirstOrDefault() as DisplayNameAttribute;
                    if (displayNameProperty != null)
                    {
                        DisplayNameCacheDic.Add(type.FullName, displayNameProperty.DisplayName);
                    }
                    //typeObj.FullName + "." + PropertyName;
                    foreach (var item in type.GetMembers())
                    {
                        //有些方法也是有DisplayName的，例如离职函数，在职月数等
                        displayNameProperty = item.GetCustomAttributes(typeof(DisplayNameAttribute), false).FirstOrDefault() as DisplayNameAttribute;
                        if (displayNameProperty != null)
                        {
                            DisplayNameCacheDic.Add(type.FullName + "." + item.Name, displayNameProperty.DisplayName);
                        }
                    }
                }
            }
        }
        /// <summary>
        /// 根据数据集名字建立Master缓存
        /// </summary>
        public static void InitMasterByCollectionName()
        {
            foreach (var mongodb in MongoDbRepository.DatabaseList.Values)
            {
                foreach (var strCol in mongodb.GetCollectionNames())
                {
                    if (strCol.StartsWith(MasterTable.MasterPrefix))
                    {
                        var masterList = MongoDbRepository.GetRecList(strCol);
                        foreach (var item in masterList)
                        {
                            //OwnerId + "." + new T().GetCollectionName() + "." + MasterName
                            var nameKey = item.GetValue(nameof(OwnerTable.OwnerId)) + "." + strCol + "." + item.GetValue(nameof(MasterTable.Name));
                            var codeKey = item.GetValue(nameof(OwnerTable.OwnerId)) + "." + strCol + "." + item.GetValue(nameof(OwnerTable.Code));
                            if (!MasterCodeCacheDic.ContainsKey(nameKey))
                            {
                                //Catalog型可能出现同名
                                MasterCodeCacheDic.Add(nameKey, item.GetValue(nameof(OwnerTable.Code)).ToString());
                            }
                            MasterNameCacheDic.Add(codeKey, item.GetValue(nameof(MasterTable.Name)).ToString());
                        }
                    }
                }
            }
        }

        /// <summary>
        ///     缓存字典(GetMasterName)
        /// </summary>
        private static readonly Dictionary<string, string> MasterNameCacheDic = new Dictionary<string, string>();
        /// <summary>
        ///     缓存字典(GetMasterCode)
        /// </summary>
        private static readonly Dictionary<string, string> MasterCodeCacheDic = new Dictionary<string, string>();
        /// <summary>
        ///     缓存字典(DisplayName)
        /// </summary>
        private static readonly Dictionary<string, string> DisplayNameCacheDic = new Dictionary<string, string>();
        /// <summary>
        ///     MasterDictionary
        /// </summary>
        private static Dictionary<string, List<MasterWrapper>> _masterDictionary = new Dictionary<string, List<MasterWrapper>>();

        /// <summary>
        ///     获得Cache
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string GetMasterNameCache(string key)
        {
            if (MasterNameCacheDic.ContainsKey(key))
            {
                //HitCount++;
                return MasterNameCacheDic[key];
            }
            return string.Empty;
        }

        /// <summary>
        ///     获得Cache
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string GetMasterCodeCache(string key)
        {
            if (MasterCodeCacheDic.ContainsKey(key))
            {
                //HitCount++;
                return MasterCodeCacheDic[key];
            }
            return string.Empty;
        }

        /// <summary>
        ///     获得Cache
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string GetDisplayNameCache(string key)
        {
            if (DisplayNameCacheDic.ContainsKey(key))
            {
                //HitCount++;
                return DisplayNameCacheDic[key];
            }
            return string.Empty;
        }
        /// <summary>
        /// 获取Master
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static List<MasterWrapper> GetMasterTableCache(string key)
        {
            if (_masterDictionary.ContainsKey(key))
            {
                return _masterDictionary[key];
            }
            return new List<MasterWrapper>();
        }
        /// <summary>
        ///     更新Cache
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static void PutMasterNameCache(string key, string value)
        {
            if (MasterNameCacheDic.ContainsKey(key))
            {
                MasterNameCacheDic[key] = value;
            }
            else
            {
                MasterNameCacheDic.Add(key, value);
            }
        }

        public static void PutMasterNameCache(MasterWrapper master, string collectionName)
        {
            var cacheKey = master.OwnerId + "." + collectionName + "." + master.Code;
            PutMasterNameCache(cacheKey, master.Name);
        }

        /// <summary>
        ///     更新Cache
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static void PutMasterCodeCache(string key, string value)
        {
            if (MasterCodeCacheDic.ContainsKey(key))
            {
                MasterCodeCacheDic[key] = value;
            }
            else
            {
                MasterCodeCacheDic.Add(key, value);
            }
        }
        public static void PutMasterCodeCache(MasterWrapper master, string collectionName)
        {
            var cacheKey = master.OwnerId + "." + collectionName + "." + master.Name;
            PutMasterNameCache(cacheKey, master.Code);
        }
        /// <summary>
        ///     更新Cache
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static void PutDisplayNameCache(string key, string value)
        {
            if (DisplayNameCacheDic.ContainsKey(key))
            {
                DisplayNameCacheDic[key] = value;
            }
            else
            {
                DisplayNameCacheDic.Add(key, value);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="collectionName"></param>
        /// <param name="value"></param>
        public static void PutMasterTableCache(string collectionName, List<MasterWrapper> value)
        {
            foreach (var item in value)
            {
                PutMasterNameCache(item, collectionName);
                PutMasterCodeCache(item, collectionName);
            }
            if (_masterDictionary.ContainsKey(collectionName))
            {
                _masterDictionary[collectionName] = value;
            }
            else
            {
                _masterDictionary.Add(collectionName, value);
            }
        }

        /// <summary>
        /// 获得DisplayName
        /// </summary>
        /// <param name="propertyName"></param>
        /// <param name="typeObj"></param>
        /// <returns></returns>
        public static string GetDisplayName(string propertyName, Type typeObj)
        {
            var cacheKey = typeObj.FullName + "." + propertyName;
            var cacheValue = GetDisplayNameCache(cacheKey);
            if (!string.IsNullOrEmpty(cacheValue))
            {
                return cacheValue;
            }
            var displayNameProperty = typeObj.GetProperty(propertyName).GetCustomAttributes(typeof(DisplayNameAttribute), false).FirstOrDefault() as DisplayNameAttribute;
            PutDisplayNameCache(cacheKey, displayNameProperty.DisplayName);
            return displayNameProperty.DisplayName;
        }

        /// <summary>
        ///     获得DisplayName
        /// </summary>
        /// <param name="typeObj"></param>
        /// <returns></returns>
        public static string GetDisplayName(Type typeObj)
        {
            var cacheKey = typeObj.FullName;
            var cacheValue = GetDisplayNameCache(cacheKey);
            if (!string.IsNullOrEmpty(cacheValue))
            {
                return cacheValue;
            }
            var displayNameProperty = typeObj.GetCustomAttributes(typeof(DisplayNameAttribute), false).FirstOrDefault() as DisplayNameAttribute;
            if (displayNameProperty == null)
            {
                PutDisplayNameCache(cacheKey, string.Empty);
                return string.Empty;
            }
            PutDisplayNameCache(cacheKey, displayNameProperty.DisplayName);
            return displayNameProperty.DisplayName;
        }

    }
}