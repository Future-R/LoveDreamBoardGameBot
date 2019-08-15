using Native.Csharp.App.EventArgs;
using Native.Csharp.App.Interface;
using Native.Csharp.Sdk.Cqp;
using Native.Csharp.Sdk.Cqp.Model;
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
    /// 群聊回复
    /// CqPrivateMessageEventArgs→CqGroupMessageEventArgs
    /// SendPrivateMessage→ReceiveGroupMessage
    /// e.FromQQ→e.FromGroup
    /// </summary>
    public class Event_ReceiveGroupMessage : IReceiveGroupMessage
    {
        //静态全局变量与函数
        public static int Number
        {
            get; set;
        }
        public static int CountValue
        {
            get; set;
        }
        public static List<long> botCloseList = new List<long>();
        public static float Values;
        private static long groupId;
        private static long qqId;

        public static float GetValue()
        {
            return Values;
        }

        public static void SetNumber(int number)
        {
            Number = number;
        }
        //固有指令合集
        string commandList = ".计算 .骰子 .创建 .清空 .销毁 .添加 .删除 .移动 .插入 .移除 .抽牌 .查看 .洗牌 .清点 .检索 .发现 .翻转 .导入 .属性 .定义 .报错 .去重 .复制 .棋盘 .日志 .转化 .如果 .清理 .开启 .关闭 .退群 .变量";
        //变量的键名和键值
        public static List<string> vKey = new List<string>();
        public static List<string> vValue = new List<string>();

        public void ReceiveGroupMessage(object sender, CqGroupMessageEventArgs e)
        {
            string input = e.Message;
            input = new Regex("[\\s]+").Replace(input, " ");//合并复数空格
            input = input.Trim().Replace("色子","骰子");//去除前后空格，统一一些措辞
            try
            {
                if (input.Substring(0, 1) != "." && (input.Substring(0, 1) != "。" ))//没有扳机就不触发
                {
                    return;
                }
                //把用户输入的第一个中文句号替换为英文
                if (input.Substring(0, 1) == "。")
                {
                    input = "." + input.Remove(0, 1);
                }
            }
            catch (Exception)
            {
                Common.CqApi.SendGroupMessage(e.FromGroup, "天杀的错误！");
            }
            if (input.Length < 3)//没有3字就不触发
            {
                return;
            }
            if (input.Substring(1,1) == ".")//第二个字符也是扳机可能是误触
            {
                return;
            }

            if (input == ".帮助")
            {
                Common.CqApi.SendGroupMessage(e.FromGroup, @"恋梦桌游姬V1.1.0 By未来菌
方括号内为参数，带*的为选填参数：

.计算 [算式]：进行四则运算

.骰子 [数量*] [面数]：投掷X枚骰子，默认为1

.创建 [区域]：创建1个可以放置卡牌或记录信息的区域

.销毁 [区域]：销毁区域，在区域名开头加入'Att\'可以销毁人物属性。

.清空 [区域]：清空区域

.添加 [区域] [牌名] [牌名*]：添加若干指定名称的牌进入区域，卡牌间用空格表示并列

.删除 [区域] [牌名] [牌名*]：删除区域中搜索到的最前方的指定卡牌

.插入 [区域] [序号] [牌名] [牌名*]：在区域的第X张牌之前插入若干张牌，序号可填'随机'

.移除 [区域] [序号]：移除区域的第X张牌

.抽牌 [旧区域] [新区域] [数量*]：从旧区域抽X张牌到新区域，默认为1

.移动 [旧区域] [新区域] [牌名] [牌名*]：从旧区域移动指定卡牌到新区域

.查看 [区域]：打印区域内容

.洗牌 [区域]：区域卡牌乱序排序

.清点 [区域]：计算区域卡牌数量

.检索 [区域] [字段] [所有*]：查找随机1张或所有包含关键字的卡牌

.发现 [区域] [数量*]：展示区域内随机X张牌，默认为3

.翻转 [区域]：牌序反转

.去重 [区域]：去除区域重复元素

.转化 [区域] [旧牌名] [新牌名] [所有*]：将区域第1张或所有对应卡牌转化为指定卡牌。

.复制 [旧区域] [新区域]：创建新区域并复制旧区域的元素

.属性 [目标] [值名:数值] [值名:数值*]：设置人物的属性，如'战士 HP:15'，冒号可替换为四则运算符来进行数值的修改

.导入 [区域] [文本]：将CSV表转化为文本并添加到已存在的区域,行格式'[牌名],[数量]'

.棋盘 [房间号]：打开对应房间号的棋盘，房间号约束在0~9

.变量 [变量名] [表达式/【区域】/删除]：设定一个变量，内容为表达式结果，或是某区域的内容，或是删除这个变量。在任意指令中'[变量名]'将会替换为变量的内容

.定义 [添加/删除] [新指令 甲 乙 丙]#[指令 甲 乙]#[指令 甲 丙]：自定义1个新指令，新指令会执行后面每条指令

.如果 [表达式] [>/</=/!] [数值]?[指令 甲 乙]?[指令 甲 丙*]：如果表达式的结果大于/小于/等于/不等于指定值，执行后面每条指令

.清理 [天数]：[群管]清理X天前创建的所有数据

.退群 [QQ号*]：[群管]将机器人踢出群可能导致封号，请使用这个指令使机器人退群。输入QQ号使对应的1个机器人退群

.开启/关闭 [QQ号*]：将机器人禁言可能导致账号数据异常，请使用这个指令开关机器人。输入QQ号开关群内对应的1个机器人。关闭期间不会处理其它指令

注：可以在区域名开头加入'私密'，如'私密牌库'。非私密区域在某些卡牌变化场景会打印内容。
牌名可以写成'假名【真名】'的形式，伪装的牌在离开非私密区域时会显露原形。
表达式格式：'骰子+清点*2'，当前支持的环境变量：骰子、清点");
                return;
            }
            
            try
            {
                if (input.Substring(0,3) == ".开启")
                {
                    if (input.Length > 8)
                    {
                        if (input.Substring(3).Trim() == Convert.ToString(Common.CqApi.GetLoginQQ()))
                        {
                            botCloseList.Remove(e.FromGroup);
                            Common.CqApi.SendGroupMessage(e.FromGroup, $"{Convert.ToString(Common.CqApi.GetLoginQQ())}已开启！");
                        }
                        else
                        {
                            return;
                        }
                    }
                    else
                    {
                        botCloseList.Remove(e.FromGroup);
                        Common.CqApi.SendGroupMessage(e.FromGroup, "已开启！");
                    }
                }
                if (input.Substring(0, 3) == ".关闭")
                {
                    if (input.Length > 8)
                    {
                        if (input.Substring(3).Trim() == Convert.ToString(Common.CqApi.GetLoginQQ()))
                        {
                            botCloseList.Add(e.FromGroup);
                            botCloseList = botCloseList.Distinct().ToList();
                            Common.CqApi.SendGroupMessage(e.FromGroup, $"{Convert.ToString(Common.CqApi.GetLoginQQ())}已关闭！");
                        }
                        else
                        {
                            return;
                        }
                    }
                    else
                    {
                        botCloseList.Add(e.FromGroup);
                        botCloseList = botCloseList.Distinct().ToList();
                        Common.CqApi.SendGroupMessage(e.FromGroup, "已关闭！");
                    }
                }
            }
            catch (Exception)
            {
                Common.CqApi.SendGroupMessage(e.FromGroup, "意外的错误！");
            }
            

            //用户输入指令
            if (input.Length > 2 && !botCloseList.Exists((long f) => f == e.FromGroup))
            {
                int vvc = 0;
                foreach (var item in vValue)
                {
                    input = input.Replace(vKey[vvc], item);
                    vvc++;
                }
                groupId = e.FromGroup;
                qqId = e.FromQQ;
                GroupMember member = Common.CqApi.GetMemberInfo(groupId, qqId, true);//获取群成员
                string pt = Convert.ToString(member.PermitType);//获取权限：Holder群主，Manage管理，None群员

                //判断路径是否存在
                if (!Directory.Exists(AppDomain.CurrentDomain.BaseDirectory + @"SaveDir\Command\"))
                {
                    Directory.CreateDirectory(AppDomain.CurrentDomain.BaseDirectory + @"SaveDir\Command\");//不存在就创建
                    Directory.CreateDirectory(AppDomain.CurrentDomain.BaseDirectory + @"SaveDir\Att\");//属性功能初始化
                }

                try
                {
                    List<string> commandInput = new List<string>(input.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries));//切割用户输入的指令 .xx a b c
                    if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + @"SaveDir\Command\" + commandInput[0] + ".ini"))//如果找到用户定义的指令
                    {
                        var tempLoad = File.ReadAllText(AppDomain.CurrentDomain.BaseDirectory + @"SaveDir\Command\" + commandInput[0] + ".ini").Trim();
                        if (tempLoad.Substring(0,1) == "#")
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
                        foreach (var item in loadEndList)
                        {
                            
                            CommandIn(item, e, pt);

                        }

                    }
                    else
                    {
                        CommandIn(input, e, pt);//查一下是不是固有指令
                    }
                }
                catch (Exception)
                {
                    Common.CqApi.SendGroupMessage(e.FromGroup, "出现异常！");
                }
            }
        }

        //模块区_______________________________________________________________________________________________________________________________________________________________________

        //固有指令
        public void CommandIn(string input, CqGroupMessageEventArgs e,string pt)
        {
            if (input.Length < 2)//降低错误触发
            {
                return;
            }
            switch (input.Substring(1, 2))//偷懒，只匹配.后2个字符
            {

                case "计算":
                    Common.CqApi.SendGroupMessage(e.FromGroup, Calc(input));
                    return;

                case "骰子":
                    Common.CqApi.SendGroupMessage(e.FromGroup, Dices(input));
                    return;

                case "创建":
                    switch (Crea(input))
                    {
                        case 0:
                            Common.CqApi.SendGroupMessage(e.FromGroup, $@"创建{input.Trim().Substring(3).Trim()}成功！");//待优化
                            return;

                        default:
                            Common.CqApi.SendGroupMessage(e.FromGroup, $@"创建{input.Trim().Substring(3).Trim()}失败！");
                            return;
                    }

                case "销毁":
                    Common.CqApi.SendGroupMessage(e.FromGroup, Boom(input, out string leave1) + Environment.NewLine + leave1);
                    return;

                case "清空":
                    Boom(input, out string leave2);
                    if (leave2.Length > 1)
                    {
                        Common.CqApi.SendGroupMessage(e.FromGroup, leave2);
                    }
                    switch (Crea(input))//创建
                    {
                        case 0:
                            Common.CqApi.SendGroupMessage(e.FromGroup, $@"清空{input.Trim().Substring(3).Trim()}成功！");
                            return;

                        default:
                            Common.CqApi.SendGroupMessage(e.FromGroup, $@"{input.Trim().Substring(3).Trim()}丢失！");
                            return;
                    }


                case "添加":
                    Common.CqApi.SendGroupMessage(e.FromGroup, Add(input));
                    return;

                case "删除":
                    Common.CqApi.SendGroupMessage(e.FromGroup, $@"{DelInfos(input, out string trueNameList)}");
                    if (trueNameList.Length >= 1) Common.CqApi.SendGroupMessage(e.FromGroup, $@"{trueNameList.Trim()}离开了区域！");
                    return;

                case "移动":
                    Common.CqApi.SendGroupMessage(e.FromGroup, Move(input, out string fakeName));
                    if (fakeName.Length >= 1) Common.CqApi.SendGroupMessage(e.FromGroup, $@"{fakeName.Trim()}离开了区域！");
                    return;

                case "抽牌":
                    Common.CqApi.SendGroupMessage(e.FromGroup, Draw(input));
                    return;

                case "查看":
                    GetInfo(input, out string looname, out string looret, out string loofak);
                    Common.CqApi.SendGroupMessage(e.FromGroup, $@"{looname}:
{loofak}");
                    return;

                case "清点":
                    CountNum(input, out string num);
                    CountValue = int.Parse(num);
                    Common.CqApi.SendGroupMessage(e.FromGroup, $@"{input.Substring(3).Trim()}有{num}张牌");
                    return;

                case "检索":
                    Common.CqApi.SendGroupMessage(e.FromGroup, Search(input));
                    return;

                case "移除":
                    Common.CqApi.SendGroupMessage(e.FromGroup, RemovePos(input));
                    return;

                case "导入":
                    Common.CqApi.SendGroupMessage(e.FromGroup, CopyIn(input));
                    return;

                case "发现":
                    Common.CqApi.SendGroupMessage(e.FromGroup, DisCover(input));
                    return;

                //case "存档":
                //    return;

                //case "读档":
                //    return;

                case "复制":
                    Common.CqApi.SendGroupMessage(e.FromGroup, CopyTo(input));
                    return;

                case "洗牌":
                    Common.CqApi.SendGroupMessage(e.FromGroup, Shuffle(input));
                    return;

                case "插入":
                    Common.CqApi.SendGroupMessage(e.FromGroup, Inser(input));
                    return;

                case "翻转":
                    Common.CqApi.SendGroupMessage(e.FromGroup, Reverse(input));
                    return;

                //case "建表":
                //    var db = (IRepository)Common.UnityContainer.Resolve(typeof(IRepository));
                //    return;

                case "定义":
                    Common.CqApi.SendGroupMessage(e.FromGroup, Command(input));
                    return;

                //case "报错":
                //    Common.CqApi.SendGroupMessage(1045740922, Attributes(input));
                //    Common.CqApi.SendGroupMessage(e.FromGroup, "感谢您的反馈！这将帮助改进桌游姬！");
                //    return;

                case "属性":
                    Common.CqApi.SendGroupMessage(e.FromGroup, Attributes(input));
                    return;

                case "去重":
                    Common.CqApi.SendGroupMessage(e.FromGroup, DelSame(input));
                    return;

                case "转化":
                    Common.CqApi.SendGroupMessage(e.FromGroup, Variety(input));
                    return;

                case "变量":
                    Common.CqApi.SendGroupMessage(e.FromGroup, Variable(input));
                    return;

                case "退群":

                    if (pt == "None")//屁民瞎发啥指令
                    {
                        Common.CqApi.SendGroupMessage(e.FromGroup, "权限不足！");
                        return;
                    }
                    if (input.Length > 8)//有参数
                    {
                        if (input.Substring(3).Trim() == Convert.ToString(Common.CqApi.GetLoginQQ()))//膝盖中了一箭
                        {
                            Common.CqApi.SendGroupMessage(e.FromGroup, "感谢你的支持，再见！");
                            Common.CqApi.SetGroupExit(e.FromGroup, false);
                            return;
                        }
                        return;//踢的不是我，溜了溜了
                    }
                    else//没参数，意思全退呗
                    {
                        Common.CqApi.SendGroupMessage(e.FromGroup, "感谢你的支持，再见！");
                        Common.CqApi.SetGroupExit(e.FromGroup, false);
                        return;
                    }

                case "清理":
                    if (pt == "None")
                    {
                        Common.CqApi.SendGroupMessage(e.FromGroup, "权限不足！");
                        return;
                    }
                    try
                    {
                        DeleteOldFiles(System.AppDomain.CurrentDomain.BaseDirectory + @"SaveDir\", int.Parse(input.Substring(3).Trim()));
                        DeleteOldFiles(System.AppDomain.CurrentDomain.BaseDirectory + @"SaveDir\Att\", int.Parse(input.Substring(3).Trim()));
                        DeleteOldFiles(System.AppDomain.CurrentDomain.BaseDirectory + @"SaveDir\Command\", int.Parse(input.Substring(3).Trim()));
                    }
                    catch (Exception)
                    {
                        Common.CqApi.SendGroupMessage(e.FromGroup, "抛出错误！");
                        return;
                    }
                    Common.CqApi.SendGroupMessage(e.FromGroup, "清理完成！");
                    return;

                case "棋盘":
                    if (IsNumeric(input.Substring(3).Trim()))
                    {
                        string[] tables = { "GHhvRqDCJ9JJdk3R/" , "Vw8vwjRDrH9YWwTG/", "jJ3qyHpyrxtqGwWw/",
                        "dDwgDJg9g99KT63h/" , "GQcHhVWd9DkY6KTR/" , "gdKYVKkCg8qKVjGk/" , "K3QyPhxY6rhrQJtx/" ,
                        "HG8wPtjdwkyCGdrT/" , "t3qk6T3dyyxDddkG/" , "kYKCw8jcWCVkJgRj/" };
                        Common.CqApi.SendGroupMessage(e.FromGroup, $@"https://shimo.im/sheets/" + $@"{tables[int.Parse(input.Substring(3).Trim())]} 可复制链接后用石墨文档 App 或小程序打开");
                    }
                    else
                    {
                        Common.CqApi.SendGroupMessage(e.FromGroup, "房间号有误！");
                    }
                    
                    return;

                case "日志":
                    Common.CqApi.SendGroupMessage(e.FromGroup, @"更新日志：
Ver1.1.0：现在开放群私聊操作；新增变量指令；新增开关指令；新增退群指令；如果指令现在支持表达式；清理与退群指令现在需要群管操作。
Ver1.0.3：新增如果指令；新增清理指令；现在计算指令支持mod、e、π；现在能自定义无参指令；指令集中的指令忘记加点现在会自动补全；检索结果现在会换行显示；修复单枚骰子的潜在BUG。
Ver1.0.2：删除报错指令；新增转化指令；降低误触几率。
Ver1.0.1：修复骰子过多触发的BUG；去除一个不必要的提示；增加查看更新日志的功能。");
                    return;

                case "如果"://.如果 [表达式] [>/</=/!] [指定值]?[指令 A B]
                    input = input.Substring(3).Trim().Replace("大于", ">").Replace("小于", "<").Replace("等于", "=").Replace("？", "?").Replace("！", "!").Replace("不等于" ,"!");//[表达式] [>/</=/!] [指定值]?[指令 A B]
                    
                    try
                    {
                        List<string> sharpInput = new List<string>(input.Split(new string[] { "?" }, StringSplitOptions.RemoveEmptyEntries));
                        List<string> expreesInput = new List<string>(sharpInput[0].Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries));//[表达式] [>/</=/!] [指定值]

                        string expression = expreesInput[0].Replace("骰子", $"{Number}").Replace("清点", $"{CountValue}");
                        string oper = expreesInput[1];
                        string value = expreesInput[2];
                       
                        sharpInput.RemoveAt(0);
                        for (int i = 0; i < sharpInput.Count; i++)
                        {
                            if (sharpInput[i].Substring(0,1) != "." && sharpInput[i].Substring(0, 1) != "。")
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

                                        CommandIn(item, e, pt);

                                    }
                                }
                                break;

                            case "<":
                                if ((int)new DataTable().Compute(expression, "") < int.Parse(value))
                                {
                                    foreach (var item in sharpInput)
                                    {

                                        CommandIn(item, e, pt);

                                    }
                                }
                                break;

                            case "=":
                                if ((int)new DataTable().Compute(expression, "") == int.Parse(value))
                                {
                                    foreach (var item in sharpInput)
                                    {

                                        CommandIn(item, e, pt);

                                    }
                                }
                                break;

                            case "!":
                                if ((int)new DataTable().Compute(expression, "") != int.Parse(value))
                                {
                                    foreach (var item in sharpInput)
                                    {

                                        CommandIn(item, e, pt);

                                    }
                                }
                                break;

                            default:
                                Common.CqApi.SendGroupMessage(e.FromGroup, "找不到比较符！");
                                break;
                        }
                    }
                    catch (Exception)
                    {
                        Common.CqApi.SendGroupMessage(e.FromGroup, "抛出异常！");
                    }
                    return;
            }

            //Common.CqApi.SendGroupMessage(e.FromGroup, "不存在的指令！获取指令目录请输入“.帮助”！");
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
                            Number = (int)new DataTable().Compute(results, "");
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
                            Number = result;
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
                        Number = result;
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
        public string Boom(string input,out string leave)
        {
            GetInfo(input,out string name,out string reture,out string refake);
            List<string> tempInput = new List<string>(reture.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries));
            List<string> fromListFake = new List<string>();
            foreach (var item in tempInput)//tempInput截去真名，传入fromListFake
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
            leave = String.Join(" ", fromListFake.ToArray()) + "离开了区域！";
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
            int spsNum = input.IndexOf(" ");
            try
            {
                //如果文件存在的话……我会抽时间优化一下这块代码
                if (File.Exists(System.AppDomain.CurrentDomain.BaseDirectory + @"SaveDir\" + input.Substring(0, spsNum) + ".ini"))
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
        public void GetInfo(string input, out string name, out string reture,out string refake)
        {
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
            refake = String.Join(" ", list2.ToArray());
        }

        //删除
        public string DelInfos(string input,out string trueName)//.删除 区域 AA BB CC
        {
            trueName = "";
            GetInfo(input, out string remname, out string truRet,out string fakRet);//获取区域内容
            List<string> tempInput = new List<string>(input.Substring(3).Trim().Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries));//区域 AA BB CC
            try
            {
                if (File.Exists(System.AppDomain.CurrentDomain.BaseDirectory + @"SaveDir\" + tempInput[0] + ".ini"))
                {
                    List<string> delList = new List<string>(tempInput.GetRange(1,tempInput.Count - 1));//肃反名单
                    GetInfo($@".查看 {tempInput[0]}",out string fromName,out string fromTList,out string fromFList);
                    List<string> collection = new List<string>(fromFList.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries));//假名列表（AA BB CC DD）
                    List<string> collection2 = new List<string>(fromTList.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries));//真名列表（AA BB CC DD）
                    int find;
                    for (int i = 0; i < delList.Count; i++)
                    {
                        find = collection.FindIndex((string f) => f.Equals(delList[i]));
                        if (find == -1)
                        {
                            trueName = "";
                            return $@"无法找到{String.Join(" ", delList.ToArray())}";
                        } 
                        if (collection2[find + i].Contains("【"))
                        {
                            trueName += $@" {collection2[find + i]}";
                        } 
                        collection.Remove(delList[i]);
                    }
                    DelInfo(tempInput[0]); CreateInfo(tempInput[0]);
                    WriteInfo(tempInput[0], $@"{String.Join(" ", collection.ToArray())}");
                    return $@"{tempInput[0]}:
{String.Join(" ", collection.ToArray())}";
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
                    string notExistValue = String.Join(" ", list2.ToArray());
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

        //移动
        public string Move(string input,out string trueName)//.移动 区域A 区域B XXX YYY ZZZ
        {
            string fakeName = "";
            trueName = "";
            try
            {
                input = input.Substring(3).Trim();
                List<string> list = new List<string>(input.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries));//区域A 区域B XXX YYY ZZZ
                string name1 = list[0];
                string name2 = list[1];
                List<string> nameList = new List<string>();
                for (int i = 0; i < list.Count - 2; i++)
                {
                    nameList.Add(list[i + 2]);//需要移动的对象
                }
                List<string> fromList = new List<string>(
                    File.ReadAllText(System.AppDomain.CurrentDomain.BaseDirectory + @"SaveDir\" + name1 + ".ini")
                    .Trim().Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries));//来源区域全称
                List<string> toList = new List<string>(
                    File.ReadAllText(System.AppDomain.CurrentDomain.BaseDirectory + @"SaveDir\" + name2 + ".ini")
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
                    fakeName = String.Join(" ", fromListFake.ToArray());

                    int find;
                    for (int i = 0; i < nameList.Count; i++)
                    {
                        find = fromListFake.FindIndex((string f) => f.Equals(nameList[i]));
                        if (find == -1)
                        {
                            trueName = "";
                            return $@"无法找到{String.Join(" ", nameList.ToArray())}";
                        }
                        if (fromList[find + i].Contains("【"))
                        {
                            trueName += $@" {fromList[find + i]}";
                        }
                        toList.Add(fromList[find + i]);
                        fromList.Remove(fromList[find + i]);
                    }

                    foreach (var item in nameList)
                    {
                        int strNum = fromListFake.IndexOf(item);//找到第一个对象的位置
                        fromListFake.RemoveAt(strNum);
                    }
                    DelInfo(name1); CreateInfo(name1);
                    WriteInfo(name1, $@" {String.Join(" ", fromList.ToArray())}");//写入来源区域
                    DelInfo(name2); CreateInfo(name2);
                    WriteInfo(name2, $@" {String.Join(" ", toList.ToArray())}");//写入目标区域
                    if (name2.Contains("私密"))
                    {
                        return "移动成功！";
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
{String.Join(" ", toListFake.ToArray())}";//打印更改后的内容
                    }

                }
                catch (Exception)
                {
                    trueName = "";
                    return "移动失败！";
                }

            }
            catch (Exception)
            {
                trueName = "";
                return "移动失败！";
            }
        }

        //抽牌
        public string Draw(string input)//.抽牌 旧区域 新区域 数量
        {
            try
            {
                input = input.Substring(3).Trim();
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
        public void CountNum(string input,out string num)
        {
            try
            {
                input = input.Substring(3).Trim();
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
                List<string> insList = list.GetRange(2,list.Count - 2);//插入内容
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
                    if (list[2] == "所有")
                    {
                        return String.Join(Environment.NewLine, toList.ToArray());//换行
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

                    if (list1.Count < count + 1)
                    {
                        return String.Join(" ", list1.ToArray());
                    }

                    for (int i = 0; i < count; i++)
                    {
                        int rd = new Random(Guid.NewGuid().GetHashCode()).Next(0, Convert.ToInt32(list1.Count - 1));
                        list2.Add(list1[rd]);
                        list1.RemoveAt(rd);
                    }
                    return String.Join(" ", list2.ToArray());
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
                            WriteInfo(list[0], String.Join(" ", list1.ToArray()));
                            return ret;
                        }
                        list1.RemoveAt(count - 1);
                        DelInfo(list[0]); CreateInfo(list[0]);
                        WriteInfo(list[0], String.Join(" ", list1.ToArray()));
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
                    if (tempInput[i].Substring(0, 1) != "." && tempInput[i].Substring(0, 1) != "。")
                    {
                        tempInput[i] = "." + tempInput[i];
                    }
                }
                List<string> comInput = new List<string>(tempInput[0].Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries));//添加/删除  新指令  A  B  C
                List<string> comOuput = new List<string>(comInput.GetRange(1,comInput.Count - 1));//新指令  A  B  C
                if (comOuput[0].Substring(0, 1) != "." && comOuput[0].Substring(0, 1) != "。")
                {
                    comOuput[0] = "." + comOuput[0];
                }
                switch (comInput[0])
                {
                    case "添加":
                        if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + @"SaveDir\Command\" + comOuput[0] + ".ini"))
                        {
                            return $@"已存在指令（{string.Join( " " , comOuput.ToArray() )}）";
                        }
                        else
                        {
                            foreach (var item in new List<string>(commandList.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries)))
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
                return "抛出异常！";
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
                List<string> tempInput = new List<string>(input.Substring(3).Trim().Replace("：", ":").Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries));//[人物] [值名: 数值] [值名: 数值*]
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
                List<string> tempInput = new List<string>(input.Substring(3).Trim().Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries));//[区域] [旧牌名] [新牌名] [所有*]
                List<string> tempOutput = new List<string>(LoadInfo(tempInput[0]).Trim().Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries));//AAA BBB CC AA VVV CC
                List<string> endOutput = new List<string>();
                tempInput.Add(" ");
                if (tempInput[3] == "所有")
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
                    tempOutput.Insert(find,tempInput[2]);
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
                int indexer = vKey.FindIndex((string f) => f == "&#91;" + tempInput[0] + "&#93;");//获取索引
                if (tempInput[1] == "删除")
                {
                    if (indexer == -1)
                    {
                        return $"变量[{tempInput[0]}]不存在！";
                    }
                    else
                    {
                        vKey.RemoveAt(indexer);
                        vValue.RemoveAt(indexer);
                        return $"变量[{tempInput[0]}]删除成功！";
                    }
                }
                if (tempInput[1].Substring(0,1) == "【" && tempInput[1].Substring(tempInput[1].Length - 1, 1) == "】")//是区域
                {
                    if (indexer == -1)//变量不存在，添加新项
                    {
                        vKey.Add("&#91;" + tempInput[0] + "&#93;");
                        string vv = LoadInfo(tempInput[1].Replace("【", "").Replace("】", ""));
                        vValue.Add(vv);
                        return $"添加变量[{tempInput[0]}] = {vv}";
                    }
                    else//变量存在，修改键值
                    {
                        string vv = LoadInfo(tempInput[1].Replace("【", "").Replace("】", ""));
                        vValue[indexer] = vv;
                        return $"修改变量[{tempInput[0]}] = {vv}";
                    }
                }
                //如果都不是，那就是表达式
                string expression = tempInput[1].Replace("骰子", $"{Number}").Replace("清点", $"{CountValue}");
                if (indexer == -1)//变量不存在，添加新项
                {
                    vKey.Add("&#91;" + tempInput[0] + "&#93;");
                    string vv = Convert.ToString(new DataTable().Compute(expression, ""));
                    vValue.Add(vv);
                    return $"添加变量[{tempInput[0]}] = {vv}";
                }
                else
                {
                    string vv = Convert.ToString(new DataTable().Compute(expression, ""));
                    vValue[indexer] = vv;
                    return $"修改变量[{tempInput[0]}] = {vv}";
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
        public static bool WriteInfo(string name,string str)
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
