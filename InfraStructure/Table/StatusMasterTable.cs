using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using InfraStructure.Helper;

namespace InfraStructure.Table
{
    /// <summary>
    /// StatusMasterTable
    /// </summary>
    public abstract class StatusMasterTable : MasterTable
    {
        /// <summary>
        ///     表示顺序
        /// </summary>
        [DisplayName("表示顺序")]
        [Range(1, 99, ErrorMessage = "请输入1-99的数字")]
        public int SortRank { get; set; }

        /// <summary>
        ///     背景颜色
        /// </summary>
        [DisplayName("背景颜色")]
        public WarningType BgColor { get; set; }

        /// <summary>
        /// 获得Code Vs SortRank 字典
        /// </summary>
        /// <param name="ownerId"></param>
        /// <returns></returns>
        public static Dictionary<string, int> GetCodeSortRankDic<T>(string ownerId) where T : StatusMasterTable, new()
        {
            var rankDic = new Dictionary<string, int>();
            var t = OwnerTableOperator.GetRecListByOwnerId<T>(new T().GetCollectionName(), ownerId);
            foreach (var item in t)
            {
                rankDic.Add(item.Code, item.SortRank);
            }
            return rankDic;
        }
    }
}
