// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 12/09/2014 by Victor Chebotok

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Aspose.Bidi;
using Aspose.Collections;
using Aspose.Common;
using Aspose.TestFx;
using NUnit.Framework;

namespace Aspose.Tests.Bidi
{
    /// <summary>
    /// Tests our implementation of the Unicode bidirectional algorithm (the class <see cref="BidiAlgorithm"/>
    /// for conformance to the specification, as described here: http://www.unicode.org/reports/tr9/#Bidi_Conformance_Testing
    /// At the moment, only BidiTest.txt file is used.
    /// </summary>
    [TestFixture]
    public class TestBidiAlgorithmConformance
    {
        static TestBidiAlgorithmConformance()
        {
            gCharacters = new StringToCharDictionary();
            gCharacters.Add("LRE", BidiChars.LRE);
            gCharacters.Add("RLE", BidiChars.RLE);
            gCharacters.Add("PDF", BidiChars.PDF);
            gCharacters.Add("LRO", BidiChars.LRO);
            gCharacters.Add("RLO", BidiChars.RLO);
            gCharacters.Add("BN",  '\u00AD'); // SOFT HYPHEN
            gCharacters.Add("L",   '\u0041'); // LATIN CAPITAL LETTER A
            gCharacters.Add("R",   '\u05D0'); // HEBREW LETTER ALEF
            gCharacters.Add("EN",  '\u0033'); // DIGIT THREE
            gCharacters.Add("ES",  '\u002B'); // PLUS SIGN
            gCharacters.Add("ET",  '\u0024'); // DOLLAR SIGN
            gCharacters.Add("CS",  '\u002C'); // COMMA
            gCharacters.Add("NSM", '\u0302'); // COMBINING CIRCUMFLEX ACCENT
            gCharacters.Add("B",   '\u000A'); // LINE FEED (LF)
            gCharacters.Add("S",   '\u0009'); // CHARACTER TABULATION
            gCharacters.Add("WS",  '\u0020'); // SPACE
            gCharacters.Add("ON",  '\u0021'); // EXCLAMATION MARK
            gCharacters.Add("AL" , '\u062D'); // ARABIC LETTER HAA
            gCharacters.Add("AN",  '\u0663'); // ARABIC-INDIC DIGIT THREE
        }

        [Test]
        [Category("CppSlowTest")]
        public void TestAllCases()
        {
            BidiUcdTestCase[] testCases = LoadTestCases();
            int failedCount = 0;
            for (int i = 0; i < testCases.Length; i++)
            {
                bool passed = TestOneCase(testCases[i], i, true);
                if (!passed)
                {
                    ++failedCount;
                }
            }
            if (failedCount > 0)
            {
                int totalCount = testCases.Length;
                double failedPercent = (totalCount > 0)
                    ? (double)failedCount / totalCount * 100
                    : 0;
                Assert.Fail(string.Format("{0} of {1} ({2:N2}%) tests failed.", failedCount, totalCount, failedPercent));
            }
        }

        [Test]
        [Ignore("When debugging, this test is used to manually run just one specific test case.")]
        public void TestOneCaseByIndex()
        {
            TestOneCaseByIndex(162871);
        }

        private void TestOneCaseByIndex(int caseIndex)
        {
            BidiUcdTestCase[] testCases = LoadTestCases();
            bool passed = TestOneCase(testCases[caseIndex], caseIndex, true);
            if (!passed)
            {
                Assert.Fail("Test failed.");
            }
        }

        private bool TestOneCase(BidiUcdTestCase testCase, int testCaseIndex, bool outputDebugInfo)
        {
            BidiRun[] expectedRuns = testCase.GetExpectedRuns();
            BidiRun[] actualRuns = BidiAlgorithm.Apply(testCase.Text, testCase.IsRtlParagraph);

            string expectedText = RunsToString(expectedRuns);
            string actualText = RunsToString(actualRuns);
            
            bool testPassed = actualText == expectedText;
            if ((!testPassed) && outputDebugInfo)
            {
                string paragraphDirection = (testCase.IsRtlParagraph)
                    ? "RTL"
                    : "LTR";
                Debug.WriteLine(string.Format("Expected <{0}> but was <{1}> at line {2} ({3}). Case index {4}.", expectedText, actualText, testCase.LineNumber, paragraphDirection, testCaseIndex));
            }
            return testPassed;
        }

        private string RunsToString(BidiRun[] runs)
        {
            StringBuilder result = new StringBuilder();
            bool firstRun = true;
            foreach (BidiRun run in runs)
            {
                string filteredRunText = FilterOutBidiControlChars(run.Text);

                if (!StringUtil.HasChars(filteredRunText))
                {
                    continue;
                }

                if (!firstRun)
                {
                    result.Append(' ');
                }
                firstRun = false;

                result.Append(string.Format("{0}:(", run.Level));

                bool firstCodePoint = true;
                foreach (int c in new StringUtf32Enumerator(filteredRunText))
                {
                    if (!firstCodePoint)
                    {
                        result.Append(' ');
                    }
                    firstCodePoint = false;

                    result.Append(FormatterPal.IntToStrX4(c));
                }

                result.Append(")");
            }
            
            return result.ToString();
        }

        private string FilterOutBidiControlChars(string text)
        {
            StringBuilder result = new StringBuilder(text.Length);
            foreach (char c in text)
            {
                // Our implementation of the Unicode Bidi Algorithm does not filter the boundary neutral (BN) characters,
                // while the original UBA does.
                bool filtered = BidiAlgorithm.IsControlChar(c) ||
                    (UnicodeCharacterDataResolver.GetBidiCharacterType(c) == BidiCharacterType.BN);
                if (!filtered)
                {
                    result.Append(c);
                }
            }
            return result.ToString();
        }

