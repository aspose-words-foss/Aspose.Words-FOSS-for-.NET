// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 29/03/2013 by Ivan Pereshein
using System;
using Aspose.Words.RW.Markdown;
using NUnit.Framework;

namespace Aspose.Words.Tests
{
    [TestFixture]
    public class TestLinkedList
    {
        [Test]
        public void TestInsertFirstNode()
        {
            // Expected list:
            // A(0)
            LinkedList list = new LinkedList();
            Assert.That(list.IsEmpty, Is.True);

            ILinkedListNode nodeA = new FakeLinkedListNode("A");

            list.Insert(nodeA);
            list.MoveObject(nodeA);

            Assert.That(nodeA.Index, Is.EqualTo(0));
            Assert.That(nodeA.SecondaryIndex, Is.EqualTo(0));
            Assert.That(nodeA, Is.EqualTo(list.GetHead_Test));
            Assert.That(nodeA, Is.EqualTo(list.GetTail_Test));
            Assert.That(nodeA, Is.EqualTo(list.Current));
        }

        /// <summary>
        /// Creates list: A[0|0], B[1|0], C[2|0], D[3|0].
        /// </summary>
        private static LinkedList CreateABCDList()
        {
            LinkedList list = new LinkedList();

            ILinkedListNode nodeA = new FakeLinkedListNode("A");

            list.Insert(nodeA);
            list.MoveObject(nodeA);

            ILinkedListNode nodeB = new FakeLinkedListNode("B");
            list.Append(nodeB);
            list.MoveObject(nodeB);

            ILinkedListNode nodeC = new FakeLinkedListNode("C");
            list.Append(nodeC);
            list.MoveObject(nodeC);

            ILinkedListNode nodeD = new FakeLinkedListNode("D");
            list.Append(nodeD);
            list.MoveObject(nodeD);

            return list;
        }

        [Test]
        public void TestCreateSimpleList()
        {
            // A[0|0], B[1|0], C[2|0], D[3|0]
            LinkedList list = CreateABCDList();

            ILinkedListNode nodeA = list.GetHead_Test;
            ILinkedListNode nodeB = nodeA.NextNode;
            ILinkedListNode nodeC = nodeB.NextNode;
            ILinkedListNode nodeD = nodeC.NextNode;

            Assert.That("A", Is.EqualTo(nodeA.ToString()));
            Assert.That("B", Is.EqualTo(nodeB.ToString()));
            Assert.That("C", Is.EqualTo(nodeC.ToString()));
            Assert.That("D", Is.EqualTo(nodeD.ToString()));

            Assert.That("A", Is.EqualTo(list.GetHead_Test.ToString()));
            Assert.That("D", Is.EqualTo(list.GetTail_Test.ToString()));

            Assert.That(nodeA.PrevNode, Is.Null);
            Assert.That(nodeA.NextNode, Is.EqualTo(nodeB));
            Assert.That(nodeB.PrevNode, Is.EqualTo(nodeA));
            Assert.That(nodeB.NextNode, Is.EqualTo(nodeC));
            Assert.That(nodeC.PrevNode, Is.EqualTo(nodeB));
            Assert.That(nodeC.NextNode, Is.EqualTo(nodeD));
            Assert.That(nodeD.PrevNode, Is.EqualTo(nodeC));
            Assert.That(nodeD.NextNode, Is.Null);

            Assert.That(nodeA.Index, Is.EqualTo(0));
            Assert.That(nodeA.SecondaryIndex, Is.EqualTo(0));
            Assert.That(nodeB.Index, Is.EqualTo(1));
            Assert.That(nodeB.SecondaryIndex, Is.EqualTo(0));
            Assert.That(nodeC.Index, Is.EqualTo(2));
            Assert.That(nodeC.SecondaryIndex, Is.EqualTo(0));
            Assert.That(nodeD.Index, Is.EqualTo(3));
            Assert.That(nodeD.SecondaryIndex, Is.EqualTo(0));
        }

        [Test]
        public void TestInsertIntoList()
        {
            // A[0|0], B[1|0], Ba[1|1], Bb[1|2], C[2|0], D[3|0]

            LinkedList list = CreateABCDList();

            ILinkedListNode nodeB = list.GetNode_Test("B");

            // Append new node (Ba) after (B)
            list.MoveObject(nodeB);
            list.Append(new FakeLinkedListNode("Ba"));

            ILinkedListNode nodeBa = list.GetNode_Test("Ba");
            ILinkedListNode nodeC = list.GetNode_Test("C");

            Assert.That(nodeB.NextNode, Is.EqualTo(nodeBa));
            Assert.That(nodeBa.PrevNode, Is.EqualTo(nodeB));
            Assert.That(nodeBa.NextNode, Is.EqualTo(nodeC));
            Assert.That(nodeC.PrevNode, Is.EqualTo(nodeBa));

            Assert.That(nodeB.Index, Is.EqualTo(1));
            Assert.That(nodeB.SecondaryIndex, Is.EqualTo(0));

            Assert.That(nodeBa.Index, Is.EqualTo(1));
            Assert.That(nodeBa.SecondaryIndex, Is.EqualTo(1000000000));

            // Insert new node before (C)
            list.MoveObject(nodeC);
            
            list.Insert(new FakeLinkedListNode("Bb"));

            ILinkedListNode nodeBb = list.GetNode_Test("Bb");

            Assert.That(nodeBa.NextNode, Is.EqualTo(nodeBb));
            Assert.That(nodeBb.PrevNode, Is.EqualTo(nodeBa));
            Assert.That(nodeBb.NextNode, Is.EqualTo(nodeC));
            Assert.That(nodeC.PrevNode, Is.EqualTo(nodeBb));
            Assert.That(nodeBb.Index, Is.EqualTo(1));
            Assert.That(nodeBb.SecondaryIndex, Is.EqualTo(2000000000));
        }

