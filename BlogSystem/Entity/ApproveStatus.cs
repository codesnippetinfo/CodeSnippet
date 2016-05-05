public enum ApproveStatus
{
    /// <summary>
    /// 未发布(所有文章公开文章初识状态)
    /// </summary>
    None = 0,
    /// <summary>
    /// 私有(无需发布状态)
    /// </summary>
    NotNeed = 5, 
    /// <summary>
    /// 申请中
    /// </summary>
    Pending = 10,
    /// <summary>
    /// 拒绝
    /// </summary>
    Reject = 90,
    /// <summary>
    /// 接受
    /// </summary>
    Accept = 99,
}
