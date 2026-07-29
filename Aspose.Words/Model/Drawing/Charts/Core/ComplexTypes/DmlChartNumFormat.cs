// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 20/06/2012 by Alexey Noskov

using System;

namespace Aspose.Words.Drawing.Charts.Core.ComplexTypes
{
    /// <summary>
    /// Represents 5.7.2.122 numFmt (Number Format) element.
    /// This element specifies number formatting for the parent element.
    /// </summary>
    internal class DmlChartNumFormat
    {
        internal DmlChartNumFormat Clone()
        {
            return (DmlChartNumFormat)MemberwiseClone();
        }

        public override bool Equals(object obj)
        {
            // Same instance.
            if (obj == this)
                return true;

            // Type or hashcode does not match.
            if (!ArgumentUtil.TypeAndHashCodeMatches(this, obj))
                return false;

            DmlChartNumFormat value = (DmlChartNumFormat)obj;

            return
                (ArgumentUtil.BothAreNull(FormatCode, value.FormatCode) ||
                    ((FormatCode != null) && FormatCode.Equals(value.FormatCode, StringComparison.Ordinal))) &&
                (ArgumentUtil.BothAreNull(SourceFormatCode, value.SourceFormatCode) ||
                    ((SourceFormatCode != null) && SourceFormatCode.Equals(value.SourceFormatCode, StringComparison.Ordinal))) &&
                (SourceLinked == value.SourceLinked);
        }

        public override int GetHashCode()
        {
            int hash = 0;
            if (FormatCode != null)
                hash ^= FormatCode.GetHashCode();
            if (SourceFormatCode != null)
                hash ^= SourceFormatCode.GetHashCode();
            hash ^= SourceLinked.GetHashCode();

            return hash;
        }

        /// <summary>
        /// Format code validated for rendering.
        /// </summary>
        internal string FormatCode
        {
            get { return mFormatCode; }
            set
            {
                mSourceFormatCode = value;
                mFormatCode = DmlChartFormatCodeValidator.ValidateFormatCode(value);
            }
        }

        internal bool SourceLinked
        {
            get { return mSourceLinked; }
            set { mSourceLinked = value; }
        }

        /// <summary>
        /// Returns format code value read from the document.
        /// Used to preserve format code upon writing chart back to DOCX document form the model.
        /// </summary>
        internal string SourceFormatCode
        {
            get { return mSourceFormatCode; }
        }

        private string mSourceFormatCode;
        private string mFormatCode;
        private bool mSourceLinked = true;
    }
}
