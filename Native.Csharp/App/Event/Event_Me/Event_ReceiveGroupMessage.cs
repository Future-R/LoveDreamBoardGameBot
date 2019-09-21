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
    /// 群聊回复
    /// CqPrivateMessageEventArgs→CqGroupMessageEventArgs
    /// SendPrivateMessage→ReceiveGroupMessage
    /// e.FromQQ→e.FromGroup
    /// </summary>
    public class Event_ReceiveGroupMessage : IReceiveGroupMessage
    {

        Event_ReceiveFriendMessage ReceiveFriendMessage = new Event_ReceiveFriendMessage();

        public void ReceiveGroupMessage(object sender, CqGroupMessageEventArgs e)
        {
            Event_Variable.isGroup = true;
            string input = e.Message.Replace("&#91;", "[").Replace("&#93;", "]");
            Event_Variable.QQQ = e.FromQQ;
            Event_Variable.varDelay = false;
            Event_Variable.varNeedExp = true;
            Event_Variable.idNum = e.FromGroup;

            input = new Regex("[\\s]+").Replace(input, " ");//合并复数空格
            input = input.Trim();//去除前后空格
            //检查是否启用变量延迟解释
            if (input.StartsWith("~"))
            {
                Event_Variable.varDelay = true;
                input = input.Substring(1);
            }
            if (input.EndsWith("!") || input.EndsWith("！"))//叹号结尾，不需要被解释
            {
                Event_Variable.varNeedExp = false;
                input = input.Remove(input.Length - 1 , 1);//去掉结尾
            }
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
                            Event_Variable.botCloseList.Remove(e.FromGroup);
                            Common.CqApi.SendGroupMessage(e.FromGroup, $"{Convert.ToString(Common.CqApi.GetLoginQQ())}已开启！");
                        }
                        else
                        {
                            return;
                        }
                    }
                    else
                    {
                        Event_Variable.botCloseList.Remove(e.FromGroup);
                        Common.CqApi.SendGroupMessage(e.FromGroup, "已开启！");
                    }
                }
                if (input.StartsWith(".关闭"))
                {
                    if (input.Length > 8)
                    {
                        if (input.Substring(3).Trim() == Convert.ToString(Common.CqApi.GetLoginQQ()))
                        {
                            Event_Variable.botCloseList.Add(e.FromGroup);
                            Event_Variable.botCloseList = Event_Variable.botCloseList.Distinct().ToList();
                            Common.CqApi.SendGroupMessage(e.FromGroup, $"{Convert.ToString(Common.CqApi.GetLoginQQ())}已关闭！");
                        }
                        else
                        {
                            return;
                        }
                    }
                    else
                    {
                        Event_Variable.botCloseList.Add(e.FromGroup);
                        Event_Variable.botCloseList = Event_Variable.botCloseList.Distinct().ToList();
                        Common.CqApi.SendGroupMessage(e.FromGroup, "已关闭！");
                    }
                }
            }
            catch (Exception)
            {
                Common.CqApi.SendGroupMessage(e.FromGroup, "意外的错误！");
            }
            //群号在黑名单里
            if (Event_Variable.botCloseList.Exists((long f) => f == e.FromGroup))
            {
                return;
            }

            try
            {
                //用户输入的前缀为连续扳机，去掉第1个并传递给群私聊类处理
                if (input.StartsWith("..") || input.StartsWith(".。") || input.StartsWith("。。") || input.StartsWith("!!") || input.StartsWith("！！"))
                {
                    CqPrivateMessageEventArgs ee = new CqPrivateMessageEventArgs(e.Id, e.MsgId, e.FromQQ, input.Substring(1));
                    ReceiveFriendMessage.ReceiveFriendMessage(sender, ee);
                    return;
                }
            }
            catch (Exception ex)
            {
                Common.CqApi.SendGroupMessage(e.FromGroup, Event_CheckError.CheckError(ex));
            }

            if (input.StartsWith(".帮助"))//帮助指令
            {
                if (input.Length < 5)//无参帮助
                {
                    Common.CqApi.SendGroupMessage(e.FromGroup, Event_Variable.helpDescription);
                }
                else//含参帮助
                {
                    Common.CqApi.SendGroupMessage(e.FromGroup, Event_HelpDescription.helpDescription(input));
                }
                return;
            }


            //用户输入指令
            if (!Event_Variable.varDelay && Event_Variable.varNeedExp)
            {
                int vvc = 0;
                foreach (var item in Event_Variable.VariableList)//变量解释器
                {
                    input = input.Replace('[' + item.Key + ']', item.Value);
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
                        if (temp.StartsWith("！!") || temp.StartsWith("!！") || temp.StartsWith(".."))//复合指令中的连续扳机
                        {
                            CqPrivateMessageEventArgs ee = new CqPrivateMessageEventArgs(e.Id, e.MsgId, e.FromQQ, temp.Substring(1));
                            ReceiveFriendMessage.ReceiveFriendMessage(sender, ee);
                        }
                        else
                        {
                            Event_Variable.Defa = false;
                            if (temp.StartsWith("！") || temp.StartsWith("!"))//该指令是自动补全参数指令
                            {
                                Event_Variable.Defa = true;
                                temp = "." + temp.Remove(0, 1);
                            }
                            if (Event_Variable.varDelay && Event_Variable.varNeedExp)
                            {
                                int vvc = 0;
                                foreach (var itemx in Event_Variable.VariableList)//变量解释器
                                {
                                    temp = temp.Replace('[' + itemx.Key + ']', itemx.Value);
                                    vvc++;
                                }
                            }
                            Event_OriginalCommand.CommandIn(temp, e.FromGroup, true);
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
                    if (Event_Variable.varDelay && Event_Variable.varNeedExp)
                    {
                        int vvc = 0;
                        foreach (var itemx in Event_Variable.VariableList)//变量解释器
                        {
                            input = input.Replace('[' + itemx.Key + ']', itemx.Value);
                            vvc++;
                        }
                    }
                    Event_OriginalCommand.CommandIn(input, e.FromGroup, true);//查一下是不是固有指令
                }
            }
            catch (Exception)
            {
                Common.CqApi.SendGroupMessage(e.FromGroup, "指令输入错误！输入'.帮助'检查你的格式！");
            }
        }
    }
}
