using Native.Csharp.App.EventArgs;
using Native.Csharp.Sdk.Cqp.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Native.Csharp.App.Event.Event_Me
{
    public class 数据
    {
        public static bool 开发模式 = false;

        public static bool 私聊
        {
            get;set;
        }

        public static Dictionary<string, Dictionary<string, string>> 实体 = new Dictionary<string, Dictionary<string, string>>();

        public static Dictionary<string, string> 规则 = new Dictionary<string, string>();

        public static Dictionary<string, List<string>> 模块 = new Dictionary<string, List<string>>();

        public static string 词典位置 = AppDomain.CurrentDomain.BaseDirectory + @"app\dict.ini";

        public static string 词典读取 = File.ReadAllText(词典位置, Encoding.GetEncoding("gb2312"));

        public static List<string> 反义词典 => new List<string>(词典读取.Split(new string[] { "A", Environment.NewLine}, StringSplitOptions.RemoveEmptyEntries));

        public static Dictionary<string, string> DND核心法术 = null;

        public static bool 转义
        {
            get;set;
        }
        public static string 接口
        {
            get;set;
        }

        public static string 上次调用接口的时间
        {
            get;set;
        }

        public static string 报错
        {
            get
            {
                return new string[]
                {
                    "我懵了。", "哎？", "不对……", "怎么回事？", "什么情况？",
                    "奇怪的错误……", "搞错了吧？" , "……", "又来了……" , "¿",
                    "菜", "算了吧……" , "唔。", "瞎输啥啊……" , "[CQ:face,id=14]" , "[CQ:face,id=39]"
                }
                [new Random(Guid.NewGuid().GetHashCode()).Next(0, 16)];
            }
        }

        public const string 帮助 =
            @"桌游姬方若茗V0.1.0229
帮助文档：https://shimo.im/docs/dqcWQvHjk38QtRq3/
<部分指令>(*号表示必填参数)
.rd [掷骰表达式*] [原因]			普通掷骰
.nn [名称]					设置/删除昵称
.w/ww XaY						骰池
.set [1-99999之间的整数]			设置默认骰
.dnd [个数]					DND人物作成
.coc7/6 [个数]					COC7/6人物作成
.coc7/6d					详细版COC7/6人物作成
.ti/li					疯狂发作-临时/总结症状
.jrrp [on/off]				今日人品检定
.help						显示帮助
.开启/关闭 [机器人QQ号/QQ号后4位]		机器人开启或关闭";

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

        public static CqGroupMessageEventArgs 群聊目标
        {
            get;set;
        }
        public static CqPrivateMessageEventArgs 私聊目标
        {
            get; set;
        }
        public static CqDiscussMessageEventArgs 讨论组目标
        {
            get; set;
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

        public static GroupMember 娶群友(long QQ号)
        {
            return Common.CqApi.GetMemberInfo(群聊目标.FromGroup, QQ号, true);
        }

        public static string 获取昵称()
        {
            string 昵称 = "";
            if (!私聊 && 群聊目标 != null)
            {
                GroupMember 群友 = 娶群友(群聊目标.FromQQ);
                昵称 = string.IsNullOrWhiteSpace(群友.Card)//取群名片
                    ? 群友.Nick : 群友.Card;//取QQ昵称
                if (实体.ContainsKey(群聊目标.FromQQ.ToString()))//取玩家设置昵称
                {
                    if (实体[群聊目标.FromQQ.ToString()].ContainsKey("昵称"))
                    {
                        昵称 = 实体[群聊目标.FromQQ.ToString()]["昵称"];
                    }
                }
            }
            else
            {
                QQInfo 好友 = Common.CqApi.GetQQInfo(私聊目标.FromQQ);
                昵称 = 好友.Nick;
            }
            return 昵称 + "的";
        }

        public static void 写入实体(string 实体名, string 组件名, string 组件值)
        {
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

        [Obsolete]
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
            if (参数[0] == "我")
            {
                参数[0] = 私聊目标.FromQQ.ToString();
            }
            if (实体.ContainsKey(参数[0]))
            {
                if (参数.Count == 1)
                {
                    参数.Add("值");
                }
                if (实体[参数[0]].ContainsKey(参数[1]))
                {
                    return 实体[参数[0]][参数[1]];
                }
                else
                {
                    return 获取语句;
                }
            }
            else
            {
                return 获取语句;
            }
        }
    }
}
