// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 22/01/2013 by Ivan Lyagin

using System;
using System.Collections.Generic;
using Aspose.Collections;
using Aspose.Collections.Generic;
using Aspose.Common;
using Aspose.Words.Lists;

namespace Aspose.Words.Fields
{
    /// <summary>
    /// Sequentially updates the data used to set the results of LISTNUM, AUTONUM, AUTONUMOUT and AUTONUMLGL fields
    /// for a document. This data can be accessed through <see cref="Document.GetFieldNumListLabel"/> in further.
    /// </summary>
    /// <remarks>
    /// This class may affect <see cref="ListNumberState"/> objects used to maintain paragraph list labels' updating
    /// routine. By this reason its methods must be sequentially called within this routine to make it work properly.
    /// </remarks>
    /// <dev>
    /// ECMA-376 describes how to build LISTNUM fields' results, but MS Word does it in its own way. Moreover,
    /// AUTONUM, AUTONUMOUT and AUTONUMLGL are MS Word transitional fields which are not described in ECMA-376.
    /// By this reason this class is made basing on experiments in MS Word main part of which is contained within
    /// TestFieldsNumbering.TestFieldNumGeneral() test.
    ///
    /// There are some shortcuts widely used in the comments of this class:
    /// - FIELDNUM means LISTNUM, AUTONUM, AUTONUMOUT and AUTONUMLGL;
    /// - AUTONUM sometimes means AUTONUM, AUTONUMOUT and AUTONUMLGL depending on a context.
    /// </dev>
    internal class FieldNumListLabelUpdater
    {
        /// <summary>
        /// Ctor.
        /// </summary>
        internal FieldNumListLabelUpdater(Document document)
        {
            mDocument = document;
            mDocument.ClearFieldNumListLabels();
        }

        /// <summary>
        /// Processes a paragraph entry.
        /// </summary>
        internal void ProcessParagraph(
            Paragraph paragraph,
            bool isListLabelUpdated,
            bool isInsideBody)
        {
            // Process current paragraph change.
            ProcessParagraphHeadingListLevel(paragraph);

            // Set default values for per-paragraph member variables.
            mParagraphIsInsideBody = isInsideBody;
            mParagraphHasAutoNumField = false;
            mParagraphHasListNumBreak = false;
            mParagraphLastListNumFieldEnd = null;
            mParagraphListNumBaseList = null;

            ClearParagraphListNumListLevels();

            // If the paragraph contains a list label and it is successfully updated then we should process it
            // like a LISTNUM entry before a LISTNUM break (see ProcessParagraphListNumEntry() and
            // GetNextListNumListNumberState() for details).
            if (isListLabelUpdated)
            {
                mParagraphListNumBaseList = paragraph.ListFormat.ListFinal;
                mCurrentListNumContext.BaseList = mParagraphListNumBaseList;

                // AM. Not really sure what this code does, just added check for null.
                if (mParagraphListNumBaseList != null)
                    SetParagraphListNumListLevel(mParagraphListNumBaseList, paragraph.ListFormat.ListLevelNumberFinal + 1);
            }
        }

        internal void ProcessArea(ListNumberGenerator generator)
        {
            ProcessCurrentListNumberGenerator(generator);
        }

        /// <summary>
        /// Processes current paragraph's heading level change.
        /// </summary>
        private void ProcessParagraphHeadingListLevel(Paragraph paragraph)
        {
            // Remember the heading level of the last paragraph if it has a heading style applied.
            if (mParagraphHeadingListLevel != InvalidListLevel)
            {
                mCurrentListNumContext.LastHeadingListLevel = mParagraphHeadingListLevel;

                // We should consider only paragraphs inside a body which contain a FIELDNUM entry for AUTONUM fields.
                if (mParagraphIsInsideBody && mParagraphHasAutoNumField)
                    mLastAutoNumHeadingListLevel = mParagraphHeadingListLevel;
            }

            // Calculate the zero-based heading level for the new paragraph basing on a style or outline level applied.
            mParagraphHeadingListLevel = ExtractParagraphHeadingListLevel(paragraph);
        }

