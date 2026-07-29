// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 10/06/2011 by Alexey Titov

using Aspose.Words.Drawing.Core.Dml;
using Aspose.Words.Drawing.Core.Dml.Fills;

namespace Aspose.Words.Drawing.Core
{
    /// <summary>
    /// Represents a data source of DrawingML pictures.
    /// </summary>
    internal class DrawingMLImageDataSource : IImageDataSource
    {
        public DrawingMLImageDataSource(ShapeBase drawingML)
        {
            mDrawingML = drawingML;
        }

        public string SourceFullName
        {
            get
            {
                DmlBlipFill blip = GetBlipFill();
                if (blip == null)
                    return null;

                return blip.Blip.ImageLink;
            }
            set
            {
                DmlBlipFill blip = GetBlipFill();
                if (blip == null)
                    return;

                blip.Blip.ImageLink = value;
            }
        }

        public byte[] ImageBytes
        {
            get
            {
                DmlBlipFill blip = GetBlipFill();
                if (blip == null)
                    return null;

                return blip.Blip.EmbedImage;
            }
            set
            {
                // Create blip fill if possible.
                DmlBlipFill blip = FetchBlip();
                if (blip != null)
                    blip.Blip.EmbedImage = value;

                // Set fallback image too.
                if (mDrawingML.FallbackShape != null && mDrawingML.FallbackShape.CanHaveImage)
                    mDrawingML.FallbackShape.SetShapeAttrInternal(ShapeAttr.ImageBytes, ImageDataCore.GetImageBytes(value));
            }
        }

        bool IImageDataSource.HasImageBytes
        {
            get
            {
                DmlBlipFill blip = GetBlipFill();
                if ((blip == null) || (blip.Blip == null))
                    return false;

                return blip.Blip.HasEmbedImage;
            }
        }

        private DmlBlipFill GetBlipFill()
        {
            DmlPicture pict = mDrawingML.DmlNode as DmlPicture;
            if (pict != null)
                return pict.BlipFill;

            DmlFillableNode fillableNode = mDrawingML.DmlNode as DmlFillableNode;
            if (fillableNode != null)
                return fillableNode.Fill as DmlBlipFill;

            return null;
        }

        /// <summary>
        /// Returns either existing blip fill, or create new.
        /// </summary>
        private DmlBlipFill FetchBlip()
        {
            DmlBlipFill blip = GetBlipFill();
            if (blip != null)
                return blip;

            DmlPicture pict = mDrawingML.DmlNode as DmlPicture;
            if (pict != null)
                pict.BlipFill = new DmlBlipFill();

            DmlFillableNode fillableNode = mDrawingML.DmlNode as DmlFillableNode;
            if (fillableNode != null)
                fillableNode.Fill = new DmlBlipFill();

            return GetBlipFill();
        }

        [CodePorting.Translator.Cs2Cpp.CppWeakPtr]
        private readonly ShapeBase mDrawingML;
    }
}
