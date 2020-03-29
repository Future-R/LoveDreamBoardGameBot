using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Native.Csharp.App.Event.Event_Me
{
    public class 解释
    {
        public static void 语法分析(string 用户输入)
        {
            if (用户输入 == "帮助" || 用户输入.ToLower() == "help")
            {
                Common.CqApi.SendPrivateMessage(数据.私聊目标.FromQQ, 数据.帮助);
                return;
            }
            if (用户输入 == "开发模式")
            {
                数据.开发模式 = true;
                return;
            }
            if (用户输入 == "用户模式")
            {
                数据.开发模式 = false;
                return;
            }
            if (!数据.私聊)
            {
                if (用户输入.StartsWith("开启"))
                {
                    if (用户输入.Length == 2)
                    {
                        操作.开关(数据.群聊目标.FromGroup, false);
                    }
                    else if (用户输入.Substring(2).Trim() == Common.CqApi.GetLoginQQ().ToString() ||
                        用户输入.Substring(2).Trim() == Common.CqApi.GetLoginQQ().ToString().Substring(Common.CqApi.GetLoginQQ().ToString().Length - 4, 4))
                    {
                        操作.开关(数据.群聊目标.FromGroup, false);
                    }
                    return;
                }
                if (用户输入.StartsWith("关闭"))
                {
                    if (用户输入.Length == 2)
                    {
                        操作.开关(数据.群聊目标.FromGroup, true);
                    }
                    else if (用户输入.Substring(2).Trim() == Common.CqApi.GetLoginQQ().ToString() ||
                        用户输入.Substring(2).Trim() == Common.CqApi.GetLoginQQ().ToString().Substring(Common.CqApi.GetLoginQQ().ToString().Length - 4, 4))
                    {
                        操作.开关(数据.群聊目标.FromGroup, true);
                    }
                    return;
                }
                if (用户输入.StartsWith("退群"))
                {
                    if (用户输入.Length == 2)
                    {
                        if (数据.娶群友(数据.私聊目标.FromQQ).PermitType == Sdk.Cqp.Enum.PermitType.None)
                        {
                            Common.CqApi.SendGroupMessage(数据.群聊目标.FromGroup, 数据.报错 + "需要黄帽或绿帽！");
                        }
                        else
                        {
                            Common.CqApi.SetGroupExit(数据.群聊目标.FromGroup);
                        }
                    }
                    else if (用户输入.Substring(2).Trim() == Common.CqApi.GetLoginQQ().ToString() ||
                        用户输入.Substring(2).Trim() == Common.CqApi.GetLoginQQ().ToString().Substring(Common.CqApi.GetLoginQQ().ToString().Length - 4, 4))
                    {
                        if (数据.娶群友(数据.私聊目标.FromQQ).PermitType == Sdk.Cqp.Enum.PermitType.None)
                        {
                            Common.CqApi.SendGroupMessage(数据.群聊目标.FromGroup, 数据.报错 + "需要黄帽或绿帽！");
                        }
                        else
                        {
                            Common.CqApi.SetGroupExit(数据.群聊目标.FromGroup);
                        }
                    }
                    return;
                }
            }

            if (!数据.私聊 && 操作.机器人开关.Contains(数据.群聊目标.FromGroup))
            {
                return;
            }
            用户输入 = 转义.输入(用户输入);
            List<string> 语句集 = new List<string>();
            if (用户输入.StartsWith("！"))
            {
                语句集 = new List<string>(用户输入.Substring(1).Split(new[] { '！' }, StringSplitOptions.RemoveEmptyEntries));
                数据.转义 = false;
            }
            else
            {
                语句集 = new List<string>(用户输入.Split(new[] { '。' }, StringSplitOptions.RemoveEmptyEntries));
                数据.转义 = true;
            }
            数据.写入实体("输入", "段落", 用户输入);
            List<string> 临时语句集 = new List<string>();
            语句集.ForEach((语句) =>
           {
               临时语句集.Add(语句.Trim());
           });
            语句集 = 临时语句集;
            for (int 序号 = 0; 序号 < 语句集.Count; 序号++)
            {
                string 语句 = 语句集[序号];
                if (!语句.StartsWith("页码："))
                {
                    string 执行结果 = 执行(语句);
                    if (执行结果.StartsWith("跳转到页码") && 执行结果.Length > 5)
                    {
                        string 页码 = 执行结果.Substring(5);
                        if (语句集.Contains("页码：" + 页码))
                        {
                            int 坐标 = 语句集.FindIndex((string 匹配) => 匹配.Equals("页码：" + 页码));
                            if (坐标 == 语句集.Count - 1)
                            {
                                return;
                            }
                            序号 = 坐标;
                        }
                        else
                        {
                            Common.CqApi.SendPrivateMessage(数据.私聊目标.FromQQ, $"{数据.报错}{Environment.NewLine}找不到页码{页码}！");
                            return;
                        }
                        执行结果 = "";
                    }
                    else 操作.发送(执行结果);
                }
            }
        }

        public static string 执行(string 语句)
        {
            数据.临时空间 = string.Empty;
            数据.写入实体("输入", "语句", 语句);

            if (数据.转义)
            {
                语句 = 变量解释(语句);
                语句 += 数据.临时空间;
            }

            if (数据.循环次数 <= 65536)
            {
                数据.循环次数++;
                //关键字打头
                if (语句.StartsWith("翻开") && 语句.Length > 2)
                {
                    return $"跳转到页码{语句.Substring(2)}";
                }
                if (语句.StartsWith("设"))
                {
                    if (语句.Contains("分别为"))
                    {
                        语句 = 语句.Substring(1);
                        if (!语句.Contains("的")) //在"为"之前补全"的值"
                        {
                            语句 = 语句.Insert(语句.IndexOf("为"), "的值");
                        }
                        //十的级别、百的级别、千的级别、万的级别、亿的级别分别是1、2、3、4、8。
                        //十、百、千、万、亿的级别分别是1、2、3、4、8。
                        string 左侧 = 语句.Substring(0, 语句.IndexOf("分别为"));
                        string 右侧 = 语句.Substring(语句.IndexOf("分别为") + 3);
                        string 推测属性 = "值";
                        List<string> 左侧集合 = 左侧.Split('、').ToList();
                        List<string> 右侧集合 = 右侧.Split('、').ToList();
                        if (左侧集合.Count > 右侧集合.Count)
                        {
                            return $"怎么把{右侧集合.Count}个糖果平均分给{左侧集合.Count}个小朋友？";
                        }
                        if (左侧集合[左侧集合.Count - 1].Contains("的"))
                        {
                            推测属性 = 左侧集合[左侧集合.Count - 1].Substring(左侧集合[左侧集合.Count - 1].IndexOf("的") + 1);
                        }
                        try
                        {
                            for (int i = 0; i < 左侧集合.Count; i++)
                            {
                                if (!左侧集合[i].Contains("的"))
                                {
                                    if (左侧集合[i] == "我")
                                    {
                                        左侧集合[i] = 数据.私聊目标.FromQQ.ToString();
                                    }
                                    数据.写入实体(左侧集合[i], 推测属性, 右侧集合[i]);
                                }
                                else
                                {
                                    if (左侧集合[i].Substring(0, 左侧集合[i].IndexOf("的")) == "我")
                                    {
                                        左侧集合[i] = 数据.私聊目标.FromQQ.ToString() + "的" + 左侧集合[i].Substring(左侧集合[i].IndexOf("的") + 1);
                                    }
                                    数据.写入实体(左侧集合[i].Substring(0, 左侧集合[i].IndexOf("的")), 左侧集合[i].Substring(左侧集合[i].IndexOf("的") + 1), 右侧集合[i]);
                                }
                            }
                        }
                        catch (Exception)
                        {
                            return $"{数据.报错}{Environment.NewLine}{数据.实体["输入"]["语句"]}操作失败！";
                        }
                        if (语句.Substring(0, 语句.IndexOf("的")) == "我")
                        {
                            数据.写入实体(数据.私聊目标.FromQQ.ToString(), 数值.取中间(语句, "的", "为"), 语句.Substring(语句.IndexOf("为") + 1));
                        }
                        else
                        {
                            数据.写入实体(语句.Substring(0, 语句.IndexOf("的")), 数值.取中间(语句, "的", "为"), 语句.Substring(语句.IndexOf("为") + 1));
                        }
                        return "";
                    }
                    else if (语句.Contains("为"))
                    {
                        语句 = 语句.Substring(1);
                        if (!语句.Contains("的")) //在"为"之前补全"的值"
                        {
                            语句 = 语句.Insert(语句.IndexOf("为"), "的值");
                        }
                        if (语句.Substring(0, 语句.IndexOf("的")) == "我")
                        {
                            数据.写入实体(数据.私聊目标.FromQQ.ToString(), 数值.取中间(语句, "的", "为"), 语句.Substring(语句.IndexOf("为") + 1));
                        }
                        else
                        {
                            数据.写入实体(语句.Substring(0, 语句.IndexOf("的")), 数值.取中间(语句, "的", "为"), 语句.Substring(语句.IndexOf("为") + 1));
                        }
                        return "";
                    }
                }
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
                        var 新对象 = 对象;
                        if (新对象 == "我")
                        {
                            新对象 = 数据.私聊目标.FromQQ.ToString();
                        }
                        if (数据.实体.ContainsKey(新对象))
                        {
                            打印日志 += $"【{新对象}】\r\n";
                            foreach (var item in 数据.实体[新对象].Keys)
                            {
                                打印日志 += $"{item}：{数据.实体[新对象][item]}\r\n";
                            }
                        }
                        else
                        {
                            打印日志 += $"没找到{新对象}。\r\n";
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
                    数据.写入实体("计算", "结果", 计算结果);
                    return 计算结果;
                }

                #endregion
                #region 如果
                if (语句.StartsWith("如果"))
                {
                    string 判别式 = 语句.Substring(2, 语句.IndexOf("：") - 2);
                    List<string> 语句集 = new List<string>(
                            语句.Substring(语句.IndexOf("：") + 1)
                            .Split(new[] { '；' }, StringSplitOptions.RemoveEmptyEntries));

                    if (判定(判别式))
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
                                if (数据.群聊目标 == null)
                                {
                                    Common.CqApi.SendDiscussMessage(数据.讨论组目标.FromDiscuss, 转义.输出(执行结果));
                                }
                                else if (数据.私聊)
                                {
                                    Common.CqApi.SendPrivateMessage(数据.私聊目标.FromQQ, 转义.输出(执行结果));
                                }
                                else
                                {
                                    Common.CqApi.SendGroupMessage(数据.群聊目标.FromGroup, 转义.输出(执行结果));
                                }
                            }
                        }
                    }
                    return "";
                }
                #endregion
                #region 遍历
                if (语句.StartsWith("遍历"))
                {
                    if (语句.Length > 2)
                    {
                        string 遍历目标 = 语句.Substring(2, 语句.IndexOf("：") - 2);
                        List<string> 元素集合 = 集合.静态集合生成(遍历目标);
                        var 输入 = 语句.Substring(语句.IndexOf("：") + 1).Split(new[] { '；' }, StringSplitOptions.RemoveEmptyEntries);
                        //List<string> 语句集 = new List<string>(
                        //    语句.Substring(语句.IndexOf("：") + 1)
                        //    .Split(new[] { '；' }, StringSplitOptions.RemoveEmptyEntries));
                        List<string> debug = 输入.ToList();
                        List<string> 语句集 = 输入.ToList();

                        try
                        {
                            if (数据.循环次数 <= 65536)
                            {
                                //for (int i = 0; i < debug.Count; i++)
                                //{
                                //    数据.写入实体("元素", "值", 元素集合[i]);
                                //    操作.发送(执行(debug[i].Trim()));
                                //}
                                foreach (var 元素 in 元素集合)
                                {
                                    数据.写入实体("元素", "值", 元素);
                                    for (int i = 0; i < debug.Count; i++)
                                    {
                                        操作.发送(执行(debug[i].Trim()));
                                    }
                                }
                            }
                            数据.实体.Remove("元素");
                            return "";
                        }
                        catch (Exception ex)
                        {
                            数据.实体.Remove("元素");
                            return "遍历出错！";
                        }
                    }
                    else return "";
                }
                #endregion
                #region 直到
                if (语句.StartsWith("直到"))
                {
                    string 判别式 = 语句.Substring(2, 语句.IndexOf("：") - 2).Replace("我的", 数据.私聊目标.FromQQ.ToString() + "的");
                    List<string> 语句集 = new List<string>(
                            语句.Substring(语句.IndexOf("：") + 1)
                            .Split(new[] { '；' }, StringSplitOptions.RemoveEmptyEntries));

                    while (!判定(判别式) && 数据.循环次数 <= 65536)
                    {
                        foreach (var 子语句 in 语句集)
                        {
                            操作.发送(执行(子语句.Trim()));
                        }
                    }
                    return "";
                }
                #endregion
                #region 对于
                //if (语句.StartsWith("对于"))
                //{
                //    List<string> 对于语句 = new List<string>(语句.Substring(2).Split(new[] { '，' }, StringSplitOptions.RemoveEmptyEntries));
                //    if (对于语句.Count > 1)
                //    {
                //        if (对于语句[1].StartsWith("若") && 对于语句[2].StartsWith("则"))
                //        {
                //            //此处是规则写入
                //        }
                //    }
                //}
                #endregion
                #region 回复
                if (语句.StartsWith("回复"))
                {
                    return 语句.Substring(2);
                }
                #endregion
                #region 获取
                if (语句.StartsWith("获取"))
                {
                    var 结果 = 操作.超时检测(2000, () =>
                    {
                        return 转义.内部输入(JSON.获取(语句.Substring(2)));
                    });
                    数据.写入实体("获取", "结果", 结果);
                    return "";
                }
                #endregion
                #region 连接
                if (语句.StartsWith("连接"))
                {
                    try
                    {
                        if (语句.Length > 16 && 语句.Contains("操作"))
                        {
                            string 连接 = 数值.取中间(语句, "连接", "操作").Trim();
                            string 操作 = 语句.Replace("连接" + 连接, "").Trim().Substring(2);
                            return MySql.查(连接, 操作);
                        }
                        else
                        {
                            //Host=localhost;User ID=root;Password=;Port = 3306;DataBase=animals;Charset=utf8;
                            //"select *from student where name='kobe'"
                            return "格式：“。连接Host=[IP地址(必填)];User ID=[用户名];Password=[密码];Port=[端口];DataBase=[数据库名(必填)];Charset=[字符集];”(顺序不限定)操作[一条操作语句]";
                        }
                    }
                    catch (Exception ex)
                    {
                        Event_CheckError.CheckError(ex);
                    }

                }
                #endregion
                #region 骰子
                if (语句.StartsWith("骰子"))
                {
                    语句 = "r" + 语句.Substring(2);
                }
                if (语句.ToLower().StartsWith("r") || 语句.ToLower().StartsWith("w"))
                {
                    return 运算.骰子(语句);
                }

                if (语句.ToLower().StartsWith("set"))
                {
                    if (语句.Length > 3)
                    {
                        if (运算.是纯数(语句.Substring(3).Trim()) && !语句.Substring(3).Contains("."))
                        {
                            if (Convert.ToDecimal(语句.Substring(3).Trim()) > 1000000 && -1000000 > Convert.ToDecimal(语句.Substring(3).Trim()))
                            {
                                return 数据.报错 + "\n还踢球？";
                            }
                            数据.写入实体(数据.私聊目标.FromQQ.ToString(), "默认骰", 语句.Substring(3).Trim());
                            return $"默认骰设置为 {语句.Substring(3).Trim()}";
                        }
                        else
                        {
                            return 数据.报错 + "\n默认骰必须是整数！";
                        }
                    }
                    else//移除
                    {
                        数据.实体[数据.私聊目标.FromQQ.ToString()].Remove("默认骰");
                    }
                }

                if (语句.ToLower().StartsWith("dnd查询") && 语句.ToLower() != "dnd查询")
                {
                    string 参数 = 语句.Substring(5).Trim().ToLower();
                    if (数据.实体["DND3R"].ContainsKey(参数))
                    {
                        return 数据.实体["DND3R"][参数];
                    }
                    else
                    {
                        return $"找不到名为{参数}的DND3R法术/专长/物品！";
                    }
                }
                if (语句.ToLower().StartsWith("nn"))
                {
                    if (语句.Length > 2)
                    {
                        数据.写入实体(数据.私聊目标.FromQQ.ToString(), "昵称", 语句.Substring(2).Trim());
                        return $"昵称设置为{语句.Substring(2).Trim()}";
                    }
                    else//移除
                    {
                        数据.实体[数据.私聊目标.FromQQ.ToString()].Remove("昵称");
                        return "移除昵称成功！";
                    }

                }
                if (语句.ToLower().StartsWith("coc6d"))
                {
                    return 人物卡.COC6D();
                }
                if (语句.ToLower().StartsWith("coc7d") || 语句.ToLower().StartsWith("cocd"))
                {
                    return 人物卡.COC7D();
                }
                if (语句.ToLower().StartsWith("coc6"))
                {
                    if (语句.ToLower() == "coc6")
                    {
                        语句 = "COC61";
                    }
                    try
                    {
                        int 次数 = Convert.ToInt32(语句.Substring(4).Trim());
                        return 人物卡.COC6(次数);
                    }
                    catch (Exception)
                    {
                        return "错误：格式为COC [次数]";
                    }
                }
                if (语句.ToLower().StartsWith("coc"))
                {
                    if (语句.ToLower() == "coc")
                    {
                        语句 = "COC1";
                    }
                    try
                    {
                        int 次数 = Convert.ToInt32(语句.Substring(3).Trim());
                        return 人物卡.COC7(次数);
                    }
                    catch (Exception)
                    {
                        return "错误：格式为COC [次数]";
                    }
                }
                if (语句.ToLower().StartsWith("dnd"))
                {
                    if (语句.ToLower() == "dnd")
                    {
                        语句 = "DND1";
                    }
                    try
                    {
                        int 次数 = 1; string 参数 = 语句.Substring(3).Trim(); string 表达式 = "";
                        if (语句.Substring(3).Trim().Contains(" "))
                        {
                            次数 = Convert.ToInt32(参数.Substring(0, 参数.IndexOf(' ')));
                            表达式 = 参数.Substring(参数.IndexOf(' ') + 1);
                        }
                        else
                        {
                            次数 = Convert.ToInt32(参数);
                        }
                        return 人物卡.DND(次数, 表达式);
                    }
                    catch (Exception)
                    {
                        return "错误：格式为DND [次数*] [表达式*]";
                    }
                }
                if (语句.ToLower() == "ti")
                {
                    return 人物卡.症状发作(true);
                }
                if (语句.ToLower() == "li")
                {
                    return 人物卡.症状发作(false);
                }
                if (语句.ToLower() == "jrrp")
                {
                    return 人物卡.今日人品();
                }

                #endregion

                //关键字包含
                #region 赋值
                if (语句.Contains("分别是"))
                {
                    语句 = 语句.Substring(1);
                    if (!语句.Contains("的")) //在"为"之前补全"的值"
                    {
                        语句 = 语句.Insert(语句.IndexOf("是"), "的值");
                    }
                    //十的级别、百的级别、千的级别、万的级别、亿的级别分别是1、2、3、4、8。
                    //十、百、千、万、亿的级别分别是1、2、3、4、8。
                    string 左侧 = 语句.Substring(0, 语句.IndexOf("分别是"));
                    string 右侧 = 语句.Substring(语句.IndexOf("分别是") + 3);
                    string 推测属性 = "值";
                    List<string> 左侧集合 = 左侧.Split('、').ToList();
                    List<string> 右侧集合 = 右侧.Split('、').ToList();
                    if (左侧集合.Count > 右侧集合.Count)
                    {
                        return $"怎么把{右侧集合.Count}个糖果平均分给{左侧集合.Count}个小朋友？";
                    }
                    if (左侧集合[左侧集合.Count - 1].Contains("的"))
                    {
                        推测属性 = 左侧集合[左侧集合.Count - 1].Substring(左侧集合[左侧集合.Count - 1].IndexOf("的") + 1);
                    }
                    try
                    {
                        for (int i = 0; i < 左侧集合.Count; i++)
                        {
                            if (!左侧集合[i].Contains("的"))
                            {
                                if (左侧集合[i] == "我")
                                {
                                    左侧集合[i] = 数据.私聊目标.FromQQ.ToString();
                                }
                                数据.写入实体(左侧集合[i], 推测属性, 右侧集合[i]);
                            }
                            else
                            {
                                if (左侧集合[i].Substring(0, 左侧集合[i].IndexOf("的")) == "我")
                                {
                                    左侧集合[i] = 数据.私聊目标.FromQQ.ToString() + "的" + 左侧集合[i].Substring(左侧集合[i].IndexOf("的") + 1);
                                }
                                数据.写入实体(左侧集合[i].Substring(0, 左侧集合[i].IndexOf("的")), 左侧集合[i].Substring(左侧集合[i].IndexOf("的") + 1), 右侧集合[i]);
                            }
                        }
                    }
                    catch (Exception)
                    {
                        return $"{数据.报错}{Environment.NewLine}{数据.实体["输入"]["语句"]}操作失败！";
                    }
                    if (语句.Substring(0, 语句.IndexOf("的")) == "我")
                    {
                        数据.写入实体(数据.私聊目标.FromQQ.ToString(), 数值.取中间(语句, "的", "为"), 语句.Substring(语句.IndexOf("是") + 1));
                    }
                    else
                    {
                        数据.写入实体(语句.Substring(0, 语句.IndexOf("的")), 数值.取中间(语句, "的", "为"), 语句.Substring(语句.IndexOf("是") + 1));
                    }
                    return "";
                }
                else if (语句.Contains("是"))
                {
                    if (!语句.Contains("的")) //在"是"之前补全"的值"
                    {
                        语句 = 语句.Insert(语句.IndexOf("是"), "的值");
                    }
                    if (语句.Substring(0, 语句.IndexOf("的")) == "我")
                    {
                        数据.写入实体(数据.私聊目标.FromQQ.ToString(), 数值.取中间(语句, "的", "是"), 语句.Substring(语句.IndexOf("是") + 1));
                    }
                    else
                    {
                        数据.写入实体(语句.Substring(0, 语句.IndexOf("的")), 数值.取中间(语句, "的", "是"), 语句.Substring(语句.IndexOf("是") + 1));
                    }

                    return "";
                }
                #endregion
                #region 加减乘除
                if (语句.Contains("+"))
                {
                    List<string> 内容 = new List<string>();
                    if (语句.Contains("的"))
                    {
                        内容 = new List<string>(new List<string>(语句.Split(new[] { '+', '的' }, StringSplitOptions.RemoveEmptyEntries)));
                        if (内容[0] == "我")
                        {
                            内容[0] = 数据.私聊目标.FromQQ.ToString();
                        }
                        if (!数据.实体.ContainsKey(内容[0]))
                        {
                            数据.写入实体(内容[0], 内容[1], "0");
                        }
                        else if (!数据.实体[内容[0]].ContainsKey(内容[1]))
                        {
                            数据.写入实体(内容[0], 内容[1], "0");
                        }
                        内容[2] = new DataTable().Compute(数据.实体[内容[0]][内容[1]] + "+" + 内容[2], "").ToString();
                        数据.写入实体(内容[0], 内容[1], 内容[2]);
                    }
                    else//不输入组件则默认为“值”组件
                    {
                        内容 = new List<string>(语句.Split(new[] { '+' }, StringSplitOptions.RemoveEmptyEntries));
                        内容.Insert(1, "值");
                        if (!数据.实体.ContainsKey(内容[0]))
                        {
                            数据.写入实体(内容[0], 内容[1], "0");
                        }
                        else if (!数据.实体[内容[0]].ContainsKey(内容[1]))
                        {
                            数据.写入实体(内容[0], 内容[1], "0");
                        }
                        内容[2] = new DataTable().Compute(数据.实体[内容[0]][内容[1]] + "+" + 内容[2], "").ToString();
                        数据.写入实体(内容[0], 内容[1], 内容[2]);
                    }
                    if (内容[0] == 数据.私聊目标.FromQQ.ToString())
                    {
                        内容[0] = 数据.获取昵称().TrimEnd('的');
                    }
                    if (数据.开发模式)
                    {
                        return "";
                    }
                    return $"{内容[0]}的{内容[1]}={内容[2]}";
                }
                if (语句.Contains("-"))
                {
                    List<string> 内容 = new List<string>();
                    if (语句.Contains("的"))
                    {
                        内容 = new List<string>(new List<string>(语句.Split(new[] { '-', '的' }, StringSplitOptions.RemoveEmptyEntries)));
                        if (内容[0] == "我")
                        {
                            内容[0] = 数据.私聊目标.FromQQ.ToString();
                        }
                        if (!数据.实体.ContainsKey(内容[0]))
                        {
                            数据.写入实体(内容[0], 内容[1], "0");
                        }
                        else if (!数据.实体[内容[0]].ContainsKey(内容[1]))
                        {
                            数据.写入实体(内容[0], 内容[1], "0");
                        }
                        内容[2] = new DataTable().Compute(数据.实体[内容[0]][内容[1]] + "-" + 内容[2], "").ToString();
                        数据.写入实体(内容[0], 内容[1], 内容[2]);
                    }
                    else//不输入组件则默认为“值”组件
                    {
                        内容 = new List<string>(语句.Split(new[] { '-' }, StringSplitOptions.RemoveEmptyEntries));
                        内容.Insert(1, "值");
                        if (!数据.实体.ContainsKey(内容[0]))
                        {
                            数据.写入实体(内容[0], 内容[1], "0");
                        }
                        else if (!数据.实体[内容[0]].ContainsKey(内容[1]))
                        {
                            数据.写入实体(内容[0], 内容[1], "0");
                        }
                        内容[2] = new DataTable().Compute(数据.实体[内容[0]][内容[1]] + "-" + 内容[2], "").ToString();
                        数据.写入实体(内容[0], 内容[1], 内容[2]);
                    }
                    if (内容[0] == 数据.私聊目标.FromQQ.ToString())
                    {
                        内容[0] = 数据.获取昵称().TrimEnd('的');
                    }
                    if (数据.开发模式)
                    {
                        return "";
                    }
                    return $"{内容[0]}的{内容[1]}={内容[2]}";
                }
                if (语句.Contains("*"))
                {
                    List<string> 内容 = new List<string>();
                    if (语句.Contains("的"))
                    {
                        内容 = new List<string>(new List<string>(语句.Split(new[] { '*', '的' }, StringSplitOptions.RemoveEmptyEntries)));
                        if (内容[0] == "我")
                        {
                            内容[0] = 数据.私聊目标.FromQQ.ToString();
                        }
                        if (!数据.实体.ContainsKey(内容[0]))
                        {
                            数据.写入实体(内容[0], 内容[1], "0");
                        }
                        else if (!数据.实体[内容[0]].ContainsKey(内容[1]))
                        {
                            数据.写入实体(内容[0], 内容[1], "0");
                        }
                        内容[2] = new DataTable().Compute(数据.实体[内容[0]][内容[1]] + "*" + 内容[2], "").ToString();
                        数据.写入实体(内容[0], 内容[1], 内容[2]);
                    }
                    else//不输入组件则默认为“值”组件
                    {
                        内容 = new List<string>(语句.Split(new[] { '*' }, StringSplitOptions.RemoveEmptyEntries));
                        内容.Insert(1, "值");
                        if (!数据.实体.ContainsKey(内容[0]))
                        {
                            数据.写入实体(内容[0], 内容[1], "0");
                        }
                        else if (!数据.实体[内容[0]].ContainsKey(内容[1]))
                        {
                            数据.写入实体(内容[0], 内容[1], "0");
                        }
                        内容[2] = new DataTable().Compute(数据.实体[内容[0]][内容[1]] + "*" + 内容[2], "").ToString();
                        数据.写入实体(内容[0], 内容[1], 内容[2]);
                    }
                    if (内容[0] == 数据.私聊目标.FromQQ.ToString())
                    {
                        内容[0] = 数据.获取昵称().TrimEnd('的');
                    }
                    if (数据.开发模式)
                    {
                        return "";
                    }
                    return $"{内容[0]}的{内容[1]}={内容[2]}";
                }
                if (语句.Contains("/"))
                {
                    List<string> 内容 = new List<string>();
                    if (语句.Contains("的"))
                    {
                        内容 = new List<string>(new List<string>(语句.Split(new[] { '/', '的' }, StringSplitOptions.RemoveEmptyEntries)));
                        if (内容[0] == "我")
                        {
                            内容[0] = 数据.私聊目标.FromQQ.ToString();
                        }
                        if (!数据.实体.ContainsKey(内容[0]))
                        {
                            数据.写入实体(内容[0], 内容[1], "0");
                        }
                        else if (!数据.实体[内容[0]].ContainsKey(内容[1]))
                        {
                            数据.写入实体(内容[0], 内容[1], "0");
                        }
                        内容[2] = new DataTable().Compute(数据.实体[内容[0]][内容[1]] + "/" + 内容[2], "").ToString();
                        数据.写入实体(内容[0], 内容[1], 内容[2]);
                    }
                    else//不输入组件则默认为“值”组件
                    {
                        内容 = new List<string>(语句.Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries));
                        内容.Insert(1, "值");
                        if (!数据.实体.ContainsKey(内容[0]))
                        {
                            数据.写入实体(内容[0], 内容[1], "0");
                        }
                        else if (!数据.实体[内容[0]].ContainsKey(内容[1]))
                        {
                            数据.写入实体(内容[0], 内容[1], "0");
                        }
                        内容[2] = new DataTable().Compute(数据.实体[内容[0]][内容[1]] + "/" + 内容[2], "").ToString();
                        数据.写入实体(内容[0], 内容[1], 内容[2]);
                    }
                    if (内容[0] == 数据.私聊目标.FromQQ.ToString())
                    {
                        内容[0] = 数据.获取昵称().TrimEnd('的');
                    }
                    if (数据.开发模式)
                    {
                        return "";
                    }
                    return $"{内容[0]}的{内容[1]}={内容[2]}";
                }
                if (语句.Contains("^"))
                {
                    List<string> 内容 = new List<string>();
                    if (语句.Contains("的"))
                    {
                        内容 = new List<string>(new List<string>(语句.Split(new[] { '^', '的' }, StringSplitOptions.RemoveEmptyEntries)));
                        if (内容[0] == "我")
                        {
                            内容[0] = 数据.私聊目标.FromQQ.ToString();
                        }
                        if (!数据.实体.ContainsKey(内容[0]))
                        {
                            数据.写入实体(内容[0], 内容[1], "0");
                        }
                        else if (!数据.实体[内容[0]].ContainsKey(内容[1]))
                        {
                            数据.写入实体(内容[0], 内容[1], "0");
                        }
                        内容[2] = Math.Pow(Convert.ToDouble(数据.实体[内容[0]][内容[1]]), Convert.ToDouble(内容[2])).ToString();
                        数据.写入实体(内容[0], 内容[1], 内容[2]);
                    }
                    else//不输入组件则默认为“值”组件
                    {
                        内容 = new List<string>(语句.Split(new[] { '^' }, StringSplitOptions.RemoveEmptyEntries));
                        内容.Insert(1, "值");
                        if (!数据.实体.ContainsKey(内容[0]))
                        {
                            数据.写入实体(内容[0], 内容[1], "0");
                        }
                        else if (!数据.实体[内容[0]].ContainsKey(内容[1]))
                        {
                            数据.写入实体(内容[0], 内容[1], "0");
                        }
                        内容[2] = Math.Pow(Convert.ToDouble(数据.实体[内容[0]][内容[1]]), Convert.ToDouble(内容[2])).ToString();
                        数据.写入实体(内容[0], 内容[1], 内容[2]);
                    }
                    if (内容[0] == 数据.私聊目标.FromQQ.ToString())
                    {
                        内容[0] = 数据.获取昵称().TrimEnd('的');
                    }
                    if (数据.开发模式)
                    {
                        return "";
                    }
                    return $"{内容[0]}的{内容[1]}={内容[2]}";
                }
                #endregion

                //判断是否是方法
                方法.检查方法(语句);
                return "";
            }
            return "";
        }


        public static string 变量解释(string 语句)
        {
            if (语句.Length > 4096 || (语句.Length > 100 && 语句.Contains("【输入的语句】")))
            {
                return "";
            }
            if (语句.Contains("：") && (语句.Contains("直到") || 语句.Contains("如果") || 语句.Contains("遍历")))
            {
                数据.临时空间 = 语句.Substring(语句.IndexOf("："));
                语句 = 语句.Substring(0, 语句.IndexOf("："));
            }
            if (!语句.Contains("【") || !语句.Contains("】"))
            {
                return 语句;
            }
            else
            {
                Stack<char> 符号栈 = new Stack<char>(); Stack<char> 内容栈 = new Stack<char>(); string 栈内容 = "";
                for (int i = 0; i < 语句.Length; i++)
                {
                    if (符号栈.Count > 9999 && 内容栈.Count > 9999)
                    {
                        return "";
                    }
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

                                if (参数[0] == "我")
                                {
                                    参数[0] = 数据.私聊目标.FromQQ.ToString();
                                }
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

                                    符号栈.Pop();
                                    参数.RemoveRange(0, 2);
                                    while (参数.Count > 0)
                                    {
                                        替换内容 = 数值.的(替换内容, 参数[0]);
                                        参数.RemoveAt(0);
                                    }
                                }
                                else if (参数[0].StartsWith("“") && 参数[0].EndsWith("”"))//求返回值
                                {
                                    替换内容 = 执行(参数[0].Substring(1, 参数[0].Length - 2));
                                    符号栈.Pop();
                                    参数.RemoveRange(0, 1);
                                    while (参数.Count > 0)
                                    {
                                        替换内容 = 数值.的(替换内容, 参数[0]);
                                        参数.RemoveAt(0);
                                    }
                                }

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
            else if (判别式.Contains("等于"))
            {
                模式 = "等于";
                委托 = new 数据.布尔委托(比较.等于);
            }
            else if (判别式.Contains("大于"))
            {
                模式 = "大于";
                委托 = new 数据.布尔委托(比较.大于);
            }
            else if (判别式.Contains("小于"))
            {
                模式 = "小于";
                委托 = new 数据.布尔委托(比较.小于);
            }
            else if (判别式.Contains("包含"))
            {
                模式 = "包含";
                委托 = new 数据.布尔委托(比较.包含);
            }
            else if (判别式.Contains("开头是"))
            {
                模式 = "开头是";
                委托 = new 数据.布尔委托(比较.开头是);
            }
            else if (判别式.Contains("结尾是"))
            {
                模式 = "结尾是";
                委托 = new 数据.布尔委托(比较.结尾是);
            }
            else if (判别式.Contains("="))
            {
                模式 = "=";
                委托 = new 数据.布尔委托(比较.等于);
            }

            List<string> 集合 = new List<string>(判别式.Split(new string[] { 模式 }, StringSplitOptions.RemoveEmptyEntries));

            if (集合.Count != 2)
            {
                return false;
            }
            if (集合[0].EndsWith("不"))
            {
                集合[0] = 集合[0].TrimEnd('不');
                try
                {
                    Convert.ToDecimal(集合[0]);//试图转换集合左值为数字，如果转换成功，大概率是常量
                    数据.写入实体("常量", "值", 集合[0]);
                    return !委托("常量的值", 集合[1]);
                }
                catch (Exception)
                {
                    return !委托(集合[0], 集合[1]);
                }
            }

            try
            {
                Convert.ToDecimal(集合[0]);//试图转换集合左值为数字，如果转换成功，大概率是常量
                数据.写入实体("常量", "值", 集合[0]);
                return 委托("常量的值", 集合[1]);
            }
            catch (Exception)
            {
                return 委托(集合[0], 集合[1]);
            }
        }
    }
}
