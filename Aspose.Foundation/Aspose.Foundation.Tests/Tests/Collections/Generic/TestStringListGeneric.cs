// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 28/05/2024 by Ilya Navrotskiy

using System.Collections.Generic;
using Aspose.Collections.Generic;
using Aspose.JavaAttributes;
using NUnit.Framework;

namespace Aspose.Foundation.Tests.Tests.Collections.Generic
{
    [TestFixture]
    [JavaManual("replase new int[] with new Integer[]")]
    public class TestStringListGeneric
    {
        /// <summary>
        /// Tests creation of the list.
        /// </summary>
        [Test]
        public void TestCreate()
        {
            StringListGeneric<int> listA = new StringListGeneric<int>(new string[] { "a", "c", "b" }, new int[] { 1, 3, 2 });
            Assert.That(listA.Count, Is.EqualTo(3));
            Assert.That(listA["a"], Is.EqualTo(1));
            Assert.That(listA["c"], Is.EqualTo(3));
            Assert.That(listA["b"], Is.EqualTo(2));

            StringListGeneric<int> listB = new StringListGeneric<int>(listA);
            Assert.That(listB.Count, Is.EqualTo(3));
            Assert.That(listB["a"], Is.EqualTo(1));
            Assert.That(listB["c"], Is.EqualTo(3));
            Assert.That(listB["b"], Is.EqualTo(2));
        }

        /// <summary>
        /// Tests get value at a specified index.
        /// </summary>
        [Test]
        public void TestGetValueAt()
        {
            StringListGeneric<int> list = new StringListGeneric<int>(new string[] { "a", "c", "b" }, new int[] { 1, 3, 2 });
            Assert.That(list.GetByIndex(0), Is.EqualTo(1));
            Assert.That(list.GetByIndex(1), Is.EqualTo(3));
            Assert.That(list.GetByIndex(2), Is.EqualTo(2));
        }

        /// <summary>
        /// Tests remove element at a specified index.
        /// </summary>
        [Test]
        public void TestRemoveAt()
        {
            StringListGeneric<int> list = new StringListGeneric<int>(new string[] { "a", "c", "b" }, new int[] { 1, 3, 2 });
            Assert.That(list.Count, Is.EqualTo(3));

            list.RemoveAt(1);
            Assert.That(list.Count, Is.EqualTo(2));
            Assert.That(list.GetByIndex(0), Is.EqualTo(1));
            Assert.That(list.GetByIndex(1), Is.EqualTo(2));

            list.RemoveAt(0);
            Assert.That(list.Count, Is.EqualTo(1));
            Assert.That(list.GetByIndex(0), Is.EqualTo(2));

            list.RemoveAt(0);
            Assert.That(list.Count, Is.EqualTo(0));
        }

        /// <summary>
        /// Tests get index of a key in the list.
        /// </summary>
        [Test]
        public void TestIndexOfKey()
        {
            StringListGeneric<int> list = new StringListGeneric<int>(new string[] { "a", "a", "b" }, new int[] { 1, 3, 2 });
            Assert.That(list.IndexOfKey("a"), Is.EqualTo(0));
            Assert.That(list.IndexOfKey("b"), Is.EqualTo(2));
        }

        /// <summary>
        /// Tests whether a specified key is contained in the List.
        /// </summary>
        [Test]
        public void TestContainsKey()
        {
            StringListGeneric<int> list = new StringListGeneric<int>(new string[] { "a", "c", "b" }, new int[] { 1, 3, 2 });
            Assert.That(list.ContainsKey("a"), Is.True);
            Assert.That(list.ContainsKey("b"), Is.True);
            Assert.That(list.ContainsKey("c"), Is.True);
            Assert.That(list.ContainsKey("d"), Is.False);
        }

        /// <summary>
        /// Tests adding element to the List.
        /// </summary>
        [Test]
        public void TestAdd()
        {
            StringListGeneric<int> list = new StringListGeneric<int>();
            Assert.That(list.Count, Is.EqualTo(0));

            list.Add("a", 1);
            Assert.That(list.Count, Is.EqualTo(1));
            Assert.That(list["a"], Is.EqualTo(1));

            list.Add("b", 2);
            Assert.That(list.Count, Is.EqualTo(2));
            Assert.That(list["a"], Is.EqualTo(1));
            Assert.That(list["b"], Is.EqualTo(2));

            list.Add("a", 3);
            Assert.That(list.Count, Is.EqualTo(3));
            Assert.That(list["a"], Is.EqualTo(3));
            Assert.That(list["b"], Is.EqualTo(2));
            Assert.That(list.GetKey(0), Is.EqualTo("a"));
            Assert.That(list.GetByIndex(0), Is.EqualTo(1));
        }

