// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 08/10/2015 by Alexey Morozov

using System;
using System.IO;
using Aspose.Words.Drawing.Ole;

namespace Aspose.Words.Forms2
{
    /// <summary>
    /// Implements [MS-OFORMS] 2.2.10.12.1 OleSiteConcreteControl
    /// </summary>
    internal class OleSiteConcreteControl
    {
        /// <summary>
        /// Reads site from a binary reader.
        /// </summary>
        internal OleSiteConcreteControl(BinaryReader reader)
        {
            int version = reader.ReadUInt16();
            Debug.Assert(Version == version);

            // cbSite
            reader.ReadUInt16();
            SitePropMask flags = (SitePropMask)reader.ReadUInt32();

            Forms2PrReader prReader = new Forms2PrReader(reader, mPr);
            prReader.Flags = (uint)flags;

            uint cName = prReader.ReadData((uint)SitePropMask.Name, 4);
            uint cTag = prReader.ReadData((uint)SitePropMask.Tag, 4);
            prReader.ReadData((uint)SitePropMask.ID, 4, Forms2Attr.ID);
            prReader.ReadData((uint) SitePropMask.HelpContextID, 4, Forms2Attr.HelpContextID);
            prReader.ReadData((uint)SitePropMask.BitFlags, 4, Forms2Attr.BitFlagsSite);
            prReader.ReadData((uint) SitePropMask.ObjectStreamSize, 4, Forms2Attr.ObjectStreamSize);
            prReader.ReadData((uint)SitePropMask.TabIndex, 2, Forms2Attr.TabIndex);
            prReader.ReadData((uint) SitePropMask.ClsidCacheIndex, 2, Forms2Attr.ClsidCacheIndex);
            prReader.ReadData((uint) SitePropMask.GroupID, 2, Forms2Attr.GroupID);

            uint cTipText = prReader.ReadData((uint)SitePropMask.ControlTipText, 4);
            uint cRuntimeLicKey = prReader.ReadData((uint)SitePropMask.RuntimeLicKey, 4);
            uint cControlSource = prReader.ReadData((uint)SitePropMask.ControlSource, 4);
            uint cRowSource = prReader.ReadData((uint)SitePropMask.RowSource, 4);

            prReader.ReadString((uint)SitePropMask.Name, cName, Forms2Attr.Name);
            prReader.ReadString((uint)SitePropMask.Tag, cTag, Forms2Attr.Tag);

            prReader.ReadBytes((uint)SitePropMask.Position, 8, Forms2Attr.SitePosition);

            prReader.ReadString((uint)SitePropMask.ControlTipText, cTipText, Forms2Attr.ControlTipText);
            prReader.ReadString((uint)SitePropMask.RuntimeLicKey, cRuntimeLicKey, Forms2Attr.RuntimeLicKey);
            prReader.ReadString((uint)SitePropMask.ControlSource, cControlSource, Forms2Attr.ControlSource);
            prReader.ReadString((uint)SitePropMask.RowSource, cRowSource, Forms2Attr.RowSource);

            prReader.ReadPadding(4);
        }

        /// <summary>
        /// Writes a control site data to a binary writer.
        /// </summary>
        internal static void Write(BinaryWriter writer, Forms2OleControl control)
        {
            int startPos = (int)writer.BaseStream.Position;

            writer.Write(Version);

            // The size of data and property flags will be written later.
            writer.Seek(2, SeekOrigin.Current);// cbSite
            writer.Seek(4, SeekOrigin.Current);// SitePropMask

            Forms2PrWriter prWriter = new Forms2PrWriter(writer, control.Pr);

            // Update clsidCacheIndex from the actual type of the control,
            // as it can be missed in case when control was created from the scratch.
            if (control.Pr.IsDefaultValue(Forms2Attr.ClsidCacheIndex))
                control.Pr[Forms2Attr.ClsidCacheIndex] = control.ClsidCacheIndex;

            uint flags = 0;
            byte[] name = prWriter.WriteCountOfBytesWithCompressionFlag(control.Name);
            byte[] tag = prWriter.WriteCountOfBytesWithCompressionFlag(control.Tag);
            flags |= prWriter.Write(Forms2Attr.ID, 4) ? (uint)SitePropMask.ID : 0;
            flags |= prWriter.Write(Forms2Attr.HelpContextID, 4) ? (uint)SitePropMask.HelpContextID : 0;
            flags |= prWriter.Write(Forms2Attr.BitFlagsSite, 4) ? (uint)SitePropMask.BitFlags : 0;
            flags |= prWriter.Write(Forms2Attr.ObjectStreamSize, 4) ? (uint)SitePropMask.ObjectStreamSize : 0;
            flags |= prWriter.Write(Forms2Attr.TabIndex, 2) ? (uint)SitePropMask.TabIndex : 0;
            flags |= prWriter.Write(Forms2Attr.ClsidCacheIndex, 2) ? (uint)SitePropMask.ClsidCacheIndex : 0;
            flags |= prWriter.Write(Forms2Attr.GroupID, 2) ? (uint)SitePropMask.GroupID : 0;
            byte[] tooltip = prWriter.WriteCountOfBytesWithCompressionFlag(control.Tooltip);
            byte[] lic = prWriter.WriteCountOfBytesWithCompressionFlag(control.RuntimeLicKey);
            byte[] controlSource = prWriter.WriteCountOfBytesWithCompressionFlag(control.ControlSource);
            byte[] rowSource = prWriter.WriteCountOfBytesWithCompressionFlag(control.RowSource);

            // Write padding to the end, so the size of written data is divisible by 4.
            prWriter.WritePadding(4);

            // Extra data block.
            flags |= prWriter.WriteBytes(name, 4) ? (uint)SitePropMask.Name : 0;
            flags |= prWriter.WriteBytes(tag, 4) ? (uint)SitePropMask.Tag : 0;
            flags |= prWriter.Write(Forms2Attr.SitePosition, 8) ? (uint)SitePropMask.Position : 0;
            flags |= prWriter.WriteBytes(tooltip, 4) ? (uint)SitePropMask.ControlTipText : 0;
            flags |= prWriter.WriteBytes(lic, 4) ? (uint)SitePropMask.RuntimeLicKey : 0;
            flags |= prWriter.WriteBytes(controlSource, 4) ? (uint)SitePropMask.ControlSource : 0;
            flags |= prWriter.WriteBytes(rowSource, 4) ? (uint)SitePropMask.RowSource : 0;

            prWriter.WritePadding(4);

            // Write cbSite and SitePropMask.
            prWriter.WriteDataSizeAndPropMask(startPos, flags);
        }

