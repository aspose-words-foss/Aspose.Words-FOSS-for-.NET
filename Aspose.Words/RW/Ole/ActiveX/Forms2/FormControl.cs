// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 08/10/2015 by Alexey Morozov

using System;
using System.IO;
using Aspose.Collections.Generic;
using Aspose.Ss;
using Aspose.Words.Drawing.Ole;
using Aspose.Words.Drawing.Ole.Core;

namespace Aspose.Words.Forms2
{
    /// <summary>
    /// Implements [MS-OFORMS] 2.2.10.1 FormControl.
    /// </summary>
    internal class FormControl : Forms2OleControl
    {
        /// <summary>
        /// Ctor.
        /// </summary>
        internal FormControl(string name) : base(name, new FormControlPr())
        {
            Size = mDefaultSize;
        }

        /// <summary>
        /// Reads the control from a storage.
        /// </summary>
        internal override void Read(MemoryStorage storage)
        {
            ReadFormControl(storage);
        }

        /// <summary>
        /// Writes the control to a storage.
        /// </summary>
        internal override void Write(MemoryStorage storage)
        {
            WriteFormControl(storage);
        }

        /// <summary>
        /// Reads the control from a binary reader.
        /// </summary>
        internal override void Read(BinaryReader reader)
        {
            int version = reader.ReadUInt16();
            Debug.Assert(Version == version);

            reader.ReadUInt16();        // cbForms
            FormPropMask flags = (FormPropMask)reader.ReadUInt32();

            Forms2PrReader prReader = new Forms2PrReader(reader, Pr);
            prReader.Flags = (uint) flags;

            // Data block.
            prReader.ReadData((uint)FormPropMask.BackColor, 4, Forms2Attr.BackgroundColor);
            prReader.ReadData((uint)FormPropMask.ForeColor, 4, Forms2Attr.ForegroundColor);
            prReader.ReadData((uint)FormPropMask.NextAvailableID, 4, Forms2Attr.NextAvailableID);
            prReader.ReadData((uint)FormPropMask.BooleanProperties, 4, Forms2Attr.BooleanProperties);
            prReader.ReadData((uint)FormPropMask.BorderStyle, 1, Forms2Attr.BorderStyle);
            prReader.ReadData((uint)FormPropMask.MousePointer, 1, Forms2Attr.MousePointer);
            prReader.ReadData((uint)FormPropMask.ScrollBars, 1, Forms2Attr.ScrollBars);
            prReader.ReadData((uint)FormPropMask.GroupCnt, 4, Forms2Attr.GroupCnt);
            prReader.SkipData((uint)FormPropMask.MouseIcon, 2);
            prReader.ReadData((uint)FormPropMask.Cycle, 1, Forms2Attr.Cycle);
            prReader.ReadData((uint)FormPropMask.SpecialEffect, 1, Forms2Attr.SpecialEffect);
            prReader.ReadData((uint)FormPropMask.BorderColor, 4, Forms2Attr.BorderColor);
            uint cCaption = prReader.ReadData((uint)FormPropMask.Caption, 4);
            prReader.SkipData((uint)FormPropMask.Font, 2);
            prReader.SkipData((uint)FormPropMask.Picture, 2);
            prReader.ReadData((uint)FormPropMask.Zoom, 4, Forms2Attr.Zoom);
            prReader.ReadData((uint)FormPropMask.PictureAlignment, 1, Forms2Attr.PictureAlignment);
            prReader.ReadData((uint)FormPropMask.PictureSizeMode, 1, Forms2Attr.PictureSizeMode);
            prReader.ReadData((uint)FormPropMask.ShapeCookie, 4, Forms2Attr.ShapeCookie);
            prReader.ReadData((uint)FormPropMask.DrawBuffer, 4, Forms2Attr.DrawBuffer);

            Pr.SetAttr(Forms2Attr.PictureTiling, (flags & FormPropMask.PictureTiling) != 0);

            // Extra block
            prReader.ReadBytes((uint)FormPropMask.DisplayedSize, 8, Forms2Attr.Size);
            prReader.ReadBytes((uint)FormPropMask.LogicalSize, 8, Forms2Attr.LogicalSize);
            prReader.ReadBytes((uint)FormPropMask.ScrollPosition, 8, Forms2Attr.ScrollPosition);

            prReader.ReadString((uint)FormPropMask.Caption, cCaption, Forms2Attr.Caption);

            prReader.ReadPadding(4);

            // Stream data.
            if ((flags & FormPropMask.MouseIcon) != 0)
                prReader.ReadGuidAndPicture(Forms2Attr.MouseIcon);

            if ((flags & FormPropMask.Font) != 0)
                prReader.ReadGuidAndFont();

            if ((flags & FormPropMask.Picture) != 0)
                prReader.ReadGuidAndPicture(Forms2Attr.Picture);
        }

