// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 15/10/2024 by Konstantin Kornilov

using System.IO;
using Aspose.Fonts.Sfnt;
using Aspose.Fonts.TrueType;

namespace Aspose.Fonts.Ttc
{
    internal class TtcOpenTypeReader : OpenTypeReader
    {
        public TtcOpenTypeReader(Stream stream, TtcPhysicalFontData data)
            : base(stream)
        {
            mData = data;
        }

        public override bool TryReadHeader()
        {
            TtcReader ttcReader = new TtcReader(BinaryReader);
            if (!ttcReader.TryReadHeader())
                return false;
            mFaceIndex =
                mData.TtcIndexPresent
                    ? mData.TtcIndex
                    : FindTtcFontIndex(ttcReader, mData.FullFontName);
            if (mFaceIndex < 0 || mFaceIndex >= ttcReader.NumberOfFonts)
                return false;

            ttcReader.SeekToFont(mFaceIndex);
            return base.TryReadHeader();
        }

        private int FindTtcFontIndex(TtcReader reader, string fullFontName)
        {
            for (int i = 0; i < reader.NumberOfFonts; i++)
            {
                reader.SeekToFont(i);
                if (CheckFontName(fullFontName))
                    return i;
            }

            return -1;
        }

        private bool CheckFontName(string fontName)
        {
            SfntReader reader = new SfntReader(BaseStream);
            if (!reader.TryReadHeader())
                return false;
            if (!reader.TableRecordEntries.ContainsKey(OpenTypeTableTag.Name))
                return false;

            reader.SeekToTable(OpenTypeTableTag.Name);
            TTFontNames names = TTFontNames.Read(BinaryReader);

            foreach (string name in names.FullFontNamesAllLanguages)
                if (StringUtil.EqualsOrdinalIgnoreCase(fontName, name))
                    return true;

            return false;
        }

        public override PhysicalFontData GetUpdatedFontData()
        {
            return new TtcPhysicalFontData(mData.FileData, mFaceIndex, mData.FullFontName);
        }

        protected override string InvalidDataMessage { get { return "The TTC font is invalid."; } }

        private readonly TtcPhysicalFontData mData;
        private int mFaceIndex;
    }
}
