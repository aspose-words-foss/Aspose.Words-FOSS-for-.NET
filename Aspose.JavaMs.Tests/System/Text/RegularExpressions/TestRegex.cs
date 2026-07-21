// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 24/01/2007 by Konstantin Sidorenko
// 2016/04/28 by Anatoliy Sidorenko

using System;
using System.Text.RegularExpressions;
using NUnit.Framework;

namespace Aspose.JavaMs.Tests.System.Text.RegularExpressions
{
    [TestFixture]
    public class TestRegex
    {
#if JAVA
        /// <summary>
        /// Null internal Regex should be returned as null for public api.
        /// </summary>
        [Test]
        public void TestGetNullRegex()
        {
            Regex regex = null;
            Assert.assertNull(Regex.toJava(regex));
        }

        [Test]
        public void TestRegexCtor()
        {
            Regex regex = new Regex("regex");
            msAssert.areEqual(regex.toJava().pattern(), "regex");
        }
#endif

        [Test]
        public void TestJava2238()
        {
            string input = "1";

            Assert.That(Regex.Match(input, "^(1) (?:(2)(3))?$").Groups.Count, Is.EqualTo(1));
            Assert.That(Regex.Match(input, "^(1)(?:(2)(3))?$").Groups.Count, Is.EqualTo(4));
            Assert.That(Regex.Match(input, "^(?:1)(?:(2)(3))?$").Groups.Count, Is.EqualTo(3));
        }

        [Test]
        public void TestRegexEscape()
        {
            string str = "\\,*,+,?,|,{,[,(,),^,$,.,#,and white space ";
            string escaped = "\\\\,\\*,\\+,\\?,\\|,\\{,\\[,\\(,\\),\\^,\\$,\\.,\\#,and\\ white\\ space\\ ";
            Assert.That(escaped, Is.EqualTo(Regex.Escape(str)));

            Assert.That("", Is.EqualTo(Regex.Escape("")));
        }

        [Test][ExpectedException(typeof(ArgumentNullException))] 
        public void TestRegexEscapeNull()
        {
            Assert.That("", Is.EqualTo(Regex.Escape(null)));
        }

        [Test]
        public void TestIsMatch()
        {
            string regex = "\\<html";
            string input = "..<html>aaaa</html>";

            bool match = Regex.IsMatch(input, regex, RegexOptions.IgnoreCase);
            Debug.WriteLine("match = " + match);
        }

        [Test]
        public void TestEscapeWorksWithUnicode()
        {
            string regexStr = "\\w+";
            string input = "Лозунг";
            
            Regex regex = new Regex(regexStr);
            Match match = regex.Match(input);
            string value = match.Value;
            Assert.That(input, Is.EqualTo(value));
        }

        [Test]
        public void TestSplit()
        {
            string regexStr = "\\s";
            string[] words;
            Regex regex = new Regex(regexStr);
            words = regex.Split(" 2018-08-15 00:00:00", 0);
            Assert.That("", Is.EqualTo(words[0]));
            Assert.That("2018-08-15", Is.EqualTo(words[1]));
            Assert.That("00:00:00", Is.EqualTo(words[2]));

            words = regex.Split(" 2018-08-15 00:00:00", 0, 1);
            Assert.That(" 2018-08-15", Is.EqualTo(words[0]));
            Assert.That("00:00:00", Is.EqualTo(words[1]));

            words = regex.Split(" 2018- 08- 15 00:00:00", 0, 1);
            Assert.That(" 2018-", Is.EqualTo(words[0]));
            Assert.That("08-", Is.EqualTo(words[1]));
            Assert.That("15", Is.EqualTo(words[2]));
            Assert.That("00:00:00", Is.EqualTo(words[3]));
        }

