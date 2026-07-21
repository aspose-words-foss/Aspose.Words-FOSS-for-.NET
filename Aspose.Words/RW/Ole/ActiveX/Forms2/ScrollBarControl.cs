// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 07/10/2015 by Alexey Morozov

using System;
using System.IO;
using Aspose.Words.Drawing.Ole;

namespace Aspose.Words.Forms2
{
    /// <summary>
    /// The ScrollBar control scrolls through a range of values when a user clicks the scroll arrows.
    /// </summary>
    internal class ScrollBarControl : ScrollableControlBase
    {
        /// <summary>
        /// Ctor.
        /// </summary>
        internal ScrollBarControl(string name) : base(name, new ScrollBarControlPr())
        {
            Size = mDefaultSize;
        }

        /// <summary>
        /// Reads the control from a binary stream.
        /// </summary>
        internal override void Read(BinaryReader reader)
        {
            int version = reader.ReadUInt16();
            Debug.Assert(version == Forms2Version);

            reader.ReadUInt16();    // cbScrollBar
            ScrollBarPropMask flags = (ScrollBarPropMask)reader.ReadUInt32();

            Forms2PrReader prReader = new Forms2PrReader(reader, Pr);
            prReader.Flags = (uint)flags;

            // Data block.
            prReader.ReadData((uint)ScrollBarPropMask.ForeColor, 4, Forms2Attr.ForegroundColor);
            prReader.ReadData((uint)ScrollBarPropMask.BackColor, 4, Forms2Attr.BackgroundColor);
            prReader.ReadData((uint)ScrollBarPropMask.VariousPropertyBits, 4, Forms2Attr.VariousPropertyBits);

            prReader.ReadData((uint)ScrollBarPropMask.MousePointer, 1, Forms2Attr.MousePointer);
            prReader.ReadData((uint)ScrollBarPropMask.Min, 4, Forms2Attr.Min);
            prReader.ReadData((uint)ScrollBarPropMask.Max, 4, Forms2Attr.Max);
            prReader.ReadData((uint)ScrollBarPropMask.Position, 4, Forms2Attr.Position);
            prReader.ReadData((uint)ScrollBarPropMask.PrevEnabled, 4, Forms2Attr.PrevEnabled);
            prReader.ReadData((uint)ScrollBarPropMask.NextEnabled, 4, Forms2Attr.NextEnabled);
            prReader.ReadData((uint)ScrollBarPropMask.SmallChange, 4, Forms2Attr.SmallChange);
            prReader.ReadData((uint)ScrollBarPropMask.LargeChange, 4, Forms2Attr.LargeChange);
            prReader.ReadData((uint)ScrollBarPropMask.Orientation, 4, Forms2Attr.Orientation);
            prReader.ReadData((uint)ScrollBarPropMask.ProportionalThumb, 2, Forms2Attr.ProportionalThumb);
            prReader.ReadData((uint)ScrollBarPropMask.Delay, 2, Forms2Attr.Delay);
            prReader.ReadData((uint)ScrollBarPropMask.MouseIcon, 2, Forms2Attr.MouseIcon);

            // Extra block
            prReader.ReadBytes((uint)(flags & ScrollBarPropMask.Size), 8, Forms2Attr.Size);

            // Stream data.
            if ((flags & ScrollBarPropMask.MouseIcon) != 0)
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
            writer.Seek(2, SeekOrigin.Current);// cbScrollBar
            writer.Seek(4, SeekOrigin.Current);// PropMask

            Forms2PrWriter prWriter = new Forms2PrWriter(writer, Pr);

            uint flags = 0;
            flags |= prWriter.Write(Forms2Attr.ForegroundColor, 4) ? (uint)ScrollBarPropMask.ForeColor : 0;
            flags |= prWriter.Write(Forms2Attr.BackgroundColor, 4) ? (uint)ScrollBarPropMask.BackColor : 0;
            flags |= prWriter.Write(Forms2Attr.VariousPropertyBits, 4) ? (uint)ScrollBarPropMask.VariousPropertyBits : 0;
            flags |= prWriter.Write(Forms2Attr.MousePointer, 1) ? (uint)ScrollBarPropMask.MousePointer : 0;
            flags |= prWriter.Write(Forms2Attr.Min, 4) ? (uint)ScrollBarPropMask.Min : 0;
            flags |= prWriter.Write(Forms2Attr.Max, 4) ? (uint)ScrollBarPropMask.Max : 0;
            flags |= prWriter.Write(Forms2Attr.Position, 4) ? (uint)ScrollBarPropMask.Position : 0;
            flags |= prWriter.Write(Forms2Attr.PrevEnabled, 4) ? (uint)ScrollBarPropMask.PrevEnabled : 0;
            flags |= prWriter.Write(Forms2Attr.NextEnabled, 4) ? (uint)ScrollBarPropMask.NextEnabled : 0;
            flags |= prWriter.Write(Forms2Attr.SmallChange, 4) ? (uint)ScrollBarPropMask.SmallChange : 0;
            flags |= prWriter.Write(Forms2Attr.LargeChange, 4) ? (uint)ScrollBarPropMask.LargeChange : 0;
            flags |= prWriter.Write(Forms2Attr.Orientation, 4) ? (uint)ScrollBarPropMask.Orientation : 0;
            flags |= prWriter.Write(Forms2Attr.ProportionalThumb, 2) ? (uint)ScrollBarPropMask.ProportionalThumb : 0;
            flags |= prWriter.Write(Forms2Attr.Delay, 4) ? (uint)ScrollBarPropMask.Delay : 0;
            flags |= prWriter.Write(Forms2Attr.MouseIcon, 2) ? (uint)ScrollBarPropMask.MouseIcon : 0;

            // Write padding to the end, so the size of written data is divisible by 4.
            prWriter.WritePadding(4);

            // Extra block.
            flags |= prWriter.Write(Forms2Attr.Size, 8) ? (uint)ScrollBarPropMask.Size : 0;

            // Write padding to the end, so the size of written data is divisible by 4.
            prWriter.WritePadding(4);

            // Write cbScrollBar and PropMask.
            prWriter.WriteDataSizeAndPropMask(startPos, flags);

            // Stream data.
            if (((ScrollBarPropMask)flags & ScrollBarPropMask.MouseIcon) != 0)
                prWriter.WriteGuidAndPicture(MouseIcon);

            return (uint)(writer.BaseStream.Position - startPos);
        }

