// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 23/01/2007 by Konstantin Sidorenko
// 2016/01/15 by Anatoliy Sidorenko

using System;
using NUnit.Framework;

namespace Aspose.JavaMs.Tests.System
{
    [TestFixture]
    public class TestString
    {
        [Test]
        public void TestCompareOrdinal()
        {
            Assert.That(string.CompareOrdinal(".2.", 1, "2", 0, 1), Is.EqualTo(0));
            Assert.That(string.CompareOrdinal(".2.", ".2"), Is.EqualTo(1));
            Assert.That(string.Compare(".2.", 1, "2", 0, 1, StringComparison.Ordinal), Is.EqualTo(0));
            Assert.That(string.Compare(".2.", ".2", StringComparison.Ordinal), Is.EqualTo(1));
            //ignore case
            Assert.That(string.Compare(".a.", 1, "A", 0, 1, StringComparison.OrdinalIgnoreCase), Is.EqualTo(0));
            Assert.That(string.Compare(".a.", ".A.", StringComparison.OrdinalIgnoreCase), Is.EqualTo(0));
        }

        [Test]
        public void TestCompare()
        {
            Assert.That(string.Compare(".2.", 1, "2", 0, 1), Is.EqualTo(0));
            Assert.That(string.Compare(".2.", ".2"), Is.EqualTo(1));
            Assert.That(string.Compare(".2.", 1, "2", 0, 1, StringComparison.CurrentCulture), Is.EqualTo(0));
            Assert.That(string.Compare(".2.", ".2", StringComparison.CurrentCulture), Is.EqualTo(1));
            Assert.That(string.Compare(".2.", 1, "2", 0, 1, StringComparison.InvariantCulture), Is.EqualTo(0));
            Assert.That(string.Compare(".2.", ".2", StringComparison.InvariantCulture), Is.EqualTo(1));
            //ignore case
            Assert.That(string.Compare(".a.", 1, "A", 0, 1, true), Is.EqualTo(0));
            Assert.That(string.Compare(".a.", ".A.", true), Is.EqualTo(0));
            Assert.That(string.Compare(".a.", 1, "A", 0, 1, StringComparison.CurrentCultureIgnoreCase), Is.EqualTo(0));
            Assert.That(string.Compare(".a.", ".A.", StringComparison.CurrentCultureIgnoreCase), Is.EqualTo(0));
            Assert.That(string.Compare(".a.", 1, "A", 0, 1, StringComparison.InvariantCultureIgnoreCase), Is.EqualTo(0));
            Assert.That(string.Compare(".a.", ".A.", StringComparison.InvariantCultureIgnoreCase), Is.EqualTo(0));
        }

        [Test]
        public void TestCompare2()
        {
            Assert.That(string.Compare("a", "a", StringComparison.Ordinal) == 0, Is.True);
            Assert.That(string.Compare("a", "A", StringComparison.Ordinal) > 0, Is.True);
            Assert.That(string.Compare("a", "b", StringComparison.Ordinal) < 0, Is.True);
            Assert.That(string.Compare("a", "ä", StringComparison.Ordinal) < 0, Is.True);

            Assert.That(string.Compare("a", "a", StringComparison.OrdinalIgnoreCase) == 0, Is.True);
            Assert.That(string.Compare("a", "A", StringComparison.OrdinalIgnoreCase) == 0, Is.True);
            Assert.That(string.Compare("a", "b", StringComparison.OrdinalIgnoreCase) < 0, Is.True);
            Assert.That(string.Compare("a", "ä", StringComparison.OrdinalIgnoreCase) < 0, Is.True);

            Assert.That(string.Compare("a", "a", StringComparison.CurrentCulture) == 0, Is.True);
            Assert.That(string.Compare("a", "A", StringComparison.CurrentCulture) < 0, Is.True);
            Assert.That(string.Compare("a", "b", StringComparison.CurrentCulture) < 0, Is.True);
            Assert.That(string.Compare("a", "ä", StringComparison.CurrentCulture) < 0, Is.True);

            Assert.That(string.Compare("a", "a", StringComparison.CurrentCultureIgnoreCase) == 0, Is.True);
            Assert.That(string.Compare("a", "A", StringComparison.CurrentCultureIgnoreCase) == 0, Is.True);
            Assert.That(string.Compare("a", "b", StringComparison.CurrentCultureIgnoreCase) < 0, Is.True);
            Assert.That(string.Compare("a", "ä", StringComparison.CurrentCultureIgnoreCase) < 0, Is.True);
        }

        [Test]
        public void TestStringCtors()
        {
            Assert.That("", Is.EqualTo(String.Empty));

            Assert.That("XXX", Is.EqualTo(new string('X', 3)));
            Assert.That("", Is.EqualTo(new string('Y', 0)));
        }

        [Test]
        public void TestStringRemoveInsert()
        {
            Assert.That("0126789", Is.EqualTo("0123456789".Remove(3, 3)));

            Assert.That("012XXX3456789", Is.EqualTo("0123456789".Insert(3, "XXX")));
        }