        /// <summary>
        /// Writes the control to a binary writer.
        /// </summary>
        internal override uint Write(BinaryWriter writer)
        {
            int startPos = (int)writer.BaseStream.Position;
            Init();

            writer.Write(Version);

            // The size of data and property flags will be written later.
            writer.Seek(2, SeekOrigin.Current);// cbForm
            writer.Seek(4, SeekOrigin.Current);// FormPropMask

            Forms2PrWriter prWriter = new Forms2PrWriter(writer, Pr);
            uint flags = 0;
            flags |= prWriter.Write(Forms2Attr.BackgroundColor, 4) ? (uint)FormPropMask.BackColor : 0;
            flags |= prWriter.Write(Forms2Attr.ForegroundColor, 4) ? (uint)FormPropMask.ForeColor : 0;
            flags |= prWriter.Write(Forms2Attr.NextAvailableID, 4) ? (uint)FormPropMask.NextAvailableID : 0;
            flags |= prWriter.Write(Forms2Attr.BooleanProperties, 4) ? (uint)FormPropMask.BooleanProperties : 0;
            flags |= prWriter.Write(Forms2Attr.BorderStyle, 1) ? (uint)FormPropMask.BorderStyle : 0;
            flags |= prWriter.Write(Forms2Attr.MousePointer, 1) ? (uint)FormPropMask.MousePointer : 0;
            flags |= prWriter.Write(Forms2Attr.ScrollBars, 1) ? (uint)FormPropMask.ScrollBars : 0;
            flags |= prWriter.Write(Forms2Attr.GroupCnt, 4) ? (uint)FormPropMask.GroupCnt : 0;
            flags |= prWriter.Write(Forms2Attr.MouseIcon, 2) ? (uint)FormPropMask.MouseIcon : 0;
            flags |= prWriter.Write(Forms2Attr.Cycle, 1) ? (uint)FormPropMask.Cycle: 0;
            flags |= prWriter.Write(Forms2Attr.SpecialEffect, 1) ? (uint)FormPropMask.SpecialEffect : 0;
            flags |= prWriter.Write(Forms2Attr.BorderColor, 4) ? (uint)FormPropMask.BorderColor : 0;
            byte[] caption = prWriter.WriteCountOfBytesWithCompressionFlag(Caption);
            flags |= prWriter.Write(Forms2Attr.Font, 2) ? (uint)FormPropMask.Font : 0;
            flags |= prWriter.Write(Forms2Attr.Picture, 2) ? (uint)FormPropMask.Picture : 0;
            flags |= prWriter.Write(Forms2Attr.Zoom, 4) ? (uint)FormPropMask.Zoom : 0;
            flags |= prWriter.Write(Forms2Attr.PictureAlignment, 1) ? (uint)FormPropMask.PictureAlignment : 0;
            flags |= prWriter.Write(Forms2Attr.PictureSizeMode, 1) ? (uint)FormPropMask.PictureSizeMode : 0;
            flags |= prWriter.Write(Forms2Attr.ShapeCookie, 4) ? (uint)FormPropMask.ShapeCookie : 0;
            flags |= prWriter.Write(Forms2Attr.DrawBuffer, 4) ? (uint)FormPropMask.DrawBuffer : 0;

            flags |= ((bool)Pr.FetchAttr(Forms2Attr.PictureTiling)) ? (uint)FormPropMask.PictureTiling : 0;

            // Write padding to the end, so the size of written data is divisible by 4.
            prWriter.WritePadding(4);

            // Extra block.
            flags |= prWriter.Write(Forms2Attr.Size, 8) ? (uint)FormPropMask.DisplayedSize : 0;
            flags |= prWriter.Write(Forms2Attr.LogicalSize, 8) ? (uint)FormPropMask.LogicalSize : 0;
            flags |= prWriter.Write(Forms2Attr.ScrollPosition, 8) ? (uint)FormPropMask.ScrollPosition : 0;
            flags |= prWriter.WriteBytes(caption, 4) ? (uint)FormPropMask.Caption : 0;

            // Write padding to the end, so the size of written data is divisible by 4.
            prWriter.WritePadding(4);

            // Write cbForm and PropMasks.
            prWriter.WriteDataSizeAndPropMask(startPos, flags);

            // Stream data.
            if (((FormPropMask)flags & FormPropMask.MouseIcon) != 0)
                prWriter.WriteGuidAndPicture(MouseIcon);

            if (((FormPropMask)flags & FormPropMask.Font) != 0)
                prWriter.WriteGuidAndFont();

            if (((FormPropMask)flags & FormPropMask.Picture) != 0)
                prWriter.WriteGuidAndPicture(Picture);

            return (uint)(writer.BaseStream.Position - startPos);
        }

