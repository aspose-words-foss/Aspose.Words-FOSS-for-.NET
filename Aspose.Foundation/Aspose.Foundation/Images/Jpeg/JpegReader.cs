// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 20/07/2024 by Konstantin Kornilov

using Aspose.IO;

namespace Aspose.Images.Jpeg
{
    internal class JpegReader
    {
        public JpegReader(BigEndianBinaryReader reader)
        {
            mReader = reader;
        }

        public bool GoToNextMarker()
        {
            // All JPEG markers are 2 bytes length. The leading byte is always 0xFF, and the trailing byte is not equal
            // to 0x00 or 0xFF. Areas before markers may be filled with 0xFF bytes.

            // WORDSNET-6573 Normally, at the moment the reader is positioned right before the next marker.
            // But if JPEG file is broken and contains invalid segment sizes, we can find ourselves anywhere in the middle
            // of a segment. In this case, we search for the nearest following marker to recover image processing.

            short leadingByte = 0;
            short trailingByte = 0;
            bool markerFound = false;

            while (!markerFound)
            {
                // Find the next possible leading byte.
                while (leadingByte != 0xFF)
                {
                    if (IsEndOfStream)
                        break;

                    leadingByte = mReader.ReadByte();
                }

                if (IsEndOfStream)
                    break;

                // A possible leading byte found. Read and check the trailing byte.
                trailingByte = mReader.ReadByte();
                if ((trailingByte == 0) || (trailingByte == 0xFF))
                {
                    // The trailing byte is invalid, and the 2-byte sequence is not a marker. Let's advance one byte
                    // and continue searching.
                    leadingByte = trailingByte;
                    trailingByte = 0;
                }
                else
                {
                    // The trailing byte is valid. We found a marker.
                    markerFound = true;
                }
            }

            CurrentMarker = (ushort)(markerFound ? (0xFF00 | (trailingByte & 0xFF)) : NoMarker);
            return !IsEndOfStream;
        }

        public ushort CurrentMarker { get; private set; }

        public bool MarkerFound
        {
            get { return CurrentMarker != NoMarker; }
        }

        public bool IsEoiMarker
        {
            get { return CurrentMarker == MarkerEoi; }
        }

        public bool IsEndOfStream
        {
            get { return mReader.IsEndOfStream; }
        }

        private readonly BigEndianBinaryReader mReader;

        public const ushort MarkerSoi = 0xFFD8;
        public const ushort MarkerEoi = 0xFFD9;
        public const ushort MarkerExif = 0xFFE1;
        public const ushort NoMarker = 0;
    }
}
