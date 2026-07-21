// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 09/12/2019 by Ilya Navrotskiy

using System;
using Aspose.Words.Drawing;
using Aspose.Words.Drawing.Core.Dml.Fills;
using NUnit.Framework;

namespace Aspose.Words.Tests.Model.FillFormat
{
    /// <summary>
    /// The class for testing RotateWithObject property of the FillFormat object.
    /// </summary>
    [TestFixture]
    public class TestFillFormatRotateWithObject : TestFillFormatBase
    {
        /// <summary>
        /// Tests RotateWithObject getter.
        /// </summary>
        /// <remarks>This is the test for fills which getter does not throw in both: VML and DML.</remarks>
        [TestCase("Solid", false)]
        [TestCase("PresetTextured", true)]
        [TestCase("PresetGradient", true)]
        [TestCase("Patterned", false)]
        [TestCase("OneColorGradient", true)]
        [TestCase("TwoColorGradient", true)]
        [TestCase("UserTextured", true)]
        [TestCase("UserPicture", true)]
        [TestCase("NoFill", false)]
        public void TestRotateGetter(string testName, bool expectedValue)
        {
            // DML
            Document doc = Open(testName, ShapeMarkupLanguage.Dml);
            Fill fill = GetFill(doc);
            Assert.That(fill.RotateWithObject, Is.EqualTo(expectedValue));

            // VML
            doc = Open(testName, ShapeMarkupLanguage.Vml);
            fill = GetFill(doc);
            Assert.That(fill.RotateWithObject, Is.EqualTo(expectedValue));
        }

        /// <summary>
        /// Tests RotateWithObject getter throws an exception.
        /// </summary>
        /// <remarks>
        /// This is the test for fills which getter throws in both: Vml and Dml.
        /// Note, to check exception is thrown properly, we should perform tests
        /// for Dml and Vml separately each in its own test case.
        /// </remarks>
        [TestCase("TextSolid", ShapeMarkupLanguage.Dml)]
        [TestCase("TextSolid", ShapeMarkupLanguage.Vml)]
        [TestCase("TextOneColorGradient", ShapeMarkupLanguage.Dml)]
        [TestCase("TextOneColorGradient", ShapeMarkupLanguage.Vml)]
        [TestCase("TextTwoColorGradient", ShapeMarkupLanguage.Dml)]
        [TestCase("TextTwoColorGradient", ShapeMarkupLanguage.Vml)]
        [TestCase("TextPresetGradient", ShapeMarkupLanguage.Dml)]
        [TestCase("TextPresetGradient", ShapeMarkupLanguage.Vml)]
        [TestCase("TextNoFill", ShapeMarkupLanguage.Dml)]
        [TestCase("TextNoFill", ShapeMarkupLanguage.Vml)]
        [ExpectedException(typeof(InvalidOperationException), ExpectedMessage = DmlFill.MsgInvalidAction)]
        public void TestGetterException(string testName, ShapeMarkupLanguage markupLanguage)
        {
            Document doc = Open(testName, markupLanguage);
            Fill fill = GetFill(doc);

            // Getter throws.
            Assert.That(fill.RotateWithObject, Is.False);
        }

        /// <summary>
        /// Tests RotateWithObject setter throws an exception.
        /// </summary>
        /// <remarks> The same as above, but checks setter. </remarks>
        [TestCase("TextSolid", ShapeMarkupLanguage.Dml)]
        [TestCase("TextSolid", ShapeMarkupLanguage.Vml)]
        [TestCase("TextOneColorGradient", ShapeMarkupLanguage.Dml)]
        [TestCase("TextOneColorGradient", ShapeMarkupLanguage.Vml)]
        [TestCase("TextTwoColorGradient", ShapeMarkupLanguage.Vml)]
        [TestCase("TextPresetGradient", ShapeMarkupLanguage.Vml)]
        [TestCase("TextNoFill", ShapeMarkupLanguage.Dml)]
        [TestCase("TextNoFill", ShapeMarkupLanguage.Vml)]
        [ExpectedException(typeof(InvalidOperationException), ExpectedMessage = DmlFill.MsgInvalidAction)]
        public void TestSetterExceptionC(string testName, ShapeMarkupLanguage markupLanguage)
        {
            Document doc = Open(testName, markupLanguage);
            Fill fill = GetFill(doc);

            // Setter throws.
            fill.RotateWithObject = false;
        }

