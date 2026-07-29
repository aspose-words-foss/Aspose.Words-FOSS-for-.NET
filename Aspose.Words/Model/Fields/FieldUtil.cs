// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 07/10/2010 by Dmitry Vorobyev

using System;
using System.IO;
using System.Text.RegularExpressions;
using Aspose.Collections;
using Aspose.Common;
using Aspose.Words.Loading;

namespace Aspose.Words.Fields
{
    /// <summary>
    /// Provides field utility functions.
    /// </summary>
    internal static class FieldUtil
    {
        /// <summary>
        /// Returns a value indicating whether a field of the specified type requires its document's layout to be built
        /// before its update or not.
        /// </summary>
        internal static bool RequiresLayoutDocumentOnUpdate(FieldType fieldType)
        {
            switch (fieldType)
            {
                case FieldType.FieldIndex:
                case FieldType.FieldNoteRef:
                case FieldType.FieldTOA:
                    return true;
                default:
                    return IsComputedByLayout(fieldType);
            }
        }

        /// <summary>
        /// Returns true for field types whose value is provided by layout engine.
        /// Examples: PAGE, NUMPAGES etc.
        /// </summary>
        internal static bool IsComputedByLayout(FieldType fieldType)
        {
            switch (fieldType)
            {
                case FieldType.FieldPage:
                case FieldType.FieldNumPages:
                case FieldType.FieldPageRef:
                case FieldType.FieldSectionPages:
                case FieldType.FieldSection:
                    return true;
                default:
                    return false;
            }
        }

        /// <summary>
        /// Returns true for field types that should be updated when layout is updated.
        /// Word updates most of the fields in header/footer implicitly when saving to pdf.
        /// Some fields are not updated however.
        /// To mimic Word, we update these fields with layout.
        /// </summary>
        internal static bool IsUpdatedWithLayoutInHeaderFooter(FieldType fieldType)
        {
            switch (fieldType)
            {
                case FieldType.FieldStyleRef:
                case FieldType.FieldNoteRef:
                case FieldType.FieldSequence:
                    return true;
                default:
                    return IsComputedByLayout(fieldType);
            }
        }

        /// <summary>
        /// Returns a value indicating whether a field of the specified type located in header/footer can be updated
        /// when the update was not initiated by layout.
        /// </summary>
        internal static bool IsUpdatedWithoutLayoutInHeaderFooter(FieldType fieldType)
        {
            return !IsUpdatedWithLayoutInHeaderFooter(fieldType);
        }

        /// <summary>
        /// Returns true if this is a "dead" field. We don't really care about live or dead in
        /// the model, but this info is needed when writing to MS Word formats.
        /// </summary>
        internal static bool IsDead(FieldType fieldType)
        {
            // The Macro field can be dead or live. I ignore it here so it will get written as live.

            switch (fieldType)
            {
                case FieldType.FieldIndexEntry:
                case FieldType.FieldTOAEntry:
                case FieldType.FieldTOCEntry:
                case FieldType.FieldPrivate:
                case FieldType.FieldRefDoc:
                    return true;
                default:
                    return false;
            }
        }

        internal static bool IsFormField(FieldType fieldType)
        {
            switch (fieldType)
            {
                case FieldType.FieldFormTextInput:
                case FieldType.FieldFormCheckBox:
                case FieldType.FieldFormDropDown:
                    return true;
                default:
                    return false;
            }
        }

        /// <summary>
        /// Retrieves a field type from the specified string.
        /// If cannot find the corresponding field type, returns FieldType.FieldNone.
        /// Field code can have spaces in front. Not case-sensitive.
        /// WORDSNET-2727 Field code is not separated by space so we don't use space as separator anymore.
        /// </summary>
        internal static FieldType GetFieldType(string fieldCode)
        {
            fieldCode = fieldCode.Trim();

            if (!StringUtil.HasChars(fieldCode))
                return FieldType.FieldNone;

            // WORDSNET-940 where a formula field is not recognized.
            if (fieldCode[0] == '=')
                return FieldType.FieldFormula;

            fieldCode = GetFirstWord(fieldCode);
            int fieldType = gFieldNamesToTypes[fieldCode];

            return StringToIntDictionary.IsNullSubstitute(fieldType) ? FieldType.FieldNone : (FieldType)fieldType;
        }

