// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 15/10/2024 by Konstantin Kornilov

using System;
using System.IO;
using Aspose.Fonts.Sfnt;
using Aspose.Fonts.Ttc;
using Aspose.IO;

namespace Aspose.Fonts.TrueType
{
    internal class OpenTypeReader
    {
        public OpenTypeReader(Stream stream)
        {
            SfntReader = new SfntReader(stream);
        }

        public static OpenTypeReader Create(Stream stream, PhysicalFontData data)
        {
            if (data.IsTtc)
                return new TtcOpenTypeReader(stream, (TtcPhysicalFontData)data);
            return new OpenTypeReader(stream);
        }

        public static OpenTypeReader Create(Stream stream, IFontData fontData, string fontName)
        {
            TtcOpenTypeReader ttcReader = new TtcOpenTypeReader(stream, new TtcPhysicalFontData(fontData, -1, fontName));
            bool isTtc = ttcReader.TryReadHeader();
            stream.Position = 0;

            return isTtc
                ? ttcReader
                : new OpenTypeReader(stream);
        }

        public void ReadHeader()
        {
            if (!TryReadHeader())
                throw new InvalidOperationException(InvalidDataMessage);
        }

        public virtual bool TryReadHeader()
        {
            if (mIsInitialized)
                return true;

            mIsInitialized = true;
            if (!SfntReader.TryReadHeader())
                return false;

            // Check TrueType tables.
            if (ContainsTable(OpenTypeTableTag.Glyf) && ContainsTable(OpenTypeTableTag.Loca))
                return true;

            // Check CFF tables.
            if (ContainsTable(OpenTypeTableTag.Cff))
                return true;

            // There also may be bitmap fonts which we do not support for now.
            return false;
        }

        public virtual PhysicalFontData GetUpdatedFontData()
        {
            return null;
        }

        public bool ContainsTable(string tag)
        {
            return SfntReader.TableRecordEntries.ContainsKey(tag);
        }

        public virtual byte[] GetTableData(string tag)
        {
            return ContainsTable(tag)
                ? SfntReader.ReadTable(tag)
                : null;
        }

        protected virtual FontMetrics ReadOs2()
        {
            SfntReader.SeekToTable(OpenTypeTableTag.Os2);
            return new FontMetrics(BinaryReader);
        }

        protected virtual VerticalHeader ReadVhea()
        {
            if (!ContainsTable(OpenTypeTableTag.Vhea))
                return null;
            SfntReader.SeekToTable(OpenTypeTableTag.Vhea);
            return new VerticalHeader(BinaryReader);
        }

        protected virtual HorizontalHeader ReadHhea()
        {
            SfntReader.SeekToTable(OpenTypeTableTag.Hhea);
            return new HorizontalHeader(BinaryReader);
        }

        protected virtual PostScript ReadPost()
        {
            SfntReader.SeekToTable(OpenTypeTableTag.Post);
            return new PostScript(BinaryReader, SfntReader.TableRecordEntries[OpenTypeTableTag.Post].Length);
        }

        protected virtual CvtTable ReadCvt()
        {
            if (!ContainsTable(OpenTypeTableTag.Cvt))
                return null;
            SfntReader.SeekToTable(OpenTypeTableTag.Cvt);
            return CvtTable.Read(BinaryReader, SfntReader.TableRecordEntries[OpenTypeTableTag.Cvt].Length);
        }

        protected virtual HorizontalMetrics ReadHmtx()
        {
            int numberOfHMetrics = Hhea.NumberOfHMetrics;
            int numGlyphs = Maxp.NumGlyphs;
            SfntReader.SeekToTable(OpenTypeTableTag.Hmtx);
            return HorizontalMetrics.Read(BinaryReader, numberOfHMetrics, numGlyphs);
        }

        protected virtual VerticalMetrics ReadVmtx()
        {
            if (!ContainsTable(OpenTypeTableTag.Vhea) || !ContainsTable(OpenTypeTableTag.Vmtx))
                return null;

            int numberOfVMetrics = Vhea.NumberOfVMetrics;
            int numGlyphs = Maxp.NumGlyphs;

            SfntReader.SeekToTable(OpenTypeTableTag.Vmtx);
            return new VerticalMetrics(BinaryReader, numberOfVMetrics, numGlyphs);
        }

        public FontHeader Head
        {
            get
            {
                if (mHead == null)
                {
                    SfntReader.SeekToTable(OpenTypeTableTag.Head);
                    mHead = FontHeader.Read(BinaryReader);
                }

                return mHead;
            }
        }

        public HorizontalHeader Hhea
        {
            get { return mHhea ?? (mHhea = ReadHhea()); }
        }

        public MaximumProfile Maxp
        {
            get
            {
                if (mMaxp == null)
                {
                    SfntReader.SeekToTable(OpenTypeTableTag.Maxp);
                    mMaxp = new MaximumProfile(BinaryReader);
                }

                return mMaxp;
            }
        }

        public TTFontNames Name
        {
            get
            {
                if (mName == null)
                {
                    SfntReader.SeekToTable(OpenTypeTableTag.Name);
                    mName = TTFontNames.Read(BinaryReader);
                }

                return mName;
            }
        }

        public FontMetrics Os2
        {
            get { return mOs2 ?? (mOs2 = ReadOs2()); }
        }

        public Cmap Cmap
        {
            get
            {
                if (mCmap == null)
                {
                    SfntReader.SeekToTable(OpenTypeTableTag.Cmap);
                    mCmap = Cmap.Read(BinaryReader);
                }

                return mCmap;
            }
        }

        public PostScript Post
        {
            get { return mPost ?? (mPost = ReadPost()); }
        }

        public CvtTable Cvt
        {
            get { return mCvt ?? (mCvt = ReadCvt()); }
        }

        public HorizontalMetrics Hmtx
        {
            get { return mHmtx ?? (mHmtx = ReadHmtx()); }
        }

        public VerticalHeader Vhea
        {
            get { return mVhea ?? (mVhea = ReadVhea()); }
        }

        public VerticalMetrics Vmtx
        {
            get { return mVmtx ?? (mVmtx = ReadVmtx()); }
        }

        public ColrTable Colr
        {
            get
            {
                if (mColr == null && ContainsTable(OpenTypeTableTag.Colr))
                {
                    SfntReader.SeekToTable(OpenTypeTableTag.Colr);
                    mColr = new ColrTable(BinaryReader);
                }

                return mColr;
            }
        }

        public CpalTable Cpal
        {
            get
            {
                if (mCpal == null && ContainsTable(OpenTypeTableTag.Cpal))
                {
                    SfntReader.SeekToTable(OpenTypeTableTag.Cpal);
                    mCpal = new CpalTable(BinaryReader);
                }

                return mCpal;
            }
        }

        public SfntReader SfntReader { get; }
        public BigEndianBinaryReader BinaryReader
        {
            get { return SfntReader.BinaryReader; }
        }
        protected Stream BaseStream
        {
            get { return SfntReader.BaseStream; }
        }
        protected virtual string InvalidDataMessage { get { return "The file is not recognized as a valid font."; } }

        private bool mIsInitialized;
        private FontHeader mHead;
        private HorizontalHeader mHhea;
        private VerticalHeader mVhea;
        private MaximumProfile mMaxp;
        private TTFontNames mName;
        private FontMetrics mOs2;
        private HorizontalMetrics mHmtx;
        private VerticalMetrics mVmtx;
        private Cmap mCmap;
        private PostScript mPost;
        private CvtTable mCvt;
        private ColrTable mColr;
        private CpalTable mCpal;
    }
}
