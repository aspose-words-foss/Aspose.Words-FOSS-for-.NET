// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 05/11/2013 by Andrey Noskov

using System;
using Aspose.Common;

namespace Aspose.Words.Markup
{
    /// <summary>
    /// Implements updating content of not data bound SDT.
    /// </summary>
    internal class SdtContentUpdater
    {
        /// <summary>
        /// Updates structured document tag content without bound data.
        /// </summary>
        internal static void UpdateNonBoundDataContent(StructuredDocumentTag sdt)
        {
            if (!sdt.XmlMapping.IsEmpty || (sdt.Document.NodeType == NodeType.GlossaryDocument))
                return;

            switch (sdt.SdtType)
            {
                case SdtType.Date:
                    UpdateDateFromFullDate(sdt);
                    break;

                case SdtType.DropDownList:
                case SdtType.ComboBox:
                    UpdateSelectedItemContent(sdt);
                    break;

                case SdtType.Checkbox:
                    SdtContentHelper.UpdateCheckboxContent(sdt);
                    break;

                default:
                    // Do nothing.
                    break;
            }
        }

        /// <summary>
        /// If combobox or dropdown list has selected value set, then we'll have to set IsShowingPlaceholderText to false and
        /// and update sdt content with value text to make sure MS Word shows it ok.
        /// </summary>
        private static void UpdateSelectedItemContent(StructuredDocumentTag sdt)
        {
            SdtListItemCollection items = sdt.ListItems;
            if (items.SelectedValue != null)
            {
                sdt.IsShowingPlaceholderText = false;
                SdtContentHelper.InsertContent(sdt, new Run(sdt.FetchDocument(), items.SelectedValue.DisplayText), true);
            }
        }

        /// <summary>
        /// Updates content of Sdt with FullDate according to the current sdt datetime display format.
        /// </summary>
        private static void UpdateDateFromFullDate(StructuredDocumentTag sdt)
        {
            if (!((SdtDate)sdt.ControlProperties).NeedToUpdateContent)
                return;

            string fieldUpdateCultureName = LocaleConverter.LocaleToDocxTag(sdt.DateDisplayLocale);
            bool needSetLocation = StringUtil.HasChars(fieldUpdateCultureName);

            try
            {
                if (needSetLocation)
                {
                    SystemPal.SaveCulture();
                    SystemPal.SetCulture(fieldUpdateCultureName);
                }

                if (sdt.FullDate != DateTime.MinValue)
                {
                    string result = WordUtil.FormatDateTime(sdt.FullDate, sdt.DateDisplayFormat, sdt.DateDisplayLocale);

                    SdtContentHelper.SetDisplayText(sdt, result);
                }
                else
                {
                    SdtContentHelper.ReplaceContentWithPlaceholder(sdt);
                }
            }
            finally
            {
                if (needSetLocation)
                    SystemPal.RestoreCulture();
            }
        }
    }
}
