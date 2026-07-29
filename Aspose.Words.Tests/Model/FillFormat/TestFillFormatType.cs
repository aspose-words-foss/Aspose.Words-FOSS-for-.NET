// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 10/12/2019 by Ilya Navrotskiy

using System;
using Aspose.Words.Drawing;
using NUnit.Framework;

namespace Aspose.Words.Tests.Model.FillFormat
{
    /// <summary>
    /// The class for testing Type property of the FillFormat object.
    /// </summary>
    [TestFixture]
    public class TestFillFormatType : TestFillFormatBase
    {
        /// <summary>
        /// Tests Type of the various DML fills.
        /// </summary>
        [TestCase("NoFill")]
        [TestCase("TextNoFill")]
        [TestCase("OneColorGradient")]
        [TestCase("Patterned")]
        [TestCase("PresetGradient")]
        [TestCase("PresetTextured")]
        [TestCase("Solid")]
        [TestCase("TextOneColorGradient")]
        [TestCase("TextPresetGradient")]
        [TestCase("TextSolid")]
        [TestCase("TextTwoColorGradient")]
        [TestCase("TwoColorGradient")]
        [TestCase("UserPicture")]
        [TestCase("UserTextured")]
        public void TestFillType(string testName)
        {
            CheckFillType(testName, ShapeMarkupLanguage.Dml);
            CheckFillType(testName, ShapeMarkupLanguage.Vml);
        }

        /// <summary>
        /// Checks FillFormatType for a specified test file considering markup language.
        /// </summary>
        private static void CheckFillType(string testName, ShapeMarkupLanguage markupLanguage)
        {
            Document doc = Open(testName, markupLanguage);
            Fill fill = GetFill(doc);

            FillType expectedType = GetExpectedFillType(testName, markupLanguage);
            Assert.That(fill.FillType, Is.EqualTo(expectedType), string.Format("MarkupLanguage:{0}", markupLanguage));
        }

        /// <summary>
        /// Returns expected <see cref="FillType"/> for a specified test name and markup language.
        /// </summary>
        private static FillType GetExpectedFillType(string testFileName, ShapeMarkupLanguage markupLanguage)
        {
            switch (testFileName)
            {
                case "NoFill":
                case "TextNoFill":
                case "Solid":
                case "TextSolid":
                    return FillType.Solid;
                case "TextPresetGradient":
                case "TextTwoColorGradient":
                case "TextOneColorGradient":
                    return (markupLanguage == ShapeMarkupLanguage.Dml) ? FillType.Gradient : FillType.Solid;
                case "OneColorGradient":
                case "PresetGradient":
                case "TwoColorGradient":
                    return FillType.Gradient;
                case "Patterned":
                    return FillType.Patterned;
                case "PresetTextured":
                case "UserTextured":
                    return FillType.Textured;
                case "UserPicture":
                    return FillType.Picture;
                default:
                    throw new InvalidOperationException(string.Format(
                        "Cannot determine expected FillType for '{0}' in '{1}'", testFileName, markupLanguage));
            }
        }
    }
}
