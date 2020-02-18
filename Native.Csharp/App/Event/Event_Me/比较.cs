using System;
using System.Linq;

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
        public static decimal 相似(string 原本, string 标本)
        {

            decimal 权重甲 = 2;
            decimal 权重乙 = 1;
            decimal 权重丙 = 1;

            char[] 阴 = 原本.ToCharArray();
            char[] 阳 = 标本.ToCharArray();

            int 甲 = 阴.Intersect(阳).Count();
            int 乙 = 阴.Length - 甲;
            int 丙 = 阳.Length - 甲;

            int 匹配 = 0; int 长度 = 0; int 字数 = 0;
            if (原本.Length >= 标本.Length)
            {
                长度 = 标本.Length;
                字数 = 原本.Length;
            }
            else
            {
                长度 = 原本.Length;
                字数 = 标本.Length;
            }
            for (int 位置 = 0; 位置 < 长度; 位置++)
            {
                if (原本[位置] == 标本[位置])
                {
                    匹配++;
                }
            }

            return 权重甲 * 甲 / (权重甲 * 甲 + 权重乙 * 丙 + 权重丙 * 乙) * (匹配 + 字数*4) / (字数 + 字数*4);
        }
    }
}
