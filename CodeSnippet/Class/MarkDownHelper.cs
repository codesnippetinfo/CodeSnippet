using CommonMark;
using System.Text;

namespace CodeSnippet
{
    public static class MarkDownHelper
    {
        public static string TransferMD2HTML(string MarkDown)
        {
            StringBuilder fix = new StringBuilder();
            foreach (string line in MarkDown.Split(System.Environment.NewLine.ToCharArray()))
            {
                fix.AppendLine(FixMarkDown(line));
            }
            return CommonMarkConverter.Convert(fix.ToString());
        }
        /// <summary>
        /// 增加了对于#后没有空格的兼容性问题
        /// </summary>
        /// <param name="org"></param>
        /// <returns></returns>
        private static string FixMarkDown(string org)
        {
            if (org.StartsWith("#"))
            {
                int OrgLength = org.Length;
                org = org.TrimStart("#".ToCharArray());
                int cnt = OrgLength - org.Length;
                org = org.TrimStart(" ".ToCharArray());
                org = new string("#".ToCharArray()[0], cnt) + " " + org;
            }
            return org;
        }
    }
}