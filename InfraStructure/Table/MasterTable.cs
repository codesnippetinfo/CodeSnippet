using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using InfraStructure.DataBase;
using InfraStructure.Helper;
using InfraStructure.Utility;
using MongoDB.Driver.Builders;
using InfraStructure.Misc;

namespace InfraStructure.Table
{
    /// <summary>
    /// 辅助表
    /// </summary>
    public abstract class MasterTable : OwnerTable
    {

        //MasterCode和Owner的Code（内部编号）合并为一个编号

        /// <summary>
        ///     名字
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
        ///     等级
        /// </summary>
        [DisplayName("等级")]
        [Range(1, 99, ErrorMessage = "请输入1-99的数字")]
        [Required]
        public int Rank { get; set; }

        /// <summary>
        ///     是否启用
        /// </summary>
        [DisplayName("启用")]
        public bool IsActive { get; set; }

        /// <summary>
        /// 未知
        /// </summary>
        public const string UnKnownCode = "000000";

        /// <summary>
        /// 最大
        /// </summary>
        public const string MaxCode = "999999";

        /// <summary>
        /// Code格式
        /// </summary>
        public new const string CodeFormat = "D6";

        /// <summary>
        /// 辅助表前缀 
        /// </summary>
        public const string MasterPrefix = "M_";
        /// <summary>
        /// 空列表
        /// </summary>
        public const string StrEmptyList = "[空列表]";
        /// <summary>
        /// 未知
        /// </summary>
        public const string StrUnknown = "[未知]";

        #region Method
        /// <summary>
        ///     获得Rank列表
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ownerId"></param>
        /// <returns></returns>
        public static Dictionary<string, int> GetCodeRankDic<T>(string ownerId) where T : MasterTable, new()
        {
            var rankDic = new Dictionary<string, int>();
            var t = OwnerTableOperator.GetRecListByOwnerId<T>(new T().GetCollectionName(), ownerId);
            foreach (var item in t)
            {
                rankDic.Add(item.Code, item.Rank);
            }
            return rankDic;
        }

