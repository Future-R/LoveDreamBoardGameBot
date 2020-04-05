using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Native.Csharp.App.Event.Event_Me
{
    class 请问
    {
        public static bool 对吗(string 判别式)
        {
            Dictionary<string, string> 规则列表 = new Dictionary<string, string>();
            foreach (var 键值对 in 数据.实体)
            {
                if (键值对.Value.ContainsKey("规则"))
                {
                    规则列表.Add(键值对.Key, 键值对.Value["规则"]);
                }
            }
            Dictionary<string, Dictionary<string, string>> 推理 = new Dictionary<string, Dictionary<string, string>>(数据.实体);
            
            return false;
        }
    }
}
