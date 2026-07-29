// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 16/03/2020 by Edward Voronov

using System;
using System.Collections.Generic;
using System.Globalization;
using Aspose.Common;
using Aspose.Drawing;
using Aspose.Words.Fields;
using NUnit.Framework;

namespace Aspose.Words.Tests.Fields
{
    [TestFixture]
    [JavaAttributes.AndroidManual]
    public class TestRichStringNumberFormatter : TestFieldsBase
    {
        static TestRichStringNumberFormatter()
        {
            gRedFont = new RunPr();
            gRedFont.Bold = AttrBoolEx.True;
            gRedFont.Color = DrColor.Red;

            gGreenFont = new RunPr();
            gGreenFont.Italic = AttrBoolEx.True;
            gGreenFont.Color = DrColor.Green;

            gBlueFont = new RunPr();
            gBlueFont.Size = 8;
            gBlueFont.Color = DrColor.Blue;

            gYellowFont = new RunPr();
            gYellowFont.Underline = Underline.Single;
            gYellowFont.Color = DrColor.Yellow;
        }

        [Test]
        [TestCaseSource("TestNumberToRichStringCases")]
        public void TestNumberToRichString(TestNumberToRichStringCase testCase)
        {
            RichString actual = (RichString)NumberFormatter.NumberToString(
                testCase.Format,
                testCase.Value,
                CultureInfo.CurrentCulture,
                NumberFormattingOptions.Default,
                RichStringBehaviour.Instance);

            TestRichStrings.AssertRichString(testCase.ExpectedResult, actual);
        }

        private static IEnumerable<TestCaseData> TestNumberToRichStringCases()
        {
            List<TestCaseData> cases = new List<TestCaseData>();

            cases.Add(new TestCaseData(new TestNumberToRichStringCase(
                7,
                BuildRichString(),
                BuildRichString("7"))));

            cases.Add(new TestCaseData(new TestNumberToRichStringCase(
                4,
                BuildRichString("#"),
                BuildRichString("4"))));

            cases.Add(new TestCaseData(new TestNumberToRichStringCase(
                4,
                BuildRichString("0"),
                BuildRichString("4"))));

            cases.Add(new TestCaseData(new TestNumberToRichStringCase(
                386,
                BuildRichString("#", gRedFont, "#", gGreenFont, "#", gBlueFont),
                BuildRichString("3", gRedFont, "8", gGreenFont, "6", gBlueFont))));

            cases.Add(new TestCaseData(new TestNumberToRichStringCase(
                386,
                BuildRichString("0", gRedFont, "0", gGreenFont, "0", gBlueFont),
                BuildRichString("3", gRedFont, "8", gGreenFont, "6", gBlueFont))));

            cases.Add(new TestCaseData(new TestNumberToRichStringCase(
                95,
                BuildRichString("#", gRedFont),
                BuildRichString("95", gRedFont))));

            cases.Add(new TestCaseData(new TestNumberToRichStringCase(
                95,
                BuildRichString("0", gRedFont),
                BuildRichString("95", gRedFont))));

            cases.Add(new TestCaseData(new TestNumberToRichStringCase(
                3,
                BuildRichString("###", gRedFont),
                BuildRichString("  3", gRedFont))));

            cases.Add(new TestCaseData(new TestNumberToRichStringCase(
                3,
                BuildRichString("000", gRedFont),
                BuildRichString("003", gRedFont))));

            cases.Add(new TestCaseData(new TestNumberToRichStringCase(
                9475,
                BuildRichString("#", "#", gRedFont, "#"),
                BuildRichString("94", "7", gRedFont, "5"))));

            cases.Add(new TestCaseData(new TestNumberToRichStringCase(
                9475,
                BuildRichString("0", "0", gRedFont, "0"),
                BuildRichString("94", "7", gRedFont, "5"))));

            cases.Add(new TestCaseData(new TestNumberToRichStringCase(
                -281,
                BuildRichString("#", gRedFont),
                BuildRichString("-281", gRedFont))));

            cases.Add(new TestCaseData(new TestNumberToRichStringCase(
                -281,
                BuildRichString("0", gRedFont),
                BuildRichString("-281", gRedFont))));

            cases.Add(new TestCaseData(new TestNumberToRichStringCase(
                -305,
                BuildRichString("#", "#", gRedFont),
                BuildRichString("-30", "5", gRedFont))));

            cases.Add(new TestCaseData(new TestNumberToRichStringCase(
                -305,
                BuildRichString("0", "0", gRedFont),
                BuildRichString("-30", "5", gRedFont))));

            cases.Add(new TestCaseData(new TestNumberToRichStringCase(
                8191,
                BuildRichString("#", ",", gRedFont, "###"),
                BuildRichString("8,191"))));

            cases.Add(new TestCaseData(new TestNumberToRichStringCase(
                8192,
                BuildRichString("#", gGreenFont, ",", gRedFont, "###", gBlueFont),
                BuildRichString("8,", gGreenFont, "192", gBlueFont))));

            cases.Add(new TestCaseData(new TestNumberToRichStringCase(
                8193,
                BuildRichString("#", gGreenFont, "##,#", gBlueFont),
                BuildRichString("8,", gGreenFont, "193", gBlueFont))));

            cases.Add(new TestCaseData(new TestNumberToRichStringCase(
                1234567890,
                BuildRichString("#", gGreenFont, ",", gRedFont, "###", gBlueFont, ",", gRedFont, "###", gYellowFont),
                BuildRichString("1,234,", gGreenFont, "567,", gBlueFont, "890", gYellowFont))));

            cases.Add(new TestCaseData(new TestNumberToRichStringCase(
                5.29,
                BuildRichString("#.#", gGreenFont),
                BuildRichString("5.3", gGreenFont))));

            cases.Add(new TestCaseData(new TestNumberToRichStringCase(
                5.29,
                BuildRichString("0.0", gGreenFont),
                BuildRichString("5.3", gGreenFont))));

            cases.Add(new TestCaseData(new TestNumberToRichStringCase(
                6.6,
                BuildRichString("#.##", gGreenFont),
                BuildRichString("6.6 ", gGreenFont))));

            cases.Add(new TestCaseData(new TestNumberToRichStringCase(
                6.6,
                BuildRichString("0.00", gGreenFont),
                BuildRichString("6.60", gGreenFont))));

            cases.Add(new TestCaseData(new TestNumberToRichStringCase(
                0.35,
                BuildRichString("0", gGreenFont, ".", gBlueFont, "00", gYellowFont),
                BuildRichString("0", gGreenFont, ".", gBlueFont, "35", gYellowFont))));

            cases.Add(new TestCaseData(new TestNumberToRichStringCase(
                0.35,
                BuildRichString("#", gGreenFont, ".", gBlueFont, "##", gYellowFont),
                BuildRichString(" ", gGreenFont, ".", gBlueFont, "35", gYellowFont))));

            cases.Add(new TestCaseData(new TestNumberToRichStringCase(
                0.97,
                BuildRichString("0", gRedFont, "0", gYellowFont, "0", gGreenFont, ".", gBlueFont, "00", gYellowFont),
                BuildRichString("0", gRedFont, "0", gYellowFont, "0", gGreenFont, ".", gBlueFont, "97", gYellowFont))));

            cases.Add(new TestCaseData(new TestNumberToRichStringCase(
                0.97,
                BuildRichString("#", gRedFont, "#", gYellowFont, "#", gGreenFont, ".", gBlueFont, "##", gYellowFont),
                BuildRichString(" ", gRedFont, " ", gYellowFont, " ", gGreenFont, ".", gBlueFont, "97", gYellowFont))));

            cases.Add(new TestCaseData(new TestNumberToRichStringCase(
                1,
                BuildRichString("'XXXX'", gBlueFont, "x", gGreenFont, "#####", gRedFont),
                BuildRichString("XXXX", gBlueFont, " ", gGreenFont, "    1", gRedFont))));

            cases.Add(new TestCaseData(new TestNumberToRichStringCase(
                123456789,
                BuildRichString("'XXXX'", gBlueFont, "x", gGreenFont, "#####", gRedFont),
                BuildRichString("XXXX", gBlueFont, "4", gGreenFont, "56789", gRedFont))));

#if !CPLUSPLUS
            foreach (TestCaseData testCaseData in cases)
            {
                TestNumberToRichStringCase @case = (TestNumberToRichStringCase)testCaseData.Arguments[0];
                testCaseData.SetName(string.Format(
                    "TestNumberToRichString({0}, \"{1}\")",
                    @case.Value,
                    @case.Format.ToSystemString()));
            }
#endif

            return cases;
        }

