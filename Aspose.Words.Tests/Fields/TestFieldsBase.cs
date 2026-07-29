// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 03/04/2015 by Roman Korchagin

using System.Collections.Generic;
using Aspose.Common;
using Aspose.Words.Fields;
using NUnit.Framework;

namespace Aspose.Words.Tests.Fields
{
    /// <summary>
    /// Represents a base class for all classes that test how fields work.
    /// </summary>
    [TestFixture]
    public abstract class TestFieldsBase
    {
        [TestFixtureSetUp]
        public void SetUp()
        {
            TestUtil.SetUpTests();
        }

        [SetUp]
        public void TestSetUp()
        {
            RandomUtil.Reset();

            // Parsing of numbers and dates is performed using the current culture and this is by design.
            // So we have to select the standard culture before we run a test in order to pass on all
            // development machines that can have different cultures selected.
            SystemPal.SaveCulture();
            SystemPal.SetStandardCulture();
        }

        [TearDown]
        public void TestShutDown()
        {
            SystemPal.RestoreCulture();
        }

        protected static Document OpenSaveOpenUpdateFields(string fileName)
        {
            return OpenSaveOpenUpdateFields(fileName, true);
        }

        /// <summary>
        /// Works exactly like Open but also clears then updates field results throughout the document.
        /// Used by the field evaluation engine tests.
        /// </summary>
        /// <param name="fileName">Can be relative to the TestData folder or an explicit path.</param>
        /// <param name="setGarbage"></param>
        protected static Document OpenSaveOpenUpdateFields(string fileName, bool setGarbage)
        {
            Document doc = TestUtil.OpenSaveOpen(fileName);

            IList<Field> fields = FieldExtractor.ExtractToCollection(doc);

            if (setGarbage)
            {
                foreach (Field field in fields)
                {
                    if (field.HasSeparator)
                        field.Result = "34twrsdgrw546565twetwet";   // Some garbage text that should never appear during update.
                }
            }

            doc.UpdateFields();

            return doc;
        }

        /// <summary>
        /// Opens a file with the given relative path in the TestData folder, loads the document, updates its fields
        /// without BIDI text support, saves and opens it performing a gold compare test.
        /// </summary>
        protected static void OpenUpdateFieldsSaveCheckGold(string fileName)
        {
            OpenUpdateFieldsSaveCheckGold(fileName, false);
        }

        /// <summary>
        /// Opens a file with the given relative path in the TestData folder, loads the document, updates its fields
        /// with optionally BIDI text support, saves and opens it performing a gold compare test.
        /// </summary>
        protected static void OpenUpdateFieldsSaveCheckGold(string fileName, bool isBidiSupported)
        {
            Document doc = TestUtil.Open(fileName);

            doc.FieldOptions.IsBidiTextSupportedOnUpdate = isBidiSupported;
            doc.UpdateFields();

            TestUtil.SaveCheckGold(doc, fileName);
        }

        protected static void AssertAreEqual(Field field1, Field field2)
        {
            Assert.That(field2.Start, Is.EqualTo(field1.Start));
            Assert.That(field2.Separator, Is.EqualTo(field1.Separator));
            Assert.That(field2.End, Is.EqualTo(field1.End));
            Assert.That(field2.Type, Is.EqualTo(field1.Type));
            Assert.That(field2.GetFieldCode(), Is.EqualTo(field1.GetFieldCode()));
            Assert.That(field2.Result, Is.EqualTo(field1.Result));
        }

        protected static string[] GetFieldResults(IEnumerable<Field> fields)
        {
            List<string> results = new List<string>();

            foreach (Field field in fields)
                results.Add(field.Result);

            return results.ToArray();
        }
    }
}
