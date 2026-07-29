// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 13/01/2019 by Ilya Navrotskiy

using System;
using System.IO;
using Aspose.Words.Drawing.Ole;

namespace Aspose.Words.Forms2
{
    /// <summary>
    /// Implements [MS-OFORMS] 2.2.10.11.1 DesignExtender.
    /// </summary>
    internal static class Forms2DesignExtender
    {
        /// <summary>
        /// Reads extender data from a binary reader to a specified collection.
        /// </summary>
        internal static void Read(BinaryReader reader, Forms2Pr pr)
        {
            int version = reader.ReadInt16();
            Debug.Assert(version == Forms2OleControl.Forms2Version);

            reader.ReadInt16(); // cbDesignExtender
            DesignExtenderPropMask flags = (DesignExtenderPropMask)reader.ReadUInt32();

            Forms2PrReader prReader = new Forms2PrReader(reader, pr);
            prReader.Flags = (uint)flags;

            prReader.ReadData((uint)DesignExtenderPropMask.BitFlags, 4, Forms2Attr.BitFlagsDX);
            prReader.ReadData((uint)DesignExtenderPropMask.GridX, 4, Forms2Attr.GridX);
            prReader.ReadData((uint)DesignExtenderPropMask.GridY, 4, Forms2Attr.GridY);
            prReader.ReadData((uint)DesignExtenderPropMask.ClickControlMode, 4, Forms2Attr.ClickControlMode);
            prReader.ReadData((uint)DesignExtenderPropMask.DblClickControlMode, 4, Forms2Attr.DblClickControlMode);

            prReader.ReadPadding(4);
        }

        /// <summary>
        /// Writes extender data from a specified collection to a binary writer.
        /// </summary>
        internal static void Write(BinaryWriter writer, Forms2Pr pr)
        {
            int startPos = (int)writer.BaseStream.Position;

            writer.Write(Forms2OleControl.Forms2Version);

            // The size of data and property flag will be calculated and written later.
            writer.Seek(2, SeekOrigin.Current);// cbDesignExtender
            writer.Seek(4, SeekOrigin.Current);// PropMask

            Forms2PrWriter prWriter = new Forms2PrWriter(writer, pr);
            uint flags = 0;
            flags |= prWriter.Write(Forms2Attr.BitFlagsDX, 4) ? (uint)DesignExtenderPropMask.BitFlags : 0;
            flags |= prWriter.Write(Forms2Attr.GridX, 4) ? (uint)DesignExtenderPropMask.GridX : 0;
            flags |= prWriter.Write(Forms2Attr.GridY, 4) ? (uint)DesignExtenderPropMask.GridY : 0;
            flags |= prWriter.Write(Forms2Attr.ClickControlMode, 4) ? (uint)DesignExtenderPropMask.ClickControlMode : 0;
            flags |= prWriter.Write(Forms2Attr.DblClickControlMode, 4) ? (uint)DesignExtenderPropMask.DblClickControlMode : 0;

            // Write padding to the end, so the size of written data is divisible by 4.
            prWriter.WritePadding(4);

            // Write cbDesignExtender and PropMask.
            prWriter.WriteDataSizeAndPropMask(startPos, flags);
        }

        /// <summary>
        /// Returns true, if extender data in a specified collection has all default values.
        /// </summary>
        internal static bool HasAllDefaultValues(Forms2Pr pr)
        {
            return pr.IsDefaultValue(Forms2Attr.BitFlagsDX) &&
                   pr.IsDefaultValue(Forms2Attr.GridX) &&
                   pr.IsDefaultValue(Forms2Attr.GridY) &&
                   pr.IsDefaultValue(Forms2Attr.ClickControlMode) &&
                   pr.IsDefaultValue(Forms2Attr.DblClickControlMode);
        }

        /// <summary>
        /// [MS-OFORMS] 2.2.10.11.2 DesignExtenderPropMask.
        /// </summary>
        [Flags]
        private enum DesignExtenderPropMask : uint
        {
            BitFlags = 0x00000001,
            GridX = 0x00000002,
            GridY = 0x00000004,
            ClickControlMode = 0x00000010,
            DblClickControlMode = 0x00000020,
        }
    }
}
