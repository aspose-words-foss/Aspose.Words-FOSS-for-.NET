// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 08/10/2015 by Alexey Morozov

using System;
using System.IO;
using Aspose.Words.Drawing.Ole;

namespace Aspose.Words.Forms2
{
    /// <summary>
    /// The Label control displays text.
    /// </summary>
    internal class LabelControl : Forms2OleControl
    {
        /// <summary>
        /// Ctor.
        /// </summary>
        internal LabelControl(string name) : base(name, new LabelControlPr())
        {
            Size = mDefaultSize;
        }

        /// <summary>
        /// Reads the control from a binary reader.
        /// </summary>
        internal override void Read(BinaryReader reader)
        {
            int version = reader.ReadInt16();
            Debug.Assert(version == Forms2Version);

            // cbLabel
            reader.ReadUInt16();
            LabelPropMask flags = (LabelPropMask)reader.ReadUInt32();

            Forms2PrReader prReader = new Forms2PrReader(reader, Pr);
            prReader.Flags = (uint) flags;

            // Data block.
            prReader.ReadData((uint)LabelPropMask.ForeColor, 4, Forms2Attr.ForegroundColor);
            prReader.ReadData((uint)LabelPropMask.BackColor, 4, Forms2Attr.BackgroundColor);
            prReader.ReadData((uint)LabelPropMask.VariousPropertyBits, 4, Forms2Attr.VariousPropertyBits);
            uint cCaption = prReader.ReadData((uint)LabelPropMask.Caption, 4);
            prReader.ReadData((uint)LabelPropMask.PicturePosition, 4, Forms2Attr.PicturePosition);
            prReader.ReadData((uint)LabelPropMask.MousePointer, 1, Forms2Attr.MousePointer);
            prReader.ReadData((uint)LabelPropMask.BorderColor, 4, Forms2Attr.BorderColor);
            prReader.ReadData((uint)LabelPropMask.BorderStyle, 2, Forms2Attr.BorderStyle);
            prReader.ReadData((uint)LabelPropMask.SpecialEffect, 2, Forms2Attr.SpecialEffect);
            prReader.ReadData((uint)LabelPropMask.Picture, 2, Forms2Attr.Picture);
            prReader.ReadData((uint)LabelPropMask.Accelerator, 2, Forms2Attr.Accelerator);
            prReader.ReadData((uint)LabelPropMask.MouseIcon, 2, Forms2Attr.MouseIcon);

            // Extra data block.
            prReader.ReadString((uint)(flags & LabelPropMask.Caption), cCaption, Forms2Attr.Caption);
            prReader.ReadBytes((uint)(flags & LabelPropMask.Size), 8, Forms2Attr.Size);

            prReader.ReadPadding(4);

            // Stream data.
            if ((flags & LabelPropMask.Picture) != 0)
                prReader.ReadGuidAndPicture(Forms2Attr.Picture);
            if ((flags & LabelPropMask.MouseIcon) != 0)
                prReader.ReadGuidAndPicture(Forms2Attr.MouseIcon);

            // Text props.
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
            writer.Seek(2, SeekOrigin.Current);// cbLabel
            writer.Seek(4, SeekOrigin.Current);// LabelPropMask

            Forms2PrWriter prWriter = new Forms2PrWriter(writer, Pr);

            uint flags = 0;
            flags |= prWriter.Write(Forms2Attr.ForegroundColor, 4) ? (uint)LabelPropMask.ForeColor : 0;
            flags |= prWriter.Write(Forms2Attr.BackgroundColor, 4) ? (uint)LabelPropMask.BackColor : 0;
            flags |= prWriter.Write(Forms2Attr.VariousPropertyBits, 4) ? (uint)LabelPropMask.VariousPropertyBits : 0;

            byte[] caption = prWriter.WriteCountOfBytesWithCompressionFlag(Caption);

            flags |= prWriter.Write(Forms2Attr.PicturePosition, 4) ? (uint)LabelPropMask.PicturePosition : 0;
            flags |= prWriter.Write(Forms2Attr.MousePointer, 1) ? (uint)LabelPropMask.MousePointer : 0;
            flags |= prWriter.Write(Forms2Attr.BorderColor, 4) ? (uint)LabelPropMask.BorderColor : 0;
            flags |= prWriter.Write(Forms2Attr.BorderStyle, 2) ? (uint)LabelPropMask.BorderStyle : 0;
            flags |= prWriter.Write(Forms2Attr.SpecialEffect, 2) ? (uint)LabelPropMask.SpecialEffect : 0;
            flags |= prWriter.Write(Forms2Attr.Picture, 2) ? (uint)LabelPropMask.Picture : 0;
            flags |= prWriter.Write(Forms2Attr.Accelerator, 2) ? (uint)LabelPropMask.Accelerator : 0;
            flags |= prWriter.Write(Forms2Attr.MouseIcon, 2) ? (uint)LabelPropMask.MouseIcon : 0;

            // Write padding to the end, so the size of written data is divisible by 4.
            prWriter.WritePadding(4);

            // Extra data block.
            flags |= prWriter.WriteBytes(caption, 4) ? (uint)LabelPropMask.Caption : 0;
            flags |= prWriter.Write(Forms2Attr.Size, 8) ? (uint)LabelPropMask.Size : 0;

            // Write padding to the end, so the size of written data is divisible by 4.
            prWriter.WritePadding(4);

            // Write cbLabel and PropMask.
            prWriter.WriteDataSizeAndPropMask(startPos, flags);

            // Stream data.
            if (((LabelPropMask)flags & LabelPropMask.Picture) != 0)
                prWriter.WriteGuidAndPicture(Picture);
            if (((LabelPropMask)flags & LabelPropMask.MouseIcon) != 0)
                prWriter.WriteGuidAndPicture(MouseIcon);

            // Text props.
            Forms2TextProps.Write(writer, Pr);

            return (uint)(writer.BaseStream.Position - startPos);
        }

