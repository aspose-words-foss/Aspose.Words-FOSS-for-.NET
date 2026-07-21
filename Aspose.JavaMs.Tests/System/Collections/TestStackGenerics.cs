// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 28/01/2018 by Anatoly Sidorenko

using System.Collections.Generic;
using NUnit.Framework;

namespace Aspose.JavaMs.Tests.System.Collections
{
    [TestFixture]
    public class TestStackGenerics
    {
        [Test]
        public void TestCtor()
        {
            Stack<string> stack = new Stack<string>();
            Assert.That(stack, IsNot.Null());
            Assert.That(0, Is.EqualTo(stack.Count));

            stack.Push("cde");
            stack.Push("abc");
            stack.Push("bcd");

            Assert.That(3, Is.EqualTo(stack.Count));

            Assert.That("bcd", Is.EqualTo(stack.Peek()));
            Assert.That("bcd", Is.EqualTo(stack.Pop()));
            Assert.That("abc", Is.EqualTo(stack.Peek()));
            Assert.That("abc", Is.EqualTo(stack.Pop()));
            Assert.That("cde", Is.EqualTo(stack.Peek()));
            Assert.That("cde", Is.EqualTo(stack.Pop()));

            Assert.That(0, Is.EqualTo(stack.Count));
        }

        [Test]
        public void TestCtor2()
        {
            Stack<string> stack = new Stack<string>(100);
            Assert.That(stack, IsNot.Null());
            Assert.That(0, Is.EqualTo(stack.Count));

            stack = new Stack<string>(0);
            Assert.That(stack, IsNot.Null());
            Assert.That(0, Is.EqualTo(stack.Count));
        }

        [Test]
        public void TestCount()
        {
            Stack<string> stack = new Stack<string>();
            Assert.That(0, Is.EqualTo(stack.Count));

            stack.Push("cde");
            Assert.That(1, Is.EqualTo(stack.Count));

            stack.Push("abc");
            Assert.That(2, Is.EqualTo(stack.Count));

            stack.Push("bcd");
            Assert.That(3, Is.EqualTo(stack.Count));

            stack = GetPopulatedStack(50);
            Assert.That(50, Is.EqualTo(stack.Count));
        }


        [Test]
        public void TestForeach()
        {
            Stack<string> stack = GetPopulatedStack(3);

            string result = "";
            foreach (string name in stack)
            {
                result += name;
            }

            Assert.That(" val_2 val_1 val_0", Is.EqualTo(result));
            Assert.That(3, Is.EqualTo(stack.Count));
        }

        private static Stack<string> GetPopulatedStack(int size)
        {
            Stack<string> st = new Stack<string>();
            for (int i = 0; i < size; i++)
                st.Push(" val_" + i);
            return st;
        }
    }
}
