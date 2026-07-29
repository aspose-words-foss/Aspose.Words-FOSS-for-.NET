// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 07/10/2015 by Alexey Morozov

using System;
using System.IO;
using Aspose.Words.Forms2;

namespace Aspose.Words.Drawing.Ole
{
    /// <summary>
    /// The MorphDataControl structure is an aggregate of six controls: CheckBox, ComboBox, ListBox, OptionButton, TextBox, and ToggleButton.
    /// </summary>
    public abstract class MorphDataControl : Forms2OleControl
    {
        /// <summary>
        /// Ctor.
        /// </summary>
        protected MorphDataControl(string name) : base(name, new MorphDataControlPr())
        {
        }

        /// <summary>
        /// Reads the control from a binary reader.
        /// </summary>
        internal override void Read(BinaryReader reader)
        {
            int version = reader.ReadInt16();
            Debug.Assert(Forms2Version == version);

            reader.ReadInt16();     // cbMorphData
            MorphDataPropMask1 flags1 = (MorphDataPropMask1)reader.ReadUInt32();
            MorphDataPropMask2 flags2 = (MorphDataPropMask2)reader.ReadUInt32();

            Forms2PrReader prReader = new Forms2PrReader(reader, Pr);
            prReader.Flags = (uint)flags1;

            prReader.ReadData((uint)MorphDataPropMask1.VariousPropertyBits, 4, Forms2Attr.VariousPropertyBits);
            prReader.ReadData((uint)MorphDataPropMask1.BackColor, 4, Forms2Attr.BackgroundColor);
            prReader.ReadData((uint)MorphDataPropMask1.ForeColor, 4, Forms2Attr.ForegroundColor);
            prReader.ReadData((uint)MorphDataPropMask1.MaxLength, 4, Forms2Attr.MaxLength);
            prReader.ReadData((uint)MorphDataPropMask1.BorderStyle, 1, Forms2Attr.BorderStyle);
            prReader.ReadData((uint)MorphDataPropMask1.ScrollBars, 1, Forms2Attr.ScrollBars);
            prReader.ReadData((uint)MorphDataPropMask1.DisplayStyle, 1, Forms2Attr.DisplayStyle);
            prReader.ReadData((uint)MorphDataPropMask1.MousePointer, 1, Forms2Attr.MousePointer);
            prReader.ReadData((uint)MorphDataPropMask1.PasswordChar, 2, Forms2Attr.PasswordChar);
            prReader.ReadData((uint)MorphDataPropMask1.ListWidth, 4, Forms2Attr.ListWidth);
            prReader.ReadData((uint)MorphDataPropMask1.BoundColumn, 2, Forms2Attr.BoundColumn);
            prReader.ReadData((uint)MorphDataPropMask1.TextColumn, 2, Forms2Attr.TextColumn);
            prReader.ReadData((uint)MorphDataPropMask1.ColumnCount, 2, Forms2Attr.ColumnCount);
            prReader.ReadData((uint)MorphDataPropMask1.ListRows, 2, Forms2Attr.ListRows);
            prReader.ReadData((uint)MorphDataPropMask1.ColumnInfo, 2, Forms2Attr.ColumnInfo);
            prReader.ReadData((uint)MorphDataPropMask1.MatchEntry, 1, Forms2Attr.MatchEntry);
            prReader.ReadData((uint)MorphDataPropMask1.ListStyle, 1, Forms2Attr.ListStyle);
            prReader.ReadData((uint)MorphDataPropMask1.ShowDropButtonWhen, 1, Forms2Attr.ShowDropButtonWhen);
            prReader.ReadData((uint)MorphDataPropMask1.DropButtonStyle, 1, Forms2Attr.DropButtonStyle);
            prReader.ReadData((uint)MorphDataPropMask1.MultiSelect, 1, Forms2Attr.MultiSelect);
            uint cValue = prReader.ReadData((uint)MorphDataPropMask1.Value, 4);
            uint cCaption = prReader.ReadData((uint)MorphDataPropMask1.Caption, 4);
            prReader.ReadData((uint)MorphDataPropMask1.PicturePosition, 4, Forms2Attr.PicturePosition);
            prReader.ReadData((uint)MorphDataPropMask1.BorderColor, 4, Forms2Attr.BorderColor);
            prReader.ReadData((uint)MorphDataPropMask1.SpecialEffect, 4, Forms2Attr.SpecialEffect);
            prReader.ReadData((uint)MorphDataPropMask1.MouseIcon, 2, Forms2Attr.MouseIcon);
            prReader.ReadData((uint)MorphDataPropMask1.Picture, 2, Forms2Attr.Picture);
            prReader.ReadData((uint)MorphDataPropMask1.Accelerator, 2, Forms2Attr.Accelerator);

            prReader.Flags = (uint)flags2;
            uint cGroupName = prReader.ReadData((uint)MorphDataPropMask2.GroupName, 4);

            // Extra block.
            prReader.Flags = (uint)flags1;
            prReader.ReadBytes((uint)MorphDataPropMask1.Size, 8, Forms2Attr.Size);
            prReader.ReadString((uint)MorphDataPropMask1.Value, cValue, Forms2Attr.Value);
            prReader.ReadString((uint)MorphDataPropMask1.Caption, cCaption, Forms2Attr.Caption);

            prReader.Flags = (uint)flags2;
            prReader.ReadString((uint)MorphDataPropMask2.GroupName, cGroupName, Forms2Attr.GroupName);

            prReader.ReadPadding(4);

            // Stream data.
            if ((flags1 & MorphDataPropMask1.MouseIcon) != 0)
                prReader.ReadGuidAndPicture(Forms2Attr.MouseIcon);

            if ((flags1 & MorphDataPropMask1.Picture) != 0)
                prReader.ReadGuidAndPicture(Forms2Attr.Picture);

            // TextProps.
            Forms2TextProps.Read(reader, Pr);
        }

