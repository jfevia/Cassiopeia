using System;
using System.IO;
using System.Text;

namespace Cassiopeia.Common
{
    public static class ByteArrayExtensions
    {
        private static readonly char[] HexChars = "0123456789abcdef".ToCharArray();

        public static string ToUrlEncode(this byte[] bytes)
        {
            if (bytes == null)
                throw new ArgumentNullException(nameof(bytes));

            var result = new MemoryStream(bytes.Length);
            foreach (var b in bytes) UrlEncode((char)b, result, false);

            return Encoding.ASCII.GetString(result.ToArray());
        }

        private static bool NotEncoded(char c)
        {
            return c == '!' || c == '(' || c == ')' || c == '*' || c == '-' || c == '.' || c == '_' || c == '\'';
        }

        private static void UrlEncode(char c, Stream result, bool isUnicode)
        {
            if (c > ' ' && NotEncoded(c))
            {
                result.WriteByte((byte)c);
                return;
            }
            if (c == ' ')
            {
                result.WriteByte((byte)'+');
                return;
            }
            if (c < '0' || c < 'A' && c > '9' || c > 'Z' && c < 'a' || c > 'z')
            {
                if (isUnicode && c > 127)
                {
                    result.WriteByte((byte)'%');
                    result.WriteByte((byte)'u');
                    result.WriteByte((byte)'0');
                    result.WriteByte((byte)'0');
                }
                else
                {
                    result.WriteByte((byte)'%');
                }

                var idx = c >> 4;
                result.WriteByte((byte)HexChars[idx]);
                idx = c & 0x0F;
                result.WriteByte((byte)HexChars[idx]);
            }
            else
            {
                result.WriteByte((byte)c);
            }
        }

        public static bool ByteMatch(this byte[] originalArray, byte[] targetArray)
        {
            if (originalArray == null)
                throw new ArgumentNullException(nameof(originalArray));

            if (targetArray == null)
                throw new ArgumentNullException(nameof(targetArray));

            if (originalArray.Length != targetArray.Length)
                return false;

            return ByteMatch(originalArray, 0, targetArray, 0, originalArray.Length);
        }

        public static bool ByteMatch(this byte[] originalArray, int originalOffset, byte[] targetArray, int targetOffset,
            int count)
        {
            if (originalArray == null)
                throw new ArgumentNullException(nameof(originalArray));

            if (targetArray == null)
                throw new ArgumentNullException(nameof(targetArray));

            // If either of the arrays is too small, they're not equal
            if (originalArray.Length - originalOffset < count || targetArray.Length - targetOffset < count)
                return false;

            // Check if any element is not equal
            for (var i = 0; i < count; i++)
                if (originalArray[originalOffset + i] != targetArray[targetOffset + i])
                    return false;

            return true;
        }
    }
}