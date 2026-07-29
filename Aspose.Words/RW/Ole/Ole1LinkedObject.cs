// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 02/07/2012 by Alexey Morozov

using System;
using System.IO;
using Aspose.Words.Drawing;
using Aspose.Words.Drawing.Core;

namespace Aspose.Words.RW.Ole
{
    /// <summary>
    /// Implements [MS-OLEDS] 2.2.6 LinkedObject.
    /// </summary>
    internal class Ole1LinkedObject : Ole1Object
    {
        internal Ole1LinkedObject(OleFormat oleFormat, byte[] metafileData)
        {
            mClassName = oleFormat.ProgId;
            mTopicName = oleFormat.SourceFullName;
            mItemName = oleFormat.SourceItem;
            mAutoUpdate = oleFormat.AutoUpdate;

            mPresentationObject = (metafileData != null) ? 
                (PresentationObject)new PresentationObjectMetafile(metafileData) : 
                new PresentationObjectEmpty();
        }

        internal Ole1LinkedObject(ObjectHeader objectHeader, BinaryReader reader)
        {
            mClassName = objectHeader.ClassName;
            mTopicName = objectHeader.TopicName;
            mItemName = objectHeader.ItemName;

            mNetworkName = OleUtil.ReadLengthPrefixedAnsiString(reader);

            int reserved = reader.ReadInt32();
            Debug.Assert(reserved == 0x00);

            int linkUpdateOptions = reader.ReadInt32();
            // There is something wrong with spec. 
            // It defines that linkUpdateOptions is OLEUPDATE type which defined as:
            // OLEUPDATE_ALWAYS   = 1
            // OLEUPDATE_ONCALL   = 3
            // But values only seen is 0 and 2.
            mAutoUpdate = linkUpdateOptions == 0x00;
            Debug.Assert(linkUpdateOptions == 0x00 || linkUpdateOptions == 0x02);

            mPresentationObject = PresentationObject.Read(reader);
        }

        internal override void Write(BinaryWriter writer)
        {
            ObjectHeader objectHeader = new ObjectHeader(OleObjectType.Linked, mClassName, mTopicName, mItemName);
            objectHeader.Write(writer);

            OleUtil.WriteLengthPrefixedAnsiString(writer, mNetworkName);
            // Reserved. Must be 0x00000000.
            writer.Write((Int32)0x00);

            writer.Write(mAutoUpdate ? 0x00 : 0x02);
            
            mPresentationObject.Write(writer);
        }

        internal override OleObjectType ObjectType
        {
            get { return OleObjectType.Linked; }
        }

        /// <summary>
        /// Specifies the absolute path name of the linked file.
        /// </summary>
        internal string TopicName
        {
            get { return mTopicName; }
        }

        /// <summary>
        /// Specifies a string that is used by the application or higher-level protocol 
        /// to identify the item within the file to which is being linked.
        /// </summary>
        internal string ItemName
        {
            get { return mItemName; }
        }

        /// <summary>
        /// If the <see cref="TopicName" /> field contains a path that starts with a drive letter and if the drive letter is for a remote drive, 
        /// the NetworkName field MUST contain the path name of the linked file in the Universal Naming Convention (UNC) format.
        /// </summary>
        internal string NetworkName
        {
            get { return mNetworkName; }
        }

        internal bool AutoUpdate
        {
            get { return mAutoUpdate; }
        }

        /// <summary>
        /// Specifies OLE1.0 presentation data to display this object in container applications.
        /// </summary>
        internal PresentationObject PresentationObject
        {
            get { return mPresentationObject; }
        }

        private readonly string mTopicName;
        private readonly string mItemName;
        private readonly string mNetworkName;
        private readonly bool mAutoUpdate;
        private readonly PresentationObject mPresentationObject;
    }
}
