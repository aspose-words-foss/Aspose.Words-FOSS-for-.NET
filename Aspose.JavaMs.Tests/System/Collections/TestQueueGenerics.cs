// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 28/01/2018 by Anatoly Sidorenko

using System.Collections.Generic;
using NUnit.Framework;

namespace Aspose.JavaMs.Tests.System.Collections
{
    [TestFixture]
    public class TestQueueGenerics
    {
        [Test]
        public void TestCtor()
        {
            Queue<string> queue = new Queue<string>();
            Assert.That(queue, IsNot.Null());
            Assert.That(0, Is.EqualTo(queue.Count));

            queue.Enqueue("cde");
            queue.Enqueue("abc");
            queue.Enqueue("bcd");

            Assert.That(3, Is.EqualTo(queue.Count));

            Assert.That("cde", Is.EqualTo(queue.Dequeue()));
            Assert.That("abc", Is.EqualTo(queue.Dequeue()));
            Assert.That("bcd", Is.EqualTo(queue.Dequeue()));

            Assert.That(0, Is.EqualTo(queue.Count));
        }

        [Test]
        public void TestCtor2()
        {
            Queue<string> queue = new Queue<string>(100);
            Assert.That(queue, IsNot.Null());
            Assert.That(0, Is.EqualTo(queue.Count));

            queue = new Queue<string>(0);
            Assert.That(queue, IsNot.Null());
            Assert.That(0, Is.EqualTo(queue.Count));
        }

        [Test]
        public void TestCount()
        {
            Queue<string> queue = new Queue<string>();
            Assert.That(0, Is.EqualTo(queue.Count));

            queue.Enqueue("cde");
            Assert.That(1, Is.EqualTo(queue.Count));

            queue.Enqueue("abc");
            Assert.That(2, Is.EqualTo(queue.Count));

            queue.Enqueue("bcd");
            Assert.That(3, Is.EqualTo(queue.Count));

            queue = GetPopulatedQueue(50);
            Assert.That(50, Is.EqualTo(queue.Count));
        }


        [Test]
        public void TestForeach()
        {
            Queue<string> queue = GetPopulatedQueue(3);

            string result = "";
            foreach (string name in queue)
            {
                result += name;
            }

            Assert.That(" val_0 val_1 val_2", Is.EqualTo(result));
            Assert.That(3, Is.EqualTo(queue.Count));
        }

        private static Queue<string> GetPopulatedQueue(int size)
        {
            Queue<string> qu = new Queue<string>();
            for (int i = 0; i < size; i++)
                qu.Enqueue(" val_" + i);
            return qu;
        }
    }
}
