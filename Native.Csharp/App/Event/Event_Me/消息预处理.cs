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
            return 处理文本;
        }

        public static void 环境初始化()
        {
            数据.循环次数 = 0;
            数据.发送次数 = 0;
            数据.临时空间 = string.Empty;
            数据.接口 = "http://www.dzyong.top:3005/yiqing/history";
        }
    }
}