        private static int ExtractParagraphHeadingListLevel(Paragraph paragraph)
        {
            Style headingStyle = GetHeadingStyle(paragraph.GetParagraphStyle(RevisionsView.Final));
            if (headingStyle != null)
                return headingStyle.Istd - StyleIndex.Heading1;

            // WORDSNET-16900 Use paragraph outline level when it does not have a heading style applied
            OutlineLevel outlineLevel = (OutlineLevel)paragraph.FetchParaAttr(ParaAttr.OutlineLevel, RevisionsView.Final);
            if (outlineLevel != OutlineLevel.BodyText)
                return (int)outlineLevel;

            return InvalidListLevel;
        }

        /// <summary>
        /// Processes current list number generator change.
        /// </summary>
        private void ProcessCurrentListNumberGenerator(ListNumberGenerator generator)
        {
            // If current list number generator is not changed, simply return.
            if (mCurrentListNumberGenerator == generator)
                return;

            // Make a LISTNUM context relative to the current list number generator current.
            // We have different LISTNUM contexts relative to different list number generators because
            // each of the number generators is relative to some numbering area (i.e. a document area
            // where numbering is separated from numberings in other areas) and hence we get different
            // LISTNUM contexts relative to different numbering areas which are independent.
            mCurrentListNumContext = mListNumContexts.GetValueOrNull(generator);
            if (mCurrentListNumContext == null)
            {
                mCurrentListNumContext = new ListNumContext();
                mListNumContexts[generator] = mCurrentListNumContext;
            }

            // Set current list number generator.
            mCurrentListNumberGenerator = generator;
        }

        /// <summary>
        /// Clears <see cref="mParagraphListNumListLevels"/> properly.
        /// </summary>
        private void ClearParagraphListNumListLevels()
        {
            // SortedIntegerList.Clear() reallocates resources regardless of whether the underlying list
            // contains any elements or not. Let's handle this properly as the list is empty almost all the time.
            if (mParagraphListNumListLevels.Count != 0)
                mParagraphListNumListLevels.Clear();
        }

        /// <summary>
        /// Sets the next list level to be applied for a LISTNUM field using the specified list.
        /// </summary>
        /// <remarks>
        /// The specified list level value is upper-normalized inside but it must be lower-normalized before
        /// any call of this method.
        /// </remarks>
        private void SetParagraphListNumListLevel(List list, int level)
        {
            level = System.Math.Min(level, gListLevelObjects.Length - 1);
            mParagraphListNumListLevels[list.ListDefId] = gListLevelObjects[level];
        }

        /// <summary>
        /// Gets the next list level to be applied for a LISTNUM field using the specified list.
        /// Returns <see cref="InvalidListLevel"/> if the requested value was not set previously.
        /// </summary>
        private int GetParagraphListNumListLevel(List list)
        {
            object levelObject = mParagraphListNumListLevels[list.ListDefId];
            if (levelObject == null)
                return InvalidListLevel;

            return Array.IndexOf(gListLevelObjects, levelObject);
        }

        /// <summary>
        /// Processes a field start entry.
        /// </summary>
        internal void ProcessFieldStart(FieldStart fieldStart)
        {
            // Do not process deleted fields.
            if (fieldStart.IsDeleteRevision)
                return;

            // Although MS-OE376 says that nested FIELDNUMs are ignored (see
            // http://msdn.microsoft.com/en-us/library/ff533802(v=office.12).aspx,
            // http://msdn.microsoft.com/en-us/library/ff534359(v=office.12).aspx, etc.)
            // this is not true for the fields like INCLUDETEXT, IF and SET according to experiments with MS Word.
            // MS Word processes FIELDNUM entries inside these fields' results as if they would not be nested.
            // Actually relative MS Word behavior is more complex than that so we just let FIELDNUMs to be nested
            // to any field's result hoping this will cover the majority of possible cases because if a user will
            // include FIELDNUM to any forbidden field's code and will not see its value in this field's result
            // then he probably will not use FIELDNUMs in this way at all.
            if (IsNestedNumFieldIgnored(fieldStart.FieldType))
                mFieldLockCount++;

            if ((mFieldLockCount == 1) && FieldNumUtil.IsFieldNum(fieldStart.FieldType))
                mDocument.SetFieldNumListLabel(fieldStart, BuildListLabel(fieldStart));
        }

