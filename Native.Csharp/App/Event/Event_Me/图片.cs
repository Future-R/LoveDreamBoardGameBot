using System;
using System.Collections.Generic;
using System.Drawing;

namespace Native.Csharp.App.EventArgs
{
    public class 图片
    {
        private string 文本 = "";
        private int 文字上限 = 1000;

        private int 行宽 = 1000;
        private int 行高 = 48;
        private int 行间距 = 8;
        private int 出血宽度 = 40;
        private int 出血高度 = 40;

        private string 字体 = 数据.正在使用的字体;
        private int 字体大小 = 24;
        private Color 字体颜色 = Color.Black;
        private Color 背景色 = Color.White;

        public 图片(string 传入文本)
        {
            文本 = 传入文本;
        }

        public 图片 出血宽高为(int 正整数甲, int 正整数乙)
        {
            this.出血宽度 = 正整数甲;
            this.出血高度 = 正整数乙;
            return this;
        }

        public 图片 行宽高为(int 正整数甲, int 正整数乙)
        {
            this.行宽 = 正整数甲;
            this.行高 = 正整数乙;
            return this;
        }

        public 图片 行间距为(int 正整数)
        {
            this.行间距 = 正整数;
            return this;
        }

        public 图片 文字上限为(int 正整数)
        {
            this.文字上限 = 正整数;
            return this;
        }

        public 图片 字体为(string 字符串)
        {
            this.字体 = 字符串;
            return this;
        }

        public 图片 字体大小为(int 正整数)
        {
            this.字体大小 = 正整数;
            return this;
        }

        public 图片 字体颜色为(Color 颜色)
        {
            this.字体颜色 = 颜色;
            return this;
        }

        public 图片 背景色为(Color 颜色)
        {
            this.背景色 = 颜色;
            return this;
        }

        public Bitmap 开始生成()
        {
            // 文字上限
            if (文本.Length > 文字上限)
            {
                文本 = 文本.Substring(0, 文字上限) + "...";
            }

            // 设置字体
            Font font = new Font(字体, 字体大小, FontStyle.Regular);

            // 设置字体颜色
            SolidBrush solidBrush = new SolidBrush(字体颜色);

            // 预计算画板宽高

            int 非全角字符数 = 0;
            int 全角字符数 = 0;
            foreach (char ch in 文本)
            {
                // 筛选汉字与正常的Aiisc值用于计算宽度
                if (ch >= 255)
                {
                    全角字符数++;
                }
                else
                {
                    非全角字符数++;
                }
            }

            int 画板宽度 = Convert.ToInt32(Math.Ceiling(全角字符数 * 字体大小 * 1.6) + Math.Ceiling(非全角字符数 * 字体大小 * 0.8));
            int 画板高度 = 行高 + (出血高度 * 2);

            // 自动换行
            List<string> 换行结果 = 文字.换行(文本, 行宽, font, new Bitmap(画板宽度, 画板高度));
            int 行数 = 换行结果.Count;


            画板高度 = ((行高 + 行间距) * 行数) - 行间距 + (出血高度 * 2);
            画板宽度 = 行宽 + (出血宽度 * 2);

            // 因为手机QQ显示宽图时，会把后面截断。所以这里尝试把宽度缩小再进行计算，看结果会不会使边长更短。
            while (画板宽度 > 画板高度 * 2.5)
            {
                行宽 = Convert.ToInt32(Math.Ceiling(行宽 * 0.75));
                换行结果 = 文字.换行(文本, 行宽, font, new Bitmap(画板宽度, 画板高度));
                行数 = 换行结果.Count;
                画板宽度 = 行宽 + (出血宽度 * 2);
                画板高度 = ((行高 + 行间距) * 行数) - 行间距 + (出血高度 * 2);
            }
            int 原边长 = 画板宽度 + 画板高度;
            int 现边长 = 原边长;
            int 临时行宽 = 行宽;
            int 尝试步长 = 40;
            List<string> 尝试换行结果;
            while (现边长 <= 原边长)
            {
                临时行宽 -= 尝试步长;
                尝试换行结果 = 文字.换行(文本, 临时行宽, font, new Bitmap(画板宽度, 画板高度));
                现边长 = ((行高 + 行间距) * 尝试换行结果.Count) - 行间距 + (出血高度 * 2)
                    + 临时行宽 + (出血宽度 * 2);
                if (现边长 <= 原边长)
                {
                    换行结果 = 尝试换行结果;
                    行数 = 换行结果.Count;
                    行宽 = 临时行宽;
                    原边长 = 现边长;
                }
            }
            画板宽度 = 行宽 + (出血宽度 * 2);
            画板高度 = ((行高 + 行间距) * 行数) - 行间距 + (出血高度 * 2);
            

            //if (行数 == 1 && 画板宽度 > (行宽 * 0.5))
            //{
            //    行宽 = Convert.ToInt32(Math.Ceiling(行宽 * 0.42));
            //    画板宽度 = 行宽 + (出血宽度 * 2);
            //    画板高度 = ((行高 + 行间距) * 4) - 行间距 + (出血高度 * 2);
            //    换行结果 = 文字.换行(文本, 行宽, font, new Bitmap(画板宽度, 画板高度));
            //    行数 = 换行结果.Count;
            //    画板高度 = ((行高 + 行间距) * 行数) - 行间距 + (出血高度 * 2);
            //}

            //创建图片
            Bitmap 返回值 = new Bitmap(画板宽度, 画板高度);

            //创建画板
            Graphics graphics = Graphics.FromImage(返回值);

            //设置背景色
            graphics.Clear(背景色);



            //文本相对坐标
            float X = 0;
            float Y = 0;

            //逐行写字
            X += 出血宽度;
            Y += 出血高度;
            foreach (string 一行文本 in 换行结果)
            {
                graphics.DrawString(一行文本, font, solidBrush, X, Y);
                Y += 行高 + 行间距;
            }
            graphics.Dispose();

            return 返回值;
        }
    }
}
