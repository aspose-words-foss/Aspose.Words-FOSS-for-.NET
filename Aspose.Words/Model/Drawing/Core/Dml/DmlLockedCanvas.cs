// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 10/10/2010 by Alexey Titov

using System;
using Aspose.Words.Drawing.Core.Dml.Outlines;
using Aspose.Words.Drawing.Core.Dml.Styles;

namespace Aspose.Words.Drawing.Core.Dml
{
    internal class DmlLockedCanvas : DmlCompositeNode, IDmlCommonShapePrSource
    {
        internal DmlLockedCanvas(DmlNodeType canvasType)
        {
            if (canvasType != DmlNodeType.LockedCanvas && canvasType != DmlNodeType.WordprocessingCanvas)
                throw new ArgumentException("Unexpected canvas type");

            mCanvasType = canvasType;
        }

        internal override DmlNode Clone(bool isCloneChildren, INodeCloningListener cloningListener)
        {
            return (DmlNode)MemberwiseClone();
        }

        internal override DmlNodeType DmlNodeType
        {
            get { return mCanvasType; }
        }

        /// <summary>
        /// Outline specified via 'whole' formatting of word-processing canvas.
        /// </summary>
        public DmlOutline Outline
        {
            get { return mOutline; }
            set { mOutline = value; }
        }

        DmlShapeStyle IDmlCommonShapePrSource.Style
        {
            get { return mStyle; }
            set { mStyle = value; }
        }

        private readonly DmlNodeType mCanvasType;
        private DmlOutline mOutline;
        private DmlShapeStyle mStyle;
    }
}