        /// <summary>
        ///     将MasterCode列表转换为Master描述字符
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="masterCodeList"></param>
        /// <param name="ownerId"></param>
        /// <returns></returns>
        public static List<string> GetMasterNameList<T>(List<string> masterCodeList, string ownerId)
            where T : MasterTable, new()
        {
            var rtn = new List<string>();
            if (masterCodeList == null) return rtn;
            foreach (var code in masterCodeList)
            {
                rtn.Add(GetMasterName<T>(code, ownerId));
            }
            if (rtn.Count == 0)
            {
                rtn.Add(StrEmptyList);
            }
            return rtn;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="collectionName"></param>
        /// <param name="masterCodeList"></param>
        /// <param name="ownerId"></param>
        /// <returns></returns>
        public static List<string> GetMasterNameList(string collectionName, List<string> masterCodeList, string ownerId)
        {
            var rtn = new List<string>();
            if (masterCodeList == null) return rtn;
            foreach (var code in masterCodeList)
            {
                rtn.Add(GetMasterName(collectionName, code, ownerId));
            }
            if (rtn.Count == 0)
            {
                rtn.Add(StrEmptyList);
            }
            return rtn;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="masterCodeList"></param>
        /// <param name="ownerId"></param>
        /// <returns></returns>
        public static List<string> GetMasterNameList<T>(List<ItemWithGrade> masterCodeList, string ownerId)
            where T : MasterTable, new()
        {
            var rtn = new List<string>();
            if (masterCodeList == null) return rtn;
            foreach (var gradeItem in masterCodeList)
            {
                rtn.Add(GetMasterName<T>(gradeItem.MasterCode, ownerId) + "-" + gradeItem.Grade);
            }
            if (rtn.Count == 0)
            {
                rtn.Add(StrEmptyList);
            }
            return rtn;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="collectionName"></param>
        /// <param name="masterCodeList"></param>
        /// <param name="ownerId"></param>
        /// <returns></returns>
        public static List<string> GetMasterNameList(string collectionName, List<ItemWithGrade> masterCodeList, string ownerId)
        {
            var rtn = new List<string>();
            if (masterCodeList == null) return rtn;
            foreach (var gradeItem in masterCodeList)
            {
                rtn.Add(GetMasterName(collectionName, gradeItem.MasterCode, ownerId) + "-" + gradeItem.Grade);
            }
            if (rtn.Count == 0)
            {
                rtn.Add(StrEmptyList);
            }
            return rtn;
        }

        /// <summary>
        ///     将MasterCode转换为Master描述字符
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="masterCode"></param>
        /// <param name="ownerId"></param>
        /// <returns></returns>
        public static string GetMasterName<T>(string masterCode, string ownerId) where T : MasterTable, new()
        {
            if (masterCode == null || string.IsNullOrEmpty(masterCode) || masterCode == UnKnownCode)
            {
                return StrUnknown;
            }
            var cacheKey = ownerId + "." + new T().GetCollectionName() + "." + masterCode;
            var cacheValue = CacheSystem.GetMasterNameCache(cacheKey);
            if (!string.IsNullOrEmpty(cacheValue)) return cacheValue;
            //检索
            var m = OwnerTableOperator.GetRecByCodeAtOwner<T>(new T().GetCollectionName(), ownerId, masterCode);
            if (m == null)
            {
                return StrUnknown;
            }
            CacheSystem.PutMasterNameCache(cacheKey, m.Name);
            return m.Name;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="collectionName"></param>
        /// <param name="masterCode"></param>
        /// <param name="ownerId"></param>
        /// <returns></returns>
        public static string GetMasterName(string collectionName, string masterCode, string ownerId)
        {
            if (masterCode == null || string.IsNullOrEmpty(masterCode) || masterCode == UnKnownCode)
            {
                return StrUnknown;
            }
            var cacheKey = ownerId + "." + collectionName + "." + masterCode;
            var cacheValue = CacheSystem.GetMasterNameCache(cacheKey);
            if (!string.IsNullOrEmpty(cacheValue)) return cacheValue;
            //检索
            var m = OwnerTableOperator.GetRecByCodeAtOwner(collectionName, ownerId, masterCode);
            if (m == null)
            {
                return StrUnknown;
            }
            CacheSystem.PutMasterNameCache(cacheKey, m.GetValue(nameof(Name)).ToString());
            return m.GetValue(nameof(Name)).ToString();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="CollectionName"></typeparam>
        /// <param name="gradeList"></param>
        /// <param name="ownerId"></param>
        /// <returns></returns>
        public static List<string> GetMasterNameGradeList(string collectionName, List<ItemWithGrade> gradeList, string ownerId)
        {
            var resultList = new List<string>();
            foreach (var item in gradeList)
            {
                resultList.Add(GetMasterName(collectionName, item.MasterCode, ownerId) + "-" + item.Grade);
            }
            if (resultList.Count == 0)
            {
                resultList.Add(StrEmptyList);
            }
            return resultList;
        }


        /// <summary>
        /// GetMasterCode
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="masterName"></param>
        /// <param name="ownerId"></param>
        /// <returns></returns>

        public static string GetMasterCode<T>(string masterName, string ownerId) where T : MasterTable, new()
        {
            if (masterName == null || string.IsNullOrEmpty(masterName))
            {
                return UnKnownCode;
            }
            var cacheKey = ownerId + "." + new T().GetCollectionName() + "." + masterName;
            var cacheValue = CacheSystem.GetMasterCodeCache(cacheKey);
            if (!string.IsNullOrEmpty(cacheValue)) return cacheValue;

            var ownerQuery = OwnerTableOperator.OwnerIdQuery(ownerId);
            var nameQuery = Query.EQ(nameof(Name), masterName);
            var query = Query.And(ownerQuery, nameQuery);
            var m = MongoDbRepository.GetFirstRec<T>(query);
            if (m == null)
            {
                return UnKnownCode;
            }
            CacheSystem.PutMasterCodeCache(cacheKey, m.Code);
            return m.Code;
        }

        public static string GetMasterCode(string collectionName, string masterName, string ownerId)
        {
            if (masterName == null || string.IsNullOrEmpty(masterName))
            {
                return UnKnownCode;
            }
            var cacheKey = ownerId + "." + collectionName + "." + masterName;
            var cacheValue = CacheSystem.GetMasterCodeCache(cacheKey);
            if (!string.IsNullOrEmpty(cacheValue)) return cacheValue;

            var ownerIdQuery = OwnerTableOperator.OwnerIdQuery(ownerId);
            var nameQuery = Query.EQ(nameof(Name), masterName);
            var query = Query.And(ownerIdQuery, nameQuery);
            var m = MongoDbRepository.GetFirstRec(collectionName, query);
            if (m == null)
            {
                return UnKnownCode;
            }
            CacheSystem.PutMasterCodeCache(cacheKey, m.GetValue(nameof(Code)).ToString());
            return m.GetValue(nameof(Code)).ToString();
        }

        /// <summary>
        /// GetMasterCodeList
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="masterNameList"></param>
        /// <param name="ownerId"></param>
        /// <returns></returns>
        public static List<string> GetMasterCodeList<T>(List<string> masterNameList, string ownerId) where T : MasterTable, new()
        {
            var rtn = new List<string>();
            if (masterNameList == null) return rtn;
            foreach (var code in masterNameList)
            {
                rtn.Add(GetMasterCode<T>(code, ownerId));
            }
            if (rtn.Count == 0)
            {
                rtn.Add(StrEmptyList);
            }
            return rtn;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="collectionName"></param>
        /// <param name="masterNameList"></param>
        /// <param name="OrganizationId"></param>
        /// <returns></returns>
        public static List<string> GetMasterCodeList(string collectionName, List<string> masterNameList, string ownerId)
        {
            var rtn = new List<string>();
            if (masterNameList == null) return rtn;
            foreach (var code in masterNameList)
            {
                rtn.Add(GetMasterCode(collectionName, code, ownerId));
            }
            if (rtn.Count == 0)
            {
                rtn.Add(StrEmptyList);
            }
            return rtn;
        }
        /// <summary>
        /// GetMasterCodeList
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="masterNameList"></param>
        /// <param name="ownerId"></param>
        /// <returns></returns>
        public static List<ItemWithGrade> GetMasterCodeGradeList<T>(List<string> masterNameList, string ownerId) where T : MasterTable, new()
        {
            var rtn = new List<ItemWithGrade>();
            if (masterNameList == null) return rtn;
            foreach (var gradeItem in masterNameList)
            {
                var t = gradeItem.Split("-".ToCharArray());
                rtn.Add(new ItemWithGrade
                {
                    MasterCode = GetMasterCode<T>(t[0], ownerId),
                    Grade = t[1].GetEnum(CommonGrade.NotAvaliable)
                });
            }
            return rtn;
        }


        /// <summary>
        /// 根据Rank获得Code
        /// </summary>
        /// <param name="rank"></param>
        /// <param name="ownerId"></param>
        /// <returns></returns>
        public static string GetCodeByRank<T>(int rank, string ownerId) where T : MasterTable, new()
        {
            var t = OwnerTableOperator.GetRecListByOwnerId<T>(new T().GetCollectionName(), ownerId);
            foreach (var status in t)
            {
                if (status.Rank == rank)
                {
                    return status.Code;
                }
            }
            return string.Empty;
        }
        #region Master

        /// <summary>
        /// 获得启用Master
        /// </summary>
        /// <param name="collectionName"></param>
        /// <param name="ownerId"></param>
        /// <returns></returns>
        public static List<MasterWrapper> GetActiveMasterWrapper(string collectionName, string ownerId) 
        {
            var query = Query.And(OwnerTableOperator.OwnerIdQuery(ownerId), Query.EQ(nameof(IsActive), true));
            return MongoDbRepository.GetRecList<MasterWrapper>(collectionName, query);
        }
        /// <summary>
        /// 获得启用Master
        /// </summary>
        /// <param name="collectionName"></param>
        /// <returns></returns>
        public static List<MasterWrapper> GetActiveMasterWrapper(string collectionName)
        {
            var query = Query.And(OwnerTableOperator.OwnerIdQuery(DefaultOwnerId), Query.EQ(nameof(IsActive), true));
            return MongoDbRepository.GetRecList<MasterWrapper>(collectionName, query);
        }
        /// <summary>
        ///     获得启用Master
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collectionName"></param>
        /// <param name="ownerId"></param>
        /// <returns></returns>
        [Obsolete]
        public static List<T> GetActiveMasterRec<T>(string collectionName, string ownerId) where T : MasterTable
        {
            var query = Query.And(OwnerTableOperator.OwnerIdQuery(ownerId), Query.EQ(nameof(IsActive), true));
            return MongoDbRepository.GetRecList<T>(collectionName, query);
        }

        /// <summary>
        ///     获得启用Master
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ownerId"></param>
        /// <returns></returns>
        public static List<T> GetActiveMasterRec<T>(string ownerId) where T : MasterTable, new()
        {
            var query = Query.And(OwnerTableOperator.OwnerIdQuery(ownerId), Query.EQ(nameof(IsActive), true));
            return MongoDbRepository.GetRecList<T>(new T().GetCollectionName(), query);
        }
        #endregion
        /// <summary>
        /// 插入Master
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ownerId"></param>
        /// <param name=nameof(MasterTable.Name)></param>
        /// <param name="rank"></param>
        public static void InsertMasterItem<T>(string ownerId, string name, int rank)
            where T : MasterTable, new()
        {
            var master = new T
            {
                OwnerId = ownerId,
                Name = name,
                Description = name,
                Rank = rank,
                IsActive = true
            };
            master.Code = GetNewCodeByOwnerId(master);
            MongoDbRepository.InsertRec(master);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ownerId"></param>
        /// <param name="name"></param>
        /// <param name="description"></param>
        /// <param name="rank"></param>
        public static void InsertMasterItem<T>(string ownerId, string name, string description, int rank)
            where T : MasterTable, new()
        {
            var master = new T
            {
                OwnerId = ownerId,
                Name = name,
                Description = description,
                Rank = rank,
                IsActive = true
            };
            master.Code = GetNewCodeByOwnerId(master);
            MongoDbRepository.InsertRec(master);
        }

        /// <summary>
        /// 更新缓存
        /// </summary>
        /// <param name="obj"></param>
        public static void UpdateCache(EntityBase obj)
        {
            var typeName = obj.GetType().Name;
            if (typeName.StartsWith(MasterPrefix))
            {
                var masterTable = (MasterTable)obj;
                var cacheKey = masterTable.OwnerId + "." + typeName + "." + masterTable.Code;
                CacheSystem.PutMasterNameCache(cacheKey, masterTable.Name);
                cacheKey = masterTable.OwnerId + "." + typeName + "." + masterTable.Name;
                CacheSystem.PutMasterCodeCache(cacheKey, masterTable.Code);
            }

            if (typeName == typeof(MasterWrapper).Name)
            {
                //MasterWrapper
                var masterWrapper = (MasterWrapper)obj;
                var cacheKey = masterWrapper.OwnerId + "." + masterWrapper.CollectionName + "." + masterWrapper.Code;
                CacheSystem.PutMasterNameCache(cacheKey, masterWrapper.Name);
                cacheKey = masterWrapper.OwnerId + "." + masterWrapper.CollectionName + "." + masterWrapper.Name;
                CacheSystem.PutMasterCodeCache(cacheKey, masterWrapper.Code);
            }
        }
        #endregion

        #region WithoutOwner
        /// <summary>
        /// 获得启用Master
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static List<T> GetActiveMasterRec<T>() where T : MasterTable, new()
        {
            return GetActiveMasterRec<T>(DefaultOwnerId);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static List<MasterWrapper> GetActiveMasterWrapperByCache(string masterName)
        {
            return CacheSystem.GetMasterTableCache(masterName);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="code"></param>
        /// <param name="name"></param>
        /// <param name="description"></param>
        /// <param name="rank"></param>
        /// <returns></returns>
        public static MasterWrapper GetMasterWrapper(int code, string name, string description, int rank)
        {
            var master = new MasterWrapper
            {
                Code = code.ToString(CodeFormat),
                OwnerId = DefaultOwnerId,
                Name = description,
                Description = name,
                Rank = rank,
                IsActive = true
            };
            return master;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="code"></param>
        /// <param name="name"></param>
        /// <param name="description"></param>
        /// <param name="catalogCode"></param>
        /// <param name="rank"></param>
        /// <returns></returns>
        public static MasterWrapper GetCatalogMasterWrapper(int code, string name, string description, int catalogCode, int rank)
        {
            var master = new MasterWrapper
            {
                Code = code.ToString(CodeFormat),
                CatalogCode = catalogCode.ToString(CodeFormat),
                OwnerId = DefaultOwnerId,
                Name = description,
                Description = name,
                Rank = rank,
                IsActive = true
            };
            return master;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="masterCode"></param>
        /// <returns></returns>
        public static string GetMasterName<T>(string masterCode) where T : MasterTable, new()
        {
            return GetMasterName<T>(masterCode, DefaultOwnerId);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="catalogCode"></param>
        /// <param name="name"></param>
        /// <param name="rank"></param>
        public static void InsertCatalogMasterItem<T>(string catalogCode, string name, int rank)
            where T : CatalogMasterTable, new()
        {
            var master = new T
            {
                OwnerId = DefaultOwnerId,
                CatalogCode = catalogCode,
                Name = name,
                Description = name,
                Rank = rank,
                IsActive = true
            };
            master.Code = GetNewCodeByOwnerId(master);
            MongoDbRepository.InsertRec(master);
        }
        #endregion

    }


}