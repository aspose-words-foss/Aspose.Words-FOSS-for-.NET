// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 22/02/2013 by Alexey Noskov

using System.IO;
using Aspose.JavaAttributes;

namespace Aspose.Fonts
{
    internal interface IFontBuilder
    {
        int SfntVersion { get; set; }

        void AddTable(string tag, byte[] tableData);

        [JavaThrows(true)]
        byte[] WriteFileToByteArray();

        [JavaThrows(true)]
        void WriteFileToStream(Stream stream);
    }
}
