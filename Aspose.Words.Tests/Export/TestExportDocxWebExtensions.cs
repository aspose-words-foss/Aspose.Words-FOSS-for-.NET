// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 15/10/2019 by Dmitry Sokolov

using System.Xml;
using Aspose.Common;
using Aspose.Images;
using Aspose.Words.Drawing.Core.Dml.Fills;
using Aspose.Words.Nrx;
using Aspose.Words.Saving;
using Aspose.Words.Tests.Export.Docx;
using Aspose.Words.WebExtensions;
using NUnit.Framework;

namespace Aspose.Words.Tests.Export
{
    /// <summary>
    /// Implements tests which checks export of task pane add-ins.
    /// </summary>
    [TestFixture]
    public class TestExportDocxWebExtensions
    {
        [TestFixtureSetUp]
        public void SetUp()
        {
            TestUtil.SetUpTests();
        }

        [SetUp]
        public void Init()
        {
            RandomUtil.Reset();
        }

        /// <summary>
        /// WORDSNET-18681 Support for adding dynamic WebExtension element nodes in Word Document.
        /// Appropriate public interface added.
        /// </summary>
        [Test]
        public void Test18681()
        {
            TaskPane taskPane = new TaskPane();
            taskPane.Width = 350;
            taskPane.Row = 4;

            WebExtension webExension = taskPane.WebExtension;
            webExension.Id = "{90416049-4341-4B33-9824-972826A82B7D}";
            webExension.Reference.Id = "e504fb41-a92a-4526-b101-542f357b7acb";
            webExension.Reference.StoreType = WebExtensionStoreType.FileSystem;
            webExension.Reference.Store = @"\\W10-JAVADEV2001\manifest";
            webExension.Reference.Version = "1.0.0.0";

            webExension.Properties.Add(new WebExtensionProperty("Office.AutoShowTaskpaneWithDocument", "true"));

            Document doc = new Document();
            doc.WebExtensionTaskPanes.Add(taskPane);

            doc = TestUtil.SaveOpen(doc, @"ExportDocxWebExtensions\Test18681", UnifiedScenario.Docx2Docx);
            Assert.That(doc.WebExtensionTaskPanes.Count, Is.EqualTo(1));
        }

        /// <summary>
        /// WORDSNET-10593 Add support Web Extensions into the model.
        /// Appropriate public interface added in scope of the WORDSNET-18681
        /// </summary>
        [Test]
        public void Test10593()
        {
            Document doc = TestUtil.Open(@"ExportDocx\Test10593.docx");

            TaskPane taskPane = new TaskPane();
            taskPane.IsVisible = true;
            taskPane.Width = 350;
            taskPane.Row = 3;

            WebExtension webExension = taskPane.WebExtension;
            webExension.Id = "{8FB3EB6F-F75E-4E2C-B57F-27DFDC0CA5FE}";
            webExension.Reference.Id = "77086593-cfc7-4abb-8711-085bb2ed3995";
            webExension.Reference.StoreType = WebExtensionStoreType.FileSystem;
            webExension.Reference.Store = @"\\SHAREPOINT\Apps";
            webExension.Reference.Version = "1.0.0.12";

            doc.WebExtensionTaskPanes.Add(taskPane);

            doc = TestUtil.SaveOpen(doc, @"ExportDocxWebExtensions\Test10593", UnifiedScenario.Docx2Docx);
            Assert.That(doc.WebExtensionTaskPanes.Count, Is.EqualTo(1));
        }

        /// <summary>
        /// WORDSNET-18634 Provide support for adding dynamic WebExtension element nodes in Word Document.
        /// Appropriate public interface added in scope of the WORDSNET-18681
        /// </summary>
        [Test]
        public void Test18634()
        {
            TaskPane taskPane = new TaskPane();
            taskPane.IsVisible = true;
            taskPane.Width = 350;

            WebExtension webExension = taskPane.WebExtension;
            webExension.Id = "{52811C31-4593-43B8-A697-EB873422D156}";
            webExension.Reference.Id = "af8fa5ba-4010-4bcc-9e03-a91ddadf6dd3";
            webExension.Reference.StoreType = WebExtensionStoreType.ExCatalog;
            webExension.Reference.Store = @"EXCatalog";
            webExension.Reference.Version = "1.0.0.0";

            Document doc = new Document();
            doc.WebExtensionTaskPanes.Add(taskPane);

            doc = TestUtil.SaveOpen(doc, @"ExportDocxWebExtensions\Test18634", UnifiedScenario.Docx2Docx);
            Assert.That(doc.WebExtensionTaskPanes.Count, Is.EqualTo(1));
        }

