using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Native.Csharp.App.Event.Event_Me
{
    public class 数据
    {
        public static bool 私聊
        {
            get;set;
        }

        public static Dictionary<string, Dictionary<string, string>> 实体 = new Dictionary<string, Dictionary<string, string>>();

        public static Dictionary<string, string> 规则 = new Dictionary<string, string>();

        public static string 词典位置 = AppDomain.CurrentDomain.BaseDirectory + @"app\dict.ini";

        public static string 词典读取 = File.ReadAllText(词典位置, Encoding.GetEncoding("gb2312"));

        public static List<string> 反义词典 => new List<string>(词典读取.Split(new string[] { "A", Environment.NewLine}, StringSplitOptions.RemoveEmptyEntries));

        public static string 报错
        {
            get
            {
                return new string[]
                {
                    "我懵了。", "哎？", "不对……", "怎么回事？", "什么情况？",
                    "奇怪的错误……", "搞错了吧？" , "……", "又来了……" , "¿",
                    "菜", "算了吧……" , "唔。", "[CQ:face,id=14]" , "[CQ:face,id=39]"
                }
                [new Random(Guid.NewGuid().GetHashCode()).Next(0, 15)];
            }
        }

        public static string 临时空间
        {
            get;set;
        }

        public static int 循环次数
        {
            get;set;
        }

        public static int 发送次数
        {
            get;set;
        }

        public static long 发送目标
        {
            get;set;
        }

        public enum 真假
        {
            假 = 0,
            错 = 0,
            真 = 1,
            对 = 1
        }

        public delegate string 字符委托(string 参数1, string 参数2);
        public delegate bool 布尔委托(string 参数1, string 参数2);

        public static string 空(string 参数1, string 参数2)
        {
            return "错误：引用了不正确的目标。";
        }

        public static bool 错(string 参数1, string 参数2)
        {
            return false;
        }

        public static void 写入实体(List<string> 参数)
        {
            string 实体名 = 参数[0]; string 组件名 = 参数[1]; string 组件值 = 参数[2];
            if (!实体.ContainsKey(实体名))//如果没有这个实体就创建实体，并添加对应的组件
            {
                实体.Add(实体名, new Dictionary<string, string>());
                实体[实体名].Add(组件名, 组件值);
            }
            else
            {
                if (!实体[实体名].ContainsKey(组件名))
                {
                    实体[实体名].Add(组件名, 组件值);
                }
                else
                {
                    实体[实体名][组件名] = 组件值;
                }
            }
        }

        public static string 读取组件(string 获取语句)
        {
            List<string> 参数 = new List<string>(获取语句.Split(new[] { '的' }, StringSplitOptions.RemoveEmptyEntries));
            if (数据.实体.ContainsKey(参数[0]))
            {
                if (参数.Count == 1)
                {
                    参数.Add("值");
                }
                if (数据.实体[参数[0]].ContainsKey(参数[1]))
                {
                    return 数据.实体[参数[0]][参数[1]];
                }
                else
                {
                    return $"{参数[0]}没有{参数[1]}。";
                }
            }
            else
            {
                return $"没找到{参数[0]}。";
            }
        }
    }
}
