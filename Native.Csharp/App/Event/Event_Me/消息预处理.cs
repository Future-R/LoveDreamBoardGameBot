using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

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
    }
}
