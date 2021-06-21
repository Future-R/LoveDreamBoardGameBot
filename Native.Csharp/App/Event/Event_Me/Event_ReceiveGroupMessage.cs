using Native.Csharp.App.EventArgs;
using Native.Csharp.App.Interface;
using System;

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
            try
            {
                lock (数据.实体)
                {
                    string 用户输入 = 消息预处理.处理(e.Message);
                    if (!用户输入.StartsWith(".") || 用户输入.Length < 2 || 用户输入.StartsWith("..") || 用户输入.StartsWith(".。"))//防误触
                    {
                        return;
                    }
                    数据.私聊 = false;
                    数据.群聊目标 = e;
                    数据.私聊目标 = e;
                    数据.讨论组目标 = null;
                    消息预处理.环境初始化();
                    解释.语法分析(用户输入.Substring(1));
                }
            }
            catch (Exception ex)
            {
                Common.CqApi.SendPrivateMessage(e.FromQQ, Event_CheckError.CheckError(ex));
            }

        }
    }
}