        private static RichString BuildRichString(params object[] parts)
        {
            List<IChar> chars = new List<IChar>();

            string lastText = null;
            foreach (object part in parts)
            {
                string text = part as string;
                if (text != null)
                {
                    if (lastText != null)
                    {
                        foreach (char c in lastText)
                            chars.Add(new RichChar(c, new RunPr()));
                    }

                    lastText = text;
                    continue;
                }

                RunPr font = part as RunPr;
                if (font != null)
                {
                    if (lastText == null)
                        throw new ArgumentException();

                    foreach (char c in lastText)
                        chars.Add(new RichChar(c, font));

                    lastText = null;

                    continue;
                }

                throw new ArgumentException();
            }

            if (lastText != null)
            {
                foreach (char c in lastText)
                    chars.Add(new RichChar(c, new RunPr()));
            }

            return new RichString(chars);
        }

        [Test]
        public void TestNumberFormatterSequentialCalls()
        {
            RichString richFormat = RichString.CreateFromString("#");
            SystemStringAdapter plainTextFormat = SystemStringAdapter.Create("#");
            const double value = 123;

            Assert.That(NumberFormatter.NumberToString(richFormat, value, CultureInfo.CurrentCulture, NumberFormattingOptions.Default, RichStringBehaviour.Instance), Is.InstanceOf(typeof(RichString)));

            Assert.That(NumberFormatter.NumberToString(plainTextFormat, value, CultureInfo.CurrentCulture, NumberFormattingOptions.Default, SystemStringBehaviour.Instance), Is.InstanceOf(typeof(SystemStringAdapter)));

            Assert.That(NumberFormatter.NumberToString(richFormat, value, CultureInfo.CurrentCulture, NumberFormattingOptions.Default, RichStringBehaviour.Instance), Is.InstanceOf(typeof(RichString)));

            Assert.That(NumberFormatter.NumberToString(plainTextFormat, value, CultureInfo.CurrentCulture, NumberFormattingOptions.Default, SystemStringBehaviour.Instance), Is.InstanceOf(typeof(SystemStringAdapter)));
        }

        private static readonly RunPr gRedFont;
        private static readonly RunPr gGreenFont;
        private static readonly RunPr gBlueFont;
        private static readonly RunPr gYellowFont;

        public class TestNumberToRichStringCase
        {
            internal TestNumberToRichStringCase(double value, RichString format, RichString expectedResult)
            {
                Value = value;
                Format = format;
                ExpectedResult = expectedResult;
            }

            internal double Value { get; }
            internal RichString Format { get;  }
            internal RichString ExpectedResult { get; }
        }
    }
}