        /// <summary>
        /// Gets type of Forms 2.0 control.
        /// </summary>
        public override Forms2OleControlType Type
        {
            get { return Forms2OleControlType.ScrollBar; }
        }

        /// <summary>
        /// Gets ClsidCacheIndex.
        /// </summary>
        internal override ClsidCacheIndex ClsidCacheIndex
        {
            get { return ClsidCacheIndex.ScrollBar; }
        }

        /// <summary>
        /// Gets a Clsid of the embedded object.
        /// </summary>
        protected override string ClsidVirtual
        {
            get { return ScrollBarControlClsid; }
        }

        internal FormOrientation Orientation
        {
            get { return (FormOrientation)Pr.FetchAttr(Forms2Attr.Orientation); }
            set { Pr.SetAttr(Forms2Attr.Orientation, value); }
        }

        internal ProportionalThumb ProportionalThumb
        {
            get { return (ProportionalThumb)Pr.FetchAttr(Forms2Attr.ProportionalThumb); }
            set { Pr.SetAttr(Forms2Attr.ProportionalThumb, value); }
        }

        /// <summary>
        /// Implements [MS-OFORMS] 2.2.7.2 ScrollBarPropMask.
        /// </summary>
        [Flags]
        private enum ScrollBarPropMask : uint
        {
            ForeColor = 0x00000001,
            BackColor = 0x00000002,
            VariousPropertyBits = 0x00000004,
            Size = 0x00000008,
            MousePointer = 0x00000010,
            Min = 0x00000020,
            Max = 0x00000040,
            Position = 0x00000080,
            UnusedBits1 = 0x00000100,
            PrevEnabled = 0x00000200,
            NextEnabled = 0x00000400,
            SmallChange = 0x00000800,
            LargeChange = 0x00001000,
            Orientation = 0x00002000,
            ProportionalThumb = 0x00004000,
            Delay = 0x00008000,
            MouseIcon = 0x00010000
        }

        private readonly OleSize mDefaultSize = OleSize.FromPoints(13, 64);
    }
}
