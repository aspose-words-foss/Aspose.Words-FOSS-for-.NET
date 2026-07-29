// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 26/04/2013 by Ivan Lyagin

using Aspose.Words.Fields.Expressions;

namespace Aspose.Words.Fields
{
    /// <summary>
    /// Holds an information used to build page numbers while an INDEX field result building.
    /// </summary>
    internal class IndexEntryPageNumberInfo
    {
        internal IndexEntryPageNumberInfo(
            int firstPageNumber,
            int lastPageNumber,
            FieldSeq firstFieldSeq,
            FieldSeq lastFieldSeq,
            FieldCodeXE refFieldCode,
            FieldXE refField,
            FieldSeqDataProvider fieldSeqDataProvider)
        {
            mFieldSeqDataProvider = fieldSeqDataProvider;
            FirstPageNumber = firstPageNumber;
            LastPageNumber = lastPageNumber;
            mFirstFieldSeq = firstFieldSeq;
            RefFieldCode = refFieldCode;
            RefField = refField;
            mLastFieldSeq = lastFieldSeq;
        }

        /// <summary>
        /// Gets the number of the first page in range if any or the number of a single page otherwise.
        /// </summary>
        internal int FirstPageNumber { get; }

        /// <summary>
        /// Gets the number of the last page in range if any.
        /// </summary>
        internal int LastPageNumber { get; }

        internal int FirstSequenceNumber
        {
            get
            {
                return GetSequenceNumber(mFirstFieldSeq);
            }
        }

        internal int LastSequenceNumber
        {
            get
            {
                return GetSequenceNumber(mLastFieldSeq);
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance represents a valid page range.
        /// </summary>
        internal bool HasPageRange
        {
            get { return LastPageNumber != InvalidPageNumber; }
        }

        /// <summary>
        /// Gets a value indicating whether this instance represents a valid page range where the first
        /// and the last page numbers are the same.
        /// </summary>
        internal bool HasSinglePageRange
        {
            get { return FirstPageNumber == LastPageNumber; }
        }

        /// <summary>
        /// Gets the corresponding <see cref="FieldCodeXE"/> instance.
        /// </summary>
        internal FieldCodeXE RefFieldCode { get; }

        /// <summary>
        /// Gets the corresponding <see cref="FieldXE"/> instance.
        /// </summary>
        internal FieldXE RefField { get; }

        private int GetSequenceNumber(FieldSeq field)
        {
            if (field == null)
                return 0;

            Constant constant = mFieldSeqDataProvider.GetValue(field);
            if (constant == null)
                return 0;

            return (int)constant.ValueDouble;
        }

        private readonly FieldSeq mFirstFieldSeq;
        private readonly FieldSeq mLastFieldSeq;
        private readonly IFieldUpdateDataProvider mFieldSeqDataProvider;

        [CodePorting.Translator.Cs2Cpp.CppConstexpr]
        internal const int InvalidPageNumber = -1;
    }
}