        [Test, Ignore("For a while.")]
        public void TestStringIndexOf()
        {
            string s1 = "ani\u00ADmal";
            string s2 = "animal";
            string empty = "";

            // Empty string
            Assert.That(0, Is.EqualTo(s1.IndexOf(empty, StringComparison.CurrentCulture)));
            Assert.That(0, Is.EqualTo(s1.IndexOf(empty, StringComparison.InvariantCulture)));
            Assert.That(0, Is.EqualTo(s1.IndexOf(empty, StringComparison.Ordinal)));
            Assert.That(0, Is.EqualTo(empty.IndexOf(empty, StringComparison.Ordinal)));

            // Use culture-sensitive comparison to find the soft hyphen.
            Assert.That(3, Is.EqualTo(s1.IndexOf("\u00AD", StringComparison.CurrentCulture)));
            Assert.That(-1, Is.EqualTo(s2.IndexOf("\u00AD", StringComparison.CurrentCulture)));

            // Use culture-sensitive comparison to find the soft hyphen followed by "n".
            Assert.That(-1, Is.EqualTo(s1.IndexOf("\u00ADn", StringComparison.CurrentCulture)));
            Assert.That(-1, Is.EqualTo(s2.IndexOf("\u00ADn", StringComparison.CurrentCulture)));

            // Use culture-sensitive comparison to find the soft hyphen followed by "m".
            Assert.That(3, Is.EqualTo(s1.IndexOf("\u00ADm", StringComparison.CurrentCulture)));
            Assert.That(-1, Is.EqualTo(s2.IndexOf("\u00ADm", StringComparison.CurrentCulture)));

            // Use InvariantCulture-sensitive comparison to find the soft hyphen.
            Assert.That(3, Is.EqualTo(s1.IndexOf("\u00AD", StringComparison.InvariantCulture)));
            Assert.That(-1, Is.EqualTo(s2.IndexOf("\u00AD", StringComparison.InvariantCulture)));

            // Use InvariantCulture-sensitive comparison to find the soft hyphen followed by "n".
            Assert.That(-1, Is.EqualTo(s1.IndexOf("\u00ADn", StringComparison.InvariantCulture)));
            Assert.That(-1, Is.EqualTo(s2.IndexOf("\u00ADn", StringComparison.InvariantCulture)));

            // Use InvariantCulture-sensitive comparison to find the soft hyphen followed by "m".
            Assert.That(3, Is.EqualTo(s1.IndexOf("\u00ADm", StringComparison.InvariantCulture)));
            Assert.That(-1, Is.EqualTo(s2.IndexOf("\u00ADm", StringComparison.InvariantCulture)));
            
            // Use ordinal comparison to find the soft hyphen.
            Assert.That(3, Is.EqualTo(s1.IndexOf("\u00AD", StringComparison.Ordinal)));
            Assert.That(-1, Is.EqualTo(s2.IndexOf("\u00AD", StringComparison.Ordinal)));

            // Use ordinal comparison to find the soft hyphen followed by "n".
            Assert.That(-1, Is.EqualTo(s1.IndexOf("\u00ADn", StringComparison.Ordinal)));
            Assert.That(-1, Is.EqualTo(s2.IndexOf("\u00ADn", StringComparison.Ordinal)));

            // Use ordinal comparison to find the soft hyphen followed by "m".
            Assert.That(3, Is.EqualTo(s1.IndexOf("\u00ADm", StringComparison.Ordinal)));
            Assert.That(-1, Is.EqualTo(s2.IndexOf("\u00ADm", StringComparison.Ordinal)));
        }

        [Test, Ignore("For a while.")]
        public void TestStringIndexOfIgnoreCase()
        {
            string s1 = "ani\u00ADmal";
            string s2 = "animal";
            string empty = "";

            // Empty string
            Assert.That(0, Is.EqualTo(s1.IndexOf(empty, StringComparison.CurrentCultureIgnoreCase)));
            Assert.That(0, Is.EqualTo(s1.IndexOf(empty, StringComparison.InvariantCultureIgnoreCase)));
            Assert.That(0, Is.EqualTo(s1.IndexOf(empty, StringComparison.OrdinalIgnoreCase)));
            Assert.That(0, Is.EqualTo(empty.IndexOf(empty, StringComparison.OrdinalIgnoreCase)));

            // Use culture-sensitive comparison to find the soft hyphen.
            Assert.That(3, Is.EqualTo(s1.IndexOf("\u00AD", StringComparison.CurrentCultureIgnoreCase)));
            Assert.That(-1, Is.EqualTo(s2.IndexOf("\u00AD", StringComparison.CurrentCultureIgnoreCase)));

            // Use culture-sensitive comparison to find the soft hyphen followed by "n".
            Assert.That(-1, Is.EqualTo(s1.IndexOf("\u00ADN", StringComparison.CurrentCultureIgnoreCase)));
            Assert.That(-1, Is.EqualTo(s2.IndexOf("\u00ADN", StringComparison.CurrentCultureIgnoreCase)));

            // Use culture-sensitive comparison to find the soft hyphen followed by "m".
            Assert.That(3, Is.EqualTo(s1.IndexOf("\u00ADM", StringComparison.CurrentCultureIgnoreCase)));
            Assert.That(-1, Is.EqualTo(s2.IndexOf("\u00ADM", StringComparison.CurrentCultureIgnoreCase)));

            // Use InvariantCulture-sensitive comparison to find the soft hyphen.
            Assert.That(3, Is.EqualTo(s1.IndexOf("\u00AD", StringComparison.InvariantCultureIgnoreCase)));
            Assert.That(-1, Is.EqualTo(s2.IndexOf("\u00AD", StringComparison.InvariantCultureIgnoreCase)));

            // Use InvariantCulture-sensitive comparison to find the soft hyphen followed by "n".
            Assert.That(-1, Is.EqualTo(s1.IndexOf("\u00ADN", StringComparison.InvariantCultureIgnoreCase)));
            Assert.That(-1, Is.EqualTo(s2.IndexOf("\u00ADN", StringComparison.InvariantCultureIgnoreCase)));

            // Use InvariantCulture-sensitive comparison to find the soft hyphen followed by "m".
            Assert.That(3, Is.EqualTo(s1.IndexOf("\u00ADM", StringComparison.InvariantCultureIgnoreCase)));
            Assert.That(-1, Is.EqualTo(s2.IndexOf("\u00ADM", StringComparison.InvariantCultureIgnoreCase)));

            // Use ordinal comparison to find the soft hyphen.
            Assert.That(3, Is.EqualTo(s1.IndexOf("\u00AD", StringComparison.OrdinalIgnoreCase)));
            Assert.That(-1, Is.EqualTo(s2.IndexOf("\u00AD", StringComparison.OrdinalIgnoreCase)));

            // Use ordinal comparison to find the soft hyphen followed by "n".
            Assert.That(-1, Is.EqualTo(s1.IndexOf("\u00ADN", StringComparison.OrdinalIgnoreCase)));
            Assert.That(-1, Is.EqualTo(s2.IndexOf("\u00ADN", StringComparison.OrdinalIgnoreCase)));

            // Use ordinal comparison to find the soft hyphen followed by "m".
            Assert.That(3, Is.EqualTo(s1.IndexOf("\u00ADM", StringComparison.OrdinalIgnoreCase)));
            Assert.That(-1, Is.EqualTo(s2.IndexOf("\u00ADM", StringComparison.OrdinalIgnoreCase)));
        }