        /// <summary>
        /// Returns FieldType if it is defined inside FieldType enum, if not returns FieldType.FieldNone.
        /// Switch used for java: Enum.IsDefined() is not autoportable since demands relatively slow reflection code.
        /// NOTE: The switch should be updated accordingly after each FieldType enum change.
        /// </summary>
        internal static FieldType GetFieldType(int fieldTypeId)
        {
            switch (fieldTypeId)
            {
                case (int)FieldType.FieldNone: return FieldType.FieldNone;
                case (int)FieldType.FieldCannotParse: return FieldType.FieldCannotParse;
                case (int)FieldType.FieldRefNoKeyword: return FieldType.FieldRefNoKeyword;
                case (int)FieldType.FieldRef: return FieldType.FieldRef;
                case (int)FieldType.FieldIndexEntry: return FieldType.FieldIndexEntry;
                case (int)FieldType.FieldFootnoteRef: return FieldType.FieldFootnoteRef;
                case (int)FieldType.FieldSet: return FieldType.FieldSet;
                case (int)FieldType.FieldIf: return FieldType.FieldIf;
                case (int)FieldType.FieldIndex: return FieldType.FieldIndex;
                case (int)FieldType.FieldTOCEntry: return FieldType.FieldTOCEntry;
                case (int)FieldType.FieldStyleRef: return FieldType.FieldStyleRef;
                case (int)FieldType.FieldRefDoc: return FieldType.FieldRefDoc;
                case (int)FieldType.FieldSequence: return FieldType.FieldSequence;
                case (int)FieldType.FieldTOC: return FieldType.FieldTOC;
                case (int)FieldType.FieldInfo: return FieldType.FieldInfo;
                case (int)FieldType.FieldTitle: return FieldType.FieldTitle;
                case (int)FieldType.FieldSubject: return FieldType.FieldSubject;
                case (int)FieldType.FieldAuthor: return FieldType.FieldAuthor;
                case (int)FieldType.FieldKeyword: return FieldType.FieldKeyword;
                case (int)FieldType.FieldComments: return FieldType.FieldComments;
                case (int)FieldType.FieldLastSavedBy: return FieldType.FieldLastSavedBy;
                case (int)FieldType.FieldCreateDate: return FieldType.FieldCreateDate;
                case (int)FieldType.FieldSaveDate: return FieldType.FieldSaveDate;
                case (int)FieldType.FieldPrintDate: return FieldType.FieldPrintDate;
                case (int)FieldType.FieldRevisionNum: return FieldType.FieldRevisionNum;
                case (int)FieldType.FieldEditTime: return FieldType.FieldEditTime;
                case (int)FieldType.FieldNumPages: return FieldType.FieldNumPages;
                case (int)FieldType.FieldNumWords: return FieldType.FieldNumWords;
                case (int)FieldType.FieldNumChars: return FieldType.FieldNumChars;
                case (int)FieldType.FieldFileName: return FieldType.FieldFileName;
                case (int)FieldType.FieldTemplate: return FieldType.FieldTemplate;
                case (int)FieldType.FieldDate: return FieldType.FieldDate;
                case (int)FieldType.FieldTime: return FieldType.FieldTime;
                case (int)FieldType.FieldPage: return FieldType.FieldPage;
                case (int)FieldType.FieldFormula: return FieldType.FieldFormula;
                case (int)FieldType.FieldQuote: return FieldType.FieldQuote;
                case (int)FieldType.FieldInclude: return FieldType.FieldInclude;
                case (int)FieldType.FieldPageRef: return FieldType.FieldPageRef;
                case (int)FieldType.FieldAsk: return FieldType.FieldAsk;
                case (int)FieldType.FieldFillIn: return FieldType.FieldFillIn;
                case (int)FieldType.FieldData: return FieldType.FieldData;
                case (int)FieldType.FieldNext: return FieldType.FieldNext;
                case (int)FieldType.FieldNextIf: return FieldType.FieldNextIf;
                case (int)FieldType.FieldSkipIf: return FieldType.FieldSkipIf;
                case (int)FieldType.FieldMergeRec: return FieldType.FieldMergeRec;
                case (int)FieldType.FieldDDE: return FieldType.FieldDDE;
                case (int)FieldType.FieldDDEAuto: return FieldType.FieldDDEAuto;
                case (int)FieldType.FieldGlossary: return FieldType.FieldGlossary;
                case (int)FieldType.FieldPrint: return FieldType.FieldPrint;
                case (int)FieldType.FieldEquation: return FieldType.FieldEquation;
                case (int)FieldType.FieldGoToButton: return FieldType.FieldGoToButton;
                case (int)FieldType.FieldMacroButton: return FieldType.FieldMacroButton;
                case (int)FieldType.FieldAutoNumOutline: return FieldType.FieldAutoNumOutline;
                case (int)FieldType.FieldAutoNumLegal: return FieldType.FieldAutoNumLegal;
                case (int)FieldType.FieldAutoNum: return FieldType.FieldAutoNum;
                case (int)FieldType.FieldImport: return FieldType.FieldImport;
                case (int)FieldType.FieldLink: return FieldType.FieldLink;
                case (int)FieldType.FieldSymbol: return FieldType.FieldSymbol;
                case (int)FieldType.FieldEmbed: return FieldType.FieldEmbed;
                case (int)FieldType.FieldMergeField: return FieldType.FieldMergeField;
                case (int)FieldType.FieldUserName: return FieldType.FieldUserName;
                case (int)FieldType.FieldUserInitials: return FieldType.FieldUserInitials;
                case (int)FieldType.FieldUserAddress: return FieldType.FieldUserAddress;
                case (int)FieldType.FieldBarcode: return FieldType.FieldBarcode;
                case (int)FieldType.FieldDisplayBarcode: return FieldType.FieldDisplayBarcode;
                case (int)FieldType.FieldMergeBarcode: return FieldType.FieldMergeBarcode;
                case (int)FieldType.FieldDocVariable: return FieldType.FieldDocVariable;
                case (int)FieldType.FieldSection: return FieldType.FieldSection;
                case (int)FieldType.FieldSectionPages: return FieldType.FieldSectionPages;
                case (int)FieldType.FieldIncludePicture: return FieldType.FieldIncludePicture;
                case (int)FieldType.FieldIncludeText: return FieldType.FieldIncludeText;
                case (int)FieldType.FieldFileSize: return FieldType.FieldFileSize;
                case (int)FieldType.FieldFormTextInput: return FieldType.FieldFormTextInput;
                case (int)FieldType.FieldFormCheckBox: return FieldType.FieldFormCheckBox;
                case (int)FieldType.FieldNoteRef: return FieldType.FieldNoteRef;
                case (int)FieldType.FieldTOA: return FieldType.FieldTOA;
                case (int)FieldType.FieldTOAEntry: return FieldType.FieldTOAEntry;
                case (int)FieldType.FieldMergeSeq: return FieldType.FieldMergeSeq;
                case (int)FieldType.FieldPrivate: return FieldType.FieldPrivate;
                case (int)FieldType.FieldDatabase: return FieldType.FieldDatabase;
                case (int)FieldType.FieldAutoText: return FieldType.FieldAutoText;
                case (int)FieldType.FieldCompare: return FieldType.FieldCompare;
                case (int)FieldType.FieldAddin: return FieldType.FieldAddin;
                case (int)FieldType.FieldFormDropDown: return FieldType.FieldFormDropDown;
                case (int)FieldType.FieldAdvance: return FieldType.FieldAdvance;
                case (int)FieldType.FieldDocProperty: return FieldType.FieldDocProperty;
                case (int)FieldType.FieldOcx: return FieldType.FieldOcx;
                case (int)FieldType.FieldHyperlink: return FieldType.FieldHyperlink;
                case (int)FieldType.FieldAutoTextList: return FieldType.FieldAutoTextList;
                case (int)FieldType.FieldListNum: return FieldType.FieldListNum;
                case (int)FieldType.FieldHtmlActiveX: return FieldType.FieldHtmlActiveX;
                case (int)FieldType.FieldBidiOutline: return FieldType.FieldBidiOutline;
                case (int)FieldType.FieldAddressBlock: return FieldType.FieldAddressBlock;
                case (int)FieldType.FieldGreetingLine: return FieldType.FieldGreetingLine;
                case (int)FieldType.FieldShape: return FieldType.FieldShape;
                case (int)FieldType.FieldCitation: return FieldType.FieldCitation;
                case (int)FieldType.FieldBibliography: return FieldType.FieldBibliography;

                default:
                    return FieldType.FieldNone;
            }
        }

