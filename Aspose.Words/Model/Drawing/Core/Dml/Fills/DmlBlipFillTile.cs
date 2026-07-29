// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 07/05/2011 by Alexey Titov

using System;
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
    /// 20.1.8.58 tile (Tile)
    /// This element specifies that a BLIP should be tiled to fill the available space.
    /// This element defines a "tile" rectangle within the bounding box. The image is
    /// encompassed within the tile rectangle, and the tile rectangle is tiled across
    /// the bounding box to fill the entire area.
    /// </summary>
    internal class DmlBlipFillTile : IDmlBlipFillMode
    {
        public IDmlBlipFillMode Clone()
        {
            DmlBlipFillTile tile = new DmlBlipFillTile();
            tile.Alignment = Alignment;
            tile.HorizontalOffset = HorizontalOffset;
            tile.HorizontalRatio = HorizontalRatio;
            tile.VerticalOffset = VerticalOffset;
            tile.VerticalRatio = VerticalRatio;
            tile.TileFlipMode = TileFlipMode;
            return tile;
        }

        public override bool Equals(object obj)
        {
            // Same instance.
            if (obj == this)
                return true;

            // Type or hashcode does not match.
            if (!ArgumentUtil.TypeAndHashCodeMatches(this, obj))
                return false;

            DmlBlipFillTile value = (DmlBlipFillTile)obj;

            return (value.Alignment == Alignment) &&
                   (value.TileFlipMode == TileFlipMode) &&
                   MathUtil.AreEqual(value.HorizontalOffset, HorizontalOffset) &&
                   MathUtil.AreEqual(value.HorizontalRatio, HorizontalRatio) &&
                   MathUtil.AreEqual(value.VerticalOffset, VerticalOffset) &&
                   MathUtil.AreEqual(value.VerticalRatio, VerticalRatio);

        }

        public override int GetHashCode()
        {
            int hash = 0;
            hash ^= Alignment.GetHashCode();
            hash ^= HorizontalOffset.GetHashCode();
            hash ^= HorizontalRatio.GetHashCode();
            hash ^= VerticalOffset.GetHashCode();
            hash ^= VerticalRatio.GetHashCode();
            hash ^= TileFlipMode.GetHashCode();
            return hash;
        }

        public void Write(IDmlShapeWriterContext writer)
        {
            NrxXmlBuilder builder = writer.Builder;
            bool isIsoStrict = (writer.Compliance == OoxmlComplianceCore.IsoStrict);

            builder.StartElement("a:tile");
            builder.WriteAttribute("tx", HorizontalOffset);
            builder.WriteAttribute("ty", VerticalOffset);
            builder.WriteAttribute("sx", DmlPercentageUtil.ToPercentOrDmlPercent(HorizontalRatio, isIsoStrict));
            builder.WriteAttribute("sy", DmlPercentageUtil.ToPercentOrDmlPercent(VerticalRatio, isIsoStrict));
            builder.WriteAttribute("flip", DmlEnum.TileFlipModeToDml(TileFlipMode));
            builder.WriteAttribute("algn", DmlEnum.RectangleAlignmentToDml(Alignment));
            builder.EndElement("a:tile");
        }

        public FillTypeCore FillType
        {
            get { return FillTypeCore.Texture; }
        }

        /// <summary>
        /// Specifies where to align the first tile with respect to the shape.
        /// Alignment happens after the scaling, but before the additional offset.
        /// </summary>
        internal DmlRectangleAlignment Alignment
        {
            get { return mAlignment; }
            set { mAlignment = value; }
        }

        /// <summary>
        /// Specifies the direction(s) in which to flip the source image while tiling.
        /// Images can be flipped horizontally, vertically, or in both directions
        /// to fill the entire region.
        /// </summary>
        internal DmlTileFlipMode TileFlipMode
        {
            get { return mTileFlipMode; }
            set { mTileFlipMode = value; }
        }

        /// <summary>
        /// Specifies the amount to horizontally scale the source rectangle.
        /// </summary>
        internal double HorizontalRatio
        {
            get { return mHorizontalRatio; }
            set { mHorizontalRatio = value; }
        }

        /// <summary>
        /// Specifies the amount to vertically  scale the source rectangle.
        /// </summary>
        internal double VerticalRatio
        {
            get { return mVerticalRatio; }
            set { mVerticalRatio = value; }
        }

        /// <summary>
        /// Specifies additional horizontal offset after alignment.
        /// </summary>
        internal double HorizontalOffset
        {
            get { return mHorizontalOffset; }
            set { mHorizontalOffset = value; }
        }

        /// <summary>
        /// Specifies additional vertical offset after alignment.
        /// </summary>
        internal double VerticalOffset
        {
            get { return mVerticalOffset; }
            set { mVerticalOffset = value; }
        }

        bool IDmlBlipFillMode.HasOffsets
        {
            get { return false; }
        }

        private DmlRectangleAlignment mAlignment;
        private double mHorizontalOffset;
        private double mHorizontalRatio = 1.0;
        private DmlTileFlipMode mTileFlipMode;
        private double mVerticalOffset;
        private double mVerticalRatio = 1.0;
    }
}