        /// <summary>
        /// Writes the control to a binary writer.
        /// </summary>
        internal override uint Write(BinaryWriter writer)
        {
            int startPos = (int)writer.BaseStream.Position;

            writer.Write(Forms2Version);

            // The size of data and property flags will be written later.
            writer.Seek(2, SeekOrigin.Current);// cbMorphData
            writer.Seek(4, SeekOrigin.Current);// MorphDataPropMask1
            writer.Seek(4, SeekOrigin.Current);// MorphDataPropMask2

            Forms2PrWriter prWriter = new Forms2PrWriter(writer, Pr);
            // According to [MS-OFORMS] 2.2.5.2, Reserved bit MUST be set to 1 and MUST be ignored.
            uint flags1 = (uint)MorphDataPropMask1.Reserved;
            flags1 |= prWriter.Write(Forms2Attr.VariousPropertyBits, 4) ? (uint)MorphDataPropMask1.VariousPropertyBits : 0;
            flags1 |= prWriter.Write(Forms2Attr.BackgroundColor, 4) ? (uint)MorphDataPropMask1.BackColor : 0;
            flags1 |= prWriter.Write(Forms2Attr.ForegroundColor, 4) ? (uint)MorphDataPropMask1.ForeColor : 0;
            flags1 |= prWriter.Write(Forms2Attr.MaxLength, 4) ? (uint)MorphDataPropMask1.MaxLength : 0;
            flags1 |= prWriter.Write(Forms2Attr.BorderStyle, 1) ? (uint)MorphDataPropMask1.BorderStyle : 0;
            flags1 |= prWriter.Write(Forms2Attr.ScrollBars, 1) ? (uint)MorphDataPropMask1.ScrollBars : 0;
            flags1 |= prWriter.Write(Forms2Attr.DisplayStyle, 1) ? (uint)MorphDataPropMask1.DisplayStyle : 0;
            flags1 |= prWriter.Write(Forms2Attr.MousePointer, 1) ? (uint)MorphDataPropMask1.MousePointer : 0;
            flags1 |= prWriter.Write(Forms2Attr.PasswordChar, 2) ? (uint)MorphDataPropMask1.PasswordChar : 0;
            flags1 |= prWriter.Write(Forms2Attr.ListWidth, 4) ? (uint)MorphDataPropMask1.ListWidth : 0;
            flags1 |= prWriter.Write(Forms2Attr.BoundColumn, 2) ? (uint)MorphDataPropMask1.BoundColumn : 0;
            flags1 |= prWriter.Write(Forms2Attr.TextColumn, 2) ? (uint)MorphDataPropMask1.TextColumn: 0;
            flags1 |= prWriter.Write(Forms2Attr.ColumnCount, 2) ? (uint)MorphDataPropMask1.ColumnCount : 0;
            flags1 |= prWriter.Write(Forms2Attr.ListRows, 2) ? (uint)MorphDataPropMask1.ListRows : 0;
            flags1 |= prWriter.Write(Forms2Attr.ColumnInfo, 2) ? (uint)MorphDataPropMask1.ColumnInfo : 0;
            flags1 |= prWriter.Write(Forms2Attr.MatchEntry, 1) ? (uint)MorphDataPropMask1.MatchEntry : 0;
            flags1 |= prWriter.Write(Forms2Attr.ListStyle, 1) ? (uint)MorphDataPropMask1.ListStyle : 0;
            flags1 |= prWriter.Write(Forms2Attr.ShowDropButtonWhen, 1) ? (uint)MorphDataPropMask1.ShowDropButtonWhen : 0;
            flags1 |= prWriter.Write(Forms2Attr.DropButtonStyle, 1) ? (uint)MorphDataPropMask1.DropButtonStyle : 0;
            flags1 |= prWriter.Write(Forms2Attr.MultiSelect, 1) ? (uint)MorphDataPropMask1.MultiSelect : 0;

            byte[] value = prWriter.WriteCountOfBytesWithCompressionFlag(Value);
            byte[] caption = prWriter.WriteCountOfBytesWithCompressionFlag(Caption);

            flags1 |= prWriter.Write(Forms2Attr.PicturePosition, 4) ? (uint)MorphDataPropMask1.PicturePosition : 0;
            flags1 |= prWriter.Write(Forms2Attr.BorderColor, 4) ? (uint)MorphDataPropMask1.BorderColor : 0;
            flags1 |= prWriter.Write(Forms2Attr.SpecialEffect, 4) ? (uint)MorphDataPropMask1.SpecialEffect : 0;
            flags1 |= prWriter.Write(Forms2Attr.MouseIcon, 2) ? (uint)MorphDataPropMask1.MouseIcon : 0;
            flags1 |= prWriter.Write(Forms2Attr.Picture, 2) ? (uint)MorphDataPropMask1.Picture : 0;
            flags1 |= prWriter.Write(Forms2Attr.Accelerator, 2) ? (uint)MorphDataPropMask1.Accelerator : 0;
            byte[] groupName = prWriter.WriteCountOfBytesWithCompressionFlag(GroupName);

            // Write padding to the end, so the size of written data is divisible by 4.
            prWriter.WritePadding(4);

            // Extra block.
            flags1 |= prWriter.Write(Forms2Attr.Size, 8) ? (uint)MorphDataPropMask1.Size : 0;

            flags1 |= prWriter.WriteBytes(value, 4) ? (uint)MorphDataPropMask1.Value : 0;
            flags1 |= prWriter.WriteBytes(caption, 4) ? (uint)MorphDataPropMask1.Caption : 0;

            uint flags2 = prWriter.WriteBytes(groupName, 4) ? (uint)MorphDataPropMask2.GroupName : 0;

            // Write padding to the end, so the size of written data is divisible by 4.
            prWriter.WritePadding(4);

            // Write cbMorphData and PropMasks.
            prWriter.WriteDataSizeAndPropMasks(startPos, flags1, flags2);

            // Stream data.
            if (((MorphDataPropMask1)flags1 & MorphDataPropMask1.MouseIcon) != 0)
                prWriter.WriteGuidAndPicture(MouseIcon);

            if (((MorphDataPropMask1)flags1 & MorphDataPropMask1.Picture) != 0)
                prWriter.WriteGuidAndPicture(Picture);

            // TextProps.
            Forms2TextProps.Write(writer, Pr);

            return (uint)(writer.BaseStream.Position - startPos);
        }

