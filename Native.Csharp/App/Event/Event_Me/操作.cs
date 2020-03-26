using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

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

        public static T 超时检测<T>(int 毫秒, Func<T> 函数)
        {
            var wait = new ManualResetEvent(false);
            bool RunOK = false;
            var task = Task.Run<T>(() =>
            {
                var result = 函数.Invoke();
                RunOK = true;
                wait.Set();
                return result;
            });
            wait.WaitOne(毫秒);
            if (RunOK)
            {
                return task.Result;
            }
            else
            {
                return default(T);
            }
        }
    }
}
