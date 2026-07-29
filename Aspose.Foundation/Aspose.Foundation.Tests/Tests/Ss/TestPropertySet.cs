// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 14/08/2013 by Roman Korchagin

using System;
using System.IO;
using System.Text;
using Aspose.Ss;
using Aspose.Ss.Property;
using Aspose.TestFx;
using NUnit.Framework;

namespace Aspose.Tests.Ss
{
    /// <summary>
    /// Test that structured storage property sets can be loaded and saved.
    /// </summary>
    [TestFixture]
    public class TestPropertySet
    {
        [SetUp]
        public void SetUp()
        {
            Directory.CreateDirectory(TestFxUtil.GetInTestOutPath("Ss"));
        }

        /// <summary>
        /// Test how unicode property names and values are read.
        /// </summary>
        [Test]
        public void TestPropertiesReadUnicode()
        {
            using (Stream srcStream = File.OpenRead(TestFxUtil.BuildTestFileName(@"Ss\DocumentSummaryInfoUnicode.bin")))
            {
                PropertySet ps = new PropertySet(srcStream);
                DoTestProperties(ps, 11, 5, "Test String");
            }
        }

        /// <summary>
        /// Test how non unicode property names and values are read.
        /// </summary>
        [Test]
        public void TestPropertiesRead()
        {
            FileSystem fs = new FileSystem(TestFxUtil.BuildTestFileName(@"Ss\TestProperties.doc"));
            DoTestProperties(fs);
        }

        /// <summary>
        /// Test how non unicode property names and values are read and written.
        /// Note that we always write in unicode.
        /// </summary>
        [Test]
        public void TestPropertiesReadWrite()
        {
            FileSystem fs = new FileSystem(TestFxUtil.BuildTestFileName(@"Ss\TestProperties.doc"));

            //Load and save the property set.
            Stream srcStream = fs.Root["\x0005DocumentSummaryInformation"] as Stream;
            PropertySet ps = new PropertySet(srcStream);
            
            MemoryStream dstStream = new MemoryStream();
            ps.Save(dstStream);
            fs.Root["\x0005DocumentSummaryInformation"] = dstStream;

            //Load and check that the property set can be loaded after it was saved by us.
            string outFileName = TestFxUtil.BuildOutFileName(@"Ss\TestProperties.doc", "", "");
            fs.Save(outFileName);
            fs = new FileSystem(outFileName);
            DoTestProperties(fs);
        }

        private void DoTestProperties(FileSystem fs)
        {
            Stream stream = fs.Root["\x0005DocumentSummaryInformation"] as Stream;
            PropertySet ps = new PropertySet(stream);
            DoTestProperties(ps, 13, 6, "Привет");
        }

        private static void DoTestProperties(PropertySet ps, int sect1Count, int sect2Count, string myStringValue)
        {
            Assert.That(ps.Sections.Count, Is.EqualTo(2));

            //These are doc summary info properties
            PropertySetSection sect1 = (PropertySetSection)ps.Sections[0];
            Assert.That(sect1.FmtId, Is.EqualTo(PropertySetSection.FMTID_DocSummaryInfo));
            Assert.That(sect1.Properties.Count, Is.EqualTo(sect1Count));
            Assert.That((string)sect1.Properties.GetById((int)DocSummaryInfoProperty.Company).Value, Is.EqualTo("Test Company"));

            //These are user defined properties
            PropertySetSection sect2 = (PropertySetSection)ps.Sections[1];
            Assert.That(sect2.FmtId, Is.EqualTo(PropertySetSection.FMTID_UserDefined));
            Assert.That(sect2.Properties.Count, Is.EqualTo(sect2Count));
            Assert.That(sect2.Properties.GetByName("MyString").Value, Is.EqualTo(myStringValue));
            Assert.That(sect2.Properties.GetByName("MyInt").Value, Is.EqualTo(123));
            Assert.That((double)sect2.Properties.GetByName("MyDouble").Value, Is.EqualTo(123.45));
            Assert.That(sect2.Properties.GetByName("MyBool").Value, Is.EqualTo(true));
            DateTime d = (DateTime)(sect2.Properties.GetByName("MyDate").Value);
            Assert.That(d, Is.EqualTo(new DateTime(2003, 1, 31, 12, 0, 0)));
        }

