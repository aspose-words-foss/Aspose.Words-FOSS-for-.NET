// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 16/05/2017 by Nikolay Sezganov

using System.Collections.Generic;
using Aspose.Common;
using Aspose.Words.Markup;
using Aspose.Words.RW.Html.Css;

namespace Aspose.Words.RW.Html.Reader
{
    /// <summary>
    /// Responsible for importing HTML input elements as SDT controls in document.
    /// </summary>
    internal class HtmlControlAsSdtReader : HtmlControlReader
    {
        internal HtmlControlAsSdtReader(
            DocumentBuilder builder,
            DocumentFormatter documentFormatter)
            : base(builder, documentFormatter)
        {
            //Empty constructor.
        }

        protected override void InsertDateControl(
            string name,
            bool enabled,
            string value,
            int maxLength)
        {
            StructuredDocumentTag sdt = CreateSdtControlNode(name, SdtType.Date, enabled);
            sdt.DateDisplayFormat = FormatterPal.GetShortDatePatternCurrent();
            sdt.FullDate = FormatterPal.TryParseDateTimeCurrent(value);
        }

        protected override void InsertCheckboxControl(
            string name,
            bool disabled,
            bool editable,
            bool value)
        {
            StructuredDocumentTag sdt = CreateSdtControlNode(name, SdtType.Checkbox, editable);
            sdt.Checked = value;
        }

        protected override void InsertTextControl(
            string name,
            string value,
            bool enabled,
            int maxLength,
            string placeholder)
        {
            bool valueExists = StringUtil.HasChars(value);
            bool placeholderExists = StringUtil.HasChars(placeholder);

            StructuredDocumentTag sdt = CreateSdt(name, SdtType.PlainText, enabled);
            sdt.PlaceholderName = (placeholderExists)
                ? Builder.Document.SdtPlaceholderManager.CreateCustomPlaceholder(placeholder)
                : GetDefaultPlaceholderName(SdtType.PlainText);

            Debug.Assert(sdt.IsShowingPlaceholderText);
            Debug.Assert(sdt.FirstChild.NodeType == NodeType.Run);
            int placeholderStyle = ((Run)sdt.FirstChild).RunPr.Istd;
            sdt.GetChildNodes(NodeType.Any, false).Clear();

            Run run = new Run(Builder.Document, (valueExists) ? value : placeholder);
            Formatter.ToFont(run.Font, Builder.CurrentParagraph.ParagraphStyle);

            if (!valueExists && placeholderExists)
                run.RunPr.Istd = placeholderStyle;

            sdt.AppendChild(run);
            sdt.IsShowingPlaceholderText = !valueExists;
        }

        protected override void InsertDropDownControl(
            string name,
            bool enabled,
            IEnumerable<HtmlOptionElementInfo> items)
        {
            StructuredDocumentTag dropDown = CreateSdtControlNode(name, SdtType.DropDownList, enabled);
            foreach (HtmlOptionElementInfo htmlOptionInfo in items)
                AddItemToSdtDropDown(dropDown, htmlOptionInfo.Text, htmlOptionInfo.Value, htmlOptionInfo.IsSelected);
        }

        private StructuredDocumentTag CreateSdtControlNode(
            string name,
            SdtType sdtType,
            bool enabled)
        {
            StructuredDocumentTag sdt = CreateSdt(name, sdtType, enabled);
            if (sdt.IsShowingPlaceholderText)
                sdt.PlaceholderName = GetDefaultPlaceholderName(sdtType);
            return sdt;
        }

        private StructuredDocumentTag CreateSdt(
            string name,
            SdtType sdtType,
            bool enabled)
        {
            StructuredDocumentTag sdt = new StructuredDocumentTag(Builder.Document, sdtType, MarkupLevel.Inline);
            sdt.LockContents = !enabled;
            if (StringUtil.HasChars(name))
            {
                sdt.Title = name;
                sdt.Tag = name;
            }

            Formatter.ToFont(sdt.ContentsFont, Builder.CurrentParagraph.ParagraphStyle);
            Builder.InsertNode(sdt);
            return sdt;
        }

        private string GetDefaultPlaceholderName(SdtType sdtType)
        {
            return Builder.Document.SdtPlaceholderManager.FetchPlaceholderByType(sdtType).Name;
        }

        private static void AddItemToSdtDropDown(
            StructuredDocumentTag dropDown,
            string displayText,
            string value,
            bool isSelected)
        {
            if (!StringUtil.HasChars(value) && !StringUtil.HasChars(displayText))
                return;

            if (!StringUtil.HasChars(value))
                value = displayText;

            SdtListItem listItem = StringUtil.HasChars(displayText)
                ? new SdtListItem(displayText, value)
                : new SdtListItem(value);

            dropDown.ListItems.Add(listItem);
            if (isSelected || (dropDown.ListItems.Count == 1))
                dropDown.ListItems.SelectedValue = listItem;
        }
    }
}
