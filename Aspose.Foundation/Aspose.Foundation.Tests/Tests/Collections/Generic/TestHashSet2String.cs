// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 05/09/2017 by Tengiz Sharafiev

using System.Collections.Generic;
using NUnit.Framework;

namespace Aspose.Tests.Collections.Generic
{
    /// <summary>
    /// Tests HashSetGeneric class' functionality.
    /// </summary>
    /// <remarks>
    /// Every individual test of this class corresponds to a ByteList member.
    /// See the corresponding HashSetGeneric members' descriptions for any additional information.
    /// </remarks>
    [TestFixture]
    public class TestHashSet2String
    {
        [Test]
        public void TestDefaultCtor()
        {
            Aspose.Collections.Generic.HashSetGeneric<string> hashSet = new Aspose.Collections.Generic.HashSetGeneric<string>();
            Assert.That(hashSet.Count, Is.EqualTo(0));
        }

        [Test]
        public void TestIEnumerableCtor()
        {
            List<string> list = new List<string>();
            list.Add("1");
            list.Add("2");
            list.Add("3");
            Aspose.Collections.Generic.HashSetGeneric<string> hashSet = new Aspose.Collections.Generic.HashSetGeneric<string>(list);
            Assert.That(hashSet.Count, Is.EqualTo(3));
        }

        [Test]
        public void TestAdd()
        {
            Aspose.Collections.Generic.HashSetGeneric<string> hashSet = new Aspose.Collections.Generic.HashSetGeneric<string>();
            Assert.That(hashSet.Add("1"), Is.True);
            Assert.That(hashSet.Add("1"), Is.False);
            Assert.That(hashSet.Add("2"), Is.True);
            Assert.That(hashSet.Count, Is.EqualTo(2));
        }

        [Test]
        public void TestClear()
        {
            Aspose.Collections.Generic.HashSetGeneric<string> hashSet = new Aspose.Collections.Generic.HashSetGeneric<string>();
            hashSet.Add("1");
            Assert.That(hashSet.Count, Is.EqualTo(1));
            hashSet.Clear();
            Assert.That(hashSet.Count, Is.EqualTo(0));
        }

        [Test]
        public void TestContains()
        {
            List<string> list = new List<string>();
            list.Add("1");
            list.Add("2");
            list.Add("3");
            Aspose.Collections.Generic.HashSetGeneric<string> hashSet = new Aspose.Collections.Generic.HashSetGeneric<string>(list);
            Assert.That(hashSet.Contains("1"), Is.True);
            Assert.That(hashSet.Contains("4"), Is.False);
        }

        [Test]
        public void TestRemove()
        {
            List<string> list = new List<string>();
            list.Add("1");
            list.Add("2");
            list.Add("3");
            Aspose.Collections.Generic.HashSetGeneric<string> hashSet = new Aspose.Collections.Generic.HashSetGeneric<string>(list);
            Assert.That(hashSet.Count, Is.EqualTo(3));
            Assert.That(hashSet.Remove("4"), Is.False);
            Assert.That(hashSet.Remove("1"), Is.True);
            Assert.That(hashSet.Count, Is.EqualTo(2));
        }
    }
}
