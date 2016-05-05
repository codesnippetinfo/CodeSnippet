using System;

namespace InfraStructure.Helper
{
    /// <summary>
    /// 带评价的项目
    /// MongoDB不能反序列化结构，只能反序列化类
    /// </summary>
    [Serializable]
    public class ItemWithGrade
    {
        /// <summary>
        /// 项目
        /// </summary>
        public string MasterCode;
        /// <summary>
        /// 级别
        /// </summary>
        public CommonGrade Grade;

    }
}
