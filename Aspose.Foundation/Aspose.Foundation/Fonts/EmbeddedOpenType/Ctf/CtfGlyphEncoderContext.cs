// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 05/10/2016 by Ilya Navrotskiy

using System.IO;
using Aspose.IO;

namespace Aspose.Fonts.EmbeddedOpenType.Ctf
{
    /// <summary>
    /// Helper class that encapsulates all three binary writers for <see cref="CtfGlyphEncoder"/>.
    /// </summary>
    internal class CtfGlyphEncoderContext
    {
        /// <summary>
        /// ctor.
        /// </summary>
        internal CtfGlyphEncoderContext(MemoryStream glyphTable, MemoryStream pushData, MemoryStream instructions)
        {
            mGlyphTableWriter = new BigEndianBinaryWriter(glyphTable);
            mPushDataWriter = new BigEndianBinaryWriter(pushData);
            mInstructionsWriter = new BigEndianBinaryWriter(instructions);
        }

        /// <summary>
        /// Glyph table data writer.
        /// </summary>
        internal BigEndianBinaryWriter GlyphTableWriter
        {
            get { return mGlyphTableWriter; }
        }

        /// <summary>
        /// PushData writer.
        /// </summary>
        internal BigEndianBinaryWriter PushDataWriter
        {
            get { return mPushDataWriter; }
        }

        /// <summary>
        /// Instructions writer.
        /// </summary>
        internal BigEndianBinaryWriter InstructionsWriter
        {
            get { return mInstructionsWriter; }
        }

        private readonly BigEndianBinaryWriter mGlyphTableWriter;
        private readonly BigEndianBinaryWriter mPushDataWriter;
        private readonly BigEndianBinaryWriter mInstructionsWriter;
    }
}
