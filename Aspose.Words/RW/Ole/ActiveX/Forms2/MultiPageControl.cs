// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 08/10/2015 by Alexey Morozov

using System;
using System.IO;
using Aspose.Ss;
using Aspose.Words.Drawing.Ole;

namespace Aspose.Words.Forms2
{
    /// <summary>
    /// The MultiPage control presents multiple screens of information as a single set.
    /// </summary>
    internal class MultiPageControl : FormControl
    {
        /// <summary>
        /// Ctor.
        /// </summary>
        internal MultiPageControl(string name) : base(name)
        {
            Size = mDefaultSize;
        }

#if CPLUSPLUS
        internal override void Read(BinaryReader reader)
        {
            base.Read(reader);
        }

        internal override uint Write(BinaryWriter writer)
        {
            return base.Write(writer);
        }
#endif

        /// <summary>
        /// Reads the control from a storage.
        /// </summary>
        internal override void Read(MemoryStorage storage)
        {
            ReadFormControl(storage);

            MemoryStream xStream = storage.GetStreamZeroPositioned("x");
            BinaryReader xReader = new BinaryReader(xStream);

            // The first PageProperties in the array MUST be ignored. [MS-OFORMS] 2.1.2.3 MultiPage Control Structure.
            ReadPageProperties(xReader, new FormControl(""));

            // Read the rest of page properties.
            foreach (Forms2OleControl childNode in ChildNodes)
            {
                if (childNode.Type == Forms2OleControlType.Form)
                    ReadPageProperties(xReader, childNode);
            }

            ReadMultiPageProperties(xReader);
        }

        /// <summary>
        /// Writes the control to a storage.
        /// </summary>
        internal override void Write(MemoryStorage storage)
        {
            WriteFormControl(storage);

            MemoryStream xStream = new MemoryStream();
            BinaryWriter xWriter = new BinaryWriter(xStream);

            // The first PageProperties in the array MUST be ignored, so write dummy page.
            WritePageProperties(xWriter, new FormControl(""));

            // Write pages.
            int pagesCount = 0;
            foreach (Forms2OleControl childNode in ChildNodes)
            {
                if (childNode.Type == Forms2OleControlType.Form)
                {
                    WritePageProperties(xWriter, childNode);
                    pagesCount++;
                }
            }

            // Update pages count.
            if (pagesCount > 0)
                Pr[Forms2Attr.PageCount] = pagesCount;

            WriteMultiPageProperties(xWriter);

            storage["x"] = xStream;
        }

        /// <summary>
        /// Reads page properties from a binary reader.
        /// </summary>
        private static void ReadPageProperties(BinaryReader reader, Forms2OleControl page)
        {
            int version = reader.ReadUInt16();
            Debug.Assert(version == Forms2Version);

            // cbPage
            reader.ReadUInt16();
            PagePropMask flags = (PagePropMask)reader.ReadUInt32();

            Forms2PrReader prReader = new Forms2PrReader(reader, page.Pr);
            prReader.Flags = (uint) flags;

            // Data block.
            prReader.ReadData((uint)PagePropMask.TransitionEffect, 4, Forms2Attr.TransitionEffect);
            prReader.ReadData((uint)PagePropMask.TransitionPeriod, 4, Forms2Attr.TransitionPeriod);
        }

        /// <summary>
        /// Writes page properties to a binary writer.
        /// </summary>
        private static void WritePageProperties(BinaryWriter writer, Forms2OleControl page)
        {
            int startPos = (int)writer.BaseStream.Position;

            writer.Write(Forms2Version);

            // The size of data and property flags will be written later.
            writer.Seek(2, SeekOrigin.Current);// cbPage
            writer.Seek(4, SeekOrigin.Current);// PagePropMask

            Forms2PrWriter prWriter = new Forms2PrWriter(writer, page.Pr);

            uint flags = 0;
            flags |= prWriter.Write(Forms2Attr.TransitionEffect, 4) ? (uint)PagePropMask.TransitionEffect : 0;
            flags |= prWriter.Write(Forms2Attr.TransitionPeriod, 4) ? (uint)PagePropMask.TransitionPeriod : 0;

            // Write cbPage and PropMask.
            prWriter.WriteDataSizeAndPropMask(startPos, flags);
        }

