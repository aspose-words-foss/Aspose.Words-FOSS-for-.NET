// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 06/04/2016 by Konstantin Kornilov, Andrey Noskov

using System.Collections.Generic;
using System.Xml.Serialization;

namespace Aspose.Words.Tests.Fonts
{
#if !JAVA
    // Used only by TestFonts that launched only in .Net by design.
    [XmlInclude(typeof(TrueTypeCollection))]
    [XmlInclude(typeof(OpenTypeFont))]
    [XmlInclude(typeof(FontMetaInfo))]
#endif
    internal abstract class FontFile
    {
        public string Path;
        public string Md5;
        public long Size;
        public bool IsInRegistry;
        public bool IsInFontsFolder;
        public List<FontMetaInfo> Items = new List<FontMetaInfo>();
    }

    internal class TrueTypeCollection : FontFile
    {
    }

    internal class OpenTypeFont : FontFile
    {
        public FontMetaInfo FontInfo
        {
            get { return Items[0]; }
        }
    }
}
