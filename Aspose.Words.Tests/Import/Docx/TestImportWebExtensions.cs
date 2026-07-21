// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 15/10/2019 by Dmitry Sokolov

using Aspose.Common;
using Aspose.Images;
using Aspose.Words.Drawing.Core.Dml.Fills;
using Aspose.Words.WebExtensions;
using NUnit.Framework;

namespace Aspose.Words.Tests.Import.Docx
{
    /// <summary>
    /// Contains tests which checks how work import of task pane add-ins.
    /// </summary>
    [TestFixture]
    public class TestImportWebExtensions
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
        /// Related to WORDSNET-18681
        /// Checks reading of task pane add-in without bindings, properties and etc.
        /// </summary>
        [Test]
        public void Test18681ReadWebExtension()
        {
            Document doc = TestUtil.Open(@"ImportDocx\WebExtensions\Test18681ReadWebExtension.docx");

            // Prepare expected task pane.
            TaskPane expectedTaskPane = CreateTaskPane(6, 662.0d, "{EF15991F-8C78-4838-B0D8-FABD0105D596}");
            // Prepare expected web extension.
            PopulateExtensionReference(expectedTaskPane.WebExtension.Reference, "wa104099688", "1.3.0.0", "en-US");

            // Compare expected and actual value.
            Assert.That(doc.WebExtensionTaskPanes.Count, Is.EqualTo(1));
            AssertTaskPane(expectedTaskPane, doc.WebExtensionTaskPanes.First);
        }

        /// <summary>
        ///  Related to WORDSNET-18681
        ///  Checks case when task pane add-in parts placed in non-standard path.
        /// </summary>
        [Test]
        public void Test18681ReadWebExtensionDocLevel()
        {
            Document doc = TestUtil.Open(@"ImportDocx\WebExtensions\Test18681ReadWebExtensionDocLevel.docx");

            // Prepare expected task pane.
            TaskPane expectedTaskPane = CreateTaskPane(7, 350.0d, "{6301161A-C1FB-44AB-8365-DE52F792C01D}");

            // Prepare expected web extension. 
            PopulateExtensionReference(expectedTaskPane.WebExtension.Reference, "wa104381269", "1.0.0.4", "en-US");
            AddAlternateReference(expectedTaskPane.WebExtension, "wa104381269", "1.0.0.4", "wa104381269");

            // Compare expected and actual value.
            AssertTaskPane(expectedTaskPane, doc.WebExtensionTaskPanes.First);
            Assert.That(doc.WebExtensionTaskPanes.First.WebExtension.AlternateReferences.Count, Is.EqualTo(1));
        }

        /// <summary>
        /// Related to WORDSNET-18681
        /// Checks case when document contains two task pane add-ins.
        /// </summary>
        [Test]
        public void Test18681ReadTwoWebExtensions()
        {
            Document doc = TestUtil.Open(@"ImportDocx\WebExtensions\Test18681TwoTaskPanes.docx");

            TaskPaneCollection taskPanes = doc.WebExtensionTaskPanes;
            Assert.That(taskPanes.Count, Is.EqualTo(2));

            // Check first web extension.
            TaskPane expectedTaskPane = CreateTaskPane(6, 350.0d, "{9C7F2463-912E-4103-8978-2276AB15D4AC}");
            PopulateExtensionReference(expectedTaskPane.WebExtension.Reference, "wa102923726", "1.0.0.0", "en-US");
            AddAlternateReference(expectedTaskPane.WebExtension, "WA102923726", "1.0.0.0", "WA102923726");

            AssertTaskPane(expectedTaskPane, taskPanes.First);

            // Check second web extension.
            expectedTaskPane = CreateTaskPane(7, 350.0d, "{818247EE-A692-4795-AA5B-20ADCA44E6F0}");
            PopulateExtensionReference(expectedTaskPane.WebExtension.Reference, "wa104124372", "1.2.0.0", "en-US");
            AddAlternateReference(expectedTaskPane.WebExtension, "WA104124372", "1.2.0.0", "WA104124372");

            AssertTaskPane(expectedTaskPane, taskPanes.Last);
        }

