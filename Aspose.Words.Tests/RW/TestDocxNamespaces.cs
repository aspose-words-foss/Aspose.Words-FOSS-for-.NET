// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 12/05/2016 by Alexander Zhiltsov

using Aspose.Words.Nrx;
using NUnit.Framework;

namespace Aspose.Words.Tests.RW
{
    /// <summary>
    /// Tests methods of the <see cref="DocxNamespaces"/> class.
    /// </summary>
    [TestFixture]
    public class TestDocxNamespaces
    {
        [TestFixtureSetUp]
        public void SetUp()
        {
            TestUtil.SetUpTests();
        }

        /// <summary>
        /// Tests namespace conversions performed by the methods <see cref="DocxNamespaces.ToIsoStrict"/> and
        /// <see cref="DocxNamespaces.ToIsoTransitional"/>.
        /// </summary>
        [TestCase((int)DocxNamespace.Main)] // Casting for C++.
        [TestCase((int)DocxNamespace.Relationships)]
        public void TestDocxNamespaceConversion(int value)
        {
            string strictNamespace = DocxNamespaces.GetNamespace((DocxNamespace)value, true);
            string transitionalNamespace = DocxNamespaces.GetNamespace((DocxNamespace)value, false);
            Assert.That(DocxNamespaces.ToIsoStrict(transitionalNamespace), Is.EqualTo(strictNamespace));
            Assert.That(DocxNamespaces.ToIsoTransitional(strictNamespace), Is.EqualTo(transitionalNamespace));
        }
    }
}