        /// <summary>
        /// Complex test how Regex and Match emulate .Net's System.Text.RegularExpressions.Regex and
        /// System.Text.RegularExpressions.Match.
        /// </summary>
        [Test]
        public void TestMatch()
        {
            Regex regex = new Regex(pattern);

            for (int i = 0; i < fieldcodes.Length; i++)
            {
                Match match = regex.Match(fieldcodes[i]);

                //0 group: the whole input string for matched regex or "" if regex match failed (java throw)
                string group0 = match.Groups[0].Value;
                Assert.That(result[i][0], Is.EqualTo(group0));

                //optional group: "" for non-matched (in java - null)
                string group1 = match.Groups[1].Value;
                Assert.That(result[i][1], Is.EqualTo(group1));

                //ordinal capturung group: "" for non-matched group (i.e. if regex match failed)
                string group2 = match.Groups[2].Value;
                Assert.That(result[i][2], Is.EqualTo(group2));

                //out of index group (regex have only 2 groups) - in java throws
                string group3 = match.Groups[3].Value;
                Assert.That(result[i][3], Is.EqualTo(group3));

                //negative index - in java throws
                string groupNegative = match.Groups[-33].Value;
                Assert.That(result[i][4], Is.EqualTo(groupNegative));
            }
        }

        /// <summary>
        /// Some greedy patterns, for example <tt>a*</tt> or <tt>[^a]*</tt>, match the empty string.
        /// When used in Regex.matches() them infinitely return this empty group.
        /// </summary>
        [Test]
        public void TestGreedy()
        {
            //was: matcher's start == end and both equl the end of text string (14).
            string pattern = "([^a]*)";
            string text = " FORMCHECKBOX ";
            Regex regex = new Regex(pattern);
            MatchCollection matchers = regex.Matches(text); //infinite loop was here.

            //was: matcher's start == end and both equl the start of text string (0).
            pattern = "a*";
            regex = new Regex(pattern);
            matchers = regex.Matches(text); //infinite loop was here.
        }

        /// <summary>
        /// WORDSJAVA-1422
        /// Tests if Java can handle not escaped {} in case they are not repetition expression.
        /// </summary>
        [TestCase("\\{([^}]*.?)}", "\\{([^}]*.?)}")]
        [TestCase("^(([^\\.]+)?\\.)?(\\$?([A-Z]+)\\$?(\\d+)$)", "^(([^\\.]+)?\\.)?(\\$?([A-Z]+)\\$?(\\d+)$)")]
        [TestCase("[0-9]{1,4}", "[0-9]{1,4}")]
        [TestCase("[0-9]{5}", "[0-9]{5}")]
#if JAVA
        // Java must escape { when the expression {} is not a repetition one. 
        // It is enought to escape just open curve brace.
        [TestCase("{[^}]*}", "\\{[^}]*}")] 
        [TestCase("\\p{IsL}", "\\p{IsL}")] 
        [TestCase("[0-9]{5m}", "[0-9]\\{5m}")]
#else
        // .NET doesn't escape it at all.
        [TestCase("{[^}]*}", "{[^}]*}")]
        [TestCase("[0-9]{5m}", "[0-9]{5m}")]
#endif
        public void TestRepetition(string actual, string expected)
        {
            Regex normalized = new Regex(actual);
            Assert.That(normalized.ToString(), Is.EqualTo(expected));
        }

        private string pattern =
            "\\S+" + //one or more non spaces HYPERLINK or other word in other languages
            "\\s+" + //one or more spaces
            "(?:\"\"\\s+)?" + //non capturing optional "" and one or more spaces, found in one of the customers files.
            "(\\\\l\\s+)?" + //optional \l flag followed by one or more spaces
            "\"" + //one apostrophe
            "([^\"]+)" + //one or more chars except apostrophe (destination)
            "\""; //one closing apostrophe

        private static string[] fieldcodes =
        {
            "HYPERLINK mailto:a.shearer01@hotmail.com",
            "HYPERLINK \"mailto:a.shearer01@hotmail.com\"",
            "HYPERLINK \"\" \"mailto:a.shearer01@hotmail.com\"",
            "HYPERLINK \\l \"mailto:a.shearer01@hotmail.com\""
        };

        private string[][] result =
        {
            new string[]{"", "", "", "", ""},
            new string[]{fieldcodes[1], "", "mailto:a.shearer01@hotmail.com", "", ""},
            new string[]{fieldcodes[2], "", "mailto:a.shearer01@hotmail.com", "", ""},
            new string[]{fieldcodes[3], "\\l ", "mailto:a.shearer01@hotmail.com", "", ""}
        };
    }
}