        /// <summary>
        /// Gets or sets a type of the MorphDataControl.
        /// </summary>
        internal DisplayStyle DisplayStyle
        {
            get { return (DisplayStyle)Pr.FetchAttr(Forms2Attr.DisplayStyle); }
            set { Pr.SetAttr(Forms2Attr.DisplayStyle, value); }
        }

        /// <summary>
        /// Implements [MS-OFORMS] 2.2.5.2 MorphDataPropMask (first 32 bits).
        /// </summary>
        [Flags]
        private enum MorphDataPropMask1 : uint
        {
            VariousPropertyBits = 0x00000001,
            BackColor = 0x00000002,
            ForeColor = 0x00000004,
            MaxLength = 0x00000008,
            BorderStyle = 0x00000010,
            ScrollBars = 0x00000020,
            DisplayStyle = 0x00000040,
            MousePointer = 0x00000080,
            Size = 0x00000100,
            PasswordChar = 0x00000200,
            ListWidth = 0x00000400,
            BoundColumn = 0x00000800,
            TextColumn = 0x00001000,
            ColumnCount = 0x00002000,
            ListRows = 0x00004000,
            ColumnInfo = 0x00008000,
            MatchEntry = 0x00010000,
            ListStyle = 0x00020000,
            ShowDropButtonWhen = 0x00040000,
            UnusedBits1 = 0x00080000,
            DropButtonStyle = 0x00100000,
            MultiSelect = 0x00200000,
            Value = 0x00400000,
            Caption = 0x00800000,
            PicturePosition = 0x01000000,
            BorderColor = 0x02000000,
            SpecialEffect = 0x04000000,
            MouseIcon = 0x08000000,
            Picture = 0x10000000,
            Accelerator = 0x20000000,
            UnusedBits2 = 0x40000000,
            Reserved = 0x80000000
        }

        /// <summary>
        /// Implements [MS-OFORMS] 2.2.5.2 MorphDataPropMask (last 32 bits).
        /// </summary>
        [Flags]
        private enum MorphDataPropMask2 : uint
        {
            GroupName = 0x00000001
        }
    }
}
