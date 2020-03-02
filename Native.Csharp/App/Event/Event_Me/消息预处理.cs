using System;
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
            数据.写入实体("现在","时间", $"{DateTime.Now.DayOfYear.ToString()}{DateTime.Now.TimeOfDay.ToString().Substring(0, 8).Replace(":", "")}");
            if (数据.实体.ContainsKey("COC"))//初始化COC的恐惧症和狂躁症
            {
                if (!数据.实体["COC"].ContainsKey("恐惧症"))
                {
                    数据.写入实体("COC","恐惧症", 转义.内部输入(人物卡.恐惧症));
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
        }
    }
}
