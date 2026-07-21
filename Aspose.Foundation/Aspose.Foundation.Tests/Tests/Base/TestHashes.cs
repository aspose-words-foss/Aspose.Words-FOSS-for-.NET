// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 14/08/2013 by Roman Korchagin

using System.Text;
using Aspose.Crypto;
using NUnit.Framework;

namespace Aspose.Tests.Base
{
    /// <summary>
    /// A very useful source of test vectors http://www.febooti.com/products/filetweak/members/hash-and-crc/test-vectors/
    /// </summary>
    [TestFixture]
    public class TestHashes
    {
        
        [Test]
        public void TestSha1()
        {
            byte[] hash = HashUtil.ComputeHash(DigestAlgorithm.Sha1, Encoding.ASCII.GetBytes(""));
            Assert.That(StringUtil.BytesToHex(hash).ToLower(), Is.EqualTo("da39a3ee5e6b4b0d3255bfef95601890afd80709"));

            hash = HashUtil.ComputeHash(DigestAlgorithm.Sha1, Encoding.ASCII.GetBytes("The quick brown fox jumps over the lazy dog"));
            Assert.That(StringUtil.BytesToHex(hash).ToLower(), Is.EqualTo("2fd4e1c67a2d28fced849ee1bb76e7391b93eb12"));

            // This is our own test vector to make sure characters bytes greater than 127 are hashed correctly on Java.
            hash = HashUtil.ComputeHash(DigestAlgorithm.Sha1, Encoding.ASCII.GetBytes("test Message тестовое Сообщение"));
            Assert.That(StringUtil.BytesToHex(hash).ToLower(), Is.EqualTo("dbb501dce410b24aff2b4f38ab2687041d561d28"));
        }

        [Test]
        public void TestHashConsistency()
        {
            byte[] hash = HashUtil.ComputeHash(DigestAlgorithm.Sha512, Encoding.ASCII.GetBytes(""));
            Assert.That(StringUtil.BytesToHex(hash).ToLower(), Is.EqualTo("cf83e1357eefb8bdf1542850d66d8007d620e4050b5715dc83f4a921d36ce9ce47d0d13c5d85f2b0ff8318d2877eec2f63b931bd47417a81a538327af927da3e"));

            hash = HashUtil.ComputeHash(DigestAlgorithm.Sha512, Encoding.ASCII.GetBytes("The quick brown fox jumps over the lazy dog"));
            Assert.That(StringUtil.BytesToHex(hash).ToLower(), Is.EqualTo("07e547d9586f6a73f73fbac0435ed76951218fb7d0c8d788a309d785436bbb642e93a252a954f23912547d1e8a3b5ed6e1bfd7097821233fa0538f3db854fee6"));

            // This is our own test vector to make sure characters bytes greater than 127 are hashed correctly on Java.
            hash = HashUtil.ComputeHash(DigestAlgorithm.Sha512, Encoding.ASCII.GetBytes("test Message тестовое Сообщение"));
            Assert.That(StringUtil.BytesToHex(hash).ToLower(), Is.EqualTo("c81521aaf439b65943378b0f9d7bb1a5edded4d2da117cbc7e929c466b577b6868541fd2f8afbd464826105b0434f4dc89f734d00f8e0d420bb553ac68b5743c"));
        }
    }
}