        private static string GetFirstWord(string fieldCode)
        {
            Match matchResult = gFieldCodeRegex.Match(fieldCode);

            if (matchResult.Groups.Count > 0)
                return matchResult.Groups[matchResult.Groups.Count - 1].Value;

            return string.Empty;
        }

        internal static string FetchFieldCode(FieldType fieldType)
        {
            string fieldCode = FetchFieldCodeSafe(fieldType);
            if (fieldCode != null)
                return fieldCode;

            throw new InvalidOperationException(string.Format("Field type '{0}' is invalid or not supported.", FieldTypeToString(fieldType)));
        }

        internal static string FetchFieldCodeSafe(FieldType fieldType)
        {
            return gFieldTypesToNames[(int)fieldType];
        }

        /// <summary>
        /// Majority of field types can have (and do have) a field separator.
        /// However, some field types should not have a separator, otherwise it could create corrupted files.
        /// See comments below.
        /// </summary>
        internal static FieldSeparatorPresence GetSeparatorPresence(FieldType fieldType)
        {
            switch (fieldType)
            {
                case FieldType.FieldAdvance:
                case FieldType.FieldAutoNum:
                case FieldType.FieldAutoNumLegal:
                case FieldType.FieldAutoNumOutline:
                case FieldType.FieldBidiOutline:
                case FieldType.FieldGoToButton:
                case FieldType.FieldFormDropDown:
                case FieldType.FieldFormCheckBox:
                case FieldType.FieldListNum:
                case FieldType.FieldMacroButton:
                case FieldType.FieldPrint:
                case FieldType.FieldSymbol:
                case FieldType.FieldDisplayBarcode:
                case FieldType.FieldBarcode:
                    {
                        // These field types usually do not have a separator. If we output a separator for
                        // these fields into the DOC file, the file could come out corrupted.
                        return FieldSeparatorPresence.Never;
                    }
                case FieldType.FieldNone:
                case FieldType.FieldNext:
                case FieldType.FieldNextIf:
                case FieldType.FieldSkipIf:
                    {
                        // These fields may or may not have a separator. For example, NEXT and NEXTIF do not have separators
                        // when created, but after direct update a separator is inserted to set the result.
                        return FieldSeparatorPresence.Sometimes;
                    }
                default:
                    {
                        if (IsDead(fieldType))
                        {
                            // According to MS SPEC, dead fields cannot have a separator.
                            return FieldSeparatorPresence.Never;
                        }
                        else
                        {
                            // All other (the majority of) fields can have a separator and most of the time
                            // they do have it.
                            //
                            // Sometimes normal fields do not have a separator and that might cause hickups
                            // in some places in the code, it has to be improved to handle all fields with
                            // and without separators. MS Word seems to regenerate separator and result
                            // if needed, we should be able to do the same eventually.
                            return FieldSeparatorPresence.Always;
                        }
                    }
            }
        }

        /// <summary>
        /// Returns a value indicating whether the field result is locked and should not be updated.
        /// </summary>
        internal static bool GetEffectiveFieldLocked(Field field)
        {
            if (!field.IsLocked)
                return false;

            switch (field.Type)
            {
                // WORDSNET-25232 MS Word ignores fldLock attribute of PAGE and NUMPAGES fields in header/footer.
                case FieldType.FieldPage:
                case FieldType.FieldNumPages:
                    return !field.IsInHeaderFooter;
                default:
                    return true;
            }
        }

        /// <summary>
        /// Returns a value indicating whether the result of a field of the given type is invisible in MS Word editor.
        /// You should use this overload only when <see cref="HasConstantFieldResultVisibility"/> returns <c>true</c>
        /// for the given field type.
        /// </summary>
        internal static bool IsFieldResultInvisible(FieldType fieldType)
        {
            Debug.Assert(HasConstantFieldResultVisibility(fieldType));

            return ProducesBookmarkAsResult(fieldType);
        }

        /// <summary>
        /// Returns a value indicating whether the result of a field composed of the given field char is invisible
        /// in MS Word editor.
        /// </summary>
        internal static bool IsFieldResultInvisible(FieldChar fieldChar)
        {
            if (HasConstantFieldResultVisibility(fieldChar.FieldType))
                return IsFieldResultInvisible(fieldChar.FieldType);

            FieldBundle bundle = FieldBundle.GetFieldBundle(fieldChar);
            return IsFieldResultInvisible(bundle.Start, bundle.FieldCodeEnd);
        }

        /// <summary>
        /// Returns a value indicating whether the result of a field which code is strictly contained between
        /// the specified nodes is invisible in MS Word editor.
        /// </summary>
        internal static bool IsFieldResultInvisible(FieldStart fieldStart, FieldChar fieldCodeEnd)
        {
            if (HasConstantFieldResultVisibility(fieldStart.FieldType))
                return IsFieldResultInvisible(fieldStart.FieldType);

            return !FieldUnknown.IsBookmarkRef(fieldStart, fieldCodeEnd);
        }

