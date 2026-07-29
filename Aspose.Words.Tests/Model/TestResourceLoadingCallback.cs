// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 18/09/2012 by Andrey Noskov
using System;
using System.IO;
using Aspose.Common;
using Aspose.Words.Drawing;
using Aspose.Words.Loading;
using NUnit.Framework;

namespace Aspose.Words.Tests.Model
{
    /// <summary>
    /// Test suite to verify how ResourceLoadingCallback works.
    /// </summary>
    [TestFixture]
    public class TestResourceLoadingCallback
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



        // FOSS: TestBanNetworkAccess removed — its input is a .doc (Doc reader removed) with an
        // INCLUDEPICTURE field to an external image and no .docx sibling. The network-ban callback
        // path stays covered by SkipLoadingDrawingMlExternalImage (a DrawingML external image).


        /// <summary>
        /// WORDSNET-20391 Make sure all external resources are importing via IResourceLoadingCallback.
        /// This case is about loading external template during UpdateStyles.
        /// There is no way to set AttachedTemplate as web link through the MS Word UI but it can be done manually unpacking DOCX zip,
        /// or using AW.
        /// So, it needs to be cover using IResourceLoadingCallback.
        /// </summary>
        [Test]
        public void Test20391_UpdateStyles()
        {
            LoadOptions loadOptions = new LoadOptions();
            SkipLoadingTemplateCallback skipLoad = new SkipLoadingTemplateCallback();
            loadOptions.ResourceLoadingCallback = skipLoad;

            Assert.That(skipLoad.CallCount, Is.EqualTo(0));

            // Try to open document with Document.AutomaticallyUpdateStyles turned on.
            Document doc = TestUtil.Open(@"Model\Other\ExternalAttachedTemplate.docx", loadOptions);
            Assert.That(doc.AutomaticallyUpdateStyles, Is.True);

            // Resource loading callback not called because we do not update styles on loading for a while.
            Assert.That(skipLoad.CallCount, Is.EqualTo(0));
            Assert.That(doc.DocPr.AttachedTemplate, Is.EqualTo("https://github.com/aspose-words/template.docx"));

            // FOSS: Pdf save removed; the attached-template load during style update runs on the
            // OOXML save path too.
            doc.Save(new MemoryStream(), SaveFormat.Docx);

            // Resource loading callback called.
            Assert.That(skipLoad.CallCount, Is.EqualTo(1));
        }

        /// <summary>
        /// Skip loading template from external resources.
        /// </summary>
        private class SkipLoadingTemplateCallback : IResourceLoadingCallback
        {
            public ResourceLoadingAction ResourceLoading(ResourceLoadingArgs args)
            {
                if (args.ResourceType == ResourceType.Document)
                {
                    CallCount++;
                    return ResourceLoadingAction.Skip;
                }

                return ResourceLoadingAction.Default;
            }

            internal int CallCount { get; private set; }
        }

        private class BanNetworkAccessHandler : IResourceLoadingCallback
        {
            public ResourceLoadingAction ResourceLoading(ResourceLoadingArgs args)
            {
                if (args.ResourceType == ResourceType.Image)
                    return ResourceLoadingAction.Skip;
             
                return ResourceLoadingAction.Default;
            }
        }

        private class InsertBase64ImageHandler : IResourceLoadingCallback
        {
            public ResourceLoadingAction ResourceLoading(ResourceLoadingArgs args)
            {
                if (args.ResourceType == ResourceType.Image)
                {
                    string image64 = args.OriginalUri;
                    byte[] imageBytes = Convert.FromBase64String(image64);
                    args.SetData(imageBytes);
                    return ResourceLoadingAction.UserProvided;
                }
                else
                {
                    return ResourceLoadingAction.Default;
                }
            }
        }
    }
}
