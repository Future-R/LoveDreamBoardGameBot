using JiebaNet.Segmenter.Common;
using JiebaNet.Segmenter.PosSeg;
using Native.Csharp.Sdk.Cqp.Model;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Native.Csharp.App.EventArgs
{
    using static 数据;
    class 分析
    {
        public static string 日报统计(CqGroupMessageEventArgs e, DateTime 日期, int 发言统计数 = 3, int 热词统计数 = 5)
        {
            List<消息记录> 本群消息列表 = 获取指定日期的记录(e.FromGroup, 日期);
            if (本群消息列表.Count < 20)
            {
                return "消息过少，不予统计！";
            }
            string 返回值 = $"{日期.ToShortDateString()}日报统计";
            //JObject 群成员列表 = CqApiV2.GetGroupMemberList(e.RobotQQ.Id.ToString(), e.FromGroup.ToString());
            List<GroupMember> 群员列表 = Common.CqApi.GetMemberList(e.FromGroup);

            #region 发言统计&复读统计
            Dictionary<long, int> 发言统计容器 = new Dictionary<long, int>();
            Dictionary<long, int> 复读统计容器 = new Dictionary<long, int>();
            Dictionary<string, int> 复读语句统计容器 = new Dictionary<string, int>();
            int 复读对比窗口大小 = 5; // 即使中间插了5-1条，仍算复读

            for (int i = 0; i < 本群消息列表.Count; i++)
            {
                消息记录 当前消息 = 本群消息列表[i];

                // 复读
                if (i != 0)
                {
                    int 窗口起点 = i - 复读对比窗口大小 + 1;
                    int 对比信息数 = 复读对比窗口大小 - 1;
                    if (窗口起点 < 0)
                    {
                        窗口起点 = 0;
                        对比信息数 = i;
                    }
                    List<string> 窗口内信息 = 本群消息列表.GetRange(窗口起点, 对比信息数).Select(m => m.消息).ToList();
                    if (窗口内信息.Contains(当前消息.消息))
                    {
                        if (复读统计容器.ContainsKey(当前消息.QQ号))
                        {
                            复读统计容器[当前消息.QQ号] += 1;
                        }
                        else
                        {
                            复读统计容器.Add(当前消息.QQ号, 1);
                        }

                        if (复读语句统计容器.ContainsKey(当前消息.消息))
                        {
                            复读语句统计容器[当前消息.消息] += 1;
                        }
                        else
                        {
                            复读语句统计容器.Add(当前消息.消息, 1);
                        }
                    }
                }

                // 发言
                if (发言统计容器.ContainsKey(当前消息.QQ号))
                {
                    发言统计容器[当前消息.QQ号] += 1;
                }
                else
                {
                    发言统计容器.Add(当前消息.QQ号, 1);
                }
            }

            string 群复读机 = "";
            int 复读次数 = 0;

            string 复读最多的一句话是 = "";
            int 这句话复读的次数 = 0;

            if (复读统计容器.Count > 0)
            {
                var 复读机QQ号和复读次数 = (from pair in 复读统计容器
                                   orderby pair.Value
                                   descending
                                   select pair).First();
                群复读机 = 获取群名片(e, 复读机QQ号和复读次数.Key, 群员列表);
                复读次数 = 复读机QQ号和复读次数.Value;

                var 复读语句统计 = (from pair in 复读语句统计容器
                              orderby pair.Value
                              descending
                              select pair).First();

                复读最多的一句话是 = 复读语句统计.Key;
                这句话复读的次数 = 复读语句统计.Value;
            }

            返回值 += $"{Environment.NewLine}当天群友一共水了{本群消息列表.Count}条消息，其中最厉害的是：";

            var 发言排序结果 = from pair in 发言统计容器
                         orderby pair.Value
                         descending
                         select pair;
            int 迭代次数 = 0;

            foreach (var item in 发言排序结果)
            {
                string 群名片 = 获取群名片(e, item.Key, 群员列表);
                返回值 += $"{Environment.NewLine}{群名片}：{item.Value}";

                迭代次数++;
                if (迭代次数 >= 发言统计数)
                {
                    break;
                }
            }
            #endregion

            #region 词频统计
            返回值 += $"{Environment.NewLine}经过统计发现，";
            Dictionary<string, int> 词频统计 = 热词词频统计(e.FromGroup, 日期, 1, 热词统计数);
            foreach (var item in 词频统计)
            {
                返回值 += $"“{item.Key}({item.Value})”、";
            }
            返回值 = $"{返回值.Substring(0, 返回值.Length - 1)}是群友最热衷的话题。";
            #endregion

            #region at统计
            string 待分析文本 = (from M in 本群消息列表 select M.消息).Join(" ");
            var at匹配结果组 = Regex.Matches(待分析文本, @"(?<=\[@)\d+(?=\]\s*)");
            List<string> 被at统计容器 = new List<string>();
            string 被Q最多的群友 = "";
            int 被Q次数 = 0;
            foreach (var item in at匹配结果组)
            {
                被at统计容器.Add(item.ToString());
            }
            var 被at统计结果 = from item in 被at统计容器   //每一项                          
                          group item by item into gro   //按项分组，没组就是gro                          
                          orderby gro.Count() descending   //按照每组的数量进行排序                          
                          select new { id = gro.Key, nums = gro.Count() };
            foreach (var item in 被at统计结果.Take(1))
            {
                被Q最多的群友 = 获取群名片(e, Convert.ToInt64(item.id), 群员列表);
                被Q次数 = item.nums;
            }

            if (被Q次数 != 0)
            {
                // 不知为何，QQ里每次被at都是两次
                被Q次数 = Convert.ToInt32(Math.Ceiling(被Q次数 * 0.5f));
                if (被Q次数 >= 100)
                {
                    返回值 += $"{Environment.NewLine}{被Q最多的群友}一定是机器人！当日被Cue了足足{被Q次数}次！";
                }
                else if (被Q次数 >= 50)
                {
                    返回值 += $"{Environment.NewLine}没想到居然会有这种事情！{被Q最多的群友}一天下来被Cue了足足{被Q次数}次！";
                }
                else if (被Q次数 >= 20)
                {
                    返回值 += $"{Environment.NewLine}{被Q最多的群友}是群友们关注的焦点，一天下来被Cue了足足{被Q次数}次！";
                }
                else if (被Q次数 >= 10)
                {
                    返回值 += $"{Environment.NewLine}{被Q最多的群友}是当日的风云人物，一天下来被Cue了{被Q次数}次。";
                }
                else if (被Q次数 >= 4)
                {
                    返回值 += $"{Environment.NewLine}说到最受欢迎的群友，果然是{被Q最多的群友}……当日一共被at了{被Q次数}次。";
                }
                else
                {
                    返回值 += $"{Environment.NewLine}最受关注的群友是{被Q最多的群友}，被at了{被Q次数}次。";
                }
            }
            #endregion

            #region 守夜冠军
            消息记录 守夜冠军 = null;
            // 如果第一条消息比凌晨5点早
            if (DateTime.Compare(本群消息列表[0].时间戳, 日期.AddHours(5)) < 0)
            {
                // 如果最后一条比凌晨5点早，守夜冠军就是最后一人
                if (DateTime.Compare(本群消息列表[本群消息列表.Count - 1].时间戳, 日期.AddHours(5)) < 0)
                {
                    守夜冠军 = 本群消息列表[本群消息列表.Count - 1];
                }
                // 否则逐条遍历，直到找到晚于凌晨5点的消息，然后取上一条
                else
                {
                    for (int i = 0; i < 本群消息列表.Count; i++)
                    {
                        if (DateTime.Compare(本群消息列表[i].时间戳, 日期.AddHours(5)) >= 0)
                        {
                            守夜冠军 = 本群消息列表[i - 1];
                            break;
                        }
                    }
                }
                if (DateTime.Compare(守夜冠军.时间戳, 日期.AddHours(4)) > 0)
                {
                    string 冠军昵称 = 获取群名片(e, 守夜冠军.QQ号, 群员列表);
                    返回值 += $"{Environment.NewLine}不知道该说是守夜冠军还是早起冠军了……{冠军昵称}直到凌晨{守夜冠军.时间戳.ToShortTimeString()}，还在群里说：“{守夜冠军.消息}”";
                }
                else if (DateTime.Compare(守夜冠军.时间戳, 日期.AddHours(3)) > 0)
                {
                    string 冠军昵称 = 获取群名片(e, 守夜冠军.QQ号, 群员列表);
                    返回值 += $"{Environment.NewLine}{冠军昵称}是当之无愧的守夜冠军！直到深夜{守夜冠军.时间戳.ToShortTimeString()}，{冠军昵称}还在群里说：“{守夜冠军.消息}”";
                }
                else if (DateTime.Compare(守夜冠军.时间戳, 日期.AddHours(2)) > 0)
                {
                    string 冠军昵称 = 获取群名片(e, 守夜冠军.QQ号, 群员列表);
                    返回值 += $"{Environment.NewLine}{冠军昵称}夺得了当日的守夜冠军！直到{守夜冠军.时间戳.ToShortTimeString()}，{冠军昵称}还在群里说：“{守夜冠军.消息}”";
                }
                else
                {
                    string 冠军昵称 = 获取群名片(e, 守夜冠军.QQ号, 群员列表);
                    返回值 += $"{Environment.NewLine}守夜冠军是{冠军昵称}！{守夜冠军.时间戳.ToShortTimeString()}的时候，{冠军昵称}还在群里说：“{守夜冠军.消息}”";
                }

            }
            #endregion
            if (复读次数 > 1)
            {
                返回值 += $"{Environment.NewLine}群复读机是{群复读机}";
                if (复读次数 >= 100)
                {
                    返回值 += $"，复读了……呃{复读次数}次？这肯定是出Bug了吧……";
                }
                else if (复读次数 >= 50)
                {
                    返回值 += $"，复读了{复读次数}次？这种事是真的存在吗？";
                }
                else if (复读次数 >= 30)
                {
                    返回值 += $"，复读了{复读次数}次？到底谁才是机器人？";
                }
                else if (复读次数 >= 20)
                {
                    返回值 += $"，复读了{复读次数}次！你一定是不需要工作的有钱人吧？";
                }
                else if (复读次数 >= 10)
                {
                    返回值 += $"，居然复读了{复读次数}次！";
                }
                else
                {
                    返回值 += $"，复读了{复读次数}次。";
                }

                if (这句话复读的次数 > 2)
                {
                    返回值 += $"{Environment.NewLine}“{复读最多的一句话是}”是群友最喜欢复读的话，一共被复读了{这句话复读的次数}次！";
                }
            }
            return 返回值;
        }
        public static string 语句分词(string 文本)
        {
            var 分词器 = new PosSegmenter();
            var 分词结果 = 分词器.Cut(文本);
            return string.Join(" ", 分词结果.Select(token => string.Format("{0}/{1}", token.Word, token.Flag)));
        }

        public static Dictionary<string, int> 发言排行统计(CqGroupMessageEventArgs e, DateTime 基准日期, int 天数 = 1, int 排名数量 = 10)
        {
            List<GroupMember> 群员列表 = Common.CqApi.GetMemberList(e.FromGroup);
            Dictionary<long, int> 统计容器 = new Dictionary<long, int>();
            Dictionary<string, int> 返回值 = new Dictionary<string, int>();

            foreach (消息记录 记录 in 获取指定日期的记录(e.FromGroup, 基准日期, 天数))
            {
                if (统计容器.ContainsKey(记录.QQ号))
                {
                    统计容器[记录.QQ号] += 1;
                }
                else
                {
                    统计容器.Add(记录.QQ号, 1);
                }
            }

            var 排序结果 = from pair in 统计容器 orderby pair.Value descending select pair;
            int 迭代次数 = 0;

            foreach (var item in 排序结果)
            {
                //QMLog.CurrentApi.Log(LogLevel.Infomaction, $"{迭代次数}：{item.Key}");
                返回值.Add(获取群名片(e, item.Key, 群员列表), item.Value);

                迭代次数++;
                if (迭代次数 >= 排名数量)
                {
                    return 返回值;
                }
            }
            return 返回值;
        }

        public static string 热词统计报告(long 群号, int 天数 = 1, int 排名数量 = 10)
        {
            string 返回值;
            if (天数 == 1)
            {
                返回值 = $"今日热词报告：{Environment.NewLine}";
            }
            else
            {
                返回值 = $"{天数}日热词报告：{Environment.NewLine}";
            }
            Dictionary<string, int> 权重结果 = 热词词频统计(群号, DateTime.Today, 天数, 排名数量);
            if (权重结果.Count < 5)
            {
                return "信息过少，无法分析……";
            }

            foreach (var item in 权重结果)
            {
                返回值 += $"{item.Key}：{item.Value}{Environment.NewLine}";
            }

            if (天数 == 1)
            {
                返回值 += $"——{DateTime.Now.ToShortTimeString()}";
            }
            else
            {
                返回值 += $"——{DateTime.Now.ToShortDateString()}";
            }

            return 返回值;
        }

        public static Dictionary<string, int> 热词词频统计(long 群号, DateTime 基准日期, int 天数 = 1, int 排名数量 = 10, List<消息记录> 本群消息列表 = null)
        {
            if (本群消息列表 == null)
            {
                本群消息列表 = 获取指定日期的记录(群号, 基准日期, 天数);
            }
            本群消息列表.ForEach(F => F.消息 = F.过滤特殊消息());
            string 待分析文本 = (from M in 本群消息列表 select M.消息).Join(" ");

            var 分词器 = new PosSegmenter();
            var 分词结果 = 分词器.Cut(待分析文本);

            Dictionary<string, int> 词频统计 = new Dictionary<string, int>();
            foreach (var token in 分词结果)
            {
                switch (token.Flag)
                {
                    case "o":
                    case "p":
                    case "w":
                    case "x":
                    case "r":
                    case "d":
                    case "df":
                    case "dg":
                    case "u":
                    case "c":
                    case "v":
                    case "m":
                    case "t":
                    case "ad":
                    case "ul":
                    case "eng":
                        break;
                    default:
                        if (token.Word.Length < 2)
                        {
                            break;
                        }
                        if (词频统计.ContainsKey(token.Word))
                        {
                            词频统计[token.Word] += 1;
                        }
                        else
                        {
                            词频统计.Add(token.Word, 1);
                        }
                        break;
                }
            }
            var 词频排序 = 词频统计.OrderByDescending(p => p.Value);
            Dictionary<string, int> 返回值 = new Dictionary<string, int>();
            foreach (var item in 词频排序)
            {
                排名数量--;
                返回值.Add(item.Key, item.Value);
                if (排名数量 <= 0)
                {
                    return 返回值;
                }
            }
            return null;
        }
    }
}
