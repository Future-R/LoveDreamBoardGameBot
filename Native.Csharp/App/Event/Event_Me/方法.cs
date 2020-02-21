using System.Collections.Generic;

namespace Native.Csharp.App.Event.Event_Me
{
    class 方法
    {
        public static void 检查方法(string 语句)
        {
            List<string> 待处理列表 = new List<string>(); List<string> 触发语句列表 = new List<string>();
            foreach (var 键值对 in 数据.实体)
            {
                if (键值对.Value.ContainsKey("触发") && 键值对.Value.ContainsKey("语句"))
                {
                    if (解释.判定(解释.变量解释(键值对.Value["触发"])))
                    {
                        待处理列表.Add(键值对.Value["语句"]);
                        触发语句列表.Add(语句);
                    }
                }
            }
            for (int 序号 = 0; 序号 < 待处理列表.Count; 序号++)
            {
                数据.写入实体("触发", "语句", 触发语句列表[序号]);
                解释.语法分析(待处理列表[序号]);
            }
        }
    }
}
