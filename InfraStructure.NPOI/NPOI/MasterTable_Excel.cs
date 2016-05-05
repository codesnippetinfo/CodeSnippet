using InfraStructure.DataBase;
using InfraStructure.Helper;
using InfraStructure.Utility;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Web;

namespace InfraStructure.Table
{
    /// <summary>
    /// 辅助表
    /// </summary>
    public abstract class MasterTable_Excel : MasterTable
    {
        #region Import & Export
        /// <summary>
        /// 
        /// </summary>
        /// <param name="masterNameList"></param>
        /// <param name="ownerId"></param>
        /// <returns></returns>
        public static MemoryStream ExportToExcel(List<string> masterNameList, string ownerId)
        {
            var workbook = new HSSFWorkbook();
            var ms = new MemoryStream();
            foreach (var mastername in masterNameList)
            {
                ExportToExcelSheet(workbook.CreateSheet(mastername), OwnerTableOperator.GetRecListByOwnerId<MasterWrapper>(mastername, ownerId)); ;
            }
            workbook.Write(ms);
            workbook = null;
            return ms;
        }

        /// <summary>
        /// 导出到Excel
        /// </summary>
        /// <param name="masterList"></param>
        /// <returns></returns>
        private static void ExportToExcelSheet<T>(ISheet sheet, List<T> masterList) where T : MasterTable
        {
            var colcnt = 0;
            var rowcnt = 0;
            // Header
            var header = sheet.CreateRow(rowcnt); rowcnt++;
            colcnt = 0;

            var typeObj = typeof(T);
            Func<string, string> getName = x =>
            {
                return CacheSystem.GetDisplayName(x, typeObj);
            };

            header.CreateCell(colcnt).SetCellValue(nameof(Code)); colcnt++;
            header.CreateCell(colcnt).SetCellValue(getName(nameof(MasterTable.Name))); colcnt++;
            header.CreateCell(colcnt).SetCellValue(getName("Description")); colcnt++;
            header.CreateCell(colcnt).SetCellValue(getName("Rank")); colcnt++;
            header.CreateCell(colcnt).SetCellValue(getName("IsActive")); colcnt++;

            for (var i = 0; i < masterList.Count; i++)
            {
                var t = masterList[i];
                var row = sheet.CreateRow(rowcnt); rowcnt++;
                colcnt = 0;
                row.CreateCell(colcnt).SetCellValue(t.Code); colcnt++;
                //BAD SMELL！！
                row.CreateCell(colcnt).SetCellValue(t.Name); colcnt++;
                row.CreateCell(colcnt).SetCellValue(t.Description); colcnt++;
                row.CreateCell(colcnt).SetCellValue(t.Rank); colcnt++;
                row.CreateCell(colcnt).SetCellValue(t.IsActive ? "是" : "否"); colcnt++;
            }
        }

        /// <summary>
        /// 从Excel导入
        /// </summary>
        /// <param name="file"></param>
        /// <param name="ownerId"></param>
        /// <returns></returns>
        public static string ImportFromExcelSheet(HttpPostedFileBase file, string collectionName, string ownerId)
        {
            var strResult = string.Empty;
            var colcnt = 0;
            try
            {
                var excelFileStream = file.InputStream;
                IWorkbook workbook = new HSSFWorkbook(excelFileStream);
                var sheet = workbook.GetSheetAt(0);
                var rowCount = sheet.LastRowNum;
                var masterList = new List<MasterWrapper>();
                var errorFlg = false;
                for (var i = 1; i <= rowCount; i++)
                {
                    var row = sheet.GetRow(i);
                    if (string.IsNullOrEmpty(row.GetCell(1).ToString())) break;
                    colcnt = 0;
                    var code = row.GetCell(colcnt, MissingCellPolicy.CREATE_NULL_AS_BLANK).ToString(); colcnt++;
                    MasterWrapper t = null;
                    if (string.IsNullOrEmpty(code))
                    {
                        t = new MasterWrapper();
                    }
                    else
                    {
                        t = OwnerTableOperator.GetRecByCodeAtOwner<MasterWrapper>(collectionName, ownerId, code);
                        if (t == null)
                        {
                            strResult += "记录号码为:[" + code + "]的记录没有找到！" + Environment.NewLine;
                            errorFlg = true;
                            continue;
                        }
                        t.Code = code;
                    }
                    t.OwnerId = ownerId;
                    t.Name = row.GetCell(colcnt, MissingCellPolicy.CREATE_NULL_AS_BLANK).ToString(); colcnt++;
                    t.Description = row.GetCell(colcnt, MissingCellPolicy.CREATE_NULL_AS_BLANK).ToString(); colcnt++;
                    t.Rank = row.GetCell(colcnt, MissingCellPolicy.CREATE_NULL_AS_BLANK).GetInt(0); colcnt++;
                    t.IsActive = row.GetCell(colcnt, MissingCellPolicy.CREATE_NULL_AS_BLANK).ToString().Equals("是"); colcnt++;
                    masterList.Add(t);
                }
                if (!errorFlg)
                {
                    var insertCnt = 0;
                    var upDateCnt = 0;
                    for (var i = 0; i < masterList.Count; i++)
                    {
                        //根据有无Code进行不同操作
                        if (string.IsNullOrEmpty(masterList[i].Code))
                        {
                            //新增模式
                            masterList[i].Code = GetNewCodeByOwnerId(collectionName, ownerId);
                            MongoDbRepository.InsertRec(masterList[i], collectionName, "SYSTEM_IMPORT");
                            insertCnt++;
                        }
                        else
                        {
                            //修改模式
                            MongoDbRepository.UpdateRec(masterList[i], collectionName, "SYSTEM_IMPORT");
                            upDateCnt++;
                        }
                    }
                    strResult = "全体件数:" + masterList.Count + Environment.NewLine;
                    strResult += "追加件数:" + insertCnt + Environment.NewLine;
                    strResult += "修改件数:" + upDateCnt + Environment.NewLine;
                }
            }
            catch (Exception ex)
            {
                strResult = ex.ToString();
            }
            return strResult;
        }
        #endregion

    }

}