// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 05/03/2009 by Konstantin Sidorenko
// 2016/07/14 by Anatoliy Sidorenko

using System.Collections;
using NUnit.Framework;

namespace Aspose.JavaMs.Tests.System.Collections
{
    /// <summary>
    /// Tests adapted from Rotor.
    /// </summary>
    [TestFixture]
    public class TestDictionaryEntry
    {
        [Test]
        public void TestGetKey()
        {
            entry = new DictionaryEntry("Hello", "World");
            Assert.That("Hello", Is.EqualTo(entry.Key));

            entry = new DictionaryEntry();
            entry = new DictionaryEntry(null, null);
            Assert.That(null, Is.EqualTo(entry.Key));

            entry = new DictionaryEntry(5, 6);
            Assert.That(5, Is.EqualTo(entry.Key));
        }

        [Test]
        public void TestGetValue()
        {
            entry = new DictionaryEntry("Hello", "World");
            Assert.That("World", Is.EqualTo(entry.Value));
            
            entry = new DictionaryEntry(5, 6);
            Assert.That(6, Is.EqualTo(entry.Value));
        }

        [Test]
        public void TestSetValue()
        {
            entry = new DictionaryEntry("What", "ever");
            strValue = "Hello";
            entry.Value = strValue;
            Assert.That(strValue, Is.EqualTo(entry.Value));

            entry = new DictionaryEntry(5, 6);
            strValue = "Hello";
            entry.Value = strValue;
            Assert.That(strValue, Is.EqualTo(entry.Value));

            entry = new DictionaryEntry("string", 5);
            entry.Value = 6;
            Assert.That(6, Is.EqualTo(entry.Value));
        }

        //JAVA-specific Generics:

#if JAVA
        [Test]
        public void TestGetKeyGenerics()
        {
            entry = new DictionaryEntry<String,String>("Hello", "World");
		    Assert.assertEquals(entry.getKey(), "Hello");

		    entry = new DictionaryEntry<Integer,Integer>(5, 6);
		    Assert.assertEquals(entry.getKey(), 5);

		    DictionaryEntry<String,String> entryStr = new DictionaryEntry<String,String>("Hello", "World");
		    Assert.assertEquals(entryStr.getKey(), "Hello");

		    DictionaryEntry<Integer,Integer> entryInt = new DictionaryEntry<Integer,Integer>(5, 6);
		    Assert.assertEquals((int) entryInt.getKey(), 5);
        }

        [Test]
        public void TestGetValueGenerics()
        {
            entry = new DictionaryEntry<String,String>("Hello", "World");
		    Assert.assertEquals(entry.getValue(), "World");

		    entry = new DictionaryEntry<Integer,Integer>(5, 6);
		    Assert.assertEquals(entry.getValue(), 6);

		    DictionaryEntry<String,String> entryStr = new DictionaryEntry<String,String>("Hello", "World");
		    Assert.assertEquals(entryStr.getValue(), "World");

		    DictionaryEntry<Integer,Integer> entryInt = new DictionaryEntry<Integer,Integer>(5, 6);
		    Assert.assertEquals((int) entryInt.getValue(), 6);
        }

        [Test]
        public void TestSetValueGenerics()
        {
            DictionaryEntry<String,String> entryStr = new DictionaryEntry<String,String>();
		    strValue = "Hello";
            entryStr.setValue(strValue);
		    Assert.assertEquals(entryStr.getValue(), strValue);

		    entryStr = new DictionaryEntry<String,String>("What", "ever");
		    strValue = "Hello";
            entryStr.setValue(strValue);
		    Assert.assertEquals(entryStr.getValue(), strValue);

		    DictionaryEntry<String,Integer> entryStrInt = new DictionaryEntry<String,Integer>("STring", 5);
		    entryStrInt.setValue(6);
		    Assert.assertEquals((int) entryStrInt.getValue(), 6);
        }
#endif

        private DictionaryEntry entry;
        private string strValue;
    }
}
