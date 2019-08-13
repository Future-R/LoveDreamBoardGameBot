using Native.Csharp.App.EventArgs;
using Native.Csharp.App.Interface;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace Native.Csharp.App.Event.Event_Me
{
    /// <summary>
    /// 私聊回复
    /// </summary>
    public class Event_ReceiveFriendMessage : IReceiveFriendMessage
    {
        private int ret;

        public void ReceiveFriendMessage(object sender, CqPrivateMessageEventArgs e)
        {

            string input = e.Message;

            input = new Regex("[\\s]+").Replace(input, " ");//合并复数空格
            input = input.Trim().Replace("色子","骰子").
                Replace("RD", "骰子").Replace("rd", "骰子").Replace("Rd", "骰子");//去除前后空格，统一一些措辞
            //把用户输入的第一个中文句号替换为英文
            if (input.Substring(0, 1) == "。")
            {
                input = "." + input.Remove(0, 1);
            }

            if (input == ".帮助")
            {
                Common.CqApi.SendPrivateMessage(e.FromQQ, @"恋梦桌游姬V0.9.0 By未来菌
方括号内为参数，带*的为选填参数：
.计算 [算式]：进行四则运算
.骰子 [数量*] [面数]：投掷X枚骰子，默认为1
.创建 [区域]：创建一个可以放置卡牌或记录信息的区域
.销毁 [区域]：销毁区域
.清空 [区域]：清空区域
.添加 [区域] [牌名] [牌名*]：添加若干指定名称的牌进入区域，卡牌间用空格表示并列
.删除 [区域] [牌名] [牌名*]：删除区域中搜索到的最前方的指定卡牌
.抽牌 [旧区域] [新区域] [数量]：从旧区域抽X张牌到新区域
.查看 [区域]：打印区域内容
.洗牌 [区域]：区域卡牌乱序排序
.清点 [区域]：计算区域卡牌数量
.插入 [区域] [序号] [牌名] [牌名*]：在区域的第X张牌之前插入若干张牌
.发现 [区域] [数量*]：展示区域内随机X张牌，默认为3（施工中）
.翻转 [区域]：私密变为公开，公开变为私密，牌序反转（施工中）
.建表 [区域]：创建一个二维棋盘区域（施工中）

注：可以在区域开头加入“私密”字样，如[私密牌库]。非私密区域在某些卡牌变化场景会打印内容。
");
                return;
            }

            if (input.Substring(0, 1) == ".")//检测到用户输入指令
            {
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
                        Common.CqApi.SendPrivateMessage(e.FromQQ, Boom(input));
                        return;

                    case "清空":
                        Boom(input);
                        switch (Crea(input))//创建
                        {
                            case 0:
                                Common.CqApi.SendPrivateMessage(e.FromQQ, $@"清空{input.Trim().Substring(3).Trim()}成功！");//待优化
                                return;

                            default:
                                Common.CqApi.SendPrivateMessage(e.FromQQ, $@"{input.Trim().Substring(3).Trim()}丢失！");
                                return;
                        }

                    case "添加":
                        Common.CqApi.SendPrivateMessage(e.FromQQ, Add(input));
                        return;

                    case "删除":
                        Common.CqApi.SendPrivateMessage(e.FromQQ, DelInfos(input));
                        return;

                    case "移动":
                        //
                        return;

                    case "抽牌":
                        Common.CqApi.SendPrivateMessage(e.FromQQ, Draw(input));
                        return;

                    case "查看":
                        GetInfo(input,out string looname,out string looret);
                        Common.CqApi.SendPrivateMessage(e.FromQQ,$@"{looname}:
{looret}");
                        return;

                    case "清点":
                        CountNum(input,out string num);
                        Common.CqApi.SendPrivateMessage(e.FromQQ, $@"{input.Substring(3).Trim()}有{num}张牌");
                        return;

                    case "发现":
                        return;

                    case "存档":
                        return;

                    case "读档":
                        return;

                    case "复制":
                        return;

                    case "洗牌":
                        Common.CqApi.SendPrivateMessage(e.FromQQ, Shuffle(input));
                        return;

                    case "插入":
                        Common.CqApi.SendPrivateMessage(e.FromQQ, Inser(input));
                        return;

                    case "移除"://
                        return;

                    case "翻转"://顺序颠倒，私密变公开，公开变私密
                        return;

                    case "建表":
                        return;
                }

                Common.CqApi.SendPrivateMessage(e.FromQQ, "不存在的指令！获取指令目录请输入“.帮助”！");

            }
        }

        //模块区_______________________________________________________________________________________________________________________________________________________________________

        //计算
        public string Calc(string input)
        {
            input = input.Substring(3).Trim().Replace("×", "*").Replace("x", "*").Replace("X", "*")
                            .Replace("（", "(").Replace("）", ")").Replace("÷", "/").Replace("％","/100")
                            .Replace("%","/100");//中文运算符都换成程序运算符

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
        public string Dices(string input)
        {
            input = input.Substring(3).Trim();
            string[] inputArray = input.Split(new char[3] { 'D', 'd', ' ' });

            if (inputArray.Length > 1)//多个参数
            {
                if (IsNumeric(inputArray[1]))//如果第二个参数是纯数，则为复数骰子
                {
                    try
                    {
                        string results = "";
                        for (int i = 0; i < Convert.ToInt32(inputArray[0]); i++)
                        {
                            int result = new Random(Guid.NewGuid().GetHashCode()).Next(0, Convert.ToInt32(inputArray[1])) + 1;
                            results = results + "+" + result;
                        }
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
                        Random rd = new Random();
                        int result = rd.Next(0, Convert.ToInt32(inputArray[0])) + 1;
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
                    Random rd = new Random();
                    int result = rd.Next(0, Convert.ToInt32(inputArray[0])) + 1;
                    return $"{result}";
                }
                catch (Exception)
                {

                    return "非法输入！";
                }
            }
        }

        //创建
        public int Crea(string input)
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

        //销毁
        public string Boom(string input)
        {
            input = input.Substring(3).Trim();
            string[] inputArray = input.Split(' ');
            ret = DelInfo(inputArray[0]);
            if (ret == 2)
            {
                return $@"找不到{inputArray[0]}！";
            }
            else
            {
                if (ret == 0)
                {
                    return $@"{inputArray[0]}已销毁！";
                }
                else
                {
                    return $@"{inputArray[0]}清空失败！";
                }
            }
        }

        //添加
        public string Add(string input)
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

        //获取信息
        public void GetInfo(string input, out string name, out string ret)
        {
            input = input.Substring(3).Trim();
            string[] inputArray = input.Split(' ');
            name = $@"{inputArray[0]}";
            ret = $@"{LoadInfo(inputArray[0])}";
        }

        //删除
        public string DelInfos(string input)
        {
            GetInfo(input, out string remname, out string remret);//获取区域内容
            input = input.Substring(3).Trim();//截取参数
            int spsNum = input.IndexOf(" ");//分隔参数
            try
            {
                //如果文件存在的话……我会抽时间优化一下这块代码
                if (File.Exists(System.AppDomain.CurrentDomain.BaseDirectory + @"SaveDir\" + input.Substring(0, spsNum) + ".ini"))
                {
                    try
                    {
                        string tempInput = remret.Trim();//用来修改的字符串
                        string[] delList = input.Substring(spsNum + 1).Split(' ');//肃反名单
                        foreach (var item in delList)
                        {
                            int strNum = tempInput.IndexOf(item);//找到第一个对象的位置
                            tempInput = tempInput.Substring(0, strNum) + tempInput.Substring(strNum + item.Length);//字符串对象左侧和右侧直接相连
                        }

                        DelInfo(remname); CreateInfo(remname);//清空又创建，真的好暴力
                        tempInput = new Regex("[\\s]+").Replace(tempInput, " ");
                        WriteInfo(remname, tempInput);
                        if (remname.Contains("私密"))
                        {
                            return "删除成功！";
                        }
                        else
                        {
                            return $@"{remname}:
{tempInput}";//打印更改后的内容
                        }
                    }
                    catch (Exception)
                    {

                        return $@"找不到{input.Substring(spsNum + 1)}！";
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

        //洗牌
        public string Shuffle(string input)
        {
            try
            {
                input = input.Substring(3).Trim();
                string inputs = LoadInfo(input);
                if (inputs != "读取失败！" || inputs != "读取错误！")
                {
                    List<string> list = new List<string>(inputs.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries));
                    List<string> list2 = new List<string>();
                    int count = list.Count;
                    for (int i = 0; i < count -1; i++)
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
        public string Move(string input)//移动 区域A 区域B XXX YYY ZZZ
        {
            try
            {
                input = input.Substring(3).Trim();
                List<string> list = new List<string>(input.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries));
                string name1 = list[0];
                string name2 = list[1];
                list.RemoveRange(0,2);//用户想要移动的名单
                List<string> fromList = new List<string>(File.ReadAllText(name1).Trim().Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries));//来源区域
                List<string> toList = new List<string>(File.ReadAllText(name2).Trim().Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries));//目标区域

                try
                {
                    foreach (var item in list)
                    {
                        int strNum = fromList.IndexOf(item);//找到第一个对象的位置
                        fromList.RemoveAt(strNum);
                    }
                    for (int i = 0; i < fromList.Count; i++)
                    {
                        WriteInfo(name1, $@" {fromList[i]}");//写入来源区域
                    }
                    toList.AddRange(list);
                    for (int i = 0; i < toList.Count; i++)
                    {
                        WriteInfo(name2, $@" {toList[i]}");//写入目标区域
                    }

                    if (name2.Contains("私密"))
                    {
                        return "移动成功！";
                    }
                    else
                    {
                        return $@"{name2}:
{"string"}";//打印更改后的内容
                    }

                }
                catch (Exception)
                {
                    return "移动失败！";
                }

            }
            catch (Exception)
            {
                return "移动失败！";
            }
        }

        //抽取
        public string Draw(string input)//抽牌 旧区域 新区域 数量
        {
            try
            {
                input = input.Substring(3).Trim();
                List<string> list = new List<string>(input.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries)); //区域A 区域B int ...
                List<string> listA = new List<string>(LoadInfo(list[0]).Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries));
                List<string> listB = new List<string>(LoadInfo(list[1]).Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries));
                if (Convert.ToInt32(list[2]) > listA.Count)
                {
                    return $@"{listA}剩余数量不足{list[2]}张！";
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
                    return $@"{list[1]}:
{LoadInfo(list[1])}";
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
                if (inputs != "读取失败！" || inputs != "读取错误！")
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
                zoneList.InsertRange(int.Parse(list[1]) - 1, insList);
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

        //骰子
        public void Dice6(long qq, string input)
        {

        }

        //骰子
        public void Dice7(long qq, string input)
        {

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
            string CurDir = System.AppDomain.CurrentDomain.BaseDirectory + @"SaveDir\";
            //判断路径是否存在
            if (!System.IO.Directory.Exists(CurDir))
            {
                System.IO.Directory.CreateDirectory(CurDir);
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
                System.IO.StreamWriter file = new System.IO.StreamWriter(FilePath, false);
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
            name = System.AppDomain.CurrentDomain.BaseDirectory + @"SaveDir\" + name + ".ini";
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
            name = System.AppDomain.CurrentDomain.BaseDirectory + @"SaveDir\" + name + ".ini";
            try
            {
                if (File.Exists(name))
                {
                    string load = File.ReadAllText(name).Trim();
                    return load;
                }
                else
                {
                    return "读取失败！";
                }
            }
            catch
            {
                return "读取错误！";
            }
        }
    }
}
