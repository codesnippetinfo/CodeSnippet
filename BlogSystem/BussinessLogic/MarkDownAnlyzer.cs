using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace BlogSystem.BussinessLogic
{
    /// <summary>
    /// MarkDown内容统计
    /// </summary>
    public static class MarkDownAnlyzer
    {
        /// <summary>
        /// 分析
        /// </summary>
        /// <param name="MarkDown"></param>
        /// <returns></returns>
        public static AnlyzeResult Anlyze(string strMarkDown)
        {
            AnlyzeResult result = new AnlyzeResult();
            result.ImageList = new List<string>();
            result.Url = new List<string>();

            //http://qiita.com/hibara/items/78454f6c70c55e7aa612
            Regex rg = new Regex(@"\[.*\]\((https?|ftp)(:\/\/[-_.!~*\'()a-zA-Z0-9;\/?:\@&=+\$,%#]+)[\t{1,}|\s{1,}]"".*""\)");
            Regex rg2 = new Regex(@"\[.*\]\((https?|ftp)(:\/\/[-_.!~*\\'()a-zA-Z0-9;\/?:\@&=+\$,%#]+)\)");


            result.CodeLineCnt = new Dictionary<string, object>();
            var lines = strMarkDown.Split(System.Environment.NewLine.ToCharArray());
            result.LineCnt = lines.Length;
            var lan = string.Empty;
            var lanCnt = 0;
            foreach (var line in lines)
            {
                if (line.StartsWith("![") && string.IsNullOrEmpty(lan))
                {
                    //![Alt text](/path/to/img.jpg) 考虑使用正则表达式
                    result.ImageList.Add(line.Substring(line.IndexOf("=") + 1).TrimEnd(")".ToCharArray()));
                    continue;
                }
                if (rg.IsMatch(line) || rg2.IsMatch(line))
                {
                    //添加URL
                }
                if (line.StartsWith("```"))
                {
                    if (line == "```")
                    {
                        //结束语言
                        if (!string.IsNullOrEmpty(lan))
                        {
                            if (lan.Equals(nameof(AnlyzeResult.ImageList)) ||
                               lan.Equals(nameof(AnlyzeResult.CodeLineCnt)) ||
                               lan.Equals(nameof(AnlyzeResult.LineCnt)))
                            {
                                //如果出现result.CodeLineCnt.Add("LineCnt", 123);
                                //则加在CodeLineCnt里面的LineCnt将覆盖原来的LineCnt
                                //使用Mongo工具则发生重复元素的问题,这里进行重命名
                                lan = "#" + lan + "#";
                            }
                            if (result.CodeLineCnt.ContainsKey(lan))
                            {
                                result.CodeLineCnt[lan] = (int)result.CodeLineCnt[lan] + (int)lanCnt;
                            }
                            else
                            {
                                result.CodeLineCnt.Add(lan, lanCnt);
                            }
                        }
                        lan = string.Empty;
                        lanCnt = 0;
                    }
                    else
                    {
                        //开始语言
                        lan = line.Substring(3);
                        lanCnt = 0;
                    }
                    continue;
                }
                if (!string.IsNullOrEmpty(lan))
                {
                    //非空行计数
                    if (!string.IsNullOrEmpty(line.Trim())) lanCnt++;
                }
            }
            return result;
        }
    }
}
