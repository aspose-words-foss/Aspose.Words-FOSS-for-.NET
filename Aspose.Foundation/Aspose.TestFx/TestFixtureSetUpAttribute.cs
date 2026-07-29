// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 09/06/2017 by Andrey Noskov

using System;
using Aspose.JavaAttributes;

namespace NUnit.Framework
{
    // Attribute used to identify a method that is called once to perform setup before
    // any child tests are run.
    [JavaDelete("Autoported directly to Java's TestNG analog. No need in Java.")]
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public class TestFixtureSetUpAttribute : OneTimeSetUpAttribute
    {
    }
}
