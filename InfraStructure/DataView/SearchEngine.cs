using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using InfraStructure.DataBase;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;

namespace InfraStructure.DataView
{
    /// <summary>
    /// 搜索引擎
    /// </summary>
    public static class SearchEngine
    {
        /// <summary>
        ///     关键字检索
        ///     指定的项目列表里面是否包含指定的关键字(精确匹配)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="keyWord"></param>
        /// <param name="fieldList"></param>
        /// <param name="addtionalQuery"></param>
        /// <returns></returns>
        public static List<T> SearchByKeyWord<T>(string keyWord, string[] fieldList, IMongoQuery addtionalQuery = null) where T : EntityBase, new()
        {
            IMongoQuery totalQuery = null;
            foreach (var field in fieldList)
            {
                totalQuery = totalQuery == null ? Query.EQ(field, keyWord) : Query.Or(totalQuery, Query.EQ(field, keyWord));
            }
            if (addtionalQuery != null)
            {
                totalQuery = Query.And(addtionalQuery, totalQuery);
            }
            return MongoDbRepository.GetRecList<T>(totalQuery);
        }
        /// <summary>
        ///     关键字检索
        ///     指定的项目列表里面是否包含指定的关键字列表之一(精确匹配)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="searchKey"></param>
        /// <param name="listField"></param>
        /// <param name="setting"></param>
        /// <param name="addtionalQuery"></param>
        /// <returns></returns>
        private static List<T> SearchListByKeyWord<T>(string[] searchArray, string[] listField, Action<MongoCursor> setting, IMongoQuery addtionalQuery = null) where T : EntityBase, new()
        {
            //或者关系查询关键字在非代码字段中
            var totalQuery = GetQuery(searchArray,listField,addtionalQuery);
            return MongoDbRepository.GetRecList<T>(totalQuery, setting);
        }
        /// <summary>
        ///     关键字检索
        ///     指定的项目里面是否包含指定的关键字之一(精确匹配)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="keyWord"></param>
        /// <param name="fieldList"></param>
        /// <param name="addtionalQuery"></param>
        /// <returns></returns>
        public static IMongoQuery GetQuery(string[] keyWord, string[] fieldList, IMongoQuery addtionalQuery = null)
        {
            IMongoQuery totalQuery = null;
            foreach (var searchItem in keyWord)
            {
                foreach (var field in fieldList)
                {
                    totalQuery = totalQuery == null ? Query.EQ(field, searchItem) : Query.Or(totalQuery, Query.EQ(field, searchItem));
                }
            }
            if (addtionalQuery != null)
            {
                totalQuery = Query.And(addtionalQuery, totalQuery);
            }
            return totalQuery;
        }
        /// <summary>
        /// 将带空格关键字转换为关键字组
        /// </summary>
        /// <param name="searchKey"></param>
        /// <returns></returns>
        static string[] SplitSearchWord(string searchKey)
        {
            searchKey = searchKey.Trim();
            var searchKeyArray = Regex.Split(searchKey, @"\s+");
            return searchKeyArray;
        }
        /// <summary>
        ///     关键字检索(Query.Matches)
        ///     指定项目列表里面，是不是包含某个指定关键字
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="searchKey"></param>
        /// <param name="setting"></param>
        /// <param name="addtionalQuery"></param>
        /// <returns></returns>
        public static List<T> SearchFuzzyByKeyWordList<T>(string[] searchArray, string[] fuzzyFields, Action<MongoCursor> setting, IMongoQuery addtionalQuery = null) where T : EntityBase, new()
        {
            IMongoQuery totalQuery = null;
            //或者关系查询关键字在非代码字段中
            foreach (var searchItem in searchArray)
            {
                foreach (var field in fuzzyFields)
                {
                    totalQuery = totalQuery == null ? Query.Matches(field, BsonRegularExpression.Create(new Regex(searchItem, RegexOptions.IgnoreCase))) : Query.Or(totalQuery, Query.Matches(field, BsonRegularExpression.Create(new Regex(searchItem, RegexOptions.IgnoreCase))));
                }
            }
            if (addtionalQuery != null)
            {
                totalQuery = Query.And(addtionalQuery, totalQuery);
            }
            return MongoDbRepository.GetRecList<T>(totalQuery, setting);
        }
        /// <summary>
        /// 关键字检索(TextIndex引擎)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="searchKey"></param>
        /// <param name="setting"></param>
        /// <param name="addtionalQuery"></param>
        /// <returns></returns>
        public static List<T> SearcKeyWordViaTextIndex<T>(string searchKey, Action<MongoCursor> setting, IMongoQuery addtionalQuery = null) where T : EntityBase, new()
        {
            var totalQuery = Query.Text(searchKey);
            if (addtionalQuery != null)
            {
                totalQuery = Query.And(addtionalQuery, totalQuery);
            }
            return MongoDbRepository.GetRecList<T>(totalQuery, setting);
        }
        /// <summary>
        ///     关键字检索(TextIndex引擎 + Query.Matches + Query.EQ For List)
        /// </summary>
        /// <typeparam name="T">返回记录泛型</typeparam>
        /// <param name="searchKey">关键字</param>
        /// <param name="fuzzyFields">普通文本的模糊匹配</param>
        /// <param name="listField">从列表中精确匹配，例如Tag列表字段</param>
        /// <param name="limit">长度限制</param>
        /// <param name="projectFieldList">返回字段列表</param>
        /// <param name="addtionalQuery">查询附加</param>
        /// <returns>检索结果列表</returns>
        public static List<T> SearchKeyByFuzzyAndTextIndex<T>(string searchKey, string[] fuzzyFields, string[] listField,int limit = 100,
                                                              string[] projectFieldList = null, IMongoQuery addtionalQuery = null) where T : EntityBase, new()
        {
            Action<MongoCursor> setting = null;
            if (projectFieldList != null)
            {
                setting = x => { x.SetFields(projectFieldList).SetLimit(limit); };
            }
            else
            {
                setting = x => { x.SetLimit(limit); };
            }
            //$text 和 Match 指令不能兼容
            var textIndexResult = SearcKeyWordViaTextIndex<T>(searchKey, setting, addtionalQuery);
            //关键字列表
            var searchArray = SplitSearchWord(searchKey);
            //模糊匹配
            var fuzzyResult = SearchFuzzyByKeyWordList<T>(searchArray, fuzzyFields, setting, addtionalQuery);
            //精确匹配
            var listResult = SearchListByKeyWord<T>(searchArray, listField, setting, addtionalQuery);
            //合并 + 去重
            textIndexResult.AddRange(fuzzyResult);
            textIndexResult.AddRange(listResult);
            return textIndexResult.Distinct(x => (x.Sn)).ToList();
        }



    }
}