        /// <summary>
        /// Returns corresponding Forms 2.0 OLE control.
        /// </summary>
        internal Forms2OleControl GetForms2OleControl()
        {
            Forms2OleControl forms2OleControl = Forms2OleControl.Create(Type, Name);
            Pr.CopyTo(forms2OleControl.Pr);

            return forms2OleControl;
        }

        /// <summary>
        /// Converts a ClsidCacheIndex to a control type.
        /// </summary>
        private static Forms2OleControlType ClsidCacheIndexToType(ClsidCacheIndex clsidCacheIndex)
        {
            switch (clsidCacheIndex)
        {
                case ClsidCacheIndex.OptionButton:
                    return Forms2OleControlType.OptionButton;
                case ClsidCacheIndex.Label:
                    return Forms2OleControlType.Label;
                case ClsidCacheIndex.TextBox:
                    return Forms2OleControlType.Textbox;
                case ClsidCacheIndex.CheckBox:
                    return Forms2OleControlType.CheckBox;
                case ClsidCacheIndex.ToggleButton:
                    return Forms2OleControlType.ToggleButton;
                case ClsidCacheIndex.SpinButton:
                    return Forms2OleControlType.SpinButton;
                case ClsidCacheIndex.ComboBox:
                    return Forms2OleControlType.ComboBox;
                case ClsidCacheIndex.Frame:
                    return Forms2OleControlType.Frame;
                case ClsidCacheIndex.MultiPage:
                    return Forms2OleControlType.MultiPage;
                case ClsidCacheIndex.CommandButton:
                    return Forms2OleControlType.CommandButton;
                case ClsidCacheIndex.Image:
                    return Forms2OleControlType.Image;
                case ClsidCacheIndex.ScrollBar:
                    return Forms2OleControlType.ScrollBar;
                case ClsidCacheIndex.TabStrip:
                    return Forms2OleControlType.TabStrip;
                case ClsidCacheIndex.ListBox:
                    return Forms2OleControlType.ListBox;
                case ClsidCacheIndex.Form:
                    return Forms2OleControlType.Form;
                default:
                    throw new InvalidOperationException(string.Format("Unexpected ClsidCacheIndex: {0}", clsidCacheIndex));
            }
        }

        /// <summary>
        /// Gets size, in bytes, of an embedded control that is persisted to the Object stream of a Form.
        /// </summary>
        internal int ObjectStreamSize
        {
            get { return (int)(uint)mPr.FetchAttr(Forms2Attr.ObjectStreamSize); }
        }

        /// <summary>
        /// Gets a name of a storage for a parent control on a Form.
        /// </summary>
        internal string StorageName
        {
            get
        {
                int id = (int)mPr.FetchAttr(Forms2Attr.ID);
                return string.Format("i{0:d2}", id);
            }
        }

        /// <summary>
        /// Gets a corresponding Forms2 Ole control type of this Ole site concrete control.
        /// </summary>
        private Forms2OleControlType Type
        {
            get { return ClsidCacheIndexToType(ClsidCacheIndex); }
        }

        /// <summary>
        /// Gets a name of the control.
        /// </summary>
        private string Name
        {
            get { return (string)mPr.FetchAttr(Forms2Attr.Name); }
        }

        /// <summary>
        /// Gets Clsid that specifies the type of a FormEmbeddedActiveXControl on a parent control.
        /// </summary>
        private ClsidCacheIndex ClsidCacheIndex
        {
            get { return (ClsidCacheIndex)mPr.FetchAttr(Forms2Attr.ClsidCacheIndex); }
        }

        /// <summary>
        /// Gets a collection of properties of the control.
        /// </summary>
        private Forms2Pr Pr
        {
            get { return mPr; }
        }

        /// <summary>
        /// [MS-OFORMS] 2.2.10.12.2 SitePropMask.
        /// </summary>
        [Flags]
        private enum SitePropMask : uint
        {
            Name = 0x00000001,
            Tag = 0x00000002,

            ID = 0x00000004,
            HelpContextID = 0x00000008,
            BitFlags = 0x00000010,
            ObjectStreamSize = 0x00000020,
            TabIndex = 0x00000040,
            ClsidCacheIndex = 0x00000080,
            Position = 0x00000100,
            GroupID = 0x00000200,
            Unused1 = 0x00000400,
            ControlTipText = 0x00000800,
            RuntimeLicKey = 0x00001000,
            ControlSource = 0x00002000,
            RowSource = 0x00004000
        }

        private readonly Forms2Pr mPr = new Forms2Pr();

        /// <summary>
        /// The version of the control as specified in [MS-OFORMS] 2.2.10.12.1 OleSiteConcreteControl.
        /// </summary>
        private const ushort Version = 0x0000;
    }
}
