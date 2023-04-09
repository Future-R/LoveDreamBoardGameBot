using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text.RegularExpressions;

namespace Native.Csharp.App.EventArgs
{
    public static class 文字
    {
        public static List<string> 换行(string 源文本, int 行宽上限, Font 字体, Bitmap bitmap)
        {
            List<string> 返回值 = new List<string>();
            // 不能放在开头或结尾的符号。随便写了几个，肯定不全，随时补充：
            string 避头点集合 = "!),.:;?]}¨·ˇˉ―‖’”…∶、。〉》」』】〕〗！＂＇），．：；？］｀｜｝～￠";
            string 避尾点集合 = "([{·‘“〈《「『【〔〖（．［｛￡￥";
            int 悬挂宽度 = 40;
            Graphics graphics = Graphics.FromImage(bitmap);

            string[] 手动换行集合 = Regex.Split(源文本, Environment.NewLine);
            // 如果一行就能放下，直接返回了
            if (手动换行集合 != null && 手动换行集合.Length == 1 && graphics.MeasureString(源文本, 字体).Width <= 行宽上限)
            {
                返回值.Add(源文本);
                return 返回值;
            }

            // 遍历手动换行的每行，如果能直接放下，那就直接Add；否则，逐字判断是否需要自动换行。
            foreach (var 文本 in 手动换行集合)
            {
                if (graphics.MeasureString(文本, 字体).Width <= 行宽上限)
                {
                    返回值.Add(文本);
                }
                else
                {
                    string 剩余文本 = 文本;
                    string 当前文本 = "";
                    // 只要字不写完就一直写
                    while (!string.IsNullOrEmpty(剩余文本))
                    {
                        // 往后写一个字
                        当前文本 += 剩余文本[0];
                        float 当前文本的宽度 = graphics.MeasureString(当前文本, 字体).Width;
                        // 如果往后写的字超出了行宽
                        if (当前文本的宽度 > 行宽上限)
                        {
                            // 如果才写了一个字就超出行宽，报错
                            if (当前文本.Length < 2)
                            {
                                //QMLog.CurrentApi.Error($"当前文本为{当前文本}");
                                return null;
                            }

                            // 本行回退一个字，存为一行
                            string 储存一行 = 当前文本[0..^1];
                            // 新行只保留最后一个字
                            当前文本 = 当前文本[^1].ToString();

                            // 数字&单词防切断：Todo

                            // 标点悬挂：
                            // 如果新行首个字符是需要避头的，尝试放到上一行的末尾，看看能不能挂；
                            //     如果能挂，且下一个字也不是避头字符，就挂过去；
                            //         如果下一个字还是避头字符，再分两种情况：
                            //             1.如果上一个字不是避头字符，那么把上一个字移到下一行的开头；
                            //             2.否则，放弃治疗。
                            if (避头点集合.Contains(当前文本))
                            {
                                if (graphics.MeasureString(储存一行 + 当前文本, 字体).Width <= (行宽上限 + 悬挂宽度))
                                {
                                    if (剩余文本.Length > 1 && !避头点集合.Contains(剩余文本[1]))
                                    {
                                        储存一行 += 当前文本;
                                        当前文本 = "";
                                    }
                                    else
                                    {
                                        if (!避头点集合.Contains(储存一行[^1]))
                                        {
                                            当前文本 = $"{储存一行[^1]}{当前文本}";
                                            储存一行 = 储存一行[0..^1];
                                        }
                                    }
                                }
                            }

                            // 标点避尾：Todo

                            返回值.Add(储存一行);

                        }
                        // 待写的字去掉一个
                        剩余文本 = 剩余文本.Remove(0, 1);
                    }
                    if (!string.IsNullOrEmpty(当前文本))
                    {
                        返回值.Add(当前文本);
                    }
                }
            }
            return 返回值;
        }
    }
}
