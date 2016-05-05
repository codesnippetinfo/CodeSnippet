using System;
using System.Collections.Generic;
using System.IO;
using InfraStructure.Utility;
using NPOI.HSSF.UserModel;
using NPOI.POIFS.FileSystem;
using NPOI.SS.UserModel;
using InfraStructure.FilterSet;
using System.Web;
using InfraStructure.Helper;
using InfraStructure.DataBase;

namespace InfraStructure.Table
{
    public static class OwnerTableExcelHelper
    {
     
        /// <summary>
        ///     导出到Excel
        /// </summary>
        /// <param name="recordList"></param>
        /// <param name="outPutFields"></param>
        /// <returns></returns>
        public static MemoryStream ExportToExcel<T>(List<T> recordList, List<string> outPutFields)
            where T : OwnerTable, new()
        {
            var workbook = new HSSFWorkbook();
            var ms = new MemoryStream();
            var sheet = workbook.CreateSheet(CacheSystem.GetDisplayName(typeof(T)));
            var rowcnt = 0;
            var colcnt = 0;
            // Header
            var header = sheet.CreateRow(rowcnt); rowcnt++;
            var typeObj = typeof(T);
            header.CreateCell(colcnt).SetCellValue(nameof(OwnerTable.Code)); colcnt++;
            foreach (var property in typeObj.GetProperties())
            {
                if (outPutFields.Contains(property.Name))
                {
                    header.CreateCell(colcnt).SetCellValue(CacheSystem.GetDisplayName(property.Name, typeObj));
                    colcnt++;
                }
            }

            T t;
            for (var i = 0; i < recordList.Count; i++)
            {
                t = recordList[i];
                var row = sheet.CreateRow(rowcnt); rowcnt++;
                colcnt = 0;
                row.CreateCell(colcnt).SetCellValue(t.Code); colcnt++;
                foreach (var property in typeObj.GetProperties())
                {
                    if (outPutFields.Contains(property.Name))
                    {
                        //根据类型进行处理
                        var displayValue = OwnerTableExtend.GetDisplayValue(t, property);
                        row.CreateCell(colcnt).SetCellValue(displayValue);
                        colcnt++;
                    }
                }
            }
            workbook.Write(ms);
            return ms;
        }

        /// <summary>
        ///     从Excel导入
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="file"></param>
        /// <param name="ownerId"></param>
        /// <returns></returns>
        public static string ImportFromExcel<T>(HttpPostedFileBase file, string ownerId) where T : OwnerTable, new()
        {
            string strResult;
            try
            {
                var excelFileStream = file.InputStream;
                IWorkbook workbook = new HSSFWorkbook(excelFileStream);
                var sheet = workbook.GetSheetAt(0);
                return ImportFromSheet<T>(sheet, ownerId);
            }
            catch (OfficeXmlFileException)
            {
                strResult = "请确认一下文件格式，暂时只支持Office2003和Office2007。请将文件另存为Office2003格式（*.xls）";
            }
            catch (Exception ex)
            {
                strResult = ex.ToString();
            }
            return strResult;
        }

