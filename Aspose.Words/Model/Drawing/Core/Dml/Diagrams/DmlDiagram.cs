// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 01/05/2013 by Konstantin Kornilov

using Aspose.Words.Drawing.Core.Dml.Fills;
using Aspose.Words.Drawing.Core.Dml.Outlines;
using Aspose.Words.Drawing.Core.Dml.Styles;

namespace Aspose.Words.Drawing.Core.Dml.Diagrams
{
    /// <summary>
    /// Represents DML diagram.
    /// </summary>
    internal class DmlDiagram : DmlNode, IDmlCommonShapePrSource
    {
        internal override DmlNodeType DmlNodeType
        {
            get { return DmlNodeType.Diagram; }
        }

        internal DmlDiagramLayout Layout
        {
            get { return mLayout; }
            set { mLayout = value; }
        }

        internal DmlDiagramDataModel Data
        {
            get { return mData; }
            set { mData = value; }
        }

        internal DmlDiagramStyleDefinition StyleDefinition
        {
            get { return mStyleDefinition; }
            set { mStyleDefinition = value; }
        }

        internal DmlDiagramColorTransform ColorTransformDefinition
        {
            get { return mColorTransformDefinition; }
            set { mColorTransformDefinition = value; }
        }

        #region IDmlCommonShapePrSource implementation

        public DmlFill Fill
        {
            get { return mData.Background.Fill; }
            set { mData.Background.Fill = value; }
        }

        public DmlOutline Outline
        {
            get { return mData.WholeFormatting.Outline; }
            set { mData.WholeFormatting.Outline = value; }
        }

        public DmlShapeStyle Style
        {
            get
            {
                // According to spec ShapeStyle is not supported by DmlDiagram, null is always returned.
                return null;
            }
            set
            {
                // Do nothing.
            }
        }

        #endregion

        public override string AlternativeText
        {
            get
            {
                // WORDSNET-14547 DML diagram object has not non visual shape properties.
                // Non-visual properties moves to graphic frame after grouping shapes.
                return Dml.IsTopLevel ? base.AlternativeText : string.Empty;
            }
            set
            {
                if (Dml.IsTopLevel)
                    base.AlternativeText = value;
            }
        }

        public override string Title
        {
            get
            {
                return Dml.IsTopLevel ? base.Title : string.Empty;
            }
            set
            {
                if (Dml.IsTopLevel)
                    base.Title = value;
            }
        }

        public override string Name
        {
            get
            {
                return Dml.IsTopLevel ? base.Name : string.Empty;
            }
            set
            {
                if (Dml.IsTopLevel)
                    base.Name = value;
            }
        }

        internal override bool Hidden
        {
            get { return Dml.IsTopLevel && base.Hidden; }
            set
            {
                if (Dml.IsTopLevel)
                    base.Hidden = value;
            }
        }

        internal override bool AspectRatioLocked
        {
            get
            {
                return Dml.IsTopLevel && base.AspectRatioLocked;
            }
            set
            {
                if (Dml.IsTopLevel)
                    base.AspectRatioLocked = value;
            }
        }

        /// <summary>
        /// Flag for enabling SmartArt cold rendering logging.
        /// </summary>
        internal bool EnableColdRenderingLogging
        {
            get { return mEnableColdRenderingLogging; }
            set { mEnableColdRenderingLogging = value; }
        }

        private DmlDiagramLayout mLayout;
        private DmlDiagramDataModel mData;
        private DmlDiagramColorTransform mColorTransformDefinition;
        private DmlDiagramStyleDefinition mStyleDefinition;
        private bool mEnableColdRenderingLogging;
    }
}
