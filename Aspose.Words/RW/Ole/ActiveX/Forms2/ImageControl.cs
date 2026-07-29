// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 08/10/2015 by Alexey Morozov

using System;
using System.IO;
using Aspose.Words.Drawing.Ole;

namespace Aspose.Words.Forms2
{
    /// <summary>
    /// The Image control is used to display a picture.
    /// </summary>
    internal class ImageControl : Forms2OleControl
    {
        /// <summary>
        /// Ctor.
        /// </summary>
        internal ImageControl(string name) : base(name, new ImageControlPr())
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

            // cbImage
            reader.ReadUInt16();
            ImagePropMask flags = (ImagePropMask)reader.ReadUInt32();

            Forms2PrReader prReader = new Forms2PrReader(reader, Pr);
            prReader.Flags = (uint) flags;

            // Data block.
            prReader.ReadData((uint)ImagePropMask.BorderColor, 4, Forms2Attr.BorderColor);
            prReader.ReadData((uint)ImagePropMask.BackColor, 4, Forms2Attr.BackgroundColor);
            prReader.ReadData((uint)ImagePropMask.BorderStyle, 1, Forms2Attr.BorderStyle);
            prReader.ReadData((uint)ImagePropMask.MousePointer, 1, Forms2Attr.MousePointer);
            prReader.ReadData((uint)ImagePropMask.PictureSizeMode, 1, Forms2Attr.PictureSizeMode);
            prReader.ReadData((uint)ImagePropMask.SpecialEffect, 1, Forms2Attr.SpecialEffect);
            prReader.ReadData((uint)ImagePropMask.Picture, 2, Forms2Attr.Picture);
            prReader.ReadData((uint)ImagePropMask.PictureAlignment, 1, Forms2Attr.PictureAlignment);
            prReader.ReadData((uint)ImagePropMask.VariousPropertyBits, 4, Forms2Attr.VariousPropertyBits);
            prReader.ReadData((uint)ImagePropMask.MouseIcon, 2, Forms2Attr.MouseIcon);
            Pr.SetAttr(Forms2Attr.AutoSize, (flags & ImagePropMask.AutoSize) != 0);
            Pr.SetAttr(Forms2Attr.PictureTiling, (flags & ImagePropMask.PictureTiling) != 0);

            // Extra block.
            prReader.ReadBytes((uint)(flags & ImagePropMask.Size), 8, Forms2Attr.Size);

            // Stream data.
            if ((flags & ImagePropMask.Picture) != 0)
                prReader.ReadGuidAndPicture(Forms2Attr.Picture);

            if ((flags & ImagePropMask.MouseIcon) != 0)
                prReader.ReadGuidAndPicture(Forms2Attr.MouseIcon);
        }

