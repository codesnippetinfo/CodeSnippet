using System.Collections.Generic;
using System.Linq;

namespace InfraStructure.Misc
{
    public class RankContain
    {
        /// <summary>
        /// 
        /// </summary>
        public List<struRank> RankList = new List<struRank>();
        /// <summary>
        /// 整体数
        /// </summary>
        public int TotalItemCnt;
        /// <summary>
        /// 排名
        /// </summary>
        public struct struRank
        {
            /// <summary>
            /// 关键字
            /// </summary>
            public string Key;
            /// <summary>
            /// 数量
            /// </summary>
            public int Count;
            /// <summary>
            /// 排名
            /// </summary>
            public int Rank;
        }
        /// <summary>
        /// 构造器
        /// </summary>
        /// <param name="RawData"></param>
        public RankContain(Dictionary<string, int> RawData)
        {
            RankList = GetRankList(RawData);
            foreach (var rankItem in RankList)
            {
                KeyCntDic.Add(rankItem.Key, rankItem.Count);
                KeyRankDic.Add(rankItem.Key, rankItem.Rank);
                RankKeyDic.Add(rankItem.Rank, rankItem.Key);
            }
            TotalItemCnt = RankList.Sum((x) => { return x.Count; });
        }
        /// <summary>
        /// 获得百分比字符
        /// </summary>
        /// <param name="strkey"></param>
        /// <returns></returns>
        public string GetPercentStringByKey(string strkey)
        {
            var tagcnt = GetCountByKey(strkey);
            double percent = tagcnt / (double)TotalItemCnt;
            return string.Format("{0:P}", percent);
        }
        /// <summary>
        /// 标签排位
        /// </summary>
        public Dictionary<string, int> KeyCntDic = new Dictionary<string, int>();
        /// <summary>
        /// 标签VS排位
        /// </summary>
        public Dictionary<string, int> KeyRankDic = new Dictionary<string, int>();
        /// <summary>
        /// 排位VS标签
        /// </summary>
        public Dictionary<int, string> RankKeyDic = new Dictionary<int, string>();
        /// <summary>
        /// 获得标签排位
        /// 不同标签颜色不同
        /// </summary>
        /// <returns></returns>
        public int GetRankByKey(string strKey)
        {
            if (KeyRankDic.ContainsKey(strKey)) return KeyRankDic[strKey];
            return 9999;
        }
        /// <summary>
        /// 获得某个排位的标签
        /// </summary>
        /// <param name="Rank"></param>
        /// <returns></returns>
        public string GetKeyByRank(int Rank)
        {
            if (RankKeyDic.ContainsKey(Rank)) return RankKeyDic[Rank];
            return string.Empty;
        }
        /// <summary>
        /// 获得标签数
        /// </summary>
        /// <param name="strKey"></param>
        /// <returns></returns>
        public int GetCountByKey(string strKey)
        {
            if (KeyCntDic.ContainsKey(strKey)) return KeyCntDic[strKey];
            return 0;
        }
        /// <summary>
        /// 获得排名列表
        /// </summary>
        /// <param name="RawData"></param>
        /// <returns></returns>
        public static List<struRank> GetRankList(Dictionary<string, int> RawData)
        {
            List<struRank> ranks = new List<struRank>();
            List<string> TagRankList = new List<string>();
            foreach (var item in RawData)
            {
                TagRankList.Add(item.Value.ToString("D8") + "|" + item.Key);
            }
            TagRankList.Sort();
            TagRankList.Reverse();
            for (int i = 0; i < TagRankList.Count; i++)
            {
                ranks.Add(new struRank()
                {
                    Rank = i + 1,
                    Key = TagRankList[i].Substring(9),
                    Count = int.Parse(TagRankList[i].Substring(0, 8))
                });
            }
            return ranks;
        }
    }
}
