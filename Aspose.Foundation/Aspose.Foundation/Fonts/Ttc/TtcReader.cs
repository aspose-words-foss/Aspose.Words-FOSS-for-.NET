// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 01/10/2015 by Konstantin Kornilov

using Aspose.IO;

namespace Aspose.Fonts.Ttc
{
    /// <summary>
    /// Responsible for reading TTC font collections.
    /// </summary>
    /// <remarks>
    /// See http://www.microsoft.com/typography/otspec/otff.htm for more info about TTC format.
    /// </remarks>
    public class TtcReader
    {
        /// <summary>
        /// Ctor.
        /// </summary>
        public TtcReader(BigEndianBinaryReader reader)
        {
            mReader = reader;
        }

        /// <summary>
        /// Tries to read a TTC header. Returns true if header was read successfully.
        /// </summary>
        public bool TryReadHeader()
        {
            // According to TTC specification TTC header is always located at the beginning of the file.
            mReader.BaseStream.Position = 0;
            string ttcfTag = new string(mReader.ReadByteChars(4));
            if (ttcfTag != "ttcf")
                return false;

            int fontVersion = mReader.ReadInt32();
            if ((fontVersion != 0x00010000) && (fontVersion != 0x00020000))
                return false;

            mNumberOfFonts = mReader.ReadInt32();
            if (mNumberOfFonts <= 0)
                return false;

            OffsetTables = new uint[NumberOfFonts];

            for (int i = 0; i < NumberOfFonts; i++)
                OffsetTables[i] = mReader.ReadUInt32();

            // TTC header version 2.0 has 3 extra fields.
            if (0x00020000 == fontVersion)
            {
                mReader.ReadUInt32();    // ULONG ulDsigTag.
                mReader.ReadUInt32();    // ULONG ulDsigTag.
                mReader.ReadUInt32();    // ULONG ulDsigOffset.
            }
            return true;
        }

        /// <summary>
        /// Sets stream position to the beginning of the specified font.
        /// </summary>
        public void SeekToFont(int fontIndex)
        {
            uint offset = OffsetTables[fontIndex];
            mReader.BaseStream.Position = offset;
        }

        /// <summary>
        /// Number of fonts in collection.
        /// </summary>
        public int NumberOfFonts
        {
            get { return mNumberOfFonts; }
        }

        /// <summary>
        /// Array of offsets to the OffsetTable for each font from the beginning of the file.
        /// </summary>
        public uint[] OffsetTables
        {
            get { return mOffsetTables; }
            set { mOffsetTables = value; }
        }

        private readonly BigEndianBinaryReader mReader;
        private int mNumberOfFonts;
        private uint[] mOffsetTables;
    }
}
