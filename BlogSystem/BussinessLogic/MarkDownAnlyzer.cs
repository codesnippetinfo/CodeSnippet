using System.Collections.Generic;

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
            result.CodeLineCnt = new  List<AnlyzeResult.struCodeLine>();
            //注意：如果是测试文本文件的时候，这里会出现/r/n重复计算的问题
            //MongoDB的数据从HTML里面进来，没有这个问题。
            var lines = strMarkDown.Split(System.Environment.NewLine.ToCharArray());
            var lan = string.Empty;
            var lanCnt = 0;
            //是否为一门语言的开始
            var IsStart = true;
            foreach (var line in lines)
            {
                if (string.IsNullOrEmpty(line.Trim()))
                {
                    continue;
                }
                //图片
                if (line.StartsWith("![") && string.IsNullOrEmpty(lan))
                {
                    //![Alt text](/path/to/img.jpg) 考虑使用正则表达式
                    result.ImageList.Add(line.Substring(line.IndexOf("=") + 1).TrimEnd(")".ToCharArray()));
                    continue;
                }
                //代码起始标志
                if (line.StartsWith("```"))
                {
                    if (!IsStart)
                    {
                        //结束语言
                        if (result.IsContainLanguage(lan))
                        {
                            for (int i = 0; i < result.CodeLineCnt.Count; i++)
                            {
                                if (result.CodeLineCnt[i].Language.Equals(lan))
                                {
                                    result.CodeLineCnt[i].Count += lanCnt;
                                    break;
                                }
                            }
                        }
                        else
                        {
                            var x = new AnlyzeResult.struCodeLine() { Language = lan, Count = lanCnt };
                            result.CodeLineCnt.Add(x);
                        }
                        lanCnt = 0;
                        lan = string.Empty;
                        IsStart = true;
                    }
                    else
                    {
                        //开始语言
                        lan = line.Substring(3);
                        if (string.IsNullOrEmpty(lan)) lan = "Gerneric";
                        lanCnt = 0;
                        IsStart = false;
                    }
                    continue;
                }
                //代码段计数
                if (!string.IsNullOrEmpty(lan))
                {
                    if (!string.IsNullOrEmpty(line.Trim())) lanCnt++;
                    continue;
                }
                result.LineCnt++;
            }
            return result;
        }
    }
}
