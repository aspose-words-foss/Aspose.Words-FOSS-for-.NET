// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 28/01/2018 by Anatoly Sidorenko

using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;

namespace Aspose.JavaMs.Tests.System.Collections
{
    [TestFixture]
    public class TestGenericCollection
    {
        [Test]
        public void TestEnumerator()
        {
            string[] array = new[] {"abc", "bcd", "cde"};
            TestCollection coll = new TestCollection(array);
            Assert.That(coll, IsNot.Null());
            Assert.That(3, Is.EqualTo(coll.Count));

            IEnumerator<string> enumerator = coll.GetEnumerator();
            for (int index = 0; enumerator.MoveNext(); index++)
            {
                Assert.That(array[index], Is.EqualTo(enumerator.Current));
            }
            Assert.That(false, Is.EqualTo(enumerator.MoveNext()));
        }

        [Test]
        public void TestCollectionEnumerator()
        {
            string[] array = new[] {"abc", "bcd", "cde"};
            TestCollection coll = new TestCollection(array);
            for (int index = 0; coll.MoveNext(); index++)
            {
                Assert.That(array[index], Is.EqualTo(coll.Current));
            }
            Assert.That(false, Is.EqualTo(coll.MoveNext()));
            coll.Reset();
            Assert.That(true, Is.EqualTo(coll.MoveNext()));
        }

        [Test]
        public void TestForeach()
        {
            string[] array = new[] { "abc", "bcd", "cde" };
            TestCollection coll = new TestCollection(array);

            int index = 0;
            foreach (string name in coll)
            {
                Assert.That(array[index], Is.EqualTo(name));
                index++;
            }
        }

        [Test]
        public void TestForeachEnumerable()
        {
            string[] array = new[] { "abc", "bcd", "cde" };
            TestCollection coll = new TestCollection(array);

            IEnumerable<string> enumerable = coll;
            int index = 0;
            foreach (string name in enumerable)
            {
                Assert.That(array[index], Is.EqualTo(name));
                index++;
            }
        }

        private class TestCollection : IEnumerator<string>, IEnumerable<string>
        {
            public TestCollection(string[] collection)
            {
                mArray = collection;
            }

            public int Count
            {
                get { return mArray.Length; }
            }

            #region Implementation of IEnumerator

            public bool MoveNext()
            {
                if (mIndex >= mArray.Length - 1)
                    return false;

                mIndex++;
                return true;
            }

            public void Reset()
            {
                mIndex = -1;
            }

            public string Current
            {
                get { return mArray[mIndex]; }
            }

            object IEnumerator.Current
            {
                get { return Current; }
            }

            void IDisposable.Dispose()
            {
                // Do nothing.
            }

            #endregion

            #region Implementation of IEnumerable

            public IEnumerator<string> GetEnumerator()
            {
                return this;
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }

            #endregion

            private string[] mArray ;
            private int mIndex = -1;
        }
    }
}
