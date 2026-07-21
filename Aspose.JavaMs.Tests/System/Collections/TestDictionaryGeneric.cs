// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 22/02/2018 by Anatoly Sidorenko

using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace Aspose.JavaMs.Tests.System.Collections
{
    [TestFixture]
    public class TestDictionaryGeneric
    {
        /// <summary>
        /// Tests how items are added to a dictionary.
        /// </summary>
        [Test]
        public void TestAdd()
        {
            Dictionary<string, string> dictionary = new Dictionary<string, string>();
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
            Dictionary<string, string> dictionary = new Dictionary<string, string>();
            dictionary.Add(gFirstKey, gFirstValue);
            TestAddDuplicate(dictionary, gFirstKey);
        }

        /// <summary>
        /// Tests the addition of a duplicate key to a dictionary.
        /// </summary>
        private static void TestAddDuplicate(Dictionary<string, string> dictionary, string duplicateKey)
        {
            try
            {
                dictionary.Add(duplicateKey, gFirstValue);
            }
            catch (ArgumentException)
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
            Dictionary<string, string> dictionary = new Dictionary<string, string>();
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
            Dictionary<string, string> dictionary = new Dictionary<string, string>();
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
            Dictionary<string, string> dictionary = new Dictionary<string, string>();
            dictionary.Add(gFirstKey, gFirstValue);
            dictionary.Add(gSecondKey, gSecondValue);

            IEnumerator<KeyValuePair<string, string>> enumerator = dictionary.GetEnumerator();
            TestEnumerator(dictionary, enumerator);
        }

        /// <summary>
        /// Tests a single pass of a dictionary enumerator.
        /// </summary>
        private static void TestEnumerator(Dictionary<string, string> dictionary, IEnumerator<KeyValuePair<string, string>> enumerator)
        {
            string[] keys = new string[dictionary.Count];
            string[] values = new string[dictionary.Count];
            int index = 0;

            while (enumerator.MoveNext())
            {
                KeyValuePair<string, string> current = enumerator.Current;
                keys[index] = current.Key;
                values[index] = current.Value;
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

#if !JAVA
        /// <summary>
        /// Tests that modifications of a dictionary while its enumerating are prohibited.
        /// </summary>
        [Test, ExpectedException(typeof(InvalidOperationException))]
        public void TestEnumeratorChangeWhileMoving()
        {
            Dictionary<string, string> dictionary = new Dictionary<string, string>();
            dictionary.Add(gFirstKey, gFirstValue);

            IEnumerator<KeyValuePair<string, string>> enumerator = dictionary.GetEnumerator();
            bool isMoved = enumerator.MoveNext();
            Assert.That(isMoved, Is.True);

            dictionary.Add(gSecondKey, gSecondValue);
            enumerator.MoveNext();
        }
#endif

        /// <summary>
        /// Tests how dictionary item removal works.
        /// </summary>
        [Test]
        public void TestRemove()
        {
            Dictionary<string, string> dictionary = new Dictionary<string, string>();
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
        /// Tests how dictionary item count calculation works.
        /// </summary>
        [Test]
        public void TestCount()
        {
            Dictionary<string, string> dictionary = new Dictionary<string, string>();
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
            Dictionary<string, string> dictionary = new Dictionary<string, string>();
            dictionary[gFirstKey] = gFirstValue;
            dictionary[gFirstKey] = gSecondValue;
            Assert.That(dictionary.Count, Is.EqualTo(1));
            Assert.That(dictionary[gFirstKey], Is.EqualTo(gSecondValue));

            Assert.That(dictionary.ContainsKey(gFirstKey), Is.EqualTo(true));
            Assert.That(dictionary.GetValueOrNull(gFirstKey), Is.EqualTo(gSecondValue));

            Assert.That(dictionary.ContainsKey(gSecondKey), Is.EqualTo(false));
        }

        /// <summary>
        /// Tests how a dictionary contents are copied from another dictionary.
        /// </summary>
        [Test]
        public void TestCopyCtor()
        {
            Dictionary<string, string> dictionary1 = new Dictionary<string, string>();
            dictionary1[gFirstKey] = gFirstValue;
            dictionary1[gSecondKey] = gSecondValue;

            Dictionary<string, string> dictionary2 = new Dictionary<string, string>(dictionary1);
            Assert.That(dictionary2.Count, Is.EqualTo(2));
            Assert.That(dictionary2[gFirstKey], Is.EqualTo(gFirstValue));
            Assert.That(dictionary2[gSecondKey], Is.EqualTo(gSecondValue));

            // Ensure that the dictionaries are independent.
            dictionary2[gFirstKey] = gSecondValue;
            Assert.That(dictionary2[gFirstKey], Is.EqualTo(gSecondValue));
            Assert.That(dictionary1[gFirstKey], Is.EqualTo(gFirstValue));
        }

        /// <summary>
        /// Tests how a dictionary with case-sensitive string keys works.
        /// </summary>
        [Test]
        public void TestCaseSensitiveDictionary()
        {
            TestCaseSensitiveDictionary(new Dictionary<string, string>());
        }

        private static void TestCaseSensitiveDictionary(Dictionary<string, string> dictionary)
        {
            // Add two values to a dictionary with keys which differ only by character case and
            // ensure that they are considered to be separate dictionary entries.
            dictionary["Iя"] = gFirstValue;
            dictionary["iя"] = gSecondValue;
            Assert.That(dictionary.Count, Is.EqualTo(2));
            Assert.That(dictionary["Iя"], Is.EqualTo(gFirstValue));
            Assert.That(dictionary["iя"], Is.EqualTo(gSecondValue));
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
            Assert.That(dictionary["iя"], Is.EqualTo(gSecondValue));

            // Ensure that the removed dictionary entry is not considered to be a duplicate while repeated adding.
            dictionary.Add("Iя", gFirstValue);
        }

        /// <summary>
        /// Tests how dictionary key enumeration works.
        /// </summary>
        [Test]
        public void TestKeyEnumeration()
        {
            Dictionary<string, string> dictionary = new Dictionary<string, string>();
            dictionary[gFirstKey] = gFirstValue;
            dictionary[gSecondKey] = gSecondValue;

            int index = 0;
            foreach (string key in dictionary.Keys)
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

        /// <summary>
        /// Tests how dictionary value enumeration works.
        /// </summary>
        [Test]
        public void TestValueEnumeration()
        {
            Dictionary<string, string> dictionary = new Dictionary<string, string>();
            dictionary[gFirstKey] = gFirstValue;
            dictionary[gSecondKey] = gSecondValue;

            int index = 0;
            foreach (string value in dictionary.Values)
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

        /// <summary>
        /// Returns the first dictionary key to test.
        /// </summary>
        private static string GetFirstKey()
        {
            return "a";
        }

        /// <summary>
        /// Returns the second dictionary key to test.
        /// </summary>
        private static string GetSecondKey()
        {
            return "b";
        }

        /// <summary>
        /// Returns the first dictionary value to test.
        /// </summary>
        private static string GetFirstValue()
        {
            return "A";
        }

        /// <summary>
        /// Returns the second dictionary value to test.
        /// </summary>
        private static string GetSecondValue()
        {
            return "B";
        }

        // Calculate once, use multiple times.
        private static readonly string gFirstKey = GetFirstKey();
        private static readonly string gSecondKey = GetSecondKey();
        private static readonly string gFirstValue = GetFirstValue();
        private static readonly string gSecondValue = GetSecondValue();
    }
}

