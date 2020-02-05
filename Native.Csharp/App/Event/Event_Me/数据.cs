using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Native.Csharp.App.Event.Event_Me
{
    public class 数据
    {
        public static bool 私聊
        {
            get;set;
        }

        public static Dictionary<string, Dictionary<string, string>> 实体 = new Dictionary<string, Dictionary<string, string>>();

        public static string 词典位置 = File.ReadAllText(AppDomain.CurrentDomain.BaseDirectory + @"app\dict.ini", Encoding.GetEncoding("gb2312"));

        public static List<string> 反义词典 => new List<string>(词典位置.Split(new string[] { "A", Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries));


        public static void 写入实体(List<string> 参数)
        {
            string 实体名 = 参数[0]; string 组件名 = 参数[1]; string 组件值 = 参数[2];
            if (!实体.ContainsKey(实体名))//如果没有这个实体就创建实体，并添加对应的组件
            {
                实体.Add(实体名, new Dictionary<string, string>());
                实体[实体名].Add(组件名, 组件值);
            }
            else
            {
                if (!实体[实体名].ContainsKey(组件名))
                {
                    实体[实体名].Add(组件名, 组件值);
                }
                else
                {
                    实体[实体名][组件名] = 组件值;
                }
            }
        }
    }
}
