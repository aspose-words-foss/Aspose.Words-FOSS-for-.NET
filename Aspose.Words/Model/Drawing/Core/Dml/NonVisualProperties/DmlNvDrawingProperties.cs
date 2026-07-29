// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 18/07/2014 by Andrey Noskov

namespace Aspose.Words.Drawing.Core.Dml.NonVisualProperties
{
    /// <summary>
    /// 21.3.2.7 cNvPr (Non-Visual Drawing Properties)
    /// This element specifies non-visual canvas properties. This allows for additional information that does not affect
    /// the appearance of the picture to be stored.
    /// </summary>
    internal class DmlNvDrawingProperties : DmlExtensionListSource
    {
        internal DmlNvDrawingProperties Clone()
        {
            DmlNvDrawingProperties lhs = (DmlNvDrawingProperties)MemberwiseClone();
            lhs.Extensions = CloneExtensions();
            return lhs;
        }

        internal DmlNvDrawingProperties()
        {
        }

        internal DmlNvDrawingProperties(int id, string name)
        {
            mId = id;
            mName = name;
        }

        /// <summary>
        /// Specifies alternative text for the current DrawingML object.
        /// </summary>
        internal string Description
        {
            get { return mDescription; }
            set { mDescription = value; }
        }

        /// <summary>
        /// Specifies the name of the object.
        /// </summary>
        internal string Name
        {
            get { return mName; }
            set { mName = value; }
        }

        /// <summary>
        /// Specifies the title (caption) of the current DrawingML object.
        /// </summary>
        internal string Title
        {
            get { return mTitle; }
            set { mTitle = value; }
        }

        /// <summary>
        /// Specifies whether this DrawingML object is displayed.
        /// </summary>
        internal bool Hidden
        {
            get { return mHidden; }
            set { mHidden = value; }
        }

        /// <summary>
        /// Specifies a unique identifier for the current DrawingML object within the current
        /// document. This ID can be used to assist in uniquely identifying this object so that it can
        /// be referred to by other parts of the document.
        /// </summary>
        internal int Id
        {
            get { return mId; }
            set { mId = value; }
        }

        /// <summary>
        /// Specifies the on-click hyperlink information to be applied to a run of text. When the hyperlink text is clicked the
        /// link will be fetched.
        /// </summary>
        internal DmlHlink HlinkClick
        {
            get { return mHlinkClick; }
            set { mHlinkClick = value; }
        }

        /// <summary>
        /// Specifies the hyperlink information to be activated when the user's mouse is hovered over the corresponding object.
        /// </summary>
        internal DmlHlink HlinkHover
        {
            get { return mHlinkHover; }
            set { mHlinkHover = value; }
        }

        private int mId;
        private bool mHidden;
        private string mName;
        private string mTitle;
        private string mDescription;
        private DmlHlink mHlinkClick;
        private DmlHlink mHlinkHover;
    }
}