        /// <summary>
        /// Builds <see cref="FieldNumListLabel"/> object for a FIELDNUM which start is specified.
        /// </summary>
        private FieldNumListLabel BuildListLabel(FieldStart fieldStart)
        {
            switch (fieldStart.FieldType)
            {
                case FieldType.FieldAutoNum:
                case FieldType.FieldAutoNumOutline:
                case FieldType.FieldAutoNumLegal:
                    ProcessAutoNumEntry();
                    break;
                default:
                    // Do nothing.
                    break;
            }

            switch (fieldStart.FieldType)
            {
                case FieldType.FieldAutoNum:
                {
                    FieldAutoNum fieldAutoNum = (FieldAutoNum)fieldStart.GetField();

                    // WORDSNET-9802 Build list label according to field formatting.
                    GeneralFormat fieldFormat = fieldAutoNum.Format.GeneralFormats[0];
                    return BuildAutoNumListLabel(
                        FieldNumListType.Number,
                        fieldAutoNum.SeparatorCharacterCore,
                        true,
                        fieldFormat);
                }
                case FieldType.FieldAutoNumOutline:
                {
                    return BuildAutoNumListLabel(
                        FieldNumListType.Outline,
                        FieldNumUtil.DefaultSeparator,
                        true,
                        GeneralFormat.None);
                }
                case FieldType.FieldAutoNumLegal:
                {
                    FieldAutoNumLgl fieldAutoNumLgl = (FieldAutoNumLgl)fieldStart.GetField();

                    // WORDSNET-9802 Build list label according to field formatting.
                    GeneralFormat fieldFormat = fieldAutoNumLgl.Format.GeneralFormats[0];
                    return BuildAutoNumListLabel(
                        FieldNumListType.Legal,
                        fieldAutoNumLgl.SeparatorCharacterCore,
                        !fieldAutoNumLgl.RemoveTrailingPeriod,
                        fieldFormat);
                }
                case FieldType.FieldListNum:
                {
                    FieldListNum fieldListNum = (FieldListNum)fieldStart.GetField();
                    return BuildListNumListLabel(fieldListNum);
                }
                default:
                {
                    throw new InvalidOperationException();
                }
            }
        }

        /// <summary>
        /// Processes FIELDNUM entry within the paragraph.
        /// </summary>
        private void ProcessAutoNumEntry()
        {
            // Only the first occurance in paragraph should be considered.
            if (mParagraphHasAutoNumField)
                return;

            mParagraphHasAutoNumField = true;

            // AUTONUMs which are contained only within the body numbering area correspond to a numbering state.
            // This gives us an opportunity to use a single container for AUTONUM list numbers in contrast to
            // LISTNUM contexts. We do not use a single ListNumberState object for this purpose because AUTONUM's
            // list numbers are changed in the way which can not be effectively mimicked using a ListNumberState object.
            if (!mParagraphIsInsideBody)
                return;

            // Create on the first demand.
            if (mAutoNumListNumbers == null)
                mAutoNumListNumbers = new int[ListLevel.MaxLevels + 1];

            // Increase AUTONUM list number for the current paragraph's heading level.
            // This is a place where LISTNUM fields affect AUTONUM fields.
            // Note, that we use an one-based index to access mParagraphHeadingListLevel items.
            // This is done to preserve an AUTONUM list number corresponding to InvalidListLevel heading
            // level as a zero item as it must be separated from the other ones not to influence on them.
            mAutoNumListNumbers[mParagraphHeadingListLevel + 1]++;
            if (mParagraphHeadingListLevel != InvalidListLevel)
            {
                // If we've encountered a non-InvalidListLevel heading level we should either set to zero
                // dependent AUTONUM list numbers.
                mAutoNumListNumbers[InvalidListLevel + 1] = 0;
                for (int i = mParagraphHeadingListLevel + 1; i < ListLevel.MaxLevels; i++)
                    mAutoNumListNumbers[i + 1] = 0;
            }
        }

