// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 20/01/2012 by Alexey Morozov

using System;
using Aspose.Collections;
using Aspose.Collections.Generic;
using Aspose.Words.Drawing;
using Aspose.Words.Lists;
using Aspose.Words.Saving;

namespace Aspose.Words.Validation
{
    /// <summary>
    /// Performs validation of list collection i.e makes lists 1-based sorted array and sorts list definitions. Also updates paragraph ListId if needed.
    /// </summary>
    /// <remarks>
    /// AM. List collection should be validated mostly for binary DOC format because other formats has ListId attributes for list elements and 
    /// therefore list collection might be written unsorted (with minor writers update). But for binary doc the following are required:
    /// - list elements are written sorted by ListId (they have no ListId and accessed by element position) 
    /// - list definitions are written sorted by ListDefId for binary DOC otherwise Word can lost list formatting (Seems Word use binary search).
    ///
    /// I decided to sort lists regardless save format to get uniform behavior.
    /// </remarks>
    internal class ListValidator
    {
        internal ListValidator(ListCollection lists, SaveInfo saveInfo, IWarningCallback warningCallback)
        {
            mLists = lists;
            // Check this once and cache result for speed reason.
            mIsTranslationRequired = IsTranslationRequired;
            mSaveInfo = saveInfo;
            mWarningCallback = warningCallback;
        }

        /// <summary>
        /// Translates list IDs in document styles.
        /// </summary>
        internal void VisitDocumentStart()
        {
            // WORDSNET-27209 List ID translation in document styles must be performed at document start because
            // TableFormattingExpander may expand list ID attributes from styles to paragraph properties at visiting
            // a table end.
            if (mIsTranslationRequired)
            {
                StyleCollection styles = mLists.Document.Styles;
                foreach (Style style in styles)
                    TranslateListId(style.ParaPr);
            }
        }

        /// <summary>
        /// Validates list collection.
        /// </summary>
        /// <remarks>
        /// Valid list collection is:
        ///   - lists are sorted by ListId and are 1-based indexed array without gaps.
        ///   - list definitions are sorted by ListDefId.
        /// </remarks>
        internal void VisitDocumentEnd()
        {
            if (mIsTranslationRequired)
            {
                // Update lists with 1-based ListId.
                mLists.RemoveTranslation();
            }

            // Remove duplicated list definitions.
            IntToObjDictionary<ListDef> listDefs = new IntToObjDictionary<ListDef>();
            int index = 0;
            while (index < mLists.ListDefs.Count)
            {
                ListDef listDef = mLists.ListDefs[index];

                if (listDefs[listDef.ListDefId] != null)
                {
                    Warn(WarningType.MinorFormattingLoss, WarningStrings.ListValidatorNonUniqueDefinition);
                    mLists.ListDefs.RemoveAt(index);
                }
                else
                {
                    listDefs.Add(listDef.ListDefId, listDef);
                    index++;
                }
            }

            ValidateListDefs();

            // Sort list definitions.
            if (!IsListDefsSorted)
                mLists.ListDefs.Sort();

            ValidatePictureBullets();

            CheckDurableIds();
        }

        /// <summary>
        /// Checks <see cref="List.DurableId"/> values for duplicates. Marks the document as having DOCX extensions
        /// if any non-empty <see cref="List.DurableId"/> exists.
        /// </summary>
        private void CheckDurableIds()
        {
            if (IsUserDefinedCompliance && (OoxmlCompliance == OoxmlComplianceCore.Ecma376))
                return; // DurableId is not preserved.

            HashSetGeneric<int> durableIds = new HashSetGeneric<int>();

            // New lists are added to the end of mLists, so a simple mList enumeration preserves DurableId for older lists.
            foreach (List list in mLists)
            {
                int durableId = list.DurableId;
                if (durableId != 0)
                {
                    OoxmlComplianceInfo.MarkAsHasDocxExtensionsOf(mSaveInfo.Document, MsWordVersionCore.Word2016);

                    if (!durableIds.Contains(durableId))
                        durableIds.Add(durableId);
                    else
                        list.DurableId = 0; // Remove duplicate.
                }
            }
        }

        /// <summary>
        /// Updates paragraph ListId if needed.
        /// </summary>
        /// <param name="paragraph"></param>
        internal void VisitParagraphStart(Paragraph paragraph)
        {
            Debug.Assert(paragraph.IsListItemOriginal || paragraph.IsListItemFinal);

            if (mIsTranslationRequired)
            {
                TranslateListId(paragraph.ParaPr);

                if (paragraph.ParaPr.HasFormatRevision)
                    TranslateListId((ParaPr)paragraph.ParaPr.FormatRevision.RevPr);
            }
        }

        /// <summary>
        /// Updates ListId to new value taken from translation table.
        /// </summary>
        private void TranslateListId(ParaPr paraPr)
        {
            // Do not translate if it's not a list item.
            if (paraPr.ListId == 0)
                return;

            paraPr.ListId = mLists.GetTranslatedListId(paraPr.ListId);
        }