        /// <summary>
        /// Related to WORDSNET-18681
        /// Checks reading of "snapshot" element.
        /// </summary>
        [Test]
        public void Test18681ReadSnapshot()
        {
            Document doc = TestUtil.Open(@"ImportDocx\WebExtensions\Test18681ReadSnapshot.docx");
            TaskPaneCollection taskPanes = doc.WebExtensionTaskPanes;
            DmlBlip blip = taskPanes.First.WebExtension.Snapshot;

            Assert.That(blip.ImageLink, Is.Null);
            Assert.That(blip.HasExtensions, Is.False);
            Assert.That(blip.Effects.Count, Is.EqualTo(0));
            Assert.That(blip.CompressionState, Is.EqualTo(DmlCompressionState.Email));
            ArrayUtil.IsArrayEqual(ImageUtil.GetNoImageBytes(), blip.EmbedImage);
        }

        /// <summary>
        /// Related to WORDSNET-18681
        /// Check reading of custom properties.
        /// </summary>
        [Test]
        public void Test18681ReadProperties()
        {
            Document doc = TestUtil.Open(@"ImportDocx\TestJira12228.docx");
            WebExtensionPropertyCollection actualProperties = doc.WebExtensionTaskPanes.First.WebExtension.Properties;
            WebExtensionPropertyCollection expectedProperties = CreatePropertyCollection(
                new WebExtensionProperty[] { new WebExtensionProperty("mailchimpCampaign", "mailchimpCampaign") });

            AssertProperties(expectedProperties, actualProperties);
        }




        /// <summary>
        /// Related to WORDSNET-18681
        /// Checks reading of "DockState" attribute.
        /// </summary>
        [Test]
        public void Test18681ReadDockState()
        {
            Document doc = TestUtil.Open(@"ImportDocx\WebExtensions\Test18681ReadDockState.docx");
            Assert.That(doc.WebExtensionTaskPanes.First.DockState, Is.EqualTo(TaskPaneDockState.Left));
        }

        /// <summary>
        /// Creates bindings collection and populates it with passed content.
        /// </summary>
        private WebExtensionBindingCollection CreateBindings(WebExtensionBinding[] bindings)
        {
            WebExtensionBindingCollection collection = new WebExtensionBindingCollection();

            foreach (WebExtensionBinding binding in bindings)
                collection.Add(binding);

            return collection;
        }

        /// <summary>
        /// Creates properties collection and populates it with passed content.
        /// </summary>
        private WebExtensionPropertyCollection CreatePropertyCollection(WebExtensionProperty[] properties)
        {
            WebExtensionPropertyCollection collection = new WebExtensionPropertyCollection();
            foreach(WebExtensionProperty property in properties)
                collection.Add(property);

            return collection;
        }

        /// <summary>
        /// Adds alternate reference to web extension object.
        /// </summary>
        private void AddAlternateReference(WebExtension extension, string id, string version, string store)
        {
            WebExtensionReference reference = new WebExtensionReference();
            PopulateExtensionReference(reference, id, version, store);

            extension.AlternateReferences.Add(reference);
        }

        /// <summary>
        /// Checks equality of task pane add-ins.
        /// </summary>
        private void AssertTaskPane(TaskPane expectedTaskPane, TaskPane actualTaskPane)
        {
            Assert.That(actualTaskPane.Row, Is.EqualTo(expectedTaskPane.Row));
            Assert.That(actualTaskPane.IsLocked, Is.EqualTo(expectedTaskPane.IsLocked));
            Assert.That(actualTaskPane.IsVisible, Is.EqualTo(expectedTaskPane.IsVisible));
            Assert.That(actualTaskPane.DockState, Is.EqualTo(expectedTaskPane.DockState));
            Assert.That(actualTaskPane.Width, Is.EqualTo(expectedTaskPane.Width).Within(0.000001));

            AssertWebExtension(expectedTaskPane.WebExtension, actualTaskPane.WebExtension);
        }

        /// <summary>
        /// Checks equality of web extension objects.
        /// </summary>
        private void AssertWebExtension(WebExtension expectedWebExtension, WebExtension actualWebExtension)
        {
            Assert.That(actualWebExtension.Id, Is.EqualTo(expectedWebExtension.Id));
            Assert.That(actualWebExtension.IsFrozen, Is.EqualTo(expectedWebExtension.IsFrozen));

            AssertBindings(expectedWebExtension.Bindings, actualWebExtension.Bindings);
            AssertReference(expectedWebExtension.Reference, actualWebExtension.Reference);
            AssertProperties(expectedWebExtension.Properties, actualWebExtension.Properties);
            AssertAlternateReferences(expectedWebExtension.AlternateReferences, 
                actualWebExtension.AlternateReferences);
        }

