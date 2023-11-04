using System.Text;
using System.Text.RegularExpressions;

namespace LightApi.Core.Extension
{
    public static class StringExtension
    {
        public static bool IsMatch(this string source,string regexStr)
        {
            Regex regex = new(regexStr);
            return regex.IsMatch(source);
        }
        public static bool IsDouble(this string source)
        {
            return double.TryParse(source, out _);
        }

        public static bool IsInt(this string source)
        {
            return int.TryParse(source, out _);
        }

        public static bool IsPositiveInt(this string source)
        {
            bool success = int.TryParse(source, out var num);

            if (!success)
            {
                return false;
            }

            return num > 0;
        }

        public static string ZFill(this string source, int length)
        {
            return source.PadLeft(length, '0');
        }

        public static bool Is24HexString(this string obj)
        {
            return obj?.Length == 24;
        }

        /// <summary>
        /// 过滤中文
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static string FilterChineseChar(this string source)
        {
            Regex p_regex = new Regex("^[\u4e00-\u9fa5]{0,}$");
            StringBuilder builder = new();

            for (var i = 0; i < source.Length; i++)
            {
                if (p_regex.IsMatch(source[i].ToString()) == false)
                {
                    builder.Append(source[i]);
                }
            }

            return builder.ToString();
        }
    }
}