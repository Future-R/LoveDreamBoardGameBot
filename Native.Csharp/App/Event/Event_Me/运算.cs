using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace Native.Csharp.App.Event.Event_Me
{
    public class 运算
    {
        public static void 比较运算(string 算式)
        {
            bool 声明组件 = true;
            if (算式.Contains("大") || 算式.Contains("多"))
            {
                if (!算式.Contains("的"))//没有声明组件，自动填充为"值"
                {
                    声明组件 = false;
                }
                List<string> 分词 = new List<string>(算式.Split(new[] { '比', '大', '多', '的' }, StringSplitOptions.RemoveEmptyEntries));

                if (算式.IndexOf("的") == 算式.LastIndexOf("的"))//如果只有一个"的"
                {
                    if (插二(算式, "比")) 分词.Insert(1, 分词[2]);
                    else 分词.Insert(3, 分词[1]);
                }

                if (!声明组件)
                {
                    分词.Insert(1, "值"); 分词.Insert(3, "值");
                }
                if (!是纯数(分词[4]))
                {
                    分词[4] = 数据.实体[分词[4]]["值"];
                }
                if (!数据.实体.ContainsKey(分词[0]))//如果没有这个实体就创建实体，并添加对应的组件
                {
                    数据.实体.Add(分词[0], new Dictionary<string, string>());
                    数据.实体[分词[0]].Add(分词[1], new DataTable().Compute(数据.实体[分词[2]][分词[3]] + "+" + 分词[4], "").ToString());
                }
                else
                {
                    数据.实体[分词[0]][分词[1]] = new DataTable().Compute(数据.实体[分词[2]][分词[3]] + "+" + 分词[4], "").ToString();
                }
            }
            else
            {
                if (!算式.Contains("的"))
                {
                    声明组件 = false;
                }
                List<string> 分词 = new List<string>(算式.Split(new[] { '比', '小', '少', '的' }, StringSplitOptions.RemoveEmptyEntries));
                if (!声明组件)
                {
                    分词.Insert(1, "值"); 分词.Insert(3, "值");
                }
                if (!是纯数(分词[4]))
                {
                    分词[4] = 数据.实体[分词[4]]["值"];
                }
                if (!数据.实体.ContainsKey(分词[0]))//如果没有这个实体就创建实体，并添加对应的组件
                {
                    数据.实体.Add(分词[0], new Dictionary<string, string>());
                    数据.实体[分词[0]].Add(分词[1], new DataTable().Compute(数据.实体[分词[2]][分词[3]] + "-" + 分词[4], "").ToString());
                }
                else
                {
                    数据.实体[分词[0]][分词[1]] = new DataTable().Compute(数据.实体[分词[2]][分词[3]] + "-" + 分词[4], "").ToString();
                }
            }
            return;
        }

        public static void 相同运算(string 算式)
        {
            string 模式 = "相同";
            if (算式.EndsWith("一样"))
            {
                算式 = 算式.TrimEnd('样').TrimEnd('一') + "相同";
            }
            if (算式.Contains("相反")) 模式 = "相反";
            
            List<string> 分词 = new List<string>(算式.Split(new string[] { "和", "与", "的", 模式 }, StringSplitOptions.RemoveEmptyEntries));

            if (算式.IndexOf("的") == 算式.LastIndexOf("的"))//如果只有一个"的"
            {
                if (插二(算式, "和")) 分词.Insert(1, 分词[2]);
                else 分词.Insert(3, 分词[1]);
            }

            if (算式.Contains("的"))
            {
                if (!数据.实体.ContainsKey(分词[0]))//如果没有这个实体就创建实体，并添加对应的组件
                {
                    数据.实体.Add(分词[0], new Dictionary<string, string>());
                    数据.实体[分词[0]].Add(分词[1], 数据.实体[分词[2]][分词[3]]);
                }
                else
                {
                    数据.实体[分词[0]][分词[1]] = 数据.实体[分词[2]][分词[3]];
                }
                if (模式 == "相反")
                {
                    int 位置 = 数据.反义词典.IndexOf(数据.实体[分词[0]][分词[1]]);
                    if (位置 % 2 == 0)
                    {
                        数据.实体[分词[0]][分词[1]] = 数据.反义词典[位置 + 1];
                    }
                    else
                    {
                        数据.实体[分词[0]][分词[1]] = 数据.反义词典[位置 - 1];
                    }
                }
            }
            else//组件未声明，覆盖复制实体
            {
                if (!数据.实体.ContainsKey(分词[0]))
                {
                    数据.实体.Add(分词[0], 数据.实体[分词[1]]);
                }
                else
                {
                    数据.实体[分词[0]] = 数据.实体[分词[1]];
                }
            }
            return;
        }

        public static string 计算(string 算式)
        {
            算式 = 算式.Replace("×", "*").Replace("x", "*").Replace("X", "*")
                .Replace("（", "(").Replace("）", ")").Replace("÷", "/");
            //此处应替换为波兰逆运算
            return new DataTable().Compute(算式.Trim(), "").ToString();
        }

        public static bool 是纯数(string str) //接收一个string类型的参数,保存到str里
        {
            str = str.Replace(".", "").Replace("-", "").Replace("+", "");
            if (str == null || str.Length == 0)    //验证这个参数是否为空
                return false;                           //是，就返回False
            ASCIIEncoding ascii = new ASCIIEncoding();//new ASCIIEncoding 的实例
            byte[] bytestr = ascii.GetBytes(str);         //把string类型的参数保存到数组里

            foreach (byte c in bytestr)                   //遍历这个数组里的内容
            {
                if (c < 48 || c > 57)                          //判断是否为数字
                {
                    return false;                              //不是，就返回False
                }
            }
            return true;                                        //是，就返回True
        }

        public static bool 插二(string 算式, string 比较符)
        {
            if (算式.IndexOf("的") > 算式.IndexOf(比较符))//比较符在"的"前
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        //public static void 等式运算(string 算式)
        //{

        //    return;
        //}
    }
}
