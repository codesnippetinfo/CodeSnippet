using System.Collections.Generic;
using InfraStructure.Utility;

namespace InfraStructure.Chart
{
    /// <summary>
    /// 图表
    /// </summary>
    public static class ChartHelper
    {
        /// <summary>
        /// 获得图表
        /// </summary>
        /// <param name="ownerId"></param>
        /// <param name="imageName"></param>
        /// <param name="title"></param>
        /// <param name="chanelList"></param>
        /// <param name="chartType"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        public static string GetColumnChart(string ownerId, string imageName, string title,
            Dictionary<string, int> chanelList,
            ChartType chartType = ChartType.Column, int width = 600, int height = 400)
        {
            var chartImageFileName = TempFileExtend.GetChartFileName(ownerId, imageName);
            var mChart = new System.Web.Helpers.Chart(width, height);
            mChart.AddTitle(title);
            mChart.AddSeries(
                chartType: chartType.ToString(),
                   xValue: chanelList.Keys,
                  yValues: chanelList.Values);
            mChart.Save(chartImageFileName);
            return chartImageFileName;
        }
        /// <summary>
        /// 获得图表
        /// </summary>
        /// <param name="chartSet"></param>
        /// <returns></returns>
        public static string GetColumnChart(ChartSetCenter chartSet)
        {
            var chartImageFileName = TempFileExtend.GetChartFileName(chartSet.OwnerId, chartSet.FieldName);
            var mChart = new System.Web.Helpers.Chart(chartSet.Width, chartSet.Height);
            mChart.AddTitle(chartSet.Title);
            var chartData = chartSet.GetChartData();
            mChart.AddSeries(
                chartType: chartSet.ChartType.ToString(),
                   xValue: chartData.Keys,
                  yValues: chartData.Values);
            mChart.Save(chartImageFileName);
            return chartImageFileName;
        }
    }
}