// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 07/10/2015 by Alexey Morozov

using System;
using System.IO;
using Aspose.Words.Forms2;

namespace Aspose.Words.Drawing.Ole
{
    /// <summary>
    /// The CommandButton control runs a macro that performs an action when a user clicks it.
    /// </summary>
    public class CommandButtonControl : Forms2OleControl
    {
        /// <summary>
        /// Initializes a new instance of <see cref="CommandButtonControl"/> class.
        /// </summary>
        public CommandButtonControl() : this("CommandButton")
        {
            // Mimic Word by setting the same caption alignment.
            ParagraphAlign = ParagraphAlign.Center;
        }

        /// <summary>
        /// Ctor.
        /// </summary>
        internal CommandButtonControl(string name) : base(name)
        {
            Size = mDefaultSize;
        }

        /// <summary>
        /// Reads the control from a binary reader.
        /// </summary>
        internal override void Read(BinaryReader reader)
        {
            int version = reader.ReadUInt16();
            Debug.Assert(version == Forms2Version);

            reader.ReadUInt16();     // cbCommandButton
            CommandButtonPropMask flags = (CommandButtonPropMask)reader.ReadUInt32();

            Forms2PrReader prReader = new Forms2PrReader(reader, Pr);
            prReader.Flags = (uint) flags;

            // Data block.
            prReader.ReadData((uint)CommandButtonPropMask.ForeColor, 4, Forms2Attr.ForegroundColor);
            prReader.ReadData((uint)CommandButtonPropMask.BackColor, 4, Forms2Attr.BackgroundColor);
            prReader.ReadData((uint)CommandButtonPropMask.VariousPropertyBits, 4, Forms2Attr.VariousPropertyBits);
            uint cCaption = prReader.ReadData((uint)CommandButtonPropMask.Caption, 4);
            prReader.ReadData((uint)CommandButtonPropMask.PicturePosition, 4, Forms2Attr.PicturePosition);
            prReader.ReadData((uint)CommandButtonPropMask.MousePointer, 1, Forms2Attr.MousePointer);
            prReader.ReadData((uint)CommandButtonPropMask.Picture, 2, Forms2Attr.Picture);
            prReader.ReadData((uint)CommandButtonPropMask.Accelerator, 2, Forms2Attr.Accelerator);
            prReader.ReadData((uint)CommandButtonPropMask.MouseIcon, 2, Forms2Attr.MouseIcon);

            Pr.SetAttrIfNotDefault(Forms2Attr.TakeFocusOnClick, ((flags & CommandButtonPropMask.TakeFocusOnClick) == 0));

            // Extra block.
            prReader.ReadString((uint)CommandButtonPropMask.Caption, cCaption, Forms2Attr.Caption);
            prReader.ReadBytes((uint)CommandButtonPropMask.Size, 8, Forms2Attr.Size);

            prReader.ReadPadding(4);

            // Stream data.
            if ((flags & CommandButtonPropMask.Picture) != 0)
                prReader.ReadGuidAndPicture(Forms2Attr.Picture);
            if ((flags & CommandButtonPropMask.MouseIcon) != 0)
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
            writer.Seek(2, SeekOrigin.Current);// cbCommandButton
            writer.Seek(4, SeekOrigin.Current);// PropMask

            Forms2PrWriter prWriter = new Forms2PrWriter(writer, Pr);

            uint flags = 0;
            flags |= prWriter.Write(Forms2Attr.ForegroundColor, 4) ? (uint)CommandButtonPropMask.ForeColor : 0;
            flags |= prWriter.Write(Forms2Attr.BackgroundColor, 4) ? (uint)CommandButtonPropMask.BackColor : 0;
            flags |= prWriter.Write(Forms2Attr.VariousPropertyBits, 4) ? (uint)CommandButtonPropMask.VariousPropertyBits : 0;

            byte[] caption = prWriter.WriteCountOfBytesWithCompressionFlag(Caption);

            flags |= prWriter.Write(Forms2Attr.PicturePosition, 4) ? (uint)CommandButtonPropMask.PicturePosition : 0;
            flags |= prWriter.Write(Forms2Attr.MousePointer, 1) ? (uint)CommandButtonPropMask.MousePointer : 0;
            flags |= prWriter.Write(Forms2Attr.Picture, 2) ? (uint)CommandButtonPropMask.Picture : 0;
            flags |= prWriter.Write(Forms2Attr.Accelerator, 2) ? (uint)CommandButtonPropMask.Accelerator : 0;
            flags |= prWriter.Write(Forms2Attr.MouseIcon, 2) ? (uint)CommandButtonPropMask.MouseIcon : 0;

            flags |= (!Pr.IsDefaultValue(Forms2Attr.TakeFocusOnClick)) ? (uint)CommandButtonPropMask.TakeFocusOnClick : 0;

            // Write padding to the end, so the size of written data is divisible by 4.
            prWriter.WritePadding(4);

            // Extra data block.
            flags |= prWriter.WriteBytes(caption, 4) ? (uint)CommandButtonPropMask.Caption : 0;
            flags |= prWriter.Write(Forms2Attr.Size, 8) ? (uint)CommandButtonPropMask.Size : 0;

            // Write padding to the end, so the size of written data is divisible by 4.
            prWriter.WritePadding(4);

            // Write cbLabel and PropMask.
            prWriter.WriteDataSizeAndPropMask(startPos, flags);

            // Stream data.
            if (((CommandButtonPropMask)flags & CommandButtonPropMask.Picture) != 0)
                prWriter.WriteGuidAndPicture(Picture);
            if (((CommandButtonPropMask)flags & CommandButtonPropMask.MouseIcon) != 0)
                prWriter.WriteGuidAndPicture(MouseIcon);

            // Text props.
            Forms2TextProps.Write(writer, Pr);

            return (uint)(writer.BaseStream.Position - startPos);
        }

        /// <summary>
        /// Gets a Clsid of the embedded object.
        /// </summary>
        protected override string ClsidVirtual
        {
            get { return CommandButtonControlClsid; }
        }

        /// <summary>
        /// Gets type of Forms 2.0 control.
        /// </summary>
        public override Forms2OleControlType Type
        {
            get { return Forms2OleControlType.CommandButton; }
        }

        /// <summary>
        /// Gets ClsidCacheIndex.
        /// </summary>
        internal override ClsidCacheIndex ClsidCacheIndex
        {
            get { return ClsidCacheIndex.CommandButton; }
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

        /// <summary>
        /// Gets or sets <see cref="Aspose.Words.Forms2.ParagraphAlign"/> enumeration value
        /// that is responsible for caption alignment in a <see cref="CommandButtonControl"/>.
        /// </summary>
        internal ParagraphAlign ParagraphAlign
        {
            get { return (ParagraphAlign)Pr.FetchAttr(Forms2Attr.ParagraphAlign); }
            set { Pr.SetAttr(Forms2Attr.ParagraphAlign, value); }
        }

        /// <summary>
        /// Implements [MS-OFORMS] 2.2.1.2 CommandButtonPropMask.
        /// </summary>
        [Flags]
        private enum CommandButtonPropMask
        {
            ForeColor = 0x00000001,
            BackColor = 0x00000002,
            VariousPropertyBits = 0x00000004,
            Caption = 0x00000008,
            PicturePosition = 0x00000010,
            Size = 0x00000020,
            MousePointer = 0x00000040,
            Picture = 0x00000080,
            Accelerator = 0x00000100,
            TakeFocusOnClick = 0x00000200,
            MouseIcon = 0x00000400
        }

        private readonly OleSize mDefaultSize = OleSize.FromPoints(72, 24);
    }
}