        /// <summary>
        /// Tests RotateWithObject.
        /// </summary>
        /// <remarks>
        /// This is the test for fills that does not throw either in VML or DML in both: getter and setter.
        /// </remarks>
        [TestCase("PresetTextured")]
        [TestCase("PresetGradient")]
        [TestCase("OneColorGradient")]
        [TestCase("TwoColorGradient")]
        [TestCase("UserTextured")]
        [TestCase("UserPicture")]
        public void TestRotateA(string testName)
        {
            // DML
            TestFillRotateWithObject(testName, ShapeMarkupLanguage.Dml, true, false);
            // VML
            TestFillRotateWithObject(testName, ShapeMarkupLanguage.Vml, true, false);
        }

        /// <summary>
        /// Tests RotateWithObject.
        /// </summary>
        /// <remarks> This is the test for fills that does not throw only in VML in both: getter and setter. </remarks>
        [TestCase("Solid")]
        [TestCase("Patterned")]
        [TestCase("NoFill")]
        public void TestRotateB(string testName)
        {
            TestFillRotateWithObject(testName, ShapeMarkupLanguage.Vml, false, true);
        }

        /// <summary>
        /// Tests RotateWithObject setter throws an exception.
        /// </summary>
        /// <remarks> This is the test for the fills that throws only in setter (they are Ok in getter). </remarks>
        [TestCase("Solid")]
        [TestCase("Patterned")]
        [TestCase("NoFill")]
        [ExpectedException(typeof(InvalidOperationException), ExpectedMessage = DmlFill.MsgValueOutOfRange)]
        public void TestSetterExceptionA(string testName)
        {
            TestFillRotateWithObject(testName, ShapeMarkupLanguage.Dml, false, true);
        }

        /// <summary>
        /// Tests RotateWithObject setter throws an exception.
        /// The same as above, but another exception.
        /// </summary>
        /// <remarks> This is the test for the fills that throws only in setter (they are Ok in getter). </remarks>
        [TestCase("TextTwoColorGradient")]
        [TestCase("TextPresetGradient")]
        [ExpectedException(typeof(InvalidOperationException), ExpectedMessage = DmlFill.MsgInvalidAction)]
        public void TestSetterExceptionB(string testName)
        {
            TestFillRotateWithObject(testName, ShapeMarkupLanguage.Dml, false, true);
        }

        /// <summary>
        /// Tests RotateWithObject of the first fillable object in a document at a specified path.
        /// </summary>
        private static void TestFillRotateWithObject(string testFileNameWithoutExtension,
            ShapeMarkupLanguage markupLanguage, bool originalValue, bool desiredValue)
        {
            TestFillRotateWithObject(testFileNameWithoutExtension,
                markupLanguage, originalValue, desiredValue, desiredValue);
        }

        /// <summary>
        /// Tests RotateWithObject of the first fillable object in a document at a specified path.
        /// </summary>
        private static void TestFillRotateWithObject(string testFileNameWithoutExtension,
            ShapeMarkupLanguage markupLanguage, bool originalValue, bool desiredValue, bool actualValue)
        {
            Document doc = Open(testFileNameWithoutExtension, markupLanguage);
            TestFillRotateWithObject(doc, testFileNameWithoutExtension,
                markupLanguage, originalValue, desiredValue, actualValue);
        }

        /// <summary>
        /// Tests RotateWithObject of the first fillable object in a specified document.
        /// </summary>
        private static void TestFillRotateWithObject(Document doc, string testFileNameWithoutExtension,
            ShapeMarkupLanguage markupLanguage, bool originalValue, bool desiredValue, bool actualValue)
        {
            Fill fillFormat = GetFill(doc);
            Assert.That(fillFormat.RotateWithObject, Is.EqualTo(originalValue));

            // Set desired value.
            fillFormat.RotateWithObject = desiredValue;
            Assert.That(fillFormat.RotateWithObject, Is.EqualTo(actualValue));

            string ext = MarkupLanguageToExtension(markupLanguage);
            string outFileName = string.Format("FillFormat\\{0}_RotateWithObject_{1}{2}",
                testFileNameWithoutExtension, desiredValue, ext);

            // Roundtrip the document.
            doc = TestUtil.SaveOpen(doc, outFileName, CreateSaveOptions(markupLanguage), false);

            fillFormat = GetFill(doc);
            Assert.That(fillFormat.RotateWithObject, Is.EqualTo(actualValue));
        }
    }
}
