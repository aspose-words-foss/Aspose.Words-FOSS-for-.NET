// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 08/06/2023 by Edward Voronov

using System.Globalization;
using Aspose.Common;

namespace Aspose.Words.Fields
{
    /// <summary>
    /// Common utils for bibliography fields.
    /// </summary>
    internal static class FieldBibliographyUtils
    {
        /// <summary>
        /// Parses string representation of LCID.
        /// </summary>
        internal static int GetEffectiveLcid(string value)
        {
            NullableInt32 lcid = TryParseLcid(value);
            if (lcid.HasValue)
                return lcid.Value;

            CultureInfo culture = SystemPal.TryGetCulture(value);
            if (culture != null)
                return culture.LCID;

            return GetCurrentCultureLcid();
        }

        private static NullableInt32 TryParseLcid(string value)
        {
            if (string.IsNullOrEmpty(value))
                return NullableInt32.Null;

            NullableInt32 result = FormatterPal.ParseNullableInt(value);
            if (!result.HasValue)
                return NullableInt32.Null;

            if (result.Value < 0)
                return NullableInt32.Null;

            // same as unchecked((ushort)result.Value)
            int lcid = result.Value % (ushort.MaxValue + 1);
            if (lcid == 0)
                return NullableInt32.Null;

            return lcid.AsNullable();
        }

        /// <summary>
        /// Returns current culture LCID.
        /// </summary>
        /// <returns></returns>
        internal static int GetCurrentCultureLcid()
        {
            return SystemPal.GetCurrentCulture().LCID;
        }
    }
}
