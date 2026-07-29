// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 09/06/2011 by Alexey Titov

using System.Collections.Generic;
using Aspose.Crypto;
using Aspose.Images;
using Aspose.Words.Drawing.Core.Dml.Fills;
using Aspose.Words.Drawing.Core.Dml.NonVisualProperties;

namespace Aspose.Words.Drawing.Core.Dml
{
    /// <summary>
    /// Represents a picture.
    /// </summary>
    /// <remarks>
    /// 20.1.2.2.30 pic (Picture)
    /// This element specifies the existence of a picture object within the document.
    /// </remarks>
    internal class DmlPicture : DmlShapeBase
    {
        internal override DmlNode Clone(bool isCloneChildren, INodeCloningListener cloningListener)
        {
            DmlPicture lhs = (DmlPicture)base.Clone(isCloneChildren, cloningListener);
            if (mBlipFill != null)
                lhs.mBlipFill = (DmlBlipFill)mBlipFill.Clone();
            return lhs;
        }

        public override bool HasTransparency
        {
            get
            {
                return base.HasTransparency || PictureHasAlphaChannel();
            }
        }

        private bool PictureHasAlphaChannel()
        {
            // Check whether image has alpha channel.
            if (BlipFill.ImageBytes != null)
            {
                // Since picture might be changed upon document processing 
                // it is required to get transparency for image that is in blip now. 
                // It is expensive to get HasAlphaChannel every time the property is requested, 
                // so calculate the current image hash and cache HasAlphaChannel value for it in the collection.
                int pictureHash = HashUtil.GetSHA512Hash(BlipFill.ImageBytes).GetHashCode();

                Dictionary<int, bool> transparencyFlagCache = (Dml.Document != null)
                    ? Dml.Document.ImageTransparencyFlagCache
                    : null;

                bool hasAlphaChannel;
                if ((transparencyFlagCache != null) && transparencyFlagCache.TryGetValue(pictureHash, out hasAlphaChannel))
                    return hasAlphaChannel;
                try
                {
                    hasAlphaChannel = ImageUtil.HasAlphaChannel(BlipFill.ImageBytes);
                }
                catch
                {
                    // There might be invalid image data, consider such images as not transparent.
                    hasAlphaChannel = false;
                }

                if (transparencyFlagCache != null)
                    transparencyFlagCache.Add(pictureHash, hasAlphaChannel);

                return hasAlphaChannel;
            }

            return false;
        }

        internal override DmlNodeType DmlNodeType
        {
            get { return DmlNodeType.Picture; }
        }

        internal DmlBlipFill BlipFill
        {
            get { return mBlipFill; }
            set { mBlipFill = value; }
        }

        internal override void SetHRef(string href)
        {
            base.SetHRef(href);

            if (!StringUtil.HasChars(href) &&
                ((NonVisualPr == null) ||
                (NonVisualPr.NvDrawingProperties == null) ||
                (NonVisualPr.NvDrawingProperties.HlinkClick == null)))
            {
                return;
            }

            NonVisualPr = (NonVisualPr == null) ? new DmlNvPrPicture() : NonVisualPr;
            DmlNvDrawingProperties nvDrawingPr = NonVisualPr.NvDrawingProperties;

            nvDrawingPr = (nvDrawingPr == null) ? new DmlNvDrawingProperties(mShape.Id, "") : nvDrawingPr;
            nvDrawingPr.HlinkClick = (nvDrawingPr.HlinkClick == null) ? new DmlHlink() : nvDrawingPr.HlinkClick;

            // WORDSNET-21498 Update non-visual drawing properties while changing picture hyperlink.
            nvDrawingPr.HlinkClick.Id = href;
            NonVisualPr.NvDrawingProperties = nvDrawingPr;
        }

        private DmlBlipFill mBlipFill;
    }
}
