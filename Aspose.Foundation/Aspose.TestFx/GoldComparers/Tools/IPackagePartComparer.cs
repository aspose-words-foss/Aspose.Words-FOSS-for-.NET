// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 01/09/2016 by Ivan Lyagin

using Aspose.JavaAttributes;

namespace Aspose.TestFx.GoldComparers
{
    /// <summary>
    /// Represents a package part comparer used by <see cref="PackagePartComparisonInfo"/> while performing GOLD tests.
    /// </summary>
    public interface IPackagePartComparer
    {
        /// <summary>
        /// Compares an out package part buffer with a GOLD one returning true if those are equal and false otherwise.
        /// </summary>
        /// <param name="partName">A package part name.</param>
        /// <param name="buffer1">An out package part buffer.</param>
        /// <param name="buffer2">A GOLD package part buffer.</param>
        /// <param name="isPreferXmlDiff">
        /// A value indicating whether XML comparison should be preferred over byte comparison for XML package parts.
        /// </param>
        [JavaThrows(true)]
        bool CompareBuffers(string partName, byte[] buffer1, byte[] buffer2, bool isPreferXmlDiff);
    }
}
