// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 06/05/2025 by Ilya Navrotskiy

using System.Text;
using Aspose.Charset;
using NUnit.Framework;

namespace Aspose.Words.Tests.Other
{
    /// <summary>
    /// The class to test <see cref="TextRange"/>.
    /// </summary>
    [TestFixture]
    public class TestTextRange
    {
        [TestFixtureSetUp]
        public void SetUp()
        {
            TestUtil.SetUpTests();
        }

        /// <summary>
        /// Tests <see cref="TextRange.Find"/>.
        /// Tests simple case with start, end and two required texts between them.
        /// </summary>
        [Test]
        public void TestFindA()
        {
            byte[] input = Encoding.UTF8.GetBytes("Lorem ipsum dolor sit amet");
            TextRange range = new TextRange("Lorem","amet", new string[]{"ipsum", "sit"});

            Assert.That(range.Find(input, 0), Is.True);

            Assert.That(range.StartIndex, Is.EqualTo(0));
            Assert.That(range.EndIndex, Is.EqualTo(22));
            Assert.That(range.NextIndex, Is.EqualTo(26));
        }

        /// <summary>
        /// Tests <see cref="TextRange.Find"/>.
        /// Tests that there can not be end text before required texts.
        /// </summary>
        [Test]
        public void TestFindB()
        {
            byte[] input = Encoding.UTF8.GetBytes("Start req1 end req2 sit end");
            TextRange range = new TextRange("Start", "end", new string[] { "req1", "req2" });

            Assert.That(range.Find(input, 0), Is.False);
        }

        /// <summary>
        /// Tests <see cref="TextRange.Find"/>.
        /// Tests that end can be missing in the search array.
        /// </summary>
        [Test]
        public void TestFindC()
        {
            byte[] input = Encoding.UTF8.GetBytes("Start req1 req2 sit");
            TextRange range = new TextRange("Start", "end", new string[] { "req1", "req2" });

            Assert.That(range.Find(input, 0), Is.True);
            Assert.That(range.StartIndex, Is.EqualTo(0));
            Assert.That(range.EndIndex, Is.EqualTo(-1));
            Assert.That(range.NextIndex, Is.EqualTo(0));
        }

        /// <summary>
        /// Tests <see cref="TextRange.Find"/>.
        /// Tests when search start index is greater than beginning of start text.
        /// </summary>
        [Test]
        public void TestFindD()
        {
            byte[] input = Encoding.UTF8.GetBytes("Start req1 req2 sit end");
            TextRange range = new TextRange("Start", "end", new string[] { "req1", "req2" });

            Assert.That(range.Find(input, 1), Is.False);
        }

        /// <summary>
        /// Tests <see cref="TextRange.FindEnd"/>.
        /// Tests when end is located in other than start input array.
        /// </summary>
        [Test]
        public void TestFindEnd()
        {
            byte[] input = Encoding.UTF8.GetBytes("Start req1 req2 sit");
            TextRange range = new TextRange("Start", "end", new string[] { "req1", "req2" });

            Assert.That(range.Find(input, 0), Is.True);
            Assert.That(range.StartIndex, Is.EqualTo(0));
            Assert.That(range.EndIndex, Is.EqualTo(-1));

            input = Encoding.UTF8.GetBytes("second buffer with end");
            Assert.That(range.FindEnd(input, 0), Is.True);
            Assert.That(range.StartIndex, Is.EqualTo(0));
            Assert.That(range.EndIndex, Is.EqualTo(19));
        }

        /// <summary>
        /// Tests <see cref="TextRange.IsBefore"/>.
        /// </summary>
        [Test]
        public void TestIsBeforeA()
        {
            byte[] input = Encoding.UTF8.GetBytes("Start1 req1 Start2 req2 sit end");
            TextRange rangeA = new TextRange("Start1", "end", new string[] { "req1" });
            TextRange rangeB = new TextRange("Start2", "end", new string[] { "req2" });

            Assert.That(rangeA.Find(input, 0), Is.True);
            Assert.That(rangeB.Find(input, 0), Is.True);

            Assert.That(rangeA.IsBefore(rangeB), Is.True);
            Assert.That(rangeB.IsBefore(rangeA), Is.False);
        }

        /// <summary>
        /// Tests <see cref="TextRange.IsBefore"/>.
        /// Tests when other range is null.
        /// </summary>
        [Test]
        public void TestIsBeforeB()
        {
            byte[] input = Encoding.UTF8.GetBytes("Start1 req1 Start2 req2 sit end");
            TextRange rangeA = new TextRange("Start1", "end", new string[] { "req1" });

            Assert.That(rangeA.Find(input, 0), Is.True);
            Assert.That(rangeA.IsBefore(null), Is.True);
        }

        /// <summary>
        /// Tests <see cref="TextRange.IsBefore"/>.
        /// Tests when other range is not found.
        /// </summary>
        [Test]
        public void TestIsBeforeC()
        {
            byte[] input = Encoding.UTF8.GetBytes("Start1 req1 Start2 req2 sit end");
            TextRange rangeA = new TextRange("Start1", "end", new string[] { "req1" });
            TextRange rangeB = new TextRange("No such text", "end", new string[] { "req1" });

            Assert.That(rangeA.Find(input, 0), Is.True);
            Assert.That(rangeB.Find(input, 0), Is.False);
            Assert.That(rangeA.IsBefore(rangeB), Is.True);
        }
    }
}
