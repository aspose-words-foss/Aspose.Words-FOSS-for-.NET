// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 24/08/2010 by Roman Korchagin

using System.Globalization;
using System.IO;
using System.Text;
using System.Threading;
using System.Xml;
using Aspose.JavaAttributes;
using XmlUnit;

namespace Aspose.TestFx.GoldComparers
{
    /// <summary>
    /// This class is to be ported manually. I am not making it part of the PAL because it is used for tests only.
    /// </summary>
    [JavaManual("Invokes XmlUnit to compare XML files.")]
    public static class XmlComparer
    {
        public static bool CompareXmlStrings(string expectedXml, string actualXml)
        {
            byte[] expectedBytes = Encoding.UTF8.GetBytes(expectedXml);
            byte[] actualBytes = Encoding.UTF8.GetBytes(actualXml);

            return CompareXmlBuffers(expectedBytes, actualBytes);
        }

        public static bool CompareXmlBuffers(byte[] buffer1, byte[] buffer2)
        {
            // RK I have to copy streams because XMLUnit closes them and that's unacceptable.
            MemoryStream dstStream1 = new MemoryStream(buffer1);
            MemoryStream dstStream2 = new MemoryStream(buffer2);

            CultureInfo savedCulture = Thread.CurrentThread.CurrentCulture;
            Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
            try
            {
                DiffConfiguration config = new DiffConfiguration(
                    "test",
                    false,  // Do not use validating parser. Maybe we should?
                    WhitespaceHandling.Significant,
                    false); // Do not ignore attribute order because we strive to write attributes in the same order as MS Word.

                XmlDiff diff = new XmlDiff(new XmlInput(dstStream1), new XmlInput(dstStream2), config);
                DiffResult result = diff.Compare();
                return result.Identical;
            }
            finally
            {
                Thread.CurrentThread.CurrentCulture = savedCulture;
            }
        }
    }
}
