// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 05/06/2023 by Ilya Navrotskiy

using System.IO;
using Aspose.IO;
using NUnit.Framework;

namespace Aspose.Tests.IO
{
    /// <summary>
    /// The class for testing <see cref="StreamUtil"/>.
    /// </summary>
    [TestFixture]
    public class TestStreamUtil
    {
        /// <summary>
        /// Tests <see cref="StreamUtil.IsEndsWith"/> when stream has zero length.
        /// </summary>
        [Test]
        public void TestStreamUtilEndsWithZeroStream()
        {
            using (MemoryStream stream = new MemoryStream())
            {
                Assert.That(StreamUtil.IsEndsWith(stream, new byte[] { 0x0 }), Is.False);
            }
        }

        /// <summary>
        /// Tests <see cref="StreamUtil.IsEndsWith"/> with various input data.
        /// </summary>
        [TestCase(new byte[] { }, false)]
        [TestCase(new byte[] { 1, 2, 3, 4, 5 }, true)]
        [TestCase(new byte[] { 3, 4, 5 }, true)]
        [TestCase(new byte[] { 5 }, true)]
        [TestCase(new byte[] { 1, 5 }, false)]
        [TestCase(new byte[] { 1 }, false)]
        [TestCase(new byte[] { 1, 2, 3, 4 }, false)]
        [TestCase(new byte[] { 1, 2, 3, 4, 5, 6 }, false)]
        [TestCase(new byte[] { 0, 1, 2, 3, 4, 5 }, false)]
        public void TestStreamUtilEndsWith(byte[] bytes, bool expectedResult)
        {
            using (MemoryStream stream = new MemoryStream(new byte[] { 1, 2, 3, 4, 5 }))
            {
                // Check with different buffer length.
                // Check when buffer length equals to stream length.
                Assert.That(StreamUtil.IsEndsWith(stream, bytes, 5), Is.EqualTo(expectedResult));
                // Check when buffer length is greater than stream length.
                Assert.That(StreamUtil.IsEndsWith(stream, bytes, 10), Is.EqualTo(expectedResult));
                // Check when buffer length is less than stream length.
                Assert.That(StreamUtil.IsEndsWith(stream, bytes, 2), Is.EqualTo(expectedResult));
            }
        }
    }
}
