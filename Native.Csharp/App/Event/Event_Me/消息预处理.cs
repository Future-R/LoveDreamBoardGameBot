using Native.Csharp.Sdk.Cqp.Model;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Native.Csharp.App.Event.Event_Me
{
    public class 消息预处理
    {
        public static string 处理(string 处理文本)
        {
            处理文本 = new Regex("[\\s]+").Replace(处理文本, " ").Trim();//去除不必要的空格
            if (处理文本.StartsWith("。"))
            {
                处理文本 = "." + 处理文本.Remove(0, 1);
            }
            if (处理文本.StartsWith("！"))
            {
                处理文本 = ".！" + 处理文本.Remove(0, 1);
            }
            return 处理文本;
        }

        public static void 环境初始化()
        {
            数据.循环次数 = 0;
            数据.发送次数 = 0;
            数据.临时空间 = string.Empty;
            数据.接口 = "http://www.dzyong.top:3005/yiqing/history";
            数据.写入实体("常量", "值", "");
            if (string.IsNullOrWhiteSpace(数据.上次调用接口的时间))
            {
                数据.上次调用接口的时间 = Event_CqAppEnable.启动时间;
            }
            数据.写入实体("现在", "时间", $"{DateTime.Now.DayOfYear.ToString()}{DateTime.Now.TimeOfDay.ToString().Substring(0, 8).Replace(":", "")}");
            数据.写入实体("用户", "QQ", 数据.私聊目标.FromQQ.ToString());
            QQInfo 好友 = Common.CqApi.GetQQInfo(数据.私聊目标.FromQQ);
            数据.写入实体("用户", "昵称", 数据.获取昵称().TrimEnd('的'));
            if (数据.实体.ContainsKey("COC"))//初始化COC的恐惧症和狂躁症
            {
                if (!数据.实体["COC"].ContainsKey("恐惧症"))
                {
                    数据.写入实体("COC", "恐惧症", 转义.内部输入(人物卡.恐惧症));
                }
                if (!数据.实体["COC"].ContainsKey("狂躁症"))
                {
                    数据.写入实体("COC", "狂躁症", 转义.内部输入(人物卡.狂躁症));
                }
            }
            else
            {
                数据.写入实体("COC", "恐惧症", 转义.内部输入(人物卡.恐惧症));
                数据.写入实体("COC", "狂躁症", 转义.内部输入(人物卡.狂躁症));
            }
            if (!数据.实体.ContainsKey("DND3R"))
            {
                数据.实体.Add("DND3R", new Dictionary<string, string>());
                MySql.DND3R字典();
            }
            if (!数据.实体.ContainsKey("生命游戏"))
            {
                string 棋盘 = @"〇红〇〇〇〇〇〇〇〇〇〇 
〇〇红〇〇〇〇〇〇〇〇〇 
红红红〇〇〇〇〇〇〇〇〇 
〇〇〇〇〇〇〇〇〇〇〇〇 
〇〇〇〇〇〇〇〇〇〇〇〇 
〇〇〇〇〇〇〇〇〇〇〇〇 
〇〇〇〇〇〇〇〇〇〇〇〇 
〇〇〇〇〇〇〇〇〇〇〇〇 
〇〇〇〇〇〇〇〇〇〇〇〇 
〇〇〇〇〇〇〇〇〇蓝蓝蓝 
〇〇〇〇〇〇〇〇〇蓝〇〇 
〇〇〇〇〇〇〇〇〇〇蓝〇";
                数据.写入实体("生命游戏", "棋盘", 棋盘);
            }
            if (!数据.实体.ContainsKey("反义词典"))
            {
                数据.写入实体("反义词典", "列表", string.Join("、",数据.反义词典));
            }
            #region 注释
                //if (数据.DND核心法术 == null)//初始化DND核心法术
                //{
                //    List<string> 核心法术表 = new List<string>(人物卡.核心法术.Split(new[] { '#' }, StringSplitOptions.RemoveEmptyEntries));
                //    数据.DND核心法术 = new Dictionary<string, string>(); string 英文键 = ""; string 中文键 = "";
                //    foreach (var item in 核心法术表)
                //    {
                //        英文键 = 数值.取中间(item, "（", "）").ToLower();
                //        中文键 = item.Substring(0, item.IndexOf("（"));
                //        if (数据.DND核心法术.ContainsKey(英文键))
                //        {
                //            数据.DND核心法术[英文键] += Environment.NewLine + item;
                //        }
                //        else
                //        {
                //            数据.DND核心法术.Add(英文键, item);
                //        }
                //        if (数据.DND核心法术.ContainsKey(中文键))
                //        {
                //            数据.DND核心法术[英文键] += Environment.NewLine + item;
                //        }
                //        else
                //        {
                //            数据.DND核心法术.Add(中文键, item);
                //        }
                //    }
                //}
                #endregion
        }
    }
}
