using System;

namespace Cassiopeia.Common
{
    public static class ByteArrayExtensions
    {
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