        /// <summary>
        /// Returns a value indicating whether the result of the specified field is invisible in MS Word editor.
        /// </summary>
        /// <dev>
        /// Although this method could belong to <see cref="Field"/> class, it is more convenient to place it here
        /// to keep it in sync with the above overload.
        /// </dev>
        internal static bool IsFieldResultInvisible(Field field)
        {
            if (HasConstantFieldResultVisibility(field.Type))
                return IsFieldResultInvisible(field.Type);

            return !((FieldUnknown)field).IsBookmarkRef();
        }

        /// <summary>
        /// Returns a value indicating whether field result visibility for a field of the given type does not depend on
        /// anything but the field type itself.
        /// </summary>
        internal static bool HasConstantFieldResultVisibility(FieldType fieldType)
        {
            switch (fieldType)
            {
                case FieldType.FieldNone:
                case FieldType.FieldRefNoKeyword:
                    return false;
                default:
                    return true;
            }
        }

        /// <summary>
        /// Returns a value indicating whether a field of the given type produces a bookmark in its result while updating.
        /// </summary>
        internal static bool ProducesBookmarkAsResult(FieldType fieldType)
        {
            switch (fieldType)
            {
                case FieldType.FieldSet:
                case FieldType.FieldAsk:
                    return true;
                default:
                    return false;
            }
        }

        /// <summary>
        /// Returns true if the field always formats its result according to its locale, regardless of presence of the \@ switch.
        /// </summary>
        internal static bool AlwaysConsidersLocale(FieldType fieldType)
        {
            switch (fieldType)
            {
                case FieldType.FieldDate:
                case FieldType.FieldTime:
                case FieldType.FieldCreateDate:
                case FieldType.FieldPrintDate:
                case FieldType.FieldSaveDate:
                    return true;
                default:
                    return false;
            }
        }

        /// <summary>
        /// Returns the index of last argument that should be treated as verbatim, i.e. including all whitespaces and double quotes.
        /// -1 means there is no such argument.
        /// </summary>
        internal static int GetVerbatimArgumentIndex(string fieldCode)
        {
            return GetVerbatimArgumentIndex(GetFieldType(fieldCode));
        }

        /// <summary>
        /// Returns the index of last argument that should be treated as verbatim, i.e. including all whitespaces and double quotes.
        /// -1 means there is no such argument.
        /// </summary>
        internal static int GetVerbatimArgumentIndex(FieldType fieldType)
        {
            switch (fieldType)
            {
                case FieldType.FieldGoToButton:
                case FieldType.FieldMacroButton:
                    return 1;
                case FieldType.FieldEquation:
                    return 0;
                default:
                    return -1;
            }
        }

        internal static bool IgnoreSwitchesInFieldType(FieldType fieldType)
        {
            return fieldType != FieldType.FieldFormula;
        }

        internal static bool IgnoreSwitchesInFieldType(string fieldCode)
        {
            return IgnoreSwitchesInFieldType(GetFieldType(fieldCode));
        }

        internal static bool IgnoreSymbolicQuotes(FieldType fieldType, string switchName)
        {
            return fieldType == FieldType.FieldTOAEntry && switchName == FieldTA.LongCitationSwitch;
        }

        internal static bool IgnoreSymbolicQuotes(FieldType fieldType, int argumentIndex)
        {
            switch (fieldType)
            {
                case FieldType.FieldTOCEntry:
                    return argumentIndex == FieldTC.TextArgumentIndex;
                case FieldType.FieldIndexEntry:
                    return argumentIndex == FieldXE.TextArgumentIndex;
                default:
                    return false;
            }
        }

        /// <summary>
        /// Returns true if the specified field type should retain original formatting when no field result formatting switch
        /// is specified.
        /// </summary>
        internal static bool RetainOriginalFormatting(Field field)
        {
            switch (field.Type)
            {
                case FieldType.FieldIf:
                case FieldType.FieldIncludeText:
                case FieldType.FieldInclude:
                case FieldType.FieldIncludePicture:
                case FieldType.FieldImport:
                case FieldType.FieldRef:
                case FieldType.FieldRefNoKeyword:
                case FieldType.FieldNone:
                case FieldType.FieldQuote:
                case FieldType.FieldShape:
                case FieldType.FieldAutoText:
                case FieldType.FieldDatabase:
                    return true;
                case FieldType.FieldNoteRef:
                    FieldNoteRef fieldNoteRef = (FieldNoteRef)field;
                    return fieldNoteRef.InsertReferenceMark;
                default:
                    return false;
            }
        }

        /// <summary>
        /// Returns a value indicating whether a spell checking should be disabled for a field result of the specified type.
        /// </summary>
        internal static bool DisableResultSpellChecking(FieldType fieldType)
        {
            switch (fieldType)
            {
                case FieldType.FieldMergeField:
                case FieldType.FieldFormTextInput:
                    return true;
                default:
                    return false;
            }
        }

        /// <summary>
        /// Returns true if the specified field type should retain backslashes in result.
        /// </summary>
        internal static bool PreserveEscapingBackslashesInResult(FieldType fieldType)
        {
            switch (fieldType)
            {
                case FieldType.FieldIf:
                    return true;
                default:
                    return false;
            }
        }

        /// <summary>
        /// Returns true if the specified field type should retain double quotes in result.
        /// </summary>
        internal static bool PreserveDoubleQuotesInResult(FieldType fieldType)
        {
            switch (fieldType)
            {
                case FieldType.FieldQuote:
                case FieldType.FieldShape:
                    return true;
                default:
                    return false;
            }
        }