        /// <summary>
        /// Builds <see cref="FieldNumListLabel"/> object for an AUTONUM field.
        /// </summary>
        private FieldNumListLabel BuildAutoNumListLabel(FieldNumListType listType, char separator, bool hasTrailingSeparator, GeneralFormat fieldFormat)
        {
            // Insert an error message as MS Word does in case of AUTONUM encountering outside of a document body.
            if (!mParagraphIsInsideBody)
                return BuildListLabel("Main Document Only.");

            List list = GetAutoNumList(listType, separator, hasTrailingSeparator, fieldFormat);
            ListNumberState state = GetNextAutoNumListNumberState(list);

            return BuildListLabel(state);
        }

        /// <summary>
        /// Gets an AUTONUM list.
        /// </summary>
        private List GetAutoNumList(FieldNumListType listType, char separator, bool hasTrailingSeparator, GeneralFormat fieldFormat)
        {
            // Use AUTONUM's list instead of AUTONUMOUT's one in case of a paragraph with a non-heading style applied.
            if ((listType == FieldNumListType.Outline) && (mParagraphHeadingListLevel == InvalidListLevel))
                listType = FieldNumListType.Number;

            NumberStyle numberStyle = GeneralFormatUtil.GeneralFormatToNumberStyle(fieldFormat);
            if (numberStyle == NumberStyle.None)
                numberStyle = NumberStyle.Arabic;

            // Try to find previously created AUTONUM list. Return if found.
            AutoNumListKey key = new AutoNumListKey(listType, separator, hasTrailingSeparator, numberStyle);
            List list = null;
            if (mAutoNumLists != null)
                list = mAutoNumLists.GetValueOrNull(key);
            if (list != null)
                return list;

            // Create AUTONUM list. We should use a single ListDef for all of the FIELDNUM lists.
            // Note, that it can be created together with the list if mFieldNumListDef is null
            // so we need to remember the list's ListDef in this case.
            list = FieldNumListFactory.CreateAutoNumList(
                mDocument,
                mFieldNumListDef,
                listType,
                separator,
                hasTrailingSeparator,
                numberStyle);

            mFieldNumListDef = list.ListDef;

            // Create on the first demand.
            if (mAutoNumLists == null)
                mAutoNumLists = new Dictionary<AutoNumListKey, List>();

            // Remember and return created list.
            mAutoNumLists[key] = list;
            return list;
        }

        /// <summary>
        /// Gets the next AUTONUM <see cref="ListNumberState"/> object to be used while
        /// <see cref="FieldNumListLabel"/> building.
        /// </summary>
        private ListNumberState GetNextAutoNumListNumberState(List list)
        {
            Debug.Assert(list.ListLevels.Count == ListLevel.MaxLevels);

            // Get a list level for the current paragraph and upper-normalize its value.
            int level = GetListLevelByHeading(mLastAutoNumHeadingListLevel);
            level = System.Math.Min(level, ListLevel.MaxLevels - 1);

            // AUTONUM list numbers (i.e. mAutoNumListNumbers items) are used as follows.
            // 1. For a paragraph with a heading style applied numbers for heading levels from the lowest one
            //    (excluding the one for InvalidListLevel) to the given one are used.
            // 2. For a paragraph with a non-heading style applied numbers for heading levels from the lowest one
            //    (excluding the one for InvalidListLevel) to the last encountered non-InvalidListLevel one are used
            //    as and InvalidListLevel one in a place of a list number for the next heading level.
            // An one-based indexing used to access mParagraphHeadingListLevel items shortcuts these steps.
            int[] numbers = new int[ListLevel.MaxLevels];
            numbers[level] = mAutoNumListNumbers[mParagraphHeadingListLevel + 1];
            for (int i = 0; i < level; i++)
                numbers[i] = mAutoNumListNumbers[i + 1];

            return new ListNumberState(list, level, numbers);
        }

        /// <summary>
        /// Gets a list level corresponding to the heading level of the current paragraph. If the current paragraph has
        /// non-heading style applied then the specified last encountered heading list level increased by 1 is returned.
        /// </summary>
        private int GetListLevelByHeading(int lastHeadingListLevel)
        {
            if (mParagraphHeadingListLevel != InvalidListLevel)
                return mParagraphHeadingListLevel;

            if (lastHeadingListLevel != InvalidListLevel)
                return (lastHeadingListLevel + 1);

            return 0;
        }

