using System;

namespace Native.Csharp.App.Event.Event_Me
{
    class Event_CheckError
    {
        /// <summary>
        /// 返回错误信息
        /// </summary>
        public static string CheckError(Exception ex)
        {
            string str = 数据.报错;

            //str += ex.Message + "\n";//异常消息
            //str += ex.StackTrace + "\n";//提示出错位置，不会定位到方法内部去
            if (数据.开发者.Contains(数据.私聊目标.FromQQ))
            {
                str += "\n"
                + 数据.实体["输入"]["语句"] + "\n"
                + ex.ToString().
                    Replace("System.ArgumentOutOfRangeException: ", "").
                    Replace("Native.Csharp.App.Event.Event_Me.", "").
                    Replace("String ", "").Replace("Char ", "").Replace("Int ", "").Replace("Bool ", "").
                    Replace("System.Data.SyntaxErrorException: ", "").
                    Replace("System.NullReferenceException: ", "").
                    Replace("System.IndexOutOfRangeException: ", "").
                    Replace("System.InvalidOperationException: ", "").
                    Replace("System.Collections.Generic.KeyNotFoundException: ", "").
                    Replace("System.Data.EvaluateException: ", "").
                    Replace("Event_ReceiveGroupMessage.ReceiveGroupMessage(Object sender, CqGroupMessageEventArgs e)", "主程序").
                    Replace("Event_ReceiveFriendMessage.ReceiveGroupMessage(Object sender, CqGroupMessageEventArgs e)", "主程序").
                    Replace("Event_ReceiveFriendMessage.ReceiveFriendMessage(Object sender, CqPrivateMessageEventArgs e)", "主程序");//将方法内部和外部所有出错的位置提示出来
            }
            return str;
        }
    }
}
