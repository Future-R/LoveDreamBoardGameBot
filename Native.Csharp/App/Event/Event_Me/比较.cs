using System;

namespace Native.Csharp.App.Event.Event_Me
{
    public class 比较
    {
        public static bool 等于(string 参数1, string 参数2)
        {
            参数1 = 数据.读取组件(参数1);
            if (参数1.Trim() == 参数2.Trim())
            {
                return true;
            }
            return false;
        }
        public static bool 大于(string 参数1, string 参数2)
        {
            参数1 = 数据.读取组件(参数1);
            if (Convert.ToDecimal(参数1.Trim()) > Convert.ToDecimal(参数2.Trim()))
            {
                return true;
            }
            return false;
        }
        public static bool 小于(string 参数1, string 参数2)
        {
            参数1 = 数据.读取组件(参数1);
            if (Convert.ToDecimal(参数1.Trim()) < Convert.ToDecimal(参数2.Trim()))
            {
                return true;
            }
            return false;
        }
        public static bool 包含(string 参数1, string 参数2)
        {
            参数1 = 数据.读取组件(参数1);
            if (参数1.Trim().Contains(参数2.Trim()))
            {
                return true;
            }
            return false;
        }
        public static bool 开头是(string 参数1, string 参数2)
        {
            参数1 = 数据.读取组件(参数1);
            if (参数1.Trim().StartsWith((参数2.Trim())))
            {
                return true;
            }
            return false;
        }
        public static bool 结尾是(string 参数1, string 参数2)
        {
            参数1 = 数据.读取组件(参数1);
            if (参数1.Trim().EndsWith((参数2.Trim())))
            {
                return true;
            }
            return false;
        }
        public static bool 有(string 参数1, string 参数2)
        {
            if (!数据.实体.ContainsKey(参数1))
            {
                return false;
            }
            if (数据.实体[参数1].ContainsKey(参数2))
            {
                return true;
            }
            return false;
        }
    }
}
