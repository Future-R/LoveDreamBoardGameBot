using System;
using System.IO;
using System.Net;
using System.Text;

namespace Native.Csharp.App.Event.Event_Me
{
    public class JSON
    {
        public static string 获取(string 接口)
        {
            string 现在的时间 = $"{DateTime.Now.DayOfYear.ToString()}{DateTime.Now.TimeOfDay.ToString().Substring(0, 8).Replace(":", "")}";
            if (Convert.ToInt32(运算.计算(现在的时间 +"-"+ 数据.上次调用接口的时间)) < 30)//30秒内重复调用接口，返回空值
            {
                return "";
            }
            System.GC.Collect();
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(接口);
            request.Proxy = null;
            request.KeepAlive = false;
            request.Method = "GET";
            request.ContentType = "application/json; charset=UTF-8";
            request.AutomaticDecompression = DecompressionMethods.GZip;

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            Stream myResponseStream = response.GetResponseStream();
            StreamReader myStreamReader = new StreamReader(myResponseStream, Encoding.UTF8);
            string 返回值 = myStreamReader.ReadToEnd();

            myStreamReader.Close();
            myResponseStream.Close();

            if (response != null)
            {
                response.Close();
            }
            if (request != null)
            {
                request.Abort();
            }

            数据.上次调用接口的时间 = $"{DateTime.Now.DayOfYear.ToString()}{DateTime.Now.TimeOfDay.ToString().Substring(0, 8).Replace(":", "")}";
            return 返回值;
        }
    }
}
