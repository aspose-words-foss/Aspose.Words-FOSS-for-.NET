// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 02/07/2012 by Alexey Morozov

using System;
using System.IO;
using Aspose.IO;
using Aspose.JavaAttributes;

namespace Aspose.Words.RW.Ole
{
    /// <summary>
    /// Base class for OLE1.0 PresentationObject classes.
    /// </summary>
    internal abstract class PresentationObject
    {
        internal static PresentationObject Read(BinaryReader reader)
        {
            if(!StreamUtil.HasEnoughBytesToRead(reader, 8 /* oleVersion + formatId */))
                return new PresentationObjectEmpty();

            // Any arbitrary value.
            reader.ReadInt32(); // int oleVersion = 
            int formatId = reader.ReadInt32();

            // Allowed values are 0x00 (ClassName is absent) and 0x05 (ClassName is present).
            if (formatId == 0)
            {
                // There is no PresentationData but PresentationObject itself is valid.
                return new PresentationObjectEmpty();
            }
            else if(formatId != 0x05)
            {
                // Invalid PresentationObject.
                throw new InvalidOperationException("Unexpected FormatID value, PresentationObject is corrupted.");
            }

            string className = OleUtil.ReadLengthPrefixedAnsiString(reader);

            switch(className)
            {
                case "METAFILEPICT":
                    return new PresentationObjectMetafile(reader);
                default:
                    // WARN. Presentation object of this type is not supported.
                    return new PresentationObjectEmpty();
            }
        }

        [JavaThrows(true)]
        internal abstract void Write(BinaryWriter writer);
    }
}
