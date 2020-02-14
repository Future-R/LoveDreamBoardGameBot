using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Native.Csharp.App.Event.Event_Me
{
    public class 解释
    {
        public static void 语法分析(string 用户输入)
        {
            //用户输入 = Regex.Replace(用户输入, @"[/n/r]", "");
            数据.循环次数 = 0; 数据.发送次数 = 0;
            List<string> 语句集 = new List<string>(用户输入.Split(new[] { '。' }, StringSplitOptions.RemoveEmptyEntries));
            foreach (var 语句 in 语句集)
            {
                string 新语句 = 变量解释(语句);
                //Task.Factory.StartNew(线程.新任务, 线程.取消资源请求.Token);
                string 执行结果 = 执行(新语句.Trim());
                if (执行结果 != "")
                {
                    数据.发送次数++;
                    if (数据.发送次数 > 9)
                    {
                        Common.CqApi.SendPrivateMessage(数据.发送目标, "不干了！");
                        return;
                    }
                    if (数据.私聊)
                    {
                        Common.CqApi.SendPrivateMessage(数据.发送目标, 执行结果);
                    }
                    else
                    {
                        Common.CqApi.SendGroupMessage(数据.发送目标, 执行结果);
                    }
                }
            }
        }

        public static string 执行(string 语句)
        {
            while (数据.循环次数 <= 9999)
            {
                数据.循环次数++;
                //关键字打头
                #region 关系式
                if (语句.StartsWith("已知"))
                {
                    List<string> 方程组 = new List<string>
                        (语句.Substring(2).Trim().Split(new[] { '，' }, StringSplitOptions.RemoveEmptyEntries));
                    foreach (var 算式 in 方程组)
                    {
                        if (算式.Contains("比")) 运算.比较运算(算式);
                        else if (算式.Contains("相同") || 算式.Contains("一样") || 算式.Contains("相反")) 运算.相同运算(算式);

                        //else if (算式.Contains("等于") || 算式.Contains("=")) 运算.等式运算(算式);
                        else return $"“{算式}”还不会算。";
                    }
                    return "";
                }
                #endregion
                #region 获取组件值
                if (语句.StartsWith("求") || 语句.StartsWith("问"))
                {
                    string 获取语句 = 语句.Substring(1);
                    return 数据.读取组件(获取语句);
                }
                #endregion
                #region 查询实体
                if (语句.StartsWith("查询"))
                {
                    string 查询对象 = 语句.Substring(2);
                    List<string> 查询对象表 = new List<string>(查询对象.Split(new[] { '、' }, StringSplitOptions.RemoveEmptyEntries));
                    string 打印日志 = "";
                    foreach (var 对象 in 查询对象表)
                    {
                        if (数据.实体.ContainsKey(对象))
                        {
                            打印日志 += $"【{对象}】\r\n";
                            foreach (var item in 数据.实体[对象].Keys)
                            {
                                打印日志 += $"{item}：{数据.实体[对象][item]}\r\n";
                            }
                        }
                        else
                        {
                            打印日志 += $"没找到{对象}。\r\n";
                        }
                    }
                    return 打印日志.TrimEnd('\n').TrimEnd('\r');
                }
                #endregion
                #region 销毁
                if (语句.StartsWith("销毁"))
                {
                    string 销毁语句 = 语句.Substring(2).Trim();
                    List<string> 销毁对象 = new List<string>(销毁语句.Split(new[] { '、' }, StringSplitOptions.RemoveEmptyEntries));
                    foreach (var 对象 in 销毁对象)
                    {
                        if (!对象.Contains("的"))//销毁主体
                        {
                            数据.实体.Remove(对象);
                        }
                        else//销毁组件
                        {
                            数据.实体
                                [对象.Split(new[] { '的' }, StringSplitOptions.RemoveEmptyEntries)[0]].Remove
                                (对象.Split(new[] { '的' }, StringSplitOptions.RemoveEmptyEntries)[1]);
                        }
                    }
                    return "";
                }
                #endregion
                #region 计算
                if (语句.StartsWith("计算"))
                {
                    string 计算结果 = 运算.计算(语句.Substring(2));
                    数据.写入实体(new List<string>(new string[] { "计算", "结果", 计算结果 }));
                    return "";
                }

                #endregion
                #region 如果
                if (语句.StartsWith("如果"))//如果小明有钱且小明的钱大于0：
                {
                    string 如果指令 = 语句.Substring(2);
                }
                #endregion
                #region 直到
                if (语句.StartsWith("直到"))
                {
                    string 判别式 = 语句.Substring(2, 语句.IndexOf("：") - 2);
                    List<string> 语句集 = new List<string>(
                            语句.Substring(语句.IndexOf("：") + 1)
                            .Split(new[] { '；' }, StringSplitOptions.RemoveEmptyEntries));
                    
                    while (!判定(判别式) && 数据.循环次数 <= 9999)
                    {
                        foreach (var 子语句 in 语句集)
                        {
                            string 执行结果 = 执行(子语句.Trim());
                            if (执行结果 != "")
                            {
                                数据.发送次数++;
                                if (数据.发送次数 > 9)
                                {
                                    return "不干了！";
                                }
                                if (数据.私聊)
                                {
                                    Common.CqApi.SendPrivateMessage(数据.发送目标, 执行结果);
                                }
                                else
                                {
                                    Common.CqApi.SendGroupMessage(数据.发送目标, 执行结果);
                                }
                            }
                        }
                    }
                    return "";
                }
                #endregion

                //关键字包含
                #region 赋值
                if (语句.Contains("是"))
                {
                    if (语句.Contains("的"))
                    {
                        数据.写入实体(new List<string>(语句.Split(new[] { '是', '的' }, StringSplitOptions.RemoveEmptyEntries)));
                    }
                    else//不输入组件则默认为“值”组件
                    {
                        List<string> 内容 = new List<string>(语句.Split(new[] { '是' }, StringSplitOptions.RemoveEmptyEntries));
                        内容.Insert(1, "值");
                        数据.写入实体(内容);
                    }
                    return "";
                }
                #endregion
                #region 加减乘除
                if (语句.Contains("+"))
                {
                    if (语句.Contains("的"))
                    {
                        List<string> 写入参数 = new List<string>(new List<string>(语句.Split(new[] { '+', '的' }, StringSplitOptions.RemoveEmptyEntries)));
                        写入参数[2] = 运算.计算(数据.实体[写入参数[0]][写入参数[1]] + "+" + 写入参数[2]);
                        数据.写入实体(写入参数);
                    }
                    else//不输入组件则默认为“值”组件
                    {
                        List<string> 内容 = new List<string>(语句.Split(new[] { '+' }, StringSplitOptions.RemoveEmptyEntries));
                        内容.Insert(1, "值");
                        数据.写入实体(内容);
                    }
                    return "";
                }
                if (语句.Contains("-"))
                {
                    if (语句.Contains("的"))
                    {
                        List<string> 写入参数 = new List<string>(new List<string>(语句.Split(new[] { '+', '的' }, StringSplitOptions.RemoveEmptyEntries)));
                        写入参数[2] = 运算.计算(数据.实体[写入参数[0]][写入参数[1]] + "-" + 写入参数[2]);
                        数据.写入实体(写入参数);
                    }
                    else//不输入组件则默认为“值”组件
                    {
                        List<string> 内容 = new List<string>(语句.Split(new[] { '-' }, StringSplitOptions.RemoveEmptyEntries));
                        内容.Insert(1, "值");
                        数据.写入实体(内容);
                    }
                    return "";
                }
                if (语句.Contains("*"))
                {
                    if (语句.Contains("的"))
                    {
                        List<string> 写入参数 = new List<string>(new List<string>(语句.Split(new[] { '+', '的' }, StringSplitOptions.RemoveEmptyEntries)));
                        写入参数[2] = 运算.计算(数据.实体[写入参数[0]][写入参数[1]] + "+" + 写入参数[2]);
                        数据.写入实体(写入参数);
                    }
                    else//不输入组件则默认为“值”组件
                    {
                        List<string> 内容 = new List<string>(语句.Split(new[] { '*' }, StringSplitOptions.RemoveEmptyEntries));
                        内容.Insert(1, "值");
                        数据.写入实体(内容);
                    }
                    return "";
                }
                if (语句.Contains("/"))
                {
                    if (语句.Contains("的"))
                    {
                        List<string> 写入参数 = new List<string>(new List<string>(语句.Split(new[] { '+', '的' }, StringSplitOptions.RemoveEmptyEntries)));
                        写入参数[2] = 运算.计算(数据.实体[写入参数[0]][写入参数[1]] + "+" + 写入参数[2]);
                        数据.写入实体(写入参数);
                    }
                    else//不输入组件则默认为“值”组件
                    {
                        List<string> 内容 = new List<string>(语句.Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries));
                        内容.Insert(1, "值");
                        数据.写入实体(内容);
                    }
                    return "";
                }
                #endregion


                return $"不理解“{语句}”。";
            }
            return 数据.报错;
        }


        public static string 变量解释(string 语句)
        {
            if (!语句.Contains("【") || !语句.Contains("】"))
            {
                return 语句;
            }
            else
            {
                Stack<char> 符号栈 = new Stack<char>(); Stack<char> 内容栈 = new Stack<char>(); string 栈内容 = "";
                for (int i = 0; i < 语句.Length; i++)
                {
                    char 当前字符 = 语句[i];
                    switch (当前字符)
                    {
                        case '【':
                            内容栈.Clear();
                            符号栈.Push(当前字符);
                            break;
                        case '】':
                            if (符号栈.Peek() == '【')
                            {
                                foreach (var 内容 in 内容栈)
                                {
                                    栈内容 = 栈内容.Insert(0, 内容.ToString());
                                }

                                string 替换内容 = "";
                                List<string> 参数 = new List<string>(栈内容.Split(new[] { '的' }, StringSplitOptions.RemoveEmptyEntries));
                                if (数据.实体.ContainsKey(参数[0]))
                                {
                                    if (参数.Count == 1)
                                    {
                                        参数.Add("值");
                                    }
                                    if (数据.实体[参数[0]].ContainsKey(参数[1]))
                                    {
                                        替换内容 = 数据.实体[参数[0]][参数[1]];
                                    }
                                }

                                符号栈.Pop();
                                
                                return 变量解释(语句.Substring(0, i - 内容栈.Count - 1) + 替换内容 + 语句.Substring(i + 1));
                            }
                            else
                            {
                                符号栈.Push(当前字符);
                            }
                            break;
                        default:
                            内容栈.Push(当前字符);
                            break;
                    }
                }
                return 语句;
            }
        }

        public static bool 判定(string 条件)
        {
            bool 且 = false;
            if (条件.Contains("且"))
            {
                且 = true;
            }
            List<string> 判别式集合 = new List<string>(条件.Split(new[] { '且', '或' }, StringSplitOptions.RemoveEmptyEntries));
            switch (条件.Trim().TrimEnd('的'))
            {
                case "真":
                case "对":
                    return true;
                case "假":
                case "错":
                    return false;
                default:
                    break;
            }
            if (且)//有一个假就返回错
            {
                foreach (var 判别式 in 判别式集合)
                {
                    if (!对错(判别式))
                    {
                        return false;
                    }
                }
                return true;
            }
            else//有一个真就返回对
            {
                foreach (var 判别式 in 判别式集合)
                {
                    if (对错(判别式))
                    {
                        return true;
                    }
                }
                return false;
            }
        }

        static bool 对错(string 判别式)
        {
            string 模式 = ""; 数据.布尔委托 委托 = new 数据.布尔委托(比较.等于);
            判别式 = 判别式.Replace("大于等于", "不小于").Replace("小于等于", "不大于");
            if (判别式.Contains("有"))
            {
                模式 = "有";
                委托 = new 数据.布尔委托(比较.有);
            }
            if (判别式.Contains("等于"))
            {
                模式 = "等于";
                委托 = new 数据.布尔委托(比较.等于);
            }
            if (判别式.Contains("大于"))
            {
                模式 = "大于";
                委托 = new 数据.布尔委托(比较.大于);
            }
            if (判别式.Contains("小于"))
            {
                模式 = "小于";
                委托 = new 数据.布尔委托(比较.小于);
            }
            if (判别式.Contains("包含"))
            {
                模式 = "包含";
                委托 = new 数据.布尔委托(比较.包含);
            }
            if (判别式.Contains("开头是"))
            {
                模式 = "开头是";
                委托 = new 数据.布尔委托(比较.开头是);
                if (判别式.Contains("结尾是"))
                {
                    模式 = "结尾是";
                    委托 = new 数据.布尔委托(比较.结尾是);
                }
            }

            List<string> 集合 = new List<string>(判别式.Split(new string[] { 模式 }, StringSplitOptions.RemoveEmptyEntries));

            if (集合.Count != 2)
            {
                return false;
            }
            if (集合[0].EndsWith("不"))
            {
                集合[0] = 集合[0].TrimEnd('不');
                return !委托(集合[0], 集合[1]);
            }

            return 委托(集合[0], 集合[1]);
        }
    }
}
