using System;
using System.Collections.Generic;

namespace InfraStructure.Helper
{
    public static class ExternalType
    {
        /// <summary>
        /// 类型字典
        /// </summary>
        public static Dictionary<string, Type> ExternalTypeDictionary = new Dictionary<string, Type>();

        /// <summary>
        /// 枚举字典
        /// </summary>
        public static Dictionary<string, Type> ExternalEnumDictionary = new Dictionary<string, Type>();

        public static void AddType(Dictionary<string, Type> typeDic)
        {
            foreach (var item in typeDic)
            {
                if (!ExternalTypeDictionary.ContainsKey(item.Key))
                {
                    ExternalTypeDictionary.Add(item.Key, item.Value);
                }
            }
        }

        public static void AddEnum(Dictionary<string, Type> enumDic)
        {
            foreach (var item in enumDic)
            {
                if (!ExternalEnumDictionary.ContainsKey(item.Key))
                {
                    ExternalEnumDictionary.Add(item.Key, item.Value);
                }
            }
        }

        /// <summary>
        /// 获得枚举
        /// </summary>
        /// <param name="enumName"></param>
        /// <returns></returns>
        public static Type GetEnumByName(string enumName)
        {
            if (ExternalEnumDictionary.ContainsKey(enumName))
            {
                return ExternalEnumDictionary[enumName];
            }
            return null;
        }

        /// <summary>
        /// 获得类型
        /// </summary>
        /// <param name="typeFullName"></param>
        /// <returns></returns>
        public static Type GetTypeByName(string typeFullName)
        {
            if (ExternalTypeDictionary.ContainsKey(typeFullName))
            {
                return ExternalTypeDictionary[typeFullName];
            }
            return null;
        }
    }
}
