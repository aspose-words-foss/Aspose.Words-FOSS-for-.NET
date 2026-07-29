// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 20/10/2019 by Dmitry Sokolov

using System;
using Aspose.Common;
using Aspose.Words.WebExtensions;
using NUnit.Framework;

namespace Aspose.Words.Tests.Model
{
    /// <summary>
    /// Checks manipulations with the task pane add-ins through the model.
    /// </summary>
    [TestFixture]
    public class TestWebExtensions
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
        /// Checks that client can not add empty item to task pane collection.
        /// </summary>
        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Test18681AddNullItemToTaskPaneCollection()
        {
            TaskPaneCollection collection = new TaskPaneCollection();
            collection.Add(null);
        }

        /// <summary>
        /// Related to WORDSNET-18681
        /// Checks that client can not add empty item to bindings collection.
        /// </summary>
        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Test18681AddNullItemToBindingsCollection()
        {
            WebExtensionBindingCollection collection = new WebExtensionBindingCollection();
            collection.Add(null);
        }

        /// <summary>
        /// Related to WORDSNET-18681
        /// Checks that client can not add empty item to properties collection.
        /// </summary>
        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Test18681AddNullItemToPropertiesCollection()
        {
            WebExtensionPropertyCollection collection = new WebExtensionPropertyCollection();
            collection.Add(null);
        }

        /// <summary>
        /// Related to WORDSNET-18681
        /// Checks that client can not add empty item to references collection.
        /// </summary>
        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Test18681AddNullItemToReferencesCollection()
        {
            WebExtensionReferenceCollection collection = new WebExtensionReferenceCollection();
            collection.Add(null);
        }
    }
}
