// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 28/10/2006 by Roman Korchagin

namespace Aspose.Words.Fields
{
    /// <summary>
    /// Parses and composes the INCLUDEPICTURE field code.
    /// </summary>
    internal class FieldCodeIncludePicture
    {
        /// <summary>
        /// Parses an INCLUDEPICTURE field code into parts.
        /// </summary>
        internal static FieldCodeIncludePicture Parse(FieldBundle fieldBundle)
        {
            Debug.Assert(fieldBundle.FieldType == FieldType.FieldIncludePicture || fieldBundle.FieldType == FieldType.FieldImport);

            IFieldIncludePictureCode field = (IFieldIncludePictureCode)fieldBundle.GetField();
            FieldCodeIncludePicture result = new FieldCodeIncludePicture();

            result.SourceFullName = field.SourceFullName;
            result.IsLinkOnly = field.IsLinked;

            return result;
        }

        /// <summary>
        /// Full path to the linked picture. Never null.
        /// </summary>
        internal string SourceFullName
        {
            get { return mSourceFullName; }
            private set { mSourceFullName = (value == null) ? string.Empty : value; }
        }

        /// <summary>
        /// True when the image is not stored in the document.
        /// </summary>
        internal bool IsLinkOnly { get; private set; }

        private string mSourceFullName = string.Empty;
    }
}
