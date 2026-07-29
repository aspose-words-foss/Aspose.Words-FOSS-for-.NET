// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 08/10/2015 by Alexey Morozov

using System;
using System.Collections.Generic;
using System.IO;
using Aspose.Words.Drawing.Ole;

namespace Aspose.Words.Forms2
{
    /// <summary>
    /// The TabStrip control presents a set of related controls as a visual group.
    /// </summary>
    internal class TabStripControl : Forms2OleControl
    {
        /// <summary>
        /// Ctor.
        /// </summary>
        internal TabStripControl(string name) : base(name)
        {
            Size = mDefaultSize;
            Pr[Forms2Attr.ListIndex] = 0;
        }

        /// <summary>
        /// Reads the control from a binary reader.
        /// </summary>
        internal override void Read(BinaryReader reader)
        {
            int version = reader.ReadUInt16();
            Debug.Assert(version == Forms2Version);

            reader.ReadUInt16();      // cbTabStrip
            TabStripPropMask flags = (TabStripPropMask)reader.ReadUInt32();

            Forms2PrReader prReader = new Forms2PrReader(reader, Pr);
            prReader.Flags = (uint) flags;

            // Data block.
            prReader.ReadData((uint)TabStripPropMask.ListIndex, 4, Forms2Attr.ListIndex);
            prReader.ReadData((uint)TabStripPropMask.BackColor, 4, Forms2Attr.BackgroundColor);
            prReader.ReadData((uint)TabStripPropMask.ForeColor, 4, Forms2Attr.ForegroundColor);
            uint itemsSize = prReader.ReadData((uint)TabStripPropMask.Items, 4);
            prReader.ReadData((uint)TabStripPropMask.MousePointer, 1, Forms2Attr.MousePointer);
            prReader.ReadData((uint)TabStripPropMask.TabOrientation, 4, Forms2Attr.TabOrienation);
            prReader.ReadData((uint)TabStripPropMask.TabStyle, 4, Forms2Attr.TabStyle);
            prReader.ReadData((uint)TabStripPropMask.TabFixedWidth, 4, Forms2Attr.TabFixedWidth);
            prReader.ReadData((uint)TabStripPropMask.TabFixedHeight, 4, Forms2Attr.TabFixedHeight);

            uint tipsSize = prReader.ReadData((uint)TabStripPropMask.TipStrings, 4);
            uint namesSize = prReader.ReadData((uint)TabStripPropMask.Names, 4);

            prReader.ReadData((uint)TabStripPropMask.VariousPropertyBits, 4, Forms2Attr.VariousPropertyBits);

            prReader.ReadData((uint)TabStripPropMask.TabsAllocated, 4, Forms2Attr.TabsAllocated);
            uint tagsSize = prReader.ReadData((uint)TabStripPropMask.Tags, 4);
            prReader.ReadData((uint)TabStripPropMask.TabData, 4, Forms2Attr.TabData);
            uint accelsSize = prReader.ReadData((uint)TabStripPropMask.Accelerator, 4);
            prReader.ReadData((uint)TabStripPropMask.MouseIcon, 2, Forms2Attr.MouseIcon);

            Pr.SetAttr(Forms2Attr.MultiRow, (flags & TabStripPropMask.MultiRow) != 0);
            Pr.SetAttr(Forms2Attr.NewVersion, (TabStripPropMask.NewVersion) != 0);

            // Extra block
            prReader.ReadBytes((uint)TabStripPropMask.Size, 8, Forms2Attr.Size);

            IList<string> captions = prReader.ReadStringArray((uint)TabStripPropMask.Items, itemsSize);
            IList<string> tips = prReader.ReadStringArray((uint)TabStripPropMask.TipStrings, tipsSize);
            IList<string> names = prReader.ReadStringArray((uint)TabStripPropMask.Names, namesSize);
            IList<string> tags = prReader.ReadStringArray((uint)TabStripPropMask.Tags, tagsSize);
            IList<string> accelerators = prReader.ReadStringArray((uint)TabStripPropMask.Accelerator, accelsSize);

            AddTabs(captions, tips, names, tags, accelerators);

            // Stream data.
            if ((flags & TabStripPropMask.MouseIcon) != 0)
                prReader.ReadGuidAndPicture(Forms2Attr.MouseIcon);

            // TextProps.
            Forms2TextProps.Read(reader, Pr);

            //TabFlag data.
            uint tabData = (uint)Pr.FetchAttr(Forms2Attr.TabData);
            for (int i = 0; i < tabData; i++)
            {
                // If Tabs less then tabData, then create missed ones.
                if (i >= mTabs.Count)
                    mTabs.Add(new TabStripTab(""));

                mTabs[i].TabFlag = (TabFlag)prReader.ReadData((uint)TabStripPropMask.TabData, 4);
            }
        }