        /// <summary>
        /// Tests remove element with a specified key from the List.
        /// </summary>
        [Test]
        public void TestRemove()
        {
            StringListGeneric<int> list = new StringListGeneric<int>(new string[] { "a", "b" }, new int[] { 1, 2 });
            list.Remove("b");
            Assert.That(list.Count, Is.EqualTo(1));
            Assert.That(list["a"], Is.EqualTo(1));

            // Add duplicated key.
            list.Add("a", 5);
            Assert.That(list.Count, Is.EqualTo(2));
            Assert.That(list["a"], Is.EqualTo(5));
            Assert.That(list.GetKey(0), Is.EqualTo("a"));
            Assert.That(list.GetByIndex(0), Is.EqualTo(1));

            list.Remove("a");
            Assert.That(list.Count, Is.EqualTo(1));
            Assert.That(list["a"], Is.EqualTo(1));

            list.Remove("a");
            Assert.That(list.Count, Is.EqualTo(0));
        }

        /// <summary>
        /// Tests get value safely.
        /// </summary>
        [Test]
        public void TestGetValueSafely()
        {
            StringListGeneric<int> list = new StringListGeneric<int>(new string[] { "a", "c", "b" }, new int[] { 1, 3, 2 });

            // Get existing key.
            Assert.That(list.GetValueSafe("c", 5), Is.EqualTo(3));

            // Get non-existing key.
            Assert.That(list.GetValueSafe("d", -1), Is.EqualTo(-1));

            // Add duplicated key.
            list.Add("a", 5);

            Assert.That(list.GetValueSafe("a", -1), Is.EqualTo(5));

            list.Remove("a");
            Assert.That(list.GetValueSafe("a", -1), Is.EqualTo(1));

            list.Remove("a");
            Assert.That(list.GetValueSafe("a", -1), Is.EqualTo(-1));
        }

        /// <summary>
        /// Tests get key at a specified index.
        /// </summary>
        [Test]
        public void TestGetKeyAt()
        {
            StringListGeneric<int> list = new StringListGeneric<int>(new string[] { "a", "c", "b" }, new int[] { 1, 3, 2 });
            Assert.That(list.Count, Is.EqualTo(3));
            Assert.That(list.GetKey(0), Is.EqualTo("a"));
            Assert.That(list.GetKey(1), Is.EqualTo("c"));
            Assert.That(list.GetKey(2), Is.EqualTo("b"));
        }

        /// <summary>
        /// Tests sort by key.
        /// </summary>
        [Test]
        public void TestSort()
        {
            StringListGeneric<int> list = new StringListGeneric<int>(new string[] { "d", "b", "a", "c", "b" }, new int[] { 5, 2, 1, 4, 3 });

            list.Sort();

            Assert.That(list.GetKey(0), Is.EqualTo("a"));
            Assert.That(list.GetKey(1), Is.EqualTo("b"));
            Assert.That(list.GetKey(2), Is.EqualTo("b"));
            Assert.That(list.GetKey(3), Is.EqualTo("c"));
            Assert.That(list.GetKey(4), Is.EqualTo("d"));

            Assert.That(list["a"], Is.EqualTo(1));
            Assert.That(list["b"], Is.EqualTo(3));
            Assert.That(list["c"], Is.EqualTo(4));
            Assert.That(list["d"], Is.EqualTo(5));
        }

        /// <summary>
        /// Tests Enumerator.
        /// </summary>
        [Test]
        public void TestEnumerator()
        {
            string[] keys = new string[] { "d", "b", "a", "c" };
            int[] values = new int[] { 4, 2, 1, 3 };
            StringListGeneric<int> list = new StringListGeneric<int>(keys, values);

            int i = 0;
            foreach (KeyValuePair<string, int> keyValuePair in list)
            {
                Assert.That(keyValuePair.Key, Is.EqualTo(keys[i]));
                Assert.That(keyValuePair.Value, Is.EqualTo(values[i]));
                i++;
            }
        }

        /// <summary>
        /// Tests indexer.
        /// </summary>
        [Test]
        public void TestIndexer()
        {
            StringListGeneric<int> list = new StringListGeneric<int>();

            list["a"] = 1;
            Assert.That(list["a"], Is.EqualTo(1));

            list["b"] = 2;
            Assert.That(list["b"], Is.EqualTo(2));

            list["a"] = 2;
            Assert.That(list["a"], Is.EqualTo(2));

            list["b"] = 1;
            Assert.That(list["b"], Is.EqualTo(1));
        }

        /// <summary>
        /// Tests <see cref="StringListGeneric{TValue}.Count"/> property.
        /// </summary>
        [Test]
        public void TestCount()
        {
            StringListGeneric<int> list = new StringListGeneric<int>();

            Assert.That(list.Count, Is.EqualTo(0));

            list["a"] = 1;
            Assert.That(list.Count, Is.EqualTo(1));

            list["a"] = 2;
            Assert.That(list.Count, Is.EqualTo(1));

            list["b"] = 3;
            Assert.That(list.Count, Is.EqualTo(2));

            list.Add("b", 4);
            Assert.That(list.Count, Is.EqualTo(3));

            list.Remove("a");
            Assert.That(list.Count, Is.EqualTo(2));
        }
    }
}
