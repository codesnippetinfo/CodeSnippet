using System;
using System.Collections.Generic;

namespace InfraStructure.Misc
{
    public static class StringExtend
    {

        /// <summary>
        /// 获得字符枚举值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="strEnum"></param>
        /// <returns></returns>
        public static T GetEnum<T>(this string strEnum, T Default)
        {
            if (string.IsNullOrEmpty(strEnum)) return Default;
            try
            {
                var enumValue = (T)Enum.Parse(typeof(T), strEnum);
                return enumValue;
            }
            catch (Exception)
            {
                return Default;
            }
        }

        /// <summary>
        ///     Size的文字表达
        /// </summary>
        /// <param name="size"></param>
        /// <returns></returns>
        public static string GetSize(long size)
        {
            if (size < 0) return string.Empty;
            string[] unit =
            {
                "Byte", "KB", "MB", "GB", "TB"
            };
            if (size == 0)
            {
                return "0 Byte";
            }
            byte unitOrder = 2;
            var tempSize = size / Math.Pow(2, 20);
            while (!(tempSize > 0.1 & tempSize < 1000))
            {
                if (tempSize < 0.1)
                {
                    tempSize = tempSize * 1024;
                    unitOrder--;
                }
                else
                {
                    tempSize = tempSize / 1024;
                    unitOrder++;
                }
            }
            return string.Format("{0:F2}", tempSize) + " " + unit[unitOrder];
        }

        /// <summary>
        ///     将字符列表转换为字符串
        /// </summary>
        /// <param name="list">字符列表</param>
        /// <param name="splitString">连接用字符</param>
        /// <param name="trimEnd">是否去掉尾部连接字符</param>
        /// <returns></returns>
        public static string GetJoinString(this List<string> list, string splitString, bool trimEnd = true)
        {
            var joinString = string.Empty;
            for (var i = 0; i < list.Count; i++)
            {
                joinString += list[i];
                if (i != list.Count - 1)
                {
                    joinString += splitString;
                }
                else
                {
                    if (!trimEnd) joinString += splitString;
                }
            }
            return joinString;
        }

        /// <summary>
        /// 换行处理
        /// </summary>
        /// <param name="longText"></param>
        /// <returns></returns>
        public static string ChangeNewLineToBr(this string longText,string brMark = "<br />")
        {
            if (string.IsNullOrEmpty(longText)) return string.Empty;
            return longText.Replace(Environment.NewLine, brMark);
        }
    }
}