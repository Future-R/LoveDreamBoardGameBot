using System;
using System.Collections.Generic;

namespace Native.Csharp.App.Event.Event_Me
{
    class 集合
    {
        public static List<string> 静态集合生成(string 分析文本, string 分隔符)
        {
            List<string> 静态集合 = new List<string>(
                分析文本.Split(分隔符.ToCharArray(), StringSplitOptions.RemoveEmptyEntries)
                );
            return 静态集合;
        }
    }
}
