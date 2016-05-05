using BlogSystem.Entity;
using InfraStructure.Misc;
using InfraStructure.Storage;
using Newtonsoft.Json;
using System.Configuration;
using System.Drawing;
using System.Drawing.Imaging;
using System.Web;

namespace CodeSnippet.API
{
    /// <summary>
    /// Summary description for upload
    /// </summary>
    public class Upload : IHttpHandler
    {
        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/json";
            string ownerId = context.Request["OwnerId"];
            string ArticleId = context.Request["ArticleId"];
            var httpfile = context.Request.Files[0];

            if (UploadFile.IsAboveLimit(httpfile.ContentLength, ownerId))
            {
                var AboveLimit = new
                {
                    success = ConstHelper.Fail,
                    message = "上传失败,上传资源超过最大值限制！！"
                };
                context.Response.Write(JsonConvert.SerializeObject(AboveLimit));
                return;
            }

            UploadFile upload = new UploadFile();
            upload.Size = httpfile.ContentLength;

            Image orgImage = Image.FromStream(httpfile.InputStream);
            //保存原图到七牛
            httpfile.InputStream.Position = 0;
            string strImage = ConfigurationManager.AppSettings["Image"];
            var Savefilename = string.Empty;
            switch (strImage)
            {
                case "QiNue":
                    Savefilename = QiniuStorage.upload(httpfile, ownerId);
                    break;
                case "Mongo":
                    Savefilename = MongoStorage.InsertFile(httpfile, ownerId, "Image");
                    break;
                case "FileSystem":
                    Savefilename = FileSystemStorage.PutFile(httpfile, ownerId, "Image");
                    break;
            }

            string ThumbnailUrl = "/FileSystem/Thumbnail?filename=" + Savefilename;
            string Thumbnail = ConfigurationManager.AppSettings["Thumbnail"];
            if (httpfile.FileName.ToLower().EndsWith(".gif"))
            {
                //直接保存
                switch (Thumbnail)
                {
                    case "Mongo":
                        MongoStorage.InsertFile(httpfile, ownerId, "Thumbnail");
                        break;
                    case "FileSystem":
                        Savefilename = FileSystemStorage.PutFile(httpfile, ownerId, "Thumbnail");
                        break;
                }
                upload.SmallFileSize = upload.Size;
            }
            else
            {
                //这里是拉伸图片，将图片改为BMP（PDF和画面折衷后的决定）
                var BitmapImage = ImageHelper.ResizeImage(System.Math.Min(800, orgImage.Width), orgImage.Height, ImageHelper.ResizeMode.W, orgImage);
                //string Smallfilename = MongoStorage.InsertFile(httpfile, ownerId, "Bussiness");
                //返回的名字以DefaultOwnerId开始
                var SmallStream = StreamConvert.BytesToStream(StreamConvert.BitmapToBytes(BitmapImage, ImageFormat.Gif));
                //保存有时间差，所以使用上一步保存时候的文件名称
                //同时Stream关闭后无法获得长度，长度必须先写
                upload.SmallFileSize = (int)SmallStream.Length;
                switch (Thumbnail)
                {
                    case "Mongo":
                        MongoStorage.InsertStreamWithFixFileName(SmallStream, Savefilename, "Thumbnail");
                        break;
                    case "FileSystem":
                        FileSystemStorage.InsertStreamWithFixFileName(SmallStream, Savefilename, "Thumbnail");
                        break;
                }
            }


            upload.Name = Savefilename;
            upload.ArticleID = ArticleId;
            UploadFile.InsertUploadFile(upload, ownerId);
            string strfree = UploadFile.GetFreeVolumnByAccountId(ownerId);
            var result = new
            {
                success = ConstHelper.Success,
                message = "上传成功,剩余图片空间：" + strfree,
                url = ThumbnailUrl
            };
            string json = JsonConvert.SerializeObject(result);
            context.Response.Write(json);
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}