// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 14/08/2013 by Roman Korchagin

using System;
using System.IO;
using System.Text;
using Aspose.Ss;
using Aspose.TestFx;
using NUnit.Framework;

namespace Aspose.Tests.Ss
{
    [TestFixture]
    public class TestFileSystem
    {
        [SetUp]
        public void SetUp()
        {
            Directory.CreateDirectory(TestFxUtil.GetInTestOutPath("Ss"));
        }

        [Test]
        public void TestReadHeader()
        {
            using (Stream s = File.OpenRead(TestFxUtil.BuildTestFileName(@"Ss\TestSmall.doc")))
            {
                Header header = new Header(new BinaryReader(s, Encoding.Unicode));
                Assert.That(header.CsectDif, Is.EqualTo(0));
                Assert.That(header.CsectDir, Is.EqualTo(0));
                Assert.That(header.CsectFat, Is.EqualTo(1));
                Assert.That(header.CsectMiniFat, Is.EqualTo(1));
                Assert.That(header.MajorVersion, Is.EqualTo((ushort)3));
                Assert.That(header.SectDifStart, Is.EqualTo(0xfffffffe));    //End of chain
                Assert.That(header.SectDirStart, Is.EqualTo((uint)17));            //Looked up in the file.
                Assert.That(header.SectMiniFatStart, Is.EqualTo((uint)18));        //Looked up in the file.
            }
        }

        [Test]
        public void TestReadSmallDifat()
        {
            using (Stream s = File.OpenRead(TestFxUtil.BuildTestFileName(@"Ss\TestSmall.doc")))
            {
                SectCollection difat = Difat.Read(s, 1, FatEntryType.EndOfChain, 0);
                Assert.That(difat.Count, Is.EqualTo(1));    //There is only one FAT sector in this small file.
                Assert.That(difat[0], Is.EqualTo((uint)16));        //Looked up in the file.
            }
        }

        [Test]
        public void TestReadBigDifat()
        {
            using (Stream s = File.OpenRead(TestFxUtil.BuildTestFileName(@"Ss\TestBig.doc")))
            {
                Header header = new Header(new BinaryReader(s, Encoding.Unicode));
                Assert.That(header.CsectFat, Is.EqualTo(207));            //Looked up in the file. There are 207 FAT sectors.
                Assert.That(header.SectDifStart, Is.EqualTo((uint)26180));    //Looked up in the file. This is where first DIFAT sector starts.
                Assert.That(header.CsectDif, Is.EqualTo(1));            //There is only one DIFAT sector.
                
                SectCollection difat = Difat.Read(s, header.CsectFat, header.SectDifStart, header.CsectDif);
                //109 DIFAT entries in the header plus 127 DIFAT entries in the DIFAT sector 
                //(1 last entry out of 128 is used for chaining DIFAT sectors.
                Assert.That(difat.Count, Is.EqualTo(236));    
                //All DIFAT sectors starting from 207 are not used (free).
                Assert.That(difat[(uint)206] != FatEntryType.FreeSect, Is.True);   // Cast to uint to allow autoporting to Java.
                Assert.That(difat[(uint)207] == FatEntryType.FreeSect, Is.True);
            }
        }

        [Test]
        public void TestMinimalEmpty()
        {
            string fileName = TestFxUtil.BuildOutFileName(@"Ss\TestMinimalEmpty.bin", "", "");
            Guid guid = new Guid("{00112233-4455-6677-8899-aabbccddeeff}");

            // Write an empty document.
            {
                FileSystem fs = new FileSystem(guid);
                fs.Save(fileName);
            }

            // Read the document and check.
            {
                FileSystem fs = new FileSystem(fileName);
                MemoryStorage root = fs.Root;
                Assert.That(root.Clsid, Is.EqualTo(guid));
                Assert.That(root.Count, Is.EqualTo(0));     // There are no streams in the empty file.
            }
        }

        [Test]
        public void TestMinimalStream()
        {
            string fileName = TestFxUtil.BuildOutFileName(@"Ss\TestMinimalStream.bin", "", "");
            const string helloWorld = "Hello World!";

            // Write a document with just one stream and a string in it.
            {
                MemoryStream stream = new MemoryStream();
                byte[] data = Encoding.UTF8.GetBytes(helloWorld);
                stream.Write(data, 0, data.Length);

                FileSystem fs = new FileSystem(Guid.Empty);
                fs.Root.Add("MyStream", stream);
                fs.Save(fileName);
            }

            // Read the document and check.
            {
                FileSystem fs = new FileSystem(fileName);
                MemoryStream stream = fs.Root.FetchStream("MyStream");
                string s = Encoding.UTF8.GetString(stream.GetBuffer(), 0, (int)stream.Length);
                Assert.That(s, Is.EqualTo(helloWorld));

            }
        }

