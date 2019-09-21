using Native.Csharp.Sdk.Cqp.Model;
using System.Collections.Generic;

namespace Native.Csharp.App.Event.Event_Me
{
    /// <summary>
    /// 全局变量
    /// </summary>
    public class Event_Variable
    {

        //群聊专用全局变量
        public static GroupMember member;
        public static string PT;
        public static bool isGroup = true;

        //静态全局变量与函数

        /// <summary>
        /// 上一次骰子的值
        /// </summary>
        public static int Number
        {
            get; set;
        }
        /// <summary>
        /// 上一次清点的值
        /// </summary>
        public static int CountValue
        {
            get; set;
        }
        /// <summary>
        /// 上一次计算的值
        /// </summary>
        public static string ComputeValue
        {
            get; set;
        }
        /// <summary>
        /// 自动补全参数是否开启
        /// </summary>
        public static bool Defa
        {
            get; set;
        }
        /// <summary>
        /// 机器人是开启还是关闭
        /// </summary>
        public static List<long> botCloseList = new List<long>();
        /// <summary>
        /// 变量需要被解释
        /// </summary>
        public static bool varNeedExp = true;

        /// <summary>
        /// 仅用于指示发送目标
        /// </summary>
        public static long idNum;

        public static long groupId;

        //public static float GetValue()
        //{
        //    return Values;
        //}

        /// <summary>
        /// 固有指令合集
        /// </summary>
        public static string CommandList
        {
            get; set;
        }

        /// <summary>
        /// 设置固有指令
        /// </summary>
        public static void SetCommandList()
        {
            CommandList = ".计算 .骰子 .创建 .清空 .销毁 .添加 .删除 .移动 .插入 .移除 " +
                          ".抽牌 .查看 .洗牌 .清数 .检索 .发现 .翻转 .导入 .属性 .定义 " +
                          ".报错 .去重 .复制 .棋盘 .日志 .转化 .如果 .清理 .开启 .关闭 " +
                          ".退群 .变量 .开始 .排序 .比赛";
        }
        /// <summary>
        /// 获取发言人的QQ号
        /// </summary>
        public static long QQQ;
        ///// <summary>
        ///// 变量键名
        ///// </summary>
        //public static List<string> vKey = new List<string>();
        ///// <summary>
        ///// 变量值名
        ///// </summary>
        //public static List<string> vValue = new List<string>();
        /// <summary>
        /// 变量集
        /// </summary>
        public static Dictionary<string, string> VariableList = new Dictionary<string, string>();
        /// <summary>
        /// 变量延迟拓展 true为开 默认为false
        /// </summary>
        public static bool varDelay;

        /// <summary>
        /// 帮助描述
        /// </summary>
        public static string helpDescription = $@"恋梦桌游姬V1.1.1 By未来菌
输入'.帮助 [指令名]'查看对应指令的详细解释
方括号内为参数，带*的为选填参数,带!的为自动补全参数：
.计算 [算式]
.骰子 [数量*] [面数!]
.创建 [区域] [区域*]
.销毁 [区域] [区域*]
.清空 [区域] [区域*]
.添加 [区域!] [牌名] [牌名*]
.删除 [区域!] [牌名] [牌名*]
.插入 [区域] [序号] [牌名] [牌名*]
.移除 [区域] [序号] [序号*]
.抽牌 [旧区域!] [新区域!] [数量*]
.出牌 [旧区域!] [新区域!] [牌名] [牌名*]
.查看 [区域!] [数量*]
.洗牌 [区域!] [区域*]
.排序 [区域]
.清数 [区域]
.检索 [区域] [字段] [所有*]
.发现 [区域] [数量*]
.翻转 [区域]
.去重 [区域]
.转化 [区域!] [旧牌名] [新牌名] [所有*]
.复制 [旧区域] [新区域] [新区域*]
.属性 [角色!] [键名:键值] [键名:键值*]
.导入 [区域] [文本]
.开始 [游戏名]
.棋盘 [房间号]
.变量 [变量名] [字符串/【区域】/删除]
.定义 [添加/删除] [新指令 甲 乙 丙]#[指令 甲 乙]#[指令 甲 丙*]
.如果 [表达式] [>/</=/!] [数值]?[指令 甲 乙]?[指令 甲 丙*]
.清理 [天数]（群主权限）
.退群 [QQ号*]（群管权限）
.开启/关闭 [QQ号*]

可在区域名开头加入'私密'，如'私密牌库'。非私密区域在某些卡牌变化场景会打印内容。
牌名可写成'假名【真名】'的形式，伪装的牌在离开非私密区域时会显露原形。
群聊输入2次扳机私发结果。子指令末尾加';'屏蔽结果（但也会因此无法得知错误的发生）。
扳机前加'~'开启变量延迟解释。指令末尾加'!'变量不解释；
使用'!'代替'.'作为扳机可自动补全部分指令，参考'开始'指令。
将机器人踢出群或者禁言可能导致账户被冻结或者引发程序异常，请使用退群和关闭指令来令机器人退群和暂停运行。
";

        /// <summary>
        /// 更新日志描述
        /// </summary>
        public static string updateLogDescription = @"更新日志：
Ver1.1.2：
指令和子指令末尾加上';'可以屏蔽回复；变量指令现在可以获取到角色属性；属性现在可以删除某个键；如果指令现在支持字符串比较；清点指令更名为清数；环境变量'骰子'更名为'骰点'。
Ver1.1.1（中秋版）：
大幅优化插件结构；新增排序指令；新增环境变量QQ、结果；指令末尾加'!'可以避免解释；优化洗牌、出牌算法；优化部分指令输入；修复各种Bug。
Plus1.1.0：
新增开始指令；部分指令现在可以用!代替.简化输入；群聊指令支持私发结果；重写了帮助指令；移动指令更名为出牌；出牌指令现在取符合条件的随机目标而不是最前方目标；变量指令的表达式输入改为字符串输入；变量现在可以全局使用。
Beta1.1.0：
现在开放群私聊操作；新增变量指令；新增开关指令；新增退群指令；如果指令现在支持表达式；清理与退群指令现在需要群管操作。";
    }
    /*
Ver1.0.3：新增如果指令；新增清理指令；现在计算指令支持mod、e、π；现在能自定义无参指令；指令集中的指令忘记加点现在会自动补全；检索结果现在会换行显示。
Ver1.0.2：删除报错指令；新增转化指令；降低误触几率。
Ver1.0.1：修复骰子过多触发的BUG；去除一个不必要的提示；增加查看更新日志的功能。
    */
}