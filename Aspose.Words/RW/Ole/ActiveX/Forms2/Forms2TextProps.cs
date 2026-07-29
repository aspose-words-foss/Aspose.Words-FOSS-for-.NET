// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 14/11/2018 by Ilya Navrotskiy

using System;
using System.IO;
using Aspose.Words.Drawing.Ole;

namespace Aspose.Words.Forms2
{
    /// <summary>
    /// Implements [MS-OFORMS] 2.3.1 TextProps.
    /// </summary>
    internal static class Forms2TextProps
    {
        /// <summary>
        /// Reads TextProps from a binary reader.
        /// </summary>
        internal static void Read(BinaryReader reader, Forms2Pr pr)
        {
            int version = reader.ReadInt16();
            Debug.Assert(version == Forms2OleControl.Forms2Version);

            reader.ReadInt16(); // cbTextProps
            TextPropsPropMask flags = (TextPropsPropMask)reader.ReadUInt32();

            Forms2PrReader prReader = new Forms2PrReader(reader, pr);
            prReader.Flags = (uint)flags;

            uint cFontName = prReader.ReadData((uint)TextPropsPropMask.FontName, 4);
            prReader.ReadData((uint)TextPropsPropMask.FontEffects, 4, Forms2Attr.FontEffects);
            prReader.ReadData((uint)TextPropsPropMask.FontHeight, 4, Forms2Attr.FontHeight);
            prReader.ReadData((uint)TextPropsPropMask.FontCharSet, 1, Forms2Attr.FontCharSet);
            prReader.ReadData((uint)TextPropsPropMask.FontPitchAndFamily, 1, Forms2Attr.FontPitchAndFamily);
            prReader.ReadData((uint)TextPropsPropMask.ParagraphAlign, 1, Forms2Attr.ParagraphAlign);
            prReader.ReadData((uint)TextPropsPropMask.FontWeight, 2, Forms2Attr.FontWeight);

            prReader.ReadString((uint)TextPropsPropMask.FontName, cFontName, Forms2Attr.FontName);

            prReader.ReadPadding(4);
        }

        /// <summary>
        /// Writes TextProps to a binary writer.
        /// </summary>
        internal static void Write(BinaryWriter writer, Forms2Pr pr)
        {
            int startPos = (int)writer.BaseStream.Position;

            writer.Write(Forms2OleControl.Forms2Version);

            // The size of data and property flag will be calculated and written later.
            writer.Seek(2, SeekOrigin.Current);// cbTextProps
            writer.Seek(4, SeekOrigin.Current);// PropMask

            Forms2PrWriter prWriter = new Forms2PrWriter(writer, pr);
            uint flags = 0;
            byte[] fontName = prWriter.WriteCountOfBytesWithCompressionFlag((string)pr.FetchAttr(Forms2Attr.FontName));
            flags |= prWriter.Write(Forms2Attr.FontEffects, 4) ? (uint)TextPropsPropMask.FontEffects : 0;
            flags |= prWriter.Write(Forms2Attr.FontHeight, 4) ? (uint)TextPropsPropMask.FontHeight : 0;
            flags |= prWriter.Write(Forms2Attr.FontCharSet, 1) ? (uint)TextPropsPropMask.FontCharSet : 0;
            flags |= prWriter.Write(Forms2Attr.FontPitchAndFamily, 1) ? (uint)TextPropsPropMask.FontPitchAndFamily : 0;
            flags |= prWriter.Write(Forms2Attr.ParagraphAlign, 1) ? (uint)TextPropsPropMask.ParagraphAlign : 0;
            flags |= prWriter.Write(Forms2Attr.FontWeight, 2) ? (uint)TextPropsPropMask.FontWeight : 0;

            // Write padding to the end, so the size of written data is divisible by 4.
            prWriter.WritePadding(4);

            // ExtraData block.
            flags |= prWriter.WriteBytes(fontName, 4) ? (uint)TextPropsPropMask.FontName : 0;

            // Write padding to the end, so the size of written data is divisible by 4.
            prWriter.WritePadding(4);

            // Write cbTextProps and PropMask.
            prWriter.WriteDataSizeAndPropMask(startPos, flags);
        }

        /// <summary>
        /// Returns true, if a TextProps in a specified collection has all values set to the defaults.
        /// </summary>
        internal static bool HasAllDefaultValues(Forms2Pr pr)
        {
            return pr.IsDefaultValue(Forms2Attr.FontEffects) &&
                   pr.IsDefaultValue(Forms2Attr.FontHeight) &&
                   pr.IsDefaultValue(Forms2Attr.FontCharSet) &&
                   pr.IsDefaultValue(Forms2Attr.FontPitchAndFamily) &&
                   pr.IsDefaultValue(Forms2Attr.ParagraphAlign) &&
                   pr.IsDefaultValue(Forms2Attr.FontWeight) &&
                   pr.IsDefaultValue(Forms2Attr.FontName);
        }

        internal const string Guid = "afc20920-da4e-11ce-b943-00aa006887b4";

        /// <summary>
        /// Implements [MS-OFORMS] 2.3.2 TextPropsPropMask.
        /// </summary>
        [Flags]
        private enum TextPropsPropMask : uint
        {
            FontName = 0x00000001,
            FontEffects = 0x00000002,
            FontHeight = 0x00000004,
            FontCharSet = 0x00000010,
            FontPitchAndFamily = 0x00000020,
            ParagraphAlign = 0x00000040,
            FontWeight = 0x00000080
        }
    }
}
