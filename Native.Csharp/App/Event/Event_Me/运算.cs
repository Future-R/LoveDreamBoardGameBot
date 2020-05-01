﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Native.Csharp.App.Event.Event_Me
{
    public class 运算
    {
        public static void 比较运算(string 算式)
        {
            bool 声明组件 = true;
            if (算式.Contains("大") || 算式.Contains("多"))
            {
                if (!算式.Contains("的"))//没有声明组件，自动填充为"值"
                {
                    声明组件 = false;
                }
                List<string> 分词 = new List<string>(算式.Split(new[] { '比', '大', '多', '的' }, StringSplitOptions.RemoveEmptyEntries));

                if (算式.IndexOf("的") == 算式.LastIndexOf("的"))//如果只有一个"的"
                {
                    if (插二(算式, "比")) 分词.Insert(1, 分词[2]);
                    else 分词.Insert(3, 分词[1]);
                }

                if (!声明组件)
                {
                    分词.Insert(1, "值"); 分词.Insert(3, "值");
                }
                if (!是纯数(分词[4]))
                {
                    分词[4] = 数据.实体[分词[4]]["值"];
                }
                if (!数据.实体.ContainsKey(分词[0]))//如果没有这个实体就创建实体，并添加对应的组件
                {
                    数据.实体.Add(分词[0], new Dictionary<string, string>());
                    数据.实体[分词[0]].Add(分词[1], new DataTable().Compute(数据.实体[分词[2]][分词[3]] + "+" + 分词[4], "").ToString());
                }
                else
                {
                    数据.实体[分词[0]][分词[1]] = new DataTable().Compute(数据.实体[分词[2]][分词[3]] + "+" + 分词[4], "").ToString();
                }
            }
            else
            {
                if (!算式.Contains("的"))
                {
                    声明组件 = false;
                }
                List<string> 分词 = new List<string>(算式.Split(new[] { '比', '小', '少', '的' }, StringSplitOptions.RemoveEmptyEntries));
                if (!声明组件)
                {
                    分词.Insert(1, "值"); 分词.Insert(3, "值");
                }
                if (!是纯数(分词[4]))
                {
                    分词[4] = 数据.实体[分词[4]]["值"];
                }
                if (!数据.实体.ContainsKey(分词[0]))//如果没有这个实体就创建实体，并添加对应的组件
                {
                    数据.实体.Add(分词[0], new Dictionary<string, string>());
                    数据.实体[分词[0]].Add(分词[1], new DataTable().Compute(数据.实体[分词[2]][分词[3]] + "-" + 分词[4], "").ToString());
                }
                else
                {
                    数据.实体[分词[0]][分词[1]] = new DataTable().Compute(数据.实体[分词[2]][分词[3]] + "-" + 分词[4], "").ToString();
                }
            }
            return;
        }

        public static void 相同运算(string 算式)
        {
            string 模式 = "相同";
            if (算式.EndsWith("一样"))
            {
                算式 = 算式.TrimEnd('样').TrimEnd('一') + "相同";
            }
            if (算式.Contains("相反")) 模式 = "相反";

            List<string> 分词 = new List<string>(算式.Split(new string[] { "和", "与", "的", 模式 }, StringSplitOptions.RemoveEmptyEntries));

            if (!算式.Contains("的"))
            {
                数据.实体[分词[0]] = 数据.实体[分词[1]];
            }
            else if (算式.IndexOf("的") == 算式.LastIndexOf("的"))//如果只有一个"的"
            {
                if (插二(算式, "和")) 分词.Insert(1, 分词[2]);
                else 分词.Insert(3, 分词[1]);
            }

            if (算式.Contains("的"))
            {
                if (!数据.实体.ContainsKey(分词[0]))//如果没有这个实体就创建实体，并添加对应的组件
                {
                    数据.实体.Add(分词[0], new Dictionary<string, string>());
                    数据.实体[分词[0]].Add(分词[1], 数据.实体[分词[2]][分词[3]]);
                }
                else
                {
                    数据.实体[分词[0]][分词[1]] = 数据.实体[分词[2]][分词[3]];
                }
                if (模式 == "相反")
                {
                    List<string> 反义词典 = 集合.静态集合生成(数据.实体["反义词典"]["列表"]);
                    int 位置 = 反义词典.IndexOf(数据.实体[分词[0]][分词[1]]);
                    if (位置 % 2 == 0)
                    {
                        数据.实体[分词[0]][分词[1]] = 反义词典[位置 + 1];
                    }
                    else
                    {
                        数据.实体[分词[0]][分词[1]] = 反义词典[位置 - 1];
                    }
                }
            }
            else//组件未声明，覆盖复制实体
            {
                if (!数据.实体.ContainsKey(分词[0]))
                {
                    数据.实体.Add(分词[0], 数据.实体[分词[1]]);
                }
                else
                {
                    数据.实体[分词[0]] = 数据.实体[分词[1]];
                }
            }
            return;
        }

        public static string 计算(string 算式)
        {
            算式 = 转阿拉伯数字(算式);
            string[] 数组 = 骰子(算式).Split(new[] { '=' });
            return 数组[数组.Length - 1];
        }

        public static string 骰子(string 表达式)
        {
            if (!表达式.ToLower().StartsWith("r") && !表达式.ToLower().StartsWith("w"))
            {
                表达式 = "R" + 表达式;
            }
            string 描述 = "";
            bool 展示详情 = true;
            //检查合法性
            int 分界处 = 1;
            foreach (var 字 in 表达式.Substring(1).ToUpper().Replace("×", "*").Replace("X", "*")
                .Replace("（", "(").Replace("）", ")").Replace("÷", "/"))//d5你好
            {
                if (!是数字(字) && !是符号(字) && 字 != ' ' && 字 != 'w' && 字 != 'W' && 字 != 'h' && 字 != 'H')//不是数字和符号，那就是描述
                {
                    break;
                }
                分界处++;
            }
            描述 = 表达式.Substring(分界处);
            表达式 = 表达式.Substring(0, 分界处).Replace(" ", "");

            string 返回值 = 描述;
            表达式 = 表达式.ToUpper().Replace("×", "*").Replace("X", "*")
                .Replace("（", "(").Replace("）", ")").Replace("÷", "/");
            if (表达式.Length > 1)
            {
                if (表达式[1] == 'H')
                {
                    表达式 = 表达式.Substring(0, 1) + 表达式.Substring(2);
                    if (!数据.私聊)
                    {
                        数据.私聊 = true;
                        数据.群聊目标 = 数据.私聊目标;
                    }
                }
            }

            //提供默认值
            string 骰子面 = "100"; string 骰池面 = "10"; string 成功值 = "8";
            //读取玩家设置
            if (是纯数(数据.读取组件("我的默认骰")))
            {
                骰子面 = 数据.读取组件("我的默认骰");
            }
            if (是纯数(数据.读取组件("我的骰池面")))
            {
                骰池面 = 数据.读取组件("我的骰池面");
            }
            if (是纯数(数据.读取组件("我的加骰值")))
            {
                成功值 = 数据.读取组件("我的成功值");
            }

            //wod骰补全
            if (表达式.StartsWith("W"))
            {
                展示详情 = false;
                string 补全 = "10A" + 骰池面;
                if (表达式.Replace("W", "").Length < 1)
                {
                    表达式 += 补全;
                }
                if (表达式[1] == 'W')//双W表示展示详情
                {
                    表达式 = 表达式.Substring(1);
                    展示详情 = true;
                }
                int 位置 = 2;
                foreach (var 字 in 表达式.Substring(1))
                {
                    if (是数字(字) && 补全 == "10A" + 骰池面)
                    {
                        补全 = "A" + 骰池面;
                    }
                    else if (字 == 'A')
                    {
                        break;
                    }
                    else if (是符号(字))
                    {
                        表达式 = 表达式.Insert(位置 - 1, 补全);
                        break;
                    }
                    if (位置 >= 表达式.Length)
                    {
                        表达式 = 表达式.Insert(位置, 补全);
                        break;
                    }
                    位置++;
                }
            }
            //常规骰补全
            if (表达式.StartsWith("RB") || 表达式.StartsWith("RP"))
            {
                表达式 = 表达式.Insert(1, $"1D{骰子面}");
            }
            if (表达式.Length < 3)
            {
                if (表达式.Length < 2)
                {
                    表达式 += "D";
                }
                表达式 = 表达式.Insert(1, "1") + 骰子面;
            }
            表达式 = 表达式.Substring(1);//去掉开头的R或W
            List<string> 中缀分词 = 中缀分词补全(表达式, 骰子面);

            string 昵称 = 数据.获取昵称();
            if (数据.私聊 && 数据.群聊目标 != null)
            {
                返回值 += "你骰出了";
            }
            else
            {
                返回值 += $"{昵称.TrimEnd('的')}骰出了";
            }
            string 计算结果 = 后缀计算(中缀转后缀(中缀分词), string.Join("", 中缀分词.ToArray()), 骰池面, 成功值, 展示详情);
            if (计算结果.StartsWith("错误"))
            {
                return 计算结果;
            }
            if (计算结果.Split(new[] { '=' }, StringSplitOptions.RemoveEmptyEntries)[计算结果.Split(new[] { '=' }, StringSplitOptions.RemoveEmptyEntries).Length - 1] ==
                new DataTable().Compute(计算结果.Split(new[] { '=' }, StringSplitOptions.RemoveEmptyEntries)[计算结果.Split(new[] { '=' }, StringSplitOptions.RemoveEmptyEntries).Length - 1], "").ToString())
            {
                数据.写入实体("上次", "骰点", 计算结果.Split(new[] { '=' }, StringSplitOptions.RemoveEmptyEntries)[计算结果.Split(new[] { '=' }, StringSplitOptions.RemoveEmptyEntries).Length - 1]);
                return 返回值 + 计算结果;
            }
            数据.写入实体("上次", "骰点", new DataTable().Compute(计算结果.Split(new[] { '=' }, StringSplitOptions.RemoveEmptyEntries)[计算结果.Split(new[] { '=' }, StringSplitOptions.RemoveEmptyEntries).Length - 1], "").ToString());
            return 返回值 + 计算结果 + "=" +
                new DataTable().Compute(计算结果.Split(new[] { '=' }, StringSplitOptions.RemoveEmptyEntries)[计算结果.Split(new[] { '=' }, StringSplitOptions.RemoveEmptyEntries).Length - 1], "").ToString();

        }

        public static string 后缀计算(List<string> 计算表, string 计算过程, string 骰池面, string 成功值, bool 骰池详情)
        {
            Stack<string> 计算结果 = new Stack<string>();
            foreach (var 元素 in 计算表)
            {
                if (是纯数(元素) && !"+-.".Contains(元素))
                {
                    计算结果.Push(元素);
                }
                else
                {
                    string 返回结果 = ""; decimal 参数1 = 0; decimal 参数2 = 0;
                    switch (元素)
                    {
                        //case "↑":
                        //case "↓":
                        //    参数1 = Convert.ToDecimal(计算结果.Pop());
                        //    if (元素 == "↑")
                        //    {
                        //        返回结果 = Math.Ceiling(Convert.ToDecimal(参数1)).ToString();
                        //    }
                        //    else
                        //    {
                        //        返回结果 = Math.Floor(Convert.ToDecimal(参数1)).ToString();
                        //    }
                        //    break;

                        case "+":
                        case "-":
                        case "*":
                        case "/":
                        case "%":
                            参数2 = Convert.ToDecimal(计算结果.Pop());
                            参数1 = Convert.ToDecimal(计算结果.Pop());
                            返回结果 = new DataTable().Compute($"{参数1}{元素}{参数2}", "").ToString();
                            计算过程 += "=" + 计算过程.Split(new[] { '=' }, StringSplitOptions.RemoveEmptyEntries)
                                [计算过程.Split(new[] { '=' }, StringSplitOptions.RemoveEmptyEntries).Length - 1].
                                Replace($"{参数1}{元素}{参数2}", 返回结果).
                                Replace($"({参数1}){元素}{参数2}", 返回结果).
                                Replace($"{参数1}{元素}({参数2})", 返回结果).
                                Replace($"({参数1}){元素}({参数2})", 返回结果);
                            break;

                        case ".":
                            参数2 = Convert.ToDecimal(计算结果.Pop());
                            参数1 = Convert.ToDecimal(计算结果.Pop());
                            返回结果 = new DataTable().Compute($"{参数1}{元素}{参数2}", "").ToString();
                            break;

                        case "^":
                            参数2 = Convert.ToDecimal(计算结果.Pop());
                            参数1 = Convert.ToDecimal(计算结果.Pop());
                            返回结果 = Math.Pow(Convert.ToDouble(参数1), Convert.ToDouble(参数2)).ToString();
                            计算过程 += "=" + 计算过程.Split(new[] { '=' }, StringSplitOptions.RemoveEmptyEntries)
                                [计算过程.Split(new[] { '=' }, StringSplitOptions.RemoveEmptyEntries).Length - 1].
                                Replace($"{参数1}{元素}{参数2}", 返回结果).
                                Replace($"({参数1}){元素}{参数2}", 返回结果).
                                Replace($"{参数1}{元素}({参数2})", 返回结果).
                                Replace($"({参数1}){元素}({参数2})", 返回结果);
                            break;

                        case "D":
                            参数2 = Convert.ToDecimal(计算结果.Pop());
                            参数1 = Convert.ToDecimal(计算结果.Pop());
                            if (参数1 > 4096 || 参数2 > 65535 || 参数1 < 1 || 参数2 < -65535)
                            {
                                return $"错误：{参数1}{元素}{参数2}非法！";
                            }
                            string 骰池算式 = "";
                            for (int i = 0; i < 参数1; i++)
                            {
                                int rd = new Random(Guid.NewGuid().GetHashCode()).Next(0, Convert.ToInt32(参数2)) + 1;
                                骰池算式 += rd + "+";
                            }
                            骰池算式 = 骰池算式.TrimEnd('+');
                            返回结果 = new DataTable().Compute(骰池算式, "").ToString();
                            bool 单个结果 = true;
                            if (骰池算式.Contains("+"))//多个结果，括号括起
                            {
                                骰池算式 = "(" + 骰池算式 + ")";
                                单个结果 = false;
                            }
                            if (骰池详情 && !单个结果)//展示详情且包含多个结果
                            {
                                计算过程 += "=" + 计算过程.Split(new[] { '=' }, StringSplitOptions.RemoveEmptyEntries)
                                [计算过程.Split(new[] { '=' }, StringSplitOptions.RemoveEmptyEntries).Length - 1].
                                Replace($"{参数1}D{参数2}", $"{骰池算式}").
                                Replace($"({参数1}){元素}{参数2}", $"{骰池算式}").
                                Replace($"{参数1}{元素}({参数2})", $"{骰池算式}").
                                Replace($"({参数1}){元素}({参数2})", $"{骰池算式}")
                                + "=" + 计算过程.Split(new[] { '=' }, StringSplitOptions.RemoveEmptyEntries)
                                [计算过程.Split(new[] { '=' }, StringSplitOptions.RemoveEmptyEntries).Length - 1].
                                Replace($"{参数1}D{参数2}", 返回结果).
                                Replace($"({参数1}){元素}{参数2}", 返回结果).
                                Replace($"{参数1}{元素}({参数2})", 返回结果).
                                Replace($"({参数1}){元素}({参数2})", 返回结果);
                            }
                            else
                            {
                                计算过程 += "=" + 计算过程.Split(new[] { '=' }, StringSplitOptions.RemoveEmptyEntries)
                                [计算过程.Split(new[] { '=' }, StringSplitOptions.RemoveEmptyEntries).Length - 1].
                                Replace($"{参数1}D{参数2}", 返回结果).
                                Replace($"({参数1}){元素}{参数2}", 返回结果).
                                Replace($"{参数1}{元素}({参数2})", 返回结果).
                                Replace($"({参数1}){元素}({参数2})", 返回结果);
                            }

                            //计算过程 += $"【{参数1}D{参数2}】=({骰池算式})={返回结果};{Environment.NewLine}";
                            break;

                        case "A":
                            参数2 = Convert.ToDecimal(计算结果.Pop());//加骰值
                            参数1 = Convert.ToDecimal(计算结果.Pop());//枚数
                            if (参数2 > 999 || 参数1 < 1 || 参数2 < 2)
                            {
                                return $"错误：{参数1}{元素}{参数2}非法！";
                            }
                            if (参数1 > 4096 && 骰池详情)
                            {
                                return $"错误：{参数1}{元素}{参数2}结果太长了无法显示！";
                            }

                            bool 加骰 = true; string 加骰结果 = "{ "; int 成功数 = 0; int 加骰数 = 0; decimal 临时参数1 = 参数1;
                            if (骰池详情)
                            {
                                while (加骰)
                                {
                                    加骰结果 += "+(";
                                    if (加骰结果.Length > 5000)
                                    {
                                        return $"错误：{参数1}A{参数2}结果太长：{加骰结果}……{Environment.NewLine}" +
                                            $"={骰池成功数(Convert.ToDecimal(参数1), Convert.ToDecimal(骰池面), Convert.ToDecimal(参数2), Convert.ToDecimal(成功值))}";
                                    }
                                    加骰 = false;
                                    if (加骰数 > 临时参数1)
                                    {
                                        临时参数1 = 加骰数 - 临时参数1;
                                    }
                                    else if (加骰数 != 0)
                                    {
                                        临时参数1 = 加骰数;
                                    }
                                    for (int i = 0; i < 临时参数1; i++)
                                    {
                                        int rd = new Random(Guid.NewGuid().GetHashCode()).Next(0, Convert.ToInt32(骰池面)) + 1;
                                        加骰结果 += rd.ToString() + ",";
                                        if (rd >= 参数2)
                                        {
                                            加骰数++;
                                            加骰 = true;
                                        }
                                        if (rd >= Convert.ToDecimal(成功值))
                                        {
                                            成功数++;
                                        }
                                    }
                                    加骰结果 += ")";
                                    加骰结果 = 加骰结果.Remove(加骰结果.Length - 2, 1);
                                }
                                加骰结果 = 加骰结果.Remove(2, 1);
                                返回结果 = 成功数.ToString();

                                计算过程 += "=" + 计算过程.Split(new[] { '=' }, StringSplitOptions.RemoveEmptyEntries)
                                    [计算过程.Split(new[] { '=' }, StringSplitOptions.RemoveEmptyEntries).Length - 1].
                                    Replace($"{参数1}A{参数2}", $"{加骰结果} }}").
                                    Replace($"({参数1}){元素}{参数2}", $"{加骰结果} }}").
                                    Replace($"{参数1}{元素}({参数2})", $"{加骰结果} }}").
                                    Replace($"({参数1}){元素}({参数2})", $"{加骰结果} }}")
                                    + "=" + 计算过程.Split(new[] { '=' }, StringSplitOptions.RemoveEmptyEntries)
                                    [计算过程.Split(new[] { '=' }, StringSplitOptions.RemoveEmptyEntries).Length - 1].
                                    Replace($"{参数1}A{参数2}", 返回结果).
                                    Replace($"({参数1}){元素}{参数2}", 返回结果).
                                    Replace($"{参数1}{元素}({参数2})", 返回结果).
                                    Replace($"({参数1}){元素}({参数2})", 返回结果);
                            }
                            else
                            {
                                #region 注释
                                //while (加骰)
                                //{
                                //    加骰 = false;
                                //    if (加骰数 > 临时参数1)
                                //    {
                                //        临时参数1 = 加骰数 - 临时参数1;
                                //    }
                                //    else if (加骰数 != 0)
                                //    {
                                //        临时参数1 = 加骰数;
                                //    }
                                //    for (int i = 0; i < 临时参数1; i++)
                                //    {
                                //        if (加骰数 > 999999)
                                //        {
                                //            return $"错误：一个不可名状的内容。";
                                //        }
                                //        int rd = new Random(Guid.NewGuid().GetHashCode()).Next(0, Convert.ToInt32(骰池面)) + 1;
                                //        if (rd >= 参数2)
                                //        {
                                //            加骰数++;
                                //            加骰 = true;
                                //        }
                                //        if (rd >= Convert.ToDecimal(加骰值))
                                //        {
                                //            成功数++;
                                //        }
                                //    }
                                //}
                                #endregion

                                返回结果 = 骰池成功数(Convert.ToDecimal(参数1), Convert.ToDecimal(骰池面), Convert.ToDecimal(参数2), Convert.ToDecimal(成功值));

                                计算过程 += "=" + 计算过程.Split(new[] { '=' }, StringSplitOptions.RemoveEmptyEntries)
                                    [计算过程.Split(new[] { '=' }, StringSplitOptions.RemoveEmptyEntries).Length - 1].
                                    Replace($"{参数1}A{参数2}", 返回结果).
                                    Replace($"({参数1}){元素}{参数2}", 返回结果).
                                    Replace($"{参数1}{元素}({参数2})", 返回结果).
                                    Replace($"({参数1}){元素}({参数2})", 返回结果);
                            }
                            break;

                        //case "K":
                        //    参数2 = Convert.ToDecimal(计算结果.Pop());
                        //    参数1 = Convert.ToDecimal(计算结果.Pop());
                        //    if (参数2 < 1)
                        //    {
                        //        return $"错误：{元素}{参数2}非法！";
                        //    }
                        //    string 展开前 = 计算过程.Split(new[] { '=' }, StringSplitOptions.RemoveEmptyEntries)
                        //        [计算过程.Split(new[] { '=' }, StringSplitOptions.RemoveEmptyEntries).Length - 2];
                        //    展开前 = 数值.取中间(展开前, "(", ")");//(2+3+1+3)K3=>2+3+1+3
                        //    List<string> 中转骰池 = new List<string>(展开前.Split(new[] { '+' }, StringSplitOptions.RemoveEmptyEntries));
                        //    List<int> 取骰池 = new List<int>();
                        //    foreach (var 骰 in 中转骰池)
                        //    {
                        //        取骰池.Add(Convert.ToInt32(骰));
                        //    }
                        //    if (参数2 > 取骰池.Count)
                        //    {
                        //        return "错误：取不出这么多骰子！";
                        //    }
                        //    取骰池.Sort();
                        //    取骰池.RemoveRange(0, Convert.ToInt32(取骰池.Count - 参数2));
                        //    返回结果 = new DataTable().Compute(string.Join("+", 取骰池), "").ToString();

                        //    计算过程 += "=" + 计算过程.Split(new[] { '=' }, StringSplitOptions.RemoveEmptyEntries)
                        //            [计算过程.Split(new[] { '=' }, StringSplitOptions.RemoveEmptyEntries).Length - 1].
                        //            Replace($"{参数1}K{参数2}", 返回结果);
                        //    break;

                        //case "Q":
                        //    参数2 = Convert.ToDecimal(计算结果.Pop());
                        //    参数1 = Convert.ToDecimal(计算结果.Pop());
                        //    if (参数2 < 1)
                        //    {
                        //        return $"错误：{元素}{参数2}非法！";
                        //    }
                        //    string 展开前2 = 计算过程.Split(new[] { '=' }, StringSplitOptions.RemoveEmptyEntries)
                        //        [计算过程.Split(new[] { '=' }, StringSplitOptions.RemoveEmptyEntries).Length - 2];
                        //    展开前2 = 数值.取中间(展开前2, "(", ")");//(2+3+1+3)K3=>2+3+1+3
                        //    List<string> 中转骰池2 = new List<string>(展开前2.Split(new[] { '+' }, StringSplitOptions.RemoveEmptyEntries));
                        //    List<int> 取骰池2 = new List<int>();
                        //    foreach (var 骰 in 中转骰池2)
                        //    {
                        //        取骰池2.Add(Convert.ToInt32(骰));
                        //    }
                        //    if (参数2 > 取骰池2.Count)
                        //    {
                        //        return "错误：取不出这么多骰子！";
                        //    }
                        //    取骰池2.Sort();
                        //    取骰池2.Reverse();
                        //    取骰池2.RemoveRange(0, Convert.ToInt32(取骰池2.Count - 参数2));
                        //    返回结果 = new DataTable().Compute(string.Join("+", 取骰池2), "").ToString();

                        //    计算过程 += "=" + 计算过程.Split(new[] { '=' }, StringSplitOptions.RemoveEmptyEntries)
                        //            [计算过程.Split(new[] { '=' }, StringSplitOptions.RemoveEmptyEntries).Length - 1].
                        //            Replace($"{参数1}Q{参数2}", 返回结果);
                        //    break;

                        case "B":
                        case "P":
                            参数2 = Convert.ToDecimal(计算结果.Pop());
                            参数1 = Convert.ToDecimal(计算结果.Pop());
                            if (参数1 > 999 || 参数2 > 64 || 参数1 < 1 || 参数2 < 1)
                            {
                                return $"错误：{参数1}{元素}{参数2}非法！";
                            }
                            string 骰池 = "";
                            for (int i = 0; i < 参数2; i++)
                            {
                                骰池 += new Random(Guid.NewGuid().GetHashCode()).Next(0, 10) + " ";
                            }
                            List<string> 骰池表 = new List<string>(骰池.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries));
                            string 待替换 = (参数1 % 100).ToString();
                            string 个位数 = (待替换.Length == 1) ? 待替换 : 待替换.Substring(1, 1);
                            骰池表.Add(待替换.Substring(0, 1));//先加进来排序，一会儿处理完删掉
                            List<string> 骰池表排序 = new List<string>(骰池表);
                            骰池表排序.Sort();//0124458
                            if (元素 == "B")//88B3 = 88[奖励骰:3 6 0]=8
                            {
                                骰池表.RemoveAt(骰池表.Count - 1);//删掉加进来的这个
                                计算过程 += "=" + 计算过程.Split(new[] { '=' }, StringSplitOptions.RemoveEmptyEntries)
                                    [计算过程.Split(new[] { '=' }, StringSplitOptions.RemoveEmptyEntries).Length - 1].
                                    Replace($"{参数1}B{参数2}", $"{参数1}[奖励骰:{string.Join(",", 骰池表)}]")
                                    + "=" + 计算过程.Split(new[] { '=' }, StringSplitOptions.RemoveEmptyEntries)
                                    [计算过程.Split(new[] { '=' }, StringSplitOptions.RemoveEmptyEntries).Length - 1].
                                    Replace($"{参数1}B{参数2}", 骰池表排序[0] + 个位数);
                            }
                            else
                            {
                                骰池表.RemoveAt(骰池表.Count - 1);//删掉加进来的这个
                                骰池表排序.Reverse();//翻转
                                计算过程 += "=" + 计算过程.Split(new[] { '=' }, StringSplitOptions.RemoveEmptyEntries)
                                    [计算过程.Split(new[] { '=' }, StringSplitOptions.RemoveEmptyEntries).Length - 1].
                                    Replace($"{参数1}P{参数2}", $"{参数1}[惩罚骰:{string.Join(",", 骰池表)}]")
                                    + "=" + 计算过程.Split(new[] { '=' }, StringSplitOptions.RemoveEmptyEntries)
                                    [计算过程.Split(new[] { '=' }, StringSplitOptions.RemoveEmptyEntries).Length - 1].
                                    Replace($"{参数1}P{参数2}", 骰池表排序[0] + 个位数);
                            }
                            break;

                        default://自定义骰子
                            return "自定义运算符功能还未开放！敬请期待！";
                    }
                    计算结果.Push(返回结果);
                }
            }
            //数据.写入实体("骰点","结果",计算结果.Pop());
            return 计算过程;
        }

        static int 符号优先级(string 符号)
        {
            int 优先级 = 0;
            switch (符号)
            {
                case "+":
                case "-":
                    优先级 = 1;
                    break;

                case "*":
                case "/":
                case "%":
                case "^":
                    优先级 = 2;
                    break;

                case "D":
                case "A":
                case "B":
                case "P":
                case "K":
                case "Q":
                    优先级 = 3;
                    break;

                case ".":
                    优先级 = 4;
                    break;

                case "↑":
                case "↓":
                    优先级 = 5;
                    break;

                default:
                    break;
            }
            return 优先级;
        }

        static List<string> 中缀转后缀(List<string> 中继表达式)
        {
            Stack<string> 结果栈 = new Stack<string>();
            Stack<string> 符号栈 = new Stack<string>();
            foreach (var 元素 in 中继表达式)//读取中缀表达式
            {
                if (元素 == " ")//如果是空格就跳了
                {
                    break;
                }
                if (是纯数(元素) && !"+-.".Contains(元素))//是数字就直接推入结果栈
                {
                    结果栈.Push(元素);
                }
                else
                {
                    if (符号优先级(元素) != 0)//是预设符号
                    {
                        //不是空栈时，比较优先级，如果当前符号优先级不高于符号栈顶，则符号栈顶出栈
                        while (符号栈.Count != 0)
                        {
                            if (符号优先级(元素) <= 符号优先级(符号栈.Peek()))
                            {
                                结果栈.Push(符号栈.Pop());
                            }
                            else
                            {
                                break;
                            }

                        }
                        符号栈.Push(元素);
                    }
                    else
                    {
                        switch (元素)
                        {
                            case "(":
                                符号栈.Push(元素);
                                break;

                            case ")":
                                while (符号栈.Peek() != "(")
                                {
                                    if (符号栈.Count != 0)//左括号出栈
                                    {
                                        结果栈.Push(符号栈.Pop());
                                    }
                                }
                                符号栈.Pop();
                                break;

                            default://是用户自定义的符号
                                break;
                        }
                    }
                }
            }
            while (符号栈.Count != 0)
            {
                结果栈.Push(符号栈.Pop());
            }
            List<string> 计算表 = 结果栈.ToList();
            计算表.Reverse();
            return 计算表;
        }

        static List<string> 中缀分词补全(string 输入表达式, string 骰子面)
        {
            //提前转换KQ
            var KQ = Regex.Matches(输入表达式, @"[0-9]+[D][0-9]+[KQ][0-9]+").Cast<Match>().Select(m => m.Value).ToList();
            try
            {
                if (KQ.Count > 0)
                {
                    foreach (var item in KQ)
                    {
                        string[] 切分 = item.Split(new[] { "D", "K", "Q" }, StringSplitOptions.None);
                        List<int> 骰池 = new List<int>();
                        for (int i = 0; i < int.Parse(切分[0]); i++)
                        {
                            骰池.Add(new Random(Guid.NewGuid().GetHashCode()).Next(1, Convert.ToInt32(切分[1]) + 1));
                        }
                        骰池.Sort();
                        if (item.Contains("Q"))
                        {
                            骰池.Reverse();
                        }
                        骰池.RemoveRange(0, 骰池.Count - int.Parse(切分[2]));
                        string 结果 = string.Join("+", 骰池);
                        输入表达式 = 输入表达式.Replace(item, "(" + 结果 + ")");
                    }
                }
            }
            catch (Exception)
            {
                return null;
            }

            char 文字类型 = '数'; string 预处理结果 = "";
            foreach (var 字符 in 输入表达式)
            {
                if (字符 != ' ')
                {
                    if (是数字(字符))
                    {
                        if (文字类型 == '符')
                        {
                            文字类型 = '数';
                            预处理结果 += ' ';
                        }
                    }
                    else
                    {
                        //if (文字类型 == '数')//取消了多位符号
                        {
                            文字类型 = '符';
                            预处理结果 += ' ';
                        }
                    }
                }
                if (字符 == '(' || 字符 == ')')//必须让括号前后都留空
                {
                    预处理结果 += ' ' + 字符.ToString() + ' ';
                }
                else
                {
                    预处理结果 += 字符;
                }
            }
            List<string> 中继表达式 =
                new Regex("[\\s]+").Replace(预处理结果, " ").Trim().Split(' ').ToList();//合并复数空格，去除首末空格
            int 遍历位置 = -1; List<string> 临时修改中继表达式 = new List<string>(中继表达式);
            //判断-号是否是负号，如果是负号则在-号前插入0；
            //判断D和A前是否为数字，如果不是，且不是括号，则在前面加1；
            //判断D后是否为数字，如果不是，且不是括号，则在前面加骰子面；
            //判断BP后是否为数字，如果不是，且不是括号，则在前面加1；
            foreach (var 元素 in 中继表达式)
            {
                遍历位置++;
                if (元素 == "-")
                {
                    if (遍历位置 == 0)
                    {
                        临时修改中继表达式.Insert(0, "0");
                        遍历位置++;
                    }
                    //else if (!是纯数(临时修改中继表达式[遍历位置 - 1]) || "+-".Contains(临时修改中继表达式[遍历位置 - 1]))
                    //{
                    //    if (!"()".Contains(临时修改中继表达式[遍历位置 - 1]))
                    //    {
                    //        临时修改中继表达式.Insert(遍历位置, "0");
                    //        遍历位置++;
                    //    }
                    //}
                }

                if ("DA".Contains(元素))
                {
                    if (遍历位置 == 0)
                    {
                        if (元素 == "D")
                        {
                            临时修改中继表达式.Insert(0, "1");
                        }
                        else
                        {
                            临时修改中继表达式.Insert(0, "10");
                        }
                        遍历位置++;
                    }
                    else if (!是纯数(临时修改中继表达式[遍历位置 - 1]) || "+-".Contains(临时修改中继表达式[遍历位置 - 1]))
                    {
                        if (!"()".Contains(临时修改中继表达式[遍历位置 - 1]))
                        {
                            临时修改中继表达式.Insert(遍历位置, "1");
                            遍历位置++;
                        }
                        //临时修改中继表达式.Insert(遍历位置, "1");
                    }
                    if (元素 == "D")
                    {
                        if (临时修改中继表达式.Count == 遍历位置 + 1)
                        {
                            临时修改中继表达式.Insert(遍历位置 + 1, 骰子面);
                            遍历位置++;
                        }
                        else if (!是纯数(临时修改中继表达式[遍历位置 + 1]) || "+-".Contains(临时修改中继表达式[遍历位置 + 1]))
                        {
                            if (!"()".Contains(临时修改中继表达式[遍历位置 + 1]))
                            {
                                临时修改中继表达式.Insert(遍历位置 + 1, 骰子面);
                                遍历位置++;
                            }
                            //遍历位置++;
                        }
                    }
                }
                if ("BP".Contains(元素))
                {
                    if (临时修改中继表达式.Count == 遍历位置 + 1)
                    {
                        临时修改中继表达式.Insert(遍历位置 + 1, "1");
                        遍历位置++;
                    }
                    else if (!是纯数(临时修改中继表达式[遍历位置 + 1]) || "+-".Contains(临时修改中继表达式[遍历位置 + 1]))
                    {
                        if (!"()".Contains(临时修改中继表达式[遍历位置 - 1]))
                        {
                            临时修改中继表达式.Insert(遍历位置 + 1, "1");
                            遍历位置++;
                        }
                        //临时修改中继表达式.Insert(遍历位置 + 1, "1");
                        //遍历位置++;
                    }
                }
            }
            return 临时修改中继表达式;
        }

        static bool 是数字(char 字)
        {
            if ('0' <= 字 && 字 <= '9')
            {
                return true;
            }
            return false;
        }

        static bool 是符号(char 符号)
        {
            switch (符号)
            {
                case '+':
                case '-':
                case '*':
                case '/':
                case '^':
                case '%':
                case 'D':
                case 'A':
                case 'B':
                case 'P':
                case 'K':
                case 'Q':
                case '(':
                case ')':
                case '.':
                    return true;

                default:
                    return false;
            }
        }

        public static bool 是纯数(string str)
        {
            str = str.Replace(".", "0").Replace("-", "0").Replace("+", "0");
            if (str == null || str.Length == 0)    //验证这个参数是否为空
                return false;                           //是，就返回False
            ASCIIEncoding ascii = new ASCIIEncoding();//new ASCIIEncoding 的实例
            byte[] bytestr = ascii.GetBytes(str);         //把string类型的参数保存到数组里

            foreach (byte c in bytestr)                   //遍历这个数组里的内容
            {
                if (c < 48 || c > 57)                          //判断是否为数字
                {
                    return false;                              //不是，就返回False
                }
            }
            return true;                                        //是，就返回True
        }

        public static bool 插二(string 算式, string 比较符)
        {
            if (算式.IndexOf("的") > 算式.IndexOf(比较符))//比较符在"的"前
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        ///  投掷【20】枚【10】面骰子，其中大于【9】的加骰，计算骰池中大于等于【8】的值
        /// </summary>
        /// <param name="枚数"></param>
        /// <param name="面数"></param>
        /// <param name="加骰值"></param>
        /// <param name="成功值"></param>
        /// <returns></returns>
        static string 骰池成功数(decimal 枚数, decimal 面数, decimal 加骰值, decimal 成功值)
        {
            decimal 成功率 = 1 - ((成功值 - 1) / 面数);
            if (成功率 == 0)
            {
                return "0";
            }

            decimal 触发率 = 1 - ((加骰值 - 1) / 面数);
            if (触发率 == 1)
            {
                return "∞";
            }
            if (触发率 == 0)
            {
                return 枚数.ToString();
            }

            decimal 触发率偏移 = Convert.ToDecimal(Math.Sin(new Random(Guid.NewGuid().GetHashCode()).Next(0, 900) / 100d) + 0.1d);//sin(0.00~0.89)+0.1
            decimal 偏移修正 = Convert.ToDecimal(Math.Pow(new Random(Guid.NewGuid().GetHashCode()).Next(90, 200) / 100d, 1.5d));//(0.80~1.99)^1.5
            触发率 *= 触发率偏移 / 偏移修正;

            decimal 骰池大概数量 = 枚数;
            骰池大概数量 += Math.Floor(骰池大概数量 * 触发率);
            if (成功率 > 0.99m)
            {
                return 骰池大概数量.ToString();
            }
            成功率 += new Random(Guid.NewGuid().GetHashCode()).Next(-10, 16) / 100m;//-0.10~+0.15
            if (成功率 > 0.99m)
            {
                return 骰池大概数量.ToString();
            }
            if (成功率 < 0.02m)
            {
                return "0";
            }
            int 最后修正 = new Random(Guid.NewGuid().GetHashCode()).Next(1, 111) / 100;//0~+1
            return (Math.Floor(骰池大概数量 * 成功率) + 最后修正).ToString();
        }

        public static string 转阿拉伯数字(string 字符串)
        {
            List<string> 数字组 = 字符串.Split(new[] { "加", "减", "乘", "除", "的平方", "的立方", "的根", "的开方" }, StringSplitOptions.RemoveEmptyEntries).ToList();
            foreach (var 数字 in 数字组)
            {
                string 阿拉伯数字 = "(" + 数字 + ")";
                if (阿拉伯数字.StartsWith("十"))
                {
                    阿拉伯数字 = "1" + 阿拉伯数字;
                }
                阿拉伯数字 = 阿拉伯数字.Replace("零", "0").Replace("一", "1").Replace("二", "2").Replace("两", "2").Replace("三", "3")
                    .Replace("四", "4").Replace("五", "5").Replace("六", "6").Replace("七", "7").Replace("八", "8").Replace("九", "9")
                    .Replace("十", "*10+").Replace("百", "*100+").Replace("千", "*1000+").Replace("万", "*10000+").Replace("亿", "*100000000+").Replace("点", "+0.");
                阿拉伯数字 = 阿拉伯数字.Replace("+*", "*").Replace("++", "+").TrimEnd('+');
                字符串 = 字符串.Replace(数字, 阿拉伯数字);
            }
            字符串 = 字符串.Replace("加", "+").Replace("减", "-").Replace("乘", "*").Replace("除", "/")
                .Replace("的平方", "^2").Replace("的立方", "^3").Replace("的根", "^(1/2)").Replace("的开方", "^(1/2)");
            int i = 0;
            while (i < 字符串.Length)
            {
                if ("0123456789+-*/^%.()（）DABPKQdabpkq".Contains(字符串[i]))
                {
                    i++;
                }
                else
                {
                    字符串 = 字符串.Remove(i, 1);
                }
            }
            return 字符串;
        }
    }
}
