using System.IO;

namespace InfraStructure.Utility
{
    public static class TempFileExtend
    {
        /// <summary>
        /// Excel Content Type
        /// </summary>
        public const string ExcelContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
        /// <summary>
        /// 
        /// </summary>
        public const string PlainTextContentType = "text/plain";

        /// <summary>
        /// 
        /// </summary>
        public static string ResumeBackPath = string.Empty;
        /// <summary>
        /// 
        /// </summary>
        public static string SourceCodePath = string.Empty;

        /// <summary>
        ///     获取图表文件名称
        /// </summary>
        /// <param name="ownerId"></param>
        /// <returns></returns>
        public static string GetChartFileName(string ownerId, string imageName)
        {
            return "~/Temp/" + ownerId + "_" + imageName + ".jpeg";
        }

        /// <summary>
        ///     获取Excel文件名称
        /// </summary>
        /// <param name="ownerId"></param>
        /// <param name="excelName"></param>
        /// <returns></returns>
        public static string GetExcelFileName(string ownerId, string excelName)
        {
            return "~/Temp/" + ownerId + "_" + excelName + ".xls";
        }

        /// <summary>
        ///     清除所有图表缓存
        /// </summary>
        /// <param name="rootPath">服务器根路径</param>
        public static void ClearAllTempFiles(string rootPath)
        {
            var tempPath = rootPath + "/Temp/";
            if (Directory.Exists(tempPath))
            {
                Directory.Delete(tempPath, true);
            }
            Directory.CreateDirectory(tempPath);

            ResumeBackPath = rootPath + "/Temp/Resume/";
            if (Directory.Exists(ResumeBackPath))
            {
                Directory.Delete(ResumeBackPath, true);
            }
            Directory.CreateDirectory(ResumeBackPath);


            SourceCodePath = rootPath + "/Temp/Code/";
            if (Directory.Exists(SourceCodePath))
            {
                Directory.Delete(SourceCodePath, true);
            }
            Directory.CreateDirectory(SourceCodePath);

        }
    }
}