        /// <summary>
        /// Reads FormControl as specified in [MS-OFORMS] 2.2.10.1.
        /// </summary>
        protected void ReadFormControl(MemoryStorage storage)
        {
            MemoryStream fStream = storage.GetStreamZeroPositioned("f");
            BinaryReader reader = new BinaryReader(fStream);

            // Read DataBlock, ExtraDataBlock and StreamData.
            Read(reader);

            // Read SiteData.
            ReadSiteData(reader, storage);

            // Read DesignExData.
            FormFlags formFlags = (FormFlags)Pr.FetchAttr(Forms2Attr.BooleanProperties);
            if ((formFlags & FormFlags.DesinkPersisted) != 0)
                Forms2DesignExtender.Read(reader, Pr);
        }

        /// <summary>
        /// Writes FormControl as specified in [MS-OFORMS] 2.2.10.1.
        /// </summary>
        protected void WriteFormControl(MemoryStorage storage)
            {
            MemoryStream fStream = new MemoryStream();
            BinaryWriter writer = new BinaryWriter(fStream);

            // Write DataBlock, ExtraDataBlock and StreamData.
            Write(writer);

            // Write ChildControls to a ObjectStream.
            WriteObjectStream(storage);

            // Write SiteData.
            WriteSiteData(writer);

            // Write DesignExData.
            // DesinkPersisted flag has to be updated when we wrote DataBlock, so we can rely on it here.
            FormFlags formFlags = (FormFlags)Pr.FetchAttr(Forms2Attr.BooleanProperties);
            if ((formFlags & FormFlags.DesinkPersisted) != 0)
                Forms2DesignExtender.Write(writer, Pr);

            storage["f"] = fStream;
            }

        /// <summary>
        /// Writes SiteClasses as specified in [MS-OFORMS] 2.2.10.6 FormSiteData.
        /// </summary>
        ///<remarks>For a moment we do not write ClassTables of Sites.</remarks>
        private void WriteSiteClasses(BinaryWriter writer)
        {
            FormFlags formFlags = (FormFlags)Pr.FetchAttr(Forms2Attr.BooleanProperties);
            if ((formFlags & FormFlags.DontSaveClassTable) == 0)
                writer.Write((ushort)0);
        }