        [Test]
        public void TestStringIndexOfAny()
        {
            Assert.That(3, Is.EqualTo("0123456789".IndexOfAny(new char[] { '3', '4', '5' })));
            Assert.That(3, Is.EqualTo("0123456789".IndexOfAny(new char[] { '4', '5', '3' })));
            Assert.That(0, Is.EqualTo("0123456789".IndexOfAny(new char[] { '5', '9', '0' })));
            Assert.That(9, Is.EqualTo("0123456789".IndexOfAny(new char[] { 'x', '9', 'v' })));
            Assert.That(-1, Is.EqualTo("0123456789".IndexOfAny(new char[] { 'x', 'y', 'z' })));

            Assert.That(3, Is.EqualTo("0123456789".IndexOfAny(new char[] { '3', '4', '5' }, 3)));
            Assert.That(4, Is.EqualTo("0123456789".IndexOfAny(new char[] { '3', '4', '5' }, 4)));
            Assert.That(5, Is.EqualTo("0123456789".IndexOfAny(new char[] { '3', '4', '5' }, 5)));
            Assert.That(9, Is.EqualTo("0123456789".IndexOfAny(new char[] { '3', '4', '9' }, 9)));
            Assert.That(-1, Is.EqualTo("0123456789".IndexOfAny(new char[] { '3', '4', '5' }, 6)));

            Assert.That(3, Is.EqualTo("0123456789".IndexOfAny(new char[] { '3', '4', '5' }, 3, 1)));
            Assert.That(4, Is.EqualTo("0123456789".IndexOfAny(new char[] { '3', '4', '5' }, 4, 3)));
            Assert.That(3, Is.EqualTo("0123456789".IndexOfAny(new char[] { '3', '4', '5' }, 1, 4)));
            Assert.That(-1, Is.EqualTo("0123456789".IndexOfAny(new char[] { '3', '4', '5' }, 6, 3)));
        }

        [Test]
        public void TestStringTrim()
        {
            Assert.That("0123456789XXX", Is.EqualTo("XXX0123456789XXX".TrimStart('X')));
            Assert.That("XXX0123456789XXX", Is.EqualTo("XXX0123456789XXX".TrimStart('Y')));

            Assert.That("XXX0123456789", Is.EqualTo("XXX0123456789XXX".TrimEnd('X')));
            Assert.That("XXX0123456789XXX", Is.EqualTo("XXX0123456789XXX".TrimEnd('Y')));

            Assert.That("0123456789", Is.EqualTo("XXX0123456789XXX".Trim('X')));
            Assert.That("0123456789", Is.EqualTo("XXX0123456789".Trim('X')));
            Assert.That("0123456789", Is.EqualTo("0123456789XXX".Trim('X')));
            Assert.That("XXX0123456789XXX", Is.EqualTo("XXX0123456789XXX".Trim('Y')));

            Assert.That("0123456789", Is.EqualTo("XXYYYZZZ0123456789YYYXXXZZZ".Trim(
                new char[] { 'X', 'Y', 'Z' })));
            Assert.That("ZZZ0123456789YYYXXXZZZ", Is.EqualTo("XXYYYZZZ0123456789YYYXXXZZZ".Trim(
                new char[] { 'X', 'Y' })));
            Assert.That("XXYYYZZZ0123456789YYYXXXZZZ", Is.EqualTo("XXYYYZZZ0123456789YYYXXXZZZ".Trim(
                new char[] { 'a', 'b', 'c' })));
        }

        [Test]
        public void TestStringJoin()
        {
            String[] strings = new String[] { "one", "two", "three" };
            Assert.That("oneXYZtwoXYZthree", Is.EqualTo(String.Join("XYZ", strings)));
            Assert.That("onetwothree", Is.EqualTo(String.Join("", strings)));
            Assert.That("onetwothree", Is.EqualTo(String.Join(null, strings)));
        }