        /// <summary>
        /// Builds a <see cref="FieldNumListLabel"/> object by the given error message.
        /// </summary>
        private FieldNumListLabel BuildListLabel(string errorMessage)
        {
            // Make the error message bold as MS Word does.
            RunPr runPrOverrides = new RunPr();
            runPrOverrides.Bold = AttrBoolEx.True;
            if (mDocument.FieldOptions.IsBidiTextSupportedOnUpdate)
                runPrOverrides.BoldBi = AttrBoolEx.True;

            return new FieldNumListLabel(errorMessage, runPrOverrides);
        }

        /// <summary>
        /// Builds a <see cref="FieldNumListLabel"/> object by the given <see cref="ListNumberState"/> object.
        /// </summary>
        private static FieldNumListLabel BuildListLabel(ListNumberState state)
        {
            string text = ListLabelUtil.BuildListLabel(state, null);

            RunPr runPrOverrides = null;
            ListLevel listLevel = state.GetListLevel();
            if (listLevel.NumberStyle == NumberStyle.Bullet)
            {
                // Override font names for a bullet symbol.
                // Fortunately MS Word does not support shaped bullets for LISTNUM fields.
                runPrOverrides = new RunPr();
                listLevel.RunPr.ExpandToInclusive(runPrOverrides, RunPr.FontNameAttributes);
            }

            return new FieldNumListLabel(text, runPrOverrides, state);
        }

        /// <summary>
        /// Builds <see cref="FieldNumListLabel"/> object for a LISTNUM field.
        /// </summary>
        private FieldNumListLabel BuildListNumListLabel(FieldListNum field)
        {
            ProcessParagraphListNumGap(field.Start);

            List list = GetListNumList(field.ListName);
            ListNumberState state = GetNextListNumListNumberState(field, list);

            ProcessParagraphListNumEntry(field.End, list);

            return BuildListLabel(state);
        }

        /// <summary>
        /// Processes a gap between the current LISTNUM entry and the last encountered one.
        /// </summary>
        private void ProcessParagraphListNumGap(FieldStart fieldStart)
        {
            // If the current paragraph has a LISTNUM break then there is nothing left to do, return.
            if (mParagraphHasListNumBreak)
                return;

            // Process nodes of the gap to find a LISTNUM break.
            // Note, that mParagraphLastListNumFieldEnd can be null in case of the first LISTNUM entry,
            // in this case we correctly process the gap between this entry and the start of the current paragraph.
            for (Node node = fieldStart.PreviousSibling; node != mParagraphLastListNumFieldEnd; node = node.PreviousSibling)
            {
                if (IsListNumBreak(node))
                {
                    mParagraphHasListNumBreak = true;
                    break;
                }
            }
        }

        /// <summary>
        /// Returns a value indicating whether the specified node is a LISTNUM break or not.
        /// </summary>
        /// <remarks>
        /// LISTNUM breaks are nodes with some content which break a LISTNUM entry continuous sequence.
        /// LISTNUM breaks presence affects LISTNUM list-relative routines. See <see cref="mParagraphHasListNumBreak"/>
        /// dependencies for any details.
        /// </remarks>
        private static bool IsListNumBreak(Node node)
        {
            switch (node.NodeType)
            {
                case NodeType.Run:
                    return !StringUtil.ContainsOnlyWhitespaces(node.GetText());
                case NodeType.BookmarkStart:
                case NodeType.BookmarkEnd:
                    // This needs further investigation: Maybe some other node types should be skipped too.
                    return false;
                default:
                    return true;
            }
        }

        /// <summary>
        /// Gets a LISTNUM list by the specified <see cref="ListDef"/> name.
        /// </summary>
        private List GetListNumList(string listDefName)
        {
            // 1. If the ListDef name is not skipped, find and return the corresponding list.
            if (StringUtil.HasChars(listDefName))
                return GetListNumListCore(listDefName);

            // 2. Return the last encountered list while current paragraph processing if any.
            if (mParagraphListNumBaseList != null)
                return mParagraphListNumBaseList;

            // 3. Return the last encountered list while current numbering area processing if any.
            List baseList = mCurrentListNumContext.BaseList;
            if (baseList != null)
                return baseList;

            // 4. Return the NumberDefault list by default.
            return GetListNumListCore(ListNumNumberListDefName);
        }

