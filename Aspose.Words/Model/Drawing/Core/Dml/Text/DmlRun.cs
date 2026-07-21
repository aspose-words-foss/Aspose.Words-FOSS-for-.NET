// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 10/07/2011 by Alexey Titov

using System;

namespace Aspose.Words.Drawing.Core.Dml.Text
{
    /// <summary>
    /// 21.1.2.3.8 r (Text Run)
    /// This element specifies the presence of a run of text within the containing text body. 
    /// The run element is the lowest level text separation mechanism within a text body. 
    /// A text run can contain text run properties associated with the run. If no properties 
    /// are listed then properties specified in the defRPr element are used.
    /// </summary>
    internal class DmlRun : DmlParagraphTextElementBase
    {
        /// <summary>
        /// Initializes a new instance of this class.
        /// </summary>
        internal DmlRun()
        {
        }

        /// <summary>
        /// Initializes a new instance of this class with defining run properties.
        /// </summary>
        internal DmlRun(DmlRunProperties runProperties)
            : base(runProperties)
        {
        }

        internal string Text
        {
            get
            {
                if (mText == null)
                    mText = String.Empty;
                return mText;
            }
            set { mText = value; }
        }

        private string mText;
    }
}