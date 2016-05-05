using System.Collections.Generic;

namespace InfraStructure.Helper
{
    public static class EntityHelper
    {
        /// <summary>
        ///     获得可以用于画面表示和导出的字段集合
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static Dictionary<string, string> GetFieldsDic<T>()
        {
            var type = typeof(T);
            var fieldNameDic = new Dictionary<string, string>();
            foreach (var item in type.GetMembers())
            {
                //有些方法也是有DisplayName的，例如离职函数，在职月数等
                var display = CacheSystem.GetDisplayNameCache(type.FullName + "." + item.Name);
                if (!string.IsNullOrEmpty(display))
                {
                    fieldNameDic.Add(item.Name, display);
                }
            }
            return fieldNameDic;
        }
        /// <summary>
        ///     获得可以用于画面表示和导出的字段集合
        /// </summary>
        /// <param name="modelName"></param>
        /// <returns></returns>
        public static Dictionary<string, string> GetFieldsDic(string modelName,bool includeMethod = true)
        {
            var type = ExternalType.GetTypeByName(modelName);
            var fieldNameDic = new Dictionary<string, string>();
            if (includeMethod)
            {
                foreach (var item in type.GetMembers())
                {
                    //有些方法也是有DisplayName的，例如离职函数，在职月数等
                    var display = CacheSystem.GetDisplayNameCache(type.FullName + "." + item.Name);
                    if (!string.IsNullOrEmpty(display))
                    {
                        fieldNameDic.Add(item.Name, display);
                    }
                }
            }
            else
            {
                foreach (var item in type.GetProperties())
                {
                    //有些方法也是有DisplayName的，例如离职函数，在职月数等
                    var display = CacheSystem.GetDisplayNameCache(type.FullName + "." + item.Name);
                    if (!string.IsNullOrEmpty(display))
                    {
                        fieldNameDic.Add(item.Name, display);
                    }
                }
            }
            return fieldNameDic;
        }

    }
}
