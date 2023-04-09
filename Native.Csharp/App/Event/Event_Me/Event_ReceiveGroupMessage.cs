using Native.Csharp.App.EventArgs;
using Native.Csharp.App.Interface;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Drawing;

namespace Native.Csharp.App.Event.Event_Me
{
    using static 数据;
    using static Common;
    /// <summary>
    /// 群聊回复
    /// CqPrivateMessageEventArgs→CqGroupMessageEventArgs
    /// SendPrivateMessage→ReceiveGroupMessage
    /// e.FromQQ→e.FromGroup
    /// </summary>
    public class Event_ReceiveGroupMessage : IReceiveGroupMessage
    {

        //Event_ReceiveFriendMessage ReceiveFriendMessage = new Event_ReceiveFriendMessage();

        public void ReceiveGroupMessage(object sender, CqGroupMessageEventArgs e)
        {
            string 信息 = e.Message;

            // 匹配防误触符号
            Match match = Regex.Match(信息, @"^(\.|。){1}[^\.。]{1}");
            if (match.Success)
            {
                信息 = 信息.Substring(1);
            }

            // 开关的优先级最高
            if (信息 == "开启日报统计")
            {
                if (正在统计的群.Contains(e.FromGroup))
                {
                    CqApi.SendGroupMessage(e.FromGroup, "请勿重复开启！");
                }
                else
                {
                    正在统计的群.Add(e.FromGroup);
                    所有消息合集.Add(e.FromGroup, new List<消息记录>());
                    CqApi.SendGroupMessage(e.FromGroup, "现在开始记录！");
                }
                return;
            }
            if (信息 == "关闭日报统计")
            {
                if (正在统计的群.Contains(e.FromGroup))
                {
                    正在统计的群.Remove(e.FromGroup);
                    所有消息合集.Remove(e.FromGroup);
                    string 储存目录 = $@"{Common.AppDirectory}{e.FromGroup}";
                    if (Directory.Exists(储存目录))
                    {
                        DirectoryInfo 目录 = new DirectoryInfo(储存目录);
                        目录.Delete(true);
                    }
                    CqApi.SendGroupMessage(e.FromGroup, "现在开始不再记录，并且清除历史信息……");
                }
                else
                {
                    CqApi.SendGroupMessage(e.FromGroup, "请勿重复关闭！");
                }
                return;
            }

            // 如果当前群开启统计
            if (正在统计的群.Contains(e.FromGroup))
            {
                switch (信息)
                {
                    case "七日热词":
                        保存信息();
                        CqApi.SendGroupMessage(e.FromGroup, 发图(分析.热词统计报告(e.FromGroup, 7)));
                        return;

                    case "查询系统字体":
                        List<string> 系统字体 = new List<string>();
                        FontFamily[] fontFamilys = new System.Drawing.Text.InstalledFontCollection().Families;
                        foreach (FontFamily font in fontFamilys)
                        {
                            系统字体.Add(font.Name);
                        }
                        CqApi.SendGroupMessage(e.FromGroup, 发图(string.Join("/", 系统字体)));
                        return;

                    case "日报":
                    case "查询日报":
                    case "今日日报":
                        保存信息();
                        CqApi.SendGroupMessage(e.FromGroup, 发图(分析.日报统计(e, DateTime.Today)));
                        break;

                    case "昨日日报":
                        CqApi.SendGroupMessage(e.FromGroup, 发图(分析.日报统计(e, DateTime.Today.AddDays(-1))));
                        break;

                    case "热词":
                        保存信息();
                        CqApi.SendGroupMessage(e.FromGroup, 发图(分析.热词统计报告(e.FromGroup)));

                        return;

                    case "发言排行":
                        保存信息();
                        var 发言排行统计结果 = 分析.发言排行统计(e, DateTime.Today);
                        string 发言排行统计报告 = $"今日水群排行：{Environment.NewLine}";
                        if (发言排行统计结果.Count < 2)
                        {
                            CqApi.SendGroupMessage(e.FromGroup, "结果过少，不予统计……");
                            return;
                        }
                        foreach (var item in 发言排行统计结果)
                        {
                            发言排行统计报告 += $"{item.Key}：{item.Value}{Environment.NewLine}";
                        }
                        发言排行统计报告 += $"——{DateTime.Now.ToShortTimeString()}";
                        CqApi.SendGroupMessage(e.FromGroup, 发图(发言排行统计报告));

                        return;

                    case "七日发言":
                        保存信息();
                        var 七日发言排行统计结果 = 分析.发言排行统计(e, DateTime.Today, 7);
                        string 七日发言排行统计报告 = $"7日水群排行：{Environment.NewLine}";
                        if (七日发言排行统计结果.Count < 2)
                        {
                            CqApi.SendGroupMessage(e.FromGroup, "结果过少，不予统计……");
                            return;
                        }
                        foreach (var item in 七日发言排行统计结果)
                        {
                            七日发言排行统计报告 += $"{item.Key}：{item.Value}{Environment.NewLine}";
                        }
                        七日发言排行统计报告 += $"——{DateTime.Now.ToShortDateString()}";
                        CqApi.SendGroupMessage(e.FromGroup, 发图(七日发言排行统计报告));

                        return;

                    default:
                        break;
                }

                if (信息.StartsWith("分析语句："))
                {
                    CqApi.SendGroupMessage(e.FromGroup, 分析.语句分词(信息.Substring(5)));
                    return;
                }
                if (信息.StartsWith("测试换行："))
                {
                    CqApi.SendGroupMessage(e.FromGroup, 发图(信息.Substring(5)));
                    return;
                }

                // 如果跨天，自动清理缓存
                if (DateTime.Today.Date != 上次保存时间.Date)
                {
                    if (保存信息())
                    {
                        读取信息();
                        //QMLog.CurrentApi.Info("每日自动存档完成！");
                    }
                    else
                    {
                        //QMLog.CurrentApi.Error("每日自动存档失败！");
                    }
                    上次保存时间 = DateTime.Now;
                }

                // 记录消息
                消息记录 当前消息记录 = new 消息记录
                {
                    时间戳 = DateTime.Now,
                    QQ号 = e.FromQQ,
                    消息 = e.Message
                };
                if (所有消息合集.ContainsKey(e.FromGroup))
                {
                    所有消息合集[e.FromGroup].Add(当前消息记录);
                }
                else
                {
                    所有消息合集.Add(e.FromGroup, new List<消息记录>());
                    //QMLog.CurrentApi.Log(LogLevel.Warning, $"意外找不到{e.FromGroup}记录！");
                }

                // 60分钟自动保存
                double 距离上次保存 = new TimeSpan(DateTime.Now.Ticks - 上次保存时间.Ticks).TotalMinutes;
                if (距离上次保存 >= 60)
                {
                    上次保存时间 = DateTime.Now;
                    if (保存信息())
                    {
                        //QMLog.CurrentApi.Log(LogLevel.Infomaction, "自动保存成功！");
                    }
                    else
                    {
                        //QMLog.CurrentApi.Log(LogLevel.Error, "自动保存失败！");
                    }
                }
            }


            return;
        }

        string 发图(string 文本, int 宽度上限 = 1000)
        {
            Bitmap 图 = new 图片(文本).行宽高为(宽度上限, 48).开始生成();
            图.Save($"{Common.AppDirectory}temp.jpg");
            return $"[pic={Common.AppDirectory}temp.jpg]";
        }
    }
}