        /// <summary>
        /// Writes the control to a binary writer.
        /// </summary>
        internal override uint Write(BinaryWriter writer)
            {
            int startPos = (int)writer.BaseStream.Position;

            writer.Write(Forms2Version);

            // The size of data and property flags will be written later.
            writer.Seek(2, SeekOrigin.Current);// cbImage
            writer.Seek(4, SeekOrigin.Current);// ImagePropMask

            Forms2PrWriter prWriter = new Forms2PrWriter(writer, Pr);

            uint flags = 0;
            flags |= prWriter.Write(Forms2Attr.BorderColor, 4) ? (uint)ImagePropMask.BorderColor : 0;
            flags |= prWriter.Write(Forms2Attr.BackgroundColor, 4) ? (uint)ImagePropMask.BackColor : 0;
            flags |= prWriter.Write(Forms2Attr.BorderStyle, 1) ? (uint)ImagePropMask.BorderStyle : 0;
            flags |= prWriter.Write(Forms2Attr.MousePointer, 1) ? (uint)ImagePropMask.MousePointer : 0;
            flags |= prWriter.Write(Forms2Attr.PictureSizeMode, 1) ? (uint)ImagePropMask.PictureSizeMode : 0;
            flags |= prWriter.Write(Forms2Attr.SpecialEffect, 1) ? (uint)ImagePropMask.SpecialEffect : 0;
            flags |= prWriter.Write(Forms2Attr.Picture, 2) ? (uint)ImagePropMask.Picture : 0;
            flags |= prWriter.Write(Forms2Attr.PictureAlignment, 1) ? (uint)ImagePropMask.PictureAlignment : 0;
            flags |= prWriter.Write(Forms2Attr.VariousPropertyBits, 4) ? (uint)ImagePropMask.VariousPropertyBits : 0;
            flags |= prWriter.Write(Forms2Attr.MouseIcon, 2) ? (uint)ImagePropMask.MouseIcon : 0;

            flags |= (AutoSize) ? (uint)ImagePropMask.AutoSize : 0;
            flags |= (PictureTiling) ? (uint)ImagePropMask.PictureTiling : 0;

            // Write padding to the end, so the size of written data is divisible by 4.
            prWriter.WritePadding(4);

            // Extra data block.
            flags |= prWriter.Write(Forms2Attr.Size, 8) ? (uint)ImagePropMask.Size : 0;

            // Write padding to the end, so the size of written data is divisible by 4.
            prWriter.WritePadding(4);

            // Write cbImage and PropMask.
            prWriter.WriteDataSizeAndPropMask(startPos, flags);

            // Stream data.
            if (((ImagePropMask)flags & ImagePropMask.Picture) != 0)
                prWriter.WriteGuidAndPicture(Picture);

            if (((ImagePropMask)flags & ImagePropMask.MouseIcon) != 0)
                prWriter.WriteGuidAndPicture(MouseIcon);

            return (uint)(writer.BaseStream.Position - startPos);
            }

        /// <summary>
        /// Gets type of Forms 2.0 control.
        /// </summary>
        public override Forms2OleControlType Type
        {
            get { return Forms2OleControlType.Image; }
        }

        /// <summary>
        /// Gets ClsidCacheIndex.
        /// </summary>
        internal override ClsidCacheIndex ClsidCacheIndex
        {
            get { return ClsidCacheIndex.Image; }
        }

        /// <summary>
        /// Gets or sets PictureSizeMode that specifies how to display the picture.
        /// </summary>
        internal PictureSizeMode PictureSizeMode
        {
            get { return (PictureSizeMode)Pr.FetchAttr(Forms2Attr.PictureSizeMode); }
            set { Pr.SetAttr(Forms2Attr.PictureSizeMode, value); }
        }

        /// <summary>
        /// Gets or sets PictureAlignment that specifies the alignment of the picture.
        /// </summary>
        internal PictureAlignment PictureAlignment
        {
            get { return (PictureAlignment)Pr.FetchAttr(Forms2Attr.PictureAlignment); }
            set { Pr.SetAttr(Forms2Attr.PictureAlignment, value); }
        }

        /// <summary>
        /// Gets or sets a boolean value that specifies whether the picture is tiled across the background.
        /// </summary>
        internal bool PictureTiling
        {
            get { return (bool)Pr.FetchAttr(Forms2Attr.PictureTiling); }
            set {Pr.SetAttr(Forms2Attr.PictureTiling, value);}
        }

        /// <summary>
        /// Gets a Clsid of the embedded object.
        /// </summary>
        protected override string ClsidVirtual
        {
            get { return ImageControlClsid; }
        }

        /// <summary>
        /// Implements [MS-OFORMS] 2.2.3.2 ImagePropMask.
        /// </summary>
        [Flags]
        private enum ImagePropMask : uint
        {
            UnusedBits11 = 0x00000001,
            UnusedBits12 = 0x00000002,
            AutoSize = 0x00000004,
            BorderColor = 0x00000008,
            BackColor = 0x00000010,
            BorderStyle = 0x00000020,
            MousePointer = 0x00000040,
            PictureSizeMode = 0x00000080,
            SpecialEffect = 0x00000100,
            Size = 0x00000200,
            Picture = 0x00000400,
            PictureAlignment = 0x00000800,
            PictureTiling = 0x00001000,
            VariousPropertyBits = 0x00002000,
            MouseIcon = 0x00004000
        }

        private readonly OleSize mDefaultSize = OleSize.FromPoints(72, 72);
    }
}