        /// <summary>
        /// Gets a LISTNUM list by the specified non-empty <see cref="ListDef"/> name.
        /// </summary>
        private List GetListNumListCore(string listDefName)
        {
            Debug.Assert(StringUtil.HasChars(listDefName));

            // Try to find previously created LISTNUM list. Return if found.
            List list = (mListNumNamedLists != null) ? mListNumNamedLists[listDefName] : null;
            if (list != null)
                return list;

            FieldNumListType listType = GetListNumListType(listDefName);
            if (listType == FieldNumListType.None)
            {
                // Get a persisted list of the document.
                list = mDocument.Lists[listDefName] ?? GetListNumListCore(ListNumNumberListDefName);
            }
            else
            {
                // Create LISTNUM list. We should use a single ListDef for all of the FIELDNUM lists.
                // Note, that it can be created together with the list if mFieldNumListDef is null
                // so we need to remember the list's ListDef in this case.
                list = FieldNumListFactory.CreateListNumList(mDocument, mFieldNumListDef, listType);
                mFieldNumListDef = list.ListDef;
            }

            // Create on the first demand.
            if (mListNumNamedLists == null)
                mListNumNamedLists = new StringToObjDictionary<List>(false);

            // Remember and return recieved list.
            mListNumNamedLists[listDefName] = list;
            return list;
        }

        /// <summary>
        /// Gets a <see cref="FieldNumListType"/> by the specified predefined LISTNUM <see cref="ListDef"/> name.
        /// </summary>
        private static FieldNumListType GetListNumListType(string listDefName)
        {
            string[] fieldNumListDefNames = CurrentLanguageFieldNumListDefNames;

            if (StringUtil.EqualsIgnoreCase(listDefName, fieldNumListDefNames[ListNumNumberListDefNameIndex]))
                return FieldNumListType.Number;

            if (StringUtil.EqualsIgnoreCase(listDefName, fieldNumListDefNames[ListNumOutlineListDefNameIndex]))
                return FieldNumListType.Outline;

            if (StringUtil.EqualsIgnoreCase(listDefName, fieldNumListDefNames[ListNumLegalListDefNameIndex]))
                return FieldNumListType.Legal;

            return FieldNumListType.None;
        }

        private static string ListNumNumberListDefName
        {
            get
            {
                return CurrentLanguageFieldNumListDefNames[ListNumNumberListDefNameIndex];
            }
        }

        private static string[] CurrentLanguageFieldNumListDefNames
        {
            get
            {
                int language = LanguageOnly.ToLanguageOnly(SystemPal.GetCurrentCulture().LCID);
                return gFieldNumListDefNames.GetValueOrDefault(language, gDefaultFieldNumListDefNames);
            }
        }

        /// <summary>
        /// Gets the next LISTNUM <see cref="ListNumberState"/> object to be used while
        /// <see cref="FieldNumListLabel"/> building.
        /// </summary>
        private ListNumberState GetNextListNumListNumberState(FieldListNum field, List list)
        {
            ListNumberState state = mCurrentListNumberGenerator.GetCurrentListNumberState(list);
            int nextLevel = GetParagraphListNumListLevel(list);
            bool isFirstListDefEntry = (nextLevel == InvalidListLevel);

            // 1. Use a list level override provided by the field's code if any.
            int level = field.ListLevelCore;
            if (level < 0)
            {
                if (!isFirstListDefEntry)
                {
                    // 2. Use the next level to be applied calculated previously if any.
                    level = nextLevel;
                }
                else if ((list.ListDefId == FieldNumListFactory.FieldNumListDefId) &&
                         (field.HasListName || (mCurrentListNumContext.BaseList == null)))
                {
                    // 3. Use the current paragraph's heading level in case when:
                    //    - we deal with one of the predefined LISTNUM lists and
                    //    - the field's code provides any ListDef name (even if it is not one of the
                    //      predefined ones) or there is no list encountered while the current list
                    //      numbering area processing.
                    level = GetListLevelByHeading(mCurrentListNumContext.LastHeadingListLevel);
                }
                else
                {
                    // 4. If nothing above has triggered, use a stored value from a previous paragraph.
                    level = state.ListNumStartAtLevel;
                }
            }

            // If the current paragraph does not contain a LISTNUM break at this point then a list level
            // for the next entry should be increased by 1. If the specified list's ListDef is firstly met
            // after a LISTNUM break, set a list level for the next entry without increase.
            if (!mParagraphHasListNumBreak || isFirstListDefEntry)
                SetParagraphListNumListLevel(list, mParagraphHasListNumBreak ? level : (level + 1));

            // Advance the list numbering state.
            // If the current paragraph does not contain a LISTNUM break at this point, remember the list
            // level to use it in the next paragraph.
            state.NextItem(list, level, !mParagraphHasListNumBreak);

            // Use a list starting number override provided by the field's code if any.
            int number = field.StartingNumberCore;
            if (number >= 0)
                state.SetNumber(number);

            return state;
        }

