using System;
using System.IO;
using System.Web;

namespace InfraStructure.Storage
{
    public static class FileSystemStorage
    {
        /// <summary>
        /// 保存文件的根目录
        /// </summary>
        private static string RootPath = string.Empty;
        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="root"></param>
        public static void Init(string root, string[] SubFolder)
        {
            RootPath = root;
            if (!Directory.Exists(root))
            {
                Directory.CreateDirectory(root);
            }
            foreach (var folder in SubFolder)
            {
                if (!Directory.Exists(root + folder))
                {
                    Directory.CreateDirectory(root + folder);
                }
            }
        }
        /// <summary>
        /// 写文件
        /// </summary>
        /// <param name="file"></param>
        /// <param name="filename"></param>
        /// <param name="ownerId"></param>
        /// <returns></returns>
        public static string PutFile(HttpPostedFile file, string ownerId, string SubFolder)
        {
            var strNow = DateTime.Now.ToString("yyyyMMddHHmmss");
            var saveFilename = ownerId + "_" + strNow + "_" + file.FileName;
            if (!Directory.Exists(RootPath + SubFolder + "\\" + strNow.Substring(0, 8)))
            {
                Directory.CreateDirectory(RootPath + SubFolder + "\\" + strNow.Substring(0, 8));
            }
            file.SaveAs(RootPath + SubFolder + "\\" + strNow.Substring(0, 8) + "\\" + saveFilename);
            return saveFilename;
        }
        /// <summary>
        /// 读文件
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        public static MemoryStream GetFile(string filename, string SubFolder)
        {
            var fullpath = RootPath + SubFolder + "\\" + filename.Substring(9, 8) + "\\" + filename;
            if (!File.Exists(fullpath)) return null;
            FileStream fs = new FileStream(fullpath, FileMode.Open, FileAccess.Read);
            //Read all bytes into an array from the specified file.
            int nBytes = (int)fs.Length;//计算流的长度
            byte[] byteArray = new byte[nBytes];//初始化用于MemoryStream的Buffer
            int nBytesRead = fs.Read(byteArray, 0, nBytes);//将File里的内容一次性的全部读到byteArray中去
            MemoryStream br = new MemoryStream(byteArray);//初始化MemoryStream,并将Buffer指向FileStream的读取结果数组
            return br;
        }
        /// <summary>
        /// 保存流为固定文件名
        /// </summary>
        /// <param name="file"></param>
        /// <param name="fixedFilename"></param>
        /// <param name="databaseType"></param>
        public static void InsertStreamWithFixFileName(Stream file, string fixedFilename, string SubFolder)
        {
            var fullpath = RootPath + SubFolder + "\\" + fixedFilename.Substring(9, 8) + "\\" + fixedFilename;
            if (!Directory.Exists(RootPath + SubFolder + "\\" + fixedFilename.Substring(9, 8)))
            {
                Directory.CreateDirectory(RootPath + SubFolder + "\\" + fixedFilename.Substring(9, 8));
            }
            Stream ToStream = File.Create(fullpath);
            BinaryReader br = new BinaryReader(file);
            BinaryWriter bw = new BinaryWriter(ToStream);
            bw.Write(br.ReadBytes((int)file.Length));
            bw.Flush();
            bw.Close();
            br.Close();
        }
    }
}
