namespace InfraStructure.Helper
{
    public enum WarningType
    {
        /// <summary>
        ///     无
        /// </summary>
        [EnumDisplayName("无")]
        None = 0,

        /// <summary>
        ///     主要
        /// </summary>
        [EnumDisplayName("主要")]
        Primary = 10,

        /// <summary>
        ///     成功
        /// </summary>
        [EnumDisplayName("成功")]
        Success = 20,

        /// <summary>
        ///     信息
        /// </summary>
        [EnumDisplayName("信息")]
        Info = 30,

        /// <summary>
        ///     警告
        /// </summary>
        [EnumDisplayName("警告")]
        Warning = 40,

        /// <summary>
        ///     危险
        /// </summary>
        [EnumDisplayName("危险")]
        Danger = 50
    }
}