        /// <summary>
        /// Returns a <see cref="NodeJoinMode"/> value specifying how ancestor nodes of the specified field's
        /// separator and end should be joined while the field's result removal.
        /// </summary>
        internal static NodeJoinMode GetFieldResultRemovalNodeJoinMode(Field field)
        {
            // Normally the following does for all of the fields. Any field's result nodes should be joined
            // to next siblings while removal. This is needed to achieve the same results while the primary field
            // update (when there was no result before the update) and any subsequent update of this field due to
            // MS Word behavior.
            //
            // However, there is WORDSNET-8355 Normally, an INDEX field result should be removed in the same way.
            // But when a column number is provided through its field code, additional sections should be created.
            // MS Word does not create a full-value start section's clones in this case. Instead, it creates them
            // as linked-to-previous ones. AW mimics this as well. That is why we should make an exception in this
            // case and use NodeJoinMode.JoinToPreviousSibling. Note, however, that there is a minor ensuing issue.
            // If there is a text in the same paragraph together with the field's end, then there probably will be
            // a difference between results produced by MS Word and AW. However, an INDEX field's result is
            // multi-paragraph, so there is no sense to append any text after it in the same paragraph. So the case
            // should be extremely rare and hence we can leave this behavior as is until any related client request.
            //
            // Be consistent with FieldCodeIndex.HasNumberOfColumns.
            if (field.Type == FieldType.FieldIndex)
            {
                FieldIndex fieldIndex = (FieldIndex)field;
                if (fieldIndex.HasNumberOfColumnsSwitch && (fieldIndex.NumberOfColumnsAsInt32.GetValueOrDefault() > 0))
                    return NodeJoinMode.JoinToPreviousSibling;
            }

            return NodeJoinMode.JoinToNextSibling;
        }

        /// <summary>
        /// Returns a <see cref="NodeJoinMode"/> value specifying how ancestor nodes of the specified field's
        /// separator and end should be joined while the field's old result removal.
        /// </summary>
        internal static NodeJoinMode GetFieldOldResultRemovalNodeJoinMode(Field field)
        {
            // WORDSNET-10770 We should join separator and end nodes of INCLUDETEXT field, when they located in different sections.
            // Otherwise, redundant section added during field update.
            return field.HasSeparator
                && (field.Type == FieldType.FieldIncludeText || field.Type == FieldType.FieldInclude)
                && (field.Separator.GetAncestor(NodeType.Section) != field.End.GetAncestor(NodeType.Section))
                       ? NodeJoinMode.JoinToPreviousSibling
                       : NodeJoinMode.DontJoin;
        }

        /// <summary>
        /// Returns a value indicating thet a field of the specified type can contain fields in its result,
        /// which should be updated while updating of the field.
        /// </summary>
        internal static bool RequiresUpdateFieldsInResult(FieldType fieldType)
        {
            switch (fieldType)
            {
                case FieldType.FieldIncludeText:
                case FieldType.FieldInclude:
                case FieldType.FieldTOC:
                case FieldType.FieldHyperlink:
                case FieldType.FieldIncludePicture:
                case FieldType.FieldImport:
                case FieldType.FieldAutoText:
                case FieldType.FieldFormTextInput:
                    return true;
                default:
                    return false;
            }
        }

        /// <summary>
        /// Returns a value indicating whether the end position of the specified node range stands at a field end.
        /// </summary>
        internal static bool EndsWithFieldEnd(NodeRange range)
        {
            return !range.IsVoid && (range.End.Node.NodeType == NodeType.FieldEnd);
        }

        /// <summary>
        /// If the specified field is being updated returns a bookmark from the updater cache.
        /// Otherwise returns a direct document bookmark bypassing any caches.
        /// </summary>
        internal static Bookmark GetCachedBookmark(Field field, string bookmarkName)
        {
            return field.IsUpdating
                ? field.Updater.GetCachedBookmark(bookmarkName)
                : field.Document.Range.Bookmarks[bookmarkName];
        }

        /// <summary>
        /// Returns a value indicating whether the specified bookmark start relates to a dynamic bookmark.
        /// Dynamic bookmarks are products of ASK and SET fields' update and are located in their results.
        /// </summary>
        /// <dev>
        /// MS Word processes static and dynamic bookmarks differently while fields' updating. That's why
        /// this method is useful to keep here.
        /// </dev>
        internal static bool IsDynamicBookmark(BookmarkStart bookmarkStart)
        {
            Node decisiveNode = bookmarkStart.PreviousNonAnnotationSibling;

            return ((decisiveNode != null) &&
                    (decisiveNode.NodeType == NodeType.FieldSeparator) &&
                    ProducesBookmarkAsResult(((FieldSeparator)decisiveNode).FieldType));
        }

