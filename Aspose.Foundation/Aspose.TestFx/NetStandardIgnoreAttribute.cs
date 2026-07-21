// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 11/02/2019 by Alexey Noskov

using System;
using Aspose.JavaAttributes;

namespace NUnit.Framework
{
    // Attribute used to identify a test that shoul be ignored.
    [JavaDelete("Autoported directly to Java's TestNG analog. No need in Java.")]
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public class NetStandardIgnoreAttribute :
#if NETSTANDARD
        IgnoreAttribute
#else
        Attribute
#endif
    {
        public NetStandardIgnoreAttribute(string reason)
#if NETSTANDARD
            : base(reason)
#endif
        { }
    }
}
