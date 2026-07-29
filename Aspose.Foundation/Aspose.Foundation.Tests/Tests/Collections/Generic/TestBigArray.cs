// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 12/03/2021 by Dmitry Sokolov.

using System;
using Aspose.Collections;
using NUnit.Framework;

namespace Aspose.Tests.Collections.Generic
{
    [TestFixture]
    public class TestBigArray
    {
        [TestCase(1)]
        [TestCase(65536)]
        [TestCase(65000)]
        public void TestIndexer(int length)
        {
            IntBigArray bigArray = new IntBigArray(length);

            // Setter.
            for(int i = 0; i < length; ++i)
                bigArray[i] = i + 1;

            // Getter.
            for (int i = 0; i < length; ++i)
                Assert.That(bigArray[i], Is.EqualTo(i + 1));

            Assert.That(bigArray.Length, Is.EqualTo(length));
        }

        [TestCase(0)]
        [TestCase(-1)]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void TestWrongSize(int length)
        {
            new IntBigArray(length);
        }
    }
}