        /// <summary>
        /// Writes [MS-OFORMS] 2.2.10.7 FormObjectDepthTypeCount.
        /// </summary>
        /// <remarks>We do not use this structure for a while, but MUST read it to know its length.</remarks>
        /// <returns>Number of read bytes.</returns>
        private static int ReadDepthTypeCount(BinaryReader reader)
        {
            long startPos = reader.BaseStream.Position;

            // Ignore depth for a while.
            reader.ReadByte();

            byte typeOrCount = reader.ReadByte();
            bool fCount = (typeOrCount & 0x80) != 0;
            // Ignore SITE_TYPE for a while.
            if (fCount)
                reader.ReadByte();

            return (int)(reader.BaseStream.Position - startPos);
        }

        /// <summary>
        /// Writes [MS-OFORMS] 2.2.10.7 FormObjectDepthTypeCount.
        /// </summary>
        /// <returns>Number of written bytes.</returns>
        private static int WriteDepthTypeCount(BinaryWriter writer, int count)
        {
            long startPos = writer.BaseStream.Position;

            // Ignore depth for a while.
            const byte depth = 0x00;
            writer.Write(depth);

            // Not sure, but seems Word writes fCount = true, when count > 1.
            bool fCount = (count > 1);
            byte typeOrCount = (byte)(count | (fCount ? 0x80 : 0x00));
            writer.Write(typeOrCount);

            if (fCount)
            {
                // 2.2.10.8 SITE_TYPE. Must be always set to 1.
                const byte siteType = 1;
                writer.Write(siteType);
            }

            return (int)(writer.BaseStream.Position - startPos);
        }

        /// <summary>
        /// Reads FormSiteData as specified in [MS-OFORMS] 2.2.10.6.
        /// </summary>
        private void ReadSiteData(BinaryReader reader, MemoryStorage storage)
        {
            ReadSiteClasses(reader);

            int countOfSites = (int)reader.ReadUInt32();

            // Skip CountOfBytes.
            reader.ReadUInt32();

            // Empty FormControl, nothing to read.
            if (countOfSites == 0)
                return;

            // Read SiteDepthsAndTypes.
            int len = ReadDepthTypeCount(reader);

            // Read ArrayPadding.
            int pad = (len % 4 == 0) ? 0 : 4 - len % 4;
            reader.BaseStream.Position += pad;

            // Read Sites.
            OleSiteConcreteControl[] siteControls = new OleSiteConcreteControl[countOfSites];
            for (int i = 0; i < countOfSites; i++)
                siteControls[i] = new OleSiteConcreteControl(reader);

            // Read child controls.
            MemoryStream objectStream = storage.GetStreamZeroPositioned("o");
            BinaryReader objectReader = new BinaryReader(objectStream);

            int position = 0;
            foreach (OleSiteConcreteControl siteControl in siteControls)
            {
                ChildNodes.Add(ReadChildControl(siteControl, objectReader, storage));

                position += siteControl.ObjectStreamSize;
                objectReader.BaseStream.Position = position;
            }
        }

        /// <summary>
        /// Writes site data to a binary writer.
        /// </summary>
        private void WriteSiteData(BinaryWriter writer)
        {
            // Write ClassTables.
            WriteSiteClasses(writer);

            // CountOfSites.
            writer.Write(ChildNodes.Count);

            // CountOfBytes (write this later).
            int countOfBytesPos = (int)writer.BaseStream.Position;
            writer.Write((uint)0x0);

            // Empty FormControl, nothing to write.
            if (ChildNodes.Count == 0)
                return;

            // Write SiteDepthsAndTypes.
            int len =  WriteDepthTypeCount(writer, ChildNodes.Count);

            // Write ArrayPadding.
            int pad = ((len % 4) == 0) ? 0 : 4 - (len % 4);
            writer.Write(new byte[pad]);

            // Write Sites.
            for (int i = 0; i < ChildNodes.Count; i++)
                OleSiteConcreteControl.Write(writer, ChildNodes[i]);

            // Write CountOfBytes as the sum of the sizes, in bytes, of SiteDepthsAndTypes, ArrayPadding, and Sites.
            int countOfBytes = ((int)writer.BaseStream.Position - countOfBytesPos - 4);
            writer.Seek(countOfBytesPos, SeekOrigin.Begin);
            writer.Write(countOfBytes);
            writer.Seek(0, SeekOrigin.End);
        }

