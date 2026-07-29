// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 08/10/2015 by Alexey Morozov

using System;
using System.IO;

namespace Aspose.Words.Forms2
{
    internal class SiteClassInfo
    {
        internal SiteClassInfo(BinaryReader reader)
        {
            int version = reader.ReadUInt16();
            Debug.Assert(version == 0x0000);

            uint cbClassTable = reader.ReadUInt16();

            // Ignore data in this class for a while.
            reader.BaseStream.Position += cbClassTable;
        }
    }
}
