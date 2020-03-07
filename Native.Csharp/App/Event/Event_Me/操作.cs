using System.Collections.Generic;

namespace Native.Csharp.App.Event.Event_Me
{
    class 操作
    {
        public static List<long> 机器人开关 = new List<long>();
        public static void 开关(long 群号, bool 关闭)
        {
            if (关闭)
            {
                if (!机器人开关.Contains(数据.群聊目标.FromGroup))
                {
                    机器人开关.Add(数据.群聊目标.FromGroup);
                }
            }
            else
            {
                if (机器人开关.Contains(数据.群聊目标.FromGroup))
                {
                    机器人开关.Remove(数据.群聊目标.FromGroup);
                }
            }
            
        }
    }
}
