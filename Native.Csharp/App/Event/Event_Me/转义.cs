namespace Native.Csharp.App.Event.Event_Me
{
    class 转义
    {
        public static string 内部输入(string 输入内容)
        {
            return 输入内容.Replace("。", "☯")
                .Replace("的", "☴").Replace("是", "☲").Replace("且", "☷").Replace("或", "☱")
                .Replace("，", "☰").Replace("、", "☵").Replace("！", "☶").Replace("；", "☳");
        }

        public static string 输入(string 输入内容)
        {
            return 输入内容.Replace("。。", "☯")
                .Replace("的的", "☴").Replace("是是", "☲").Replace("且且", "☷").Replace("或或", "☱")
                .Replace("，，", "☰").Replace("、、", "☵").Replace("！！", "☶").Replace("；；", "☳")
                .Replace("&#91;", "[").Replace("&#93;", "]");
        }

        public static string 输出(string 输出内容)
        {
            return 输出内容.Replace("☯", "。")
                .Replace("☴", "的").Replace("☲", "是").Replace("☷", "且").Replace("☱", "或")
                .Replace("☰", "，").Replace("☵", "、").Replace("☶", "！").Replace("☳", "；");
        }
    }
}
