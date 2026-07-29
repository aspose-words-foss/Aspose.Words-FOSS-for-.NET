// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 30/06/2016 by Anton Savko

using Aspose.Words.Drawing;
using Aspose.Words.Math;

namespace Aspose.Words.Validation
{
    /// <summary>
    /// Represents OfficeMathToShapeConverter, which doesn't make any conversions from OfficeMath to Shape.
    /// </summary>
    internal class NullOfficeMathToShapeConverter : OfficeMathToShapeConverter
    {
        internal NullOfficeMathToShapeConverter(Document document)
            : base(document, SaveFormat.Unknown)
        {
            // Do nothing.
        }

        protected override void InsertShapeAtCompatibleTreeLevel(OfficeMath officeMath, Shape mathShape)
        {
            // Do nothing.
        }

    }
}
