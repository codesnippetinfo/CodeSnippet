namespace CodeSnippet
{
    public class ConstHelper
    {
        /// <summary>
        /// USERID(OWNERID FOR OWNERTABLE,SN FOR ENTITY)
        /// </summary>
        public const string Session_USERID = "USERID";
        /// <summary>
        /// NAME
        /// </summary>
        public const string Session_NAME = "NAME";
        /// <summary>
        /// 头像
        /// </summary>
        public const string Session_AVATAR = "AVATAR";
        /// <summary>
        /// 权限
        /// </summary>
        public const string Session_PRIVILEGE = "PRIVILEGE";

        /// <summary>
        /// 成功
        /// </summary>
        public const int Success = 1;
        /// <summary>
        /// 失败
        /// </summary>
        public const int Fail = 0;
        /// <summary>
        /// 图片格式
        /// </summary>
        public const string ImageFileExtend = "['jpg','JPG','jpeg','JPEG', 'gif','GIF', 'png','PNG','bmp','BMP', 'webp','WEBP']";

        /// <summary>
        /// MongoTextSearch
        /// </summary>
        public const string MongoTextSearch = "MongoTextSearch";
        /// <summary>
        /// ElasticSearch
        /// </summary>
        public const string ElasticSearch = "ElasticSearch";

    }
}