using System;
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

            if (算式.IndexOf("的") == 算式.LastIndexOf("的"))//如果只有一个"的"
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
                    int 位置 = 数据.反义词典.IndexOf(数据.实体[分词[0]][分词[1]]);
                    if (位置 % 2 == 0)
                    {
                        数据.实体[分词[0]][分词[1]] = 数据.反义词典[位置 + 1];
                    }
                    else
                    {
                        数据.实体[分词[0]][分词[1]] = 数据.反义词典[位置 - 1];
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
            算式 = 算式.Replace("×", "*").Replace("x", "*").Replace("X", "*")
                .Replace("（", "(").Replace("）", ")").Replace("÷", "/");
            //此处应替换为波兰逆运算
            return new DataTable().Compute(算式.Trim(), "").ToString();
        }

        public static string 骰子(string 表达式)
        {
            if (!表达式.ToLower().StartsWith("r") && !表达式.ToLower().StartsWith("w"))
            {
                表达式 = "R" + 表达式;
            }
            string 描述 = "";
            bool 展示详情 = true;
            if (表达式.Contains(" "))
            {
                描述 = 表达式.Substring(表达式.IndexOf(" "));
                表达式 = 表达式.Substring(0, 表达式.IndexOf(" "));
            }
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
                    }
                }
            }

            //提供默认值
            string 骰子面 = "100"; string 骰池面 = "10"; string 加骰值 = "8";
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
                加骰值 = 数据.读取组件("我的加骰值");
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
            List<string> 中缀分词 = 中缀分词补全(表达式, 骰子面); ;
            if (数据.私聊)
            {
                返回值 += "你骰出了";
            }
            else
            {
                Sdk.Cqp.Model.GroupMember 群友 = 数据.娶群友(数据.群聊目标.FromQQ);
                string 昵称 = string.IsNullOrWhiteSpace(群友.Card)
                    ? 群友.Nick : 群友.Card;//取群名片/QQ昵称
                if (数据.实体.ContainsKey(数据.群聊目标.FromQQ.ToString()))//取玩家设置昵称
                {
                    if (数据.实体[数据.群聊目标.FromQQ.ToString()].ContainsKey("昵称"))
                    {
                        昵称 = 数据.实体[数据.群聊目标.FromQQ.ToString()]["昵称"];
                    }
                }
                返回值 += $"{昵称}骰出了";
            }
            string 计算结果 = 后缀计算(中缀转后缀(中缀分词), string.Join("", 中缀分词.ToArray()), 骰池面, 加骰值, 展示详情);
            if (计算结果.StartsWith("错误"))
            {
                return 计算结果;
            }
            if (计算结果.Split(new[] { '=' }, StringSplitOptions.RemoveEmptyEntries)[计算结果.Split(new[] { '=' }, StringSplitOptions.RemoveEmptyEntries).Length - 1] ==
                计算(计算结果.Split(new[] { '=' }, StringSplitOptions.RemoveEmptyEntries)[计算结果.Split(new[] { '=' }, StringSplitOptions.RemoveEmptyEntries).Length - 1]))
            {
                数据.写入实体("上次", "骰点", 计算结果.Split(new[] { '=' }, StringSplitOptions.RemoveEmptyEntries)[计算结果.Split(new[] { '=' }, StringSplitOptions.RemoveEmptyEntries).Length - 1]);
                return 返回值 + 计算结果;
            }
            数据.写入实体("上次", "骰点", 计算(计算结果.Split(new[] { '=' }, StringSplitOptions.RemoveEmptyEntries)[计算结果.Split(new[] { '=' }, StringSplitOptions.RemoveEmptyEntries).Length - 1]));
            return 返回值 + 计算结果 + "=" +
                计算(计算结果.Split(new[] { '=' }, StringSplitOptions.RemoveEmptyEntries)[计算结果.Split(new[] { '=' }, StringSplitOptions.RemoveEmptyEntries).Length - 1]);

        }

        public static string 后缀计算(List<string> 计算表, string 计算过程, string 骰池面, string 加骰值, bool 骰池详情)
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
                        case "+":
                        case "-":
                        case "*":
                        case "/":
                        case "%":
                        case ".":
                            参数2 = Convert.ToDecimal(计算结果.Pop());
                            参数1 = Convert.ToDecimal(计算结果.Pop());
                            返回结果 = new DataTable().Compute($"{参数1}{元素}{参数2}", "").ToString();
                            break;

                        case "^":
                            返回结果 = Math.Pow(Convert.ToDouble(参数1), Convert.ToDouble(参数2)).ToString();
                            break;

                        case "D":
                            参数2 = Convert.ToDecimal(计算结果.Pop());
                            参数1 = Convert.ToDecimal(计算结果.Pop());
                            if (参数1 > 999 || 参数2 > 65535 || 参数1 < 1 || 参数2 < -65535)
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
                                Replace($"{参数1}D{参数2}", $"{骰池算式}")
                                + "=" + 计算过程.Split(new[] { '=' }, StringSplitOptions.RemoveEmptyEntries)
                                [计算过程.Split(new[] { '=' }, StringSplitOptions.RemoveEmptyEntries).Length - 1].
                                Replace($"{参数1}D{参数2}", 返回结果);
                            }
                            else
                            {
                                计算过程 += "=" + 计算过程.Split(new[] { '=' }, StringSplitOptions.RemoveEmptyEntries)
                                [计算过程.Split(new[] { '=' }, StringSplitOptions.RemoveEmptyEntries).Length - 1].
                                Replace($"{参数1}D{参数2}", 返回结果);
                            }
                            
                            //计算过程 += $"【{参数1}D{参数2}】=({骰池算式})={返回结果};{Environment.NewLine}";
                            break;

                        case "A":
                            参数2 = Convert.ToDecimal(计算结果.Pop());
                            参数1 = Convert.ToDecimal(计算结果.Pop());
                            if (参数1 > 999 || 参数2 > 99 || 参数1 < 1 || 参数2 < 1)
                            {
                                return $"错误：{参数1}{元素}{参数2}非法！";
                            }

                            bool 加骰 = true; string 加骰结果 = "{ "; int 成功数 = 0; int 加骰数 = 0; decimal 临时参数1 = 参数1;
                            while (加骰)
                            {
                                加骰结果 += "+(";
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
                                    if (加骰数 > 999)
                                    {
                                        return $"错误：一个不可名状的内容。";
                                    }
                                    int rd = new Random(Guid.NewGuid().GetHashCode()).Next(0, Convert.ToInt32(骰池面)) + 1;
                                    加骰结果 += rd.ToString() + ",";
                                    if (rd >= 参数2)
                                    {
                                        加骰数++;
                                        加骰 = true;
                                    }
                                    if (rd >= Convert.ToDecimal(加骰值))
                                    {
                                        成功数++;
                                    }
                                }
                                加骰结果 += ")";
                                加骰结果 = 加骰结果.Remove(加骰结果.Length - 2, 1);
                            }
                            加骰结果 = 加骰结果.Remove(2,1);
                            返回结果 = 成功数.ToString();

                            if (骰池详情)
                            {
                                计算过程 += "=" + 计算过程.Split(new[] { '=' }, StringSplitOptions.RemoveEmptyEntries)
                                    [计算过程.Split(new[] { '=' }, StringSplitOptions.RemoveEmptyEntries).Length - 1].
                                    Replace($"{参数1}A{参数2}", $"{加骰结果} }}")
                                    + "=" + 计算过程.Split(new[] { '=' }, StringSplitOptions.RemoveEmptyEntries)
                                    [计算过程.Split(new[] { '=' }, StringSplitOptions.RemoveEmptyEntries).Length - 1].
                                    Replace($"{参数1}A{参数2}", 返回结果);
                            }
                            else
                            {
                                计算过程 += "=" + 计算过程.Split(new[] { '=' }, StringSplitOptions.RemoveEmptyEntries)
                                    [计算过程.Split(new[] { '=' }, StringSplitOptions.RemoveEmptyEntries).Length - 1].
                                    Replace($"{参数1}A{参数2}", 返回结果);
                            }
                            break;

                        case "K":
                            参数2 = Convert.ToDecimal(计算结果.Pop());
                            参数1 = Convert.ToDecimal(计算结果.Pop());
                            if (参数2 < 1)
                            {
                                return $"错误：{元素}{参数2}非法！";
                            }
                            string 展开前 = 计算过程.Split(new[] { '=' }, StringSplitOptions.RemoveEmptyEntries)
                                [计算过程.Split(new[] { '=' }, StringSplitOptions.RemoveEmptyEntries).Length - 2];
                            展开前 = 数值.取中间(展开前, "(", ")");//(2+3+1+3)K3=>2+3+1+3
                            List<string> 中转骰池 = new List<string>(展开前.Split(new[] { '+' }, StringSplitOptions.RemoveEmptyEntries));
                            List<int> 取骰池 = new List<int>();
                            foreach (var 骰 in 中转骰池)
                            {
                                取骰池.Add(Convert.ToInt32(骰));
                            }
                            if (参数2 > 取骰池.Count)
                            {
                                return "错误：取不出这么多骰子！";
                            }
                            取骰池.Sort();
                            取骰池.RemoveRange(0, Convert.ToInt32(取骰池.Count - 参数2));
                            返回结果 = new DataTable().Compute(string.Join("+", 取骰池),"").ToString();

                            计算过程 += "=" + 计算过程.Split(new[] { '=' }, StringSplitOptions.RemoveEmptyEntries)
                                    [计算过程.Split(new[] { '=' }, StringSplitOptions.RemoveEmptyEntries).Length - 1].
                                    Replace($"{参数1}K{参数2}", 返回结果);
                            break;

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
                                骰池 += new Random(Guid.NewGuid().GetHashCode()).Next(0, Convert.ToInt32(10)) + " ";
                            }
                            List<string> 骰池表 = new List<string>(骰池.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries));
                            decimal 换位结果 = 0;
                            string 待替换 = (参数1 % 100).ToString();
                            string 个位数 = (待替换.Length == 1) ? 待替换 : 待替换.Substring(1,1);
                            骰池表.Add(待替换.Substring(0, 1));//先加进来排序，一会儿处理完删掉
                            List<string> 骰池表排序 = new List<string>(骰池表);
                            骰池表排序.Sort();//0124458
                            if (元素 == "B")//88B3 = 88[奖励骰:3 6 0]=8
                            {
                                骰池表.RemoveAt(骰池表.Count - 1);//删掉加进来的这个
                                计算过程 += "=" + 计算过程.Split(new[] { '=' }, StringSplitOptions.RemoveEmptyEntries)
                                    [计算过程.Split(new[] { '=' }, StringSplitOptions.RemoveEmptyEntries).Length - 1].
                                    Replace($"{参数1}B{参数2}", $"{参数1}[奖励骰:{string.Join(" ", 骰池表)}]")
                                    + "=" + 计算过程.Split(new[] { '=' }, StringSplitOptions.RemoveEmptyEntries)
                                    [计算过程.Split(new[] { '=' }, StringSplitOptions.RemoveEmptyEntries).Length - 1].
                                    Replace($"{参数1}B{参数2}", 骰池表排序[0]+个位数);
                            }
                            else
                            {
                                骰池表.RemoveAt(骰池表.Count - 1);//删掉加进来的这个
                                骰池表排序.Reverse();//翻转
                                计算过程 += "=" + 计算过程.Split(new[] { '=' }, StringSplitOptions.RemoveEmptyEntries)
                                    [计算过程.Split(new[] { '=' }, StringSplitOptions.RemoveEmptyEntries).Length - 1].
                                    Replace($"{参数1}P{参数2}", $"{参数1}[惩罚骰:{string.Join(" ", 骰池表)}]")
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
                    优先级 = 3;
                    break;

                case ".":
                    优先级 = 4;
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
                if (是纯数(元素) && !"+-".Contains(元素))//是数字就直接推入结果栈
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
                                //if (符号栈.Peek() == "=")//上一个符号是=时，这里应是用户定义的骰子面
                                //{
                                //    结果栈.Push("面:" + 元素);
                                //}
                                //else//否则检查是否是自定义骰子
                                //{
                                //    if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + @"SaveDir\" + 元素 + ".ini"))
                                //    {
                                //        符号栈.Push("骰:" + 元素);
                                //    }
                                //    else//不存在的话报错返回
                                //    {
                                //        return $"错误：{元素}不是自定义骰子！";
                                //    }
                                //}
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
            //判断D和A前是否为数字，如果不是，则在前面加1；
            //判断D后是否为数字，如果不是，则在前面加骰子面；
            //判断P后是否为数字，如果不是，则在前面加1；
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
                    else if (!是纯数(临时修改中继表达式[遍历位置 - 1]) || "+-".Contains(临时修改中继表达式[遍历位置 - 1]))
                    {
                        临时修改中继表达式.Insert(遍历位置, "0");
                        遍历位置++;
                    }
                }

                if ("DA".Contains(元素))
                {
                    if (遍历位置 == 0)
                    {
                        临时修改中继表达式.Insert(0, "1");
                        遍历位置++;
                    }
                    else if (!是纯数(临时修改中继表达式[遍历位置 - 1]) || "+-".Contains(临时修改中继表达式[遍历位置 - 1]))
                    {
                        临时修改中继表达式.Insert(遍历位置, "1");
                        遍历位置++;
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
                            临时修改中继表达式.Insert(遍历位置 + 1, 骰子面);
                            遍历位置++;
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
                        临时修改中继表达式.Insert(遍历位置 + 1, "1");
                        遍历位置++;
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
                case 'D':
                case 'A':
                case 'B':
                case 'P':
                case 'K':
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

        //public static void 等式运算(string 算式)
        //{

        //    return;
        //}
    }
}
