// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 01/11/2011 by Konstantin Kornilov

using System.IO;
using Aspose.Fonts.Sfnt;
using Aspose.Fonts.TrueType;
using Aspose.TestFx;
using NUnit.Framework;

namespace Aspose.Tests.Fonts
{
    [TestFixture]
    public class TestSfnt
    {
        [TestFixtureSetUp]
        public void SetUp()
        {
            TestFxUtil.SetUpTests();
        }

        /// <summary>
        /// Check my checksum calculation works for odd-sized tables.
        /// </summary>
        [Test]
        public void TestChecksum()
        {
            byte[] b1 = new byte[]
            {
                0x01, 0x02, 0x03, 0x04,        // This is big endian 0x01020304
                0x05, 0x06, 0x07, 0x08        // This is big endian 0x05060708
            };

            // This is the sum of the above values.
            Assert.That(SfntTableRecordEntry.CalculateChecksum(b1, 0, 8), Is.EqualTo(0x06080a0c));

            // Also check how checksum works for tables whose length does not divide neatly by 4.
            Assert.That(SfntTableRecordEntry.CalculateChecksum(b1, 0, 7), Is.EqualTo(0x06080a04));
            Assert.That(SfntTableRecordEntry.CalculateChecksum(b1, 0, 6), Is.EqualTo(0x06080304));
            Assert.That(SfntTableRecordEntry.CalculateChecksum(b1, 0, 5), Is.EqualTo(0x06020304));
            Assert.That(SfntTableRecordEntry.CalculateChecksum(b1, 0, 4), Is.EqualTo(0x01020304));
            Assert.That(SfntTableRecordEntry.CalculateChecksum(b1, 0, 3), Is.EqualTo(0x01020300));
        }

#if !NETSTANDARD // SystemPal.GetWindowsFontsFolder() is not available when run on device in Xamarin.Android. Use Platform to exclude this test.
        /// <summary>
        /// Check that the checksum I calculate matches checksum in the file.
        /// </summary>
        [Test]
        public void TestChecksumFont()
        {
            string fontPath = Path.Combine(TestEnvironment.GetWinFontsFolder(), "arialbd.ttf");

            using (Stream stream = File.OpenRead(fontPath))
            {
                SfntReader reader = new SfntReader(stream);
                reader.ReadHeader();

                SfntTableRecordEntry cmap = (SfntTableRecordEntry)reader.TableRecordEntries[OpenTypeTableTag.Cmap];
                byte[] cmapData = reader.ReadTable(OpenTypeTableTag.Cmap);
                Assert.That(SfntTableRecordEntry.CalculateChecksum(cmapData), Is.EqualTo(cmap.Checksum));
            }
        }
#endif
    }
}
