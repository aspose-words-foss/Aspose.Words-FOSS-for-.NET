// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 02/07/2012 by Alexey Morozov

using System.IO;
using Aspose.IO;

namespace Aspose.Words.RW.Ole
{
    /// <summary>
    /// Implements [MS-OLEDS] 2.2.2.1 MetaFilePresentationObject.
    /// </summary>
    internal class PresentationObjectMetafile : PresentationObject
    {
        internal PresentationObjectMetafile(BinaryReader reader)
        {
            mWidth = reader.ReadInt32();
            mHeight = reader.ReadInt32();

            int dataSize = reader.ReadInt32();

            Debug.Assert(StreamUtil.HasEnoughBytesToRead(reader, dataSize - ReservedLength));// already read 'reserved' 

            mMetafileData = reader.ReadBytes(dataSize - ReservedLength);
        }

        /// <summary>
        /// Creates empty MetaFilePresentationObject.
        /// </summary>
        internal PresentationObjectMetafile(byte[] metafileData)
        {
            mMetafileData = metafileData;
        }

        internal override void Write(BinaryWriter writer)
        {
            writer.Write(OleUtil.OleVersion);
            writer.Write(0x05);
            OleUtil.WriteLengthPrefixedAnsiString(writer, "METAFILEPICT");

            writer.Write(0x00); // Width
            writer.Write(0x00); // Height

            writer.Write(mMetafileData.Length + ReservedLength);
            // Reserved.
            writer.Write(0x00);
            writer.Write(0x00);

            writer.Write(mMetafileData);
        }

        /// <summary>
        /// Contains the width of a metafile in logical units. 
        /// The MM_ANISOTROPIC mapping mode MUST be used to convert the logical units to physical units.
        /// </summary>
        internal int Width
        {
            get { return mWidth; }
        }

        /// <summary>
        /// Contains the height of a metafile in logical units. 
        /// The MM_ANISOTROPIC mapping mode MUST be used to convert the logical units to physical units.
        /// </summary>
        internal int Height
        {
            get { return mHeight; }
        }

        /// <summary>
        /// Windows metafile.
        /// </summary>
        internal byte[] MetafileData
        {
            get { return mMetafileData; }
        }

        private readonly int mWidth;
        private readonly int mHeight;

        private readonly byte[] mMetafileData;
        private const int ReservedLength = 8;
    }
}
