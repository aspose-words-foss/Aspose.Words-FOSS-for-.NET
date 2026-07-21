// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 23/01/2013 by Ivan Lyagin

using System;
using System.Collections.Generic;
using Aspose.Words.Lists;

namespace Aspose.Words.Fields
{
    /// <summary>
    /// An utility class that provides methods to work with LISTNUM, AUTONUM, AUTONUMOUT and AUTONUMLGL fields.
    /// </summary>
    internal static class FieldNumUtil
    {
        /// <summary>
        /// Returns a value indicating whether the specified field type relates to LISTNUM, AUTONUM, AUTONUMOUT
        /// or AUTONUMLGL field.
        /// </summary>
        internal static bool IsFieldNum(FieldType fieldType)
        {
            switch (fieldType)
            {
                case FieldType.FieldListNum:
                case FieldType.FieldAutoNum:
                case FieldType.FieldAutoNumOutline:
                case FieldType.FieldAutoNumLegal:
                    return true;
                default:
                    return false;
            }
        }

        /// <summary>
        /// Gets a separator character by the specified separator string.
        /// </summary>
        internal static char GetSeparatorCharacterCore(string separatorCharacter)
        {
            return StringUtil.HasChars(separatorCharacter) ? separatorCharacter[0] : DefaultSeparator;
        }

        /// <summary>
        /// Gets a list of fake result runs for LISTNUM, AUTONUM, AUTONUMOUT or AUTONUMLGL field basing on
        /// the corresponding field start and a containing field.
        /// </summary>
        internal static IList<Run> GetFakeResultList(Field refField)
        {
            return GetFakeResultList(refField.Start, refField);
        }

        /// <summary>
        /// Gets a list of fake result runs for LISTNUM, AUTONUM, AUTONUMOUT or AUTONUMLGL field basing on
        /// the corresponding field start and a containing field.
        /// </summary>
        public static IList<Run> GetFakeResultList(FieldStart fieldNumStart, Field refField)
        {
            FakeResultInfoProvider infoProvider = GetFakeResultInfoProvider(fieldNumStart, refField);
            if (infoProvider == null)
                return new List<Run>();

            return FieldTextHelper.GetSingleLineTextRunListBidiAware(
                infoProvider.ListLabel.Text,
                infoProvider.Document,
                infoProvider,
                infoProvider.IsRtlEmbedding());
        }

        /// <summary>
        /// Gets a range of fake result runs for LISTNUM, AUTONUM, AUTONUMOUT or AUTONUMLGL field basing on
        /// the corresponding field start and a containing field.
        /// </summary>
        internal static NodeRange GetFakeResultNodeRange(Field refField)
        {
            FakeResultInfoProvider infoProvider = GetFakeResultInfoProvider(refField.Start, refField);
            if (infoProvider == null)
                return null;

            return FieldTextHelper.GetSingleLineTextRunRangeBidiAware(
                infoProvider.ListLabel.Text,
                infoProvider.Document,
                infoProvider,
                infoProvider.IsRtlEmbedding());
        }

        /// <summary>
        /// Gets an <see cref="ListNumberState"/> object associated with LISTNUM, AUTONUM, AUTONUMOUT or AUTONUMLGL field.
        /// </summary>
        internal static ListNumberState GetFieldListNumberState(Field refField)
        {
            FakeResultInfoProvider infoProvider = GetFakeResultInfoProvider(refField.Start, refField);
            if (infoProvider == null)
                return null;

            return infoProvider.ListLabel.State;
        }

        /// <summary>
        /// A factory method to create a <see cref="FakeResultInfoProvider"/> instance.
        /// </summary>
        private static FakeResultInfoProvider GetFakeResultInfoProvider(FieldStart fieldNumStart, Field refField)
        {
            Debug.Assert(IsFieldNum(refField.Type) == (fieldNumStart == refField.Start));

            Document document = refField.FetchDocument();
            Debug.Assert(document == fieldNumStart.FetchDocument());

            FieldNumListLabel listLabel = document.GetFieldNumListLabel(fieldNumStart);
            return ((listLabel != null) && listLabel.HasText)
                ? new FakeResultInfoProvider(listLabel, fieldNumStart, refField, document)
                : null;
        }

        /// <summary>
        /// Provides information about LISTNUM, AUTONUM, AUTONUMOUT or AUTONUMLGL fake result. Implements
        /// <see cref="IFieldRunPrProvider"/> in the way to get MS-Word-like text look.
        /// </summary>
        private class FakeResultInfoProvider : IFieldRunPrProvider
        {
            /// <summary>
            /// Ctor.
            /// </summary>
            internal FakeResultInfoProvider(FieldNumListLabel listLabel, FieldStart fieldNumStart, Field refField, Document document)
            {
                ListLabel = listLabel;
                mFieldNumStart = fieldNumStart;
                mRefField = refField;
                Document = document;
            }

            /// <summary>
            /// Returns a value indicating whether a BIDI-text for the list label should be treated as if it was
            /// contained within RTL paragraph or not.
            /// </summary>
            internal bool IsRtlEmbedding()
            {
                if (!Document.FieldOptions.IsBidiTextSupportedOnUpdate)
                    return false;

                switch (mRefField.Type)
                {
                    case FieldType.FieldTOC:
                        // It seems like MS Word always uses LTR here. To be investigated.
                        return false;
                    case FieldType.FieldRef:
                    case FieldType.FieldRefNoKeyword:
                    case FieldType.FieldNone:
                        // See Field.GetBidiParagraphLevelOverride() for details.
                        return (bool)mFieldNumStart.RunPr.Bidi.ResolveFetchInheritedRunAttrWithNull(mFieldNumStart, FontAttr.Bidi);
                    default:
                        if (IsFieldNum(mRefField.Type))
                            return mRefField.Start.ParentParagraph.ParagraphFormat.Bidi;

                        throw new InvalidOperationException("Invalid reference field type for the operation.");
                }
            }

            RunPr IFieldRunPrProvider.GetRunPr()
            {
                RunPr runPr;
                if (IsFieldNum(mRefField.Type))
                {
                    // LISTNUM, AUTONUM, AUTONUMOUT and AUTONUMLGL fields themselves are FWR fields and hence their results
                    // are not a part of a document content so we need fully expand font properties for these results' runs.
                    runPr = mFieldNumStart.GetExpandedRunPr(RunPrExpandFlags.Layout);
                }
                else
                {
                    // Clone font properties of the containing field's start.
                    runPr = mRefField.Start.RunPr.Clone();

                    // Add LISTNUM, AUTONUM, AUTONUMOUT or AUTONUMLGL field start's font properties' overrides.
                    // This step should be skipped if the containing field is TOC.
                    if (mRefField.Type != FieldType.FieldTOC)
                        mFieldNumStart.RunPr.ExpandTo(runPr);
                }

                // Add list label font properties' overrides if any.
                if (ListLabel.HasRunPrOverrides)
                    ListLabel.RunPrOverrides.ExpandTo(runPr);

                return runPr;
            }

            /// <summary>
            /// Gets a <see cref="FieldNumListLabel"/> instance associated with this instance.
            /// </summary>
            internal FieldNumListLabel ListLabel { get; }

            /// <summary>
            /// Gets a document associated with this instance.
            /// </summary>
            internal Document Document { get; }

            private readonly FieldStart mFieldNumStart;
            private readonly Field mRefField;
        }

        /// <summary>
        /// The default separator character used to build AUTONUM fields' fake results.
        /// </summary>
        [CodePorting.Translator.Cs2Cpp.CppConstexpr]
        internal const char DefaultSeparator = '.';
    }
}
