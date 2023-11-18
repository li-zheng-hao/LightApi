namespace LightApi.Core.Helper;

/// <summary>
/// 流水号生成类
/// </summary>
public class SequenceHelper
{
    /// <summary>
    /// 10进制转2-36进制
    /// Converts the given decimal number to the numeral system with the
    /// specified radix (in the range [2, 36]).
    /// </summary>
    /// <param name="decimalNumber">The number to convert.</param>
    /// <param name="radix">The radix of the destination numeral system (in the range [2, 36]).</param>
    /// <returns></returns>
    public static string DecimalToArbitrarySystem(long decimalNumber, int radix=36)
    {
        const int BitsInLong = 64;
        const string Digits = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";

        if (radix < 2 || radix > Digits.Length)
            throw new ArgumentException("The radix must be >= 2 and <= " + Digits.Length.ToString());

        if (decimalNumber == 0)
            return "0";

        int index = BitsInLong - 1;
        long currentNumber = Math.Abs(decimalNumber);
        char[] charArray = new char[BitsInLong];

        while (currentNumber != 0)
        {
            int remainder = (int)(currentNumber % radix);
            charArray[index--] = Digits[remainder];
            currentNumber = currentNumber / radix;
        }

        string result = new String(charArray, index + 1, BitsInLong - index - 1);
        if (decimalNumber < 0)
        {
            result = "-" + result;
        }

        return result;
    }
    
    /// <summary>
    /// 2-36进制的数转换成10进制
    /// Converts the given number from the numeral system with the specified
    /// radix (in the range [2, 36]) to decimal numeral system.
    /// </summary>
    /// <param name="number">The arbitrary numeral system number to convert.</param>
    /// <param name="radix">The radix of the numeral system the given number
    /// is in (in the range [2, 36]).</param>
    /// <returns></returns>
    public static long ArbitraryToDecimalSystem(string number, int radix=36)
    {
        const string Digits = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";

        if (radix < 2 || radix > Digits.Length)
            throw new ArgumentException("The radix must be >= 2 and <= " +
                                        Digits.Length.ToString());

        if (String.IsNullOrEmpty(number))
            return 0;

        // Make sure the arbitrary numeral system number is in upper case
        number = number.ToUpperInvariant();

        long result = 0;
        long multiplier = 1;
        for (int i = number.Length - 1; i >= 0; i--)
        {
            char c = number[i];
            if (i == 0 && c == '-')
            {
                // This is the negative sign symbol
                result = -result;
                break;
            }

            int digit = Digits.IndexOf(c);
            if (digit == -1)
                throw new ArgumentException(
                    "Invalid character in the arbitrary numeral system number",
                    "number");

            result += digit * multiplier;
            multiplier *= radix;
        }

        return result;
    }
}