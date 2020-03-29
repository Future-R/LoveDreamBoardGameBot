using System;
using System.IO;
using System.Net;
//using System.Web.Script.Serialization;
using System.Text;

namespace Native.Csharp.App.Event.Event_Me
{
    public class JSON
    {
        public static string 获取(string 接口)
        {
            string 现在的时间 = $"{DateTime.Now.DayOfYear.ToString()}{DateTime.Now.TimeOfDay.ToString().Substring(0, 8).Replace(":", "")}";
            if (Convert.ToInt32(运算.计算(现在的时间 + "-" + 数据.上次调用接口的时间)) < 30)//30秒内重复调用接口，返回空值
            {
                return "";
            }
            接口 = 转义.输出(接口);//取消转义
            接口 = System.Web.HttpUtility.UrlEncode(接口);
            接口 = 接口.Replace("%3a", ":").Replace("%2f", "/").Replace("%3f", "?").Replace("%3d", "=").Replace("%26amp%3b", "&");
            GC.Collect();
            HttpWebRequest 请求 = (HttpWebRequest)WebRequest.Create(接口);
            请求.Proxy = null;
            请求.KeepAlive = false;
            请求.Method = "GET";
            请求.ContentType = "application/json; charset=UTF-8";
            请求.AutomaticDecompression = DecompressionMethods.GZip;

            HttpWebResponse 应答 = (HttpWebResponse)请求.GetResponse();
            Stream 应答流 = 应答.GetResponseStream();
            StreamReader 读取流 = new StreamReader(应答流, Encoding.UTF8);
            string 返回值 = 读取流.ReadToEnd();

            读取流.Close();
            应答流.Close();

            if (应答 != null)
            {
                应答.Close();
            }
            if (请求 != null)
            {
                请求.Abort();
            }

            数据.上次调用接口的时间 = $"{DateTime.Now.DayOfYear.ToString()}{DateTime.Now.TimeOfDay.ToString().Substring(0, 8).Replace(":", "")}";
            return 返回值;
        }

        //public static object 反序列化<T>(string 输入)
        //{
        //    JavaScriptSerializer JSON = new JavaScriptSerializer();
        //    return JSON.Deserialize<T>(输入);
        //}
        //public static string 序列化(object 对象)
        //{
        //    JavaScriptSerializer JSON = new JavaScriptSerializer();
        //    return JSON.Serialize(对象);
        //}
    }
}
