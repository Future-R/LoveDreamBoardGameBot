using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Native.Csharp.App.Event.Event_Me
{
    class Event_CheckError
    {
        /// <summary>
        /// 返回错误信息
        /// </summary>
        public static string CheckError(Exception ex)
        {
            string str = new string[] 
            { "我懵了。", "哎？", "不对……", "怎么回事？", "什么情况？", "奇怪的错误……", "搞错了吧？" , "……", "又来了……" , "¿", "菜", "算了吧……" , "唔。"}
            [new Random(Guid.NewGuid().GetHashCode()).Next(0, 12)];

            //str += ex.Message + "\n";//异常消息
            //str += ex.StackTrace + "\n";//提示出错位置，不会定位到方法内部去
            //str += ex.ToString() + "\n";//将方法内部和外部所有出错的位置提示出来
            return str;
        }
    }
}
