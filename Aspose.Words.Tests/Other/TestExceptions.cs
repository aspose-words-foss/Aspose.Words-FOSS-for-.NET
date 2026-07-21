// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 03/04/2015 by Roman Korchagin

using Newtonsoft.Json;
using NUnit.Framework;

namespace Aspose.Words.Tests.Other
{
    /// <summary>
    /// This contains tests for Aspose.Words exceptions.
    /// </summary>
    [TestFixture]
    public class TestExceptions
    {
        [TestFixtureSetUp]
        public void SetUp()
        {
            TestUtil.SetUpTests();
        }

#if !JAVA // No exception serialization on Java

        /// <summary>
        /// Test binary serialization/deserialization of FileCorruptedException.
        /// </summary>
        [Test]
        public void TestFileCorruptedExceptionBinarySerialization()
        {
            string message = "This is my exception";
            FileCorruptedException exception = new FileCorruptedException(message);
            exception = TestBinarySerialization<FileCorruptedException>(exception);
            Assert.That(exception.Message, Is.EqualTo(message));
        }

        /// <summary>
        /// Test binary serialization/deserialization of UnsupportedFileFormatException.
        /// </summary>
        [Test]
        public void TestUnsupportedFileFormatExceptionBinarySerialization()
        {
            string message = "This is my exception";
            UnsupportedFileFormatException exception = new UnsupportedFileFormatException(message);
            exception = TestBinarySerialization<UnsupportedFileFormatException>(exception);
            Assert.That(exception.Message, Is.EqualTo(message));
        }

        /// <summary>
        /// Test binary serialization/deserialization of IncorrectPasswordException.
        /// </summary>
        [Test]
        public void TestIncorrectPasswordExceptionBinarySerialization()
        {
            string message = "This is my exception";
            IncorrectPasswordException exception = new IncorrectPasswordException(message);
            exception = TestBinarySerialization<IncorrectPasswordException>(exception);
            Assert.That(exception.Message, Is.EqualTo(message));
        }

        /// <summary>
        /// Method serialize and deserialize the object using BinaryFormatter.
        /// </summary>
        private static T TestBinarySerialization<T>(object obj)
        {
            string serialized = JsonConvert.SerializeObject(obj);
            return JsonConvert.DeserializeObject<T>(serialized);
        }
#endif
    }
}
