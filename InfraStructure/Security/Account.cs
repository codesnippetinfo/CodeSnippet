using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using InfraStructure.DataBase;
using InfraStructure.Table;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver.Builders;

namespace InfraStructure.Security
{
    /// <summary>
    /// 用户帐号
    /// 必须属于只能属于一个Owner
    /// </summary>
    public class Account : OwnerTable
    {
        #region "model"

        /// <summary>
        ///     用户名
        /// </summary>
        [DisplayName("用户名")]
        [Required]
        public string User { get; set; }

        /// <summary>
        ///     密码
        /// </summary>
        [DisplayName("密码")]
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        /// <summary>
        ///     电子邮件
        /// </summary>
        [DisplayName("电子邮件")]
        [Required]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        /// <summary>
        /// 权限
        /// </summary>
        [DisplayName("权限")]
        public int Privilege { get; set; }

        /// <summary>
        ///     数据集名称
        /// </summary>
        public override string GetCollectionName()
        {
            return "Account";
        }

        /// <summary>
        ///     数据集名称静态字段
        /// </summary>
        public static string CollectionName = "Account";


        /// <summary>
        ///     数据主键前缀
        /// </summary>
        public override string GetPrefix()
        {
            return string.Empty;
        }

        /// <summary>
        ///     数据主键前缀静态字段
        /// </summary>
        public static string Prefix = string.Empty;

        /// <summary>
        ///     Mvc画面的标题
        /// </summary>
        [BsonIgnore] public static string MvcTitle = "帐户";


        /// <summary>
        ///     用户验证
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public static Account CheckUser(string username, string password)
        {
            var query = Query.EQ("User", username);
            var account = MongoDbRepository.GetFirstRec<Account>(CollectionName, query);
            //没有该用户
            if (account == null) return null;
            //CheckPassword只能对于UserName进行检查
            //Password的MD5值构造的时候，适用UserName的MD5作为Salt
            if (Authority.CheckPassword(account.User, password, account.Password))
            {
                return account;
            }
            //验证错误
            return null;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="email"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public static Account CheckEmail(string email, string password)
        {
            var query = Query.EQ("Email", email);
            var account = MongoDbRepository.GetFirstRec<Account>(CollectionName, query);
            //没有该用户
            if (account == null) return null;
            //CheckPassword只能对于UserName进行检查
            //Password的MD5值构造的时候，适用UserName的MD5作为Salt
            if (Authority.CheckPassword(account.User, password, account.Password))
            {
                return account;
            }
            //验证错误
            return null;
        }
        #endregion
    }
}