        /// <summary>
        /// Writes the control to a binary writer.
        /// </summary>
        internal override uint Write(BinaryWriter writer)
        {
            int startPos = (int)writer.BaseStream.Position;

            // Update [MS-OFORMS] 2.5.81 TabData.
            Pr.SetAttr(Forms2Attr.TabData, mTabs.Count);

            writer.Write(Forms2Version);

            // The size of data and property flags will be written later.
            writer.Seek(2, SeekOrigin.Current);// cbTabStrip
            writer.Seek(4, SeekOrigin.Current);// TabStripPropMask

            Forms2PrWriter prWriter = new Forms2PrWriter(writer, Pr);
            // According to [MS-OFORMS] 2.2.9.1 TabStripControl, fSize and fNewVersion MUST be set to 1.
            uint flags = (uint)(TabStripPropMask.Size | TabStripPropMask.NewVersion);
            flags |= prWriter.Write(Forms2Attr.ListIndex, 4) ? (uint)TabStripPropMask.ListIndex : 0;
            flags |= prWriter.Write(Forms2Attr.BackgroundColor, 4) ? (uint)TabStripPropMask.BackColor : 0;
            flags |= prWriter.Write(Forms2Attr.ForegroundColor, 4) ? (uint)TabStripPropMask.ForeColor : 0;
            uint size;
            byte[][] items = Forms2PrWriter.GetBytes(Items, out size);
            if (size > 0)
                prWriter.Write(size);
            flags |= prWriter.Write(Forms2Attr.MousePointer, 1) ? (uint)TabStripPropMask.MousePointer : 0;
            flags |= prWriter.Write(Forms2Attr.TabOrienation, 4) ? (uint)TabStripPropMask.TabOrientation : 0;
            flags |= prWriter.Write(Forms2Attr.TabStyle, 4) ? (uint)TabStripPropMask.TabStyle : 0;
            flags |= prWriter.Write(Forms2Attr.TabFixedWidth, 4) ? (uint)TabStripPropMask.TabFixedWidth : 0;
            flags |= prWriter.Write(Forms2Attr.TabFixedHeight, 4) ? (uint)TabStripPropMask.TabFixedHeight : 0;
            byte[][] tips = Forms2PrWriter.GetBytes(Tips, out size);
            if (size > 0)
                prWriter.Write(size);
            byte[][] names = Forms2PrWriter.GetBytes(Names, out size);
            if (size > 0)
                prWriter.Write(size);
            flags |= prWriter.Write(Forms2Attr.VariousPropertyBits, 4) ? (uint)TabStripPropMask.VariousPropertyBits : 0;
            flags |= prWriter.Write(Forms2Attr.TabsAllocated, 4) ? (uint)TabStripPropMask.TabsAllocated : 0;
            byte[][] tags = Forms2PrWriter.GetBytes(Tags, out size);
            if (size > 0)
                prWriter.Write(size);
            flags |= prWriter.Write(Forms2Attr.TabData, 4) ? (uint)TabStripPropMask.TabData : 0;
            byte[][] accelerators = Forms2PrWriter.GetBytes(Accelerators, out size);
            if (size > 0)
                prWriter.Write(size);
            flags |= prWriter.Write(Forms2Attr.MouseIcon, 2) ? (uint)TabStripPropMask.MouseIcon : 0;

            // Specifies whether the value of the MultiRow property is NOT the file format default.
            flags |= (!Pr.IsDefaultValue(Forms2Attr.MultiRow)) ? (uint)TabStripPropMask.MultiRow : 0;

            // Write padding to the end, so the size of written data is divisible by 4.
            prWriter.WritePadding(4);

            // Extra block.
            flags |= prWriter.Write(Forms2Attr.Size, 8) ? (uint)TabStripPropMask.Size : 0;

            flags |= prWriter.WriteBytes(items) ? (uint)TabStripPropMask.Items : 0;
            flags |= prWriter.WriteBytes(tips) ? (uint)TabStripPropMask.TipStrings : 0;
            flags |= prWriter.WriteBytes(names) ? (uint)TabStripPropMask.Names : 0;
            flags |= prWriter.WriteBytes(tags) ? (uint)TabStripPropMask.Tags : 0;
            flags |= prWriter.WriteBytes(accelerators) ? (uint)TabStripPropMask.Accelerator : 0;

            // Write padding to the end, so the size of written data is divisible by 4.
            prWriter.WritePadding(4);

            // Write cbTabStrip and TabStripPropMask.
            prWriter.WriteDataSizeAndPropMask(startPos, flags);

            // Stream data.
            if (((TabStripPropMask)flags & TabStripPropMask.MouseIcon) != 0)
                prWriter.WriteGuidAndPicture(MouseIcon);

            // TextProps.
            Forms2TextProps.Write(writer, Pr);

            //TabFlag data.
            foreach (TabStripTab tab in mTabs)
                prWriter.Write((uint)tab.TabFlag);

            return (uint)(writer.BaseStream.Position - startPos);
        }

