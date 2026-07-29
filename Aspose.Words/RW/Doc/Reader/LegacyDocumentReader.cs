// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 29/05/2024 by Alexey Morozov

using System.IO;
using System.Text;
using Aspose.IO;
using Aspose.Words.Settings;

namespace Aspose.Words.RW.Doc.Reader
{
    internal static class LegacyDocumentReader
    {
        internal static readonly byte[] FilePrefixWord20 = new byte[] { 0xdb, 0xa5, 0x2d, 0x00 };
        internal static readonly byte[] FilePrefixWordForMac50 = new byte[] { 0xfe, 0x37, 0x00, 0x23 };
    }
}
