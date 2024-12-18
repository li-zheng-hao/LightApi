namespace LightApi.Infra.Internal;

internal static class StringExtension
{
    /// <summary>
    /// 首字母转大写
    /// </summary>
    /// <param name="source"></param>
    /// <returns></returns>
    public static string ToFirstUpper(this string source)
    {
        if (string.IsNullOrEmpty(source) || char.IsUpper(source[0]))
        {
            return source;
        }

        char[] letters = source.ToCharArray();
        letters[0] = char.ToUpper(letters[0]);
        return new string(letters);
    }
}
