using Native.Csharp.Sdk.Cqp.Model;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Native.Csharp.App.EventArgs
{
    public class 数据
    {
        public static DateTime 上次保存时间;

        public static List<long> 正在统计的群 = new List<long>();

        public static Dictionary<long, List<消息记录>> 所有消息合集 = new Dictionary<long, List<消息记录>>();

        public static string 正在使用的字体 = "微软雅黑";

        public class 消息记录
        {
            public DateTime 时间戳;
            public long QQ号;
            public string 消息;

            public string 过滤特殊消息()
            {
                // 过滤聊天记录
                if (消息.Contains("[聊天记录]"))
                {
                    var 匹配结果组 = Regex.Matches(消息, @"(?<=发送内容\[)\S+(?=\])");
                    消息 = string.Join(" ", 匹配结果组);
                }
                // 过滤网址
                if (Regex.IsMatch(消息, @"(http(s)?://)?[a-zA-Z0-9][-a-zA-Z0-9]{0,62}(\.[a-zA-Z0-9][-a-zA-Z0-9]{0,62})+\.?[^\u4e00-\u9fa5]+"))
                {
                    var 匹配结果组 = Regex.Matches(消息, @"(http(s)?://)?[a-zA-Z0-9][-a-zA-Z0-9]{0,62}(\.[a-zA-Z0-9][-a-zA-Z0-9]{0,62})+\.?[^\u4e00-\u9fa5]+");
                    foreach (var item in 匹配结果组)
                    {
                        消息 = 消息.Replace(item.ToString(), "");
                    }
                }
                // 过滤"请使用最新版手机QQ体验新功能"
                if (消息.Contains("请使用最新版手机QQ体验新功能"))
                {
                    消息 = 消息.Replace("请使用最新版手机QQ体验新功能", "");
                }
                //// 过滤at
                //if (Regex.IsMatch(消息, @"\[@\d+\]\s*"))
                //{
                //    var 匹配结果组 = Regex.Matches(消息, @"\[@\d+\]\s*");
                //    foreach (var item in 匹配结果组)
                //    {
                //        消息 = 消息.Replace(item.ToString(), "");
                //    }
                //}
                // 过滤回复
                if (消息.StartsWith("[Reply,"))
                {
                    var 匹配结果组 = Regex.Matches(消息, @"\[Reply,\S+\]\s*");
                    foreach (var item in 匹配结果组)
                    {
                        消息 = 消息.Replace(item.ToString(), "");
                    }
                }
                // 过滤红包
                if (消息.StartsWith("[GroupRedBags,"))
                {
                    var 匹配结果组 = Regex.Matches(消息, "(?<=\\[GroupRedBags,Data=\\{\\\\\"Msg\\\\\":\\\\\")\\S +? (?=\\\\)");
                    foreach (var item in 匹配结果组)
                    {
                        消息 = 消息.Replace(item.ToString(), "");
                    }
                }
                return 消息;
            }
        }

        public static GroupMember 娶群友(CqGroupMessageEventArgs e, long QQ号)
        {
            return Common.CqApi.GetMemberInfo(e.FromGroup, QQ号, true);
        }

        public static string 获取群名片(CqGroupMessageEventArgs e, long QQ号, List<GroupMember> 群员列表 = null, int 最大字数 = 15)
        {
            if (群员列表 != null)
            {
                return 群员列表.Where(x => x.QQId == QQ号).FirstOrDefault().Nick;
            }

            string 昵称 = "";
            GroupMember 群友 = 娶群友(e, QQ号);
            昵称 = string.IsNullOrWhiteSpace(群友.Card)//取群名片
                ? 群友.Nick : 群友.Card;//取QQ昵称
            return 昵称;
        }

        public static bool 保存信息()
        {
            try
            {
                string 目录 = Common.AppDirectory;
                foreach (long 群号 in 正在统计的群)
                {
                    if (!Directory.Exists($@"{目录}{群号}"))
                    {
                        Directory.CreateDirectory($@"{目录}{群号}");
                    }
                    List<消息记录> 群聊记录 = 所有消息合集[群号];
                    var 记录按日期分组 = from M in 群聊记录
                                  group M by M.时间戳.ToShortDateString() into D
                                  select D;
                    Dictionary<string, List<消息记录>> 分组结果 = 记录按日期分组.ToDictionary(k => k.Key, v => v.ToList());
                    //QMLog.CurrentApi.Info($"分组结果条目{分组结果.Count}");
                    foreach (var 时间与消息记录表 in 分组结果)
                    {
                        string 文件名 = $@"{目录}{群号}\\{时间与消息记录表.Key.Replace("/", "-")}.json";
                        List<消息记录> 待写入内容 = 时间与消息记录表.Value;
                        //QMLog.CurrentApi.Info($"{时间与消息记录表.Key}待写入内容条目{待写入内容.Count}");
                        //if (File.Exists(文件名))
                        //{
                        //    待写入内容.AddRange(JsonConvert.DeserializeObject<List<消息记录>>(读取(文件名)));
                        //}
                        写入(文件名, JsonConvert.SerializeObject(待写入内容));
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                //QMLog.CurrentApi.Error(ex.ToString());
                return false;
            }
        }

        public static bool 读取信息(int 天数 = 1)
        {
            try
            {
                string 目录 = Common.AppDirectory;
                DirectoryInfo[] 所有文件夹 = new DirectoryInfo(目录).GetDirectories();
                // 初始化正在统计的群
                正在统计的群.Clear();
                所有消息合集.Clear();
                foreach (var 文件夹 in 所有文件夹)
                {
                    if (Regex.IsMatch(文件夹.Name, "^[0-9]+$"))
                    {
                        long 群号 = Convert.ToInt64(文件夹.Name);
                        正在统计的群.Add(群号);
                        所有消息合集.Add(群号, 获取指定日期的记录(群号, DateTime.Today, 天数));

                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                //QMLog.CurrentApi.Error(ex.ToString());
                return false;
            }
        }

        public static List<消息记录> 获取指定日期的记录(long 群号, DateTime 指定日期零点, int 天数 = 1)
        {
            string 目录 = Common.AppDirectory;
            List<DateTime> 指定日期范围 = new List<DateTime>();
            指定日期范围.Add(指定日期零点);
            while (天数 > 1)
            {
                天数--;
                指定日期范围.Add(指定日期范围.Last().AddDays(-1));
            }
            List<string> 指定日期范围文件名 = 指定日期范围.ConvertAll(x => x.ToShortDateString().Replace(@"/", "-"));
            List<消息记录> 返回值 = new List<消息记录>();
            try
            {
                foreach (string 文件名 in 指定日期范围文件名)
                {
                    if (File.Exists($@"{目录}{群号}\{文件名}.json"))
                    {
                        返回值.InsertRange(0, JsonConvert.DeserializeObject<List<消息记录>>(读取($@"{目录}{群号}\{文件名}.json")));
                    }
                }
            }
            catch (Exception ex)
            {
                //QMLog.CurrentApi.Error(ex.ToString());
            }
            return 返回值;
        }


        public static void 写入(string 路径, string 内容)
        {
            if (!File.Exists(路径))  // 判断是否已有相同文件 
            {
                FileStream fs1 = new FileStream(路径, FileMode.Create, FileAccess.ReadWrite);
                fs1.Close();
            }
            File.WriteAllText(路径, 内容, Encoding.UTF8);
        }

        public static string 读取(string 路径)
        {
            if (File.Exists(路径))
            {
                return File.ReadAllText(路径, Encoding.UTF8);
            }
            else
            {
                //QMLog.CurrentApi.Warning($"未找到{路径}！");
                return null;
            }
        }
    }
}
