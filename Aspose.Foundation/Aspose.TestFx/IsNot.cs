// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 08/07/2025 by Anatoly Sidorenko

using System;
using Aspose.JavaAttributes;
using NUnit.Framework.Constraints;

namespace NUnit.Framework
{
    /// <summary>
    /// A simple Wrapper for Autoporting test to Java
    /// </summary>
    [JavaDelete]
    public static class IsNot
    {
        public static IResolveConstraint Null() => Is.Not.Null;
        public static IResolveConstraint Empty() => Is.Not.Empty;

        public static EqualConstraint EqualTo( object expected) => Is.Not.EqualTo(expected);
        public static IResolveConstraint SameAs( object expected) => Is.Not.SameAs(expected);
        public static IResolveConstraint InstanceOf( Type expectedType) => Is.Not.InstanceOf(expectedType);
    }
}
