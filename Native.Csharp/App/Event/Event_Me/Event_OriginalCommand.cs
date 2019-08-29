using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Native.Csharp.App.Event.Event_Me
{
    public class Event_OriginalCommand
    {
        /// <summary>
        /// 私聊固有指令处理
        /// </summary>
        /// <param name="input">输入内容</param>
        /// <param name="id">传递消息的编号</param>
        public static void CommandIn(string input, long id)
        {
            input = input.Replace("&#91;", "[").Replace("&#93;", "]");
            if (input.Length < 2)//降低错误触发
            {
                return;
            }
            switch (input.Substring(1, 2))//偷懒，只匹配.后2个字符
            {

                case "计算":
                    Common.CqApi.SendPrivateMessage(id, Calc(input));
                    return;

                case "骰子":
                    Common.CqApi.SendPrivateMessage(id, Dices(input));
                    return;

                case "创建":
                    switch (Crea(input))
                    {
                        case 0:
                            Common.CqApi.SendPrivateMessage(id, $@"创建{input.Trim().Substring(3).Trim()}成功！");//待优化
                            return;

                        default:
                            Common.CqApi.SendPrivateMessage(id, $@"创建{input.Trim().Substring(3).Trim()}失败！");
                            return;
                    }

                case "销毁":
                    Common.CqApi.SendPrivateMessage(id, Boom(input, out string leave1) + Environment.NewLine + leave1);
                    return;

                case "清空":
                    Boom(input, out string leave2);
                    if (leave2.Length > 1)
                    {
                        Common.CqApi.SendPrivateMessage(id, leave2);
                    }
                    switch (Crea(input))//创建
                    {
                        case 0:
                            Common.CqApi.SendPrivateMessage(id, $@"清空{input.Trim().Substring(3).Trim()}成功！");
                            return;

                        default:
                            Common.CqApi.SendPrivateMessage(id, $@"{input.Trim().Substring(3).Trim()}丢失！");
                            return;
                    }


                case "添加":
                    Common.CqApi.SendPrivateMessage(id, Add(input));
                    return;

                case "删除":
                    Common.CqApi.SendPrivateMessage(id, $@"{DelInfos(input, out string trueNameList)}");
                    if (trueNameList.Length >= 1) Common.CqApi.SendPrivateMessage(id, $@"{trueNameList.Trim()}离开了区域！");
                    return;

                case "出牌":
                    Common.CqApi.SendPrivateMessage(id, Move(input, out string fakeName));
                    if (fakeName.Length >= 1) Common.CqApi.SendPrivateMessage(id, $@"{fakeName.Trim()}离开了区域！");
                    return;

                case "抽牌":
                    Common.CqApi.SendPrivateMessage(id, Draw(input));
                    return;

                case "查看":
                    GetInfo(input, out string looname, out string looret, out string loofak);
                    Common.CqApi.SendPrivateMessage(id, $@"{looname}:
{loofak}");
                    return;

                case "清点":
                    CountNum(input, out string num);
                    Event_Variable.CountValue = int.Parse(num);
                    Common.CqApi.SendPrivateMessage(id, $@"{input.Substring(3).Trim()}有{num}张牌");
                    return;

                case "检索":
                    Common.CqApi.SendPrivateMessage(id, Search(input));
                    return;

                case "移除":
                    Common.CqApi.SendPrivateMessage(id, RemovePos(input));
                    return;

                case "导入":
                    Common.CqApi.SendPrivateMessage(id, CopyIn(input));
                    return;

                case "发现":
                    Common.CqApi.SendPrivateMessage(id, DisCover(input));
                    return;

                case "复制":
                    Common.CqApi.SendPrivateMessage(id, CopyTo(input));
                    return;

                case "洗牌":
                    Common.CqApi.SendPrivateMessage(id, Shuffle(input));
                    return;

                case "插入":
                    Common.CqApi.SendPrivateMessage(id, Inser(input));
                    return;

                case "翻转":
                    Common.CqApi.SendPrivateMessage(id, Reverse(input));
                    return;

                case "定义":
                    Common.CqApi.SendPrivateMessage(id, Command(input));
                    return;

                case "属性":
                    Common.CqApi.SendPrivateMessage(id, Attributes(input));
                    return;

                case "去重":
                    Common.CqApi.SendPrivateMessage(id, DelSame(input));
                    return;

                case "转化":
                    Common.CqApi.SendPrivateMessage(id, Variety(input));
                    return;

                case "变量":
                    Common.CqApi.SendPrivateMessage(id, Variable(input));
                    return;

                case "开始":
                    Common.CqApi.SendPrivateMessage(id, GameStart(input, id));
                    Common.CqApi.SendPrivateMessage(id, "设定完毕！");
                    return;

                case "棋盘":
                    if (IsNumeric(input.Substring(3).Trim()))
                    {
                        string[] tables = { "GHhvRqDCJ9JJdk3R/" , "Vw8vwjRDrH9YWwTG/", "jJ3qyHpyrxtqGwWw/",
                        "dDwgDJg9g99KT63h/" , "GQcHhVWd9DkY6KTR/" , "gdKYVKkCg8qKVjGk/" , "K3QyPhxY6rhrQJtx/" ,
                        "HG8wPtjdwkyCGdrT/" , "t3qk6T3dyyxDddkG/" , "kYKCw8jcWCVkJgRj/" };
                        Common.CqApi.SendPrivateMessage(id, $@"https://shimo.im/sheets/" + $@"{tables[int.Parse(input.Substring(3).Trim())]} 可复制链接后用石墨文档 App 或小程序打开");
                    }
                    else
                    {
                        Common.CqApi.SendPrivateMessage(id, "房间号有误！");
                    }
                    return;

                case "日志":
                    Common.CqApi.SendPrivateMessage(id, Event_Variable.updateLogDescription);
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

                                        CommandIn(item, id);

                                    }
                                }
                                break;

                            case "<":
                                if ((int)new DataTable().Compute(expression, "") < int.Parse(value))
                                {
                                    foreach (var item in sharpInput)
                                    {

                                        CommandIn(item, id);

                                    }
                                }
                                break;

                            case "=":
                                if ((int)new DataTable().Compute(expression, "") == int.Parse(value))
                                {
                                    foreach (var item in sharpInput)
                                    {

                                        CommandIn(item, id);

                                    }
                                }
                                break;

                            case "!":
                                if ((int)new DataTable().Compute(expression, "") != int.Parse(value))
                                {
                                    foreach (var item in sharpInput)
                                    {

                                        CommandIn(item, id);

                                    }
                                }
                                break;

                            default:
                                Common.CqApi.SendPrivateMessage(id, "找不到比较符！");
                                break;
                        }
                    }
                    catch (Exception)
                    {
                        Common.CqApi.SendPrivateMessage(id, "抛出异常！");
                    }
                    return;
            }
        }

        /// <summary>
        /// 群组固有指令处理
        /// </summary>
        /// <param name="input">输入内容</param>
        /// <param name="id">传递消息的编号</param>
        public static void CommandIn(string input, long id, bool isGroup)
        {
            input = input.Replace("&#91;", "[").Replace("&#93;", "]");
            if (input.Length < 2)//降低错误触发
            {
                return;
            }
            switch (input.Substring(1, 2))//偷懒，只匹配.后2个字符
            {

                case "计算":
                    Common.CqApi.SendGroupMessage(id, Calc(input));
                    return;

                case "骰子":
                    Common.CqApi.SendGroupMessage(id, Dices(input));
                    return;

                case "创建":
                    switch (Crea(input))
                    {
                        case 0:
                            Common.CqApi.SendGroupMessage(id, $@"创建{input.Trim().Substring(3).Trim()}成功！");//待优化
                            return;

                        default:
                            Common.CqApi.SendGroupMessage(id, $@"创建{input.Trim().Substring(3).Trim()}失败！");
                            return;
                    }

                case "销毁":
                    Common.CqApi.SendGroupMessage(id, Boom(input, out string leave1) + Environment.NewLine + leave1);
                    return;

                case "清空":
                    Boom(input, out string leave2);
                    if (leave2.Length > 1)
                    {
                        Common.CqApi.SendGroupMessage(id, leave2);
                    }
                    switch (Crea(input))//创建
                    {
                        case 0:
                            Common.CqApi.SendGroupMessage(id, $@"清空{input.Trim().Substring(3).Trim()}成功！");
                            return;

                        default:
                            Common.CqApi.SendGroupMessage(id, $@"{input.Trim().Substring(3).Trim()}丢失！");
                            return;
                    }


                case "添加":
                    Common.CqApi.SendGroupMessage(id, Add(input));
                    return;

                case "删除":
                    Common.CqApi.SendGroupMessage(id, $@"{DelInfos(input, out string trueNameList)}");
                    if (trueNameList.Length >= 1) Common.CqApi.SendGroupMessage(id, $@"{trueNameList.Trim()}离开了区域！");
                    return;

                case "出牌":
                    Common.CqApi.SendGroupMessage(id, Move(input, out string fakeName));
                    if (fakeName.Length >= 1) Common.CqApi.SendGroupMessage(id, $@"{fakeName.Trim()}离开了区域！");
                    return;

                case "抽牌":
                    Common.CqApi.SendGroupMessage(id, Draw(input));
                    return;

                case "查看":
                    GetInfo(input, out string looname, out string looret, out string loofak);
                    Common.CqApi.SendGroupMessage(id, $@"{looname}:
{loofak}");
                    return;

                case "清点":
                    CountNum(input, out string num);
                    Event_Variable.CountValue = int.Parse(num);
                    Common.CqApi.SendGroupMessage(id, $@"{input.Substring(3).Trim()}有{num}张牌");
                    return;

                case "检索":
                    Common.CqApi.SendGroupMessage(id, Search(input));
                    return;

                case "移除":
                    Common.CqApi.SendGroupMessage(id, RemovePos(input));
                    return;

                case "导入":
                    Common.CqApi.SendGroupMessage(id, CopyIn(input));
                    return;

                case "发现":
                    Common.CqApi.SendGroupMessage(id, DisCover(input));
                    return;

                case "复制":
                    Common.CqApi.SendGroupMessage(id, CopyTo(input));
                    return;

                case "洗牌":
                    Common.CqApi.SendGroupMessage(id, Shuffle(input));
                    return;

                case "插入":
                    Common.CqApi.SendGroupMessage(id, Inser(input));
                    return;

                case "翻转":
                    Common.CqApi.SendGroupMessage(id, Reverse(input));
                    return;

                case "定义":
                    Common.CqApi.SendGroupMessage(id, Command(input));
                    return;

                case "属性":
                    Common.CqApi.SendGroupMessage(id, Attributes(input));
                    return;

                case "去重":
                    Common.CqApi.SendGroupMessage(id, DelSame(input));
                    return;

                case "转化":
                    Common.CqApi.SendGroupMessage(id, Variety(input));
                    return;

                case "变量":
                    Common.CqApi.SendGroupMessage(id, Variable(input));
                    return;

                case "开始":
                    Common.CqApi.SendGroupMessage(id, GameStart(input, id));
                    Common.CqApi.SendGroupMessage(id, "设定完毕！");
                    return;

                case "退群":
                    Event_Variable.groupId = id;
                    Event_Variable.qqId = Event_Variable.QQQ;
                    Event_Variable.member = Common.CqApi.GetMemberInfo(Event_Variable.groupId, Event_Variable.qqId, true);//获取群成员
                    Event_Variable.PT = Convert.ToString(Event_Variable.member.PermitType);//获取权限：Holder群主，Manage管理，None群员
                    if (Event_Variable.PT == "None")//屁民瞎发啥指令
                    {
                        Common.CqApi.SendGroupMessage(id, "权限不足！");
                        return;
                    }
                    if (input.Length > 8)//有参数
                    {
                        if (input.Substring(3).Trim() == Convert.ToString(Common.CqApi.GetLoginQQ()))//膝盖中了一箭
                        {
                            Common.CqApi.SendGroupMessage(id, "感谢你的支持，再见！");
                            Common.CqApi.SetGroupExit(id, false);
                            return;
                        }
                        return;//踢的不是我，溜了溜了
                    }
                    else//没参数，意思全退呗
                    {
                        Common.CqApi.SendGroupMessage(id, "感谢你的支持，再见！");
                        Common.CqApi.SetGroupExit(id, false);
                        return;
                    }

                case "清理":
                    if (!Event_Variable.isGroup)
                    {

                        return;
                    }
                    Event_Variable.groupId = id;
                    Event_Variable.qqId = Event_Variable.QQQ;
                    Event_Variable.member = Common.CqApi.GetMemberInfo(Event_Variable.groupId, Event_Variable.qqId, true);//获取群成员
                    Event_Variable.PT = Convert.ToString(Event_Variable.member.PermitType);//获取权限：Holder群主，Manage管理，None群员
                    if (Event_Variable.PT != "Holder")
                    {
                        Common.CqApi.SendGroupMessage(id, "权限不足！");
                        return;
                    }
                    try
                    {
                        DeleteOldFiles(AppDomain.CurrentDomain.BaseDirectory + @"SaveDir\", int.Parse(input.Substring(3).Trim()));
                        DeleteOldFiles(AppDomain.CurrentDomain.BaseDirectory + @"SaveDir\Att\", int.Parse(input.Substring(3).Trim()));
                        DeleteOldFiles(AppDomain.CurrentDomain.BaseDirectory + @"SaveDir\Command\", int.Parse(input.Substring(3).Trim()));
                    }
                    catch (Exception)
                    {
                        Common.CqApi.SendGroupMessage(id, "抛出错误！");
                        return;
                    }
                    Common.CqApi.SendGroupMessage(id, "清理完成！");
                    return;

                case "棋盘":
                    if (IsNumeric(input.Substring(3).Trim()))
                    {
                        string[] tables = { "GHhvRqDCJ9JJdk3R/" , "Vw8vwjRDrH9YWwTG/", "jJ3qyHpyrxtqGwWw/",
                        "dDwgDJg9g99KT63h/" , "GQcHhVWd9DkY6KTR/" , "gdKYVKkCg8qKVjGk/" , "K3QyPhxY6rhrQJtx/" ,
                        "HG8wPtjdwkyCGdrT/" , "t3qk6T3dyyxDddkG/" , "kYKCw8jcWCVkJgRj/" };
                        Common.CqApi.SendGroupMessage(id, $@"https://shimo.im/sheets/" + $@"{tables[int.Parse(input.Substring(3).Trim())]} 可复制链接后用石墨文档 App 或小程序打开");
                    }
                    else
                    {
                        Common.CqApi.SendGroupMessage(id, "房间号有误！");
                    }
                    return;

                case "日志":
                    Common.CqApi.SendGroupMessage(id, Event_Variable.updateLogDescription);
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

                                        CommandIn(item, id);

                                    }
                                }
                                break;

                            case "<":
                                if ((int)new DataTable().Compute(expression, "") < int.Parse(value))
                                {
                                    foreach (var item in sharpInput)
                                    {

                                        CommandIn(item, id);

                                    }
                                }
                                break;

                            case "=":
                                if ((int)new DataTable().Compute(expression, "") == int.Parse(value))
                                {
                                    foreach (var item in sharpInput)
                                    {

                                        CommandIn(item, id);

                                    }
                                }
                                break;

                            case "!":
                                if ((int)new DataTable().Compute(expression, "") != int.Parse(value))
                                {
                                    foreach (var item in sharpInput)
                                    {

                                        CommandIn(item, id);

                                    }
                                }
                                break;

                            default:
                                Common.CqApi.SendGroupMessage(id, "找不到比较符！");
                                break;
                        }
                    }
                    catch (Exception)
                    {
                        Common.CqApi.SendGroupMessage(id, "抛出异常！");
                    }
                    return;
            }
        }

        /// <summary>
        /// 计算字符串算式
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string Calc(string input)
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

        //骰子
        public static string Dices(string input)
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
        public static int Crea(string input)
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
        public static string Boom(string input, out string leave)
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
        public static string Add(string input)//.删除 区域 AA BB CC
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
        public static void GetInfo(string input, out string name, out string reture, out string refake)
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
        public static string DelInfos(string input, out string trueName)//.删除 区域 AA BB CC
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
        public static string Shuffle(string input)
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
                        input = (input + " " + deck).Trim();
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
                    ////Fisher Yates Shuffle算法的时空复杂度太高，故舍弃
                    //List<string> list2 = new List<string>();
                    //for (int i = 0; i < count; i++)
                    //{
                    //    int rd = new Random(Guid.NewGuid().GetHashCode()).Next(0, Convert.ToInt32(list.Count));
                    //    list2.Add(list[rd]);
                    //    list.RemoveAt(rd);
                    //}
                    //DelInfo(input); CreateInfo(input);
                    //string shuffles = string.Join(" ", list2.ToArray());
                    for (int count = list.Count; count > 0; count--)
                    {
                        int rd = new Random(Guid.NewGuid().GetHashCode()).Next(0, Convert.ToInt32(count));
                        list.Add(list[rd]);
                        list.RemoveAt(rd);
                    }
                    DelInfo(input); CreateInfo(input);
                    WriteInfo(input, $@"{string.Join(" ", list.ToArray())}");
                    if (input.Contains("私密"))
                    {
                        return "洗切完成！";
                    }
                    else
                    {
                        return string.Join(" ", list.ToArray());
                    }
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
        public static string Move(string input, out string trueName)//.出牌 区域A 区域B XXX YYY ZZZ
        {
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
                        input = (oldZone + " " + newZone + " " + input).Trim();
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
                    nameList.Add(list[i + 2]);//需要移动的对象假名
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

                    int find;
                    for (int i = 0; i < nameList.Count; i++)//遍历需要移动的对象假名
                    {
                        find = fromListFake.FindIndex((string f) => f.Equals(nameList[i]));//找到第一个匹配假名的元素
                        if (find == -1)//如果找不到
                        {
                            trueName = "";
                            return $@"无法找到{string.Join(" ", nameList.ToArray())}";//返回无法处理的元素集
                        }
                        if (fromList[find].Contains("【"))//找到了，且移动的元素带有伪装，将全名传入trueName，用来返回
                        {
                            trueName += $@" {fromList[find]}";
                        }
                        toList.Add(fromList[find]);
                        fromList.Remove(fromList[find]);
                        fromListFake.RemoveAt(find);
                    }
                    DelInfo(name1); CreateInfo(name1);
                    WriteInfo(name1, $@" {string.Join(" ", fromList.ToArray())}");//写入来源区域
                    DelInfo(name2); CreateInfo(name2);
                    WriteInfo(name2, $@" {string.Join(" ", toList.ToArray())}");//写入目标区域
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
                catch (Exception ex)
                {
                    trueName = "";
                    return Event_CheckError.CheckError(ex);
                    //return "出牌失败！";
                }

            }
            catch (Exception)
            {
                trueName = "";
                return "来源区域或目标区域不存在，请创建！";
            }
        }

        //抽牌
        public static string Draw(string input)//.抽牌 旧区域 新区域 数量
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
        public static void CountNum(string input, out string num)
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
        public static string Inser(string input)//.插入 [区域] [序号] [牌名]
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
        public static string Search(string input)//.检索 [区域] [关键字] [所有*]
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
        public static string DisCover(string input)//.发现 [区域] [数量*]
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
        public static string RemovePos(string input)//.移除 [区域] [序号]
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
        public static string Command(string input)
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
        public static string Reverse(string input)
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
        public static string Attributes(string input)//.属性 [人物] [值名:数值] [值名:数值*]
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
        public static string GameStart(string input, long id)
        {
            try
            {
                input = input.Substring(3).Trim();
                if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + @"SaveDir\Att\" + id + ".ini"))//如果属性文件存在
                {
                    List<string> orige = new List<string>(File.ReadAllText(AppDomain.CurrentDomain.BaseDirectory + @"SaveDir\Att\" + id + ".ini")
                        .Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries));
                    List<string> reValues = new List<string>();//接收返回值
                    int i = -1;
                    List<string> newOrige = new List<string>(File.ReadAllText(AppDomain.CurrentDomain.BaseDirectory + @"SaveDir\Att\" + id + ".ini")
                        .Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries));
                    foreach (var item in newOrige)//匹配前缀替换
                    {
                        i++;
                        switch (item.Substring(0, 3))
                        {
                            case "牌堆:":
                                orige[i] = $@"牌堆:私密{input}";
                                reValues.Add($@"{item} → 私密{input}");
                                break;
                            case "手牌:":
                                orige[i] = $@"手牌:{id}的{input}";
                                reValues.Add($@"{item} → {id}的{input}");
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
                    DelInfo(@"Att\" + id); CreateInfo(@"Att\" + id);
                    WriteInfo(@"Att\" + id, string.Join(" ", orige.Distinct().ToArray()));
                    return $@"{Event_Variable.QQQ}的属性修改:{Environment.NewLine}{string.Join(Environment.NewLine, reValues.ToArray()) }";

                }
                else//没有旧信息的话就创建
                {
                    CreateInfo(@"Att\" + id);
                    WriteInfo(@"Att\" + id, $@"牌堆:私密{input} 手牌:{id}的{input} 桌面:{input}的桌面 骰子:20");
                    CreateInfo(input); CreateInfo($@"{input}的桌面");
                    return $@"{input}的属性为
牌堆:私密{input}
手牌:{id}的{input}
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
        public static string CopyIn(string input)
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
        public static string CopyTo(string input)
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
        public static string DelSame(string input)
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
        public static string Variety(string input)//.转化 [区域] [旧牌名] [新牌名] [所有*]
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
        public static string Variable(string input)//.变量 [变量名] [表达式/区域/删除]
        {
            try
            {
                List<string> tempInput = new List<string>(input.Substring(3).Trim()
                    .Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries));//[变量名] [表达式/【区域】/删除]
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

        public static void Dicex(string input)
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
        public static bool IsNumeric(string str)
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
        public static int GetStringLastNumber(string str)
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
        public static void DeleteOldFiles(string dir, int days)
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
