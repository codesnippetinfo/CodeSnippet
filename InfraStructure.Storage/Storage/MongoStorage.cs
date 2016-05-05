using System;
using System.Collections.Generic;
using System.Web;
using MongoDB.Driver.GridFS;
using System.IO;
using MongoDB.Driver;
using System.Drawing.Imaging;
using System.Drawing;

namespace InfraStructure.Storage
{
    public static class MongoStorage
    {
        /// <summary>
        ///     服务器
        /// </summary>
        private static MongoServer _innerServer;
        /// <summary>
        ///     链接字符串
        /// </summary>
        private static readonly string Connectionstring = @"mongodb://localhost:";

        /// <summary>
        ///     初始化MongoDB
        /// </summary>
        /// <param name="dbList">除去Logger以外</param>
        /// <param name="defaultDbName"></param>
        /// <param name="port"></param>
        /// <returns></returns>
        public static bool Init(string port = "28030")
        {
            try
            {
                _innerServer = new MongoClient(Connectionstring + port).GetServer();
                _innerServer.Connect();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        /// <summary>
        ///     保存文件
        /// </summary>
        /// <param name="file"></param>
        /// <param name="ownerId"></param>
        /// <param name="databaseType"></param>
        /// <returns></returns>
        public static string InsertFile(HttpPostedFileBase file, string ownerId, string databaseType)
        {
            var mongofilename = ownerId + "_" + DateTime.Now.ToString("yyyyMMddHHmmss") + "_" + file.FileName;
            var innerFileServer = _innerServer.GetDatabase(databaseType);
            var gfs = innerFileServer.GetGridFS(new MongoGridFSSettings());
            gfs.Upload(file.InputStream, mongofilename);
            return mongofilename;
        }
        /// <summary>
        ///     保存文件
        /// </summary>
        /// <param name="file"></param>
        /// <param name="ownerId"></param>
        /// <param name="databaseType"></param>
        /// <returns></returns>
        public static string InsertFile(HttpPostedFile file, string ownerId, string databaseType)
        {
            return InsertFile(new HttpPostedFileWrapper(file) as HttpPostedFileBase, ownerId, databaseType);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="file"></param>
        /// <param name="fileName"></param>
        /// <param name="ownerId"></param>
        /// <param name="databaseType"></param>
        /// <returns></returns>
        public static string InsertFile(Stream file, string fileName, string ownerId, string databaseType)
        {
            var mongofilename = ownerId + "_" + DateTime.Now.ToString("yyyyMMddHHmmss") + "_" + fileName;
            var innerFileServer = _innerServer.GetDatabase(databaseType);
            var gfs = innerFileServer.GetGridFS(new MongoGridFSSettings());
            gfs.Upload(file, mongofilename);
            return mongofilename;
        }
        /// <summary>
        /// 保存流为固定文件名
        /// </summary>
        /// <param name="file"></param>
        /// <param name="fixedFilename"></param>
        /// <param name="databaseType"></param>
        public static void InsertStreamWithFixFileName(Stream file, string fixedFilename, string databaseType)
        {
            var innerFileServer = _innerServer.GetDatabase(databaseType);
            var gfs = innerFileServer.GetGridFS(new MongoGridFSSettings());
            gfs.Upload(file, fixedFilename);
        }


        /// <summary>
        ///     获得文件
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="filename"></param>
        public static void GetFile(Stream stream, string filename, string databaseType)
        {
            var innerFileServer = _innerServer.GetDatabase(databaseType);
            var gfs = innerFileServer.GetGridFS(new MongoGridFSSettings());
            if (gfs.Exists(filename))
            {
                gfs.Download(stream, filename);
            }
        }
        /// <summary>
        ///     用户上传图片
        /// </summary>
        /// <param name="file"></param>
        /// <param name="ownerId"></param>
        /// <param name="weight"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        public static string InsertImage(HttpPostedFileBase file, string ownerId, int weight = 64, int height = 64)
        {
            var fileName = file.FileName;
            var originalImage = Image.FromStream(file.InputStream);
            var thumbImage = originalImage.GetThumbnailImage(weight, height, null, IntPtr.Zero);
            using (var ms = new MemoryStream())
            {
                thumbImage.Save(ms, ImageFormat.Jpeg);
                //必须将位置重置
                ms.Position = 0;
                fileName = InsertFile(ms, fileName, ownerId, string.Empty);
            }
            return fileName;
        }
        /// <summary>
        /// Mongo文件结构
        /// </summary>
        public struct DbFileInfo
        {
            /// <summary>
            /// 文件名
            /// </summary>
            public string FileName;
            /// <summary>
            /// 数据库文件名
            /// </summary>
            public string DbFileName;
        }
        /// <summary>
        /// 文件备份
        /// </summary>
        /// <param name="fileList">文件列表</param>
        /// <param name="path">备份路径。注意以斜线结尾</param>
        /// <param name="databaseType">数据库名称</param>
        public static void BackUpFiles(List<DbFileInfo> fileList, string path, string databaseType)
        {
            var innerFileServer = _innerServer.GetDatabase(databaseType);
            foreach (var item in fileList)
            {
                var gfs = innerFileServer.GetGridFS(new MongoGridFSSettings());
                gfs.Download(path + item.FileName, item.DbFileName);
            }
        }
    }
}
