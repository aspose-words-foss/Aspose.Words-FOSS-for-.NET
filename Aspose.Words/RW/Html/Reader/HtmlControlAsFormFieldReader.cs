// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 16/05/2017 by Nikolay Sezganov

using System.Collections.Generic;
using Aspose.Words.Fields;
using Aspose.Words.RW.Html.Css;

namespace Aspose.Words.RW.Html.Reader
{
    /// <summary>
    /// Responsible for importing HTML input elements as Form Fields in document.
    /// </summary>
    internal class HtmlControlAsFormFieldReader : HtmlControlReader
    {
        internal HtmlControlAsFormFieldReader(
            DocumentBuilder builder,
            DocumentFormatter documentFormatter)
            : base(builder, documentFormatter)
        {
            // Empty constructor.
        }

        protected override void InsertDateControl(
            string name,
            bool editable,
            string value,
            int maxLength)
        {
            InsertAsText(name, value, editable, maxLength, TextFormFieldType.Date);
        }

        protected override void InsertCheckboxControl(
            string name,
            bool disabled,
            bool editable,
            bool isChecked)
        {
            FormField checkBox = Builder.InsertCheckBox(name, isChecked, MathUtil.DoubleToInt(Builder.Font.Size));
            checkBox.Enabled = !disabled;
        }

        protected override void InsertTextControl(
            string name,
            string value,
            bool enabled,
            int maxLength,
            string placeholder)
        {
            InsertAsText(name, value, enabled, maxLength, TextFormFieldType.Regular);
        }

        protected override void InsertDropDownControl(
            string name,
            bool enabled,
            IEnumerable<HtmlOptionElementInfo> items)
        {
            FormField formField = CreateFormFieldDropDown(name);
            foreach (HtmlOptionElementInfo htmlOptionInfo in items)
                AddItemToFormFieldDropDown(formField, htmlOptionInfo.Text, htmlOptionInfo.IsSelected);
        }

        private void InsertAsText(
            string name,
            string value,
            bool enabled,
            int maxLength,
            TextFormFieldType textType)
        {
            // Password char isn't displayed as "*"
            Formatter.ToFont(Builder.Font, Builder.CurrentParagraph.ParagraphStyle);
            FormField textInput = Builder.InsertTextInput(name, textType, "", value, maxLength);
            textInput.Enabled = enabled;
            if (!StringUtil.HasChars(value))
                textInput.Field.Result = FormField.DefaultTextInputValue;
        }

        private FormField CreateFormFieldDropDown(string name)
        {
            Formatter.ToFont(Builder.Font, Builder.CurrentParagraph.ParagraphStyle);
            FormField formFieldDropDown = Builder.InsertComboBox(name, new string[] { "" }, 0);
            Formatter.ToFont(formFieldDropDown.Font, Builder.CurrentParagraph.ParagraphStyle);
            formFieldDropDown.DropDownItems.Clear();
            formFieldDropDown.Enabled = true;
            return formFieldDropDown;
        }

        private void AddItemToFormFieldDropDown(
            FormField formField,
            string innerText,
            bool isSelectedItem)
        {
            if (formField.DropDownItems.Count < DropDownItemCollection.MaxItemsCount)
            {
                formField.DropDownItems.Add(innerText);
            }
            else
            {
                string lostItemText = innerText;

                if (isSelectedItem)
                {
                    lostItemText = formField.DropDownItems[DropDownItemCollection.MaxItemsCount - 1];
                    formField.DropDownItems[DropDownItemCollection.MaxItemsCount - 1] = innerText;
                }
                Warn(WarningType.DataLoss,
                    string.Format("Item \"{0}\" has been lost while adding DropDown elements.", lostItemText));
            }

            if (isSelectedItem)
                formField.DropDownSelectedIndex = formField.DropDownItems.Count - 1;
        }

        /// <summary>
        /// Logs a warning to the user-provided warning callback.
        /// </summary>
        private void Warn(WarningType warningType, string description)
        {
            if (Builder.Document.WarningCallback != null)
                Builder.Document.WarningCallback.Warning(new WarningInfo(warningType, WarningSource.Html, description));
        }
    }
}
