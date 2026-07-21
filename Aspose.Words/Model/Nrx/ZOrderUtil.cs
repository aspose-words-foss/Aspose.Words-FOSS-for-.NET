// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 31/01/2017 by Alexey Butalov

namespace Aspose.Words.Nrx
{
    internal static class ZOrderUtil
    {
        /// <summary>
        /// Converts z-index that Word uses into z-order used in Aspose.Words.
        /// </summary>
        internal static int ZIndexToZOrder(int zIndex, bool isBehindText)
        {
            return zIndex - (isBehindText ? ZIndexBaseBehind : ZIndexBase);
        }

        /// <summary>
        /// Converts z-order that Aspose.Words uses into z-index used in Word.
        /// </summary>
        internal static int ZOrderToZIndex(int zOrder, bool isBehindText)
        {
            return (zOrder * 0x0400) + (isBehindText ? ZIndexBaseBehind : ZIndexBase);
        }

        /// <summary>
        /// Converts Int64 to Int32 by truncating to Int32.Min and Int32.Max values.
        /// </summary>
        internal static int TruncateLongToInt(long longValue)
        {
            return (longValue < int.MinValue)
                ? int.MinValue
                : (longValue > int.MaxValue) ? int.MaxValue : (int)longValue;
        }

        /// <summary>
        /// Base z-index value Word uses for shapes behind text.
        /// </summary>
        private const int ZIndexBaseBehind = unchecked((int)0xf1000000);

        /// <summary>
        /// Base z-index value Word uses for shapes in front of text.
        /// </summary>
        private const int ZIndexBase = 0x0f000000;
    }
}
