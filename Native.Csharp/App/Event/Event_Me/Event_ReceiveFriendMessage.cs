using Native.Csharp.App.EventArgs;
using Native.Csharp.App.Interface;
using System;

namespace Native.Csharp.App.Event.Event_Me
{
    /// <summary>
    /// 私聊回复
    /// </summary>
    public class Event_ReceiveFriendMessage : IReceiveFriendMessage, IReceiveGroupPrivateMessage, IReceiveDiscussMessage
    {
        public void ReceiveFriendMessage(object sender, CqPrivateMessageEventArgs e)
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
                    数据.私聊 = true;
                    数据.私聊目标 = e;
                    数据.群聊目标 = e;
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

        /// <summary>
        /// 实现群私聊接口
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void ReceiveGroupPrivateMessage(object sender, CqPrivateMessageEventArgs e)
        {
            ReceiveFriendMessage(sender,e);
        }

        /// <summary>
        /// 实现讨论组接口
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void ReceiveDiscussMessage(object sender, CqDiscussMessageEventArgs e)
        {
            try
            {
                string 用户输入 = 消息预处理.处理(e.Message);
                if (!用户输入.StartsWith(".") || 用户输入.Length < 2 || 用户输入.StartsWith("..") || 用户输入.StartsWith(".。"))//防误触
                {
                    return;
                }
                数据.私聊 = true;
                数据.私聊目标 = new CqPrivateMessageEventArgs(e.Id, e.MsgId, e.FromQQ, e.Message);
                数据.群聊目标 = null;
                数据.讨论组目标 = e;
                消息预处理.环境初始化();
                解释.语法分析(用户输入.Substring(1));
            }
            catch (Exception ex)
            {
                Common.CqApi.SendPrivateMessage(e.FromQQ, Event_CheckError.CheckError(ex));
            }
        }
    }
}