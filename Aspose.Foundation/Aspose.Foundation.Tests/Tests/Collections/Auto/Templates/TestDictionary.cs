// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 29/08/2013 by Ivan Lyagin

#if INCLUDE_FILE
using System;
using Aspose.Collections;
using NUnit.Framework;

namespace Aspose.Tests.Collections.Auto
{
    /// <summary>
    /// Tests Dictionary&lt;TKey, TValue&gt; class' functionality.
    /// </summary>
    [TestFixture]
    public class TestDictionary<TKey, TValue>
    {
        /// <summary>
        /// Tests how items are added to a dictionary.
        /// </summary>
        [Test]
        public void TestAdd()
        {
            Dictionary<TKey, TValue> dictionary = new Dictionary<TKey, TValue>();
            dictionary.Add(gFirstKey, gFirstValue);
            dictionary.Add(gSecondKey, gSecondValue);
            Assert.That(dictionary.Count, Is.EqualTo(2));
            Assert.That(dictionary[gFirstKey], Is.EqualTo(gFirstValue));
            Assert.That(dictionary[gSecondKey], Is.EqualTo(gSecondValue));
        }

        /// <summary>
        /// Tests that duplicate keys can not be added to a dictionary.
        /// </summary>
        [Test]
        public void TestAddDuplicate()
        {
            Dictionary<TKey, TValue> dictionary = new Dictionary<TKey, TValue>();
            dictionary.Add(gFirstKey, gFirstValue);
            TestAddDuplicate(dictionary, gFirstKey);
        }

        /// <summary>
        /// Tests the addition of a duplicate key to a dictionary.
        /// </summary>
        private static void TestAddDuplicate(Dictionary<TKey, TValue> dictionary, TKey duplicateKey)
        {
            try
            {
                dictionary.Add(duplicateKey, gFirstValue);
            }
            catch (InvalidOperationException)
            {
                return;
            }

            Assert.Fail("An exception throw is expected.");
        }

        /// <summary>
        /// Tests removal of all items from a dictionary.
        /// </summary>
        [Test]
        public void TestClear()
        {
            Dictionary<TKey, TValue> dictionary = new Dictionary<TKey, TValue>();
            dictionary.Add(gFirstKey, gFirstValue);
            dictionary.Add(gSecondKey, gSecondValue);
            dictionary.Clear();
            Assert.That(dictionary.Count, Is.EqualTo(0));
        }

        /// <summary>
        /// Tests how dictionary key and value existence is checked.
        /// </summary>
        [Test]
        public void TestContains()
        {
            Dictionary<TKey, TValue> dictionary = new Dictionary<TKey, TValue>();
            dictionary.Add(gFirstKey, gFirstValue);
            Assert.That(dictionary.ContainsKey(gFirstKey), Is.True);
            Assert.That(dictionary.ContainsValue(gFirstValue), Is.True);
            Assert.That(dictionary.ContainsKey(gSecondKey), Is.False);
            Assert.That(dictionary.ContainsValue(gSecondValue), Is.False);
        }

        /// <summary>
        /// Tests how dictionary enumeration works.
        /// </summary>
        [Test]
        public void TestEnumerator()
        {
            Dictionary<TKey, TValue> dictionary = new Dictionary<TKey, TValue>();
            dictionary.Add(gFirstKey, gFirstValue);
            dictionary.Add(gSecondKey, gSecondValue);

            Dictionary<TKey, TValue>.Enumerator enumerator = dictionary.GetEnumerator();
            TestEnumerator(dictionary, enumerator);

            // Reset the enumerator state and verify that it will work exactly in the same way.
            enumerator.Reset();
            TestEnumerator(dictionary, enumerator);
        }

        /// <summary>
        /// Tests a single pass of a dictionary enumerator.
        /// </summary>
        private static void TestEnumerator(Dictionary<TKey, TValue> dictionary, Dictionary<TKey, TValue>.Enumerator enumerator)
        {
            TKey[] keys = new TKey[dictionary.Count];
            TValue[] values = new TValue[dictionary.Count];
            int index = 0;

            while (enumerator.MoveNext())
            {
                keys[index] = enumerator.CurrentKey;
                values[index] = enumerator.CurrentValue;
                index++;
            }

            Assert.That(index, Is.EqualTo(2));

            int firstKeyIndex = Array.IndexOf(keys, gFirstKey);
            Assert.That(firstKeyIndex, Is.GreaterThanOrEqualTo(0));

            int secondKeyIndex = Array.IndexOf(keys, gSecondKey);
            Assert.That(secondKeyIndex, Is.GreaterThanOrEqualTo(0));
            Assert.That(secondKeyIndex, IsNot.EqualTo(firstKeyIndex));

            int firstValueIndex = Array.IndexOf(values, gFirstValue);
            Assert.That(firstValueIndex, Is.GreaterThanOrEqualTo(0));
            Assert.That(firstValueIndex, Is.EqualTo(firstKeyIndex));

            int secondValueIndex = Array.IndexOf(values, gSecondValue);
            Assert.That(secondValueIndex, Is.GreaterThanOrEqualTo(0));
            Assert.That(secondValueIndex, IsNot.EqualTo(firstValueIndex));
            Assert.That(secondValueIndex, Is.EqualTo(secondKeyIndex));
        }

