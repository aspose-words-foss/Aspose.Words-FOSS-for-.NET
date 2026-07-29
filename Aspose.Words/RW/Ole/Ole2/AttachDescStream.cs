// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 15/08/2013 by Alexey Morozov

using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Text;
using Aspose.Ss;

namespace Aspose.Words.RW.Ole.Ole2
{
    /// <summary>
    /// Represent AttachDesc stream which is description for Outlook Attach object.
    /// http://msdn.microsoft.com/en-us/library/ee157577(v=exchg.80).aspx
    /// </summary>
    /// <remarks>
    /// AM. For a while we ignore almost whole data in this stream except file extension information.
    /// </remarks>
    [SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable",
        Justification = "It isn't necessary to dispose BinaryReader instances in this case. Also it's an internal class.")]
    internal class AttachDescStream : Ole2StreamBase
    {
        internal static AttachDescStream Read(MemoryStorage storage)
        {
            return storage.ContainsKey(AttachDescStreamName) ? new AttachDescStream(storage.GetStreamZeroPositioned(AttachDescStreamName)) : null;
        }

        private AttachDescStream(MemoryStream stream)
        {
            stream.Position = 0;

            mReader = new BinaryReader(stream);

            int version = mReader.ReadUInt16();

            // Stop reading if stream version is unknown.
            if (!IsSupportedVersion(version))
                return;

            ReadAnsiString();       // ansiLongPathName 
            ReadAnsiString();       // ansiPathName 
            ReadAnsiString();       // ansiDisplayName
            ReadAnsiString();       // ansiLongFileName
            mAnsiFileName = ReadAnsiString();

            mAnsiExtension = ReadAnsiString();

            mReader.ReadBytes(8);   // creationTime
            mReader.ReadBytes(8);   // lastModifiedTime
            mReader.ReadInt32();    // attachMethod

            if (version == RightsManagedMessageVersion)
            {
                ReadUnicodeString();    // contentId
                ReadUnicodeString();    // contentLocation
                ReadUnicodeString();    // unicodeLongPathName

                ReadUnicodeString();    // unicodePathName
                ReadUnicodeString();    // unicodeDisplayName
                ReadUnicodeString();    // unicodeLongFileName
                mUnicodeFileName = ReadUnicodeString();
                
                mUnicodeExtension = ReadUnicodeString();

                ReadUnicodeString();    // imagePreviewSmall
                ReadUnicodeString();    // imagePreviewMedium
                ReadUnicodeString();    // imagePreviewLarge

                mReader.ReadInt32();    // rendered
                mReader.ReadInt32();    // flags
            }

            Debug.Assert(stream.Position == stream.Length);
        }

        protected override void Write(BinaryWriter writer)
        {
            // AM. We never write this so implementation is postponed.
            throw new System.NotImplementedException();
        }

        protected override string Name
        {
            get { return AttachDescStreamName; }
        }

        /// <summary>
        /// Extension of attached file.
        /// </summary>
        internal string Extension
        {
            get { return StringUtil.HasChars(mUnicodeExtension) ? mUnicodeExtension : mAnsiExtension; }
        }

        /// <summary>
        /// File name of attached file.
        /// </summary>
        internal string FileName
        {
            get { return StringUtil.HasChars(mUnicodeFileName) ? mUnicodeFileName : mAnsiFileName; }
        }

        private static bool IsSupportedVersion(int version)
        {
            return (version == RightsManagedMessageVersion) || (version == UndocumentedMessageVersion);
        }

        /// <summary>
        /// Reads 2.2.4.2 Non-Unicode LPString. Prefixed with 1-byte length, ANSI characters, not null terminated.
        /// </summary>
        private string ReadAnsiString()
        {
            int length = mReader.ReadByte();
            return (length > 0) ? Encoding.GetEncoding(1251).GetString(mReader.ReadBytes(length)) : "";
        }

        /// <summary>
        /// Reads 2.2.4.1 LPString. Prefixed with 1-byte length, Unicode characters, not null terminated.
        /// </summary>
        /// <returns></returns>
        private string ReadUnicodeString()
        {
            int length = mReader.ReadByte();
            return (length > 0) ? Encoding.Unicode.GetString(mReader.ReadBytes(length*2)) : "";
        }

        private readonly BinaryReader mReader;

        private readonly string mAnsiExtension;
        private readonly string mUnicodeExtension;

        private readonly string mAnsiFileName;
        private readonly string mUnicodeFileName;


        /// <summary>
        /// Seen in TestJira9338. Undocumented, seems to be limited version of rights managed message.
        /// </summary>
        private const int UndocumentedMessageVersion = 0x0100;

        private const int RightsManagedMessageVersion = 0x0203;
    }
}