        /// <summary>
        /// Related to WORDSNET-18681
        /// Checks export of extension without bindings, properties and etc.
        /// </summary>
        [Test]
        public void Test18681ExportWiki()
        {
            Document doc = CreateDocumentWithTaskPane("{EF15991F-8C78-4838-B0D8-FABD0105D596}", "wa104099688", "1.3.0.0");
            TestUtil.SaveOpen(doc, @"ExportDocxWebExtensions\Test18681ExportWiki", UnifiedScenario.Docx2Docx);
        }

        /// <summary>
        /// Related to WORDSNET-18681
        /// Checks export of "snapshot" element.
        /// </summary>
        [Test]
        public void Test18681ExportSnapShot()
        {
            Document doc = CreateDocumentWithTaskPane("{EF15991F-8C78-4838-B0D8-FABD0105D596}", "wa104099688", "1.3.0.0");

            DmlBlip blip = new DmlBlip();
            blip.EmbedImage = ImageUtil.GetNoImageBytes();
            doc.WebExtensionTaskPanes.First.WebExtension.Snapshot = blip;

            TestUtil.SaveOpen(doc, @"ExportDocxWebExtensions\Test18681ExportSnapShot", UnifiedScenario.Docx2Docx);
        }

        /// <summary>
        /// Related to WORDSNET-18681
        /// Checks export of custom properties.
        /// </summary>
        [Test]
        public void Test18681PropertiesExport()
        {
            Document doc = TestUtil.Open(@"ImportDocx\TestJira12228.docx");

            XmlDocument xmlDoc = DocxExportContext.SaveAndGetXmlDocument(doc, new OoxmlSaveOptions(), FirstWebExtensionPart);

            XmlNodeList propertyNodes = xmlDoc.GetElementsByTagName("we:property");
            Assert.That(propertyNodes.Count, Is.EqualTo(1));
            Assert.That(propertyNodes[0].Attributes["name"].Value, Is.EqualTo("mailchimpCampaign"));
            Assert.That(propertyNodes[0].Attributes["value"].Value, Is.EqualTo("mailchimpCampaign"));

            XmlNode referenceNode = xmlDoc.GetElementsByTagName("we:reference")[0];
            Assert.That(referenceNode.Attributes["storeType"].Value, Is.EqualTo("omex"));
        }



        /// <summary>
        /// Related to WORDSNET-18681
        /// Checks export to the strict DOCX.
        /// </summary>
        [Test]
        public void Test18681StrictExport()
        {
            Document doc = TestUtil.Open(@"ImportDocx\WebExtensions\Test18681ReadDockState.docx");
            OoxmlSaveOptions so = new OoxmlSaveOptions
            {
                Compliance = OoxmlCompliance.Iso29500_2008_Strict,
                SaveFormat = SaveFormat.Docx
            };
            so.SetTestMode();

            TestUtil.SaveOpen(doc, @"ExportDocxWebExtensions\Test18681StrictExport", UnifiedScenario.Docx2Docx, so);
        }

        /// <summary>
        /// Related to WORDSNET-18681
        /// Checks export to a non-OOXML target format.
        /// </summary>
        /// <remarks>
        /// RK FOSS: the warning is gated on <c>!IsOoxmlFormat</c>, so any non-OOXML save format exercises
        /// the same code path. SaveFormat.Doc was removed in the FOSS reduction; SaveFormat.Markdown still
        /// triggers the WebExtensionValidator warning and keeps the regression coverage intact.
        /// </remarks>
        [Test]
        public void Test18681DocExport()
        {
            Document doc = TestUtil.Open(@"ImportDocx\WebExtensions\Test18681ReadDockState.docx");
            TestWarningCallback wc = new TestWarningCallback();
            doc.WarningCallback = wc;

            TestUtil.ExecuteValidator(doc, SaveFormat.Markdown);

            // Expected warning due to task pane add-in only supported by the OOXML.
            Assert.That(wc.Contains(WarningSource.Validator, WarningType.DataLossCategory,
                WarningStrings.TaskPaneAddinsWillBeSkippedOnSaving), Is.True);
            // In the model web extension still exists.
            Assert.That(doc.WebExtensionTaskPanes.Count, Is.EqualTo(1));
        }