        [Test]
        public void TestInsertBeforeHead()
        {
            // Aa[-1|0] A[0|0], B[1|0], Ba[1|1], Bb[1|2], C[2|0], D[3|0]
            LinkedList list = CreateABCDList();
            ILinkedListNode nodeA = list.GetNode_Test("A");
            list.MoveObject(nodeA);

            ILinkedListNode nodeAa = new FakeLinkedListNode("Aa");
            list.Insert(nodeAa);

            Assert.That(list.GetHead_Test, Is.EqualTo(nodeAa));
            Assert.That(nodeAa.PrevNode, Is.Null);
            Assert.That(nodeAa.NextNode, Is.EqualTo(nodeA));
            Assert.That(nodeA.PrevNode, Is.EqualTo(nodeAa));

            Assert.That(nodeAa.Index, Is.EqualTo(-1));
            Assert.That(nodeAa.SecondaryIndex, Is.EqualTo(0));
        }

        [Test]
        public void TestRemoveHeadAndTail()
        {
            LinkedList list = CreateABCDList();
            ILinkedListNode nodeA = list.GetNode_Test("A");
            list.MoveObject(nodeA);
            // Remove head.
            list.RemoveCurrentNode();
            ILinkedListNode nodeB = list.GetNode_Test("B");
            Assert.That(list.GetHead_Test, Is.EqualTo(nodeB));
            Assert.That(list.Current, Is.EqualTo(nodeB));
            Assert.That(nodeB.PrevNode, Is.Null);

            list.MoveObject(list.GetNode_Test("D"));
            // Remove tail.
            list.RemoveCurrentNode();
            ILinkedListNode nodeC = list.GetNode_Test("C");
            Assert.That(list.GetTail_Test, Is.EqualTo(nodeC));
            Assert.That(nodeC.NextNode, Is.Null);
            Assert.That(list.IsAfter, Is.True);

        }

        [Test]
        public void TestRemoveNode()
        {
            LinkedList list = CreateABCDList();

            ILinkedListNode nodeA = list.GetNode_Test("A");
            ILinkedListNode nodeB = list.GetNode_Test("B");
            ILinkedListNode nodeC = list.GetNode_Test("C");

            list.MoveObject(nodeB);
            list.RemoveCurrentNode();

            Assert.That(list.Current, Is.EqualTo(nodeC));
            Assert.That(nodeA.NextNode, Is.EqualTo(nodeC));
            Assert.That(nodeC.PrevNode, Is.EqualTo(nodeA));
        }

        [Test]
        public void TestReplaceNode()
        {
            LinkedList list = CreateABCDList();

            // A[0|0], B[1|0], C[2|0], D[3|0] => A[0|0], C[2|0], D[3|0]
            ILinkedListNode nodeA = list.GetNode_Test("A");
            ILinkedListNode nodeB = list.GetNode_Test("B");
            ILinkedListNode nodeC = list.GetNode_Test("C");

            list.MoveObject(nodeB);
            list.RemoveCurrentNode();

            // A[0|0], C[2|0], D[3|0]=> A[0|0], B_New[1|0], C[2|0], D[3|0]
            list.MoveObject(nodeA);

            ILinkedListNode nodeBNew = new FakeLinkedListNode("B_New");
            list.Append(nodeBNew);

            Assert.That(nodeBNew.Index, Is.EqualTo(1));
            Assert.That(nodeBNew.SecondaryIndex, Is.EqualTo(0));

            // A[0|0], B_New[1|0], C[2|0], D[3|0] => A[0|0], B_New[1|0], D[3|0]
            list.MoveObject(nodeC);
            list.RemoveCurrentNode();

            // A[0|0], B_New[1|0], D[3|0] => A[0|0], B_New[1|0], C_New[2|0], D[3|0]
            ILinkedListNode nodeCNew = new FakeLinkedListNode("C_New");
            list.Insert(nodeCNew);
            Assert.That(nodeCNew.Index, Is.EqualTo(2));
            Assert.That(nodeCNew.SecondaryIndex, Is.EqualTo(0));

        }

        [Test]
        public void TestCloneList()
        {
            LinkedList list = CreateABCDList();

            ILinkedListNode nodeB = list.GetNode_Test("B");
            ILinkedListNode nodeC = list.GetNode_Test("C");

            list.MoveObject(nodeB);

            LinkedList clonedList = list.Clone();
            
            Assert.That(clonedList.Current, Is.EqualTo(list.Current));

            clonedList.RemoveCurrentNode();

            Assert.That(list.Current, Is.EqualTo(nodeC));
        }

        [Test]
        [ExpectedException(typeof(InvalidOperationException))]
        public void TestPreventDuplicateInsertion()
        {
            LinkedList list = CreateABCDList();

            ILinkedListNode nodeA = list.GetNode_Test("A");
            ILinkedListNode nodeC = list.GetNode_Test("C");

            list.MoveObject(nodeA);
            list.Append(nodeC);
        }
    }
}
