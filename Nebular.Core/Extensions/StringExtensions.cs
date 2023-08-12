using System;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Nebular.Core.Extensions
{
    public static class StringExtensions
    {
        public static string FirstCharToUpper(this string input)
        {
            switch (input)
            {
                case null: throw new ArgumentNullException(nameof(input));
                case "": throw new ArgumentException($"{nameof(input)} cannot be empty", nameof(input));
                default: return input.First().ToString().ToUpper() + input.Substring(1);
            }
        }
        public static int CountOccurences(this string str, char chr, int startIndex, int count)
        {
            int occurences = 0;
            for (int i = startIndex; i < startIndex + count; i++)
            {
                if (str[i] == chr)
                {
                    occurences++;
                }
            }
            return occurences;
        }
        public static string ConcatCopy(this string str, int times)
        {
            StringBuilder builder = new StringBuilder(str.Length * times);
            for (int i = 0; i < times; i++)
            {
                builder.Append(str);
            }
            return builder.ToString();
        }
        public static string RandomString(this Random random, int size)
        {
            StringBuilder stringBuilder = new StringBuilder();
            for (int i = 0; i < size; i++)
            {
                stringBuilder.Append(Convert.ToChar(Convert.ToInt32(Math.Floor(26.0 * random.NextDouble() + 65.0))));
            }
            return stringBuilder.ToString();
        }
        public static string UpperAfterChar(this string @string, char @char)
        {
            StringBuilder result = new StringBuilder(@string.Length);
            bool makeUpper = true;
            foreach (var c in @string)
            {
                if (makeUpper)
                {
                    result.Append(Char.ToUpper(c));
                    makeUpper = false;
                }
                else
                {
                    result.Append(c);
                }
                if (c == @char)
                {
                    makeUpper = true;
                }
            }
            return result.ToString();

        }
        public static string RemoveNumbers(this string input)
        {
            return Regex.Replace(input, @"[\d-]", string.Empty);
        }
        public static string RemoveLetters(this string input)
        {
            return Regex.Replace(input, @"[^0-9.]", string.Empty);
        }
        public static string RemoveChars(this string input, params char[] chars)
        {
            string result = input;
            foreach (var @char in chars)
            {
                result = result.Replace(@char.ToString(), string.Empty);
            }
            return result;
        }
        public static string RemoveAccents(this string source)
        {
            return string.Concat(
                source.Normalize(NormalizationForm.FormD)
                      .Where(ch => CharUnicodeInfo.GetUnicodeCategory(ch) !=
                                   UnicodeCategory.NonSpacingMark)
                ).Normalize(NormalizationForm.FormC);
        }

        public static bool HasAccents(this string source)
        {
            return
                source.Normalize(NormalizationForm.FormD)
                      .Any(x => CharUnicodeInfo.GetUnicodeCategory(x) == UnicodeCategory.NonSpacingMark);
        }

    }

}