        /// <summary>
        /// Validates list definitions.
        /// </summary>
        /// <remarks>
        /// AM. Currently validates NumberFormat for bulleted lists.
        /// </remarks>
        private void ValidateListDefs()
        {
            ListDef[] listDefs = mLists.ListDefs.ToArray();

            foreach (ListDef listDef in listDefs)
            {
                foreach (ListLevel level in listDef.Levels)
                {
                    if (level.NumberStyle == NumberStyle.Bullet && IsPlaceHolder(level.NumberFormat))
                        level.NumberFormat = "";

                    if ((level.NumberStyle == NumberStyle.Custom) && !IsCustomNumberStyleSupported())
                    {
                        Warn(WarningType.MinorFormattingLoss,
                            string.Format(WarningStrings.NotSupportedNumberStyle, level.NumberStyle,
                                mSaveInfo.SaveFormat));
                        level.NumberStyle = NumberStyle.Arabic;
                    }
                }

                // WORDSNET-11410 MS Word removes reference to style from the list definition if 
                // list definition of this style hasn't reference to the style.
                // Despite we do this verification in DocxDocumentReaderBase.EnsureStylePointsToExistingList(),
                // the document might be changed later (see for example TestJira7405). So, we do this here again.
                Style style = listDef.Style;
                if ((style != null) && (style.List != null) && (style.List.ListDef != null))
                {
                    Style referenceStyleOfStyleListDef = style.List.ListDef.Style;
                    if (referenceStyleOfStyleListDef == null)
                        listDef.ListStyleIstd = StyleIndex.NoList;
                }
            }
        }

        /// <summary>
        /// Validates picture bullets.
        /// </summary>
        private void ValidatePictureBullets()
        {
            for (int i = 0; i < mLists.PictureBulletCount; i++)
            {
                Shape shape = mLists.GetPictureBullet(i);
                shape.FixZeroSize(false);
            }
        }

        /// <summary>
        /// Returns false if the document save format does not support the Custom number style. 
        /// Checks only for MS Word formats: DOCX, DOC, RTF, WML. The other formats can process it in their writers.
        /// </summary>
        private bool IsCustomNumberStyleSupported()
        {
            SaveFormat format = mSaveInfo.SaveFormat;
            return !(
                (mSaveInfo.IsOoxmlFormat && (OoxmlCompliance == OoxmlComplianceCore.Ecma376)) ||
                (format == SaveFormat.Doc) ||
                (format == SaveFormat.Dot) ||
                (format == SaveFormat.WordML) ||
                (format == SaveFormat.Rtf)
            );
        }

        /// <summary>
        /// Indicates that given number format is placeholder.
        /// </summary>
        private static bool IsPlaceHolder(string numberFormat)
        {
            return numberFormat.Length > 0 && ((int)numberFormat[0]) < 10;
        }

        /// <summary>
        /// Logs a warning to the user-provided warning callback.
        /// </summary>
        private void Warn(WarningType warningType, string description)
        {
            if (mWarningCallback != null)
                mWarningCallback.Warning(new WarningInfo(warningType, WarningSource.Validator, description));
        }

        /// <summary>
        /// Checks that list definitions are sorted by ListDefId.
        /// </summary>
        private bool IsListDefsSorted
        {
            get
            {
                if (mLists.ListDefCount == 0)
                    return true;

                int prev = int.MinValue;
                for (int i = 0; i < mLists.ListDefs.Count; i++)
                {
                    ListDef listDef = mLists.ListDefs[i];

                    if (listDef.ListDefId <= prev)
                        return false;

                    prev = listDef.ListDefId;
                }

                return true;
            }
        }

        /// <summary>
        /// Indicates that lists are NOT 1-based sorted array without gaps. 
        /// This means that paragraph's ListId should be updated.
        /// </summary>
        private bool IsTranslationRequired
        {
            get
            {
                // Don't need translation of empty lists.
                if (mLists.Count == 0)
                    return false;

                // Translation key-values should be the same.
                IntToIntDictionary.Enumerator enumerator = mLists.ListIdTranslationTable.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    if (enumerator.CurrentKey != enumerator.CurrentValue)
                        return true;
                }

                // Translation is needed if lists are not 1-based array.
                int firstListId = mLists.ListIdTranslationTable[1];
                if (IntToIntDictionary.IsNullSubstitute(firstListId))
                    throw new InvalidOperationException();

                return (firstListId != 1);
            }
        }

        /// <summary>
        /// Specifies the OOXML version for the output document on saving as DOCX formats.
        /// </summary>
        private OoxmlComplianceCore OoxmlCompliance
        {
            get
            {
                return OoxmlComplianceInfo.GetCompliance(mSaveInfo.Document.ComplianceInfo,
                    mSaveInfo.SaveOptions as OoxmlSaveOptions);
            }
        }

        /// <summary>
        /// Gets a flag indicating that user has set compliance explicitly.
        /// </summary>
        private bool IsUserDefinedCompliance
        {
            get
            {
                OoxmlSaveOptions ooxmlSaveOptions = mSaveInfo.SaveOptions as OoxmlSaveOptions;
                return (ooxmlSaveOptions != null) && (ooxmlSaveOptions.UserSetCompliance);
            }
        }

        private readonly SaveInfo mSaveInfo;
        private readonly IWarningCallback mWarningCallback;

        /// <summary>
        /// ListCollection being validated.
        /// </summary>
        private readonly ListCollection mLists;
        /// <summary>
        /// Holds IsTranslationRequired result.
        /// </summary>
        private readonly bool mIsTranslationRequired;
    }
}
