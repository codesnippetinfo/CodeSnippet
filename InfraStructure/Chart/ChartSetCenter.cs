using System.Collections.Generic;
using System.Linq;
using InfraStructure.DataBase;
using InfraStructure.FilterSet;
using InfraStructure.Helper;
using InfraStructure.Table;
using MongoDB.Bson;

namespace InfraStructure.Chart
{
    public class ChartSetCenter : ChartSetBase
    {
        /// <summary>
        ///     数据集名称
        /// </summary>
        public override string GetCollectionName()
        {
            return "ChartSet";
        }

        /// <summary>
        ///     数据集名称静态字段
        /// </summary>
        public static string CollectionName = "ChartSet";

        /// <summary>
        ///     数据主键前缀
        /// </summary>
        public override string GetPrefix()
        {
            return string.Empty;
        }

        /// <summary>
        ///     数据主键前缀静态字段
        /// </summary>
        public static string Prefix = string.Empty;

        /// <summary>
        ///     Mvc画面的标题
        /// </summary>
        public static string MvcTitle = "可视化中心";

        /// <summary>
        /// 获得作图用数据
        /// </summary>
        /// <remarks>
        /// Name Vs Count
        /// </remarks>
        public Dictionary<string, int> GetChartData()
        {
            //过滤数据
            var collectionName = ModelName.Split(".".ToCharArray()).Last();
            List<BsonDocument> docList = null;
            if (FilterCode != MasterTable.UnKnownCode)
            {
                var filter = OwnerTableOperator.GetRecByCodeAtOwner<FilterSetCenter>(FilterSetCenter.CollectionName, OwnerId, FilterCode);
                docList = MongoDbRepository.GetRecList(collectionName, filter.GetQuery());
            }
            else
            {
                docList = OwnerTableOperator.GetRecListByOwnerId(collectionName, OwnerId);
            }
            //获得分类一览
            List<MasterWrapper> classfyCode = null;
            var chartData = new Dictionary<string, int>();
            foreach (var property in ExternalType.GetTypeByName(ModelName).GetProperties())
            {
                if (property.Name == FieldName)
                {
                    var filterAttrs = property.GetCustomAttributes(typeof(FilterItemAttribute), false);
                    var filterAttr = (FilterItemAttribute)filterAttrs[0];
                    switch (filterAttr.MetaStructType)
                    {
                        case FilterItemAttribute.StructType.SingleMasterTable:
                            classfyCode = MasterWrapper.GenerateFromMaster(filterAttr.MetaType.Name, OwnerId);
                            chartData = GernerateData(docList, classfyCode);
                            break;
                        case FilterItemAttribute.StructType.SingleEnum:
                            classfyCode = MasterWrapper.GenerateFromEnum(filterAttr.MetaType.FullName);
                            chartData = GernerateData(docList, classfyCode);
                            break;
                        case FilterItemAttribute.StructType.SingleStringMaster:
                            //常规的字符
                            chartData = GernerateData(docList, FieldName);
                            break;
                        default:
                            break;
                    }
                    break;
                }
            }
            //Zero数据的过滤
            if (!ContainsZero)
            {
                var zeroList = new List<string>();
                foreach (var key in chartData.Keys)
                {
                    if (chartData[key] == 0)
                    {
                        zeroList.Add(key);
                    }
                }
                //Remove
                foreach (var zeroKey in zeroList)
                {
                    chartData.Remove(zeroKey);
                }
            }
            //排序
            switch (SortArg)
            {
                case Sort.SortType.None:
                    break;
                case Sort.SortType.Ascending:
                    chartData = chartData.OrderBy(x => x.Value).ToDictionary(o => o.Key, o => o.Value);
                    break;
                case Sort.SortType.Descending:
                    chartData = chartData.OrderByDescending(x => x.Value).ToDictionary(o => o.Key, o => o.Value);
                    break;
                default:
                    break;
            }
            //TOP
            if (!(TopCount == 0 || TopCount >= chartData.Count))
            {
                chartData = chartData.Take(TopCount).ToDictionary(o => o.Key, o => o.Value); 
            }
            return chartData;
        }

        /// <summary>
        /// Group字段
        /// </summary>
        /// <param name="docList"></param>
        /// <param name="fieldName"></param>
        /// <returns></returns>
        private Dictionary<string, int> GernerateData(List<BsonDocument> docList, string fieldName)
        {
            var groups = docList.GroupBy(x =>
              {
                  BsonValue t;
                  //不敢保证统计字段全部非Null
                  x.TryGetValue(fieldName, out t);
                  if (t is BsonNull)
                  {
                      return string.Empty;
                  }
                  return t.ToString();
              });
            var chartData = new Dictionary<string, int>();
            foreach (var groupItem in groups)
            {
                chartData.Add(groupItem.Key, groupItem.Count());
            }
            if (!ContainsUnKnown)
            {
                if (chartData.ContainsKey(string.Empty))
                {
                    chartData.Remove(string.Empty);
                }
            }
            return chartData;
        }

        /// <summary>
        ///     常规的Master
        /// </summary>
        /// <param name="docList"></param>
        /// <param name="classfyCode"></param>
        /// <returns></returns>
        private Dictionary<string, int> GernerateData(List<BsonDocument> docList, List<MasterWrapper> classfyCode)
        {
            if (ContainsUnKnown)
            {
                classfyCode.Add(new MasterWrapper
                {
                    Code = 0.ToString(CodeFormat),
                    Name = "未知"
                });
            }
            var chartData = new Dictionary<string, int>();
            foreach (var item in classfyCode)
            {
                //Code 和 FieldName 同时ToString
                chartData.Add(item.Name, docList.Count(x =>
                {
                    BsonValue t;
                    //不敢保证统计字段全部非Null
                    if (x.TryGetValue(FieldName, out t))
                    {
                        return t.ToString() == item.Code;
                    };
                    return false;
                }));
            }
            return chartData;
        }
    }
}
