// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 07/10/2015 by Alexey Morozov

using System;
using System.IO;
using Aspose.Words.Drawing.Ole;

namespace Aspose.Words.Forms2
{
    /// <summary>
    /// The SpinButton control increases or decreases a value, such as a number, time, or date.
    /// </summary>
    internal class SpinButtonControl : ScrollableControlBase
    {
        /// <summary>
        /// Ctor.
        /// </summary>
        internal SpinButtonControl(string name) : base(name)
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

            reader.ReadUInt16();  // cbSpinButton
            SpinButtonPropMask flags = (SpinButtonPropMask)reader.ReadUInt32();

            Forms2PrReader prReader = new Forms2PrReader(reader, Pr);
            prReader.Flags = (uint)flags;

            // Data block.
            prReader.ReadData((uint)SpinButtonPropMask.ForeColor, 4, Forms2Attr.ForegroundColor);
            prReader.ReadData((uint)SpinButtonPropMask.BackColor, 4, Forms2Attr.BackgroundColor);
            prReader.ReadData((uint)SpinButtonPropMask.VariousPropertyBits, 4, Forms2Attr.VariousPropertyBits);
            prReader.ReadData((uint)SpinButtonPropMask.Min, 4, Forms2Attr.Min);
            prReader.ReadData((uint)SpinButtonPropMask.Max, 4, Forms2Attr.Max);
            prReader.ReadData((uint)SpinButtonPropMask.Position, 4, Forms2Attr.Position);
            prReader.ReadData((uint)SpinButtonPropMask.PrevEnabled, 4, Forms2Attr.PrevEnabled);
            prReader.ReadData((uint)SpinButtonPropMask.NextEnabled, 4, Forms2Attr.NextEnabled);
            prReader.ReadData((uint)SpinButtonPropMask.SmallChange, 4, Forms2Attr.SmallChange);
            prReader.ReadData((uint)SpinButtonPropMask.Orientation, 4, Forms2Attr.Orientation);
            prReader.ReadData((uint)SpinButtonPropMask.Delay, 4, Forms2Attr.Delay);
            prReader.ReadData((uint)SpinButtonPropMask.MouseIcon, 2, Forms2Attr.MouseIcon);
            prReader.ReadData((uint)SpinButtonPropMask.MousePointer, 1, Forms2Attr.MousePointer);

            // Extra block.
            prReader.ReadBytes((uint)SpinButtonPropMask.Size, 8, Forms2Attr.Size);

            // Stream data.
            if ((flags & SpinButtonPropMask.MouseIcon) != 0)
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
            writer.Seek(2, SeekOrigin.Current);// cbSpinButton
            writer.Seek(4, SeekOrigin.Current);// PropMask

            Forms2PrWriter prWriter = new Forms2PrWriter(writer, Pr);

            uint flags = 0;
            flags |= prWriter.Write(Forms2Attr.ForegroundColor, 4) ? (uint)SpinButtonPropMask.ForeColor : 0;
            flags |= prWriter.Write(Forms2Attr.BackgroundColor, 4) ? (uint)SpinButtonPropMask.BackColor : 0;
            flags |= prWriter.Write(Forms2Attr.VariousPropertyBits, 4) ? (uint)SpinButtonPropMask.VariousPropertyBits : 0;
            flags |= prWriter.Write(Forms2Attr.Min, 4) ? (uint)SpinButtonPropMask.Min : 0;
            flags |= prWriter.Write(Forms2Attr.Max, 4) ? (uint)SpinButtonPropMask.Max : 0;
            flags |= prWriter.Write(Forms2Attr.Position, 4) ? (uint)SpinButtonPropMask.Position : 0;
            flags |= prWriter.Write(Forms2Attr.PrevEnabled, 4) ? (uint)SpinButtonPropMask.PrevEnabled : 0;
            flags |= prWriter.Write(Forms2Attr.NextEnabled, 4) ? (uint)SpinButtonPropMask.NextEnabled : 0;
            flags |= prWriter.Write(Forms2Attr.SmallChange, 4) ? (uint)SpinButtonPropMask.SmallChange : 0;
            flags |= prWriter.Write(Forms2Attr.Orientation, 4) ? (uint)SpinButtonPropMask.Orientation : 0;
            flags |= prWriter.Write(Forms2Attr.Delay, 4) ? (uint)SpinButtonPropMask.Delay : 0;
            flags |= prWriter.Write(Forms2Attr.MouseIcon, 2) ? (uint)SpinButtonPropMask.MouseIcon : 0;
            flags |= prWriter.Write(Forms2Attr.MousePointer, 1) ? (uint)SpinButtonPropMask.MousePointer : 0;

            // Write padding to the end, so the size of written data is divisible by 4.
            prWriter.WritePadding(4);

            // Extra block.
            flags |= prWriter.Write(Forms2Attr.Size, 8) ? (uint)SpinButtonPropMask.Size : 0;

            // Write padding to the end, so the size of written data is divisible by 4.
            prWriter.WritePadding(4);

            // Write cbSpinButton and PropMask.
            prWriter.WriteDataSizeAndPropMask(startPos, flags);

            // Stream data.
            if (((SpinButtonPropMask)flags & SpinButtonPropMask.MouseIcon) != 0)
                prWriter.WriteGuidAndPicture(MouseIcon);

            return (uint)(writer.BaseStream.Position - startPos);
        }

        /// <summary>
        /// Gets type of Forms 2.0 control.
        /// </summary>
        public override Forms2OleControlType Type
        {
            get { return Forms2OleControlType.SpinButton; }
        }

        /// <summary>
        /// Gets ClsidCacheIndex.
        /// </summary>
        internal override ClsidCacheIndex ClsidCacheIndex
        {
            get { return ClsidCacheIndex.SpinButton; }
        }

        /// <summary>
        /// Gets a Clsid of the embedded object.
        /// </summary>
        protected override string ClsidVirtual
        {
            get { return SpinButtonControlClsid; }
        }

        internal FormOrientation Orientation
        {
            get { return (FormOrientation)Pr.FetchAttr(Forms2Attr.Orientation); }
            set { Pr.SetAttr(Forms2Attr.Orientation, value); }
        }

        /// <summary>
        /// Implements [MS-OFORMS] 2.2.8.2 SpinButtonPropMask.
        /// </summary>
        [Flags]
        private enum SpinButtonPropMask : uint
        {
            ForeColor = 0x00000001,
            BackColor = 0x00000002,
            VariousPropertyBits = 0x00000004,
            Size = 0x00000008,
            UnusedBits1 = 0x00000010,
            Min = 0x00000020,
            Max = 0x00000040,
            Position = 0x00000080,
            PrevEnabled = 0x00000100,
            NextEnabled = 0x00000200,
            SmallChange = 0x00000400,
            Orientation = 0x00000800,
            Delay = 0x00001000,
            MouseIcon = 0x00002000,
            MousePointer = 0x00004000
        }

        private readonly OleSize mDefaultSize = OleSize.FromPoints(13, 26);
    }
}