        /// <summary>
        /// Performs final LISTNUM entry processing.
        /// </summary>
        private void ProcessParagraphListNumEntry(FieldEnd fieldEnd, List list)
        {
            // Set the last LISTNUM field end.
            mParagraphLastListNumFieldEnd = fieldEnd;

            // The list used to build the current LISTNUM entry's result should be stored to be reused in the current
            // paragraph if it does not contain a LISTNUM break at this point or we deal with the first LISTNUM entry
            // within the current paragraph.
            if (!mParagraphHasListNumBreak || (mParagraphListNumBaseList == null))
                mParagraphListNumBaseList = list;

            // The list used to build the current LISTNUM entry's result should be stored to be reused in a following
            // paragraph of the same numbering area only if the current paragraph does not contain a LISTNUM break
            // at this point.
            if (!mParagraphHasListNumBreak)
                mCurrentListNumContext.BaseList = list;
        }

        /// <summary>
        /// Processes a field separator entry.
        /// </summary>
        internal void ProcessFieldSeparator(FieldSeparator fieldSeparator)
        {
            // Do not process deleted fields.
            if (fieldSeparator.IsDeleteRevision)
                return;

            DecrementFieldLockCount(fieldSeparator.FieldType);
        }

        /// <summary>
        /// Reduces <see cref="mFieldLockCount"/> by 1 with check of a lower bound.
        /// </summary>
        private void DecrementFieldLockCount(FieldType fieldType)
        {
            if (IsNestedNumFieldIgnored(fieldType) && mFieldLockCount > 0)
                mFieldLockCount--;
        }

        private static bool IsNestedNumFieldIgnored(FieldType fieldType)
        {
            switch (fieldType)
            {
                case FieldType.FieldEquation:
                    return false;
                default:
                    return true;
            }
        }

        /// <summary>
        /// Processes a field end entry.
        /// </summary>
        internal void ProcessFieldEnd(FieldEnd fieldEnd)
        {
            // Do not process deleted fields.
            if (fieldEnd.IsDeleteRevision)
                return;

            if (!fieldEnd.HasSeparator)
                DecrementFieldLockCount(fieldEnd.FieldType);
        }

        /// <summary>
        /// Returns 'Heading' style if it presents in a chain of base styles
        /// or the specified <paramref name="style"/> is 'Heading' style itself.
        /// If there is no such 'Heading' style, then returns null.
        /// </summary>
        private static Style GetHeadingStyle(Style style)
        {
            if (style.IsHeading)
                return style;

            Style baseStyle = style.GetBaseStyle();
            if (baseStyle == null)
                return null;

            return GetHeadingStyle(baseStyle);
        }

        /// <summary>
        /// Stores LISTNUM data dependent on the current numbering area.
        /// </summary>
        private class ListNumContext
        {
            internal ListNumContext()
            {
                LastHeadingListLevel = InvalidListLevel;
            }

            internal List BaseList { get; set; }

            internal int LastHeadingListLevel { get; set; }
        }

