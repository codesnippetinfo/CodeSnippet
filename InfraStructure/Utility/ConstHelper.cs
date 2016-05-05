using InfraStructure.Table;

namespace InfraStructure.Utility
{
    public static class ConstHelper
    {
        /// <summary>
        /// 持有者ID
        /// </summary>
        public const string OwnerId = nameof(OwnerTable.OwnerId);
        /// <summary>
        /// 员工
        /// </summary>
        public const string EmployeeInfoType = "EMPLOYEE";
        /// <summary>
        /// 用户名称
        /// </summary>
        public const string Username = "USERNAME";
        /// <summary>
        /// 账号
        /// </summary>
        public const string Account = "ACCOUNT";
        /// <summary>
        /// 出力
        /// </summary>
        public const string FieldOutPut = "FIELDOUTPUT";

        /// <summary>
        /// 权限
        /// </summary>
        public const string Privilege = "PRIVILEGE";

        /// <summary>
        /// Admin
        /// </summary>
        public const string AdminAccount = "000001";
        /// <summary>
        /// First Master Code
        /// </summary>
        public const string FirstMasterCode = "000001";

        /// <summary>
        /// New Record
        /// </summary>
        public const string NewRecordCode = "000000";

    }
}