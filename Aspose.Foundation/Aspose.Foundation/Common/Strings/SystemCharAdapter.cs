// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 15/01/2020 by Edward Voronov

using System.Collections.Generic;
using System.Diagnostics;

namespace Aspose.Common
{
    /// <summary>Adapts <see cref="System.Char"/> to <see cref="IChar"/>.</summary>
    [DebuggerDisplay("{Value}")]
    public class SystemCharAdapter : IChar
    {
        private SystemCharAdapter(char value)
        {
            Value = value;
        }

        public static SystemCharAdapter Create(char value)
        {
            lock (gLock)
            {
                SystemCharAdapter result;
                if (!gCache.TryGetValue(value, out result))
                {
                    result = new SystemCharAdapter(value);
                    gCache[value] = result;
                }

                return result;
            }
        }

        public char ToSystemChar()
        {
            return Value;
        }

        IChar IChar.ToUpper()
        {
            return ToUpperInternal();
        }

        public SystemCharAdapter ToUpperInternal()
        {
            return new SystemCharAdapter(char.ToUpper(Value));
        }

        IChar IChar.ToLower()
        {
            return ToLowerInternal();
        }

        public SystemCharAdapter ToLowerInternal()
        {
            return new SystemCharAdapter(char.ToLower(Value));
        }

        public char Value { get; }

        private static readonly IDictionary<char, SystemCharAdapter> gCache = new Dictionary<char, SystemCharAdapter>();
        private static readonly object gLock = new object();
    }
}
