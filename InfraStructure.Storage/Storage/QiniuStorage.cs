using Qiniu.IO;
using Qiniu.RS;
using System;
using System.IO;
using System.Net;
using System.Web;

namespace InfraStructure.Storage
{
    public static class QiniuStorage
    {
        /// <summary>
        /// ACCESS_KEY
        /// </summary>
        private static string ACCESS_KEY = "<YOUR_APP_ACCESS_KEY>";
        /// <summary>
        /// SECRET_KEY
        /// </summary>
        private static string SECRET_KEY = "<YOUR_APP_SECRET_KEY>";
        /// <summary>
        /// BUCKET
        /// </summary>
        private static string BUCKET = "<BUCKET>";
        /// <summary>
        /// URLBASE
        /// </summary>
        private static string URLBASE = "<URLBASE>";
        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="accesskey"></param>
        /// <param name="secretkey"></param>
        public static void Init(string accesskey, string secretkey, string Bucket, string urlbase)
        {
            ACCESS_KEY = accesskey;
            SECRET_KEY = secretkey;
            BUCKET = Bucket;
            URLBASE = urlbase;
            Qiniu.Conf.Config.ACCESS_KEY = ACCESS_KEY;
            Qiniu.Conf.Config.SECRET_KEY = SECRET_KEY;
        }

        public static string upload(HttpPostedFile file, string ownerId)
        {
            //设置账号的AK和SK
            IOClient target = new IOClient();
            PutExtra extra = new PutExtra();
            //设置上传的空间
            string bucket = BUCKET;
            //设置上传的文件的key值
            var filekey = ownerId + "_" + DateTime.Now.ToString("yyyyMMddHHmmss") + "_" + file.FileName;
            //普通上传,只需要设置上传的空间名就可以了,第二个参数可以设定token过期时间
            PutPolicy put = new PutPolicy(bucket, 3600);
            //调用Token()方法生成上传的Token
            string upToken = put.Token();
            PutRet ret = target.Put(upToken, filekey, file.InputStream, extra);
            return filekey;
        }
        /// <summary>
        ///     私钥下载的URL做成
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="filename"></param>
        public static string GetPolicyFileUrl(string filename)
        {
            //构造私有空间的需要生成的下载的链接
            var url = URLBASE + filename;
            //调用MakeRequest方法生成私有下载链接
            return GetPolicy.MakeRequest(url);
        }

        public static MemoryStream GetFile(string url)
        {
            MemoryStream ms = new MemoryStream();
            BinaryWriter bw = new BinaryWriter(ms);
            HttpWebRequest myHttpWebRequest = WebRequest.Create(url) as HttpWebRequest;
            myHttpWebRequest.KeepAlive = false;
            myHttpWebRequest.AllowAutoRedirect = false;
            myHttpWebRequest.UserAgent = "Mozilla/5.0 (compatible; MSIE 6.0; Windows NT 5.1; SV1; .NET CLR 2.0.50727)";
            myHttpWebRequest.Timeout = 10000;
            myHttpWebRequest.ContentType = "application/x-www-form-urlencoded;charset=utf-8";
            using (HttpWebResponse res = (HttpWebResponse)myHttpWebRequest.GetResponse())
            {
                //返回为200或206
                if (res.StatusCode == HttpStatusCode.OK || res.StatusCode == HttpStatusCode.PartialContent)
                {
                    string dd = res.ContentEncoding;
                    var stream = res.GetResponseStream();
                    BinaryReader br = new BinaryReader(stream);
                    bw.Write(br.ReadBytes((int)stream.Length));
                }
            }
            return ms;
        }

    }
}
