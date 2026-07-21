// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 10/07/2013 by Ivan Lyagin

namespace Aspose.Words.Fields
{
    /// <summary>
    /// Passes data between methods while writing INDEX field result.
    /// </summary>
    internal class FieldIndexWriteContext
    {
        /// <summary>
        /// Ctor.
        /// </summary>
        internal FieldIndexWriteContext(FieldCodeIndex fieldCode, DocumentBuilder documentBuilder)
        {
            FieldCode = fieldCode;
            DocumentBuilder = documentBuilder;
        }

        /// <summary>
        /// Performs common tasks while starting of page numbers' writing.
        /// </summary>
        internal void StartWritePageNumbers()
        {
            CurrentSinglePageNumberInfo = null;
            UsePageNumberListSeparator = false;
        }

        /// <summary>
        /// Performs common tasks while ending of single page number's writing.
        /// </summary>
        internal void EndWritePageNumber()
        {
            CurrentSinglePageNumberInfo = null;
            UsePageNumberListSeparator = true;
        }

        /// <summary>
        /// Gets a shared <see cref="FieldCodeIndex"/> instance.
        /// </summary>
        internal FieldCodeIndex FieldCode { get; }

        /// <summary>
        /// Gets a shared <see cref="DocumentBuilder"/> instance.
        /// </summary>
        internal DocumentBuilder DocumentBuilder { get; }

        /// <summary>
        /// Gets or sets an <see cref="IndexEntryPageNumberInfo"/> object starting a merged sequence
        /// of single page numbers.
        /// </summary>
        internal IndexEntryPageNumberInfo CurrentSinglePageNumberInfo { get; set; }

        /// <summary>
        /// Gets a value indicating whether <see cref="CurrentSinglePageNumberInfo"/> is not null.
        /// </summary>
        internal bool HasCurrentSinglePageNumberInfo
        {
            get { return CurrentSinglePageNumberInfo != null; }
        }

        /// <summary>
        /// Gets a value indicating whether page number list separator should be used istead of page number
        /// separator while writing of the next page number.
        /// </summary>
        internal bool UsePageNumberListSeparator { get; private set; }
    }
}