        // >>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>> string.split(char[])

        [Test]
        public void TestStringSplitByChar()
        {
            String[] splitStrings = "one/two/three".Split('/');
            Assert.That(new String[] { "one", "two", "three" }, Is.EqualTo(splitStrings));

            splitStrings = "one/two/three/".Split('/');
            Assert.That(new String[] { "one", "two", "three", "" }, Is.EqualTo(splitStrings));

            splitStrings = "/one/two/three/".Split('/');
            Assert.That(new String[] { "", "one", "two", "three", "" }, Is.EqualTo(splitStrings));

            splitStrings = "/one//two/three/".Split('/');
            Assert.That(new String[] { "", "one", "", "two", "three", "" }, Is.EqualTo(splitStrings));

            splitStrings = "one/two/three".Split(';');
            Assert.That(new String[] { "one/two/three" }, Is.EqualTo(splitStrings));
        }

        [Test]
        public void TestStringSplitSeparatorsArray()
        {
            char[] separators = { ',', ';' };
            String[] splitStrings = "one,two;three".Split(separators);
            Assert.That(new String[] { "one", "two", "three" }, Is.EqualTo(splitStrings));

            splitStrings = "one,two,three".Split(separators);
            Assert.That(new String[] { "one", "two", "three" }, Is.EqualTo(splitStrings));

            splitStrings = "one;two;three".Split(separators);
            Assert.That(new String[] { "one", "two", "three" }, Is.EqualTo(splitStrings));

            splitStrings = "one,two;three;".Split(separators);
            Assert.That(new String[] { "one", "two", "three", "" }, Is.EqualTo(splitStrings));

            splitStrings = ",one;two;three,".Split(separators);
            Assert.That(new String[] { "", "one", "two", "three", "" }, Is.EqualTo(splitStrings));

            splitStrings = ";one,,two;three,".Split(separators);
            Assert.That(new String[] { "", "one", "", "two", "three", "" }, Is.EqualTo(splitStrings));

            splitStrings = "one/two/three".Split(separators);
            Assert.That(new String[] { "one/two/three" }, Is.EqualTo(splitStrings));

            //if it get null or empty separators array - it will find whitespaces.
            splitStrings = "one two three".Split(null);
            Assert.That(new String[] { "one", "two", "three" }, Is.EqualTo(splitStrings));

            splitStrings = "one two three".Split(new char[0]);
            Assert.That(new String[] { "one", "two", "three" }, Is.EqualTo(splitStrings));
        }

        [Test]
        public void TestStringSplitBySingleChar()
        {
            String[] splitStrings = "one/two/three".Split('/');
            Assert.That(new String[] { "one", "two", "three" }, Is.EqualTo(splitStrings));

            splitStrings = "one/two/three/".Split('/');
            Assert.That(new String[] { "one", "two", "three", "" }, Is.EqualTo(splitStrings));

            splitStrings = "/one/two/three/".Split('/');
            Assert.That(new String[] { "", "one", "two", "three", "" }, Is.EqualTo(splitStrings));

            splitStrings = "/one//two/three/".Split('/');
            Assert.That(new String[] { "", "one", "", "two", "three", "" }, Is.EqualTo(splitStrings));

            splitStrings = "/one//two/three/".Split('-');
            Assert.That(new String[] { "/one//two/three/" }, Is.EqualTo(splitStrings));
        }

        [Test]
        public void TestStringSplitByCharArray()
        {
            char[] separators = { '/', '-' };
            String[] splitStrings = "one/two/three".Split(separators);
            Assert.That(new String[] { "one", "two", "three" }, Is.EqualTo(splitStrings));

            splitStrings = "one-two-three".Split(separators);
            Assert.That(new String[] { "one", "two", "three" }, Is.EqualTo(splitStrings));

            splitStrings = "one/two-three/".Split(separators);
            Assert.That(new String[] { "one", "two", "three", "" }, Is.EqualTo(splitStrings));

            splitStrings = "-one/two/three-".Split(separators);
            Assert.That(new String[] { "", "one", "two", "three", "" }, Is.EqualTo(splitStrings));

            splitStrings = "/one--two/three/".Split(separators);
            Assert.That(new String[] { "", "one", "", "two", "three", "" }, Is.EqualTo(splitStrings));

            // non-found separators.
            separators = new char[] { ';', ':' };
            splitStrings = "/one/-two-three/".Split(separators);
            Assert.That(new String[] { "/one/-two-three/" }, Is.EqualTo(splitStrings));

            //if it get null or empty separators array - it will find whitespaces.
            splitStrings = "/one/-two-three/".Split(null);
            Assert.That(new String[] { "/one/-two-three/" }, Is.EqualTo(splitStrings));

            splitStrings = "one two three".Split(null);
            Assert.That(new String[] { "one", "two", "three" }, Is.EqualTo(splitStrings));

            splitStrings = "one two three".Split(new char[0]);
            Assert.That(new String[] { "one", "two", "three" }, Is.EqualTo(splitStrings));
        }

