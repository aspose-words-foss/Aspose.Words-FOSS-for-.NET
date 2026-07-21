// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 04/06/2011 by Dmitry Vorobyev

using Aspose.Common;
using Aspose.Words.Fields.Expressions;

namespace Aspose.Words.Fields
{
    /// <summary>
    /// Implements the SEQ field.
    /// <para>To learn more, visit the <a href="https://docs.aspose.com/words/net/working-with-fields/">Working with Fields</a> documentation article.</para>
    /// </summary>
    /// <remarks>
    /// Sequentially numbers chapters, tables, figures, and other user-defined lists of items in a document.
    /// </remarks>
    [CodePorting.Translator.Cs2Cpp.CppForceForwardDeclaration("Aspose.Common.NullableInt32")]
    public class FieldSeq : Field, IFieldCodeTokenInfoProvider
    {
        internal override FieldUpdateAction UpdateCore()
        {
            if (IsOutOfMainTextAndNotValid)
                return new FieldUpdateActionInsertErrorMessage(this, "Error! Main Document Only.");

            if (IsSequenceIdentifierMissing)
                return new FieldUpdateActionInsertErrorMessage(this, "Error! No sequence specified.");

            if (IsBookmarkMissing(Updater.BookmarkCache))
                return new FieldUpdateActionInsertErrorMessage(this, Bookmark.ErrorBookmarkNotDefined);

            // All numeric formatting switches prevents field result from being hidden.
            if (HideFieldResult && Format.GeneralFormats.GetNumericFormat() == GeneralFormat.None)
                return new FieldUpdateActionApplyResult(this, string.Empty);

            Updater.DataProviders.Ensure(new FieldSeqDataProvider(Updater));

            Constant value = Updater.DataProviders.GetValue(this);

            return new FieldUpdateActionApplyResult(this, value);
        }

        internal bool IsError(BookmarkCache bookmarkCache)
        {
            return IsOutOfMainTextAndNotValid || IsSequenceIdentifierMissing || IsBookmarkMissing(bookmarkCache);
        }

        private bool IsBookmarkMissing(BookmarkCache bookmarkCache)
        {
            // SPEED Get a bookmark from a cache.
            return HasBookmarkName && (bookmarkCache[BookmarkName] == null);
        }

        private bool IsOutOfMainTextAndNotValid
        {
            get
            {
                // According to Word's behaviour and MS MVP's statements on the internet, a SEQ located outside
                // of the main document only works if the \c switch is present.
                return (Start.GetAncestor(NodeType.Body) == null) && !InsertClosestPrecedingNumber;
            }
        }

        private bool IsSequenceIdentifierMissing
        {
            get { return !StringUtil.HasChars(SequenceIdentifier); }
        }

        /// <summary>
        /// Gets or sets the name assigned to the series of items that are to be numbered.
        /// </summary>
        public string SequenceIdentifier
        {
            get { return FieldCodeCache.GetArgumentAsString(SequenceIdentifierArgumentIndex); }
            set { FieldCodeCache.SetArgument(SequenceIdentifierArgumentIndex, value); }
        }

        /// <summary>
        /// Gets or sets a bookmark name that refers to an item elsewhere in the document rather than in the current location.
        /// </summary>
        public string BookmarkName
        {
            get { return FieldCodeCache.GetArgumentAsString(BookmarkNameArgumentIndex); }
            set { FieldCodeCache.SetArgument(BookmarkNameArgumentIndex, value); }
        }

        internal bool HasBookmarkName
        {
            get { return StringUtil.HasChars(BookmarkName); }
        }

        /// <summary>
        /// Gets or sets whether to insert the closest preceding sequence number.
        /// </summary>
        /// <remarks>
        /// This is useful for inserting chapter numbers in headers or footers.
        /// </remarks>
        internal bool InsertClosestPrecedingNumber
        {
            get { return FieldCodeCache.HasSwitch(InsertClosestPrecedingNumberSwitch); }
            set { FieldCodeCache.SetSwitch(InsertClosestPrecedingNumberSwitch, value); }
        }

        /// <summary>
        /// Gets or sets whether to hide field result unless a general formatting switch is also present.
        /// </summary>
        /// <remarks>
        /// This switch can be used to refer to a SEQ field in a crossreference without printing the number.
        /// </remarks>
        internal bool HideFieldResult
        {
            get { return FieldCodeCache.HasSwitch(HideFieldResultSwitch); }
            set { FieldCodeCache.SetSwitch(HideFieldResultSwitch, value); }
        }

        /// <summary>
        /// Gets or sets whether to insert the next sequence number for the specified item.
        /// </summary>
        public bool InsertNextNumber
        {
            get { return FieldCodeCache.HasSwitch(InsertNextNumberSwitch); }
            set { FieldCodeCache.SetSwitch(InsertNextNumberSwitch, value); }
        }

        /// <summary>
        /// Gets or sets an integer number to reset the sequence number to. Returns -1 if the number is absent.
        /// </summary>
        public string ResetNumber //int
        {
            get { return FieldCodeCache.GetSwitchArgumentAsString(ResetNumberSwitch); }
            set { FieldCodeCache.SetSwitchAsInt32(ResetNumberSwitch, value); }
        }

        internal bool HasResetNumberSwitch
        {
            get { return FieldCodeCache.HasSwitch(ResetNumberSwitch); }
        }

        internal NullableInt32 ResetNumberAsInt32
        {
            get { return FieldCodeCache.GetSwitchArgumentAsInt32(ResetNumberSwitch); }
        }

        /// <summary>
        /// Gets or sets an integer number representing a heading level to reset the sequence number to.
        /// Returns -1 if the number is absent.
        /// </summary>
        public string ResetHeadingLevel //int
        {
            get { return FieldCodeCache.GetSwitchArgumentAsString(ResetHeadingLevelSwitch); }
            set { FieldCodeCache.SetSwitchAsInt32(ResetHeadingLevelSwitch, value); }
        }

        internal bool HasResetHeadingLevelSwitch
        {
            get { return FieldCodeCache.HasSwitch(ResetHeadingLevelSwitch); }
        }

        internal NullableInt32 ResetHeadingLevelAsInt32
        {
            get { return FieldCodeCache.GetSwitchArgumentAsInt32(ResetHeadingLevelSwitch); }
        }

        FieldSwitchType IFieldCodeTokenInfoProvider.GetSwitchType(string switchName)
        {
            switch (switchName)
            {
                case InsertClosestPrecedingNumberSwitch:
                case HideFieldResultSwitch:
                case InsertNextNumberSwitch:
                {
                    return FieldSwitchType.Flag;
                }
                case ResetNumberSwitch:
                case ResetHeadingLevelSwitch:
                {
                    return FieldSwitchType.HasArgument;
                }
                default:
                {
                    return FieldSwitchType.Unknown;
                }
            }
        }

        private const int SequenceIdentifierArgumentIndex = 0;
        private const int BookmarkNameArgumentIndex = 1;

        private const string InsertClosestPrecedingNumberSwitch = "\\c";
        private const string HideFieldResultSwitch = "\\h";
        private const string InsertNextNumberSwitch = "\\n";
        private const string ResetNumberSwitch = "\\r";
        private const string ResetHeadingLevelSwitch = "\\s";
    }
}
