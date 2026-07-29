// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 05/04/2016 by Ilya Navrotskiy

using Aspose.IO;

namespace Aspose.Fonts.EmbeddedOpenType.Ctf
{
    /// <summary>
    /// Class for reading/writing MTX special data type 255USHORT.
    /// </summary>
    /// <remarks>
    /// See http://www.w3.org/Submission/2008/SUBM-MTX-20080305/#id_255USHORT and.
    /// See http://www.w3.org/Submission/2008/SUBM-MTX-20080305/#glyfTable for more info.
    /// </remarks>
    internal class UShort255
    {
        /// <summary>
        /// No ctor.
        /// </summary>
        private UShort255()
        {
        }

        /// <summary>
        /// Reads 255USHORT value from reader.
        /// </summary>
        internal static int Read(BigEndianBinaryReader reader)
        {
            short code = reader.ReadByte();

            switch (code)
            {
                case WordCode:
                    return reader.ReadUInt16();
                case OneMoreByteCode1:
                    return reader.ReadByte() + LowestUCode;
                case OneMoreByteCode2:
                    return reader.ReadByte() + LowestUCode * 2;
                default:
                    return code;
            }
        }

        /// <summary>
        /// Writes 255USHORT value to writer.
        /// </summary>
        internal static void Write(int value, BigEndianBinaryWriter writer)
        {
            if (value < LowestUCode)
            {
                writer.WriteByte((byte)value);
            }
            else if (value <= (LowestUCode + 255))
            {
                writer.WriteByte((byte) OneMoreByteCode1);
                writer.WriteByte((byte)(value - LowestUCode));
            }
            else if (value <= (LowestUCode * 2 + 255))
            {
                writer.WriteByte((byte) OneMoreByteCode2);
                writer.WriteByte((byte)(value - LowestUCode * 2));
            }
            else
            {
                writer.WriteByte((byte) WordCode);
                writer.WriteUInt16(value);
            }
        }

        // Originally was byte but made short for java, since java's byte is signed so 253 is -3 etc.
        private const short WordCode = 253;
        private const short OneMoreByteCode2 = 254;
        private const short OneMoreByteCode1 = 255;
        private const short LowestUCode = 253;
    }
}
