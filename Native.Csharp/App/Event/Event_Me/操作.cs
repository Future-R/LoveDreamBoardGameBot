using LZ4;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
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

        public static void 发送(string 消息)
        {
            if (消息 != "")
            {
                数据.发送次数++;
                if (数据.发送次数 > 9)
                {
                    Common.CqApi.SendPrivateMessage(数据.私聊目标.FromQQ, 转义.输出("不干了！"));
                }
                else if (数据.群聊目标 == null)
                {
                    Common.CqApi.SendDiscussMessage(数据.讨论组目标.FromDiscuss, 转义.输出(消息));
                }
                else if (数据.私聊)
                {
                    Common.CqApi.SendPrivateMessage(数据.私聊目标.FromQQ, 转义.输出(消息));
                }
                else
                {
                    Common.CqApi.SendGroupMessage(数据.群聊目标.FromGroup, 转义.输出(消息));
                }
            }
        }

        public static string 存档()
        {
            var 存档 = JsonConvert.SerializeObject(数据.实体);
            var 压缩 = Convert.ToBase64String(LZ4Codec.Wrap(Encoding.UTF8.GetBytes(存档)));
            string 目录 = AppDomain.CurrentDomain.BaseDirectory + @"app\com.frm.top\";
            if (!Directory.Exists(目录))
            {
                Directory.CreateDirectory(目录);
            }
            string 绝对路径 = 目录 + 数据.私聊目标.FromQQ.ToString() + ".rar";
            if (File.Exists(绝对路径))
            {
                File.Delete(绝对路径);
            }
            using (StreamWriter 写入 = File.AppendText(绝对路径))
            {
                写入.Write(压缩);
                写入.Close();
                写入.Dispose();
            }
            return "存档完毕！";
        }

        public static string 读档()
        {
            string 绝对路径 = AppDomain.CurrentDomain.BaseDirectory + @"app\com.frm.top\" + 数据.私聊目标.FromQQ.ToString() + ".rar";
            if (!File.Exists(绝对路径))
            {
                return "没看到你的存档！";
            }
            string 读取 = File.ReadAllText(绝对路径);
            string 反序列化 =
                Encoding.UTF8.GetString(
                    LZ4Codec.Unwrap(Convert.FromBase64String(读取)));
            存档();
            数据.实体 = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, string>>>(反序列化);
            return "读档完毕！再次读档可以撤销本次读取。";
        }

        public static void 写入词典(string 语句, bool 添加模式 = true)
        {
            List<string> 个体 = new List<string>();
            List<string> 属性 = new List<string>(); ;
            foreach (var item in 数据.实体)
            {
                个体.Add(item.Key);
            }
            foreach (var item in 个体)
            {
                foreach (var items in 数据.实体[item])
                {
                    属性.Add(items.Key);
                }
            }
            List<string> 总表 = 个体.Union(属性).Distinct().ToList();
            string[] 要添加的词汇 = 语句.Substring(3).Split('、');
            if (添加模式)
            {
                总表.AddRange(要添加的词汇);
            }
            else
            {
                foreach (var item in 要添加的词汇)
                {
                    if (总表.Contains(item))
                    {
                        总表.Remove(item);
                    }
                }
            }
            
            string 保存 = string.Join(Environment.NewLine, 总表.ToArray());
            string 目录 = AppDomain.CurrentDomain.BaseDirectory + @"app\com.frm.top\";
            if (!Directory.Exists(目录))
            {
                Directory.CreateDirectory(目录);
            }
            string 绝对路径 = 目录 + "UserDict.txt";
            if (File.Exists(绝对路径))
            {
                File.Delete(绝对路径);
            }
            using (StreamWriter 写入 = File.AppendText(绝对路径))
            {
                写入.Write(保存);
                写入.Close();
                写入.Dispose();
            }
        }

    }
}
