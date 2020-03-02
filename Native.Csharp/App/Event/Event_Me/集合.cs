using System;
using System.Collections.Generic;

namespace Native.Csharp.App.Event.Event_Me
{
    class 集合
    {
        public static List<string> 静态集合生成(string 分析文本, string 分隔符 = " ")
        {
            List<string> 静态集合 = new List<string>(
                分析文本.Split(分隔符.ToCharArray(), StringSplitOptions.RemoveEmptyEntries)
                );
            return 静态集合;
        }

        public static string 随机(List<string> 目标集合, int 次数 = 1)
        {
            string 返回值 = "";
            if (次数 < 1)
            {
                次数 = 1;
            }
            for (int i = 0; i < 次数; i++)
            {
                int rd = new Random(Guid.NewGuid().GetHashCode()).Next(0, Convert.ToInt32(目标集合.Count));
                返回值 += 目标集合[rd] + "#";
            }
            return 返回值.TrimEnd('#');
        }
    }
}
