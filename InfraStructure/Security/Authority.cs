using System.Security.Cryptography;
using System.Text;

namespace InfraStructure.Security
{
    public static class Authority
    {

        #region Password
        /// <summary>
        ///     获得MD5
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string GetMd5(string value)
        {
            var bytes = Encoding.ASCII.GetBytes(value);
            var sb = new StringBuilder();
            MD5 hash = new MD5CryptoServiceProvider();
            bytes = hash.ComputeHash(bytes);
            foreach (var b in bytes)
            {
                sb.AppendFormat("{0:x2}", b);
            }
            return sb.ToString();
        }
        /// <summary>
        ///     获得指定用户名密码组合的加密秘码
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public static string GetPassword(string username, string password)
        {
            var salt = GetMd5(username).Substring(8, 8);
            return GetMd5(salt + password);
        }
        /// <summary>
        ///     密码验证
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public static bool CheckPassword(string username, string password, string md5Password)
        {
            return md5Password.Equals(GetPassword(username, password));
        }

        #endregion
    }
}