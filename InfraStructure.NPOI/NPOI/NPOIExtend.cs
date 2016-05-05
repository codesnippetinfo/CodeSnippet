using System;
using System.Collections.Generic;
using NPOI.SS.UserModel;

namespace InfraStructure.Utility
{
    public static class NpoiExtend
    {
        /// <summary>
        /// 获得Header
        /// </summary>
        /// <param name="fieldName"></param>
        /// <param name="getName"></param>
        /// <param name="outputFields"></param>
        /// <param name="colcnt"></param>
        /// <param name="header"></param>
        /// <param name="typeObj"></param>
        public static void CreateHeader(string fieldName, Func<string, string> getName,
                                        List<string> outputFields, ref int colcnt, IRow header, Type typeObj)
        {
            if (outputFields.Contains(fieldName))
            {
                header.CreateCell(colcnt).SetCellValue(getName(fieldName));
                colcnt++;
            }
        }
        /// <summary>
        ///     文本属性
        /// </summary>
        /// <param name="cell"></param>
        /// <returns></returns>
        public static string GetCellText(this ICell cell)
        {
            var strText = string.Empty;
            switch (cell.CellType)
            {
                case CellType.Blank:
                    strText = string.Empty;
                    break;
                case CellType.Boolean:
                    strText = cell.BooleanCellValue ? "是" : "否";
                    break;
                case CellType.Error:
                    break;
                case CellType.Formula:
                    //这里无法判断Formula的返回值是什么类型
                    //只能做一个近似处理
                    try
                    {
                        strText = cell.StringCellValue;
                    }
                    catch (Exception)
                    {
                        try
                        {
                            //如果结果是数字的话
                            strText = cell.NumericCellValue.ToString();
                        }
                        catch (Exception)
                        {
                            // ignored
                        }
                    }
                    break;
                case CellType.Numeric:
                    strText = cell.ToString();
                    break;
                case CellType.String:
                    strText = cell.StringCellValue;
                    break;
                case CellType.Unknown:
                    strText = string.Empty;
                    break;
                default:
                    break;
            }
            return strText;
        }

        /// <summary>
        /// 数字转CODE
        /// </summary>
        /// <param name="ICell"></param>
        /// <param name="length"></param>
        public static string GetCode(this ICell cell, int length = 6)
        {
            if (cell.CellType == CellType.String)
            {
                var strInt = cell.StringCellValue;
                if (string.IsNullOrEmpty(strInt)) return string.Empty;
                int parseInt;
                if (int.TryParse(strInt, out parseInt))
                {
                    return parseInt.ToString("D" + length);
                }
                return string.Empty;
            }
            if ((int)cell.NumericCellValue == 0)
            {
                return string.Empty;
            }
            return ((int)cell.NumericCellValue).ToString("D" + length); ;
        }


        /// <summary>
        /// 将字符转为日期，错误则返回默认值
        /// </summary>
        /// <param name="StringInt"></param>
        /// <param name="defaultValue">默认值</param>
        /// <returns></returns>
        public static DateTime GetDate(this ICell cell, DateTime defaultValue)
        {
            if (cell.CellType == CellType.String)
            {
                var strDate = cell.StringCellValue;
                if (string.IsNullOrEmpty(strDate)) return defaultValue;
                DateTime parseDate;
                if (DateTime.TryParse(strDate, out parseDate))
                {
                    return parseDate;
                }
                return defaultValue;
            }
            //如果是空的时候，DateCellValue等于MaxValue
            //这里居然没有DateType
            if (cell.DateCellValue != DateTime.MaxValue)
            {
                return cell.DateCellValue;
            }
            return defaultValue;
        }
        /// <summary>
        ///     整形属性
        /// </summary>
        /// <param name="strDate"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static int GetInt(this ICell cell, int defaultValue)
        {
            if (cell.CellType == CellType.String)
            {
                var strInt = cell.StringCellValue;
                if (string.IsNullOrEmpty(strInt)) return defaultValue;
                int parseInt;
                if (int.TryParse(strInt, out parseInt))
                {
                    return parseInt;
                }
                return defaultValue;
            }
            return (int)cell.NumericCellValue;
        }
    }
}
