using JiebaNet.Segmenter;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Native.Csharp.App.Event.Event_Me
{
    class 数值
    {
        public static string 的(string 目标文本, string 参数)
        {
            //的长度的首个X位置的最后X位置的左对齐X的右对齐X的替换X为Y的从X截取的从X截取Y的去头X的去尾Y的向上取整的向下取整的X次方的匹配X的X和Y中间
            参数 = 参数.Trim();

            //完全匹配
            switch (参数)
            {
                case "长度":
                    return 目标文本.Length.ToString();

                case "向上取整":
                    return Math.Ceiling(Convert.ToDecimal(目标文本)).ToString();

                case "向下取整":
                    return Math.Floor(Convert.ToDecimal(目标文本)).ToString();

                case "四舍五入":
                    return Math.Round(Convert.ToDecimal(目标文本), MidpointRounding.AwayFromZero).ToString();

                case "个数":
                    return 集合.静态集合生成(目标文本).Count.ToString();

                case "打乱":
                    return 集合.打乱(集合.静态集合生成(目标文本));

                case "去重":
                    return string.Join("、", new List<string>(集合.静态集合生成(目标文本).Distinct()).ToArray());

                case "倒序":
                    List<string> 集合倒序 = 集合.静态集合生成(目标文本);
                    集合倒序.Reverse();
                    return string.Join("、", 集合倒序.ToArray());

                case "排序":
                    List<string> 集合排序 = 集合.静态集合生成(目标文本);
                    try
                    {
                        List<decimal> 新集合排序 = new List<decimal>(集合排序.ConvertAll((string 输入) => Convert.ToDecimal(输入)));
                        新集合排序.Sort();
                        return string.Join("、", 新集合排序.ToArray());
                    }
                    catch (Exception)
                    {
                        集合排序.Sort();
                        return string.Join("、", 集合排序.ToArray());
                    }


                case "频率":
                    Dictionary<string, int> 集合频率 = new Dictionary<string, int>();
                    foreach (var 元素 in 集合.静态集合生成(目标文本))
                    {
                        if (集合频率.ContainsKey(元素))
                        {
                            集合频率[元素] += 1;
                        }
                        else
                        {
                            集合频率.Add(元素, 1);
                        }
                    }
                    var 结果 = from 键值对 in 集合频率

                             orderby 键值对.Value descending

                             select 键值对;
                    string 返回值 = "";
                    foreach (var 键值对 in 结果)
                    {
                        返回值 += $"{键值对.Value}x{键值对.Key}、";
                    }
                    return 返回值.TrimEnd('、');

                case "最大值":
                    try
                    {
                        List<decimal> 数字最大值 = new List<decimal>(集合.静态集合生成(目标文本).ConvertAll((string 输入) => Convert.ToDecimal(输入)));
                        return 数字最大值.Max().ToString();
                    }
                    catch (Exception)
                    {
                        return 集合.静态集合生成(目标文本).Max();
                    }

                case "最小值":
                    try
                    {
                        List<decimal> 数字最小值 = new List<decimal>(集合.静态集合生成(目标文本).ConvertAll((string 输入) => Convert.ToDecimal(输入)));
                        return 数字最小值.Min().ToString();
                    }
                    catch (Exception)
                    {
                        return 集合.静态集合生成(目标文本).Min();
                    }

                case "和":
                    return 运算.计算(string.Join("+", 集合.静态集合生成(目标文本)));

                case "平均数":
                    List<string> 平均数集合 = 集合.静态集合生成(目标文本);
                    return 运算.计算($"({string.Join("+", 平均数集合)})/{平均数集合.Count}");

                case "方差":
                    List<string> 方差集合 = 集合.静态集合生成(目标文本);
                    decimal 平均数的方 = Convert.ToDecimal(运算.计算($"(({string.Join("+", 方差集合)})/{方差集合.Count})^2"));
                    decimal 方的平均数 = 0m;
                    foreach (var 元素 in 方差集合)
                    {
                        方的平均数 += Convert.ToDecimal(元素) * Convert.ToDecimal(元素);
                    }
                    方的平均数 /= 方差集合.Count;
                    return (方的平均数 - 平均数的方).ToString();

                case "标准差":
                    List<string> 方差集合2 = 集合.静态集合生成(目标文本);
                    decimal 平均数的方2 = Convert.ToDecimal(运算.计算($"(({string.Join("+", 方差集合2)})/{方差集合2.Count})^2"));
                    decimal 方的平均数2 = 0m;
                    foreach (var 元素 in 方差集合2)
                    {
                        方的平均数2 += Convert.ToDecimal(元素) * Convert.ToDecimal(元素);
                    }
                    方的平均数2 /= 方差集合2.Count;
                    return Math.Sqrt(Convert.ToDouble(方的平均数2 - 平均数的方2)).ToString();

                case "逐字":
                    return string.Join("、", 目标文本.ToCharArray().ToList());

                case "大写":
                    return 目标文本.ToUpper();

                case "小写":
                    return 目标文本.ToLower();

                case "分词":
                    var 分词器 = new JiebaSegmenter();
                    string 用户词典路径 = AppDomain.CurrentDomain.BaseDirectory + @"app\com.frm.top\UserDict.txt";
                    if (File.Exists(用户词典路径))
                    {
                        分词器.LoadUserDict(用户词典路径);
                    }
                    var 分词结果 = 分词器.Cut(目标文本);
                    return string.Join("、", 分词结果);

                case "转置":
                    return 集合.矩阵转字符串(集合.转置(集合.二维数组生成(目标文本)));

                case "垂直翻转":
                case "上下翻转":
                case "垂直反转":
                case "上下反转":
                    return 集合.矩阵转字符串(集合.垂直翻转(集合.二维数组生成(目标文本)));

                case "水平翻转":
                case "左右翻转":
                case "水平反转":
                case "左右反转":
                    return 集合.矩阵转字符串(集合.水平翻转(集合.二维数组生成(目标文本)));

                case "顺时针旋转":
                    return 集合.矩阵转字符串(集合.转置(集合.垂直翻转(集合.二维数组生成(目标文本))));

                case "逆时针旋转":
                    return 集合.矩阵转字符串(集合.垂直翻转(集合.转置(集合.二维数组生成(目标文本))));

                default:
                    break;
            }

            //开头匹配
            if (参数.StartsWith("首个"))
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
            else if (参数.StartsWith("正则") || 参数.StartsWith("反向正则") || 参数.StartsWith("多行正则") || 参数.StartsWith("单行正则") || 参数.StartsWith("忽略大小写正则"))
            {
                var 模式 = RegexOptions.None;
                var 模式判断 = 参数.Trim().Substring(0, 参数.IndexOf('则') + 1);
                switch (模式判断)
                {
                    case "忽略大小写正则":
                        模式 = RegexOptions.IgnoreCase;
                        break;
                    case "单行正则":
                        模式 = RegexOptions.Singleline;
                        break;
                    case "多行正则":
                        模式 = RegexOptions.Multiline;
                        break;
                    case "反向正则":
                        模式 = RegexOptions.RightToLeft;
                        break;
                    default:
                        break;
                }
                var 匹配集合 = Regex.Matches(目标文本, 参数.Substring(参数.IndexOf('则') + 1), 模式, TimeSpan.FromSeconds(2)).Cast<Match>().Select(m => m.Value).ToArray();
                return string.Join("、", 匹配集合);
            }
            else if (参数.StartsWith("移除"))
            {
                List<string> 移除集合 = 集合.静态集合生成(目标文本);
                参数 = 参数.Substring(2);
                if (参数.StartsWith("第") && 参数.EndsWith("个"))//按序号移除
                {
                    try
                    {
                        参数 = 参数.Substring(1, 参数.Length - 2);
                        if (参数.Contains("-"))//按序号连续移除
                        {
                            List<string> 序号 = 参数.Split(new[] { '-' }).ToList();
                            移除集合.RemoveRange(Convert.ToInt32(序号[0]) - 1, Convert.ToInt32(序号[1]) - Convert.ToInt32(序号[0]) + 1);
                            return string.Join("、", 移除集合);
                        }
                        else//按序号单个移除
                        {
                            移除集合.RemoveAt(Convert.ToInt32(参数) - 1);
                            return string.Join("、", 移除集合);
                        }
                    }
                    catch (Exception)
                    {
                        return "错误：移除出错！";
                    }
                }
                else//按名称移除
                {
                    List<string> 参数集合 = new List<string>(参数.Split(new[] { '、' }, StringSplitOptions.RemoveEmptyEntries));
                    foreach (var 参 in 参数集合)
                    {
                        List<int> 序号集合 = new List<int>();
                        for (int i = 0; i < 移除集合.Count; i++)
                        {
                            if (移除集合[i] == 参)
                            {
                                序号集合.Add(i);
                            }
                        }
                        if (序号集合.Count > 0)
                        {
                            int 随机序号 = new Random(Guid.NewGuid().GetHashCode()).Next(0, 序号集合.Count);
                            移除集合.RemoveAt(序号集合[随机序号]);
                        }
                    }
                    return string.Join("、", 移除集合);
                }
            }
            else if (参数.StartsWith("替换"))
            {
                参数 = 参数.Substring(2).Trim();
                string[] 目标;
                if (参数.Contains("“") && 参数.Contains("”"))
                {
                    目标 = 参数.Substring(1, 参数.Length - 2).Split(new string[] { "”为“" }, StringSplitOptions.None);
                }
                else
                {
                    目标 = 参数.Split(new[] { '为' }, StringSplitOptions.None);
                }
                if (目标.Length != 2)
                {
                    return 目标文本;
                }
                string 返回值 = 目标文本;
                if (目标[1].Contains("”、“"))//是多对多替换
                {
                    MatchCollection 匹配左 = Regex.Matches($"“{目标[0]}”", @"(?<=“).*?(?=”)", RegexOptions.None, TimeSpan.FromSeconds(2));
                    MatchCollection 匹配右 = Regex.Matches($"“{目标[1]}”", @"(?<=“).*?(?=”)", RegexOptions.None, TimeSpan.FromSeconds(2));
                    for (int i = 0; i < 匹配左.Count; i++)
                    {
                        返回值 = 返回值.Replace(匹配左[i].Value, 匹配右[i].Value);
                    }
                }
                else if (目标[0].Contains("”、“"))//是多对一替换
                {
                    MatchCollection 匹配集合 = Regex.Matches($"“{目标[0]}”", @"(?<=“).*?(?=”)", RegexOptions.None, TimeSpan.FromSeconds(2));
                    foreach (var item in 匹配集合)
                    {
                        返回值 = 返回值.Replace(item.ToString(), 目标[1]);
                    }
                }
                else//是一对一替换
                {
                    return 返回值.Replace(目标[0], 目标[1]).Replace("“换行”", Environment.NewLine);
                }
                return 返回值.Replace("“换行”", Environment.NewLine);
            }
            else if (参数.StartsWith("从"))
            {
                参数 = 参数.Substring(1);
                string[] 目标 = 参数.Split(new string[] { "截取" }, StringSplitOptions.RemoveEmptyEntries);
                try
                {
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
                catch (Exception)
                {
                    return 目标文本;
                }
            }
            else if (参数.StartsWith("在"))
            {
                参数 = 参数.Substring(1);
                string[] 目标 = 参数.Split(new string[] { "插入" }, StringSplitOptions.RemoveEmptyEntries);
                List<string> 插入元素 = 集合.静态集合生成(目标文本);
                if (目标[0] == "随机位置")
                {
                    目标[0] = new Random(Guid.NewGuid().GetHashCode()).Next(1, 插入元素.Count + 2).ToString();
                }
                插入元素.Insert(Convert.ToInt32(目标[0]) - 1, 目标[1]);
                return string.Join("、", 插入元素.ToArray());
            }
            else if (参数.StartsWith("倒数第"))
            {
                参数 = 参数.Substring(3).TrimEnd('个');
                int 序号 = Convert.ToInt32(参数) - 1;
                List<string> 获取元素 = 集合.静态集合生成(目标文本);
                if (序号 < 0 || 序号 > 获取元素.Count - 1)
                {
                    return "";
                }
                return 获取元素[获取元素.Count - 序号 - 1];
            }
            else if (参数.StartsWith("第"))
            {
                参数 = 参数.Substring(1).TrimEnd('个');
                int 序号 = Convert.ToInt32(参数) - 1;
                List<string> 获取元素 = 集合.静态集合生成(目标文本);
                if (序号 < 0 || 序号 > 获取元素.Count - 1)
                {
                    return "";
                }
                return 获取元素[序号];
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
            else if (参数.StartsWith("匹配"))
            {
                return 比较.相似(目标文本, 参数.Substring(2)).ToString();
            }
            else if (参数.StartsWith("随机"))
            {
                if (参数.Trim() == "随机")
                {
                    参数 += "1";
                }
                return 集合.随机(集合.静态集合生成(目标文本), Convert.ToInt32(参数.Substring(2)));
            }
            else if (参数.StartsWith("抽取"))
            {
                if (参数.Trim() == "抽取")
                {
                    参数 += "1";
                }
                return 集合.抽取(集合.静态集合生成(目标文本), Convert.ToInt32(参数.Substring(2)));
            }
            else if (参数.StartsWith("发现"))
            {
                if (参数.Trim() == "发现")
                {
                    参数 += "3";
                }
                return 集合.发现(集合.静态集合生成(目标文本), Convert.ToInt32(参数.Substring(2)));
            }

            //结尾匹配
            if (参数.EndsWith("次方") || 参数.EndsWith("次幂"))
            {
                return Math.Pow(Convert.ToDouble(目标文本), Convert.ToDouble(参数.Remove(参数.Length - 2))).ToString();
            }
            if (参数.EndsWith("中间") && 参数.Contains("和"))
            {
                参数 = 参数.Remove(参数.Length - 2);
                string[] 子参数 = 参数.Split('和');
                return 取中间(目标文本, 子参数[0], 子参数[1]);
            }

            //正则匹配
            //马落子在1,3
            if (!string.IsNullOrWhiteSpace(Regex.Match(参数, @"^(.+)落子(在|于)\d{1,2}(.){1}\d{1,2}$").Value))
            {
                var 棋子 = Regex.Match(参数, @"^(.+)(?=落子)");
                var 横坐标 = Regex.Match(参数, @"\d{1,}$").Value;
                var 临时参数 = 参数.Remove(参数.Length - 横坐标.Length - 1);
                var 纵坐标 = Regex.Match(临时参数, @"\d{1,}$").Value;
                var 棋盘 = 集合.二维数组生成(目标文本);
                var 取子 = 棋盘[int.Parse(纵坐标) - 1][int.Parse(横坐标) - 1];
                棋盘[int.Parse(纵坐标) - 1][int.Parse(横坐标) - 1] = 棋子.ToString();
                return 集合.矩阵转字符串(棋盘) + Environment.NewLine + "移除了" + 取子;
            }

            //1,3移动到2,4
            if (!string.IsNullOrWhiteSpace(Regex.Match(参数, @"^\d{1,2}(.){1}\d{1,2}移动到\d{1,2}(.){1}\d{1,2}$").Value))
            {
                int 旧纵坐标 = int.Parse(Regex.Match(参数, @"^\d{1,2}").Value) - 1;
                int 旧横坐标 = int.Parse(Regex.Match(参数, @"(?<=.{1})\d{1,2}(?=移动到)").Value) - 1;
                int 新纵坐标 = int.Parse(Regex.Match(参数, @"(?<=移动到)\d{1,2}(?=.{1})").Value) - 1;
                int 新横坐标 = int.Parse(Regex.Match(参数, @"\d{1,2}$").Value) - 1;
                var 棋盘 = 集合.二维数组生成(目标文本);
                string 中转棋子 = 棋盘[新纵坐标][新横坐标];
                棋盘[新纵坐标][新横坐标] = 棋盘[旧纵坐标][旧横坐标];
                棋盘[旧纵坐标][旧横坐标] = 中转棋子;
                return 集合.矩阵转字符串(棋盘);
            }

            //逐X字
            if (!string.IsNullOrWhiteSpace(Regex.Match(参数, @"(?<=^逐)(\S+?)(?=字$)").Value))
            {
                string 每行字数 = 运算.转阿拉伯数字(Regex.Match(参数, @"(?<=^逐)(\S+?)(?=字$)").Value);
                bool 是数字 = int.TryParse(每行字数, out int 字数);
                if (!是数字)
                {
                    return "";
                }
                for (int i = 字数; i < 目标文本.Length; i += 字数 + 1)
                    目标文本 = 目标文本.Insert(i, "、");
                return 目标文本;
            }

            return 目标文本;
        }

        public static string 取中间(string 目标文本, string 开头, string 结尾)
        {
            string 返回值 = string.Empty;
            int 头坐标, 尾坐标;
            try
            {
                头坐标 = 目标文本.IndexOf(开头);
                if (头坐标 == -1)
                    return 返回值;
                string 临时文本 = 目标文本.Substring(头坐标 + 开头.Length);
                尾坐标 = 临时文本.IndexOf(结尾);
                if (尾坐标 == -1)
                    return 返回值;
                返回值 = 临时文本.Remove(尾坐标);
            }
            catch (Exception)
            {
                return "";
            }
            return 返回值;
        }
    }
}