        /// <summary>
        /// Represents a key value used to distinguish AUTONUM lists stored in <see cref="mAutoNumLists"/>.
        /// </summary>
        private class AutoNumListKey
        {
            internal AutoNumListKey(FieldNumListType listType, char separator, bool hasTrailingSeparator, NumberStyle style)
            {
                mListType = listType;
                mSeparator = separator;
                mHasTrailingSeparator = hasTrailingSeparator;
                mStyle = style;
            }

            private bool Equals(AutoNumListKey other)
            {
                if (ReferenceEquals(other, null))
                    return false;

                if (ReferenceEquals(other, this))
                    return true;

                return (other.mListType == mListType) &&
                    (other.mSeparator == mSeparator) &&
                    (other.mHasTrailingSeparator == mHasTrailingSeparator) &&
                    (other.mStyle == mStyle);
            }

            public override bool Equals(object obj)
            {
                return Equals(obj as AutoNumListKey);
            }

            public override int GetHashCode()
            {
                // Char's size is 2 bytes.
                return (mSeparator | Convert.ToInt32(mHasTrailingSeparator) << 16 | (int)mListType << 17);
            }

            private readonly FieldNumListType mListType;
            private readonly char mSeparator;
            private readonly bool mHasTrailingSeparator;
            private readonly NumberStyle mStyle;
        }

        private readonly Document mDocument;
        private ListDef mFieldNumListDef;
        private int mFieldLockCount;

        private ListNumberGenerator mCurrentListNumberGenerator;
        private ListNumContext mCurrentListNumContext;

        private int mParagraphHeadingListLevel = InvalidListLevel;
        private bool mParagraphIsInsideBody;
        private bool mParagraphHasAutoNumField;
        private bool mParagraphHasListNumBreak;
        private Node mParagraphLastListNumFieldEnd;
        private List mParagraphListNumBaseList;
        private readonly SortedIntegerListGeneric<object> mParagraphListNumListLevels = new SortedIntegerListGeneric<object>();

        private StringToObjDictionary<List> mListNumNamedLists;

        private readonly Dictionary<ListNumberGenerator, ListNumContext> mListNumContexts =
            new Dictionary<ListNumberGenerator, ListNumContext>();

        private Dictionary<AutoNumListKey, List> mAutoNumLists;
        private int[] mAutoNumListNumbers;
        private int mLastAutoNumHeadingListLevel = InvalidListLevel;

        private static readonly object[] gListLevelObjects = BuildListLevelObjects();

        private static object[] BuildListLevelObjects()
        {
            // Using of this array reduces frequent boxing/unboxing operations.
            object[] listLevelObjects = new object[ListLevel.MaxLevels];
            for (int i = 0; i < listLevelObjects.Length; i++)
                listLevelObjects[i] = new object();
            return listLevelObjects;
        }

        private static readonly string[] gDefaultFieldNumListDefNames =  { "NumberDefault", "OutlineDefault", "LegalDefault" };
        private static readonly IDictionary<int, string[]> gFieldNumListDefNames = new Dictionary<int, string[]>
        {
            { LanguageOnly.French, new[] { "NuméroDéfaut", "PlanDéfaut", "LégalDéfaut" } },
            { LanguageOnly.German, new[] { "NummerStandard", "GliederungStandard", "DezimalStandard" } },
            { LanguageOnly.Italian, new[] { "NumeroPredef", "StrutturaPredef", "LegalePredef" } },
            { LanguageOnly.Spanish, new[] { "NúmeroPredeterminado", "EsquemaPredeterminado", "OficioPredeterminado" } },
            { LanguageOnly.Portuguese, new[] { "Número Predefinido", "Destaque Predefinido", "Legal Predefinido" } },
        };

        private const int ListNumNumberListDefNameIndex = 0;
        private const int ListNumOutlineListDefNameIndex = 1;
        private const int ListNumLegalListDefNameIndex = 2;

        /// <summary>
        /// Invalid list level specifier.
        /// </summary>
        /// <remarks>
        /// If you are about to change the value of the constant consider that <see cref="mAutoNumListNumbers"/>'
        /// items access logic is tightly coupled with it.
        /// </remarks>
        private const int InvalidListLevel = -1;
    }
}
