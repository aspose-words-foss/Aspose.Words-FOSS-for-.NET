// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 07/05/2011 by Alexey Titov

using System.Drawing;
using System.Drawing.Drawing2D;
using Aspose.Drawing;
using Aspose.Images;
using Aspose.Words.RW.Dml.Writer;
using Aspose.Words.RW.Nrx.Writer;
using Aspose.Words.Saving;

namespace Aspose.Words.Drawing.Core.Dml.Fills
{
    /// <summary>
    /// 20.1.8.56 stretch (Stretch)
    /// This element specifies that a BLIP should be stretched to fill
    /// the target rectangle. The other option is a tile where a BLIP
    /// is tiled to fill the available area.
    /// </summary>
    internal class DmlBlipFillStretch : IDmlBlipFillMode
    {
        public IDmlBlipFillMode Clone()
        {
            DmlBlipFillStretch blipFillStretch = new DmlBlipFillStretch();
            if (mFillRectangle != null)
                blipFillStretch.mFillRectangle = mFillRectangle.Clone();
            return blipFillStretch;
        }

        public override bool Equals(object obj)
        {
            // Same instance.
            if (obj == this)
                return true;

            // Type or hashcode does not match.
            if (!ArgumentUtil.TypeAndHashCodeMatches(this, obj))
                return false;

            DmlBlipFillStretch value = (DmlBlipFillStretch)obj;

            return object.Equals(value.FillRectangle, FillRectangle);
        }

        public override int GetHashCode()
        {
            return FillRectangle.GetHashCode();
        }

        public void Write(IDmlShapeWriterContext writer)
        {
            NrxXmlBuilder builder = writer.Builder;
            bool isIsoStrict = (writer.Compliance == OoxmlComplianceCore.IsoStrict);
            builder.StartElement("a:stretch");
            DmlWriterUtil.WritePercentageOffsetRectangle(FillRectangle, "a", "fillRect", true, builder, isIsoStrict);
            builder.EndElement("a:stretch");
        }

        public FillTypeCore FillType
        {
            get { return FillTypeCore.Picture; }
        }

        internal DmlPercentageOffsetRectangle FillRectangle
        {
            [CodePorting.Translator.Cs2Cpp.CppConstMethod()]
            get
            {
                if (mFillRectangle == null)
                    mFillRectangle = new DmlPercentageOffsetRectangle();

                return mFillRectangle;
            }
            set { mFillRectangle = value; }
        }

        [CodePorting.Translator.Cs2Cpp.CppForceSharedApi]
        bool IDmlBlipFillMode.HasOffsets
        {
            get { return !mFillRectangle.IsEmpty; }
        }

        [CodePorting.Translator.Cs2Cpp.CppMutable]
        private DmlPercentageOffsetRectangle mFillRectangle;
    }
}
