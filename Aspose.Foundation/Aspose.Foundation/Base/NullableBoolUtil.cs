// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 08/06/2023 by Edward Voronov

using System;

namespace Aspose
{
    public static class NullableBoolUtil
    {
        public static NullableBool AsNullable(bool value)
        {
            return value
                ? NullableBool.True
                : NullableBool.False;
        }

        public static bool HasValue(NullableBool value)
        {
            return value != NullableBool.NotDefined;
        }

        public static bool GetValue(NullableBool value)
        {
            if (HasValue(value))
                return value == NullableBool.True;

            throw new InvalidOperationException();
        }

        public static bool GetValueOrDefault(NullableBool value, bool defaultValue)
        {
            return HasValue(value)
                ? GetValue(value)
                : defaultValue;
        }
    }
}
