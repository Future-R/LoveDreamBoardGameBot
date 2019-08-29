using Native.Csharp.App.EventArgs;
using Native.Csharp.App.Interface;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
            Event_Variable.isGroup = false;
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

            if (input.StartsWith(".帮助"))//帮助指令
            {
                if (input.Length < 5)//无参帮助
                {
                    Common.CqApi.SendPrivateMessage(e.FromQQ, Event_Variable.helpDescription);
                }
                else//含参帮助
                {
                    Common.CqApi.SendPrivateMessage(e.FromQQ, Event_HelpDescription.helpDescription(input));
                }
                return;
            }


            //用户输入指令
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
                        {
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
                            Event_OriginalCommand.CommandIn(temp, e.FromQQ);
                        }
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
                    Event_OriginalCommand.CommandIn(input, e.FromQQ);//查一下是不是固有指令
                }
            }
            catch (Exception)
            {
                Common.CqApi.SendPrivateMessage(e.FromQQ, "指令输入错误！输入'.帮助'检查你的格式！");
            }
        }
    }
}