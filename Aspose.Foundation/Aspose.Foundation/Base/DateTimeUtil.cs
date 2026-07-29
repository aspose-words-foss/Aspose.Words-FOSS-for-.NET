// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 08/12/2011 by Andrey Soldatov

using System;

namespace Aspose
{
    /// <summary>
    /// Date and time related utility methods.
    /// </summary>
    public static class DateTimeUtil
    {
        /// <summary>
        /// <para>Sets Test flag. This flag is used to make behavior of some methods deterministic for test purposes.
        /// Call this method before running any tests which can be connected to methods of this class.</para>
        /// <para>For now, it's used by <see cref="GetNow"/> and <see cref="ToLocalTime"/> only
        /// to ensure hardcoded date/time in the updated fields so the golds remain unchanged.</para>
        /// </summary>
        public static void SetTestMode()
        {
            gTestMode = true;
        }

        /// <summary>
        /// You should mark tests that don't use test mode as NonParallelizable.
        /// </summary>
        public static void SetTestMode(bool testMode)
        {
            gTestMode = testMode;
        }

        /// <summary>
        /// Use this method instead of DateTime.FromFileTimeUtc because it is only available in .NET 1.1.
        /// This code is disassembled from mscorlib.
        /// </summary>
        public static DateTime FromFileTimeUtc(long fileTime, string paramName)
        {
            if (fileTime < 0)
                throw new ArgumentOutOfRangeException(paramName);

            long num1 = fileTime + Win32Epoch;
            return new DateTime(num1, DateTimeKind.Utc);
        }

        /// <summary>
        /// Use this method instead of DateTime.ToFileTimeUtc because it is only available in .NET 1.1.
        /// This code is disassembled from mscorlib.
        /// </summary>
        public static long ToFileTimeUtc(DateTime value, string paramName)
        {
            long num1 = value.Ticks - Win32Epoch;
            if (num1 < 0)
                throw new ArgumentOutOfRangeException(paramName);

            return num1;
        }

        /// <summary>
        /// Returns the current date/time if running is for "real". Returns a hardcoded constant if running
        /// unit tests which allows unit tests with golds to remain without changes.
        /// </summary>
        public static DateTime GetNow()
        {
            if (gTestMode)
                return UnitTestingDateTime;

            return DateTime.Now;
        }

        /// <summary>
        /// Returns the specified date/time as is or converts it to local time depending on whether unit testing is being
        /// performed.
        /// </summary>
        public static DateTime ToLocalTime(DateTime dateTime)
        {
            if (gTestMode)
                return dateTime;

            return dateTime.ToLocalTime();
        }

        /// <summary>
        /// Returns the specified date/time as is or converts it to universal time depending on whether unit testing is being
        /// performed.
        /// </summary>
        public static DateTime ToUniversalTime(DateTime dateTime)
        {
            if (gTestMode)
                return dateTime;

            return dateTime.ToUniversalTime();
        }

        /// <summary>
        /// All tests which can be connected to methods depending on this flag must call <see cref="SetTestMode"/>
        /// to set this flag to <c>true</c>.
        /// Neither results of tests nor release code in .NET or Java
        /// will change (if <see cref="SetTestMode"/> was not called from release code that MUST NOT be done).
        /// </summary>
        private static bool gTestMode = false;

        public static readonly DateTime UnitTestingDateTime = new DateTime(2006, 1, 5, 19, 9, 1);

        private const long Win32Epoch = 0x701ce1722770000;
    }
}