        /// <summary>
        /// Reads a child control for a site from a binary reader or storage.
        /// </summary>
        private static Forms2OleControl ReadChildControl(
            OleSiteConcreteControl siteControl, BinaryReader objectReader, MemoryStorage storage)
        {
            Forms2OleControl forms2OleControl = siteControl.GetForms2OleControl();
            if (forms2OleControl.IsComposite)
                forms2OleControl.Read((MemoryStorage)storage[siteControl.StorageName]);
            else
                forms2OleControl.Read(objectReader);

            return forms2OleControl;
        }

        /// <summary>
        /// Reads site classes from a binary reader.
        /// </summary>
        private void ReadSiteClasses(BinaryReader reader)
        {
            FormFlags formFlags = (FormFlags)Pr.FetchAttr(Forms2Attr.BooleanProperties);
            int countOfSiteClassInfo = ((formFlags & FormFlags.DontSaveClassTable) != 0) ? 0 : reader.ReadUInt16();

            SiteClassInfo[] sites = new SiteClassInfo[countOfSiteClassInfo];
            for (int i = 0; i < countOfSiteClassInfo; i++)
            {
                sites[i] = new SiteClassInfo(reader);
            }
        }

        /// <summary>
        /// Writes ObjectStream to a storage.
        /// </summary>
        private void WriteObjectStream(MemoryStorage storage)
        {
            MemoryStream oStream = new MemoryStream();
            BinaryWriter oWriter = new BinaryWriter(oStream);

            HashSetGeneric<int> usedIds = new HashSetGeneric<int>();
            for (int i = 0; i < ChildNodes.Count; i++)
            {
                EnsureUniqueId(ChildNodes[i], usedIds);

                if (ChildNodes[i].IsComposite)
                {
                    // MemoryStorage childStorage = OleUtil.CreateOleObject(ChildNodes[i]).Data;
                    MemoryStorage childStorage = ((IEmbeddedObject)ChildNodes[i]).GetOleObject().Data;
                    // [MS-OFORMS] 2.1.2.2.2 Embedded Parents.
                    string childStorageName = string.Format("i{0:d2}", ChildNodes[i].IdInternal);
                    storage[childStorageName] = childStorage;
                }
                else
                {
                    uint size = ChildNodes[i].Write(oWriter);
                    // Update ObjectStreamSize, as we need it when we will write Sites.
                    ChildNodes[i].ObjectStreamSize = size;
                }
            }

            storage["o"] = oStream;
        }

        /// <summary>
        /// Initializes FormControl properties that must persist in output stream.
        /// </summary>
        private void Init()
        {
            // Update DesinkPersisted flag.
            FormFlags formFlags = (FormFlags)Pr.FetchAttr(Forms2Attr.BooleanProperties);
            if (Forms2DesignExtender.HasAllDefaultValues(Pr))
                formFlags &= ~FormFlags.DesinkPersisted;
            else
                formFlags |= FormFlags.DesinkPersisted;
            Pr[Forms2Attr.BooleanProperties] = formFlags;

            // // Each Form MUST persist a value for this property.
            // // MUST be in the range from 16000 through 1048576.
            if (!Pr.Contains(Forms2Attr.DrawBuffer))
                Pr.SetAttr(Forms2Attr.DrawBuffer, (uint)32000);

            if (!Pr.Contains(Forms2Attr.LogicalSize))
                Pr.SetAttr(Forms2Attr.LogicalSize, new OleSize(0, 0));

            // Note, it is very important to set 'PromoteControls' flag for Frame, MultiPage and Page controls,
            // otherwise Word ignores properties of these controls and creates them 'by default'.
            // See [MS-OFORMS] 2.5.4.1 SITE_FLAG.
            SiteFlag siteFlag = (Pr.Contains(Forms2Attr.BitFlagsSite))
                ? (SiteFlag)Pr[Forms2Attr.BitFlagsSite]
                : SiteFlag.AutoSize | SiteFlag.TabStop | SiteFlag.Visible;
            Pr[Forms2Attr.BitFlagsSite] = siteFlag | SiteFlag.PromoteControls;
        }

