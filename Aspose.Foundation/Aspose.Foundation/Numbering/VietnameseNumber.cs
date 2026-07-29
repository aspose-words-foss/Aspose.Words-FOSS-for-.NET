// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 22/07/2019 by Edward Voronov

using System.Collections.Generic;

namespace Aspose.Numbering
{
    /// <summary>
    /// This class is a helper for <see cref="NumberConverterCore"/>,
    /// its method converts a number into Vietnamese numbering style.
    /// </summary>
    internal static class VietnameseNumber
    {
        internal static string ToVietnameseCardinal(long value)
        {
            if (value < 0)
                return string.Empty;

            if (value == 0 || value > 1000)
                return Nothing;

            if (value == 1000)
                return gDigits[1] + Separator + Thousands;

            List<int> digits = GroupSplitter.SplitToGroups(value, 1);

            int ones = digits[0];
            int tens = digits.Count > 1 ? digits[1] : 0;
            int hundreds = digits.Count > 2 ? digits[2] : 0;

            List<string> builder = new List<string>();

            if (hundreds != 0)
            {
                builder.Add(gDigits[hundreds]);
                builder.Add(Hundreds);
            }

            if (tens != 0)
            {
                if (tens == 1)
                {
                    builder.Add(Ten);
                }
                else
                {
                    builder.Add(gDigits[tens]);
                    builder.Add(Tens);
                }
            }

            if (ones != 0)
            {
                if (hundreds == 1 && tens == 0)
                    builder.Add(OnesPrefix);

                if (ones == 1 && tens > 1)
                    builder.Add(OneOverride);
                else if (ones == 5 && tens != 0)
                    builder.Add(FiveOverride);
                else
                    builder.Add(gDigits[ones]);
            }

            // alexnosk: String.Join(string, IEnumerable<string>) overload is available starting from .NET 4.0.
#if !NET40 || CPLUSPLUS
            return string.Join(Separator, builder.ToArray());
#else
            return string.Join(Separator, builder);
#endif
        }

        private static readonly string[] gDigits =
        {
            null, "một", "hai", "ba", "bốn", "năm", "sáu", "bảy", "tám", "chín"
        };

        private const string OneOverride = "mốt";
        private const string FiveOverride = "lăm";

        private const string Nothing = "không";
        private const string OnesPrefix = "lẻ";
        private const string Ten = "mười";
        private const string Tens = "mươi";
        private const string Hundreds = "trăm";
        private const string Thousands = "ngàn";

        private const string Separator = " ";
    }
}
