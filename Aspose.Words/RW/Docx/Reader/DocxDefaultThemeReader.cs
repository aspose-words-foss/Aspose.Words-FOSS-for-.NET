// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 02/02/2017 by Alexey Butalov

using System.IO;
using Aspose.IO;
using Aspose.OpcPackaging;
using Aspose.Words.Themes;
using CodePorting.Translator.Cs2Cpp;

namespace Aspose.Words.RW.Docx.Reader
{
    internal class DocxDefaultThemeReader : IDefaultThemeProvider
    {
        /// <summary>
        /// Reads default theme from the Aspose.Words.Resources.theme1.xml
        /// </summary>
#if CPLUSPLUS
        // TSh: In the CPP, for the singleton pattern, double-checked locking is not needed:
        // If control enters the declaration concurrently while the variable is being initialized,
        // the concurrent execution shall wait for completion of the initialization.
        // 6.7 [stmt.dcl] p4
        // https://en.wikipedia.org/wiki/Double-checked_locking#Usage_in_C++11
        [CppStaticVariable("defaultTheme")]
        public Theme GetDefaultTheme()
        {
            Theme defaultTheme = ReadDefaultTheme();
            return defaultTheme;
        }
#else
        public Theme GetDefaultTheme()
        {
            if (gDefaultTheme == null)
            {
                lock (gDefaultThemeLockObject)
                {
                    if (gDefaultTheme == null)
                    {
                        gDefaultTheme = ReadDefaultTheme();
                    }
                }
            }
            return gDefaultTheme;
        }
#endif

        private static Theme ReadDefaultTheme()
        {
            using (Stream stream = ResourceUtil.FetchResourceStream(ResourceName))
            {
                stream.Position = 0;
                DocxXmlReader xmlReader = new DocxXmlReader(stream, new OoxmlComplianceInfo());
                DocxDocumentReaderBase reader = DocxReaderFactory.CreateDocumentReader(GetDocxDocumentReaderStub());
                reader.PushPartReader(xmlReader);
                return DocxThemeReader.ReadThemeXml((DocxDocumentReader)reader);
            }
        }

        private static OpcPackagePart GetDocxDocumentReaderStub()
        {
            OpcPackagePart documentPartStub = new OpcPackagePart("document.xml", "test");
            documentPartStub.Stream = new MemoryStream();
            StreamUtil.WriteAnsiStringToStream("<document />", documentPartStub.Stream);
            documentPartStub.Stream.Position = 0;
            return documentPartStub;
        }

        [CppSkipEntity("C++ doesn't need double-checked locking")]
        private static Theme gDefaultTheme;
        private static readonly object gDefaultThemeLockObject = new object();
        private const string ResourceName = "Aspose.Words.Resources.theme1.xml";
    }
}