        /// <summary>
        /// Checks equality of reference objects.
        /// </summary>
        private void AssertReference(WebExtensionReference expectedReference, WebExtensionReference actualReference)
        {
            Assert.That(actualReference.Id, Is.EqualTo(expectedReference.Id));
            Assert.That(actualReference.Store, Is.EqualTo(expectedReference.Store));
            Assert.That(actualReference.Version, Is.EqualTo(expectedReference.Version));
            Assert.That(actualReference.StoreType, Is.EqualTo(expectedReference.StoreType));
        }

        /// <summary>
        /// Checks binding collections equality.
        /// </summary>
        private void AssertBindings(WebExtensionBindingCollection expectedBindings,
            WebExtensionBindingCollection actualBindings)
        {
            Assert.That(actualBindings.Count, Is.EqualTo(expectedBindings.Count));

            for (int i = 0; i < expectedBindings.Count; ++i)
                AssertBinding(expectedBindings[i], actualBindings[i]);
        }

        /// <summary>
        /// Checks bindings equality.
        /// </summary>
        private void AssertBinding(WebExtensionBinding expectedBinding, WebExtensionBinding actualBinding)
        {
            Assert.That(actualBinding.Id, Is.EqualTo(expectedBinding.Id));
            Assert.That(actualBinding.AppRef, Is.EqualTo(expectedBinding.AppRef));
            Assert.That(actualBinding.BindingType, Is.EqualTo(expectedBinding.BindingType));
        }

        /// <summary>
        /// Checks property collections equality.
        /// </summary>
        private void AssertProperties(WebExtensionPropertyCollection expectedProperties,
            WebExtensionPropertyCollection actualProperties)
        {
            Assert.That(actualProperties.Count, Is.EqualTo(expectedProperties.Count));

            for(int i = 0; i < expectedProperties.Count; ++i)
                AssertProperty(expectedProperties[i], actualProperties[i]);
        }

        /// <summary>
        /// Checks properties equality.
        /// </summary>
        private void AssertProperty(WebExtensionProperty expectedProperty, WebExtensionProperty actualProperty)
        {
            Assert.That(actualProperty.Name, Is.EqualTo(expectedProperty.Name));
            Assert.That(actualProperty.Value, Is.EqualTo(expectedProperty.Value));
        }

        /// <summary>
        /// Checks alternate references equality.
        /// </summary>
        private void AssertAlternateReferences(WebExtensionReferenceCollection expectedReferences,
            WebExtensionReferenceCollection actualReferences)
        {
            Assert.That(actualReferences.Count, Is.EqualTo(expectedReferences.Count));

            for (int i = 0; i < expectedReferences.Count; ++i)
                AssertReference(expectedReferences[i], actualReferences[i]);
        }

        /// <summary>
        /// Creates a task pane with specified parameters.
        /// </summary>
        private TaskPane CreateTaskPane(int row, double width, string extensionId)
        {
            TaskPane expectedTaskPane = new TaskPane();
            expectedTaskPane.IsVisible = true;
            expectedTaskPane.IsLocked = false;

            expectedTaskPane.Row = row;
            expectedTaskPane.Width = width;

            expectedTaskPane.WebExtension.Id = extensionId;
            expectedTaskPane.WebExtension.IsFrozen = false;

            return expectedTaskPane;
        }

        /// <summary>
        /// Fills reference with specified parameters.
        /// </summary>
        private WebExtensionReference PopulateExtensionReference(WebExtensionReference reference,
            string id, string version, string store)
        {
            return PopulateExtensionReference(reference, id, version, store, WebExtensionStoreType.OMEX);
        }

        /// <summary>
        /// Fills reference with specified parameters.
        /// </summary>
        private WebExtensionReference PopulateExtensionReference(WebExtensionReference reference,
            string id, string store, WebExtensionStoreType storeType)
        {
            return PopulateExtensionReference(reference, id, "1.0.0.0", store, storeType);
        }

        /// <summary>
        /// Fills reference with specified parameters.
        /// </summary>
        private WebExtensionReference PopulateExtensionReference(WebExtensionReference reference,
            string id, string version, string store, WebExtensionStoreType storeType)
        {
            reference.Id = id;
            reference.Store = store;
            reference.Version = version;
            reference.StoreType = storeType;

            return reference;
        }
    }
}
