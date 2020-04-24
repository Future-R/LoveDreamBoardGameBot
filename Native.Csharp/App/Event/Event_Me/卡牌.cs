using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Native.Csharp.App.Event.Event_Me
{
    class 卡牌
    {
        /*【从【我的】牌组】【抽|摸】【3|三】张牌【到【我的】手牌】

        【从【我的】手牌】【打出 | 使用 | 检索 | 发动 | 移动】【[卡名]|第X张牌|倒数第X张牌】【到【我的】场上】
        【从【我的】手牌】将[卡名] 送到【【我的】场上】

        【从【我的】手牌】【覆盖|盖伏】第X张【卡|牌】【到【我的】场上】

        【从【我的】手牌】【展示|查看】[卡名]
        【从【我的】手牌】【发现】[卡名]
        【洗切】【区域】
        */
        public static string 行动分析(string 语句)
        {
            if (!string.IsNullOrWhiteSpace(Regex.Match(语句, @"(?<=抽|摸|抓)(.*?)(?=张卡|张牌)").Value) || Regex.Match(语句, @"(抽牌|摸牌|抓牌|抽卡|摸卡|抓卡)$").Value.Length == 2)
            {
                return 抽牌(语句);
            }
            if (语句.Contains("打出") || 语句.Contains("使用") || 语句.Contains("检索") || 语句.Contains("发动") || 语句.Contains("移动") || 语句.Contains("召唤"))
            {
                return 出牌(语句);
            }
            if (!string.IsNullOrWhiteSpace(Regex.Match(语句, @"(^(洗牌|洗切)(.*)$)|(^(.*?)(洗牌|洗切)$)").Value))
            {
                return 洗牌(语句);
            }
            return "";
        }

        //【洗切|洗牌】【区域】|【区域】【洗切|洗牌】
        static string 洗牌(string 洗牌语句)
        {
            洗牌语句 = 洗牌语句.Replace("洗牌", "").Replace("洗切", "");
            if (string.IsNullOrWhiteSpace(洗牌语句))
            {
                洗牌语句 = "我的手牌";
            }
            if (!洗牌语句.Contains("的"))
            {
                洗牌语句 = "我的" + 洗牌语句;
            }
            洗牌语句 = 同义词重定向(洗牌语句);
            解释.执行($@"设{洗牌语句}为{集合.打乱(集合.静态集合生成(数据.读取(洗牌语句.Replace("我的", 数据.私聊目标.FromQQ.ToString() + "的"))))}");
            return "洗完了！";
        }

        //【从【我的】牌组】【抽|摸|抓】【3|三】张牌【到【我的】手牌】
        static string 抽牌(string 抽牌语句)
        {
            string 来源 = Regex.Match(抽牌语句, @"(?<=^从)(.+?)(?=摸|抽|抓)").Value;
            string 去向 = Regex.Match(抽牌语句, @"(?<=到)(.+?$)").Value;
            string 抽牌 = Regex.Match(抽牌语句, @"(?<=抽|摸|抓)(.*?)(?=张卡|张牌)").Value;
            int 抽牌数 = string.IsNullOrWhiteSpace(抽牌) ? 1 : Convert.ToInt32(运算.转阿拉伯数字(抽牌));
            if (string.IsNullOrEmpty(来源))
            {
                if (数据.实体[数据.私聊目标.FromQQ.ToString()].ContainsKey("牌库"))
                {
                    来源 = "我的牌库";
                }
                else
                {
                    return $"{数据.实体["输入"]["语句"]}无法找到抽牌来源：请通过“。我的牌库是XXX”来绑定默认抽牌来源。";
                }
            }
            if (string.IsNullOrEmpty(去向))
            {
                if (数据.实体[数据.私聊目标.FromQQ.ToString()].ContainsKey("手牌"))
                {
                    去向 = "我的手牌";
                }
                else
                {
                    return $"{数据.实体["输入"]["语句"]}无法找到抽牌去向：请通过“。我的手牌是XXX”来绑定默认抽牌去向。";
                }
            }
            来源 = 同义词重定向(来源);
            去向 = 同义词重定向(去向);
            string 来源卡牌 =
                数据.读取(来源.Replace("我的", 数据.私聊目标.FromQQ.ToString() + "的")) == 来源.Replace("我的", 数据.私聊目标.FromQQ.ToString() + "的") ? "" :
                数据.读取(来源.Replace("我的", 数据.私聊目标.FromQQ.ToString() + "的"));
            string 去向卡牌 =
                数据.读取(去向.Replace("我的", 数据.私聊目标.FromQQ.ToString() + "的")) == 去向.Replace("我的", 数据.私聊目标.FromQQ.ToString() + "的") ? "" :
                数据.读取(去向.Replace("我的", 数据.私聊目标.FromQQ.ToString() + "的"));
            List<string> 来源元素 = 集合.静态集合生成(来源卡牌);
            List<string> 去向元素 = 集合.静态集合生成(去向卡牌);
            if (来源元素.Count < 抽牌数)
            {
                return $"抽牌失败！{来源}卡牌数量仅剩{来源元素.Count}张，不足{抽牌数}张！";
            }
            去向元素.AddRange(来源元素.GetRange(0, 抽牌数));
            来源元素.RemoveRange(0, 抽牌数);
            解释.执行($"设{去向}为{string.Join("、", 去向元素.ToArray())}");
            解释.执行($"设{来源}为{string.Join("、", 来源元素.ToArray())}");
            Common.CqApi.SendPrivateMessage(数据.私聊目标.FromQQ, 转义.输出(string.Join("、", 去向元素.ToArray())));
            return "";
        }

        //【从【我的】手牌】【打出 | 使用 | 检索 | 发动 | 移动 | 召唤】【[卡名]|【X张|只|个】[卡名]|第X张牌|倒数第X张牌】【到【我的】场上】
        static string 出牌(string 出牌语句)
        {
            string 来源 = Regex.Match(出牌语句, @"^从(.+?)(打出|使用|检索|发动|移动|召唤)").Value;
            string 去向 = Regex.Match(出牌语句, @"到(.+?$)").Value;
            string 出牌集 = Regex.Match(出牌语句, @"(?<=打出|使用|检索|发动|移动|召唤)(.*)").Value;
            if (!string.IsNullOrWhiteSpace(来源) && !string.IsNullOrWhiteSpace(去向))
            {
                出牌集 = 出牌语句.Replace(来源, "").Replace(去向, "");
            }
            else if (!string.IsNullOrWhiteSpace(来源))
            {
                出牌集 = 出牌语句.Replace(来源, "");
            }
            else if (!string.IsNullOrWhiteSpace(去向))
            {
                出牌集 = 出牌语句.Replace(去向, "");
            }

            if (string.IsNullOrWhiteSpace(来源))
            {
                来源 = "我的手牌";
            }
            else
            {
                来源 = Regex.Match(来源, @"(?<=^从)(.+?)(?=打出|使用|检索|发动|移动|召唤)").Value;
            }
            if (string.IsNullOrWhiteSpace(去向))
            {
                去向 = "我的场上";
            }
            else
            {
                去向 = Regex.Match(去向, @"(?<=到)(.+?$)").Value;
            }
            来源 = 同义词重定向(来源);
            去向 = 同义词重定向(去向);
            string 来源卡牌 =
                数据.读取(来源.Replace("我的", 数据.私聊目标.FromQQ.ToString() + "的")) == 来源.Replace("我的", 数据.私聊目标.FromQQ.ToString() + "的") ? "" :
                数据.读取(来源.Replace("我的", 数据.私聊目标.FromQQ.ToString() + "的"));
            string 去向卡牌 =
                数据.读取(去向.Replace("我的", 数据.私聊目标.FromQQ.ToString() + "的")) == 去向.Replace("我的", 数据.私聊目标.FromQQ.ToString() + "的") ? "" :
                数据.读取(去向.Replace("我的", 数据.私聊目标.FromQQ.ToString() + "的"));
            List<string> 来源元素 = 集合.静态集合生成(来源卡牌);
            List<string> 去向元素 = 集合.静态集合生成(去向卡牌);

            List<string> 出牌 = new List<string>();
            List<string> 位置 = new List<string>();
            int 数量 = 是否是X张Y(出牌集);
            if (数量 > 0 && 数量 < 101)//是“X张Y”的形式
            {
                while (数量 > 0)
                {
                    出牌.Add(Regex.Match(出牌集, "(?<=^.+?(张|只|个))(.+)").Value);
                    数量--;
                }
            }
            else if (!string.IsNullOrWhiteSpace(Regex.Match(出牌集, "(^(倒数第|第).+?(张|只|个))(?=.+)").Value)) //是“第X张 / 倒数第X张”的形式
            {
                MatchCollection 匹配集合 = Regex.Matches(出牌集, "((倒数第|第).+?(?=张|只|个))(?=.+)", RegexOptions.None, TimeSpan.FromSeconds(2));
                foreach (var item in 匹配集合)
                {
                    位置.Add(item.ToString());
                }
            }
            else//不是“X张Y”的形式，也不是“第X张 / 倒数第X张”的形式
            {
                if (!出牌集.Contains("、"))
                {
                    出牌.AddRange(集合.静态集合生成(出牌集.Replace(" ", "、")));
                }
                else
                {
                    出牌.AddRange(集合.静态集合生成(出牌集));
                }
            }

            //按名称出牌
            if (出牌.Count > 0 && 位置.Count == 0)
            {
                try
                {
                    List<int> 坐标 = new List<int>();
                    for (int i = 0; i < 出牌.Count; i++)//遍历需要移动的对象假名
                    {
                        if (来源元素.FindIndex((string f) => f.Equals(出牌[i])) == -1)
                        {
                            return $"在“{来源}”中找不到“{出牌[i]}”！";
                        }
                        for (int j = 0; j < 来源元素.Count; j++)
                        {
                            if (出牌[i] == 来源元素[j])
                            {
                                坐标.Add(j);
                            }
                        }
                        int rd = new Random(Guid.NewGuid().GetHashCode()).Next(0, Convert.ToInt32(坐标.Count));
                        去向元素.Add(来源元素[坐标[rd]]);
                        来源元素.Remove(来源元素[坐标[rd]]);
                        坐标.Clear();
                    }
                    解释.执行($"设{去向}为{string.Join("、", 去向元素.ToArray())}");
                    解释.执行($"设{来源}为{string.Join("、", 来源元素.ToArray())}");
                    Common.CqApi.SendPrivateMessage(数据.私聊目标.FromQQ, 转义.输出(string.Join("、", 去向元素.ToArray())));
                }
                catch (Exception)
                {
                    return $"在“{来源}”中找不到“{出牌集}”！";
                }
            }
            else//按位置出牌
            {
                try
                {
                    List<int> 位置集 = new List<int>();
                    foreach (var item in 位置)
                    {
                        if (item.StartsWith("倒数第"))
                        {
                            位置集.Add(来源元素.Count - Convert.ToInt32(运算.转阿拉伯数字(item.Substring(3))));
                        }
                        else
                        {
                            位置集.Add(Convert.ToInt32(运算.转阿拉伯数字(item.Substring(1))) - 1);
                        }
                    }
                    位置集 = 位置集.Distinct().ToList();
                    for (int i = 0; i < 位置集.Count; i++)
                    {
                        去向元素.Add(来源元素[位置集[i] - i]);
                        来源元素.RemoveAt(位置集[i] - i);
                    }
                    解释.执行($"设{去向}为{string.Join("、", 去向元素.ToArray())}");
                    解释.执行($"设{来源}为{string.Join("、", 来源元素.ToArray())}");
                    Common.CqApi.SendPrivateMessage(数据.私聊目标.FromQQ, 转义.输出(string.Join("、", 去向元素.ToArray())));
                }
                catch (Exception)
                {
                    return "位置错啦！";
                }
            }

            return "";
        }

        static int 是否是X张Y(string 出牌集)
        {
            string X张Y的X = Regex.Match(出牌集, "(^.+?(张|只|个))(?=.+)").Value;
            if (string.IsNullOrWhiteSpace(X张Y的X))
            {
                return 0;
            }
            else
            {
                if (X张Y的X.StartsWith("第") || X张Y的X.StartsWith("倒数第"))
                {
                    return 0;
                }
                try
                {
                    return Convert.ToInt32(运算.转阿拉伯数字(X张Y的X));
                }
                catch (Exception)
                {
                    return 0;
                }
            }
        }

        static string 同义词重定向(string 描述)
        {
            if (!描述.Contains("的"))
            {
                描述 = "我的" + 描述;
            }
            return 描述.Replace("卡组", "牌库").Replace("牌组", "牌库").Replace("牌堆", "牌库")
                .Replace("手卡", "手牌").Replace("手中", "手牌").Replace("手里", "手牌").Replace("手上", "手牌");
        }
    }
}
