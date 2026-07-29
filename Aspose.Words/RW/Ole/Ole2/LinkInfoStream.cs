// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 06/04/2012 by Alexey Morozov

using System.IO;
using Aspose.Ss;
using Aspose.Words.Nrx;

namespace Aspose.Words.RW.Ole.Ole2
{
    /// <summary>
    /// Implements reading/writing of LinkInfo stream.
    /// </summary>
    /// <remarks>
    /// AM. There is no information about this stream in spec. It's good thing that it is simple enough.
    /// </remarks>
    internal class LinkInfoStream : Ole2StreamBase
    {
        internal static LinkInfoStream Read(MemoryStorage storage)
        {
            return storage.ContainsKey(LinkInfoStreamName) ? new LinkInfoStream(new BinaryReader(storage.GetStreamZeroPositioned(LinkInfoStreamName))) : null;
        }

        internal LinkInfoStream(string topicName, string itemName)
        {
            mTopicName = topicName;
            mItemName = itemName;
        }

        private LinkInfoStream(BinaryReader reader)
        {
            int topicNameLength = reader.ReadInt16();
            mTopicName = System.Text.Encoding.GetEncoding(1251).GetString(reader.ReadBytes(topicNameLength));
            mTopicName = mTopicName.Trim('\0');

            int itemNameLength = reader.ReadInt16();
            mItemName = System.Text.Encoding.GetEncoding(1251).GetString(reader.ReadBytes(itemNameLength));
            mItemName = mItemName.Trim('\0');

            // Seems here is Unicode marker 0xccccbbbb and the same TopicName/ItemName in Unicode.
        }

        protected override void Write(BinaryWriter writer)
        {
            DocBinaryWriter.WriteAnsiString(writer, StringUtil.HasChars(mTopicName) ? mTopicName : "", 1251, true);
            DocBinaryWriter.WriteAnsiString(writer, StringUtil.HasChars(mItemName) ? mItemName : "", 1251, true);
        }

        protected override string Name
        {
            get { return LinkInfoStreamName; }
        }

        internal string TopicName
        {
            get { return mTopicName; }
        }

        internal string ItemName
        {
            get { return mItemName; }
        }

        private readonly string mTopicName;
        private readonly string mItemName;
    }
}