        /// <summary>
        /// Reads multi page properties from a binary reader.
        /// </summary>
        private void ReadMultiPageProperties(BinaryReader reader)
        {
            int version = reader.ReadUInt16();
            Debug.Assert(version == Forms2Version);

            // cbMultiPage
            reader.ReadUInt16();
            MultiPagePropMask flags = (MultiPagePropMask)reader.ReadUInt32();

            Forms2PrReader prReader = new Forms2PrReader(reader, Pr);
            prReader.Flags = (uint) flags;

            // Data block.
            prReader.ReadData((uint)MultiPagePropMask.PageCount, 4, Forms2Attr.PageCount);
            // This is ID of child TabStrip. I think we can skip it, as it was already set when we read Sites.
            prReader.SkipData((uint)MultiPagePropMask.ID, 4);

            // Set Flag if this property is NOT the file format default. [MS-OFORMS] 2.2.6.2 MultiPagePropertiesPropMask.
            Pr.SetAttrIfNotDefault(Forms2Attr.Flags, ((flags & MultiPagePropMask.Flags) == 0));

            // There are IDs of the Pages at the very end of this stream, but I think we can skip them,
            // as we already set them accordingly when we read Sites.
        }

        /// <summary>
        /// Writes multi page properties to a binary writer.
        /// </summary>
        private void WriteMultiPageProperties(BinaryWriter writer)
        {
            int startPos = (int)writer.BaseStream.Position;

            writer.Write(Forms2Version);

            // The size of data and property flags will be written later.
            writer.Seek(2, SeekOrigin.Current);// cbMultiPage
            writer.Seek(4, SeekOrigin.Current);// MultiPagePropMask

            Forms2PrWriter prWriter = new Forms2PrWriter(writer, Pr);

            uint flags = 0;
            flags |= prWriter.Write(Forms2Attr.PageCount, 4) ? (uint)MultiPagePropMask.PageCount : 0;

            // Here we need to write ID of a child TabStrip.
            foreach (Forms2OleControl childNode in ChildNodes)
            {
                if (childNode.Type == Forms2OleControlType.TabStrip)
                {
                    prWriter.Write((uint)childNode.IdInternal);
                    flags |= (uint)MultiPagePropMask.ID;

                    break;
                }
            }

            // Specifies whether the value of the Flags property is NOT the file format default.
            flags |= (!Pr.IsDefaultValue(Forms2Attr.Flags)) ? (uint)MultiPagePropMask.Flags : 0;

            // Write cbMultiPage and PropMask.
            prWriter.WriteDataSizeAndPropMask(startPos, flags);

            // At the very end we must write IDs of Pages.
            foreach (Forms2OleControl childNode in ChildNodes)
            {
                if (childNode.Type == Forms2OleControlType.Form)
                {
                    prWriter.Write((uint)childNode.IdInternal);
                }
            }

        }

        /// <summary>
        /// Gets type of Forms 2.0 control.
        /// </summary>
        public override Forms2OleControlType Type
        {
            get { return Forms2OleControlType.MultiPage; }
        }

        /// <summary>
        /// Gets a boolean value indicating either the Forms2 OleControl is composite.
        /// </summary>
        internal override bool IsComposite
        {
            get { return true; }
        }

        /// <summary>
        /// Gets a collection of pages of this MultiPage control.
        /// The pages in collection are represented as <see cref="FormControl"/> objects.
        /// </summary>
        internal PageCollection Pages
        {
            get
            {
                if (mPages == null)
                    mPages = new PageCollection(ChildNodes);

                return mPages;
            }
        }

        /// <summary>
        /// Gets Forms 2.0 control UserType.
        /// </summary>
        protected override string UserType
        {
            get { return "Microsoft Forms 2.0 Form"; }
        }

        /// <summary>
        /// Gets a Clsid of the embedded object.
        /// </summary>
        protected override string ClsidVirtual
        {
            get { return MultiPageControlClsid; }
        }

        /// <summary>
        /// Implements [MS-OFORMS] 2.2.6.4.2 PagePropMask.
        /// </summary>
        [Flags]
        private enum PagePropMask : uint
        {
            Unused1 = 0x00000001,
            TransitionEffect= 0x00000002,
            TransitionPeriod = 0x00000004,
        }

        /// <summary>
        /// Implements [MS-OFORMS] 2.2.6.2 MultiPagePropertiesPropMask.
        /// </summary>
        [Flags]
        private enum MultiPagePropMask : uint
        {
            Unused1 = 0x00000001,
            PageCount = 0x00000002,
            ID = 0x00000004,
            Flags = 0x00000008
        }

        private PageCollection mPages;
        private readonly OleSize mDefaultSize = OleSize.FromPoints(144, 108);
    }
}
