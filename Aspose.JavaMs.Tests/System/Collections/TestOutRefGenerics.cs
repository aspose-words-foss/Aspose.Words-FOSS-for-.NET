// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 07/02/2018 by Anatoly Sidorenko

using System.Collections.Generic;
using NUnit.Framework;

namespace Aspose.JavaMs.Tests.System.Collections
{
    [TestFixture]
    public class TestOutRefGenerics
    {
        [Test]
        public void TestGenericOutRefCtor()
        {
            List<string> list;
            SortedList<string, string> dict;
            MethodRef(out list, out dict);

            Assert.That(3, Is.EqualTo(list.Count));
            Assert.That(3, Is.EqualTo(dict.Count));

            Assert.That("val_1", Is.EqualTo(list[0]));
            Assert.That("val_2", Is.EqualTo(list[1]));
            Assert.That("val_3", Is.EqualTo(list[2]));

            Assert.That("val_1", Is.EqualTo(dict["key_1"]));
            Assert.That("val_2", Is.EqualTo(dict["key_2"]));
            Assert.That("val_3", Is.EqualTo(dict["key_3"]));
        }

        private void MethodRef(out List<string> list, out SortedList<string, string> dict)
        {
            list = new List<string>();
            AddStringListValue(ref list);
            dict = new SortedList<string, string>();
            AddStringSortedListValue(ref dict);
        }

        private void AddStringListValue(ref List<string> list)
        {
            list.Add("val_1");
            list.Add("val_2");
            list.Add("val_3");
        }

        private void AddStringSortedListValue(ref SortedList<string, string> dict)
        {
            dict.Add("key_1", "val_1");
            dict.Add("key_2", "val_2");
            dict.Add("key_3", "val_3");
        }
    }
}