        /// <summary>
        /// Related to WORDSNET-18681
        /// Checks that just created web extension from the model does not produce corrupted document.
        /// </summary>
        [Test]
        public void Test18681NotInitializedWebExtensionExport()
        {
            Document doc = new Document();
            TaskPane taskPane = new TaskPane();
            doc.WebExtensionTaskPanes.Add(taskPane);

            XmlDocument xmlDoc = DocxExportContext.SaveAndGetXmlDocument(doc, new OoxmlSaveOptions(), FirstWebExtensionPart);

            XmlNode reference = xmlDoc.GetElementsByTagName("we:reference")[0];
            Assert.That(reference.Attributes["id"].Value, Is.Empty);
            Assert.That(reference.Attributes["store"].Value, Is.Empty);
            Assert.That(reference.Attributes["version"].Value, Is.Empty);
            Assert.That(reference.Attributes["storeType"].Value, Is.EqualTo("spcatalog"));

            XmlNode extension = xmlDoc.GetElementsByTagName("we:webextension")[0];
            Assert.That(extension.Attributes["id"].Value, Is.Empty);
        }

        /// <summary>
        /// Related to WORDSNET-18681
        /// Checks that document can be opened after removing of the task pane add-ins.
        /// </summary>
        [Test]
        public void Test18681RemoveWebExtensions()
        {
            Document doc = CreateDocumentWithTaskPane("{EF15991F-8C78-4838-B0D8-FABD0105D596}", "wa104099688", "1.3.0.0");

            doc = TestUtil.SaveOpen(doc, @"ExportDocxWebExtensions\Test18681ExportWiki", UnifiedScenario.Docx2DocxNoGold);
            Assert.That(doc.WebExtensionTaskPanes.Count, Is.EqualTo(1));

            doc.WebExtensionTaskPanes.Clear();
            Assert.That(doc.WebExtensionTaskPanes.Count, Is.EqualTo(0));

            TestUtil.SaveOpen(doc, @"ExportDocxWebExtensions\Test18681RemoveWebExtensions", UnifiedScenario.Docx2Docx);
        }

        /// <summary>
        /// Related to WORDSNET-18681
        /// Checks export of two task pane add-ins.
        /// </summary>
        [Test]
        public void Test18681TwoWebExtensionsExport()
        {
            Document doc = TestUtil.OpenSaveOpen(@"ImportDocx\WebExtensions\Test18681TwoTaskPanes", UnifiedScenario.Docx2Docx);
            Assert.That(doc.WebExtensionTaskPanes.Count, Is.EqualTo(2));
        }

        /// <summary>
        /// Checks content of the binding node.
        /// </summary>
        private static void AssertBindingMarkup(XmlNode bindingNode, WebExtensionBinding expectedBinding)
        {
            Assert.That(bindingNode.Attributes["id"].Value, Is.EqualTo(expectedBinding.Id));
            Assert.That(bindingNode.Attributes["appref"].Value, Is.EqualTo(expectedBinding.AppRef));
            Assert.That(bindingNode.Attributes["type"].Value, Is.EqualTo(DocxEnum.WebExtensionBindingTypeToDocx(expectedBinding.BindingType)));

            Assert.That(bindingNode.Attributes.Count, Is.EqualTo(3));
        }

        /// <summary>
        /// Creates document and inserts task pane add-in.
        /// </summary>
        private static Document CreateDocumentWithTaskPane(string webExtensionId, string reference, string version)
        {
            Document doc = new Document();

            TaskPane taskPane = new TaskPane();
            taskPane.IsVisible = true;
            taskPane.Width = 350d;
            taskPane.Row = 7;

            taskPane.WebExtension.Id = webExtensionId;
            FillReference(taskPane.WebExtension.Reference, reference, version);

            doc.WebExtensionTaskPanes.Add(taskPane);

            return doc;
        }

        /// <summary>
        /// Fills passed reference with specified parameters.
        /// </summary>
        private static void FillReference(WebExtensionReference reference, string id, string version)
        {
            reference.Id = id;
            reference.Version = version;

            reference.Store = "en-US";
            reference.StoreType = WebExtensionStoreType.OMEX;
        }

        /// <summary>
        /// Path to "webextension" part with index two.
        /// </summary>
        private string SecondWebExtensionPart { get { return string.Format(mWebExtensionPartName, 2); } }

        /// <summary>
        /// Path to "webextension" part with index one.
        /// </summary>
        private string FirstWebExtensionPart { get { return string.Format(mWebExtensionPartName, 1); } }

        /// <summary>
        /// Path pattern to "webextension" part.
        /// </summary>
        private static readonly string mWebExtensionPartName = @"word/webextensions/webextension{0}.xml";
    }
}
