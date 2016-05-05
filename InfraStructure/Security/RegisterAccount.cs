using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using InfraStructure.DataBase;
using MongoDB.Driver.Builders;

namespace InfraStructure.Security
{
    public class RegisterAccount
    {
        /// <summary>
        ///     用户名
        /// </summary>
        [DisplayName("用户名")]
        [Required]
        [RegularExpression("\\S{4,60}", ErrorMessage = "4-60位非空字符")]
        public string User { get; set; }

        /// <summary>
        ///     密码
        /// </summary>
        [DisplayName("旧密码")]
        [Required]
        [DataType(DataType.Password)]
        public string OldPassword { get; set; }

        /// <summary>
        ///     密码
        /// </summary>
        [DisplayName("密码")]
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        /// <summary>
        ///     密码
        /// </summary>
        [DisplayName("确认密码")]
        [Required]
        [Compare("Password", ErrorMessage = "密码和确认密码不相同")]
        [DataType(DataType.Password)]
        public string RePassword { get; set; }

        /// <summary>
        ///     Email
        /// </summary>
        [DisplayName("电子邮件")]
        [Required]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        /// <summary>
        /// 是否存在用户名
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        public static bool IsUserNameExist(string userName)
        {
            var cnt = MongoDbRepository.GetRecordCount(Account.CollectionName, Query.EQ("User", userName));
            return (cnt == 1);
        }

        public static bool IsEmailExist(string email)
        {
            var cnt = MongoDbRepository.GetRecordCount(Account.CollectionName, Query.EQ("Email", email));
            return (cnt == 1);
        }

    }
}