        [Test]
        public void TestStringSplitByCharArrayCount()
        {
            // Count 0 and 1 are special cases.
            // If count is less than splitted strings - the last string is concatenaded without split.

            char[] separators = { '/', '-' };
            String[] splitStrings = "one/two/three".Split(separators, 0);
            Assert.That(new String[0], Is.EqualTo(splitStrings));

            splitStrings = "one/two/three".Split(separators, 1);
            Assert.That(new String[] { "one/two/three" }, Is.EqualTo(splitStrings));

            splitStrings = "one-two-three".Split(separators, 2);
            Assert.That(new String[] { "one", "two-three" }, Is.EqualTo(splitStrings));

            splitStrings = "one/two-three/".Split(separators, 3);
            Assert.That(new String[] { "one", "two", "three/" }, Is.EqualTo(splitStrings));

            splitStrings = "-one/two/three-".Split(separators, 4);
            Assert.That(new String[] { "", "one", "two", "three-" }, Is.EqualTo(splitStrings));

            splitStrings = "/one--two/three/".Split(separators, 5);
            Assert.That(new String[] { "", "one", "", "two", "three/" }, Is.EqualTo(splitStrings));

            splitStrings = "/one--two/three/".Split(separators, 6);
            Assert.That(new String[] { "", "one", "", "two", "three", "" }, Is.EqualTo(splitStrings));

            splitStrings = "/one--two/three/".Split(separators, 7);
            Assert.That(new String[] { "", "one", "", "two", "three", "" }, Is.EqualTo(splitStrings));

            splitStrings = "/one--two/three/".Split(separators, 999);
            Assert.That(new String[] { "", "one", "", "two", "three", "" }, Is.EqualTo(splitStrings));

            separators = new char[] { ';', ':' };
            splitStrings = "/one/-two-three/".Split(separators, 1);
            Assert.That(new String[] { "/one/-two-three/" }, Is.EqualTo(splitStrings));

            splitStrings = "/one/-two-three/".Split(separators, 999);
            Assert.That(new String[] { "/one/-two-three/" }, Is.EqualTo(splitStrings));
        }

        [Test]
        public void TestStringSplitByCharArrayCountDontRemoveEmpty()
        {
            // Count 0 and 1 are special cases.
            // If count is less than splitted strings - the last string is concatenaded without split.

            char[] separators = { '/', '-' };
            String[] splitStrings = "one/two/three".Split(separators, 0, StringSplitOptions.None);
            Assert.That(new String[0], Is.EqualTo(splitStrings));

            splitStrings = "one/two/three".Split(separators, 1, StringSplitOptions.None);
            Assert.That(new String[] { "one/two/three" }, Is.EqualTo(splitStrings));

            splitStrings = "one-two-three".Split(separators, 2, StringSplitOptions.None);
            Assert.That(new String[] { "one", "two-three" }, Is.EqualTo(splitStrings));

            splitStrings = "one/two-three/".Split(separators, 3, StringSplitOptions.None);
            Assert.That(new String[] { "one", "two", "three/" }, Is.EqualTo(splitStrings));

            splitStrings = "-one/two/three-".Split(separators, 4, StringSplitOptions.None);
            Assert.That(new String[] { "", "one", "two", "three-" }, Is.EqualTo(splitStrings));

            splitStrings = "/one--two/three/".Split(separators, 5, StringSplitOptions.None);
            Assert.That(new String[] { "", "one", "", "two", "three/" }, Is.EqualTo(splitStrings));

            splitStrings = "/one--two/three/".Split(separators, 6, StringSplitOptions.None);
            Assert.That(new String[] { "", "one", "", "two", "three", "" }, Is.EqualTo(splitStrings));

            splitStrings = "/one--two/three/".Split(separators, 7, StringSplitOptions.None);
            Assert.That(new String[] { "", "one", "", "two", "three", "" }, Is.EqualTo(splitStrings));

            splitStrings = "/one--two/three/".Split(separators, 999, StringSplitOptions.None);
            Assert.That(new String[] { "", "one", "", "two", "three", "" }, Is.EqualTo(splitStrings));

            separators = new char[] { ';', ':' };
            splitStrings = "/one/-two-three/".Split(separators, 1, StringSplitOptions.None);
            Assert.That(new String[] { "/one/-two-three/" }, Is.EqualTo(splitStrings));

            splitStrings = "/one/-two-three/".Split(separators, 999, StringSplitOptions.None);
            Assert.That(new String[] { "/one/-two-three/" }, Is.EqualTo(splitStrings));
        }

        [Test]
        public void TestStringSplitByCharArrayRemoveEmpty()
        {
            char[] separators = { '/', '-' };
            String[] splitStrings = "one/two/three".Split(separators, StringSplitOptions.RemoveEmptyEntries);
            Assert.That(new String[] { "one", "two", "three" }, Is.EqualTo(splitStrings));

            splitStrings = "one-two-three".Split(separators, StringSplitOptions.RemoveEmptyEntries);
            Assert.That(new String[] { "one", "two", "three" }, Is.EqualTo(splitStrings));

            splitStrings = "one/two-three/".Split(separators, StringSplitOptions.RemoveEmptyEntries);
            Assert.That(new String[] { "one", "two", "three" }, Is.EqualTo(splitStrings));

            splitStrings = "-one/two/three-".Split(separators, StringSplitOptions.RemoveEmptyEntries);
            Assert.That(new String[] { "one", "two", "three" }, Is.EqualTo(splitStrings));

            splitStrings = "/one--two/three/".Split(separators, StringSplitOptions.RemoveEmptyEntries);
            Assert.That(new String[] { "one", "two", "three" }, Is.EqualTo(splitStrings));

            separators = new char[] { ';', ':' };
            splitStrings = "/one/-two-three/".Split(separators, StringSplitOptions.RemoveEmptyEntries);
            Assert.That(new String[] { "/one/-two-three/" }, Is.EqualTo(splitStrings));
        }