        /// <summary>
        /// Read, write and read again a small file to see if we can eat our own output.
        /// </summary>
        [Test]
        public void TestSmallFile()
        {
            FileSystem fs = new FileSystem(TestFxUtil.BuildTestFileName(@"Ss\TestSmall.doc"));
            string outFileName = TestFxUtil.BuildOutFileName(@"Ss\TestSmall.doc", "", "");
            fs.Save(outFileName);
            fs = new FileSystem(outFileName);

            //There are only 3 streams in this document.
            Assert.That(fs.Root.Count, Is.EqualTo(3));

            MemoryStream doc = fs.Root["WordDocument"] as MemoryStream;
            Assert.That(doc.Length, Is.EqualTo(0x1000L));
            
            MemoryStream table = fs.Root["1Table"] as MemoryStream;
            Assert.That(table.Length, Is.EqualTo(0x1000L));
            
            MemoryStream obj = fs.Root["\x0001CompObj"] as MemoryStream;
            Assert.That(obj.Length, Is.EqualTo(0x6aL));
        }

        [Test]
        public void TestBigFile()
        {
            FileSystem fs = new FileSystem(TestFxUtil.BuildTestFileName(@"Ss\TestBig.doc"));
            string outFileName = TestFxUtil.BuildOutFileName(@"Ss\TestBig.doc", "", "");
            fs.Save(outFileName);
            fs = new FileSystem(outFileName);

            //7 objects at this level.
            Assert.That(fs.Root.Count, Is.EqualTo(7));

            MemoryStream data = fs.Root["Data"] as MemoryStream;
            Assert.That(data.Length, Is.EqualTo(0xcc4498L));    //This is a long stream that contains all the images.

            MemoryStorage macros = fs.Root["Macros"] as MemoryStorage;
            Assert.That(macros.Count, Is.EqualTo(3));

            MemoryStorage vba = macros["VBA"] as MemoryStorage;
            Assert.That(vba.Count, Is.EqualTo(5));

            //For some reason it is important that the stream called _VBA_PROJECT appears as a last child,
            //otherwise enumeration does not work in Win32 structured storage.
            MemoryStream vbaProject = vba.GetByIndex(4) as MemoryStream;
            Assert.That(vbaProject.Length, Is.EqualTo(0x0a09L));
        }

        /// <summary>
        /// Used to be a problem with a non zero length string written after a zero length stream.
        /// </summary>
        [Test]
        public void TestZeroLength()
        {
            FileSystem srcFs = new FileSystem(Guid.Empty);
            
            //This is the zero length stream.
            MemoryStream srcA = new MemoryStream();    
            srcFs.Root["A"] = srcA;

            //Non zero length stream that follows the zero length one. 
            //Need to write more than one length of mini fat sector (more than 64 bytes).
            MemoryStream srcB = new MemoryStream();
            for (int i = 0; i < 100; i++)
                srcB.WriteByte((byte)i);
            srcFs.Root["B"] = srcB;

            //Save the complete file system.
            MemoryStream dstStream = new MemoryStream();
            srcFs.Save(dstStream);

            //Load the file system from the created ss.
            dstStream.Position = 0;
            FileSystem dstFs = new FileSystem(dstStream);

            //Check the streams loaded okay.
            MemoryStream dstA = dstFs.Root["A"] as MemoryStream;
            Assert.That(dstA.Length, Is.EqualTo(0L));

            MemoryStream dstB = dstFs.Root["B"] as MemoryStream;
            Assert.That(dstB.Length, Is.EqualTo(100L));
            for (int i = 0; i < 100; i++)
                Assert.That(dstB.ReadByte(), Is.EqualTo(i));
        }

        /// <summary>
        /// Test a file with an OLE2 object embedded.
        /// </summary>
        [Test]
        public void TestOle2()
        {
            FileSystem fs = new FileSystem(TestFxUtil.BuildTestFileName(@"Ss\TestOle2.doc"));
            string outFileName = TestFxUtil.BuildOutFileName(@"Ss\TestOle2.doc", "", "");
            fs.Save(outFileName);
            fs = new FileSystem(outFileName);

            MemoryStorage objPool = fs.Root["ObjectPool"] as MemoryStorage;
            MemoryStorage obj = objPool["_1157519979"] as MemoryStorage;    //This is object id in the storage.

            //It is important these streams are sorted properly, otherwise the file has problems in Win32 SS.
            Assert.That(obj.IndexOfKey("\x0003PRINT"), Is.EqualTo(0));
            Assert.That(obj.IndexOfKey("\x0001CompObj"), Is.EqualTo(1));
            Assert.That(obj.IndexOfKey("\x0003ObjInfo"), Is.EqualTo(2));
            Assert.That(obj.IndexOfKey("\x0003OCXNAME"), Is.EqualTo(3));
            Assert.That(obj.IndexOfKey("contents"), Is.EqualTo(4));
        }

