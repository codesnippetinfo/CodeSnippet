namespace InfraStructure.Helper
{
    public enum CommonGrade
    {
        
        /// <summary>
        /// NotAvaliable
        /// </summary>
        [EnumDisplayName("N/A")]
        NotAvaliable = 0,
        
        /// <summary>
        /// 不可接受
        /// </summary>
        [EnumDisplayName("不可接受")]
        Unacceptable = 10,
        
        /// <summary>
        /// 最低限度的
        /// </summary>
        [EnumDisplayName("最低限度的")]
        Marginal = 20,
        
        /// <summary>
        /// 可接受
        /// </summary>
        [EnumDisplayName("可接受")]
        Acceptable = 30,
        
        /// <summary>
        /// 好于平均
        /// </summary>
        [EnumDisplayName("好于平均")]
        AboveArravage = 40,

        /// <summary>
        /// 杰出
        /// </summary>
        [EnumDisplayName("杰出")]
        Outstanding = 50
    }
}