        [Test]
        public void TestStringSplitByCharArrayRemoveEmptyCount()
        {
            char[] separators = { '/', '-' };
            String[] splitStrings = "one/two/three".Split(separators, 0, StringSplitOptions.RemoveEmptyEntries);
            Assert.That(new String[0], Is.EqualTo(splitStrings));

            splitStrings = "one/two/three".Split(separators, 1, StringSplitOptions.RemoveEmptyEntries);
            Assert.That(new String[] { "one/two/three" }, Is.EqualTo(splitStrings));

            splitStrings = "one-two-three".Split(separators, 2, StringSplitOptions.RemoveEmptyEntries);
            Assert.That(new String[] { "one", "two-three" }, Is.EqualTo(splitStrings));

            splitStrings = "one/two-three/".Split(separators, 3, StringSplitOptions.RemoveEmptyEntries);
            Assert.That(new String[] { "one", "two", "three/" }, Is.EqualTo(splitStrings));

            splitStrings = "-one/two/three-".Split(separators, 4, StringSplitOptions.RemoveEmptyEntries);
            Assert.That(new String[] { "one", "two", "three" }, Is.EqualTo(splitStrings));

            splitStrings = "/one--two/three/".Split(separators, 5, StringSplitOptions.RemoveEmptyEntries);
            Assert.That(new String[] { "one", "two", "three" }, Is.EqualTo(splitStrings));

            splitStrings = "/one--two/three/".Split(separators, 999, StringSplitOptions.RemoveEmptyEntries);
            Assert.That(new String[] { "one", "two", "three" }, Is.EqualTo(splitStrings));

            separators = new char[] { ';', ':' };
            splitStrings = "/one/-two-three/".Split(separators, 1, StringSplitOptions.RemoveEmptyEntries);
            Assert.That(new String[] { "/one/-two-three/" }, Is.EqualTo(splitStrings));

            splitStrings = "/one/-two-three/".Split(separators, 7, StringSplitOptions.RemoveEmptyEntries);
            Assert.That(new String[] { "/one/-two-three/" }, Is.EqualTo(splitStrings));
        }

        // <<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<< string.split(char[])

        // >>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>> string.split(string[])

        [Test]
        public void TestStringSplitByStringDontRemoveEmpty()
        {
            String[] separators = { "/r" };
            String[] splitStrings = "one/rtwo/rthree".Split(separators, StringSplitOptions.None);
            Assert.That(new String[] { "one", "two", "three" }, Is.EqualTo(splitStrings));

            splitStrings = "one/rtwo/rthree/r".Split(separators, StringSplitOptions.None);
            Assert.That(new String[] { "one", "two", "three", "" }, Is.EqualTo(splitStrings));

            splitStrings = "/rone/rtwo/rthree/r".Split(separators, StringSplitOptions.None);
            Assert.That(new String[] { "", "one", "two", "three", "" }, Is.EqualTo(splitStrings));

            splitStrings = "/rone/r/rtwo/rthree/r".Split(separators, StringSplitOptions.None);
            Assert.That(new String[] { "", "one", "", "two", "three", "" }, Is.EqualTo(splitStrings));

            separators[0] = "/f";
            splitStrings = "/rone/r/rtwo/rthree/r".Split(separators, StringSplitOptions.None);
            Assert.That(new String[] { "/rone/r/rtwo/rthree/r" }, Is.EqualTo(splitStrings));
        }

        [Test]
        public void TestStringSplitByStringArrayDontRemoveEmpty()
        {
            String[] separators = { "/r/n", "/r", "/n" };
            String[] splitStrings = "one/rtwo/r/nthree".Split(separators, StringSplitOptions.None);
            Assert.That(new String[] { "one", "two", "three" }, Is.EqualTo(splitStrings));

            splitStrings = "one/ntwo/rthree/r/n".Split(separators, StringSplitOptions.None);
            Assert.That(new String[] { "one", "two", "three", "" }, Is.EqualTo(splitStrings));

            splitStrings = "/r/none/rtwo/nthree/r/n".Split(separators, StringSplitOptions.None);
            Assert.That(new String[] { "", "one", "two", "three", "" }, Is.EqualTo(splitStrings));

            splitStrings = "/r/none/r/n/rtwo/rthree/r/n".Split(separators, StringSplitOptions.None);
            Assert.That(new String[] { "", "one", "", "two", "three", "" }, Is.EqualTo(splitStrings));

            separators = new String[] { "/f", "/t", "/ab" };
            splitStrings = "/rone/r/rtwo/rthree/r".Split(separators, StringSplitOptions.None);
            Assert.That(new String[] { "/rone/r/rtwo/rthree/r" }, Is.EqualTo(splitStrings));
        }

