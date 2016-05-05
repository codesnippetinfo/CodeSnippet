using System;
using System.Collections.Generic;
using System.Web.Mvc;
using InfraStructure.Helper;

namespace InfraStructure.Utility
{
    public class EnumExt
    {
        /// <summary>
        ///     根据枚举成员获取自定义属性EnumDisplayNameAttribute的属性DisplayName
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        public static string GetEnumCustomDescription(object e)
        {
            //获取枚举的Type类型对象
            var t = e.GetType();
            //获取枚举的所有字段
            var ms = t.GetFields();
            //遍历所有枚举的所有字段
            foreach (var f in ms)
            {
                if (f.Name != e.ToString())
                {
                    continue;
                }
                //第二个参数true表示查找EnumDisplayNameAttribute的继承链
                if (f.IsDefined(typeof (EnumDisplayNameAttribute), true))
                {
                    return
                        (f.GetCustomAttributes(typeof (EnumDisplayNameAttribute), true)[0] as EnumDisplayNameAttribute)
                            .DisplayName;
                }
            }
            //如果没有找到自定义属性，直接返回属性项的名称
            return e.ToString();
        }

        /// <summary>
        ///     根据枚举，把枚举自定义特性EnumDisplayNameAttribut的Display属性值撞到SelectListItem中
        /// </summary>
        /// <param name="enumType">枚举</param>
        /// <returns></returns>
        public static List<SelectListItem> GetSelectList(Type enumType)
        {
            var selectList = new List<SelectListItem>();
            foreach (var e in Enum.GetValues(enumType))
            {
                selectList.Add(new SelectListItem {Text = GetEnumCustomDescription(e), Value = ((int) e).ToString()});
            }
            return selectList;
        }
    }
}