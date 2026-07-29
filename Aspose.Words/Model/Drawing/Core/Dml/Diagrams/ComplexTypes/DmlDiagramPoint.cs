// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 01/05/2013 by Konstantin Kornilov

using System.Collections.Generic;
using Aspose.Words.Drawing.Core.Dml.Diagrams.SimpleTypes;
using Aspose.Words.Drawing.Core.Dml.Text;

namespace Aspose.Words.Drawing.Core.Dml.Diagrams.ComplexTypes
{
    /// <summary>
    /// 21.4.3.5 pt (Point)
    /// </summary>
    internal class DmlDiagramPoint : DmlExtensionListSource
    {
        internal DmlModelId ModelId
        {
            get { return mModelId; }
            set { mModelId = value; }
        }

        internal DmlPointType Type
        {
            get { return mType; }
            set { mType = value; }
        }

        internal DmlModelId ConnectionId
        {
            get { return mConnectionId; }
            set { mConnectionId = value; }
        }

        internal DmlTextBody TextBody
        {
            get { return mTextBody; }
            set { mTextBody = value; }
        }

        internal DmlShapeProperties ShapeProperties
        {
            get { return mShapeProperties; }
            set { mShapeProperties = value; }
        }

        internal DmlPropertySet PropertySet
        {
            get { return mPropertySet; }
            set { mPropertySet = value; }
        }

        internal DmlDiagramPoint AlternateContentFallbackPoint
        {
            get { return mFallbackPoint; }
            set { mFallbackPoint = value; }
        }

        internal IDictionary<string, string> AlternateContentChoiceAttributes
        {
            get
            {
                if(mChoiceAttributes == null)
                    mChoiceAttributes = new Dictionary<string, string>();
                return mChoiceAttributes;
            }
        }

        internal IDictionary<string, string> AlternateContentAttributes
        {
            get
            {
                if (mAlternateContentAttributes == null)
                    mAlternateContentAttributes = new Dictionary<string, string>();
                return mAlternateContentAttributes;
            }
        }

        private DmlModelId mModelId;
        private DmlPointType mType = DmlPointType.Node;
        private DmlModelId mConnectionId;
        private DmlTextBody mTextBody;
        private DmlShapeProperties mShapeProperties;
        private DmlPropertySet mPropertySet;
        private DmlDiagramPoint mFallbackPoint;
        private Dictionary<string, string> mChoiceAttributes;
        private Dictionary<string, string> mAlternateContentAttributes;
    }
}
