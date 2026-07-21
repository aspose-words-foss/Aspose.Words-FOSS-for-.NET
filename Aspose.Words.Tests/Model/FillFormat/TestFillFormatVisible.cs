// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 08/12/2019 by Ilya Navrotskiy

using Aspose.Words.Drawing;
using NUnit.Framework;

namespace Aspose.Words.Tests.Model.FillFormat
{
    /// <summary>
    /// The class for testing Visible property of the FillFormat object.
    /// </summary>
    [TestFixture]
    public class TestFillFormatVisible : TestFillFormatBase
    {
        /// <summary>
        /// Tests Visible of the various DML fills.
        /// </summary>
        [TestCase("NoFill", false)]
        [TestCase("TextNoFill", false)]
        [TestCase("OneColorGradient", true)]
        [TestCase("Patterned", true)]
        [TestCase("PresetGradient", true)]
        [TestCase("PresetTextured", true)]
        [TestCase("Solid", true)]
        [TestCase("TextOneColorGradient", true)]
        [TestCase("TextPresetGradient", true)]
        [TestCase("TextSolid", true)]
        [TestCase("TextTwoColorGradient", true)]
        [TestCase("TwoColorGradient", true)]
        [TestCase("UserPicture", true)]
        [TestCase("UserTextured", true)]
        public void TestFillVisibleDml(string testName, bool expectedOriginalValue)
        {
            TestFillVisible(testName, ShapeMarkupLanguage.Dml, expectedOriginalValue);
        }

        /// <summary>
        /// Tests Visible of the VML shape fills.
        /// </summary>
        [TestCase("NoFill", false)]
        [TestCase("OneColorGradient", true)]
        [TestCase("Patterned", true)]
        [TestCase("PresetGradient", true)]
        [TestCase("PresetTextured", true)]
        [TestCase("Solid", true)]
        [TestCase("TwoColorGradient", true)]
        [TestCase("UserPicture", true)]
        [TestCase("UserTextured", true)]
        public void TestShapeFillVisibleVml(string testName, bool expectedOriginalValue)
        {
            TestFillVisible(testName, ShapeMarkupLanguage.Vml, expectedOriginalValue);
        }

        /// <summary>
        /// Tests Visible of the first fillable object in a document at a specified path.
        /// </summary>
        private static void TestFillVisible(string testFileNameWithoutExtension, ShapeMarkupLanguage markupLanguage,
            bool expectedOriginalValue)
        {
            bool desiredValue = !expectedOriginalValue;
            TestFillVisible(testFileNameWithoutExtension, markupLanguage, expectedOriginalValue, desiredValue, desiredValue);
        }

        /// <summary>
        /// Tests Visible of the first fillable object in a document at a specified path.
        /// </summary>
        private static void TestFillVisible(string testFileNameWithoutExtension, ShapeMarkupLanguage markupLanguage,
            bool expectedOriginalValue, bool desiredValue, bool actualValue)
        {
            Document doc = Open(testFileNameWithoutExtension, markupLanguage);
            TestFillVisible(doc, testFileNameWithoutExtension, markupLanguage,
                expectedOriginalValue, desiredValue, actualValue);
        }

        /// <summary>
        /// Tests Visible of the first fillable object in a specified document.
        /// </summary>
        private static void TestFillVisible(Document doc, string testFileNameWithoutExtension,
            ShapeMarkupLanguage markupLanguage, bool expectedOriginalValue, bool desiredValue, bool actualValue)
        {
            Fill fill = GetFill(doc);
            Assert.That(fill.Visible, Is.EqualTo(expectedOriginalValue));

            // Set desired Value.
            fill.Visible = desiredValue;
            Assert.That(fill.Visible, Is.EqualTo(desiredValue));

            string ext = MarkupLanguageToExtension(markupLanguage);
            string outFileName = string.Format("FillFormat\\{0}_Visible_{1}{2}",
                testFileNameWithoutExtension, fill.Visible, ext);

            // Roundtrip the document.
            doc = TestUtil.SaveOpen(doc, outFileName, CreateSaveOptions(markupLanguage), false);

            fill = GetFill(doc);
            Assert.That(fill.Visible, Is.EqualTo(actualValue));
        }
    }
}
