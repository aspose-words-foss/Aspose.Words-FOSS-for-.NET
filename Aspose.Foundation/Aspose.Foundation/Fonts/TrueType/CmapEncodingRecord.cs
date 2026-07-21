// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 01/05/2012 by Konstantin Kornilov

namespace Aspose.Fonts.TrueType
{
    /// <summary>
    /// Represents encoding record in 'cmap' table.
    /// </summary>
    internal class CmapEncodingRecord
    {
        /// <summary>
        /// Ctor.
        /// </summary>
        public CmapEncodingRecord(int platformId, int encodingId, int format, long position)
        {
            mPlatformId = platformId;
            mEncodingId = encodingId;
            mFormat = format;
            mPosition = position;
        }

        /// <summary>
        /// Platform ID.
        /// </summary>
        public int PlatformId
        {
            get { return mPlatformId; }
        }

        /// <summary>
        /// Platform-specific encoding ID.
        /// </summary>
        public int EncodingId
        {
            get { return mEncodingId; }
        }

        /// <summary>
        /// Encoding record format.
        /// </summary>
        public int Format
        {
            get { return mFormat; }
        }

        /// <summary>
        /// Position in the stream of the subtable for this encoding.
        /// </summary>
        public long Position
        {
            get { return mPosition; }
        }

        private readonly int mPlatformId;
        private readonly int mEncodingId;
        private readonly int mFormat;
        private readonly long mPosition;
    }
}
