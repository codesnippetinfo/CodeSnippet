using System;

namespace InfraStructure.FilterSet
{
    [AttributeUsage(AttributeTargets.Property)]
    public class FilterItemAttribute : Attribute
    {
        /// <summary>
        /// 数据形式枚举
        /// </summary>
        public enum StructType
        {
            SingleMasterTable = 0,
            MultiMasterTable = 5,
            SingleEnum = 10,
            MultiEnum = 15,
            Datetime = 20,
            Boolean = 25,
            SingleMasterTableWithGrade =30,
            MultiMasterTableWithGrade = 35,
            SingleCatalogMasterTable = 40,
            MultiCatalogMasterTable = 45,
            /// <summary>
            /// 字符型Master
            /// </summary>
            /// <remarks>
            /// 例如毕业学校这样的字段，本质上也是一个Master
            /// 只是这样的Master很难去维护一个列表，
            /// 有时候需要按照这样的字段进行统计
            /// </remarks>
            SingleStringMaster = 50
        }
        
        /// <summary>
        /// 数据形式
        /// </summary>
        public StructType MetaStructType { get; set; }
        
        /// <summary>
        /// 元数据类型
        /// </summary>
        public Type MetaType { get; set; }

        /// <summary>
        /// 目录类型
        /// </summary>
        public Type CatalogType { set; get; }
        
        /// <summary>
        /// 评价类型
        /// </summary>
        public Type GradeType { set; get; }

    }
}
