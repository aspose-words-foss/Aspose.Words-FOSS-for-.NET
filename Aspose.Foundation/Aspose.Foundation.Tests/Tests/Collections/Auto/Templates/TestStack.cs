// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 02/09/2013 by Ivan Lyagin

#if INCLUDE_FILE
using System;
using Aspose.Collections;
using NUnit.Framework;

namespace Aspose.Tests.Collections.Auto
{
    /// <summary>
    /// Tests Stack<T> class' functionality.
    /// </summary>
    /// <remarks>
    /// Every individual test of this class corresponds to a Stack<T> member.
    /// See the corresponding Stack<T> members' descriptions for any additional information.
    /// </remarks>
    [TestFixture]
    public class TestStack<T>
    {
        [Test]
        public void TestClear()
        {
            Stack<T> stack = new Stack<T>();
            stack.Push(LastItem);
            stack.Clear();
            Assert.That(stack.Count, Is.EqualTo(0));
        }

        [Test]
        public void TestContains()
        {
            Stack<T> stack = new Stack<T>();
            stack.Push(LastItem);
            Assert.That(stack.Contains(LastItem), Is.True);
            Assert.That(stack.Contains(gArray[0]), Is.False);
        }

        [Test]
        public void TestCopyTo()
        {
            T[] array = new T[gArray.Length];
            Stack<T> stack = new Stack<T>(gArray);
            stack.CopyTo(array, 0);
            Assert.That(array, Is.EqualTo(gReversedArray));
        }

        [Test, ExpectedException(typeof(ArgumentNullException))]
        public void TestCopyToNullArray()
        {
            Stack<T> stack = new Stack<T>(gArray);
            stack.CopyTo(null, 0);
        }

        [Test, ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void TestCopyToNegativeArrayIndex()
        {
            T[] array = new T[gArray.Length];
            Stack<T> stack = new Stack<T>(gArray);
            stack.CopyTo(array, -1);
        }

        [Test, ExpectedException(typeof(ArgumentException))]
        public void TestCopyToInvalidArrayRange()
        {
            T[] array = new T[gArray.Length - 1];
            Stack<T> stack = new Stack<T>(gArray);
            stack.CopyTo(array, 0);
        }

        [Test]
        public void TestPeek()
        {
            Stack<T> stack = new Stack<T>(gArray);
            Assert.That(stack.Peek(), Is.EqualTo(LastItem));
            Assert.That(stack.Count, Is.EqualTo(gArray.Length));
        }

        [Test, ExpectedException(typeof(InvalidOperationException))]
        public void TestPeekEmpty()
        {
            Stack<T> stack = new Stack<T>();
            stack.Peek();
        }

        [Test]
        public void TestPop()
        {
            Stack<T> stack = new Stack<T>(gArray);
            Assert.That(stack.Pop(), Is.EqualTo(LastItem));
            Assert.That(stack.Count, Is.EqualTo(gArray.Length - 1));
        }

        [Test, ExpectedException(typeof(InvalidOperationException))]
        public void TestPopEmpty()
        {
            Stack<T> stack = new Stack<T>();
            stack.Pop();
        }

        [Test]
        public void TestPush()
        {
            Stack<T> stack = new Stack<T>();
            stack.Push(LastItem);
            Assert.That(stack.Count, Is.EqualTo(1));
            Assert.That(stack.Peek(), Is.EqualTo(LastItem));
        }

        [Test]
        public void ToArray()
        {
            Stack<T> stack = new Stack<T>(gArray);
            Assert.That(stack.ToArray(), Is.EqualTo(gReversedArray));
        }

        [Test]
        public void TestCount()
        {
            Stack<T> stack = new Stack<T>();
            Assert.That(stack.Count, Is.EqualTo(0));

            stack.Push(LastItem);
            Assert.That(stack.Count, Is.EqualTo(1));

            stack.Push(LastItem);
            Assert.That(stack.Count, Is.EqualTo(2));

            stack.Pop();
            Assert.That(stack.Count, Is.EqualTo(1));

            stack.Clear();
            Assert.That(stack.Count, Is.EqualTo(0));
        }

        [Test]
        public void TestCopyCtors()
        {
            Stack<T> stack1 = new Stack<T>(gArray);
            Assert.That(stack1.ToArray(), Is.EqualTo(gReversedArray));

            Stack<T> stack2 = new Stack<T>(stack1);
            Assert.That(stack2.ToArray(), Is.EqualTo(gReversedArray));
        }

        [Test, ExpectedException(typeof(ArgumentNullException))]
        public void TestCopyCtorNullArray()
        {
            Stack<T> stack = new Stack<T>((T[])null);
            Assert.Fail(string.Format(string.Empty, stack)); // Force the compiler not to skip the previous instruction.
        }

        [Test, ExpectedException(typeof(ArgumentNullException))]
        public void TestCopyCtorNullStack()
        {
            Stack<T> stack = new Stack<T>((Stack<T>)null);
            Assert.Fail(string.Format(string.Empty, stack)); // Force the compiler not to skip the previous instruction.
        }

        /// <summary>
        /// Returns an array extensively used in this test.
        /// </summary>
        private static T[] GetArray()
        {
            throw new NotSupportedException("GetArrayPlaceholder");
        }

        /// <summary>
        /// Returns reversed copy of the test array.
        /// </summary>
        private static T[] GetReversedArray()
        {
            T[] array = (T[])GetArray().Clone();
            Array.Reverse(array);
            return array;
        }

        /// <summary>
        /// Gets the last item of the test array.
        /// </summary>
        private static T LastItem
        {
            get { return gArray[gArray.Length - 1]; }
        }

        // Calculate once, use multiple times.
        private static readonly T[] gArray = GetArray();
        private static readonly T[] gReversedArray = GetReversedArray();
    }
}
#endif