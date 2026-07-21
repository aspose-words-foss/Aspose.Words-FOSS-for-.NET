// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 06/12/2012 by Denis Darkin

using System.Collections.Generic;
using Aspose.JavaAttributes;
using Aspose.Words.Drawing;
using Aspose.Words.Math;

namespace Aspose.Words.Validation
{
    /// <summary>
    /// Converts OfficeMath objects to Shape objects for storing into formats that don't support OMML.
    /// </summary>
    internal class OfficeMathToShapeConverter
    {
        /// <summary>
        /// Ctor.
        /// </summary>
        internal OfficeMathToShapeConverter(Document document, SaveFormat saveFormat)
        {
            Debug.Assert(document != null);
            mDocument = document;
            mSaveFormat = saveFormat;

            mConvertedOfficeMathToShape = new Dictionary<OfficeMath, Shape>();
            mShapeToConvertedOfficeMath = new Dictionary<ShapeBase, OfficeMath>();
        }

        internal virtual VisitorAction VisitOfficeMathStart(OfficeMath officeMath, DocumentValidator validator)
        {
            // FOSS
            return VisitorAction.Continue;
        }

        [JavaThrows(true)]
        internal virtual VisitorAction VisitOfficeMathEnd(OfficeMath officeMath, DocumentValidator validator)
        {
            return VisitorAction.Continue;
        }

        /// <summary>
        /// Revert conversion of OfficeMath to Shape to keep model in original state.
        /// </summary>
        internal void Revert()
        {
            // Office Math is converted to shape with special attribute when document is saved
            // to DOC or WML or RTF formats. Reverts these changes back to get original Model.
            foreach (KeyValuePair<OfficeMath, Shape> entry in mConvertedOfficeMathToShape)
            {
                Shape mathShape = entry.Value;
                OfficeMath officeMath = entry.Key;
                InsertShapeAtCompatibleTreeLevel(officeMath, mathShape);
                mathShape.Remove();
            }
        }

        protected virtual void InsertShapeAtCompatibleTreeLevel(OfficeMath officeMath, Shape mathShape)
        {
            officeMath.SetDocument(mDocument);
            NodeUtil.InsertShapeAtCompatibleTreeLevel(officeMath, mathShape);
        }

        [JavaThrows(true)]
        protected virtual void ApplySpecialPreserveAttribute(OfficeMath officeMath, Shape mathShape)
        {
            // Do nothing.
        }

        /// <summary>
        /// Logs a warning to the user-provided warning callback.
        /// </summary>
        private void Warn(WarningType type, WarningSource source, string description)
        {
            IWarningCallback warningCallback = mDocument.WarningCallback;
            if (warningCallback != null)
                warningCallback.Warning(new WarningInfo(type, source, description));
        }

        internal IDictionary<OfficeMath, Shape> ConvertedOfficeMathToShape
        {
            get { return mConvertedOfficeMathToShape; }
        }

        internal IDictionary<ShapeBase, OfficeMath> ShapeToConvertedOfficeMath
        {
            get { return mShapeToConvertedOfficeMath; }
        }

        private readonly Document mDocument;
        private readonly SaveFormat mSaveFormat;

        /// <summary>
        /// Maps converted OfficeMath objects to original ShapeBase objects
        /// when document is saved to format which doesn't support OMML.
        /// Keys are instances of <see cref="OfficeMath"/>.
        /// Values are instances of <see cref="Shape"/>.
        /// </summary>
        private readonly Dictionary<OfficeMath, Shape> mConvertedOfficeMathToShape;

        /// <summary>
        /// Maps original ShapeBase objects to converted OfficeMath objects
        /// when document is saved to format which doesn't support OMML.
        /// Keys are instances of <see cref="ShapeBase"/>.
        /// Values are instances of <see cref="OfficeMath"/>.
        /// </summary>
        private readonly Dictionary<ShapeBase, OfficeMath> mShapeToConvertedOfficeMath;
    }
}
