// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 01/11/2011 by Konstantin Kornilov

using System;
using System.IO;
using Aspose.Fonts.EmbeddedOpenType.Ctf;
using Aspose.Fonts.EmbeddedOpenType.LzComp;
using Aspose.IO;

namespace Aspose.Fonts.EmbeddedOpenType
{
    /// <summary>
    /// Allows coding with MicroType Express (MTX) Font Format.
    /// </summary>
    /// <remarks>
    /// See http://www.w3.org/Submission/2008/SUBM-MTX-20080305 for more info.
    /// </remarks>
    internal static class MtxCoder
    {
        /// <summary>
        /// Encodes OpenType font to MTX.
        /// </summary>
        public static byte[] Encode(byte[] openTypeFont)
        {
            using (MemoryStream outStream = new MemoryStream())
            {
                BigEndianBinaryWriter writer = new BigEndianBinaryWriter(outStream);
                // Write MTX version.
                writer.WriteByte(3);
                // Copy limit. This value is not used for a moment.
                writer.WriteUInt24(0xffffff);

                CtfEncodedData ctfEncodedData = CtfEncoder.Encode(openTypeFont);
                if (ctfEncodedData == null)
                    return null;

                byte[] lzcompEncodedFontTables = LzCompEncoder.PackData(ctfEncodedData.FontTable);
                byte[] lzcompEncodedPushData = LzCompEncoder.PackData(ctfEncodedData.PushData);
                byte[] lzcompEncodedInstructions = LzCompEncoder.PackData(ctfEncodedData.Instructions);

                // MTX header is 10 bytes length.
                const int mtxHeaderLength = 10;
                int pushDataOffset = mtxHeaderLength + lzcompEncodedFontTables.Length;
                int instructionsOffset = lzcompEncodedPushData.Length + pushDataOffset;

                writer.WriteUInt24(pushDataOffset);
                writer.WriteUInt24(instructionsOffset);

                writer.WriteBytes(lzcompEncodedFontTables);
                writer.WriteBytes(lzcompEncodedPushData);
                writer.WriteBytes(lzcompEncodedInstructions);

                return outStream.ToArray();
            }
        }

        /// <summary>
        /// Decodes MTX data.
        /// </summary>
        public static byte[] Decode(byte[] data)
        {
            using (MemoryStream stream = new MemoryStream(data))
            {
                BigEndianBinaryReader reader = new BigEndianBinaryReader(stream);

                short version = reader.ReadByte();
                if (version != 3)
                    throw new InvalidOperationException("Unsupported MTX version.");

                reader.ReadUInt24();
                int pushDataOffset = reader.ReadUInt24();
                int instructionsOffset = reader.ReadUInt24();

                if(pushDataOffset > instructionsOffset || instructionsOffset >= data.Length)
                    throw new InvalidOperationException("The MTX data is not valid.");

                byte[] lzcompEncodedFontTables = reader.ReadBytes(pushDataOffset - (int) stream.Position);
                byte[] lzcompEncodedPushData = reader.ReadBytes(instructionsOffset - (int) stream.Position);
                byte[] lzcompEncodedInstructions = reader.ReadBytes(data.Length - (int) stream.Position);

                byte[] lzcompDecodedFontTables = LzCompDecoder.UnpackData(lzcompEncodedFontTables);
                byte[] lzcompDecodedPushData = LzCompDecoder.UnpackData(lzcompEncodedPushData);
                byte[] lzcompDecodedInstructions = LzCompDecoder.UnpackData(lzcompEncodedInstructions);

                return CtfDecoder.Decode(lzcompDecodedFontTables, lzcompDecodedPushData, lzcompDecodedInstructions);
            }
        }
    }
}
