// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 16/05/2013 by Ivan Lyagin
using NUnit.Framework;

namespace Aspose.Words.Tests.Model
{
    /// <summary>
    /// Tests NodeChangingCallbackChain class functionality and relative DocumentBase class functionality.
    /// </summary>
    [TestFixture]
    public class TestNodeChangingCallbackChain
    {
        /// <summary>
        /// UC1. A user callback is set, invoked and removed.
        /// </summary>
        [Test]
        public void TestUC1()
        {
            Document doc = new Document(DocumentCtorMode.EmptyDocumentNode);
            TestNodeChangingCallback callback = new TestNodeChangingCallback();

            // Set a user callback. Ensure it is fired.
            doc.NodeChangingCallback = callback;
            Assert.That(doc.NodeChangingCallback, Is.EqualTo(callback));

            // Insert a node to the document. This should trigger the callback.
            doc.AppendChild(new Section(doc));

            // Remove the user callback when it is not needed anymore. Ensure it is fired.
            doc.NodeChangingCallback = null;
            Assert.That(doc.NodeChangingCallback, Is.Null);

            // Check the callback's reaction.
            Assert.That(callback.InsertedCount, Is.EqualTo(1));
        }

        /// <summary>
        /// UC2. An internal callback is set, invoked and removed.
        /// </summary>
        [Test]
        public void TestUC2()
        {
            Document doc = new Document(DocumentCtorMode.EmptyDocumentNode);
            TestNodeChangingCallback callback = new TestNodeChangingCallback();

            // Set an internal callback.
            doc.AddInternalNodeChangingCallback(callback);

            // Insert a node to the document. This should trigger the callback.
            doc.AppendChild(new Section(doc));

            // Remove the internal callback when it is not needed anymore.
            doc.RemoveInternalNodeChangingCallback(callback);

            // Check the callback's reaction.
            Assert.That(callback.InsertedCount, Is.EqualTo(1));
        }

        /// <summary>
        /// Tests how DocumentBase.RemoveInternalNodeChangingCallback works.
        /// </summary>
        [Test]
        public void TestRemoveInternal()
        {
            Document doc = new Document(DocumentCtorMode.EmptyDocumentNode);
            TestNodeChangingCallback callback = new TestNodeChangingCallback();

            // Nothing has been added. Removal should not fire.
            Assert.That(doc.RemoveInternalNodeChangingCallback(callback), Is.False);

            doc.AddInternalNodeChangingCallback(callback);

            // Removal should fire as the callback has been added.
            Assert.That(doc.RemoveInternalNodeChangingCallback(callback), Is.True);

            // Removal should not fire as the callback has been removed.
            Assert.That(doc.RemoveInternalNodeChangingCallback(callback), Is.False);
        }

        /// <summary>
        /// Tests how NodeChangingCallbackChain.IsEmpty works.
        /// </summary>
        [Test]
        public void TestIsEmpty()
        {
            NodeChangingCallbackChain chain = new NodeChangingCallbackChain();
            Assert.That(chain.IsEmpty, Is.True);

            INodeChangingCallback callback = new TestNodeChangingCallback();
            chain.UserCallback = callback;
            Assert.That(chain.IsEmpty, Is.False);

            chain.UserCallback = null;
            Assert.That(chain.IsEmpty, Is.True);

            chain.AddInternalCallback(callback);
            Assert.That(chain.IsEmpty, Is.False);

            chain.RemoveInternalCallback(callback);
            Assert.That(chain.IsEmpty, Is.True);
        }

        /// <summary>
        /// Tests a complex use case with multiple internal callbacks and multiple values for a user callback.
        /// Ensures that a chain of invocation is not broken if its item is removed inside the corresponding callback.
        /// Ensures that all of the callbacks are fired properly, i.e. removed callbacks stop firing at the point
        /// of their removal.
        /// </summary>
        [Test]
        public void TestComplex()
        {
            Document doc = new Document(DocumentCtorMode.EmptyDocumentNode);

            TestNodeChangingCallback callback1 = new TestNodeChangingCallback();
            TestNodeChangingCallback callback2 = new TestNodeChangingCallback(doc);
            TestNodeChangingCallback callback3 = new TestNodeChangingCallback();
            TestNodeChangingCallback callback4 = new TestNodeChangingCallback();
            TestNodeChangingCallback callback5 = new TestNodeChangingCallback();
            TestNodeChangingCallback callback6 = new TestNodeChangingCallback(doc);

            // Add couple of internal callbacks.
            doc.AddInternalNodeChangingCallback(callback1);
            doc.AddInternalNodeChangingCallback(callback2);

            // Set the user callback. Ensure that it has fired.
            doc.NodeChangingCallback = callback5;
            Assert.That(doc.NodeChangingCallback, Is.EqualTo(callback5));

            // Add more internal callbacks.
            doc.AddInternalNodeChangingCallback(callback3);
            doc.AddInternalNodeChangingCallback(callback4);

            // Replace the user callback. Ensure that it has fired.
            doc.NodeChangingCallback = callback6;
            Assert.That(doc.NodeChangingCallback, Is.EqualTo(callback6));

            // Remove some internal callbacks (from the beginning and from the end). They should never fire.
            doc.RemoveInternalNodeChangingCallback(callback1);
            doc.RemoveInternalNodeChangingCallback(callback4);

            // Insert two nodes and remove one of them.
            doc.AppendChild(new Section(doc));
            doc.AppendChild(new Section(doc)).Remove();

            // A user callback should be removed while reacting.
            Assert.That(doc.NodeChangingCallback, Is.Null);

            // This callback should not fire as it was removed before any node manipulations.
            Assert.That(callback1.InsertedCount, Is.EqualTo(0));
            Assert.That(callback1.RemovedCount, Is.EqualTo(0));

            // This callback should fire once and be removed while reacting.
            Assert.That(callback2.InsertedCount, Is.EqualTo(1));
            Assert.That(callback2.RemovedCount, Is.EqualTo(0));

            // This callback should fire all of the time.
            Assert.That(callback3.InsertedCount, Is.EqualTo(2));
            Assert.That(callback3.RemovedCount, Is.EqualTo(1));

            // This callback should not fire as it was removed before any node manipulations.
            Assert.That(callback4.InsertedCount, Is.EqualTo(0));
            Assert.That(callback4.RemovedCount, Is.EqualTo(0));

            // This callback should not fire as it was removed before any node manipulations.
            Assert.That(callback5.InsertedCount, Is.EqualTo(0));
            Assert.That(callback5.RemovedCount, Is.EqualTo(0));

            // This callback should fire once and be removed while reacting.
            Assert.That(callback6.InsertedCount, Is.EqualTo(1));
            Assert.That(callback6.RemovedCount, Is.EqualTo(0));
        }


    }
}