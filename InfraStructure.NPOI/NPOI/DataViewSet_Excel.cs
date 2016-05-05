using System.Collections.Generic;
using System.IO;
using InfraStructure.DataBase;
using InfraStructure.FilterSet;
using InfraStructure.Helper;
using InfraStructure.Table;
using NPOI.HSSF.UserModel;
using MongoDB.Bson;

namespace InfraStructure.DataView
{
    public class DataViewSet_Excel : DataViewSet
    {
        /// <summary>
        /// 导出到Excel
        /// </summary>
        /// <returns></returns>
        public MemoryStream ExportToExcel()
        {
            var workbook = new HSSFWorkbook();
            var ms = new MemoryStream();
            var sheet = workbook.CreateSheet(Name);
            var colcnt = 0;
            var rowcnt = 0;
            // Header
            var header = sheet.CreateRow(rowcnt); rowcnt++;
            colcnt = 0;
            foreach (var strTitle in GetTitles())
            {
                header.CreateCell(colcnt).SetCellValue(strTitle);
                colcnt++;
            }
            var modelType = ExternalType.GetTypeByName(ModelName); 
            //数据列表
            var recordList = new List<BsonDocument>();
            if (FilterCode != MasterTable.UnKnownCode)
            {
                var filter = OwnerTableOperator.GetRecByCodeAtOwner<FilterSetCenter>(FilterSetCenter.CollectionName, OwnerId, FilterCode);
                var recordPages = new Pages(OwnerTableOperator.GetCountByOwnerId(modelType.Name, OwnerId, filter.GetQuery()));
                recordList = OwnerTableOperator.GetRecListByOwnerId(modelType.Name, OwnerId, SortArg.ToArray(), filter.GetQuery());
            }
            else
            {
                var recordPages = new Pages(OwnerTableOperator.GetCountByOwnerId(modelType.Name, OwnerId));
                recordList = OwnerTableOperator.GetRecListByOwnerId(modelType.Name, OwnerId, SortArg.ToArray());
            }

            var valueList = GetValueList(modelType, recordList);
            for (var i = 0; i < valueList.Count; i++)
            {
                var rowValueList = valueList[i];
                var row = sheet.CreateRow(rowcnt); rowcnt++;
                colcnt = 0;
                for (var j = 0; j < rowValueList.Length; j++)
                {
                    row.CreateCell(colcnt).SetCellValue(rowValueList[j]);
                    colcnt++;
                }
            }
            workbook.Write(ms);
            workbook = null;
            return ms;
        }
    }
}