        /// <summary>
        /// Just trying to figure out how Guid is saved into byte array, no need to port to Java.
        /// </summary>
        [Test]
        public void TestGuid()
        {
            Guid guid = new Guid("00010203-0405-0607-0809-0a0b0c0d0e0f");
            byte[] x = guid.ToByteArray();
            BinaryReader reader = new BinaryReader(new MemoryStream(x), Encoding.Unicode);
            Assert.That(reader.ReadInt64(), Is.EqualTo(0x0607040500010203));
            Assert.That(reader.ReadInt64(), Is.EqualTo(0x0f0e0d0c0b0a0908));
        }

        /// <summary>
        /// Make sure DateTime.MinValue is written and read from the file okay.
        /// </summary>
        [Test]
        public void TestZeroDateTime()
        {
            FileSystem fs = new FileSystem(TestFxUtil.BuildTestFileName(@"Ss\TestProperties.doc"));

            //Store DateTime.MinValue into a property.
            Stream stream = fs.Root["\x0005DocumentSummaryInformation"] as Stream;
            PropertySet ps = new PropertySet(stream);
            PropertySetSection sect = (PropertySetSection)ps.Sections[1];
            sect.Properties.GetByName("MyDate").Value = DateTime.MinValue;
            stream.Position = 0;
            ps.Save(stream);

            //Read the property again and check what's there.
            stream.Position = 0;
            ps = new PropertySet(stream);
            sect = (PropertySetSection)ps.Sections[1];
            Assert.That(sect.Properties.GetByName("MyDate").Value, Is.EqualTo(DateTime.MinValue));
        }

        /// <summary>
        /// When you open the output file in Excel, you will see a security warning
        /// with digital signature details. But it is also important to go into the 
        /// VBA Editor (Alt + F11), Tools, Digital Signature menu and check the project
        /// is signed there, since I've seen digital signature alright in the warning,
        /// but missing in the VBA Editor.
        /// </summary>
        [Test]
        public void TestExcelSigned()
        {
            FileSystem fs = new FileSystem(TestFxUtil.BuildTestFileName(@"Ss\TestExcelSigned.xls"));
            
            // Read and save the property set.
            Stream stream = fs.Root["\x0005DocumentSummaryInformation"] as Stream;
            PropertySet ps = new PropertySet(stream);
            stream.Position = 0;
            ps.Save(stream);

            // Check the digital signature property is there. The code was not reading VT_BLOB properties.
            PropertySetSection sect = (PropertySetSection)ps.Sections[0];
            Property prop = sect.Properties.GetById(24);
            Assert.That(prop, IsNot.Null());
            Assert.That(((byte[])prop.Value).Length, Is.EqualTo(5508));

            // Here was another problem. It looks like if the section with user properties
            // did not have a dictionary of property names (even if it was empty), 
            // the digital signature was disappearing in the VBA Editor.
            sect = (PropertySetSection)ps.Sections[1];
            byte[] userProps = sect.ToByteArray();
            byte[] goldUserProps = new byte[] 
            {
                0x24, 0x00, 0x00, 0x00,    // length of the section
                0x02, 0x00, 0x00, 0x00,    // count of properties
                0x00, 0x00, 0x00, 0x00,    // id of the dictionary property
                0x18, 0x00, 0x00, 0x00,    // offset of the dictionary property
                0x01, 0x00, 0x00, 0x00,    // id of the code page property
                0x1c, 0x00, 0x00, 0x00,    // offset of the code page property
                0x00, 0x00, 0x00, 0x00,    // the value of the dictionary propertyt (zero property names)
                0x02, 0x00, 0x00, 0x00,    // VT_I2
                0xb0, 0x04, 0x00, 0x00,    // 1200 unicode code page
            };
            ArrayUtil.CheckArraysEqual(userProps, goldUserProps);

            // And just write it out so I can play in MS Excel to see if it recognizes it as signed.
            fs.Save(TestFxUtil.BuildOutFileName(@"Ss\TestExcelSigned.xls", "", ""));
        }
    }
}
