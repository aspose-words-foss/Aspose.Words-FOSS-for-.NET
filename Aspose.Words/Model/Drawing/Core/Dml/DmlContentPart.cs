// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 08/26/2014 by Alexey Noskov

using System.Collections.Generic;
using Aspose.OpcPackaging;
using Aspose.Words.Drawing.Core.Dml.Fills;
using Aspose.Words.Drawing.Core.Dml.Outlines;
using Aspose.Words.Drawing.Core.Dml.Styles;

namespace Aspose.Words.Drawing.Core.Dml
{
    internal class DmlContentPart : DmlNode, IDmlCommonShapePrSource
    {
        internal override DmlNode Clone(bool isCloneChildren, INodeCloningListener cloningListener)
        {
            DmlContentPart lhs = (DmlContentPart) base.Clone(isCloneChildren, cloningListener);
            lhs.ContentPart = ContentPart.Clone();
            return lhs;
        }

        #region IDmlCommonShapePrSource implementation

        // At the moment we preserve content parts unparsed, please see ContentPart property.
        // That is why currently return new empty DmlStyleFill.
        public DmlFill Fill
        {
            get
            {
                return new DmlStyleFill();
            }
            set
            {
                // Do nothing.
            }
        }

        // At the moment we preserve content parts unparsed, please see ContentPart property.
        // That is why currently return new empty DmlOutline.
        public DmlOutline Outline
        {
            get
            {
                return new DmlOutline();
            }
            set
            {
                // Do nothing.
            }
        }

        // At the moment we preserve content parts unparsed, please see ContentPart property.
        // That is why currently return null.
        public DmlShapeStyle Style
        {
            get
            {
                return null;
            }
            set
            {
                // Do nothing.
            }
        }

        #endregion

        internal BWMode BWMode
        {
            get { return mBwMode; }
            set { mBwMode = value; }
        }

        /// <summary>
        /// The content parts's main XML part. At the moment we preserve it unparsed.
        /// </summary>
        internal OpcPackagePart ContentPart
        {
            get { return mContentPart; }
            set { mContentPart = value; }
        }

        internal override DmlNodeType DmlNodeType
        {
            get { return DmlNodeType.ContentPart; }
        }

        internal Dictionary<string, OpcPackagePart> RelatedParts
        {
            get { return mRelatedParts; }
        }

        private BWMode mBwMode;
        private OpcPackagePart mContentPart;
        private readonly Dictionary<string, OpcPackagePart> mRelatedParts = new Dictionary<string, OpcPackagePart>();
    }
}
