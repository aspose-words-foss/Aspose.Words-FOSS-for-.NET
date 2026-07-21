// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 21/08/2008 by Roman Korchagin

using System;

namespace Aspose.Words.RW.Vml
{
    internal class VmlNameValue
    {
        private VmlNameValue(string name, string value)
        {
            Name = name;
            Value = value;
        }

        internal static VmlNameValue FromString(string s)
        {
            string[] nameValue = s.Split(':');

            if (nameValue.Length < 2)
                return null;

            // WORDSNET-25533 Property names are case-insensitive and we convert them all to lower case for convenience.
            // Property values are case-insensitive too, except for ID values (used by "mso-next-textbox") and the "font-family"
            // property value (font family name).
            // Another property with a case sensitive value is "font" but MS Word doesn't load font family from that property
            // and our code doesn't support it either. As a result, it doesn't matter if we convert that property's value
            // to lower case.
            string name = nameValue[0].Trim().ToLowerInvariant();
            string value = nameValue[1].Trim();
            if ((name != "font-family") && !value.StartsWith("#", StringComparison.Ordinal))
            {
                value = value.ToLowerInvariant();
            }

            return (StringUtil.HasChars(name) && StringUtil.HasChars(value))
                ? new VmlNameValue(name, value)
                : null;
        }

        internal string Name { get; }

        internal string Value { get; }
    }
}
