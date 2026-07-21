// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 02/09/2011 by Dmitry Vorobyev

using System.Text.RegularExpressions;
using Aspose.Collections;
using Aspose.Collections.Generic;

namespace Aspose.Words.Fields
{
    /// <summary>
    /// Implements an unknown or unrecognized field.
    /// <para>To learn more, visit the <a href="https://docs.aspose.com/words/net/working-with-fields/">Working with Fields</a> documentation article.</para>
    /// </summary>
    /// <dev>
    /// Such fields are normally treated as references to bookmarks.
    /// </dev>
    public class FieldUnknown : Field, IMergeFieldSurrogate
    {
        internal override FieldUpdateStage GetUpdateStage()
        {
            // If the field is not a bookmark reference, then there is no reason to defer the field's update.
            if (!IsBookmarkRef())
                return FieldUpdateStage.MainLoop;

            return FieldRefUtil.GetFieldUpdateStage(this, BookmarkName, false);
        }

        internal override FieldUpdateAction UpdateCore()
        {
            // Do not update the field, if it is not a bookmark reference.
            if (!IsBookmarkRef())
                return new FieldUpdateActionDoNothing(this);

            // Forcibly insert field separator here.
            EnsureSeparator(true);

            // SPEED Get a bookmark from a cache.
            Bookmark bookmark = FieldUtil.GetCachedBookmark(this, BookmarkName);
            if (bookmark == null)
                return new FieldUpdateActionInsertErrorMessage(this, BookmarkNotDefinedErrorMessage);

            // WORDSNET-22358 Get bookmark node range before old field result is removed because bookmark may be inside.
            NodeRange bookmarkRange = bookmark.GetNodeRange();
            if (bookmarkRange.IsVoid)
                return new FieldUpdateActionInsertErrorMessage(this, BookmarkNotDefinedErrorMessage);

            bool hasBookmarkInResult;
            Paragraph bookmarkParagraph = FieldRefUtil.BeginFieldUpdate(this, bookmark, false, out hasBookmarkInResult);

            return FieldRefUtil.EndFieldUpdate(
                this,
                bookmark,
                bookmarkRange,
                bookmarkParagraph,
                hasBookmarkInResult,
                false);
        }

        [CodePorting.Translator.Cs2Cpp.CppForceSharedApi]
        string IMergeFieldSurrogate.GetMergeFieldName()
        {
            return BookmarkName;
        }

        [CodePorting.Translator.Cs2Cpp.CppForceSharedApi]
        bool IMergeFieldSurrogate.CanWorkAsMergeField()
        {
            return !string.IsNullOrEmpty(BookmarkName);
        }

        [CodePorting.Translator.Cs2Cpp.CppForceSharedApi]
        bool IMergeFieldSurrogate.IsMergeValueRequired()
        {
            return true;
        }

        /// <summary>
        /// Returns a value indicating whether this field is a bookmark reference.
        /// </summary>
        internal bool IsBookmarkRef()
        {
            return IsBookmarkRef(BookmarkName, Type);
        }

        /// <summary>
        /// Returns a value indicating whether a field which code is strictly contained between the specified nodes
        /// is a bookmark reference. It should be a field of type <see cref="FieldType.FieldNone"/> or
        /// <see cref="FieldType.FieldRefNoKeyword"/>.
        /// </summary>
        internal static bool IsBookmarkRef(FieldStart fieldStart, FieldChar fieldCodeEnd)
        {
            return IsBookmarkRef(FieldCodeParser.GetFieldType(fieldStart, fieldCodeEnd), fieldStart.FieldType);
        }

        /// <summary>
        /// Returns a value indicating whether a field with the specified field code and field type can be treated
        /// as a bookmark reference.
        /// </summary>
        private static bool IsBookmarkRef(string fieldCode, FieldType fieldType)
        {
            Debug.Assert((fieldType == FieldType.FieldNone) || (fieldType == FieldType.FieldRefNoKeyword));

            // Get an actual field type from the field code.
            FieldType actualFieldType = FieldUtil.GetFieldType(fieldCode);

            // Get a surrogate field type from the field start.
            // WORDSNET-8031 FieldType.FieldRefNoKeyword should be treated as FieldType.FieldNone.
            FieldType surrogateFieldType = fieldType;
            if (surrogateFieldType == FieldType.FieldRefNoKeyword)
                surrogateFieldType = FieldType.FieldNone;

            // If the field is broken, it is definitely not a bookmark reference.
            if (actualFieldType != surrogateFieldType)
                return false;

            // The field can be treated as a bookmark reference only if a bookmark name is provided through its code.
            // WORDSNET-10968
            // - Bookmark name are limited to 40 characters. All symbols over this limit are ignored.
            // - Bookmark name must contain only alphabetical characters, numbers and underline symbols.
            // - Bookmark name must start with an alphabetical character or an underline symbol.
            // WORDSNET-12492 Allow creation of bookmarks over 40 chars long when the target document has Pdf format.
            // Pdf spec has not any limitation for bookmark length. 40 characters bookmark length is a MSW feature.
            // WORDSNET-25891 MS Word disallows reserved names to be used as bookmark reference (MS-OI29500 2.1.495.g).
            return !gReservedNames.Contains(fieldCode) && gBookmarkNameRegex.IsMatch(fieldCode);
        }

        /// <summary>
        /// Gets referenced bookmark name.
        /// </summary>
        private string BookmarkName
        {
            get { return FieldCodeCache.FieldType; }
        }

        private static readonly Regex gBookmarkNameRegex = new Regex(@"^(?!\d)\w+$");
        private static readonly ISetGeneric<string> gReservedNames = new CaseInsensitiveHashSet(
            "ADDIN",
            "CONTROL",
            "DATA",
            "DISPLAYNFC",
            "EMBED",
            "FTNREF",
            "HTMLCONTROL");

        private const string BookmarkNotDefinedErrorMessage = "Error! Bookmark not defined.";
    }
}
