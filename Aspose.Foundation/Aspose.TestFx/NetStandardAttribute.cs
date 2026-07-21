// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 08/11/2020 by Alexander Sevidov.

using System;
using Aspose.JavaAttributes;

namespace NUnit.Framework
{
    // Attribute used to identify a test that shoul be run only for .NET Standard.
    [JavaDelete("Autoported directly to Java's TestNG analog. No need in Java.")]
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public class NetStandardAttribute :
#if NETSTANDARD
        Attribute
#else
        IgnoreAttribute
#endif
    {
        public NetStandardAttribute(string reason)
#if !NETSTANDARD
            : base(reason)
#endif
        { }
    }
}
