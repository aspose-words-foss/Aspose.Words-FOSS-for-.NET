// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 08/07/2013 by Ivan Lyagin

using System;
using Aspose.JavaAttributes;

namespace Aspose.Collections
{
    /// <summary>
    /// FOR MANUAL PORTING.
    /// 
    /// Provides platform-dependent methods to retrieve hash codes for values of primitive types in Java
    /// and for values of some specific types as strings.
    /// </summary>
    /// <dev>
    /// Although hash code calculation routines are almost the same on both platforms (and hence they could be placed outside 
    /// of a PAL class), their work should be as fast as possible, so we use platform-dependent optimizations and that's why 
    /// this class exists.
    /// 
    /// @SK: please, do not box primitive types on Java to get hash codes, otherwise the whole approach does not make any sense.
    /// </dev>
    [JavaManual("Platform abstraction for generating hash codes for primitive types. Manual porting by design.")]
    public static class HashCodeProviderPal
    {
        public static int GetHashCodeOrdinalIgnoreCase(string value)
        {
            return StringComparer.OrdinalIgnoreCase.GetHashCode(value);
        }

        public static int GetHashCode(byte value)
        {
            return value.GetHashCode();
        }

        public static int GetHashCode(short value)
        {
            return value.GetHashCode();
        }

        public static int GetHashCode(int value)
        {
            return value.GetHashCode();
        }

        public static int GetHashCode(long value)
        {
            return value.GetHashCode();
        }

        public static int GetHashCode(float value)
        {
            return value.GetHashCode();
        }

        public static int GetHashCode(double value)
        {
            return value.GetHashCode();
        }

        public static int GetHashCode(bool value)
        {
            return value.GetHashCode();
        }

        public static int GetHashCode(char value)
        {
            return value.GetHashCode();
        }

        public static int GetHashCode(Guid value)
        {
            return value.GetHashCode();
        }
    }
}
