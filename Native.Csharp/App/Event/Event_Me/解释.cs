using System;
using System.Collections.Generic;

namespace Native.Csharp.App.Event.Event_Me
{
    public class 解释
    {
        public static void 语法分析(string 用户输入, long 发送目标)
        {
            List<string> 语句集 = new List<string>(用户输入.Split(new[] { '。' }, StringSplitOptions.RemoveEmptyEntries));
            foreach (var 语句 in 语句集)
            {
                string 执行结果 = 执行(语句);
                if (执行结果 != "")
                {
                    if (数据.私聊)
                    {
                        Common.CqApi.SendPrivateMessage(发送目标, 执行结果);
                    }
                    else
                    {
                        Common.CqApi.SendGroupMessage(发送目标, 执行结果);
                    }
                }
            }
        }

        public static string 执行(string 语句)
        {
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


            return $"不理解“{语句}”。";
        }
    }
}
