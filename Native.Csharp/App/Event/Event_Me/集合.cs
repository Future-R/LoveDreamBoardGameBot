using System;
using System.Collections.Generic;
using System.Linq;

namespace Native.Csharp.App.Event.Event_Me
{
    class 集合
    {
        public static List<string> 静态集合生成(string 分析文本, string 分隔符 = "、")
        {
            List<string> 静态集合 = new List<string>(
                分析文本.Split(分隔符.ToCharArray(), StringSplitOptions.RemoveEmptyEntries)
                );
            return 静态集合;
        }

        public static string[][] 二维数组生成(string 分析文本, string 分隔符 = " ")
        {
            分析文本 = 分析文本.Replace("\r\n", "");
            if (分析文本.Length > 625)
            {
                return null;
            }
            string[] 数组 = 分析文本.Split(分隔符.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            string[][] 二维数组 = new string[数组.Count()][];
            try
            {
                for (int i = 0; i < 数组.Count(); i++)
                {
                    二维数组[i] = Array.ConvertAll(数组[i].ToCharArray(), s => s.ToString());
                }
                return 二维数组;
            }
            catch (Exception)
            {
                return null;
            }
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
                返回值 += 目标集合[rd] + "、";
            }
            return 返回值.TrimEnd('、');
        }

        public static string 发现(List<string> 目标集合, int 次数 = 3)
        {
            List<string> 返回值 = new List<string>();
            if (次数 < 1)
            {
                次数 = 1;
            }
            if (new List<string>(目标集合.Distinct()).Count <= 次数)//元素的数量不大于想要发现的数量
            {
                return 打乱(new List<string>(目标集合.Distinct()));
            }
            for (int i = 0; i < 次数; i++)
            {
                int rd = new Random(Guid.NewGuid().GetHashCode()).Next(0, Convert.ToInt32(目标集合.Count));
                if (返回值.Contains(目标集合[rd]))//如果已经抽到过就重抽
                {
                    i--;
                }
                else
                {
                    返回值.Add(目标集合[rd]);
                }
            }
            return string.Join("、", 返回值.ToArray());
        }

        public static string 打乱(List<string> 目标集合)
        {
            for (int count = 目标集合.Count; count > 0; count--)
            {
                int rd = new Random(Guid.NewGuid().GetHashCode()).Next(0, Convert.ToInt32(count));
                目标集合.Add(目标集合[rd]);
                目标集合.RemoveAt(rd);
            }
            return string.Join("、", 目标集合.ToArray());
        }
        public static string 抽取(List<string> 目标集合, int 数量 = 1)
        {
            if (数量 >= 目标集合.Count)
            {
                return 打乱(目标集合);
            }

            for (int count = 目标集合.Count; count > 0; count--)
            {
                int rd = new Random(Guid.NewGuid().GetHashCode()).Next(0, Convert.ToInt32(count));
                目标集合.Add(目标集合[rd]);
                目标集合.RemoveAt(rd);
            }
            return string.Join("、", 目标集合.Take(数量).ToArray());
        }
    }
}