        [Test]
        public void TestStringSplitByStringArrayDontRemoveEmptyCount()
        {
            String[] separators = { "/r/n", "/r", "/n" };
            String[] splitStrings = "one/rtwo/r/nthree".Split(separators, 0, StringSplitOptions.None);
            Assert.That(new String[0], Is.EqualTo(splitStrings));

            splitStrings = "one/rtwo/r/nthree".Split(separators, 1, StringSplitOptions.None);
            Assert.That(new String[] { "one/rtwo/r/nthree" }, Is.EqualTo(splitStrings));

            splitStrings = "one/ntwo/rthree/r/n".Split(separators, 2, StringSplitOptions.None);
            Assert.That(new String[] { "one", "two/rthree/r/n" }, Is.EqualTo(splitStrings));

            splitStrings = "/r/none/rtwo/nthree/r/n".Split(separators, 3, StringSplitOptions.None);
            Assert.That(new String[] { "", "one", "two/nthree/r/n" }, Is.EqualTo(splitStrings));

            splitStrings = "/r/none/r/n/rtwo/rthree/r/n".Split(separators, 4, StringSplitOptions.None);
            Assert.That(new String[] { "", "one", "", "two/rthree/r/n" }, Is.EqualTo(splitStrings));

            splitStrings = "/r/none/r/n/rtwo/rthree/r/n".Split(separators, 5, StringSplitOptions.None);
            Assert.That(new String[] { "", "one", "", "two", "three/r/n" }, Is.EqualTo(splitStrings));

            splitStrings = "/r/none/r/n/rtwo/rthree/r/n".Split(separators, 6, StringSplitOptions.None);
            Assert.That(new String[] { "", "one", "", "two", "three", "" }, Is.EqualTo(splitStrings));

            splitStrings = "/r/none/r/n/rtwo/rthree/r/n".Split(separators, 7, StringSplitOptions.None);
            Assert.That(new String[] { "", "one", "", "two", "three", "" }, Is.EqualTo(splitStrings));

            splitStrings = "/r/none/r/n/rtwo/rthree/r/n".Split(separators, 999, StringSplitOptions.None);
            Assert.That(new String[] { "", "one", "", "two", "three", "" }, Is.EqualTo(splitStrings));

            separators = new String[] { "/f", "/t", "/ab" };
            splitStrings = "/rone/r/rtwo/rthree/r".Split(separators, 1, StringSplitOptions.None);
            Assert.That(new String[] { "/rone/r/rtwo/rthree/r" }, Is.EqualTo(splitStrings));

            splitStrings = "/rone/r/rtwo/rthree/r".Split(separators, 3, StringSplitOptions.None);
            Assert.That(new String[] { "/rone/r/rtwo/rthree/r" }, Is.EqualTo(splitStrings));
        }

        [Test]
        public void TestStringSplitByStringRemoveEmpty()
        {
            String[] separators = { "/r" };
            String[] splitStrings = "one/rtwo/rthree".Split(separators, StringSplitOptions.RemoveEmptyEntries);
            Assert.That(new String[] { "one", "two", "three" }, Is.EqualTo(splitStrings));

            splitStrings = "one/rtwo/rthree/r".Split(separators, StringSplitOptions.RemoveEmptyEntries);
            Assert.That(new String[] { "one", "two", "three" }, Is.EqualTo(splitStrings));

            splitStrings = "/rone/rtwo/rthree/r".Split(separators, StringSplitOptions.RemoveEmptyEntries);
            Assert.That(new String[] { "one", "two", "three" }, Is.EqualTo(splitStrings));

            splitStrings = "/rone/r/rtwo/rthree/r".Split(separators, StringSplitOptions.RemoveEmptyEntries);
            Assert.That(new String[] { "one", "two", "three" }, Is.EqualTo(splitStrings));

            separators[0] = "/f";
            splitStrings = "/rone/r/rtwo/rthree/r".Split(separators, StringSplitOptions.RemoveEmptyEntries);
            Assert.That(new String[] { "/rone/r/rtwo/rthree/r" }, Is.EqualTo(splitStrings));
        }

        [Test]
        public void TestStringSplitByStringArrayRemoveEmpty()
        {
            String[] separators = { "/r/n", "/r", "/n" };
            String[] splitStrings = "one/rtwo/r/nthree".Split(separators, StringSplitOptions.RemoveEmptyEntries);
            Assert.That(new String[] { "one", "two", "three" }, Is.EqualTo(splitStrings));

            splitStrings = "one/ntwo/rthree/r/n".Split(separators, StringSplitOptions.RemoveEmptyEntries);
            Assert.That(new String[] { "one", "two", "three" }, Is.EqualTo(splitStrings));

            splitStrings = "/r/none/rtwo/nthree/r/n".Split(separators, StringSplitOptions.RemoveEmptyEntries);
            Assert.That(new String[] { "one", "two", "three" }, Is.EqualTo(splitStrings));

            splitStrings = "/r/none/r/n/rtwo/rthree/r/n".Split(separators, StringSplitOptions.RemoveEmptyEntries);
            Assert.That(new String[] { "one", "two", "three" }, Is.EqualTo(splitStrings));

            separators = new String[] { "/f", "/t", "/ab" };
            splitStrings = "/rone/r/rtwo/rthree/r".Split(separators, StringSplitOptions.RemoveEmptyEntries);
            Assert.That(new String[] { "/rone/r/rtwo/rthree/r" }, Is.EqualTo(splitStrings));
        }