        /// <summary>
        /// Reported by Laurence, trouble for 111 fat sectors. 
        /// Looks like my code was writing one less fat sector than indicated.
        /// </summary>
        [Test]
        public void TestBigExcel()
        {
            FileSystem fs = new FileSystem(TestFxUtil.BuildTestFileName(@"Ss\TestBigExcel.xls"));
            //Fat itself uses 111 sectors from 14070 to 14080
            Assert.That(fs.Fat[14070], Is.EqualTo(FatEntryType.FatSect));
            Assert.That(fs.Fat[14080], Is.EqualTo(FatEntryType.FatSect));

            string outFileName = TestFxUtil.BuildOutFileName(@"Ss\TestBigExcel.xls", "", "");
            fs.Save(outFileName);

            //Load out own file.
            fs = new FileSystem(TestFxUtil.BuildTestFileName(outFileName));
            //Fat should have the same position, but used to end one position too early.
            Assert.That(fs.Fat[14070], Is.EqualTo(FatEntryType.FatSect));
            Assert.That(fs.Fat[14080], Is.EqualTo(FatEntryType.FatSect));
        }

        /// <summary>
        /// Used to go into infinite loop, but now reads okay.
        /// Looks like does not have "end of chain" marker in the fat.
        /// </summary>
        [Test]
        public void TestInvalid1()
        {
            new FileSystem(TestFxUtil.BuildTestFileName(@"Ss\TestInvalid1.xls"));
        }

        /// <summary>
        /// WORDSNET-1016 Aspose generated documents sometimes are handled incorrectly by MS Outlook.
        /// </summary>
        [Test]
        public void TestDefect1016()
        {
            // Although this file opens in MS Excel okay, if you send this file via Outlook,
            // the file that you will receive is likely to be corrupted.
            // This happens because the file has FAT that is not entirely correct and Outlook
            // modifies it just a little bit to update some properties in the document.
            FileSystem fs = new FileSystem(TestFxUtil.BuildTestFileName(@"Ss\TestDefect1016.xls"));
            Assert.That(fs.Fat.Count, Is.EqualTo(256));
            // This is actually where the problem is visible. 
            Assert.That(fs.Fat[0x80], Is.EqualTo(0x01));
            Assert.That(fs.Fat[0x81], Is.EqualTo(0x02));

            // Saving actually corrects the file.
            string outFileName = TestFxUtil.BuildOutFileName(@"Ss\TestDefect1016.xls", "", "");
            fs.Save(outFileName);

            // Load out own file.
            fs = new FileSystem(TestFxUtil.BuildTestFileName(outFileName));
            Assert.That(fs.Fat.Count, Is.EqualTo(256));
            Assert.That(fs.Fat[0x80], Is.EqualTo(FatEntryType.FatSect));
            Assert.That(fs.Fat[0x81], Is.EqualTo(FatEntryType.FatSect));
            Assert.That(fs.Fat[0x82], Is.EqualTo(FatEntryType.FreeSect));
        }

        /// <summary>
        /// WORDSNET-1045 Document hangs up Aspose.Words when opened.
        /// Reported by Salman Safraz: Document causes hang-up when opened by Aspose.Words. 
        /// Caused by invalid directory in the structured storage file, made resilient.
        /// </summary>
        [Test]
        public void TestDefect1045()
        {
            FileSystem fs = new FileSystem(TestFxUtil.BuildTestFileName(@"Ss\TestDefect1045.doc"));
            fs.Save(TestFxUtil.BuildOutFileName(@"Ss\TestDefect1045.doc", "", ""));
        }

        /// <summary>
        /// WORDSNET-1048 Opening document in Aspose.Words fails with 'Unable to read beyond the end of the stream.' exception.
        /// The structures storage file is 1 byte shorter than it should be.
        /// Forum thread:
        ///   'Exceptions when trying to open mail merge document'
        ///   https://www.aspose.com/Community/forums/thread/50346.aspx
        ///   Openning document in Aspose fails with 'Unable to read beyond the end of the stream.' exception.
        /// </summary>
        [Test]
        public void TestDefect1048()
        {
            FileSystem fs = new FileSystem(TestFxUtil.BuildTestFileName(@"Ss\TestDefect1048.doc"));
            fs.Save(TestFxUtil.BuildOutFileName(@"Ss\TestDefect1048.doc", "", ""));
        }

        /// <summary>
        /// WORDSNET-1049 Opening document in Aspose.Words fails with 'Object reference not set to an instance of an object.' exception.
        /// The problem was the file was shorter by 1 byte. Made our code resilient.
        /// Forum thread:
        ///   'Exceptions when trying to open mail merge document'
        ///   https://www.aspose.com/Community/forums/thread/50346.aspx
        ///   Openning document in Aspose fails with 'Object reference not set to an instance of an object.' exception.
        /// </summary>
        [Test]
        public void TestDefect1049()
        {
            FileSystem fs = new FileSystem(TestFxUtil.BuildTestFileName(@"Ss\TestDefect1049.doc"));
            fs.Save(TestFxUtil.BuildOutFileName(@"Ss\TestDefect1049.doc", "", ""));
        }

        /// <summary>
        /// RK Used to throw because I was skipping EntryType.Invalid directory entries. 
        /// </summary>
        [Test]
        public void TestDirEntryTypeInvalid()
        {
            FileSystem fs = new FileSystem(TestFxUtil.BuildTestFileName(@"Ss\TestDirEntryTypeInvalid.doc"));
            fs.Save(TestFxUtil.BuildOutFileName(@"Ss\TestDirEntryTypeInvalid.doc", "", ""));
        }
    }
}