        private BidiUcdTestCase[] LoadTestCases()
        {
            List<BidiUcdTestCase> result = new List<BidiUcdTestCase>();

            using (StreamReader testFile = new StreamReader(TestFxUtil.BuildTestFileName(@"NBidi\BidiTest.txt")))
            {
                int[] levels = new int[0];

                int lineNumber = 0;
                while (true)
                {
                    string line = testFile.ReadLine();
                    if (line == null)
                        break;

                    ++lineNumber;

                    if ((line.Length == 0) || line.StartsWith("#", StringComparison.Ordinal))
                    {
                        // Skip comments and empty lines.
                        continue;
                    }

                    if (line.StartsWith("@", StringComparison.Ordinal))
                    {
                        // Unknown labels are skipped for compatibility with different versions of the BidiTests.txt file.
                        // For example, older versions of the file contained "@Type" labels, which are removed now.
                        const string levelsLabel = "@Levels:\t";
                        if (line.StartsWith(levelsLabel))
                        {
                            levels = ParseLevels(line.Substring(levelsLabel.Length));
                        }
                    }
                    else
                    {
                        BidiUcdTestCase[] testCases = ParseTestCases(line, levels, lineNumber);
                        foreach (BidiUcdTestCase testCase in testCases)
                        {
                            result.Add(testCase);
                        }
                    }
                }
            }

            return result.ToArray();
        }

        private static int[] ParseLevels(string levels)
        {
            if (!StringUtil.HasChars(levels))
            {
                return new int[0];
            }

            string[] parts = levels.Split(' ');
            int[] result = new int[parts.Length];
            for (int i = 0; i < parts.Length; i++)
            {
                if (parts[i] == "x")
                {
                    result[i] = -1;
                }
                else
                {
                    result[i] = FormatterPal.ParseInt(parts[i]);
                }
            }
            return result;
        }

        private BidiUcdTestCase[] ParseTestCases(string testCases, int[] expectedLevels, int lineNumber)
        {
            List<BidiUcdTestCase> result = new List<BidiUcdTestCase>();

            int semicolonPosition = testCases.IndexOf(';');
            if (semicolonPosition >= 0)
            {
                string text = ParseTestCaseText(testCases.Substring(0, semicolonPosition));
                int directions = FormatterPal.ParseInt(testCases.Substring(semicolonPosition + 1));
                if ((directions & 2) != 0)
                {
                    // Left-to-Right.
                    result.Add(new BidiUcdTestCase(text, expectedLevels, false, lineNumber));
                }
                if ((directions & 4) != 0)
                {
                    // Right-to-Left.
                    result.Add(new BidiUcdTestCase(text, expectedLevels, true, lineNumber));
                }
            }

            return result.ToArray();
        }

        private string ParseTestCaseText(string text)
        {
            string[] bidiTypes = text.Split(' ');
            StringBuilder result = new StringBuilder();
            foreach (string bidiType in bidiTypes)
            {
                result.Append(GetCharByBidiType(bidiType));
            }
            return result.ToString();
        }

        private static char GetCharByBidiType(string bidiType)
        {
            char result = gCharacters[bidiType];
            if (StringToCharDictionary.IsNullSubstitute(result))
            {
                Assert.Fail("Unknown bidi type: " + bidiType);
            }
            return result;
        }

        private static readonly StringToCharDictionary gCharacters = new StringToCharDictionary();
    }

    internal class BidiUcdTestCase
    {
        internal BidiUcdTestCase(string text, int[] expectedLevels, bool isInRtlParagraph, int lineNumber)
        {
            Debug.Assert(StringUtf32Enumerator.GetStringLength(text) == expectedLevels.Length);

            mText = text;
            mExpectedLevels = expectedLevels;
            mIsRtlParagraph = isInRtlParagraph;
            mLineNumber = lineNumber;
        }

        internal string Text
        {
            get { return mText; }
        }

        internal bool IsRtlParagraph
        {
            get { return mIsRtlParagraph; }
        }

        internal int LineNumber
        {
            get { return mLineNumber; }
        }

        internal BidiRun[] GetExpectedRuns()
        {
            IntList resultCharacters = new IntList();
            IntList resultLevels = new IntList();

            int charIndex = 0;
            foreach (int utf32Char in new StringUtf32Enumerator(mText))
            {
                int expectedCharLevel = mExpectedLevels[charIndex];
                ++charIndex;

                // If the character is not removed by the Bidi Algorithm, append it to the result text.
                if (expectedCharLevel >= 0)
                {
                    resultCharacters.Add(utf32Char);
                    resultLevels.Add(expectedCharLevel);
                }
            }
            Debug.Assert(resultCharacters.Count == resultLevels.Count);

            List<BidiRun> result = new List<BidiRun>();

            // Group characters by embedding level.
            int runLevel = -1;
            StringBuilder runText = new StringBuilder();
            for (int i = 0; i < (resultLevels.Count + 1); i++)
            {
                // An implicit end-of-text character with an invalid embedding level is used to terminate the last group.
                // This simplifies the processing loop and removes the code duplication (otherwise the last group
                // would be processed after the loop).
                int currentLevel = (i < resultLevels.Count)
                    ? resultLevels[i]
                    : -1;

                if (currentLevel != runLevel)
                {
                    if (runText.Length > 0)
                    {
                        result.Add(new BidiRun(runText.ToString(), runLevel, 0));
                    }
                    runLevel = currentLevel;
                    runText.Length = 0;
                }

                if (currentLevel >= 0)
                {
                    UnicodeUtil.AppendUtf32(runText, resultCharacters[i]);
                }
            }

            return result.ToArray();
        }

        private readonly string mText;
        private readonly int[] mExpectedLevels;
        private readonly bool mIsRtlParagraph;
        private readonly int mLineNumber;
    }
}
