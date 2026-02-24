namespace Utils
{
    public static class StringExt
    {
        public static int IndexOfNth(this string str, char value, int n)
        {
            var index = -1;
            for (var i = 0; i < n; i++)
            {
                index = str.IndexOf(value, index + 1);
                if (index == -1) return -1;
            }
            return index;
        }
    }
}