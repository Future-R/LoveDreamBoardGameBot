using Native.Csharp.App.EventArgs;
using Native.Csharp.App.Interface;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Native.Csharp.App.Event.Event_Me
{
    /// <summary>
    /// 私聊回复
    /// </summary>
    public class Event_ReceiveFriendMessage : IReceiveFriendMessage
    {
        public void ReceiveFriendMessage(object sender, CqPrivateMessageEventArgs e)
        {
            string input = e.Message;
            Event_Variable.QQQ = e.FromQQ;
            Event_Variable.varDelay = false;
            try
            {
                //检查是否启用变量延迟解释
                if (input.StartsWith("~"))
                {
                    Event_Variable.varDelay = true;
                    input = input.Substring(1);
                }
            }
            catch (Exception ex)
            {
                Common.CqApi.SendPrivateMessage(e.FromQQ, Event_CheckError.CheckError(ex));
            }


            input = new Regex("[\\s]+").Replace(input, " ");//合并复数空格
            input = input.Trim();//去除前后空格
            //把用户输入的第一个中文句号替换为英文
            if (input.StartsWith("。"))
            {
                input = "." + input.Remove(0, 1);//为什么不顺便把感叹号也替换了呢？因为感叹号是对应单条指令的。有的需要感叹号，有的不需要。
            }
            if (!input.StartsWith(".") && !input.StartsWith("!") && !input.StartsWith("！"))//没有扳机就不触发
            {
                return;
            }
            if (input.Length < 2 || input == ".." || input == ".。" || input == "!!" || input == "！！")//两个字都没有！
            {
                return;
            }

            try//插件开关
            {
                if (input.StartsWith(".开启"))
                {
                    if (input.Length > 8)
                    {
                        if (input.Substring(3).Trim() == Convert.ToString(Common.CqApi.GetLoginQQ()))
                        {
                            Event_Variable.botCloseList.Remove(e.FromQQ);
                            Common.CqApi.SendPrivateMessage(e.FromQQ, $"{Convert.ToString(Common.CqApi.GetLoginQQ())}已开启！");
                        }
                        else
                        {
                            return;
                        }
                    }
                    else
                    {
                        Event_Variable.botCloseList.Remove(e.FromQQ);
                        Common.CqApi.SendPrivateMessage(e.FromQQ, "已开启！");
                    }
                }
                if (input.StartsWith(".关闭"))
                {
                    if (input.Length > 8)
                    {
                        if (input.Substring(3).Trim() == Convert.ToString(Common.CqApi.GetLoginQQ()))
                        {
                            Event_Variable.botCloseList.Add(e.FromQQ);
                            Event_Variable.botCloseList = Event_Variable.botCloseList.Distinct().ToList();
                            Common.CqApi.SendPrivateMessage(e.FromQQ, $"{Convert.ToString(Common.CqApi.GetLoginQQ())}已关闭！");
                        }
                        else
                        {
                            return;
                        }
                    }
                    else
                    {
                        Event_Variable.botCloseList.Add(e.FromQQ);
                        Event_Variable.botCloseList = Event_Variable.botCloseList.Distinct().ToList();
                        Common.CqApi.SendPrivateMessage(e.FromQQ, "已关闭！");
                    }
                }
            }
            catch (Exception)
            {
                Common.CqApi.SendPrivateMessage(e.FromQQ, "意外的错误！");
            }

            if (input.StartsWith(".帮助"))//帮助指令
            {
                if (input.Length < 5)//无参帮助
                {
                    Common.CqApi.SendPrivateMessage(e.FromQQ, Event_Variable.helpDescription);
                }
                else//含参帮助
                {
                    string helpCommand = input.Substring(3).Trim();//参数
                    string helpReturn = $@"{helpCommand}不是固有指令！";
                    switch (helpCommand.Substring(0, 2))
                    {
                        case "计算":
                            helpReturn = @"进行带mod运算的四则运算。支持e和π。
.计算 [算式]
范例：'.计算 (3+5)*2*π'";
                            break;
                        case "骰子":
                            helpReturn = @"投掷若干枚自定义面数的骰子，并计算总点数。
.骰子 [数量*] [面数!]
数量：指示骰子的枚数，约束在1~999。不填则默认为'1'。
面数：指示骰子是几面骰，约束在1~999。
范例：'.骰子 2 6'";
                            break;
                        case "创建":
                            helpReturn = @"创建1个可放置元素和记录文本的区域。
.创建 [区域]
范例：'.创建 私密牌堆'
值得一提的是，大部分指令紧接的后一个参数可以不使用空格分隔，如'创建私密牌堆'。";
                            break;
                        case "销毁":
                            helpReturn = @"销毁1个区域。
.销毁 [区域]
范例：'.计算 (3+5)*2*π'";
                            break;
                        case "添加":
                            helpReturn = @"添加若干张牌进入区域末端。
.添加 [区域!] [牌名] [牌名*]
牌名：指示需要添加到区域末端的卡牌名称。多个牌名可用空格关联。
范例：'.添加 手牌 幸运币 伪造的幸运币'";
                            break;
                        case "删除":
                            helpReturn = @"删除区域中对应牌名的最前端卡牌各1张。
.删除 [区域!] [牌名] [牌名*]
范例：'.删除 手牌 幽灵铠甲 进阶之灾'";
                            break;
                        case "插入":
                            helpReturn = @"创建若干张牌，并将其插入到区域的任意位置。
.插入 [区域] [序号] [牌名] [牌名*]
序号：指示你决定将这些卡牌插入到区域的第几张卡牌之前，最小为'1'，可填'随机'。
牌名：指示需要插入到区域中的卡牌名称。多个牌名可用空格关联。
范例：'.插入 战场 2 阿古斯防御者'";
                            break;
                        case "移除":
                            helpReturn = @"移除区域的第X张牌。
.移除 [区域] [序号]
序号：指示你意图移除区域第几张牌，最小为'1'，可填'随机'。
范例：'.移除 怪兽卡区域 3'";
                            break;
                        case "抽牌":
                            helpReturn = @"将某区域前端的若干张卡牌移动到另一个区域的末端。
.抽牌 [旧区域!] [新区域!] [数量*]
旧区域：指示卡牌的来源区域。这个区域将失去最前端的X张牌。
新区域：指示卡牌的目标区域。卡牌将进入这个区域的末端。
数量：指示移动的卡牌数量，最小为'1'，不填则默认为'1'。
范例：'.抽牌 卡组 手牌 5'";
                            break;
                        case "出牌":
                            helpReturn = @"从旧区域打出指定卡牌到新区域。
.出牌 [旧区域!] [新区域!] [牌名] [牌名*]
旧区域：指示卡牌的来源区域。这个区域将失去匹配牌名的随机卡牌。
新区域：指示卡牌的目标区域。卡牌将进入这个区域的末端。
牌名：指示需要打出的卡牌名称。多个牌名可用空格关联。
范例：'.出牌 手牌 战场 混沌法球'";
                            break;
                        case "查看":
                            helpReturn = @"查看区域内容。
.查看 [区域!]
范例：'.查看 手牌'
特别地，当区域存在'假名【真名】'形式的卡牌名称，则只会显示'假名'。
这个形式可用于模拟盖牌，以及为卡牌添加不可见的标记。
这个形式的卡牌离开非私密区域时会揭示标记。";
                            break;
                        case "洗牌":
                            helpReturn = @"将区域的所有卡牌打乱顺序。
.洗牌 [区域!]
范例：'.洗牌 私密牌堆'";
                            break;
                        case "清点":
                            helpReturn = @"计算区域内的卡牌数量。
.清点 [区域!]
范例：'.清点 手牌'";
                            break;
                        case "检索":
                            helpReturn = @"查找区域内包含关键字的卡牌并展示。
.检索 [区域] [字段] [所有*]
字段：指示你检索的关键字。
所有：输入'所有'则展示所有符合条件的对象，不填则展示随机1个符合条件的对象。
范例：'.检索 爆炸猫卡查 逗猫棒 所有'";
                            break;
                        case "发现":
                            helpReturn = @"随机展示区域若干张不重复的卡牌。
.发现 [区域] [数量*]
数量：指示需要展示的卡牌数量，不填则默认为'3'。
范例：'.发现 对手手牌 2'
特别地，当区域不重复的卡牌数量不足时，则展示所有同名牌各1张。";
                            break;
                        case "翻转":
                            helpReturn = @"将区域的牌序颠倒。
.翻转 [区域]
范例：'.翻转 卡组'";
                            break;
                        case "去重":
                            helpReturn = @"合并区域的同名牌。
.去重 [区域]
范例：'.去重 千秋戏'";
                            break;
                        case "转化":
                            helpReturn = @"将区域最前端1张或所有的指定卡牌转化为另一个名称。
.转化 [区域!] [旧牌名] [新牌名] [所有*]
旧牌名：指示需要转化的卡牌名称。
新牌名：指示转化后的卡牌名称。
所有：输入'所有'则转化所有符合条件的卡牌，不填则转化区域最前方的1张卡牌。
范例：'.转化 手牌 旋风 盖牌【旋风】'";
                            break;
                        case "复制":
                            helpReturn = @"创建1个区域并从另1个区域复制内容。
.复制 [旧区域] [新区域]
范例：'.复制 初始牌组 牌组'";
                            break;
                        case "属性":
                            helpReturn = @"记录或修改角色的属性。
.属性 [角色!] [键名:键值] [键名:键值*]
角色：指示你要记录或修改的角色名。使用'!'补全时，这个参数会补全为你的QQ号。
键名：指示角色某项属性的名称。'键名:键值'间使用空格来关联。
键值：指示角色某项属性的数据。冒号可替换为'+-*/'四则运算符进行键值的修改。
范例：'.属性
HP:10
MP-3
AP*2'
值得一提的是，所有指令的参数都能使用回车代替空格分隔。";
                            break;
                        case "导入":
                            helpReturn = @"将CSV格式的文本导入到区域末端。
.导入 [区域] [文本]
文本：文本的行格式为：'[牌名],[数量]'
牌名：指示需要导入到区域末端的卡牌名称。
数量：指示需要导入几张牌。
范例：'.导入 卡组
旋风,3
大风暴,1
大龙卷,3
热带低气压,1
大寒波,2'";
                            break;
                        case "开始":
                            helpReturn = @"开始1局游戏，自动绑定你的默认牌库、手牌、桌面和默认骰子面。绑定后，将'.'替换为'!'，可以自动补全部分指令
（骰子、添加、删除、抽牌、出牌、查看、转化、属性）
.开始 [游戏名]
范例：'.开始 心灵同步'
绑定后，可以简化操作如下：
.骰子 20
!骰子
.添加 手牌 幸运币
!添加 幸运币
.抽牌 卡组 手牌 5
!抽牌 5
.出牌 手牌 战场 混沌法球
!出牌 混沌法球
.属性 858271917 骰子:100
!属性 骰子:100
想要修改绑定的信息，可以使用'开始'指令重新初始化，也可使用'属性'指令对某项绑定对象进行修改。";
                            break;
                        case "棋盘":
                            helpReturn = @"打开1个公共可写的石墨表格作为棋盘。
.棋盘 [房间号]
房间号：指示你需要打开的房间号，约束在0~9。
范例：'.棋盘 2";
                            break;
                        case "变量":
                            helpReturn = @"设置1个变量，变量在所有指令中优先被解释。
.变量 [变量名] [字符串/【区域】/删除]
变量名：指示设置的变量名称。使用成对的英文方括号将变量名括起才能使用。
字符串：变量名的后1个参数可输入。输入一个字符串，字符串会被赋值给变量，如范例所示。
【区域】：变量名的后1个参数可输入。使用成对的中文方括号括起区域名，对应区域的内容将会读取并赋值给变量。
删除：变量名的后1个参数可输入。删除指定名称的变量。
范例：'.变量 Var 看新世'
'.查[Var]界'会被解释为'.查看新世界'并执行。
在指令扳机前输入'~'如：'~.查看[var]'可以开启变量延迟解释，用于解释定义复合指令中的变量。
变量非常适合搭配'如果'指令使用。";
                            break;
                        case "定义":
                            helpReturn = @"自定义1个新指令。新指令会自动执行'#'后的每条指令。
.定义 [添加/删除] [.新指令 甲 乙 丙]#[.指令 甲 乙]#[.指令 甲 丙*]
添加/删除：指示了需要添加还是删除指令。删除指令时不需要输入指令参数，如：'.定义 删除 .抽指定牌'。
新指令 甲 乙 丙：新指令为自定义指令名；甲乙丙为自定义指令的参数。参数的个数必须为正整数。
指令 甲 乙：指示被自定义指令执行的复合指令中的1个指令。自定义指令执行时，子指令的参数会被转义为自定义指令的参数。指令之间用'#'连接。
范例：'.定义 添加 .抽指定牌 牌名#.出牌 私密套牌 手牌 牌名#.检索 手牌 牌名'
'.定义 删除 .抽指定牌'
特别地，你可以忽略'定义'和'如果'指令中指令名前的'.'扳机，而程序会为你自动补全：
'.定义 添加 抽指定牌 牌名#出牌 私密套牌 手牌 牌名'
但是如果你希望将结果私发，只能手动输入指令中所有的扳机：
'.定义 添加 抽指定牌 牌名#..出牌 私密套牌 手牌 牌名''";
                            break;
                        case "如果":
                            helpReturn = @"如果表达式的结果大于/小于/等于/不等于指定值，执行'?'后的每条指令。
.如果 [表达式] [>/</=/!] [数值]?[指令 甲 乙]?[指令 甲 丙*]
表达式：表达式应为四则运算式子。表达式支持环境变量'骰子'和'清点'，它们的值为上次'骰子'和'清点'指令的结果。
>/</=/!：指示了比较符，代表的含义分别为大于、小于、等于、不等于。如果比较的结果为'真'，执行'?'后的每条指令。
数值：指示了比较的对象。这个参数限制为纯数。
指令 甲 乙：复合指令的子指令。可参考'定义'指令。
范例：'.如果 (骰子+1)*2 > [Var]?.添加 骰池 骰子?.清点 骰池'
'[变量名]'的形式表示这是一个自定义变量，详情参考'变量'指令。
如果你希望将结果私发，只需要在指令而非子指令前连发两次扳机，这点与'定义'指令不同。
'如果'指令非常适合嵌入到'定义'指令中使用。";
                            break;
                        case "清理":
                            helpReturn = @"需要群主权限。清理若干天前创建的所有数据。
.清理 [天数]
天数：指示清理多少天前创建的所有数据。限制为非0自然数。
范例：'.清理 90'
为了其他玩家的游戏体验，强烈建议您不要清理90天以内的数据。";
                            break;
                        case "退群":
                            helpReturn = @"需要管理员权限。使机器人退群。
.退群 [QQ号*]
QQ号：输入QQ号使对应的1个机器人退群，不填则使所有机器人退群。
范例：'.退群 858271917'
将机器人踢出群可能导致账户冻结，请使用这个指令使机器人安全退群。";
                            break;
                        case "开启":
                            helpReturn = @"结束机器人的关闭状态。
.开启 [QQ号*]
QQ号：输入QQ号使对应的1个机器人开启，不填则使所有机器人开启。
范例：'.开启 858271917'
将机器人禁言可能导致账号数据异常，请使用这个指令开关机器人。输入QQ号开关群内对应的1个机器人。关闭期间不会处理群聊指令";
                            break;
                        case "关闭":
                            helpReturn = @"结束机器人的开启状态。
.关闭 [QQ号*]
QQ号：输入QQ号使对应的1个机器人关闭，不填则使所有机器人关闭。
范例：'.关闭 858271917'
将机器人禁言可能导致账号数据异常，请使用这个指令开关机器人。输入QQ号开关群内对应的1个机器人。关闭期间不会处理群聊指令";
                            break;

                        default:
                            break;
                    }
                    Common.CqApi.SendPrivateMessage(e.FromQQ, helpReturn);
                }
                return;
            }


            //用户输入指令
            if (!Event_Variable.botCloseList.Exists((long f) => f == e.FromQQ))
            {
                if (!Event_Variable.varDelay)
                {
                    int vvc = 0;
                    foreach (var item in Event_Variable.vValue)//变量解释器
                    {
                        input = input.Replace(Event_Variable.vKey[vvc], item);
                        vvc++;
                    }
                }

                //判断路径是否存在
                if (!Directory.Exists(AppDomain.CurrentDomain.BaseDirectory + @"SaveDir\Command\"))
                {
                    Directory.CreateDirectory(AppDomain.CurrentDomain.BaseDirectory + @"SaveDir\Command\");//不存在就创建
                    Directory.CreateDirectory(AppDomain.CurrentDomain.BaseDirectory + @"SaveDir\Att\");//属性功能初始化
                }

                try
                {
                    List<string> commandInput = new List<string>(input.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries));//切割用户输入的指令 .xx a b c
                    if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + @"SaveDir\Command\" + //如果找到用户定义的指令
                        "." + commandInput[0].TrimStart(Convert.ToChar(".")) + ".ini"))//复数点视为成单个点
                    {
                        var tempLoad = File.ReadAllText(AppDomain.CurrentDomain.BaseDirectory + @"SaveDir\Command\" + commandInput[0] + ".ini").Trim();
                        if (tempLoad.Substring(0, 1) == "#")//如果自定义指令无参 添加一个nil作为参数
                        {
                            commandInput.Add("nil!");
                            tempLoad = "nil!" + tempLoad;
                        }
                        List<string> loadCommandList = new List<string>(tempLoad.Split(new string[] { "#" }, StringSplitOptions.RemoveEmptyEntries));//A B C   .删除 A B   .插入 随机 C

                        List<string> loadVarList = new List<string>(loadCommandList[0].Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries));//A B C
                        List<string> loadEndList = new List<string>();//替换后的列表
                        string tempItem;
                        foreach (var item in loadCommandList)
                        {
                            tempItem = item;
                            for (int i = 0; i < loadVarList.Count; i++)
                            {
                                //将loadCommandList每一项中的 loadVarList第i项 替换为 commandInput第i+1项, 传入loadEndList
                                tempItem = tempItem.Replace(loadVarList[i], commandInput[i + 1]);
                            }
                            loadEndList.Add(tempItem);
                        }
                        loadEndList.RemoveAt(0);//.删除 区域 牌名1   .插入 随机 牌名2
                        string temp;
                        foreach (var item in loadEndList)
                        {
                            temp = item;
                            Event_Variable.Defa = false;
                            if (temp.StartsWith("！") || temp.StartsWith("!"))//该指令是自动补全参数指令
                            {
                                Event_Variable.Defa = true;
                                temp = "." + temp.Remove(0, 1);
                            }
                            if (Event_Variable.varDelay)
                            {
                                int vvc = 0;
                                foreach (var itemx in Event_Variable.vValue)//变量解释器
                                {
                                    temp = temp.Replace(Event_Variable.vKey[vvc], itemx);
                                    vvc++;
                                }
                            }
                            CommandIn(temp, e);
                        }

                    }
                    else
                    {
                        Event_Variable.Defa = false;
                        if (input.StartsWith("！") || (input.StartsWith("!")))//该指令是自动补全参数指令
                        {
                            Event_Variable.Defa = true;
                            input = "." + input.Remove(0, 1);
                        }
                        if (Event_Variable.varDelay)
                        {
                            int vvc = 0;
                            foreach (var itemx in Event_Variable.vValue)//变量解释器
                            {
                                input = input.Replace(Event_Variable.vKey[vvc], itemx);
                                vvc++;
                            }
                        }
                        CommandIn(input, e);//查一下是不是固有指令
                    }
                }
                catch (Exception)
                {
                    Common.CqApi.SendPrivateMessage(e.FromQQ, "指令输入错误！输入'.帮助'检查你的格式！");
                }
            }
        }

        //模块区_______________________________________________________________________________________________________________________________________________________________________

        //固有指令
        public void CommandIn(string input, CqPrivateMessageEventArgs e)
        {
            input = input.Replace("&#91;", "[").Replace("&#93;", "]");
            if (input.Length < 2)//降低错误触发
            {
                return;
            }
            switch (input.Substring(1, 2))//偷懒，只匹配.后2个字符
            {

                case "计算":
                    Common.CqApi.SendPrivateMessage(e.FromQQ, Calc(input));
                    return;

                case "骰子":
                    Common.CqApi.SendPrivateMessage(e.FromQQ, Dices(input));
                    return;

                case "创建":
                    switch (Crea(input))
                    {
                        case 0:
                            Common.CqApi.SendPrivateMessage(e.FromQQ, $@"创建{input.Trim().Substring(3).Trim()}成功！");//待优化
                            return;

                        default:
                            Common.CqApi.SendPrivateMessage(e.FromQQ, $@"创建{input.Trim().Substring(3).Trim()}失败！");
                            return;
                    }

                case "销毁":
                    Common.CqApi.SendPrivateMessage(e.FromQQ, Boom(input, out string leave1) + Environment.NewLine + leave1);
                    return;

                case "清空":
                    Boom(input, out string leave2);
                    if (leave2.Length > 1)
                    {
                        Common.CqApi.SendPrivateMessage(e.FromQQ, leave2);
                    }
                    switch (Crea(input))//创建
                    {
                        case 0:
                            Common.CqApi.SendPrivateMessage(e.FromQQ, $@"清空{input.Trim().Substring(3).Trim()}成功！");
                            return;

                        default:
                            Common.CqApi.SendPrivateMessage(e.FromQQ, $@"{input.Trim().Substring(3).Trim()}丢失！");
                            return;
                    }


                case "添加":
                    Common.CqApi.SendPrivateMessage(e.FromQQ, Add(input));
                    return;

                case "删除":
                    Common.CqApi.SendPrivateMessage(e.FromQQ, $@"{DelInfos(input, out string trueNameList)}");
                    if (trueNameList.Length >= 1) Common.CqApi.SendPrivateMessage(e.FromQQ, $@"{trueNameList.Trim()}离开了区域！");
                    return;

                case "出牌":
                    Common.CqApi.SendPrivateMessage(e.FromQQ, Move(input, out string fakeName));
                    if (fakeName.Length >= 1) Common.CqApi.SendPrivateMessage(e.FromQQ, $@"{fakeName.Trim()}离开了区域！");
                    return;

                case "抽牌":
                    Common.CqApi.SendPrivateMessage(e.FromQQ, Draw(input));
                    return;

                case "查看":
                    GetInfo(input, out string looname, out string looret, out string loofak);
                    Common.CqApi.SendPrivateMessage(e.FromQQ, $@"{looname}:
{loofak}");
                    return;

                case "清点":
                    CountNum(input, out string num);
                    Event_Variable.CountValue = int.Parse(num);
                    Common.CqApi.SendPrivateMessage(e.FromQQ, $@"{input.Substring(3).Trim()}有{num}张牌");
                    return;

                case "检索":
                    Common.CqApi.SendPrivateMessage(e.FromQQ, Search(input));
                    return;

                case "移除":
                    Common.CqApi.SendPrivateMessage(e.FromQQ, RemovePos(input));
                    return;

                case "导入":
                    Common.CqApi.SendPrivateMessage(e.FromQQ, CopyIn(input));
                    return;

                case "发现":
                    Common.CqApi.SendPrivateMessage(e.FromQQ, DisCover(input));
                    return;

                case "复制":
                    Common.CqApi.SendPrivateMessage(e.FromQQ, CopyTo(input));
                    return;

                case "洗牌":
                    Common.CqApi.SendPrivateMessage(e.FromQQ, Shuffle(input));
                    return;

                case "插入":
                    Common.CqApi.SendPrivateMessage(e.FromQQ, Inser(input));
                    return;

                case "翻转":
                    Common.CqApi.SendPrivateMessage(e.FromQQ, Reverse(input));
                    return;

                case "定义":
                    Common.CqApi.SendPrivateMessage(e.FromQQ, Command(input));
                    return;

                case "属性":
                    Common.CqApi.SendPrivateMessage(e.FromQQ, Attributes(input));
                    return;

                case "去重":
                    Common.CqApi.SendPrivateMessage(e.FromQQ, DelSame(input));
                    return;

                case "转化":
                    Common.CqApi.SendPrivateMessage(e.FromQQ, Variety(input));
                    return;

                case "变量":
                    Common.CqApi.SendPrivateMessage(e.FromQQ, Variable(input));
                    return;

                case "开始":
                    Common.CqApi.SendPrivateMessage(e.FromQQ, GameStart(input, e));
                    Common.CqApi.SendPrivateMessage(e.FromQQ, "设定完毕！");
                    return;

                case "棋盘":
                    if (IsNumeric(input.Substring(3).Trim()))
                    {
                        string[] tables = { "GHhvRqDCJ9JJdk3R/" , "Vw8vwjRDrH9YWwTG/", "jJ3qyHpyrxtqGwWw/",
                        "dDwgDJg9g99KT63h/" , "GQcHhVWd9DkY6KTR/" , "gdKYVKkCg8qKVjGk/" , "K3QyPhxY6rhrQJtx/" ,
                        "HG8wPtjdwkyCGdrT/" , "t3qk6T3dyyxDddkG/" , "kYKCw8jcWCVkJgRj/" };
                        Common.CqApi.SendPrivateMessage(e.FromQQ, $@"https://shimo.im/sheets/" + $@"{tables[int.Parse(input.Substring(3).Trim())]} 可复制链接后用石墨文档 App 或小程序打开");
                    }
                    else
                    {
                        Common.CqApi.SendPrivateMessage(e.FromQQ, "房间号有误！");
                    }
                    return;

                case "日志":
                    Common.CqApi.SendPrivateMessage(e.FromQQ, Event_Variable.updateLogDescription);
                    return;

                case "如果"://.如果 [表达式] [>/</=/!] [指定值]?[指令 A B]
                    input = input.Substring(3).Trim().Replace("大于", ">").Replace("小于", "<").Replace("等于", "=").Replace("？", "?").Replace("！", "!").Replace("不等于", "!");//[表达式] [>/</=/!] [指定值]?[指令 A B]

                    try
                    {
                        List<string> sharpInput = new List<string>(input.Split(new string[] { "?" }, StringSplitOptions.RemoveEmptyEntries));
                        List<string> expreesInput = new List<string>(sharpInput[0].Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries));//[表达式] [>/</=/!] [指定值]

                        string expression = expreesInput[0].Replace("骰子", $"{Event_Variable.Number}").Replace("清点", $"{Event_Variable.CountValue}");
                        string oper = expreesInput[1];
                        string value = expreesInput[2];

                        sharpInput.RemoveAt(0);
                        for (int i = 0; i < sharpInput.Count; i++)
                        {
                            if (!sharpInput[i].StartsWith(".") && !sharpInput[i].StartsWith("。"))
                            {
                                sharpInput[i] = "." + sharpInput[i];
                            }
                        }
                        switch (oper)
                        {
                            case ">":
                                if ((int)new DataTable().Compute(expression, "") > int.Parse(value))
                                {
                                    foreach (var item in sharpInput)
                                    {

                                        CommandIn(item, e);

                                    }
                                }
                                break;

                            case "<":
                                if ((int)new DataTable().Compute(expression, "") < int.Parse(value))
                                {
                                    foreach (var item in sharpInput)
                                    {

                                        CommandIn(item, e);

                                    }
                                }
                                break;

                            case "=":
                                if ((int)new DataTable().Compute(expression, "") == int.Parse(value))
                                {
                                    foreach (var item in sharpInput)
                                    {

                                        CommandIn(item, e);

                                    }
                                }
                                break;

                            case "!":
                                if ((int)new DataTable().Compute(expression, "") != int.Parse(value))
                                {
                                    foreach (var item in sharpInput)
                                    {

                                        CommandIn(item, e);

                                    }
                                }
                                break;

                            default:
                                Common.CqApi.SendGroupMessage(e.FromQQ, "找不到比较符！");
                                break;
                        }
                    }
                    catch (Exception)
                    {
                        Common.CqApi.SendGroupMessage(e.FromQQ, "抛出异常！");
                    }
                    return;
            }
        }

        //计算
        public string Calc(string input)
        {
            input = input.Substring(3).Trim().Replace("×", "*").Replace("x", "*").Replace("X", "*")
                            .Replace("（", "(").Replace("）", ")").Replace("÷", "/").Replace("％", "/100")
                            .Replace("%", "/100").Replace("e", "(" + Convert.ToString(Math.E) + ")")
                            .Replace("π", "(" + Convert.ToString(Math.PI) + ")").Replace("mod", "%");//中文运算符都换成程序运算符

            try
            {
                object result = new DataTable().Compute(input, "");
                return "计算结果为：" + result;
            }
            catch (Exception)
            {
                return "请输入正确的四则算式！";
            }
        }

        //调用Js运算可能会导致酷Q被杀软拦截，不建议使用
        //public static object Eval(string s)
        //{
        //    Microsoft.JScript.Vsa.VsaEngine ve = Microsoft.JScript.Vsa.VsaEngine.CreateEngine();
        //    return Microsoft.JScript.Eval.JScriptEvaluate(s, ve);
        //}

        //骰子
        public string Dices(string input)
        {
            try
            {
                if (Event_Variable.Defa == true)
                {
                    if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + @"SaveDir\Att\" + Event_Variable.QQQ + ".ini"))//如果属性文件存在
                    {
                        string att = LoadInfo(@"Att\" + Event_Variable.QQQ);
                        string diceFace = "";
                        foreach (string item in new List<string>(att.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries)))
                        {
                            if (item.StartsWith("骰子:"))
                            {
                                diceFace = item.Substring(3);
                            }
                        }
                        input = input + " " + diceFace;
                    }
                    else
                    {
                        return "请先使用'开始'指令绑定默认骰子面数！";
                    }
                }
                input = input.Substring(3).Trim();
                string[] inputArray = input.Split(new char[3] { 'D', 'd', ' ' });


                if (inputArray.Length > 1)//多个参数
                {
                    if (IsNumeric(inputArray[1]))//如果第二个参数是纯数，则为复数骰子
                    {
                        try
                        {
                            if (int.Parse(inputArray[0]) > 999 || int.Parse(inputArray[0]) < 1 || int.Parse(inputArray[1]) > 999 || int.Parse(inputArray[1]) < 1)
                            {
                                return "数字超界！";
                            }
                            string results = "";
                            for (int i = 0; i < Convert.ToInt32(inputArray[0]); i++)
                            {
                                int result = new Random(Guid.NewGuid().GetHashCode()).Next(0, Convert.ToInt32(inputArray[1])) + 1;
                                results = results + "+" + result;
                            }
                            Event_Variable.Number = (int)new DataTable().Compute(results, "");
                            return results.Substring(1) + " = " + new DataTable().Compute(results, "");
                        }
                        catch (Exception)
                        {
                            return "非法输入！";
                        }
                    }
                    else//否则是单个骰子
                    {
                        try
                        {
                            if (int.Parse(inputArray[0]) > 999 || int.Parse(inputArray[0]) < 1)
                            {
                                return "数字超界！";
                            }
                            Random rd = new Random();
                            int result = rd.Next(0, Convert.ToInt32(inputArray[0])) + 1;
                            Event_Variable.Number = result;
                            return $"{result}";
                        }
                        catch (Exception)
                        {
                            return "非法输入！";
                        }
                    }
                }
                else//否则是单个骰子
                {
                    try
                    {
                        if (int.Parse(inputArray[0]) > 999 || int.Parse(inputArray[0]) < 1)
                        {
                            return "数字超界！";
                        }
                        Random rd = new Random();
                        int result = rd.Next(0, Convert.ToInt32(inputArray[0])) + 1;
                        Event_Variable.Number = result;
                        return $"{result}";
                    }
                    catch (Exception)
                    {

                        return "非法输入！";
                    }
                }
            }
            catch (Exception)
            {
                return "非法输入！";
            }

        }

        //创建
        public int Crea(string input)
        {
            try
            {
                input = input.Substring(3).Trim();
                string[] inputArray = input.Split(' ');

                if (inputArray.Length == 1)//没有模板
                {
                    return CreateInfo(inputArray[0]);//接收返回值
                }
                else
                {
                    return 3;
                }
            }
            catch (Exception)
            {
                return 4;
            }

        }

        //销毁
        public string Boom(string input, out string leave)
        {
            GetInfo(input, out string name, out string reture, out string refake);
            List<string> tempInput = new List<string>(reture.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries));
            List<string> fromListFake = new List<string>();

            leave = refake + "离开了区域！";
            if (leave.Length < 7) leave = "";
            int ret = DelInfo(name);
            if (ret == 2)
            {
                return $@"找不到{name}！";
            }
            else
            {
                if (ret == 0)
                {
                    return $@"{name}已销毁！";
                }
                else
                {
                    return $@"{name}清空失败！";
                }
            }
        }

        //添加
        public string Add(string input)//.删除 区域 AA BB CC
        {
            input = input.Substring(3).Trim();
            if (Event_Variable.Defa == true)
            {
                if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + @"SaveDir\Att\" + Event_Variable.QQQ + ".ini"))//如果属性文件存在
                {
                    string att = LoadInfo(@"Att\" + Event_Variable.QQQ);
                    string hand = "";
                    foreach (string item in new List<string>(att.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries)))
                    {
                        if (item.StartsWith("手牌:"))
                        {
                            hand = item.Substring(3);
                        }
                    }
                    input = hand + " " + input;
                }
                else
                {
                    return "请先使用'开始'指令绑定手牌！";
                }
            }
            int spsNum = input.IndexOf(" ");
            try
            {
                //如果文件存在的话……我会抽时间优化一下这块代码
                if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + @"SaveDir\" + input.Substring(0, spsNum) + ".ini"))
                {
                    try
                    {
                        WriteInfo(input.Substring(0, spsNum), input.Substring(spsNum + 1));
                        if (input.Substring(0, spsNum).Contains("私密"))
                        {
                            return "添加成功！";
                        }
                        else
                        {
                            return $@"{input.Substring(0, spsNum)}:
{LoadInfo(input.Substring(0, spsNum))}";//打印更改后的内容
                        }

                    }
                    catch (Exception)
                    {

                        return $@"添加{input.Substring(spsNum + 1)}失败！";
                    }
                }
                else
                {
                    return $@"{input.Substring(0, spsNum)}不存在！";
                }
            }
            catch
            {
                return $@"非法输入！";
            }

        }

        /// <summary>
        /// 获取信息
        /// </summary>
        /// <param name="input">.查看 [区域] XXX</param>
        /// <param name="name">区域</param>
        /// <param name="reture">全称列表</param>
        /// <param name="refake">假名列表</param>
        public void GetInfo(string input, out string name, out string reture, out string refake)
        {
            if (Event_Variable.Defa == true)
            {
                if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + @"SaveDir\Att\" + Event_Variable.QQQ + ".ini"))//如果属性文件存在
                {
                    string att = LoadInfo(@"Att\" + Event_Variable.QQQ);
                    string hand = "";
                    foreach (string item in new List<string>(att.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries)))
                    {
                        if (item.StartsWith("手牌:"))
                        {
                            hand = item.Substring(3);
                        }
                    }
                    input = input + " " + hand;
                }
                else
                {
                    name = "请先使用'开始'指令绑定手牌";
                }
            }
            input = input.Substring(3).Trim();
            string[] inputArray = input.Split(' ');
            name = inputArray[0];
            if (!File.Exists(AppDomain.CurrentDomain.BaseDirectory + @"SaveDir\" + name + ".ini"))
            {
                name = name + "(区域不存在)";
            }
            List<string> list = new List<string>(LoadInfo(inputArray[0]).Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries));//全称列表
            List<string> list2 = new List<string>();//隐藏真名的列表
            //List<string> listTrue = new List<string>();//只含真名的列表

            reture = LoadInfo(inputArray[0]);
            foreach (var item in list)
            {
                if (item.Contains("【") && item.Contains("】"))
                {
                    int startPos = item.IndexOf("【");
                    //int endPos = item.IndexOf("】");
                    //listTrue.Add(item.Substring(startPos + 1, endPos - startPos - 1));//获取真名
                    list2.Add(item.Substring(0, startPos));//截去真名
                }
                else
                {
                    list2.Add(item);
                }
            }
            refake = string.Join(" ", list2.ToArray());
        }

        //删除
        public string DelInfos(string input, out string trueName)//.删除 区域 AA BB CC
        {
            trueName = "";
            if (Event_Variable.Defa == true)
            {
                if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + @"SaveDir\Att\" + Event_Variable.QQQ + ".ini"))//如果属性文件存在
                {
                    string att = LoadInfo(@"Att\" + Event_Variable.QQQ);
                    string hand = "";
                    foreach (string item in new List<string>(att.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries)))
                    {
                        if (item.StartsWith("手牌:"))
                        {
                            hand = item.Substring(3);
                        }
                    }
                    input = input.Substring(0, 3).Trim() + " " + hand + " " + input.Substring(3).Trim();
                }
                else
                {
                    return "请先使用'开始'指令绑定手牌";
                }
            }
            GetInfo(input, out string remname, out string truRet, out string fakRet);//获取区域内容
            List<string> tempInput = new List<string>(input.Substring(3).Trim().Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries));//区域 AA BB CC
            try
            {
                if (File.Exists(System.AppDomain.CurrentDomain.BaseDirectory + @"SaveDir\" + tempInput[0] + ".ini"))
                {
                    List<string> delList = new List<string>(tempInput.GetRange(1, tempInput.Count - 1));//肃反名单
                    GetInfo($@".查看 {tempInput[0]}", out string fromName, out string fromTList, out string fromFList);
                    List<string> collection = new List<string>(fromFList.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries));//假名列表（AA BB CC DD）
                    List<string> collection2 = new List<string>(fromTList.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries));//真名列表（AA BB CC DD）
                    int find;
                    for (int i = 0; i < delList.Count; i++)
                    {
                        find = collection.FindIndex((string f) => f.Equals(delList[i]));
                        if (find == -1)
                        {
                            trueName = "";
                            return $@"无法找到{string.Join(" ", delList.ToArray())}";
                        }
                        if (collection2[find + i].Contains("【"))
                        {
                            trueName += $@" {collection2[find + i]}";
                        }
                        collection.Remove(delList[i]);
                    }
                    DelInfo(tempInput[0]); CreateInfo(tempInput[0]);
                    WriteInfo(tempInput[0], $@"{string.Join(" ", collection.ToArray())}");
                    return $@"{tempInput[0]}:
{string.Join(" ", collection.ToArray())}";
                }
                else
                {
                    return "区域不存在！";
                }
            }
            catch (Exception)
            {
                return "非法输入！";
            }
        }

        //洗牌
        public string Shuffle(string input)
        {
            try
            {
                input = input.Substring(3).Trim();
                if (Event_Variable.Defa == true)
                {
                    if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + @"SaveDir\Att\" + Event_Variable.QQQ + ".ini"))//如果属性文件存在
                    {
                        string att = LoadInfo(@"Att\" + Event_Variable.QQQ);
                        string deck = "";
                        foreach (string item in new List<string>(att.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries)))
                        {
                            if (item.StartsWith("牌堆:"))
                            {
                                deck = item.Substring(3);
                            }
                        }
                        input = input + " " + deck;
                    }
                    else
                    {
                        return "请先使用'开始'指令绑定牌堆";
                    }
                }
                string inputs = LoadInfo(input);
                if (inputs != "" || inputs != "读取错误！")
                {
                    List<string> list = new List<string>(inputs.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries));
                    List<string> list2 = new List<string>();
                    int count = list.Count;
                    for (int i = 0; i < count; i++)
                    {
                        int rd = new Random(Guid.NewGuid().GetHashCode()).Next(0, Convert.ToInt32(list.Count - 1));
                        list2.Add(list[rd]);
                        list.RemoveAt(rd);
                    }
                    DelInfo(input); CreateInfo(input);
                    string notExistValue = string.Join(" ", list2.ToArray());
                    WriteInfo(input, $@"{notExistValue}");
                    return "洗切完成！";
                }
                else
                {
                    return "洗切失败！";
                }
            }
            catch (Exception)
            {
                return "洗切失败！";
            }

        }

        //出牌
        public string Move(string input, out string trueName)//.出牌 区域A 区域B XXX YYY ZZZ
        {
            string fakeName = "";
            trueName = "";
            try
            {
                input = input.Substring(3).Trim();
                if (Event_Variable.Defa == true)
                {
                    if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + @"SaveDir\Att\" + Event_Variable.QQQ + ".ini"))//如果属性文件存在
                    {
                        string att = LoadInfo(@"Att\" + Event_Variable.QQQ);
                        string oldZone = "";
                        string newZone = "";
                        foreach (string item in new List<string>(att.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries)))
                        {
                            if (item.StartsWith("手牌:"))
                            {
                                oldZone = item.Substring(3);
                            }
                            if (item.StartsWith("桌面:"))
                            {
                                newZone = item.Substring(3);
                            }
                        }
                        input = oldZone + " " + newZone + " " + input;
                    }
                    else
                    {
                        return "请先使用'开始'指令绑定手牌和桌面！";
                    }
                }
                List<string> list = new List<string>(input.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries));//区域A 区域B XXX YYY ZZZ
                string name1 = list[0];
                string name2 = list[1];
                List<string> nameList = new List<string>();
                for (int i = 0; i < list.Count - 2; i++)
                {
                    nameList.Add(list[i + 2]);//需要移动的对象
                }
                List<string> fromList = new List<string>(
                    File.ReadAllText(AppDomain.CurrentDomain.BaseDirectory + @"SaveDir\" + name1 + ".ini")
                    .Trim().Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries));//来源区域全称
                List<string> toList = new List<string>(
                    File.ReadAllText(AppDomain.CurrentDomain.BaseDirectory + @"SaveDir\" + name2 + ".ini")
                    .Trim().Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries));//目标区域
                List<string> fromListFake = new List<string>();//来源区域假名

                try
                {
                    foreach (var item in fromList)//fromList截去真名，传入fromListFake
                    {
                        if (item.Contains("【"))
                        {
                            int startPos = item.IndexOf("【");
                            fromListFake.Add(item.Substring(0, startPos));//截去真名
                        }
                        else
                        {
                            fromListFake.Add(item);
                        }
                    }
                    fakeName = string.Join(" ", fromListFake.ToArray());

                    int find;
                    for (int i = 0; i < nameList.Count; i++)
                    {
                        find = fromListFake.FindIndex((string f) => f.Equals(nameList[i]));
                        if (find == -1)
                        {
                            trueName = "";
                            return $@"无法找到{string.Join(" ", nameList.ToArray())}";
                        }
                        if (fromList[find + i].Contains("【"))
                        {
                            trueName += $@" {fromList[find + i]}";
                        }
                        toList.Add(fromList[find + i]);
                        fromList.Remove(fromList[find + i]);
                    }
                    List<int> strNum = new List<int>();
                    int strNumRD;
                    foreach (var item in nameList)//遍历需要移动的牌名名单
                    {
                        for (int i = 0; i < fromListFake.Count; i++)//遍历来源区域假名
                        {
                            if (item == fromListFake[i])//如果名单对应上某假名
                            {
                                strNum.Add(i);//将假名的位置添加到strNum
                            }
                        }
                        strNumRD = new Random(Guid.NewGuid().GetHashCode()).Next(0, Convert.ToInt32(strNum.Count - 1));
                        fromListFake.RemoveAt(strNum[strNumRD]);
                        strNum.Clear();
                    }
                    DelInfo(name1); CreateInfo(name1);
                    WriteInfo(name1, $@" {string.Join(" ", fromList.ToArray())}");//写入来源区域
                    DelInfo(name2); CreateInfo(name2);
                    WriteInfo(name2, $@" {String.Join(" ", toList.ToArray())}");//写入目标区域
                    if (name2.Contains("私密"))
                    {
                        return "出牌完毕！";
                    }
                    else
                    {
                        List<string> toListFake = new List<string>();
                        foreach (var item in toList)//toList截去真名，传入toListFake
                        {
                            if (item.Contains("【"))
                            {
                                int startPos = item.IndexOf("【");
                                toListFake.Add(item.Substring(0, startPos));//截去真名
                            }
                            else
                            {
                                toListFake.Add(item);
                            }
                        }
                        if (name1.Contains("私密"))
                        {
                            trueName = "";
                        }
                        return $@"{name2}:
{string.Join(" ", toListFake.ToArray())}";//打印更改后的内容
                    }

                }
                catch (Exception)
                {
                    trueName = "";
                    return "出牌失败！";
                }

            }
            catch (Exception)
            {
                trueName = "";
                return "出牌失败！";
            }
        }

        //抽牌
        public string Draw(string input)//.抽牌 旧区域 新区域 数量
        {
            try
            {
                input = input.Substring(3).Trim();
                if (Event_Variable.Defa == true)
                {
                    if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + @"SaveDir\Att\" + Event_Variable.QQQ + ".ini"))//如果属性文件存在
                    {
                        string att = LoadInfo(@"Att\" + Event_Variable.QQQ);
                        string oldZone = "";
                        string newZone = "";
                        foreach (string item in new List<string>(att.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries)))
                        {
                            if (item.StartsWith("牌堆:"))
                            {
                                oldZone = item.Substring(3);
                            }
                            if (item.StartsWith("手牌:"))
                            {
                                newZone = item.Substring(3);
                            }
                        }
                        input = oldZone + " " + newZone + " " + input;
                    }
                    else
                    {
                        return "请先使用'开始'指令绑定牌堆和手牌！";
                    }
                }
                List<string> list = new List<string>(input.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries)); //区域A 区域B int ...
                List<string> listA = new List<string>(LoadInfo(list[0]).Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries));
                List<string> listB = new List<string>(LoadInfo(list[1]).Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries));
                if (list.Count == 2)
                {
                    list.Add("1");
                }
                if (!IsNumeric(list[2]))
                {
                    list.Insert(2, "1");
                }
                if (Convert.ToInt32(list[2]) > listA.Count)
                {
                    return $@"{list[0]}剩余数量不足{list[2]}张！";
                }
                List<string> drawList = listA.GetRange(0, Convert.ToInt32(list[2]));
                listB.AddRange(drawList);
                listA.RemoveRange(0, Convert.ToInt32(list[2]));
                string aList = string.Join(" ", listA.ToArray());
                string bList = string.Join(" ", listB.ToArray());
                DelInfo(list[0]); CreateInfo(list[0]);
                WriteInfo(list[0], $@"{aList}");
                DelInfo(list[1]); CreateInfo(list[1]);
                WriteInfo(list[1], $@"{bList}");
                if (list[1].Contains("私密"))
                {
                    return $@"从{list[0]}抽了{list[2]}张牌！";
                }
                else
                {
                    List<string> toList = new List<string>(LoadInfo(list[1]).Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries));
                    List<string> toListFake = new List<string>();
                    string fakeName = LoadInfo(list[1]);//fakeName初始值为全称值
                    if (list[0].Contains("私密"))
                    {
                        foreach (var item in toList)//toList截去真名，传入toListFake
                        {
                            if (item.Contains("【"))
                            {
                                int startPos = item.IndexOf("【");
                                toListFake.Add(item.Substring(0, startPos));//截去真名
                            }
                            else
                            {
                                toListFake.Add(item);
                            }
                        }
                        fakeName = string.Join(" ", toListFake.ToArray());
                    }
                    return $@"{list[1]}:
{fakeName}";
                }
            }
            catch (Exception)
            {
                return "抽牌失败！";
            }
        }

        //清点
        public void CountNum(string input, out string num)
        {
            try
            {
                input = input.Substring(3).Trim();
                if (Event_Variable.Defa == true)
                {
                    if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + @"SaveDir\Att\" + Event_Variable.QQQ + ".ini"))//如果属性文件存在
                    {
                        string att = LoadInfo(@"Att\" + Event_Variable.QQQ);
                        string deck = "";
                        foreach (string item in new List<string>(att.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries)))
                        {
                            if (item.StartsWith("牌堆:"))
                            {
                                deck = item.Substring(3);
                            }
                        }
                        input = input + " " + deck;
                    }
                    else
                    {
                        num = "请先使用'开始'指令绑定牌堆";
                    }
                }
                string inputs = LoadInfo(input);
                if (inputs != "" || inputs != "读取错误！")
                {
                    List<string> list = new List<string>(inputs.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries));
                    num = $@"{list.Count - 1}";
                }
                else
                {
                    num = "清点失败！";
                }
            }
            catch (Exception)
            {
                num = "清点失败！";
            }
        }

        //插入
        public string Inser(string input)//.插入 [区域] [序号] [牌名]
        {
            try
            {
                input = input.Substring(3).Trim();
                List<string> list = new List<string>(input.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries));//[区域] [序号] [牌名]
                List<string> insList = list.GetRange(2, list.Count - 2);//插入内容
                List<string> zoneList = new List<string>(LoadInfo(list[0]).Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries));//待插入列表
                if (list[1] == "随机")
                {
                    int rd = new Random(Guid.NewGuid().GetHashCode()).Next(0, Convert.ToInt32(zoneList.Count));
                    zoneList.InsertRange(rd + 1, insList);
                }
                else
                {
                    zoneList.InsertRange(int.Parse(list[1]) - 1, insList);
                }
                string zList = string.Join(" ", zoneList.ToArray());
                DelInfo(list[0]); CreateInfo(list[0]);
                WriteInfo(list[0], $@"{zList}");
                return "插入成功！";
            }
            catch (Exception)
            {
                return "非法输入！";
            }
        }

        //检索
        public string Search(string input)//.检索 [区域] [关键字] [所有*]
        {
            try
            {
                input = input.Substring(3).Trim();
                List<string> list = new List<string>(input.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries));//[区域] [字段] [所有*]
                List<string> fromList = new List<string>(File.ReadAllText(
                    System.AppDomain.CurrentDomain.BaseDirectory + @"SaveDir\" + list[0] + ".ini"
                    ).Trim().Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries));//目标区域
                List<string> toList = new List<string>();//用来接收
                foreach (var item in fromList)//所有符合条件的卡牌都写入toList
                {
                    if (item.Contains(list[1]))
                    {
                        toList.Add(item);
                    }
                }
                if (list.Count >= 3)
                {
                    if (list[2] == "所有" || list[2] == "全部")
                    {
                        return string.Join(Environment.NewLine, toList.ToArray());//换行
                    }
                }
                return toList[new Random(Guid.NewGuid().GetHashCode()).Next(0, Convert.ToInt32(toList.Count - 1))];
            }
            catch (Exception)
            {
                return "非法输入！";
            }
        }

        //发现
        public string DisCover(string input)//.发现 [区域] [数量*]
        {
            try
            {
                input = input.Substring(3).Trim();
                List<string> list = new List<string>(input.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries));//[区域] [数量*]
                string inputs = LoadInfo(list[0]);
                int count;

                if (inputs != "" || inputs != "读取错误！")
                {
                    List<string> list1 = new List<string>(inputs.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries));//AAA BBB CC DDDD
                    List<string> list2 = new List<string>();

                    if (list.Count >= 2)//没有输入数量则默认为3
                    {
                        if (!IsNumeric(list[1]))
                        {
                            count = 3;
                        }
                        else
                        {
                            count = list1.Count;
                        }
                    }
                    else
                    {
                        count = 3;
                    }

                    if (list1.Count < count + 1)//如果数量不足或相等，直接发现所有目标
                    {
                        return string.Join(Environment.NewLine, list1.ToArray());
                    }

                    for (int i = 0; i < count; i++)
                    {
                        int rd = new Random(Guid.NewGuid().GetHashCode()).Next(0, Convert.ToInt32(list1.Count - 1));
                        list2.Add(list1[rd]);
                        list1.RemoveAt(rd);
                    }
                    return string.Join(Environment.NewLine, list2.ToArray());
                }
                else
                {
                    return "发现失败！";
                }
            }
            catch (Exception)
            {
                return "发现失败！";
            }
        }

        //移除
        public string RemovePos(string input)//.移除 [区域] [序号]
        {
            try
            {
                input = input.Substring(3).Trim();
                List<string> list = new List<string>(input.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries));//[区域] [数量*]
                string inputs = LoadInfo(list[0]);
                int count = int.Parse(list[1]);//序号
                if (inputs != "" || inputs != "读取错误！")
                {
                    List<string> list1 = new List<string>(inputs.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries));//AAA BBB CC DDDD
                    if (list1.Count < count || count < 1)
                    {
                        return "超出边界！";
                    }
                    else
                    {
                        if (list1[count - 1].Contains("【"))
                        {
                            string ret = $@"移除完成！
{list1[count - 1]}离开了区域！";
                            list1.RemoveAt(count - 1);
                            DelInfo(list[0]); CreateInfo(list[0]);
                            WriteInfo(list[0], string.Join(" ", list1.ToArray()));
                            return ret;
                        }
                        list1.RemoveAt(count - 1);
                        DelInfo(list[0]); CreateInfo(list[0]);
                        WriteInfo(list[0], string.Join(" ", list1.ToArray()));
                        return "移除完成！";
                    }
                }
                else
                {
                    return "区域为空或不存在！";
                }
            }
            catch (Exception)
            {
                return "移除失败！";
            }
        }

        //定义指令
        public string Command(string input)
        {
            try
            {
                List<string> tempInput = new List<string>(input.Substring(3).Trim().Split(new string[] { "#" }, StringSplitOptions.RemoveEmptyEntries));//[添加/删除] [新指令 A B C]#[原指令 A B]#[原指令 A C]
                for (int i = 1; i < tempInput.Count; i++)
                {
                    if (!tempInput[i].StartsWith(".") && !tempInput[i].StartsWith("。"))
                    {
                        tempInput[i] = "." + tempInput[i];
                    }
                }
                List<string> comInput = new List<string>(tempInput[0].Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries));//添加/删除  新指令  A  B  C
                List<string> comOuput = new List<string>(comInput.GetRange(1, comInput.Count - 1));//新指令  A  B  C
                if (!comOuput[0].StartsWith(".") && !comOuput[0].StartsWith("。") && !comOuput[0].StartsWith("!"))
                {
                    comOuput[0] = "." + comOuput[0];
                }
                switch (comInput[0])
                {
                    case "添加":
                        if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + @"SaveDir\Command\" + comOuput[0] + ".ini"))
                        {
                            return $@"已存在指令（{string.Join(" ", comOuput.ToArray())}）";
                        }
                        else
                        {
                            Event_Variable.SetCommandList();
                            foreach (var item in new List<string>(Event_Variable.CommandList.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries)))
                            {
                                if (item == comOuput[0])
                                {
                                    return "与固有指令冲突！";
                                }
                            }

                            //文件覆盖方式添加内容
                            StreamWriter file = new StreamWriter(AppDomain.CurrentDomain.BaseDirectory + @"SaveDir\Command\" + comOuput[0] + ".ini", false);
                            ////保存数据到文件
                            file.Write($@"{string.Join(" ", comOuput.GetRange(1, comOuput.Count - 1).ToArray())}#{string.Join("#", tempInput.GetRange(1, tempInput.Count - 1).ToArray())}");
                            //关闭文件
                            file.Close();
                            //释放对象
                            file.Dispose();
                            return $@"添加{string.Join(" ", comOuput.ToArray())}成功！";
                        }
                    case "删除":
                        if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + @"SaveDir\Command\" + comOuput[0] + ".ini"))
                        {
                            File.Delete(AppDomain.CurrentDomain.BaseDirectory + @"SaveDir\Command\" + comOuput[0] + ".ini");
                            return $@"自定义指令{comOuput[0]}已删除！";
                        }
                        else
                        {
                            return $@"找不到{comOuput[0]}！";
                        }

                    default:
                        return "格式错误！";
                }
            }
            catch (Exception)
            {
                return "无法确定自定义指令需要被添加还是删除。";
            }
        }

        //翻转
        public string Reverse(string input)
        {
            try
            {
                input = input.Substring(3).Trim();
                string inputs = LoadInfo(input);
                if (inputs != "" || inputs != "读取错误！")
                {
                    List<string> list = new List<string>(inputs.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries));
                    list.Reverse();
                    DelInfo(input); CreateInfo(input);
                    string notExistValue = string.Join(" ", list.ToArray());
                    WriteInfo(input, $@"{notExistValue}");
                    return "翻转完成！";
                }
                else
                {
                    return "翻转失败！";
                }
            }
            catch (Exception)
            {
                return "翻转失败！";
            }
        }

        //属性
        public string Attributes(string input)//.属性 [人物] [值名:数值] [值名:数值*]
        {
            try
            {
                List<string> tempInput = new List<string>(input.Substring(3).Trim().Replace("：", ":")
                    .Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries));//[人物] [值名: 数值] [值名: 数值*]
                if (Event_Variable.Defa == true)
                {
                    tempInput.Insert(0, $"{Event_Variable.QQQ}");
                }
                if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + @"SaveDir\Att\" + tempInput[0] + ".ini"))//如果属性文件存在
                {
                    string orige = File.ReadAllText(AppDomain.CurrentDomain.BaseDirectory + @"SaveDir\Att\" + tempInput[0] + ".ini");//[值名: 数值] [值名: 数值*] [值名: 数值*] [值名: 数值*]
                    List<string> origeKey = new List<string>();//[值名] [值名] [值名] [值名]
                    List<string> origeNum = new List<string>();//[数值] [数值*] [数值*] [数值*]
                    string tempItem; string tempNum; string tempKey; string tempDo;
                    foreach (string item in new List<string>(orige.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries)))//把key和num拆开
                    {
                        tempItem = item;
                        tempNum = tempItem.Substring(tempItem.IndexOf(":") + 1).Trim();//数值
                        tempKey = tempItem.Remove(tempItem.IndexOf(":")).Trim();//键名
                        origeKey.Add(tempKey);
                        origeNum.Add(tempNum);
                    }
                    foreach (var item in tempInput.GetRange(1, tempInput.Count - 1))
                    {
                        tempNum = $@"{GetStringLastNumber(item)}";//修改数值
                        tempKey = item.Replace($@"{GetStringLastNumber(item)}", "").Substring(0, item.Replace($@"{GetStringLastNumber(item)}", "").Length - 1);//修改键名
                        tempDo = item.Replace($@"{GetStringLastNumber(item)}", "").Substring(item.Replace($@"{GetStringLastNumber(item)}", "").Length - 1, 1)
                            .Replace("x", "*").Replace("÷", "/").Replace("×", "*");//操作符
                        for (int j = 0; j < origeKey.Count; j++)//遍历origeKey,搜索与修改键名相同的键名，如果都没有，添加这个键名与对应的键值。如果有，修改键值并置空操作符。
                        {
                            if (origeKey[j] == tempKey)
                            {
                                if (tempDo == ":")//如果是冒号，替换数值
                                {
                                    origeNum[j] = tempNum;
                                    tempDo = "";
                                }
                                if (tempDo == "+")
                                {
                                    origeNum[j] = $@"{ int.Parse(origeNum[j]) + int.Parse(tempNum) }";
                                    tempDo = "";
                                }
                                if (tempDo == "-")
                                {
                                    origeNum[j] = $@"{ int.Parse(origeNum[j]) - int.Parse(tempNum) }";
                                    tempDo = "";
                                }
                                if (tempDo == "*")
                                {
                                    origeNum[j] = $@"{ int.Parse(origeNum[j]) * int.Parse(tempNum) }";
                                    tempDo = "";
                                }
                                if (tempDo == "/")
                                {
                                    origeNum[j] = $@"{ int.Parse(origeNum[j]) / int.Parse(tempNum) }";
                                    tempDo = "";
                                }
                            }
                        }
                        if (tempDo != "")//如果操作符不为空，表示前面没有对键值进行操作=>找不到匹配的键名=>需要添加新项
                        {
                            origeKey.Add(tempKey);
                            origeNum.Add(tempNum);
                        }
                    }
                    List<string> list1 = new List<string>();
                    for (int i = 0; i < origeKey.Count; i++)
                    {
                        list1.Add($"{origeKey[i]}:{origeNum[i]}");//合并为list1
                    }
                    DelInfo(@"Att\" + tempInput[0]); CreateInfo(@"Att\" + tempInput[0]);
                    WriteInfo(@"Att\" + tempInput[0], string.Join(" ", list1.ToArray()));
                    return $@"{tempInput[0]}的属性为:{Environment.NewLine}{ string.Join(Environment.NewLine, list1.ToArray()) }";
                }
                else//没有旧信息的话就创建
                {
                    CreateInfo(@"Att\" + tempInput[0]);
                    WriteInfo(@"Att\" + tempInput[0], string.Join(" ", tempInput.GetRange(1, tempInput.Count - 1).ToArray()));
                    return $@"{tempInput[0]}的属性为:{Environment.NewLine}{ string.Join(Environment.NewLine, tempInput.GetRange(1, tempInput.Count - 1).ToArray()) }";
                }
            }
            catch (Exception)
            {
                return "非法输入！";
            }
        }

        //开始
        public string GameStart(string input, CqPrivateMessageEventArgs e)
        {
            try
            {
                input = input.Substring(3).Trim();
                if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + @"SaveDir\Att\" + e.FromQQ + ".ini"))//如果属性文件存在
                {
                    List<string> orige = new List<string>(File.ReadAllText(AppDomain.CurrentDomain.BaseDirectory + @"SaveDir\Att\" + e.FromQQ + ".ini")
                        .Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries));
                    List<string> reValues = new List<string>();//接收返回值
                    int i = -1;
                    List<string> newOrige = new List<string>(File.ReadAllText(AppDomain.CurrentDomain.BaseDirectory + @"SaveDir\Att\" + e.FromQQ + ".ini")
                        .Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries));
                    foreach (var item in newOrige)//匹配前缀替换
                    {
                        i++;
                        switch (item.Substring(0, 3))
                        {
                            case "牌堆:":
                                orige[i] = $@"牌堆:{input}";
                                reValues.Add($@"{item} → {input}");
                                break;
                            case "手牌:":
                                orige[i] = $@"手牌:{e.FromQQ}的{input}";
                                reValues.Add($@"{item} → {e.FromQQ}的{input}");
                                break;
                            case "桌面:":
                                orige[i] = $@"桌面:{input}的桌面";
                                reValues.Add($@"{item} → {input}的桌面");
                                break;
                            case "骰子:":
                                orige[i] = $@"骰子:20";
                                reValues.Add($@"{item} → 20");
                                break;
                            default:
                                break;
                        }
                    }
                    DelInfo(@"Att\" + e.FromQQ); CreateInfo(@"Att\" + e.FromQQ);
                    WriteInfo(@"Att\" + e.FromQQ, string.Join(" ", orige.Distinct().ToArray()));
                    return $@"{input}的属性修改:{Environment.NewLine}{string.Join(Environment.NewLine, reValues.ToArray()) }";

                }
                else//没有旧信息的话就创建
                {
                    CreateInfo(@"Att\" + e.FromQQ);
                    WriteInfo(@"Att\" + e.FromQQ, $@"牌堆:{input} 手牌:{e.FromQQ}的{input} 桌面:{input}的桌面 骰子:20");
                    CreateInfo(input); CreateInfo($@"{input}的桌面");
                    return $@"{input}的属性为
牌堆:{input}
手牌:{e.FromQQ}的{input}
桌面:{input}的桌面
骰子:20";

                }
            }
            catch (Exception ex)
            {
                return Event_CheckError.CheckError(ex);
            }

        }

        //导入
        public string CopyIn(string input)
        {
            try
            {
                //[区域]   [牌名],[数量]   [牌名],[数量]   [牌名],[数量] 
                List<string> tempInput = new List<string>(input.Substring(3).Trim().Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries));
                if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + @"SaveDir\" + tempInput[0] + ".ini"))
                {
                    string orige = string.Join(" ", tempInput.GetRange(1, tempInput.Count - 1).ToArray());//[值名,数值] [值名,数值] [值名,数值] [值名,数值]
                    List<string> origeKey = new List<string>();//[值名] [值名] [值名] [值名]
                    List<string> origeNum = new List<string>();//[数值] [数值] [数值] [数值]
                    string tempItem; string tempNum; string tempKey; string tempStr = "";
                    foreach (string item in new List<string>(orige.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries)))//把key和num拆开
                    {
                        tempItem = item;
                        tempNum = tempItem.Substring(tempItem.IndexOf(",") + 1).Trim();//数值
                        tempKey = tempItem.Remove(tempItem.IndexOf(",")).Trim();//键名
                        origeKey.Add(tempKey);
                        origeNum.Add(tempNum);
                    }
                    for (int i = 0; i < origeKey.Count; i++)
                    {
                        for (int j = 0; j < int.Parse(origeNum[i]); j++)
                        {
                            tempStr = tempStr + " " + origeKey[i];
                        }
                    }
                    WriteInfo(tempInput[0], tempStr);

                    return "导入完毕！";
                }
                else
                {
                    return "区域不存在！";
                }
            }
            catch (Exception)
            {
                return "非法输入！";
            }
        }

        //复制
        public string CopyTo(string input)
        {
            try
            {
                List<string> tempInput = new List<string>(input.Substring(3).Trim().Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries));//旧区域 新区域
                if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + @"SaveDir\" + tempInput[0] + ".ini") && !File.Exists(AppDomain.CurrentDomain.BaseDirectory + @"SaveDir\" + tempInput[1] + ".ini"))
                {
                    File.Copy(AppDomain.CurrentDomain.BaseDirectory + @"SaveDir\" + tempInput[0] + ".ini", AppDomain.CurrentDomain.BaseDirectory + @"SaveDir\" + tempInput[1] + ".ini");
                    return "复制完毕！";
                }
                else
                {
                    return $@"{tempInput[0]}不存在或{tempInput[1]}已存在！";
                }
            }
            catch (Exception)
            {
                return "非法输入！";
            }
        }

        //去重
        public string DelSame(string input)
        {
            try
            {
                List<string> tempInput = new List<string>(input.Substring(3).Trim().Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries));//区域
                List<string> tempOnput = new List<string>(LoadInfo(tempInput[0]).Trim().Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries));//AAA BBB CC AA VVV CC
                tempOnput = tempOnput.Distinct().ToList();
                DelInfo(tempInput[0]); CreateInfo(tempInput[0]);
                WriteInfo(tempInput[0], string.Join(" ", tempOnput.ToArray()));
                return "去重完毕！";
            }
            catch (Exception)
            {
                return "非法输入！";
            }
        }

        //转化
        public string Variety(string input)//.转化 [区域] [旧牌名] [新牌名] [所有*]
        {
            try
            {
                List<string> tempInput = new List<string>(input.Substring(3).Trim()
                    .Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries));//[区域] [旧牌名] [新牌名] [所有*]
                if (Event_Variable.Defa == true)
                {
                    if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + @"SaveDir\Att\" + Event_Variable.QQQ + ".ini"))//如果属性文件存在
                    {
                        string att = LoadInfo(@"Att\" + Event_Variable.QQQ);
                        string hand = "";
                        foreach (string item in new List<string>(att.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries)))
                        {
                            if (item.StartsWith("手牌:"))
                            {
                                hand = item.Substring(3);
                            }
                        }
                        tempInput.Insert(0, hand);
                    }
                    else
                    {
                        return "请先使用'开始'指令绑定手牌";
                    }
                }
                List<string> tempOutput = new List<string>(LoadInfo(tempInput[0]).Trim()
                    .Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries));//AAA BBB CC AA VVV CC
                List<string> endOutput = new List<string>();
                tempInput.Add(" ");
                if (tempInput[3] == "所有" || tempInput[3] == "全部")
                {
                    foreach (var item in tempOutput)
                    {
                        if (item == tempInput[1])
                        {
                            endOutput.Add(tempInput[2]);
                        }
                        else
                        {
                            endOutput.Add(item);
                        }
                    }
                }
                else
                {
                    int find = tempOutput.FindIndex((string f) => f.Equals(tempInput[1]));
                    tempOutput.RemoveAt(find);
                    tempOutput.Insert(find, tempInput[2]);
                    endOutput = tempOutput;
                }
                DelInfo(tempInput[0]); CreateInfo(tempInput[0]);
                WriteInfo(tempInput[0], string.Join(" ", endOutput.ToArray()));
                return string.Join(" ", endOutput.ToArray());
            }
            catch (Exception)
            {
                return "非法输入！";
            }
        }

        //变量
        public string Variable(string input)//.变量 [变量名] [表达式/区域/删除]
        {
            try
            {
                List<string> tempInput = new List<string>(input.Substring(3).Trim().Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries));//[变量名] [表达式/【区域】/删除]
                int indexer = Event_Variable.vKey.FindIndex((string f) => f == @"[" + tempInput[0] + @"]");//获取索引
                if (tempInput[1] == "删除")
                {
                    if (indexer == -1)
                    {
                        return $"变量[{tempInput[0]}]不存在！";
                    }
                    else
                    {
                        Event_Variable.vKey.RemoveAt(indexer);
                        Event_Variable.vValue.RemoveAt(indexer);
                        return $"变量[{tempInput[0]}]删除成功！";
                    }
                }
                if (tempInput[1].Substring(0, 1) == "【" && tempInput[1].Substring(tempInput[1].Length - 1, 1) == "】")//是区域
                {
                    if (indexer == -1)//变量不存在，添加新项
                    {
                        Event_Variable.vKey.Add(@"[" + tempInput[0] + @"]");
                        string vv = LoadInfo(tempInput[1].Replace("【", "").Replace("】", ""));
                        Event_Variable.vValue.Add(vv);
                        return $"添加变量[{tempInput[0]}] = {vv}";
                    }
                    else//变量存在，修改键值
                    {
                        string vv = LoadInfo(tempInput[1].Replace("【", "").Replace("】", ""));
                        Event_Variable.vValue[indexer] = vv;
                        return $"修改变量[{tempInput[0]}] = {vv}";
                    }
                }
                //如果都不是，那就是字符串
                string strInput = tempInput[1].Replace("骰子", $"{Event_Variable.Number}").Replace("清点", $"{Event_Variable.CountValue}");
                if (indexer == -1)//变量不存在，添加新项
                {
                    Event_Variable.vKey.Add(@"[" + tempInput[0] + @"]");
                    Event_Variable.vValue.Add(strInput);
                    return $"添加变量[{tempInput[0]}] = {strInput}";
                }
                else
                {
                    Event_Variable.vValue[indexer] = strInput;
                    return $"修改变量[{tempInput[0]}] = {strInput}";
                }
            }
            catch (Exception)
            {
                return "非法输入！";
            }
        }

        public void Dicex(string input)
        {
            try
            {

            }
            catch (Exception)
            {

            }
        }

        //仓库区_______________________________________________________________________________________________________________________________________________________________________

        /// <summary>
        /// 判断纯数
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public bool IsNumeric(string str)
        {
            if (str == null || str.Length == 0)         //验证这个参数是否为空
                return false;                           //是，就返回False
            ASCIIEncoding ascii = new ASCIIEncoding();  //new ASCIIEncoding 的实例
            byte[] bytestr = ascii.GetBytes(str);       //把string类型的参数保存到数组里

            foreach (byte c in bytestr)                 //遍历这个数组里的内容
            {
                if (c < 48 || c > 57)                   //判断是否为数字
                {
                    return false;                       //不是，就返回False
                }
            }
            return true;                                //是，就返回True
        }

        /// <summary>
        /// 创建信息
        /// </summary>
        /// <param name="name">文件名</param>
        /// <returns></returns>
        public static int CreateInfo(string name)
        {
            //按文件名
            string FileName = name + ".ini";
            //设置目录
            string CurDir = AppDomain.CurrentDomain.BaseDirectory + @"SaveDir\";
            //判断路径是否存在
            if (!Directory.Exists(CurDir))
            {
                Directory.CreateDirectory(CurDir);
            }
            //不存在就创建
            String FilePath = CurDir + FileName;
            //文件已存在
            if (File.Exists(FilePath))
            {
                return 2;
            }
            else
            {

                //文件覆盖方式添加内容
                StreamWriter file = new StreamWriter(FilePath, false);
                ////保存数据到文件
                //file.Write("");
                //关闭文件
                file.Close();
                //释放对象
                file.Dispose();
                if (File.Exists(FilePath))
                {
                    return 0;
                }
                return 1;
            }
        }

        /// <summary>
        /// 销毁信息
        /// </summary>
        /// <param name="name"></param>
        public static int DelInfo(string name)
        {
            name = AppDomain.CurrentDomain.BaseDirectory + @"SaveDir\" + name + ".ini";
            try
            {
                if (File.Exists(name))
                {
                    File.Delete(name);
                    if (!File.Exists(name))
                    {
                        return 0;
                    }
                    else
                    {
                        return 1;
                    }
                }
                else
                {
                    return 2;
                }
            }
            catch
            {
                return 1;
            }
        }

        /// <summary>
        /// 续行写入
        /// </summary>
        /// <param name="name">文件名</param>
        /// <param name="str">字符串</param>
        /// <returns></returns>
        public static bool WriteInfo(string name, string str)
        {
            using (StreamWriter sw = File.AppendText(System.AppDomain.CurrentDomain.BaseDirectory + @"SaveDir\" + name + ".ini"))
            {
                str = new Regex("[\\s]+").Replace(str, " ");
                sw.Write($" {str}");
                return true;
            }
        }

        /// <summary>
        /// 读取信息
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static string LoadInfo(string name)
        {
            name = AppDomain.CurrentDomain.BaseDirectory + @"SaveDir\" + name + ".ini";
            try
            {
                if (File.Exists(name))
                {
                    string load = File.ReadAllText(name).Trim();
                    return load;
                }
                else
                {
                    return "";
                }
            }
            catch
            {
                return "读取错误！";
            }
        }

        /// <summary>
        /// 获取尾数
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public int GetStringLastNumber(string str)
        {
            int result = 0;
            if (str != null && str != string.Empty)
            {
                Match match = Regex.Match(str, @"(^.+?)(\d+$)");
                if (match.Success)
                {
                    result = (int)decimal.Parse(match.Groups[2].Value);
                }
            }
            return result;
        }

        /// <summary>
        /// 删除文件夹strDir中nDays天以前的文件
        /// </summary>
        /// <param name="dir"></param>
        /// <param name="days"></param>
        void DeleteOldFiles(string dir, int days)
        {
            try
            {
                if (!Directory.Exists(dir))
                {
                    Directory.CreateDirectory(dir);
                }
                if (!Directory.Exists(dir) || days < 1) return;

                var now = DateTime.Now;
                foreach (var f in Directory.GetFileSystemEntries(dir).Where(f => File.Exists(f)))
                {
                    var t = File.GetCreationTime(f);

                    var elapsedTicks = now.Ticks - t.Ticks;
                    var elapsedSpan = new TimeSpan(elapsedTicks);

                    if (elapsedSpan.TotalDays > days) File.Delete(f);
                }
            }
            catch (Exception)
            {
                // ignored
            }
        }
    }
}
