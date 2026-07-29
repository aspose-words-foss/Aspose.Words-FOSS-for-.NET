// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 05/09/2017 by Tengiz Sharafiev

using Aspose.Collections;
using NUnit.Framework;

namespace Aspose.Tests.Collections.Generic
{
    /// <summary>
    /// Tests CaseInsensitiveHashSet class' functionality.
    /// </summary>
    /// <remarks>
    /// Every individual test of this class corresponds to a ByteList member.
    /// See the corresponding CaseInsensitiveHashSet members' descriptions for any additional information.
    /// </remarks>
    [TestFixture]
    public class TestCaseInsensitiveHashSet
    {
        [Test]
        public void TestCtor()
        {
            CaseInsensitiveHashSet hashSet = new CaseInsensitiveHashSet();
            Assert.That(hashSet.Count, Is.EqualTo(0));
        }

        [Test]
        public void TestAdd()
        {
            CaseInsensitiveHashSet hashSet = new CaseInsensitiveHashSet();
            Assert.That(hashSet.Add("i"), Is.True);
            Assert.That(hashSet.Add("b"), Is.True);
            Assert.That(hashSet.Add("i"), Is.False);
            Assert.That(hashSet.Add("I"), Is.False);
            Assert.That(hashSet.Add("B"), Is.False);
        }

        [Test]
        public void TestClear()
        {
            CaseInsensitiveHashSet hashSet = new CaseInsensitiveHashSet();
            hashSet.Add("a");
            hashSet.Add("b");
            Assert.That(hashSet.Count, Is.EqualTo(2));
            hashSet.Clear();
            Assert.That(hashSet.Count, Is.EqualTo(0));
        }

        [Test]
        public void TestContains()
        {
            CaseInsensitiveHashSet hashSet = new CaseInsensitiveHashSet();
            hashSet.Add("a");
            Assert.That(hashSet.Contains("1"), Is.False);
            Assert.That(hashSet.Contains("a"), Is.True);
            Assert.That(hashSet.Contains("A"), Is.True);
        }

        [Test]
        public void TestRemove()
        {
            CaseInsensitiveHashSet hashSet = new CaseInsensitiveHashSet();
            hashSet.Add("a");
            hashSet.Add("b");
            hashSet.Add("c");
            Assert.That(hashSet.Count, Is.EqualTo(3));
            Assert.That(hashSet.Remove("1"), Is.False);
            Assert.That(hashSet.Remove("a"), Is.True);
            Assert.That(hashSet.Remove("B"), Is.True);
            Assert.That(hashSet.Count, Is.EqualTo(1));
        }
    }
}
