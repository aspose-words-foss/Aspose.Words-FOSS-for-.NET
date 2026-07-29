// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 05/04/2016 by Ilya Navrotskiy

using System;
using Aspose.IO;

namespace Aspose.Fonts.EmbeddedOpenType.Ctf
{
    /// <summary>
    /// Class for reading/writing MTX special data type 255SHORT.
    /// </summary>
    /// <remarks>
    /// See http://www.w3.org/Submission/2008/SUBM-MTX-20080305/#id_255SHORT and.
    /// See http://www.w3.org/Submission/2008/SUBM-MTX-20080305/#glyfTable for more info.
    /// </remarks>
    [CodePorting.Translator.Cs2Cpp.CppConstexpr]
    internal static class Short255
    {
        /// <summary>
        /// Reads 255SHORT value from reader.
        /// </summary>
        internal static short Read(BigEndianBinaryReader reader)
        {
            short value;
            short code = reader.ReadByte();
            if (code == WordCode)
            {
                value = reader.ReadInt16();
            }
            else
            {
                short sign = 1;

                if (code == FlipSignCode)
                {
                    sign = -1;
                    code = reader.ReadByte();
                }

                if (code == OneMoreByteCode1 || code == OneMoreByteCode2)
                {
                    short byte2 = reader.ReadByte();
                    int multiplier = code == OneMoreByteCode1 ? 1 : 2;
                    value = (short)(byte2 + LowestCode * multiplier);
                }
                else
                {
                    value = code;
                }

                value *= sign;
            }

            return value;
        }

        /// <summary>
        /// Writes 255SHORT value to writer.
        /// </summary>
        internal static void Write(short value, BigEndianBinaryWriter writer)
        {
            // Note, that if value is less than (-1*LowestCode),
            // then we are forced to write that value using 3 bytes (FlipSignCode, OneMoreByteCode1/2 and coded value itself).
            // So in this case, seems MS Word simply writes WordCode and value 'as is', that is 3 bytes too.
            if ((Math.Abs((int)value) > (LowestCode * 2 + 255)) || (value <= -LowestCode))
            {
                writer.WriteByte(WordCode);
                writer.WriteInt16(value);
            }
            else
            {
                if (value < 0)
                {
                    writer.WriteByte((byte)FlipSignCode);
                    value *= -1;
                }

                if (value < LowestCode)
                {
                    writer.WriteByte((byte)value);
                }
                else if (value <= (LowestCode + 255))
                {
                    writer.WriteByte(OneMoreByteCode1);
                    writer.WriteByte((byte)(value - LowestCode));
                }
                else
                {
                    writer.WriteByte(OneMoreByteCode2);
                    writer.WriteByte((byte)(value - LowestCode * 2));
                }
            }
        }

        private const byte WordCode = 253;
        private const byte OneMoreByteCode2 = 254;
        private const byte OneMoreByteCode1 = 255;
        
        private const short FlipSignCode = 250;
        private const short LowestCode = 250;
    }
}
