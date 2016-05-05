using System.ComponentModel;
using InfraStructure.DataBase;

namespace InfraStructure.Table
{
    public abstract class OwnerTable : EntityBase
    {
        /// <summary>
        /// 持有者号码
        /// </summary>
        public static string DefaultOwnerId = 1.ToString(OwnerIdFormat);

        /// <summary>
        /// 共通持有者号码
        /// </summary>
        public static string CommonOwnerId = new string("9".ToCharArray()[0], OwnerIdLength);

        /// <summary>
        ///     OwnerId格式
        /// </summary>
        public const string OwnerIdFormat = "D8";

        /// <summary>
        ///     OwnerId长度
        /// </summary>
        public const int OwnerIdLength = 8;

        /// <summary>
        ///     Code格式
        /// </summary>
        public const string CodeFormat = "D6";

        /// <summary>
        ///     Code长度
        /// </summary>
        public const int CodeLength = 6;

        /// <summary>
        /// 
        /// </summary>
        public const string SystemImport = "SYSTEM_IMPORT";

        /// <summary>
        ///     组织编号
        /// </summary>
        public string OwnerId;

        /// <summary>
        ///     组织内部编号
        /// </summary>
        [DisplayName("编号")]
        public string Code { set; get; }

        /// <summary>
        ///     采番
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string GetNewCodeByOwnerId(OwnerTable obj)
        {
            var count = OwnerTableOperator.GetCountByOwnerId(obj.GetCollectionName(), obj.OwnerId, true);
            return (count + 1).ToString(CodeFormat);
        }

        /// <summary>
        ///     采番
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string GetCodeByCreateUser(OwnerTable obj)
        {
            var count = OwnerTableOperator.GetCountByCreateUser(obj.GetCollectionName(), obj.CreateUser, true);
            return (count + 1).ToString(CodeFormat);
        }

        /// <summary>
        ///     采番
        /// </summary>
        /// <param name="collectionName"></param>
        /// <param name="ownerId"></param>
        /// <returns></returns>
        public static string GetNewCodeByOwnerId(string collectionName, string ownerId)
        {
            var count = OwnerTableOperator.GetCountByOwnerId(collectionName, ownerId, true);
            return (count + 1).ToString(CodeFormat);
        }

        /// <summary>
        ///     采番
        /// </summary>
        /// <param name="collectionName"></param>
        /// <param name="accountId"></param>
        /// <returns></returns>
        public static string GetCodeByCreateUser(string collectionName, string accountId)
        {
            var count = OwnerTableOperator.GetCountByCreateUser(collectionName, accountId, true);
            return (count + 1).ToString(CodeFormat);
        }

    }

}