        /// <summary>
        /// Tests that accessing of a dictionary key being enumerated is not allowed for a dictionary enumerator 
        /// which was not successfully moved.
        /// </summary>
        [Test, ExpectedException(typeof(InvalidOperationException))]
        public void TestEnumeratorGetKeyWithoutMove()
        {
            Dictionary<TKey, TValue> dictionary = new Dictionary<TKey, TValue>();
            Dictionary<TKey, TValue>.Enumerator enumerator = dictionary.GetEnumerator();
            TKey key = enumerator.CurrentKey;
            Assert.Fail(string.Format(string.Empty, key)); // Force the compiler not to skip the previous instruction.
        }

        /// <summary>
        /// Tests that accessing of a dictionary value being enumerated is not allowed for a dictionary enumerator 
        /// which was not successfully moved.
        /// </summary>
        [Test, ExpectedException(typeof(InvalidOperationException))]
        public void TestEnumeratorGetValueWithoutMove()
        {
            Dictionary<TKey, TValue> dictionary = new Dictionary<TKey, TValue>();
            Dictionary<TKey, TValue>.Enumerator enumerator = dictionary.GetEnumerator();
            TValue value = enumerator.CurrentValue;
            Assert.Fail(string.Format(string.Empty, value)); // Force the compiler not to skip the previous instruction.
        }

        /// <summary>
        /// Tests that modifications of a dictionary while its enumerating are prohibited.
        /// </summary>
        [Test, ExpectedException(typeof(InvalidOperationException))]
        public void TestEnumeratorChangeWhileMoving()
        {
            Dictionary<TKey, TValue> dictionary = new Dictionary<TKey, TValue>();
            dictionary.Add(gFirstKey, gFirstValue);
            
            Dictionary<TKey, TValue>.Enumerator enumerator = dictionary.GetEnumerator();
            bool isMoved = enumerator.MoveNext();
            Assert.That(isMoved, Is.True);

            dictionary.Add(gSecondKey, gSecondValue);
            enumerator.MoveNext();
        }

        /// <summary>
        /// Tests how dictionary item removal works.
        /// </summary>
        [Test]
        public void TestRemove()
        {
            Dictionary<TKey, TValue> dictionary = new Dictionary<TKey, TValue>();
            dictionary.Add(gFirstKey, gFirstValue);
            bool isRemoved = dictionary.Remove(gSecondKey);
            Assert.That(isRemoved, Is.False);
            Assert.That(dictionary.Count, Is.EqualTo(1));

            isRemoved = dictionary.Remove(gFirstKey);
            Assert.That(isRemoved, Is.True);
            Assert.That(dictionary.Count, Is.EqualTo(0));

            isRemoved = dictionary.Remove(gFirstKey);
            Assert.That(isRemoved, Is.False);
            Assert.That(dictionary.Count, Is.EqualTo(0));
        }

        /// <summary>
        /// Tests how a null substitute value for a dictionary is checked.
        /// </summary>
        [Test]
        public void TestNullSubstitute()
        {
            // Ensure that the test is valid and the first test value is not a null substitute value.
            Assert.That(Dictionary<TKey, TValue>.IsNullSubstitute(gFirstValue), Is.False);

            Dictionary<TKey, TValue> dictionary = new Dictionary<TKey, TValue>();
            dictionary.Add(gFirstKey, gFirstValue);
            Assert.That(Dictionary<TKey, TValue>.IsNullSubstitute(dictionary[gFirstKey]), Is.False);
            Assert.That(Dictionary<TKey, TValue>.IsNullSubstitute(dictionary[gSecondKey]), Is.True);
        }