        /// <summary>
        ///     从Excel导入
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sheet"></param>
        /// <param name="ownerId"></param>
        /// <returns></returns>
        public static string ImportFromSheet<T>(ISheet sheet, string ownerId) where T : OwnerTable, new()
        {
            var strResult = string.Empty;
            try
            {
                var rowCount = sheet.LastRowNum;
                var entityList = new List<T>();
                var errorFlg = false;
                for (var i = 1; i <= rowCount; i++)
                {
                    var row = sheet.GetRow(i);
                    if (row.GetCell(1) == null || string.IsNullOrEmpty(row.GetCell(1).ToString())) break;
                    T t;
                    var code = row.GetCell(0, MissingCellPolicy.CREATE_NULL_AS_BLANK).GetCode();
                    if (string.IsNullOrEmpty(code))
                    {
                        t = new T();
                    }
                    else
                    {
                        t = OwnerTableOperator.GetRecByCodeAtOwner<T>(ownerId, code);
                        if (t == null)
                        {
                            strResult += "记录号码为:[" + code + "]的记录没有找到！" + Environment.NewLine;
                            errorFlg = true;
                            continue;
                        }
                    }
                    GenerateRecord(row, t);
                    entityList.Add(t);
                }
                if (!errorFlg)
                {
                    var insertCnt = 0;
                    var upDateCnt = 0;
                    strResult = string.Empty;
                    for (var i = 0; i < entityList.Count; i++)
                    {
                        //根据有无Code进行不同操作
                        if (string.IsNullOrEmpty(entityList[i].Code))
                        {
                            //新增模式
                            entityList[i].Code = OwnerTable.GetNewCodeByOwnerId(entityList[i]);
                            MongoDbRepository.InsertRec(entityList[i], OwnerTable.SystemImport);
                            insertCnt++;
                        }
                        else
                        {
                            //修改模式
                            MongoDbRepository.UpdateRec(entityList[i], OwnerTable.SystemImport);
                            upDateCnt++;
                        }
                    }
                    strResult += "全体件数:" + entityList.Count + Environment.NewLine;
                    strResult += "追加件数:" + insertCnt + Environment.NewLine;
                    strResult += "修改件数:" + upDateCnt + Environment.NewLine;
                    return strResult;
                }
            }
            catch (Exception ex)
            {
                strResult = ex.ToString();
            }
            return strResult;

        }

        /// <summary>
        /// 读取记录内容
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="row"></param>
        /// <param name="t"></param>
        public static void GenerateRecord<T>(IRow row, T t)
             where T : OwnerTable, new()
        {
            //StringCellValue
            //如果Cell里面是Formal的话，ToString方法获得的是公式的文本，不是 Text属性
            //所以这里使用StringCellValue
            var typeObj = typeof(T);
            var organizationId = t.OwnerId;
            var colcnt = 1;
            foreach (var property in typeObj.GetProperties())
            {
                //根据类型进行处理
                var filterAttrs = property.GetCustomAttributes(typeof(FilterItemAttribute), false);
                if (filterAttrs.Length == 1)
                {
                    var filterAttr = (FilterItemAttribute)filterAttrs[0];
                    switch (filterAttr.MetaStructType)
                    {
                        case FilterItemAttribute.StructType.SingleMasterTable:
                            var masterName = row.GetCell(colcnt, MissingCellPolicy.CREATE_NULL_AS_BLANK).GetCellText();
                            var masterTableName = filterAttr.MetaType.Name;
                            property.SetValue(t, MasterTable.GetMasterCode(masterTableName, masterName, organizationId));
                            break;
                        case FilterItemAttribute.StructType.MultiMasterTable:
                            var masterNameList = (List<string>)property.GetValue(t);
                            var masterListTableName = filterAttr.MetaType.Name;
                            property.SetValue(t, MasterTable.GetMasterCodeList(masterListTableName, masterNameList, organizationId));
                            break;
                        case FilterItemAttribute.StructType.Datetime:
                            property.SetValue(t, row.GetCell(colcnt, MissingCellPolicy.CREATE_NULL_AS_BLANK).GetDate(DateTime.MinValue));
                            break;
                        case FilterItemAttribute.StructType.Boolean:
                            property.SetValue(t, row.GetCell(colcnt, MissingCellPolicy.CREATE_NULL_AS_BLANK).GetCellText() == "是");
                            break;
                        default:
                            property.SetValue(t, row.GetCell(colcnt, MissingCellPolicy.CREATE_NULL_AS_BLANK).GetCellText());
                            break;
                    }
                }
                else
                {
                    property.SetValue(t, row.GetCell(colcnt, MissingCellPolicy.CREATE_NULL_AS_BLANK).GetCellText());
                }
                colcnt++;
            }
        }

    }
}
