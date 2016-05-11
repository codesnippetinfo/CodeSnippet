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
        /// <param name="title">标题</param>
        /// <param name="chanelList">数据项目</param>
        /// <param name="chartType">图表类型</param>
        /// <param name="width">高度</param>
        /// <param name="height">宽度</param>
        /// <returns></returns>
        public static string GetColumnChart(string chartImageFileName, string title,
            Dictionary<string, int> chanelList,
            ChartType chartType = ChartType.Column, int width = 600, int height = 400)
        {
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
        public static string GetColumnChart(string chartImageFileName, ChartSetCenter chartSet)
        {
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