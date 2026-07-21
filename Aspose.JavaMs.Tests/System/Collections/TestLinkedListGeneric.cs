// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 28/01/2019 by Anatoly Sidorenko

using System.Collections.Generic;
using NUnit.Framework;

namespace Aspose.JavaMs.Tests.System.Collections
{
    [TestFixture]
    public class TestLinkedListGeneric
    {
        [Test]
        public void TestLinkedListCtor()
        {
            LinkedList<object> list = new LinkedList<object>();
            Assert.That(list, IsNot.Null());
            Assert.That(list.Count, Is.EqualTo(0));

            list.AddLast("A");
            Assert.That(list.Count, Is.EqualTo(1));

            list.Clear();
            Assert.That(list.Count, Is.EqualTo(0));

            string[] array = { "A", "B", "C" };
            LinkedList<string> list1 = new LinkedList<string>(array);
            Assert.That(list1, IsNot.Null());
            Assert.That(list1.Count, Is.EqualTo(3));

            LinkedList<string> list2 = new LinkedList<string>(list1);
            Assert.That(list2, IsNot.Null());
            Assert.That(list2.Count, Is.EqualTo(3));
        }

        [Test]
        public void TestLinkedListAddLast()
        {
            LinkedList<string> list = new LinkedList<string>();

            list.AddLast("A");
            Assert.That(list.Last.Value, Is.EqualTo("A"));

            list.AddLast("B");
            Assert.That(list.Last.Value, Is.EqualTo("B"));

            list.AddLast("C");
            Assert.That(list.Last.Value, Is.EqualTo("C"));
            Assert.That(list.First.Value, Is.EqualTo("A"));
            Assert.That(list.Count, Is.EqualTo(3));
        }

        [Test]
        public void TestLinkedListAddFirst()
        {
            LinkedList<string> list = new LinkedList<string>();

            list.AddFirst("A");
            Assert.That(list.First.Value, Is.EqualTo("A"));

            list.AddFirst("B");
            Assert.That(list.First.Value, Is.EqualTo("B"));

            list.AddFirst("C");
            Assert.That(list.First.Value, Is.EqualTo("C"));
            Assert.That(list.Last.Value, Is.EqualTo("A"));
            Assert.That(list.Count, Is.EqualTo(3));
        }

        [Test]
        public void TestLinkedListNode()
        {
            string[] array = { "A", "B", "C" };
            LinkedList<string> list = new LinkedList<string>(array);

            LinkedListNode<string> node = list.First;
            Assert.That(node.Value, Is.EqualTo("A"));

            node = node.Next;
            Assert.That(node.Value, Is.EqualTo("B"));

            node = node.Next;
            Assert.That(node.Value, Is.EqualTo("C"));

            node = node.Previous;
            Assert.That(node.Value, Is.EqualTo("B"));

            node = node.Next.Next;
            Assert.That(node, Is.Null);
        }

        [Test]
        public void TestLinkedListAddAfter()
        {
            string[] array = { "A", "B", "C" };
            LinkedList<string> list = new LinkedList<string>(array);

            LinkedListNode<string> node = list.First.Next;
            Assert.That(node.Value, Is.EqualTo("B"));

            list.AddAfter(node, "D");
            Assert.That(node.Next.Value, Is.EqualTo("D"));
            Assert.That(list.Last.Previous.Value, Is.EqualTo("D"));
            Assert.That(list.Count, Is.EqualTo(4));

            node = list.AddAfter(list.First, "E");
            Assert.That(node.Value, Is.EqualTo("E"));
            Assert.That(node.Previous.Value, Is.EqualTo("A"));
            Assert.That(list.Count, Is.EqualTo(5));
        }

        [Test]
        public void TestLinkedListAddBefore()
        {
            string[] array = { "A", "B", "C" };
            LinkedList<string> list = new LinkedList<string>(array);

            LinkedListNode<string> node = list.First.Next;
            Assert.That(node.Value, Is.EqualTo("B"));

            list.AddBefore(node, "D");
            Assert.That(node.Previous.Value, Is.EqualTo("D"));
            Assert.That(list.First.Next.Value, Is.EqualTo("D"));
            Assert.That(list.Count, Is.EqualTo(4));

            node = list.AddBefore(list.Last, "E");
            Assert.That(node.Value, Is.EqualTo("E"));
            Assert.That(node.Next.Value, Is.EqualTo("C"));
            Assert.That(list.Count, Is.EqualTo(5));
        }
        
        [Test]
        public void TestLinkedListContains()
        {
            string[] array = { "A", "B", "C" };
            LinkedList<string> list = new LinkedList<string>(array);

            Assert.That(list.Contains("A"), Is.True);
            Assert.That(list.Contains("D"), Is.False);

            list.AddLast("D");
            Assert.That(list.Contains("D"), Is.True);
        }
        
        [Test]
        public void TestLinkedListRemove()
        {
            string[] array = { "A", "B", "C" };
            LinkedList<string> list = new LinkedList<string>(array);

            Assert.That(list.Contains("B"), Is.True);
            list.Remove("B");
            Assert.That(list.Contains("B"), Is.False);

            list.Remove(list.Last);
            Assert.That(list.Contains("C"), Is.False);
        }

        [Test]
        public void TestLinkedListForeach()
        {
            string[] array = { "A", "B", "C" };
            LinkedList<string> list = new LinkedList<string>(array);

            LinkedList<string> list1 = new LinkedList<string>();
            foreach (string node in list)
                list1.AddFirst(node);

            Assert.That(list1.First.Value, Is.EqualTo("C"));
            Assert.That(list1.Last.Value, Is.EqualTo("A"));
            Assert.That(list1.Count, Is.EqualTo(3));
        }

        [Test]
        public void TestLinkedListWhile()
        {
            string[] array = { "A", "B", "C" };
            LinkedList<string> list = new LinkedList<string>(array);

            LinkedListNode<string> currentListNode = list.First;
            while (currentListNode != null)
            {
                LinkedListNode<string> nextListNode = currentListNode.Next;
                string nodeValue = currentListNode.Value;
                if (nodeValue == "B")
                    list.Remove(currentListNode);
                currentListNode = nextListNode;
            }

            Assert.That(list.Contains("B"), Is.False);
        }

        [Test]
        public void TestLinkedListAddNode()
        {
            string[] array = { "A", "B", "C" };
            LinkedList<string> list = new LinkedList<string>(array);

            LinkedListNode<string> firstListNode = list.First;
            LinkedListNode<string> lastListNode = list.Last;

            list.Remove(lastListNode);
            Assert.That(list.Contains("C"), Is.False);
            list.AddFirst(lastListNode);
            Assert.That(list.Contains("C"), Is.True);

            list.Remove(firstListNode);
            Assert.That(list.Contains("A"), Is.False);
            list.AddLast(firstListNode);
            Assert.That(list.Contains("A"), Is.True);

            Assert.That(list.First.Value, Is.EqualTo("C"));
            Assert.That(list.Last.Value, Is.EqualTo("A"));
            Assert.That(list.Count, Is.EqualTo(3));
        }
    }
}
