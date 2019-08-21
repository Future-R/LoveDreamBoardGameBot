using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Native.Csharp.App.Event.Event_Me
{
    class Event_CheckError
    {
        public static string CheckError(Exception ex)
        {
            string str = "";
            str += ex.Message + "\n";//异常消息
            str += ex.StackTrace + "\n";//提示出错位置，不会定位到方法内部去
            str += ex.ToString() + "\n";//将方法内部和外部所有出错的位置提示出来
            return str;
        }
    }
}