        /// <summary>
        /// Ensures uniqueness of <see cref="Forms2OleControl.IdInternal"/> property of the control.
        /// </summary>
        /// <remarks>
        /// The ID of the control can be specified only in the properties of
        /// MS-OFORMS 2.2.10.12.3 SiteDataBlock of the OleSiteConcreteControl.
        /// So, we need ensure the uniqueness of the IDs only within the FormControl object.
        /// </remarks>
        private static void EnsureUniqueId(Forms2OleControl control, HashSetGeneric<int> usedIds)
        {
            int id = control.IdInternal;
            while (usedIds.Contains(id))
                id++;

            usedIds.Add(id);
            control.IdInternal = id;
        }

        /// <summary>
        /// Gets type of Forms 2.0 control.
        /// </summary>
        public override Forms2OleControlType Type
        {
            get { return Forms2OleControlType.Form; }
        }

        /// <summary>
        /// Gets ClsidCacheIndex.
        /// </summary>
        internal override ClsidCacheIndex ClsidCacheIndex
        {
            get { return ClsidCacheIndex.Form; }
        }

        /// <summary>
        /// Gets a boolean value indicating either the Forms2 OleControl is composite.
        /// </summary>
        internal override bool IsComposite
        {
            get { return true; }
        }

        /// <summary>
        /// Gets all immediate child Forms2 OLE controls.
        /// </summary>
        public override Forms2OleControlCollection ChildNodes
        {
            get { return mChildNodes; }
        }

        /// <summary>
        /// Gets a Clsid of the embedded object.
        /// </summary>
        protected override string ClsidVirtual
        {
            get { return FormControlClsid; }
        }

        /// <summary>
        /// Implements [MS-OFORMS] 2.2.10.2 FormPropMask.
        /// </summary>
        [Flags]
        private enum FormPropMask : uint
        {
            Unused1 = 0x00000001,
            BackColor = 0x00000002,
            ForeColor = 0x00000004,
            NextAvailableID = 0x00000008,
            Unused2 = 0x00000010,
            Unused3 = 0x00000020,
            BooleanProperties = 0x00000040,
            BorderStyle = 0x00000080,
            MousePointer = 0x00000100,
            ScrollBars = 0x00000200,
            DisplayedSize = 0x00000400,
            LogicalSize = 0x00000800,
            ScrollPosition = 0x00001000,
            GroupCnt = 0x00002000,
            Reserved = 0x00004000,
            MouseIcon = 0x00008000,
            Cycle = 0x00010000,
            SpecialEffect = 0x00020000,
            BorderColor = 0x00040000,
            Caption = 0x00080000,
            Font = 0x00100000,
            Picture = 0x00200000,
            Zoom = 0x00400000,
            PictureAlignment = 0x00800000,
            PictureTiling = 0x01000000,
            PictureSizeMode = 0x02000000,
            ShapeCookie = 0x04000000,
            DrawBuffer = 0x08000000
        }

        private readonly Forms2OleControlCollection mChildNodes = new Forms2OleControlCollection();
        private const ushort Version = 0x0400;
        private readonly OleSize mDefaultSize = OleSize.FromPoints(216, 216);
    }
}
