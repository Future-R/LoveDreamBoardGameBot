using Native.Csharp.App.EventArgs;
using Native.Csharp.App.Interface;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace Native.Csharp.App.Event.Event_Me
{
    /// <summary>
    /// 私聊回复
    /// </summary>
    public class Event_ReceiveFriendMessage : IReceiveFriendMessage,IReceiveGroupPrivateMessage
    {
        public void ReceiveFriendMessage(object sender, CqPrivateMessageEventArgs e)
        {
            
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
    }
}