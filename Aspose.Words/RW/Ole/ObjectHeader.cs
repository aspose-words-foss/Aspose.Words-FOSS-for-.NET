// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 23/10/2011 by Alexey Morozov

using System.IO;
using Aspose.Words.Drawing.Core;

namespace Aspose.Words.RW.Ole
{
    /// <summary>
    /// Implements [MS-OLEDS] 2.2.4 ObjectHeader.
    /// </summary>
    internal class ObjectHeader
    {
        internal ObjectHeader(OleObjectType oleObjectType, string className, string topicName, string itemName)
        {
            mOleObjectType = oleObjectType;
            mClassName = className;
            mTopicName = topicName;
            mItemName = itemName;
        }

        internal static ObjectHeader Read(BinaryReader reader)
        {
            // Ignore oleVersion.
            reader.ReadInt32();
            int formatId = reader.ReadInt32();

            OleObjectType oleObjectType;

            switch (formatId)
            {
                case 0x01:
                {
                    oleObjectType = OleObjectType.Linked;
                    break;
                }
                case 0x02:
                case 0x03:
                {
                    oleObjectType = OleObjectType.Embedded;
                    break;
                }
                default:
                {
                    return null;
                }
            }

            string className = OleUtil.ReadLengthPrefixedAnsiString(reader);
            string topicName = string.Empty;
            string itemName = string.Empty;

            if (oleObjectType == OleObjectType.Linked)
            {
                topicName = OleUtil.ReadLengthPrefixedAnsiString(reader);
                itemName = OleUtil.ReadLengthPrefixedAnsiString(reader);
            }
            else
            {
                // If the ObjectHeader structure is contained by an EmbeddedObject structure, the TopicName and ItemName fields SHOULD 
                // contain an empty string and MUST be ignored on processing.
                reader.ReadInt32();
                reader.ReadInt32();
            }

            return new ObjectHeader(oleObjectType, className, topicName, itemName);
        }

        internal void Write(BinaryWriter writer)
        {
            writer.Write(OleUtil.OleVersion);
            writer.Write(mOleObjectType == OleObjectType.Linked ? 0x01 : 0x02);

            OleUtil.WriteLengthPrefixedAnsiStringCore(writer, mClassName);
            OleUtil.WriteLengthPrefixedAnsiString(writer, mTopicName);
            OleUtil.WriteLengthPrefixedAnsiString(writer, mItemName);
        }

        /// <summary>
        /// Possible values are limited to OleObjectType.Linked and OleObjectType.Embedded.
        /// </summary>
        internal OleObjectType OleObjectType
        {
            get { return mOleObjectType; }
        }

        /// <summary>
        /// Contains a value identifying the creating application.
        /// </summary>
        internal string ClassName
        {
            get { return mClassName; }
        }

        /// <summary>
        /// Contains the absolute path name of the linked file. 
        /// The path name either MUST start with a drive letter or 
        /// MUST be in the Universal Naming Convention (UNC) format.
        /// </summary>
        internal string TopicName
        {
            get { return mTopicName; }
        }

        /// <summary>
        /// Contains a string that is used by the application or 
        /// higher-level protocol to identify the item within the file to which is being linked.
        /// </summary>
        internal string ItemName
        {
            get { return mItemName; }
        }

        private readonly OleObjectType mOleObjectType;
        private readonly string mClassName;
        private readonly string mTopicName;
        private readonly string mItemName;
    }
}
