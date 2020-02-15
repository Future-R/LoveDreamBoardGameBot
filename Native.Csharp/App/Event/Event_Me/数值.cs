using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Native.Csharp.App.Event.Event_Me
{
    class 数值
    {
        public static string 的(string 目标文本, string 参数)
        {
            //的长度的首个X位置的最后X位置的左对齐X的右对齐X的替换X为Y的从X截取的从X截取Y的去头X的去尾Y
            参数 = 参数.Trim();
            if (参数.StartsWith("长度"))
            {
                return 目标文本.Length.ToString();
            }
            else if (参数.StartsWith("首个"))
            {
                return (目标文本.IndexOf(参数.Substring(2).TrimEnd('置').TrimEnd('位')) + 1).ToString();
            }
            else if (参数.StartsWith("最后"))
            {
                return (目标文本.LastIndexOf(参数.Substring(2).TrimEnd('置').TrimEnd('位')) + 1).ToString();
            }
            else if (参数.StartsWith("左对齐"))
            {
                return 目标文本.PadRight(Convert.ToInt32(参数.Substring(3))).ToString();
            }
            else if (参数.StartsWith("右对齐"))
            {
                return 目标文本.PadLeft(Convert.ToInt32(参数.Substring(3))).ToString();
            }
            else if (参数.StartsWith("替换"))
            {
                参数 = 参数.Substring(2);
                string[] 目标;
                if (参数.Contains("“") && 参数.Contains("”"))
                {
                    目标 = 参数.Substring(1, 参数.Length - 2).Split(new string[] { "”为“" }, StringSplitOptions.RemoveEmptyEntries);
                }
                else
                {
                    目标 = 参数.Split(new[] { '为' }, StringSplitOptions.RemoveEmptyEntries);
                }
                if (目标.Length != 2)
                {
                    return 目标文本;
                }
                return 目标文本.Replace(目标[0], 目标[1]);
            }
            else if (参数.StartsWith("从"))
            {
                参数 = 参数.Substring(1);
                string[] 目标 = 参数.Split(new string[] { "截取" }, StringSplitOptions.RemoveEmptyEntries);
                switch (目标.Length)
                {
                    case 1:
                        return 目标文本.Substring(Convert.ToInt32(目标[0]) - 1);
                    case 2:
                        return 目标文本.Substring(Convert.ToInt32(目标[0]) - 1, Convert.ToInt32(目标[1]));
                    default:
                        return 目标文本;
                }
            }
            else if (参数.StartsWith("去头"))
            {
                char[] 去除 = 参数.Substring(2).ToCharArray();
                return 目标文本.TrimStart(去除);
            }
            else if (参数.StartsWith("去尾"))
            {
                char[] 去除 = 参数.Substring(2).ToCharArray();
                return 目标文本.TrimEnd(去除);
            }

            return 目标文本;
        }
    }
}
