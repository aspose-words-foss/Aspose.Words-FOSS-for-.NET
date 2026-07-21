// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 05/10/2016 by Ilya Navrotskiy

using System.IO;
using Aspose.IO;

namespace Aspose.Fonts.EmbeddedOpenType.Ctf
{
    /// <summary>
    /// Helper class that encapsulates all three binary readers for <see cref="CtfGlyphDecoder"/>.
    /// </summary>
    internal class CtfGlyphDecoderContext
    {
        /// <summary>
        /// ctor.
        /// </summary>
        internal CtfGlyphDecoderContext(MemoryStream glyphTableData, MemoryStream pushData, MemoryStream instructions)
        {
            mGlyphTableReader = new BigEndianBinaryReader(glyphTableData);
            mPushDataReader = new BigEndianBinaryReader(pushData);
            mInstructionsReader = new BigEndianBinaryReader(instructions);
        }

        /// <summary>
        /// Glyph table data reader.
        /// </summary>
        internal BigEndianBinaryReader GlyphTableReader
        {
            get { return mGlyphTableReader; }
        }

        /// <summary>
        /// PushData reader.
        /// </summary>
        internal BigEndianBinaryReader PushDataReader
        {
            get { return mPushDataReader; }
        }

        /// <summary>
        /// Instructions reader.
        /// </summary>
        internal BigEndianBinaryReader InstructionsReader
        {
            get { return mInstructionsReader; }
        }

        private readonly BigEndianBinaryReader mGlyphTableReader;
        private readonly BigEndianBinaryReader mPushDataReader;
        private readonly BigEndianBinaryReader mInstructionsReader;
    }
}