        /// <summary>
        /// Gets type of Forms 2.0 control.
        /// </summary>
        public override Forms2OleControlType Type
        {
            get { return Forms2OleControlType.Label; }
        }

        /// <summary>
        /// Gets ClsidCacheIndex.
        /// </summary>
        internal override ClsidCacheIndex ClsidCacheIndex
        {
            get { return ClsidCacheIndex.Label; }
        }

        /// <summary>
        /// Gets a Clsid of the embedded object.
        /// </summary>
        protected override string ClsidVirtual
        {
            get { return LabelControlClsid; }
        }

        internal string FontName
        {
            get { return (string)Pr.FetchAttr(Forms2Attr.FontName); }
            set { Pr.SetAttr(Forms2Attr.FontName, value); }
        }

        internal uint FontHeight
        {
            get { return (uint)Pr.FetchAttr(Forms2Attr.FontHeight); }
            set { Pr.SetAttr(Forms2Attr.FontHeight, value); }
        }

        internal FontEffects FontEffects
        {
            get { return (FontEffects)Pr.FetchAttr(Forms2Attr.FontEffects); }
            set { Pr.SetAttr(Forms2Attr.FontEffects, value); }
        }

        internal ParagraphAlign ParagraphAlign
        {
            get { return (ParagraphAlign)Pr.FetchAttr(Forms2Attr.ParagraphAlign); }
            set { Pr.SetAttr(Forms2Attr.ParagraphAlign, value); }
        }

        /// <summary>
        /// Implements [MS-OFORMS] 2.2.4.2 LabelPropMask.
        /// </summary>
        [Flags]
        private enum LabelPropMask : uint
        {
            ForeColor = 0x00000001,
            BackColor = 0x00000002,
            VariousPropertyBits = 0x00000004,
            Caption = 0x00000008,
            PicturePosition = 0x00000010,
            Size = 0x00000020,
            MousePointer = 0x00000040,
            BorderColor = 0x00000080,
            BorderStyle = 0x00000100,
            SpecialEffect = 0x00000200,
            Picture = 0x00000400,
            Accelerator = 0x00000800,
            MouseIcon = 0x00001000
        }

        private readonly OleSize mDefaultSize = OleSize.FromPoints(72, 18);
    }
}
