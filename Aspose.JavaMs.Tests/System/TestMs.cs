// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 19/01/2010 by Konstantin Sidorenko
// 12/01/2016 by Anatoliy Sidorenko

using System;
using System.Collections;

using NUnit.Framework;

namespace Aspose.JavaMs.Tests.System
{
    [TestFixture]
    public class TestMs
    {
        [Test]
        public void TestAs()
        {
            object obj = "aaa";
            string s;

            s = obj as string;
            Assert.That(obj, Is.EqualTo(s));

            s = "str" as string;
            Assert.That("str", Is.EqualTo(s));

            obj = new ArrayList();
            s = obj as string;
            Assert.That(s, Is.Null);

            // Java's ms.as() can do 'int as String' and similar without syntax error (returns null usually).
            // .Net can't.
        }

        [Test]
        public void TestAsWithArrays()
        {
            object obj = new int[10];
            int[] iArray;

            iArray = obj as int[];
            Assert.That(obj, Is.EqualTo(iArray));

            int[] array = {1, 2, 3};
            iArray = array as int[];
            Assert.That(array, Is.EqualTo(iArray));

            obj = array;
            iArray = obj as int[];
            Assert.That(array, Is.EqualTo(iArray));

            obj = new Object[10];
            iArray = obj as int[];
            Assert.That(iArray, Is.Null);
        }

        [Test]
        public void TestSizeOfValueTypes()
        {
            // 1 byte
            Assert.That(sizeof(byte), Is.EqualTo(1));
            Assert.That(sizeof(sbyte), Is.EqualTo(1));

            // 2 bytes
            Assert.That(sizeof(char), Is.EqualTo(2));
            Assert.That(sizeof(short), Is.EqualTo(2));
            Assert.That(sizeof(ushort), Is.EqualTo(2));

            // 4 bytes
            Assert.That(sizeof(int), Is.EqualTo(4));
            Assert.That(sizeof(uint), Is.EqualTo(4));
            Assert.That(sizeof(float), Is.EqualTo(4));

            // 8 bytes
            Assert.That(sizeof(long), Is.EqualTo(8));
            Assert.That(sizeof(ulong), Is.EqualTo(8));
            Assert.That(sizeof(double), Is.EqualTo(8));
        }
    }
}