        /// <summary>
        /// Tests how dictionary item count calculation works.
        /// </summary>
        [Test]
        public void TestCount()
        {
            Dictionary<TKey, TValue> dictionary = new Dictionary<TKey, TValue>();
            dictionary.Add(gFirstKey, gFirstValue);
            Assert.That(dictionary.Count, Is.EqualTo(1));

            dictionary.Add(gSecondKey, gSecondValue);
            Assert.That(dictionary.Count, Is.EqualTo(2));

            dictionary.Remove(gSecondKey);
            Assert.That(dictionary.Count, Is.EqualTo(1));

            dictionary.Clear();
            Assert.That(dictionary.Count, Is.EqualTo(0));
        }

        /// <summary>
        /// Tests how dictionary values are set and retrieved according to their keys.
        /// </summary>
        [Test]
        public void TestGetSet()
        {
            Dictionary<TKey, TValue> dictionary = new Dictionary<TKey, TValue>();
            dictionary[gFirstKey] = gFirstValue;
            dictionary[gFirstKey] = gSecondValue;
            Assert.That(dictionary.Count, Is.EqualTo(1));
            Assert.That(dictionary[gFirstKey], Is.EqualTo(gSecondValue));
            Assert.That(Dictionary<TKey, TValue>.IsNullSubstitute(dictionary[gSecondKey]), Is.True);
        }

        /// <summary>
        /// Tests how a dictionary contents are copied from another dictionary.
        /// </summary>
        [Test]
        public void TestCopyCtor()
        {
            Dictionary<TKey, TValue> dictionary1 = new Dictionary<TKey, TValue>();
            dictionary1[gFirstKey] = gFirstValue;
            dictionary1[gSecondKey] = gSecondValue;

            Dictionary<TKey, TValue> dictionary2 = new Dictionary<TKey, TValue>(dictionary1);
            Assert.That(dictionary2.Count, Is.EqualTo(2));
            Assert.That(dictionary2[gFirstKey], Is.EqualTo(gFirstValue));
            Assert.That(dictionary2[gSecondKey], Is.EqualTo(gSecondValue));

            // Ensure that the dictionaries are independent.
            dictionary2[gFirstKey] = gSecondValue;
            Assert.That(dictionary2[gFirstKey], Is.EqualTo(gSecondValue));
            Assert.That(dictionary1[gFirstKey], Is.EqualTo(gFirstValue));
        }

#if STRING_KEY
        /// <summary>
        /// Tests how a dictionary with case-sensitive string keys works.
        /// </summary>
        [Test]
        public void TestCaseSensitiveDictionary()
        {
            TestCaseSensitiveDictionary(new Dictionary<TKey, TValue>());
            TestCaseSensitiveDictionary(new Dictionary<TKey, TValue>(true));
        }

        private static void TestCaseSensitiveDictionary(Dictionary<TKey, TValue> dictionary)
        {
            // Add two values to a dictionary with keys which differ only by character case and
            // ensure that they are considered to be separate dictionary entries.
            dictionary["Iя"] = gFirstValue;
            dictionary["iя"] = gSecondValue;
            Assert.That(dictionary.Count, Is.EqualTo(2));
            Assert.That(dictionary["Iя"], Is.EqualTo(gFirstValue));
            Assert.That(dictionary["iя"], Is.EqualTo(gSecondValue));
            Assert.That(Dictionary<TKey, TValue>.IsNullSubstitute(dictionary["IЯ"]), Is.True);
            Assert.That(Dictionary<TKey, TValue>.IsNullSubstitute(dictionary["iЯ"]), Is.True);
            Assert.That(dictionary.ContainsKey("Iя"), Is.True);
            Assert.That(dictionary.ContainsKey("iя"), Is.True);
            Assert.That(dictionary.ContainsKey("IЯ"), Is.False);
            Assert.That(dictionary.ContainsKey("iЯ"), Is.False);

            // Remove one of the values and ensure that it is removed whereas the other one is kept.
            bool isRemoved = dictionary.Remove("Iя");
            Assert.That(isRemoved, Is.True);
            Assert.That(dictionary.Count, Is.EqualTo(1));
            Assert.That(dictionary.ContainsKey("Iя"), Is.False);
            Assert.That(dictionary.ContainsKey("iя"), Is.True);
            Assert.That(Dictionary<TKey, TValue>.IsNullSubstitute(dictionary["Iя"]), Is.True);
            Assert.That(dictionary["iя"], Is.EqualTo(gSecondValue));

            // Ensure that the removed dictionary entry is not considered to be a duplicate while repeated adding.
            dictionary.Add("Iя", gFirstValue);
        }