        internal static string FieldTypeToString(FieldType value)
        {
            switch (value)
            {
                case FieldType.FieldAddin:
                    return "FieldAddin";
                case FieldType.FieldAddressBlock:
                    return "FieldAddressBlock";
                case FieldType.FieldAdvance:
                    return "FieldAdvance";
                case FieldType.FieldAsk:
                    return "FieldAsk";
                case FieldType.FieldAuthor:
                    return "FieldAuthor";
                case FieldType.FieldAutoNum:
                    return "FieldAutoNum";
                case FieldType.FieldAutoNumLegal:
                    return "FieldAutoNumLegal";
                case FieldType.FieldAutoNumOutline:
                    return "FieldAutoNumOutline";
                case FieldType.FieldAutoText:
                    return "FieldAutoText";
                case FieldType.FieldAutoTextList:
                    return "FieldAutoTextList";
                case FieldType.FieldBarcode:
                    return "FieldBarcode";
                case FieldType.FieldBibliography:
                    return "FieldBibliography";
                case FieldType.FieldBidiOutline:
                    return "FieldBidiOutline";
                case FieldType.FieldCannotParse:
                    return "FieldCannotParse";
                case FieldType.FieldCitation:
                    return "FieldCitation";
                case FieldType.FieldComments:
                    return "FieldComments";
                case FieldType.FieldCompare:
                    return "FieldCompare";
                case FieldType.FieldCreateDate:
                    return "FieldCreateDate";
                case FieldType.FieldData:
                    return "FieldData";
                case FieldType.FieldDatabase:
                    return "FieldDatabase";
                case FieldType.FieldDate:
                    return "FieldDate";
                case FieldType.FieldDDE:
                    return "FieldDDE";
                case FieldType.FieldDDEAuto:
                    return "FieldDDEAuto";
                case FieldType.FieldDisplayBarcode:
                    return "FieldDisplayBarcode";
                case FieldType.FieldMergeBarcode:
                    return "FieldMergeBarcode";
                case FieldType.FieldDocProperty:
                    return "FieldDocProperty";
                case FieldType.FieldDocVariable:
                    return "FieldDocVariable";
                case FieldType.FieldEditTime:
                    return "FieldEditTime";
                case FieldType.FieldEmbed:
                    return "FieldEmbed";
                case FieldType.FieldEquation:
                    return "FieldEquation";
                case FieldType.FieldFileName:
                    return "FieldFileName";
                case FieldType.FieldFileSize:
                    return "FieldFileSize";
                case FieldType.FieldFillIn:
                    return "FieldFillIn";
                case FieldType.FieldFootnoteRef:
                    return "FieldFootnoteRef";
                case FieldType.FieldFormCheckBox:
                    return "FieldFormCheckBox";
                case FieldType.FieldFormDropDown:
                    return "FieldFormDropDown";
                case FieldType.FieldFormTextInput:
                    return "FieldFormTextInput";
                case FieldType.FieldFormula:
                    return "FieldFormula";
                case FieldType.FieldGlossary:
                    return "FieldGlossary";
                case FieldType.FieldGoToButton:
                    return "FieldGoToButton";
                case FieldType.FieldGreetingLine:
                    return "FieldGreetingLine";
                case FieldType.FieldHtmlActiveX:
                    return "FieldHtmlActiveX";
                case FieldType.FieldHyperlink:
                    return "FieldHyperlink";
                case FieldType.FieldIf:
                    return "FieldIf";
                case FieldType.FieldImport:
                    return "FieldImport";
                case FieldType.FieldInclude:
                    return "FieldInclude";
                case FieldType.FieldIncludePicture:
                    return "FieldIncludePicture";
                case FieldType.FieldIncludeText:
                    return "FieldIncludeText";
                case FieldType.FieldIndex:
                    return "FieldIndex";
                case FieldType.FieldIndexEntry:
                    return "FieldIndexEntry";
                case FieldType.FieldInfo:
                    return "FieldInfo";
                case FieldType.FieldKeyword:
                    return "FieldKeyword";
                case FieldType.FieldLastSavedBy:
                    return "FieldLastSavedBy";
                case FieldType.FieldLink:
                    return "FieldLink";
                case FieldType.FieldListNum:
                    return "FieldListNum";
                case FieldType.FieldMacroButton:
                    return "FieldMacroButton";
                case FieldType.FieldMergeField:
                    return "FieldMergeField";
                case FieldType.FieldMergeRec:
                    return "FieldMergeRec";
                case FieldType.FieldMergeSeq:
                    return "FieldMergeSeq";
                case FieldType.FieldNext:
                    return "FieldNext";
                case FieldType.FieldNextIf:
                    return "FieldNextIf";
                case FieldType.FieldNone:
                    return "FieldNone";
                case FieldType.FieldNoteRef:
                    return "FieldNoteRef";
                case FieldType.FieldNumChars:
                    return "FieldNumChars";
                case FieldType.FieldNumPages:
                    return "FieldNumPages";
                case FieldType.FieldNumWords:
                    return "FieldNumWords";
                case FieldType.FieldOcx:
                    return "FieldOcx";
                case FieldType.FieldPage:
                    return "FieldPage";
                case FieldType.FieldPageRef:
                    return "FieldPageRef";
                case FieldType.FieldPrint:
                    return "FieldPrint";
                case FieldType.FieldPrintDate:
                    return "FieldPrintDate";
                case FieldType.FieldPrivate:
                    return "FieldPrivate";
                case FieldType.FieldQuote:
                    return "FieldQuote";
                case FieldType.FieldRef:
                    return "FieldRef";
                case FieldType.FieldRefDoc:
                    return "FieldRefDoc";
                case FieldType.FieldRefNoKeyword:
                    return "FieldRefNoKeyword";
                case FieldType.FieldRevisionNum:
                    return "FieldRevisionNum";
                case FieldType.FieldSaveDate:
                    return "FieldSaveDate";
                case FieldType.FieldSection:
                    return "FieldSection";
                case FieldType.FieldSectionPages:
                    return "FieldSectionPages";
                case FieldType.FieldSequence:
                    return "FieldSequence";
                case FieldType.FieldSet:
                    return "FieldSet";
                case FieldType.FieldShape:
                    return "FieldShape";
                case FieldType.FieldSkipIf:
                    return "FieldSkipIf";
                case FieldType.FieldStyleRef:
                    return "FieldStyleRef";
                case FieldType.FieldSubject:
                    return "FieldSubject";
                case FieldType.FieldSymbol:
                    return "FieldSymbol";
                case FieldType.FieldTemplate:
                    return "FieldTemplate";
                case FieldType.FieldTime:
                    return "FieldTime";
                case FieldType.FieldTitle:
                    return "FieldTitle";
                case FieldType.FieldTOA:
                    return "FieldTOA";
                case FieldType.FieldTOAEntry:
                    return "FieldTOAEntry";
                case FieldType.FieldTOC:
                    return "FieldTOC";
                case FieldType.FieldTOCEntry:
                    return "FieldTOCEntry";
                case FieldType.FieldUserAddress:
                    return "FieldUserAddress";
                case FieldType.FieldUserInitials:
                    return "FieldUserInitials";
                case FieldType.FieldUserName:
                    return "FieldUserName";
                default:
                    throw new InvalidOperationException("Unknown FieldType: '" + value + "'. Please use TestFieldTypes.GenerateFieldTypeToString()" +
                                                        " to update FieldUtil.FieldTypeToString()");
            }
        }

        /// <summary>
        /// Returns a value indicating whether the result of a field of the given type is image.
        /// </summary>
        internal static bool IsImageFieldResult(FieldType type)
        {
            switch (type)
            {
                case FieldType.FieldIncludePicture:
                case FieldType.FieldImport:
                    return true;
                default:
                    return false;
            }
        }