        /// <summary>
        /// Adds tabs to the control.
        /// </summary>
        private void AddTabs(IList<string> captions, IList<string> tips, IList<string> names, IList<string> tags,
            IList<string> accelerators)
        {
            int tabsCount = MathUtil.Max(captions.Count, tips.Count, names.Count, tags.Count, accelerators.Count);
            for (int i = 0; i < tabsCount; i++)
            {
                mTabs.Add(new TabStripTab(""));

                if (i < captions.Count)
                    mTabs[i].Caption = captions[i];

                if (i < tips.Count)
                    mTabs[i].Tip = tips[i];

                if (i < names.Count)
                    mTabs[i].Name = names[i];

                if (i < tags.Count)
                    mTabs[i].Tag = tags[i];

                if (i < accelerators.Count)
                    mTabs[i].Accelerator = (StringUtil.HasChars(accelerators[i])) ? accelerators[i][0] : '\0';
            }
        }

        /// <summary>
        /// Gets type of Forms 2.0 control.
        /// </summary>
        public override Forms2OleControlType Type
        {
            get { return Forms2OleControlType.TabStrip; }
        }

        /// <summary>
        /// Gets ClsidCacheIndex.
        /// </summary>
        internal override ClsidCacheIndex ClsidCacheIndex
        {
            get { return ClsidCacheIndex.TabStrip; }
        }

        /// <summary>
        /// Gets a list of the tabs.
        /// </summary>
        internal IList<TabStripTab> Tabs
        {
            get { return mTabs; }
        }

        /// <summary>
        /// Gets a index of the selected tab.
        /// </summary>
        internal int ListIndex
        {
            get { return (int)Pr.FetchAttr(Forms2Attr.ListIndex); }
        }

        /// <summary>
        /// Gets a Clsid of the embedded object.
        /// </summary>
        protected override string ClsidVirtual
        {
            get { return TabStripControlClsid; }
        }

        /// <summary>
        /// Gets a list of captions.
        /// </summary>
        private IList<string> Items
        {
            get
            {
                IList<string> captions = new List<string>();
                foreach (TabStripTab tab in mTabs)
                    captions.Add(tab.Caption);

                return captions;
            }
        }

        /// <summary>
        /// Gets a list of tooltips.
        /// </summary>
        private IList<string> Tips
        {
            get
            {
                IList<string> tips = new List<string>();
                foreach (TabStripTab tab in mTabs)
                    tips.Add(tab.Tip);

                return tips;
            }
        }

        /// <summary>
        /// Gets a list of names.
        /// </summary>
        private IList<string> Names
        {
            get
            {
                IList<string> names = new List<string>();
                foreach (TabStripTab tab in mTabs)
                    names.Add(tab.Name);

                return names;
            }
        }

        /// <summary>
        /// Gets a list of tags.
        /// </summary>
        private IList<string> Tags
        {
            get
        {
                IList<string> tags = new List<string>();
                foreach (TabStripTab tab in mTabs)
                    tags.Add(tab.Tag);

                return tags;
            }
        }

        /// <summary>
        /// Gets a list of accelerators.
        /// </summary>
        private IList<string> Accelerators
        {
            get
            {
                IList<string> accelerators = new List<string>();
                foreach (TabStripTab tab in mTabs)
                {
                    string  accelerator = tab.Accelerator > 0 ? new string(tab.Accelerator, 1) : "";
                    accelerators.Add(accelerator);
                }

                return accelerators;
            }
        }

        /// <summary>
        /// Implements [MS-OFORMS] 2.2.9.2 TabStripPropMask.
        /// </summary>
        [Flags]
        private enum TabStripPropMask
        {
            ListIndex = 0x00000001,
            BackColor = 0x00000002,
            ForeColor = 0x00000004,
            Unused1 = 0x00000008,
            Size = 0x00000010,
            Items = 0x00000020,
            MousePointer = 0x00000040,
            Unused2 = 0x00000080,
            TabOrientation = 0x00000100,
            TabStyle = 0x00000200,
            MultiRow = 0x00000400,
            TabFixedWidth = 0x00000800,
            TabFixedHeight = 0x00001000,
            Tooltips = 0x00002000,
            Unused3 = 0x00004000,
            TipStrings = 0x00008000,
            Unused4 = 0x00010000,
            Names = 0x00020000,
            VariousPropertyBits = 0x00040000,
            NewVersion = 0x00080000,
            TabsAllocated = 0x00100000,
            Tags = 0x00200000,
            TabData = 0x00400000,
            Accelerator = 0x00800000,
            MouseIcon = 0x01000000
        }

        private readonly IList<TabStripTab> mTabs = new List<TabStripTab>();
        private readonly OleSize mDefaultSize = OleSize.FromPoints(144, 108);
    }
}