        /// <summary>
        /// Tests how a dictionary with case-insensitive string keys works.
        /// </summary>
        [Test]
        public void TestCaseInsensitiveDictionary()
        {
            // Add two values to a dictionary with keys which differ only by character case and
            // ensure that they are considered to be a single dictionary entry.
            Dictionary<TKey, TValue> dictionary = new Dictionary<TKey, TValue>(false);
            dictionary["Iя"] = gFirstValue;
            dictionary["iя"] = gSecondValue;
            Assert.That(dictionary.Count, Is.EqualTo(1));
            Assert.That(dictionary["Iя"], Is.EqualTo(gSecondValue));
            Assert.That(dictionary["iя"], Is.EqualTo(gSecondValue));
            Assert.That(dictionary["IЯ"], Is.EqualTo(gSecondValue));
            Assert.That(dictionary["iЯ"], Is.EqualTo(gSecondValue));
            Assert.That(dictionary.ContainsKey("Iя"), Is.True);
            Assert.That(dictionary.ContainsKey("iя"), Is.True);
            Assert.That(dictionary.ContainsKey("IЯ"), Is.True);
            Assert.That(dictionary.ContainsKey("iЯ"), Is.True);

            // Remove the entry and ensure that it is not accessible anymore.
            bool isRemoved = dictionary.Remove("Iя");
            Assert.That(isRemoved, Is.True);
            Assert.That(dictionary.Count, Is.EqualTo(0));
            Assert.That(dictionary.ContainsKey("Iя"), Is.False);
            Assert.That(dictionary.ContainsKey("iя"), Is.False);
            Assert.That(Dictionary<TKey, TValue>.IsNullSubstitute(dictionary["Iя"]), Is.True);
            Assert.That(Dictionary<TKey, TValue>.IsNullSubstitute(dictionary["iя"]), Is.True);

            // Ensure that keys which differ only by character case are considered to be duplicates while adding.
            dictionary.Add("Iя", gFirstValue);
            TestAddDuplicate(dictionary, "iя");
        }
#endif

#if NULLABLE_KEY
        /// <summary>
        /// Tests how dictionary key enumeration works.
        /// </summary>
        [Test]
        public void TestKeyEnumeration()
        {
            Dictionary<TKey, TValue> dictionary = new Dictionary<TKey, TValue>();
            dictionary[gFirstKey] = gFirstValue;
            dictionary[gSecondKey] = gSecondValue;

            int index = 0;
            foreach (TKey key in dictionary.Keys)
            {
                switch (index++)
                {
                    case 0:
                        Assert.That(key, Is.EqualTo(gFirstKey));
                        break;
                    case 1:
                        Assert.That(key, Is.EqualTo(gSecondKey));
                        break;
                    default:
                        // Do nothing.
                        break;
                }
            }

            Assert.That(index, Is.EqualTo(2));
        }
#endif

#if NULLABLE_VALUE
        /// <summary>
        /// Tests how dictionary value enumeration works.
        /// </summary>
        [Test]
        public void TestValueEnumeration()
        {
            Dictionary<TKey, TValue> dictionary = new Dictionary<TKey, TValue>();
            dictionary[gFirstKey] = gFirstValue;
            dictionary[gSecondKey] = gSecondValue;

            int index = 0;
            foreach (TValue value in dictionary.Values)
            {
                switch (index++)
                {
                    case 0:
                        Assert.That(value, Is.EqualTo(gFirstValue));
                        break;
                    case 1:
                        Assert.That(value, Is.EqualTo(gSecondValue));
                        break;
                    default:
                        // Do nothing.
                        break;
                }
            }

            Assert.That(index, Is.EqualTo(2));
        }
#endif

        /// <summary>
        /// Returns the first dictionary key to test.
        /// </summary>
        private static TKey GetFirstKey()
        {
            throw new NotSupportedException("GetFirstKeyPlaceholder");
        }

        /// <summary>
        /// Returns the second dictionary key to test.
        /// </summary>
        private static TKey GetSecondKey()
        {
            throw new NotSupportedException("GetSecondKeyPlaceholder");
        }

        /// <summary>
        /// Returns the first dictionary value to test.
        /// </summary>
        private static TValue GetFirstValue()
        {
            throw new NotSupportedException("GetFirstValuePlaceholder");
        }

        /// <summary>
        /// Returns the second dictionary value to test.
        /// </summary>
        private static TValue GetSecondValue()
        {
            throw new NotSupportedException("GetSecondValuePlaceholder");
        }

        // Calculate once, use multiple times.
        private static readonly TKey gFirstKey = GetFirstKey();
        private static readonly TKey gSecondKey = GetSecondKey();
        private static readonly TValue gFirstValue = GetFirstValue();
        private static readonly TValue gSecondValue = GetSecondValue();
    }
}
#endif
