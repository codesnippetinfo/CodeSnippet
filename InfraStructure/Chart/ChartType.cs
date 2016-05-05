using InfraStructure.Helper;

namespace InfraStructure.Chart
{
    public enum ChartType
    {
        /// <summary>
        /// 点图类型
        /// </summary>
        [EnumDisplayName("点图类型")]
        Point ,

        /// <summary>
        /// 快速点图类型
        /// </summary>
        [EnumDisplayName("快速点图类型")]
        FastPoint ,

        /// <summary>
        /// 气泡图类型
        /// </summary>
        [EnumDisplayName("气泡图类型")]
        Bubble ,

        /// <summary>
        /// 折线图类型
        /// </summary>
        [EnumDisplayName("折线图类型")]
        Line ,

        /// <summary>
        /// 样条图类型
        /// </summary>
        [EnumDisplayName("样条图类型")]
        Spline ,

        /// <summary>
        /// 阶梯线图类型
        /// </summary>
        [EnumDisplayName("阶梯线图类型")]
        StepLine ,

        /// <summary>
        /// 快速扫描线图类型
        /// </summary>
        [EnumDisplayName("快速扫描线图类型")]
        FastLine ,

        /// <summary>
        /// 条形图类型
        /// </summary>
        [EnumDisplayName("条形图类型")]
        Bar ,

        /// <summary>
        /// 堆积条形图类型
        /// </summary>
        [EnumDisplayName("堆积条形图类型")]
        StackedBar ,

        /// <summary>
        /// 百分比堆积条形图类型
        /// </summary>
        [EnumDisplayName("百分比堆积条形图类型")]
        StackedBar100 ,

        /// <summary>
        /// 柱形图类型
        /// </summary>
        [EnumDisplayName("柱形图类型")]
        Column ,

        /// <summary>
        /// 堆积柱形图类型
        /// </summary>
        [EnumDisplayName("堆积柱形图类型")]
        StackedColumn ,

        /// <summary>
        /// 百分比堆积柱形图类型
        /// </summary>
        [EnumDisplayName("百分比堆积柱形图类型")]
        StackedColumn100 ,

        /// <summary>
        /// 面积图类型
        /// </summary>
        [EnumDisplayName("面积图类型")]
        Area ,

        /// <summary>
        /// 样条面积图类型
        /// </summary>
        [EnumDisplayName("样条面积图类型")]
        SplineArea ,

        /// <summary>
        /// 堆积面积图类型
        /// </summary>
        [EnumDisplayName("堆积面积图类型")]
        StackedArea ,

        /// <summary>
        /// 百分比堆积面积图类型
        /// </summary>
        [EnumDisplayName("百分比堆积面积图类型")]
        StackedArea100 ,

        /// <summary>
        /// 饼图类型
        /// </summary>
        [EnumDisplayName("饼图类型")]
        Pie ,

        /// <summary>
        /// 圆环图类型
        /// </summary>
        [EnumDisplayName("圆环图类型")]
        Doughnut ,

        /// <summary>
        /// 股价图类型
        /// </summary>
        [EnumDisplayName("股价图类型")]
        Stock ,

        /// <summary>
        /// K线图类型
        /// </summary>
        [EnumDisplayName("K线图类型")]
        Candlestick ,

        /// <summary>
        /// 范围图类型
        /// </summary>
        [EnumDisplayName("范围图类型")]
        Range ,

        /// <summary>
        /// 样条范围图类型
        /// </summary>
        [EnumDisplayName("样条范围图类型")]
        SplineRange ,

        /// <summary>
        /// 范围条形图类型
        /// </summary>
        [EnumDisplayName("范围条形图类型")]
        RangeBar ,

        /// <summary>
        /// 范围柱形图类型
        /// </summary>
        [EnumDisplayName("范围柱形图类型")]
        RangeColumn ,

        /// <summary>
        /// 雷达图类型
        /// </summary>
        [EnumDisplayName("雷达图类型")]
        Radar ,

        /// <summary>
        /// 极坐标图类型
        /// </summary>
        [EnumDisplayName("极坐标图类型")]
        Polar ,

        /// <summary>
        /// 误差条形图类型
        /// </summary>
        [EnumDisplayName("误差条形图类型")]
        ErrorBar ,

        /// <summary>
        /// 盒须图类型
        /// </summary>
        [EnumDisplayName("盒须图类型")]
        BoxPlot ,

        /// <summary>
        /// 砖形图类型
        /// </summary>
        [EnumDisplayName("砖形图类型")]
        Renko ,

        /// <summary>
        /// 新三值图类型
        /// </summary>
        [EnumDisplayName("新三值图类型")]
        ThreeLineBreak ,

        /// <summary>
        /// 卡吉图类型
        /// </summary>
        [EnumDisplayName("卡吉图类型")]
        Kagi ,

        /// <summary>
        /// 点数图类型
        /// </summary>
        [EnumDisplayName("点数图类型")]
        PointAndFigure ,

        /// <summary>
        /// 漏斗图类型
        /// </summary>
        [EnumDisplayName("漏斗图类型")]
        Funnel ,

        /// <summary>
        /// 棱锥图类型
        /// </summary>
        [EnumDisplayName("棱锥图类型")]
        Pyramid 

    }
}