        [Test]
        public void TestStringSplitByStringArrayRemoveEmptyCount()
        {
            // Count 0 and 1 are special cases.
            // If count is less than splitted strings - the last string is concatenaded without split.

            String[] separators = { "/r/n", "/r", "/n" };
            String[] splitStrings = "one/rtwo/r/nthree".Split(separators, 0, StringSplitOptions.RemoveEmptyEntries);
            Assert.That(new String[0], Is.EqualTo(splitStrings));

            splitStrings = "one/rtwo/r/nthree".Split(separators, 1, StringSplitOptions.RemoveEmptyEntries);
            Assert.That(new String[] { "one/rtwo/r/nthree" }, Is.EqualTo(splitStrings));

            splitStrings = "one/ntwo/rthree/r/n".Split(separators, 2, StringSplitOptions.RemoveEmptyEntries);
            Assert.That(new String[] { "one", "two/rthree/r/n" }, Is.EqualTo(splitStrings));

            splitStrings = "/r/none/rtwo/nthree/r/n".Split(separators, 3, StringSplitOptions.RemoveEmptyEntries);
            Assert.That(new String[] { "one", "two", "three/r/n" }, Is.EqualTo(splitStrings));

            splitStrings = "/r/none/r/n/rtwo/rthree/r/n".Split(separators, 4, StringSplitOptions.RemoveEmptyEntries);
            Assert.That(new String[] { "one", "two", "three" }, Is.EqualTo(splitStrings));

            separators = new String[] { "/f", "/t", "/ab" };
            splitStrings = "/rone/r/rtwo/rthree/r".Split(separators, 1, StringSplitOptions.RemoveEmptyEntries);
            Assert.That(new String[] { "/rone/r/rtwo/rthree/r" }, Is.EqualTo(splitStrings));

            splitStrings = "/rone/r/rtwo/rthree/r".Split(separators, 4, StringSplitOptions.RemoveEmptyEntries);
            Assert.That(new String[] { "/rone/r/rtwo/rthree/r" }, Is.EqualTo(splitStrings));
        }

        // <<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<< string.split(string[])

        [Test]
        public void TestIsNullOrEmpty()
        {
            String nullStr = null;
            String emptyStr = "";

            Assert.That(String.IsNullOrEmpty(nullStr), Is.True);
            Assert.That(String.IsNullOrEmpty(emptyStr), Is.True);
        }

        [Test]
        public void TestToUpperInvariant()
        {
            Assert.That(("Hello" + MicroSign).ToUpperInvariant(), Is.EqualTo("HELLO" + MicroSign));
        }

        [Test]
        public void TestToLowerInvariant()
        {
            Assert.That(("HELLO" + MicroSign).ToLowerInvariant(), Is.EqualTo("hello" + MicroSign));
        }

        [Test]
        public void TestCompareInvariant()
        {
            for (int i = (int)'a'; i <= (int)'z'; i++)
            {
                // normal rules: a < A and a'b < a'B
                Assert.That(-1, Is.EqualTo(string.Compare("" + (char)i, "" + Char.ToUpper((char)i), StringComparison.InvariantCulture)));
                Assert.That(-1, Is.EqualTo(string.Compare("a'" + (char)i, "a'" + Char.ToUpper((char)i), StringComparison.InvariantCulture)));

                // weird rules:
                //  ab < a'b    , but
                //  ab > a' b
                Assert.That(-1, Is.EqualTo(string.Compare("a" + (char)i, "a'" + (char)i, StringComparison.InvariantCulture)));
                Assert.That(1, Is.EqualTo(string.Compare("a" + (char)i, "a' " + (char)i, StringComparison.InvariantCulture)));

                // uppercase weird rules:
                //  aB < a'B    , but
                //  aB > a' B
                Assert.That(-1, Is.EqualTo(string.Compare("a" + Char.ToUpper((char)i), "a'" + Char.ToUpper((char)i), StringComparison.InvariantCulture)));
                Assert.That(1, Is.EqualTo(string.Compare("a" + Char.ToUpper((char)i), "a' " + Char.ToUpper((char)i), StringComparison.InvariantCulture)));
            }
        }

        [Test]
        public void TestCompareInvariantIgnoreCase()
        {
            for (int i = (int)'a'; i <= (int)'z'; i++)
            {
                // normal rules: a = A and a'b = a'B
                Assert.That(0, Is.EqualTo(string.Compare("" + (char)i, "" + Char.ToUpper((char)i), StringComparison.InvariantCultureIgnoreCase)));
                Assert.That(0, Is.EqualTo(string.Compare("a'" + (char)i, "a'" + Char.ToUpper((char)i), StringComparison.InvariantCultureIgnoreCase)));

                // weird rules:
                //  ab < a'b    , but
                //  ab > a' b
                Assert.That(-1, Is.EqualTo(string.Compare("a" + (char)i, "a'" + (char)i, StringComparison.InvariantCultureIgnoreCase)));
                Assert.That(1, Is.EqualTo(string.Compare("a" + (char)i, "a' " + (char)i, StringComparison.InvariantCultureIgnoreCase)));

                // uppercase weird rules:
                //  aB < a'B    , but
                //  aB > a' B
                Assert.That(-1, Is.EqualTo(string.Compare("a" + Char.ToUpper((char)i), "a'" + Char.ToUpper((char)i), StringComparison.InvariantCultureIgnoreCase)));
                Assert.That(1, Is.EqualTo(string.Compare("a" + Char.ToUpper((char)i), "a' " + Char.ToUpper((char)i), StringComparison.InvariantCultureIgnoreCase)));
            }
        }

        // J1678: micro sign char stays the same while converting to upper or to lower invariant in .Net.
        private const string MicroSign = "\u00b5";
    }
}