        internal static NodeRange BuildFieldResultNodeRange(Node start, Node end)
        {
            return new NodeRange(
                    ExtractFirstLastNode(start, true),
                    ExtractFirstLastNode(end, false));
        }

        /// <summary>
        /// Indicates that field of given type can have custom private data.
        /// </summary>
        internal static bool CanHavePrivateData(FieldType fieldType)
        {
            return (fieldType == FieldType.FieldAddin) || (fieldType == FieldType.FieldPrivate);
        }

        private static Node ExtractFirstLastNode(Node node, bool isFirst)
        {
            if (!node.IsComposite)
                return node;

            if (node.NodeLevel == NodeLevel.Inline)
                return node;

            if (node.NodeType == NodeType.Table)
                return node;

            CompositeNode compositeNode = (CompositeNode)node;
            if (!compositeNode.HasChildNodes)
            {
                if (node.NodeType != NodeType.Paragraph)
                    return node;

                // Appending empty run prevents extra empty para in field result.
                compositeNode.AppendChild(new Run(node.Document, string.Empty));
            }

            return ExtractFirstLastNode(isFirst ? compositeNode.FirstChild : compositeNode.LastChild, isFirst);
        }

        internal static Stream OpenStream(string fileName, IResourceLoadingCallback resourceLoadingCallback)
        {
            if (resourceLoadingCallback == null)
                return OpenStream(fileName);

            ResourceLoadingArgs args = new ResourceLoadingArgs(string.Empty, fileName, ResourceType.Document);
            switch (resourceLoadingCallback.ResourceLoading(args))
            {
                case ResourceLoadingAction.Default:
                    return OpenStream(fileName);
                case ResourceLoadingAction.Skip:
                    return null;
                case ResourceLoadingAction.UserProvided:
                    return new MemoryStream(args.GetData());
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private static Stream OpenStream(string fileName)
        {
            // WORDSNET-4608 We try to create Uri safely. If unsuccessful we check file existence in addition
            // because an invalid uri might be a valid file path.
            Uri uri = UriUtil.CreateUriSafely(fileName);
            bool isHttpOrHttpsLink = IsUriHttpOrHttps(uri);

            if (isHttpOrHttpsLink)
                return SystemPal.OpenStreamFromHref(uri.AbsoluteUri);

            string path = GetLocalPathSafely(uri) ?? fileName;

            if (!File.Exists(path))
                return null;

            return File.OpenRead(path);
        }

        private static string GetLocalPathSafely(Uri uri)
        {
            if (uri == null)
                return null;

            try
            {
                // WORDSNET-9570 Handle an exception thrown when the source path is invalid.
                return uri.LocalPath;
            }
            catch
            {
                return null;
            }
        }

        private static bool IsUriHttpOrHttps(Uri uri)
        {
            if (uri == null || !uri.IsAbsoluteUri)
                return false;

            string scheme = uri.Scheme.ToLower();
            return scheme == "http" || scheme == "https";
        }

        static FieldUtil()
        {
            AddMapEntry(string.Empty, FieldType.FieldNone);
            AddMapEntry("=", FieldType.FieldFormula);
            AddMapEntry("ADDIN", FieldType.FieldAddin);
            AddMapEntry("ADDRESSBLOCK", FieldType.FieldAddressBlock);
            AddMapEntry("ADVANCE", FieldType.FieldAdvance);
            AddMapEntry("ASK", FieldType.FieldAsk);
            AddMapEntry("AUTHOR", FieldType.FieldAuthor);
            AddMapEntry("AUTONUM", FieldType.FieldAutoNum);
            AddMapEntry("AUTONUMLGL", FieldType.FieldAutoNumLegal);
            AddMapEntry("AUTONUMOUT", FieldType.FieldAutoNumOutline);
            AddMapEntry("AUTOTEXT", FieldType.FieldAutoText);
            AddMapEntry("AUTOTEXTLIST", FieldType.FieldAutoTextList);
            AddMapEntry("BARCODE", FieldType.FieldBarcode);
            AddMapEntry("BIBLIOGRAPHY", FieldType.FieldBibliography);
            AddMapEntry("BIDIOUTLINE", FieldType.FieldBidiOutline);
            AddMapEntry("CITATION", FieldType.FieldCitation);
            AddMapEntry("COMMENTS", FieldType.FieldComments);
            AddMapEntry("COMPARE", FieldType.FieldCompare);
            AddMapEntry("CREATEDATE", FieldType.FieldCreateDate);
            AddMapEntry("DATA", FieldType.FieldData); // In OOXML?
            AddMapEntry("DATABASE", FieldType.FieldDatabase);
            AddMapEntry("DATE", FieldType.FieldDate);
            AddMapEntry("DDE", FieldType.FieldDDE); // In OOXML?
            AddMapEntry("DDEAUTO", FieldType.FieldDDEAuto); // In OOXML?
            AddMapEntry("DISPLAYBARCODE", FieldType.FieldDisplayBarcode);
            AddMapEntry("MERGEBARCODE", FieldType.FieldMergeBarcode);
            AddMapEntry("DOCPROPERTY", FieldType.FieldDocProperty);
            AddMapEntry("DOCVARIABLE", FieldType.FieldDocVariable);
            AddMapEntry("EDITTIME", FieldType.FieldEditTime);
            AddMapEntry("EMBED", FieldType.FieldEmbed); // Not in OOXML.
            AddMapEntry("EQ", FieldType.FieldEquation);
            AddMapEntry("FILENAME", FieldType.FieldFileName);
            AddMapEntry("FILESIZE", FieldType.FieldFileSize);
            AddMapEntry("FILLIN", FieldType.FieldFillIn);
            AddMapEntry("FOOTNOTEREF", FieldType.FieldFootnoteRef); // In OOXML?
            AddMapEntry("FORMTEXT", FieldType.FieldFormTextInput);
            AddMapEntry("FORMCHECKBOX", FieldType.FieldFormCheckBox);
            AddMapEntry("FORMDROPDOWN", FieldType.FieldFormDropDown);
            AddMapEntry("GLOSSARY", FieldType.FieldGlossary);
            AddMapEntry("GOTOBUTTON", FieldType.FieldGoToButton);
            AddMapEntry("GREETINGLINE", FieldType.FieldGreetingLine);
            AddMapEntry("HYPERLINK", FieldType.FieldHyperlink);
            AddMapEntry("IF", FieldType.FieldIf);
            AddMapEntry("IMPORT", FieldType.FieldImport); // DD: FieldType.FieldImport is not used anywhere,
            // but allows to fix 9345 by keeping separator from
            // being nulled. Look for usuallyHasSeparator
            // var in the code for more.
            AddMapEntry("INCLUDE", FieldType.FieldInclude);
            AddMapEntry("INCLUDEPICTURE", FieldType.FieldIncludePicture);
            AddMapEntry("INCLUDETEXT", FieldType.FieldIncludeText);
            AddMapEntry("INDEX", FieldType.FieldIndex);
            AddMapEntry("INFO", FieldType.FieldInfo);
            AddMapEntry("KEYWORDS", FieldType.FieldKeyword);
            AddMapEntry("LASTSAVEDBY", FieldType.FieldLastSavedBy);
            AddMapEntry("LINK", FieldType.FieldLink);
            AddMapEntry("LISTNUM", FieldType.FieldListNum);
            AddMapEntry("MACROBUTTON", FieldType.FieldMacroButton);
            AddMapEntry("MERGEFIELD", FieldType.FieldMergeField);
            AddMapEntry("MERGEREC", FieldType.FieldMergeRec);
            AddMapEntry("MERGESEQ", FieldType.FieldMergeSeq);
            AddMapEntry("NEXT", FieldType.FieldNext);
            AddMapEntry("NEXTIF", FieldType.FieldNextIf);
            AddMapEntry("NOTEREF", FieldType.FieldNoteRef);
            AddMapEntry("NUMCHARS", FieldType.FieldNumChars);
            AddMapEntry("NUMPAGES", FieldType.FieldNumPages);
            AddMapEntry("NUMWORDS", FieldType.FieldNumWords);
            AddMapEntry("OCX", FieldType.FieldOcx); // Not in OXML.
            AddMapEntry("PAGE", FieldType.FieldPage);
            AddMapEntry("PAGEREF", FieldType.FieldPageRef);
            AddMapEntry("PRINT", FieldType.FieldPrint);
            AddMapEntry("PRINTDATE", FieldType.FieldPrintDate);
            AddMapEntry("PRIVATE", FieldType.FieldPrivate); //English, German
            AddMapEntry("PRIVE", FieldType.FieldPrivate, false); //French
            AddMapEntry("PRIVATESPAN", FieldType.FieldPrivate, false); //Spanish
            AddMapEntry("QUOTE", FieldType.FieldQuote);
            AddMapEntry("RD", FieldType.FieldRefDoc); //All languages
            AddMapEntry("REF", FieldType.FieldRef);
            AddMapEntry("REVNUM", FieldType.FieldRevisionNum);
            AddMapEntry("SAVEDATE", FieldType.FieldSaveDate);
            AddMapEntry("SECTION", FieldType.FieldSection);
            AddMapEntry("SECTIONPAGES", FieldType.FieldSectionPages);
            AddMapEntry("SEQ", FieldType.FieldSequence);
            AddMapEntry("SET", FieldType.FieldSet);
            AddMapEntry("SHAPE", FieldType.FieldShape);
            AddMapEntry("SKIPIF", FieldType.FieldSkipIf);
            AddMapEntry("STYLEREF", FieldType.FieldStyleRef);
            AddMapEntry("SUBJECT", FieldType.FieldSubject);
            AddMapEntry("SYMBOL", FieldType.FieldSymbol);
            AddMapEntry("TA", FieldType.FieldTOAEntry); //All languages
            AddMapEntry("TC", FieldType.FieldTOCEntry); //English, Spanish
            AddMapEntry("TE", FieldType.FieldTOCEntry, false); //French
            AddMapEntry("INHALT", FieldType.FieldTOCEntry, false); //German
            AddMapEntry("TEMPLATE", FieldType.FieldTemplate);
            AddMapEntry("TIME", FieldType.FieldTime);
            AddMapEntry("TITLE", FieldType.FieldTitle);
            AddMapEntry("TOA", FieldType.FieldTOA);
            AddMapEntry("TOC", FieldType.FieldTOC);
            AddMapEntry("USERADDRESS", FieldType.FieldUserAddress);
            AddMapEntry("USERINITIALS", FieldType.FieldUserInitials);
            AddMapEntry("USERNAME", FieldType.FieldUserName);
            AddMapEntry("XE", FieldType.FieldIndexEntry); //English, German
            AddMapEntry("EX", FieldType.FieldIndexEntry, false); //French
            AddMapEntry("E", FieldType.FieldIndexEntry, false); //Spanish
        }

        private static void AddMapEntry(string fieldName, FieldType fieldType)
        {
            AddMapEntry(fieldName, fieldType, true);
        }

        private static void AddMapEntry(string fieldName, FieldType fieldType, bool addToFieldTypesToNames)
        {
            gFieldNamesToTypes.Add(fieldName, (int)fieldType);

            if (addToFieldTypesToNames)
                gFieldTypesToNames.Add((int)fieldType, fieldName);
        }

        /// <summary>
        /// Map of field names into field types.
        /// </summary>
        private static readonly StringToIntDictionary gFieldNamesToTypes = new StringToIntDictionary(false);
        /// <summary>
        /// Map of field types into field names.
        /// </summary>
        private static readonly IntToObjDictionary<string> gFieldTypesToNames = new IntToObjDictionary<string>();

        // WORDSNET-12237 MS Word accepts special chars in field name.
        private static readonly Regex gFieldCodeRegex = new Regex(
            "^\\\"?\\\\?([\\w!#-~]*)\\\"?",
            RegexOptions.Compiled);
    }
}
