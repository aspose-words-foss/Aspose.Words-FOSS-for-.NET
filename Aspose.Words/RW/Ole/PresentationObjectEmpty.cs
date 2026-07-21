// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 02/07/2012 by Alexey Morozov

using System.IO;

namespace Aspose.Words.RW.Ole
{
    /// <summary>
    /// Represent empty PresentationObject which is used when object has no presentation data.
    /// </summary>
    internal class PresentationObjectEmpty : PresentationObject
    {
        internal override void Write(BinaryWriter writer)
        {
            writer.Write(OleUtil.OleVersion);

            // FormatId value 0x00 means no PresentationData.
            writer.Write(0x00);
        }
    }
}
