using MySql.Data.MySqlClient;
using System;

namespace Native.Csharp.App.Event.Event_Me
{
    class MySql
    {
        public static string 查(string 连接语句, string 操作语句)
        {
            MySqlConnection 连接 = new MySqlConnection(连接语句);
            连接.Open();
            MySqlCommand 命令 = new MySqlCommand(操作语句, 连接);
            MySqlDataReader 读取 = 命令.ExecuteReader();
            读取.Read();
            object 键 = 读取.GetValue(0);
            object 值 = 读取.GetValue(1);
            //var debug00 = 读取.GetName(0);
            //var debug01 = 读取.GetName(1);
            string 返回值 = $"键：{键}{Environment.NewLine}值：{值}";
            连接.Close();
            return 返回值;
        }

        public static void DND3R字典()
        {
            string 连接语句 = "Host=bj-cdb-0mud5lvw.sql.tencentcdb.com;User ID=future;Password=VA5a5rTSfnMG2G97euC92ofHUZIyf899h;Port = 61922;DataBase=dnd;";
            string 操作语句 = "select * from 3ry";
            MySqlConnection 连接 = new MySqlConnection(连接语句);
            连接.Open();
            MySqlCommand 命令 = new MySqlCommand(操作语句, 连接);
            MySqlDataReader 读取 = 命令.ExecuteReader();
            while (读取.Read())
            {
                var 键 = 读取.GetString(0);
                var 值 = 读取.GetString(1);
                数据.实体["DND3R"].Add(键, 值);
            }
            连接.Close();
            return;
        }
    }
}
