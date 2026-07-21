// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 16/05/2017 by Nikolay Sezganov

using System.Collections.Generic;
using System.IO;
using Aspose.JavaAttributes;
using Aspose.Words.Drawing.Ole.Core;
using Aspose.Words.RW.Html.Css;
using Aspose.Words.RW.Html.Parser;
using Aspose.Words.RW.HtmlCommon;
#if NETSTANDARD
using Image = SkiaSharp.SKBitmap;
#else
#endif

namespace Aspose.Words.RW.Html.Reader
{
    /// <summary>
    /// Base class responsible for importing HTML 'input' and 'select' elements into corresponding controls in document.
    /// </summary>
    internal abstract class HtmlControlReader
    {
        /// <summary>
        /// Ctor.
        /// </summary>
        protected HtmlControlReader(DocumentBuilder builder, DocumentFormatter documentFormatter)
        {
            Debug.Assert(builder != null);
            Debug.Assert(documentFormatter != null);

            mBuilder = builder;
            mDocumentFormatter = documentFormatter;
        }

        internal void HandleInput(HtmlElementNode node, string type)
        {
            string name = node.Attributes.GetAttributeValue("name", "");
            string value = node.Attributes.GetAttributeValue("value", "");
            int maxLength = node.Attributes.GetAttributeValue("maxlength", 0);
            bool disabled = node.Attributes["disabled"] != null;
            bool readOnly = node.Attributes["readonly"] != null;
            bool editable = !disabled && !readOnly;

            switch (type)
            {
                case "checkbox":
                {
                    bool isChecked = node.Attributes["checked"] != null;
                    InsertCheckboxControl(name, disabled, editable, isChecked);
                    break;
                }
                case "date":
                {
                    InsertDateControl(name, editable, value, maxLength);
                    break;
                }
                case "radio":
                {
                    InsertRadioButton(node);
                    break;
                }
                case "reset":
                {
                    mDocumentFormatter.ToFont(Builder.Font, Builder.CurrentParagraph.ParagraphStyle);
                    Builder.InsertField("MACROBUTTON ResetFormField [" + value + "]");
                    break;
                }
                case "submit":
                case "button":
                {
                    mDocumentFormatter.ToFont(Builder.Font, Builder.CurrentParagraph.ParagraphStyle);
                    Builder.InsertField("MACROBUTTON DoFieldClick  [" + value + "]");
                    break;
                }
                case "hidden": //unusable
                {
                    break;
                }
                default:
                {
                    // WORDSNET-21408 Take into account a placeholder attribute of an input HTML element.
                    string placeholder = node.Attributes.GetAttributeValue("placeholder", "");
                    InsertTextControl(name, value, editable, maxLength, placeholder);
                    break;
                }
            }
        }

        internal void HandleSelect(HtmlElementNode node, IList<HtmlOptionElementInfo> items)
        {
            string name = node.Attributes.GetAttributeValue("name", "dropdown");
            bool disabled = node.Attributes["disabled"] != null;
            bool readOnly = node.Attributes["readonly"] != null;

            InsertDropDownControl(name, !disabled && !readOnly, items);
        }

        [JavaThrows(true)]
        protected abstract void InsertDateControl(string name, bool editable, string value, int maxLength);

        [JavaThrows(true)]
        protected abstract void InsertCheckboxControl(string name, bool disabled, bool editable, bool isChecked);

        [JavaThrows(true)]
        protected abstract void InsertTextControl(string name, string value, bool enabled, int maxLength, string placeholder);

        [JavaThrows(true)]
        protected abstract void InsertDropDownControl(string name, bool enabled, IEnumerable<HtmlOptionElementInfo> items);

        protected DocumentBuilder Builder
        {
            get { return mBuilder; }
        }

        protected DocumentFormatter Formatter
        {
            get { return mDocumentFormatter; }
        }

        private void InsertRadioButton(HtmlElementNode node)
        {
            // WORDSNET-14401 Radio buttons were not shown as 'selected' or 'checked' when converting to fixed html.
            bool isChecked = node.Attributes["checked"] != null;
            byte[] imageBytes = GetRadioButtonImage(Builder.Document, isChecked);

            HtmlElementNode inputNode = new HtmlElementNode("input");
            inputNode.Attributes.Add(new HtmlAttribute("type", "radio"));
            string[] supportedAttributes = new string[] { "name", "disabled", "checked", "value", "required", "form", "alt" };
            foreach (string attrName in supportedAttributes)
            {
                HtmlAttribute attribute = node.Attributes[attrName];
                if (attribute != null)
                {
                    // MS Word "understands" only empty 'checked' or 'checked=true' attributes. We can't generate 
                    // empty attributes because use SerializeToXml() method (XML doesn't support empty attributes),
                    // so we use 'checked=true' to check on the radio button control.
                    if (attrName == "checked")
                    {
                        attribute = new HtmlAttribute("checked", "true");
                    }

                    inputNode.Attributes.Add(attribute);
                }
            }

            HtmlOleControl oleControl = new HtmlOleControl(string.Format("DefOcxName{0}", ++mHtmlOleControlCounter),
                HtmlOleControlType.Option, inputNode.SerializeToXml());
            Stream memoryStream = new MemoryStream(imageBytes);
            Builder.InsertHtmlOleControl(oleControl, memoryStream);
        }

        private byte[] GetRadioButtonImage(Document doc, bool isChecked)
        {
            if (isChecked)
            {
                if (mCheckedRadioButtonImage == null)
                    mCheckedRadioButtonImage = HtmlImageUtil.CreateRadioButtonImage(doc, true);
                return mCheckedRadioButtonImage;
            }
            else
            {
                if (mUncheckedRadioButtonImage == null)
                    mUncheckedRadioButtonImage = HtmlImageUtil.CreateRadioButtonImage(doc, false);
                return mUncheckedRadioButtonImage;
            }
        }

        /// <summary>
        /// Cached image for checked radio button.
        /// </summary>
        private byte[] mCheckedRadioButtonImage;
        /// <summary>
        /// Cached image for unchecked radio button.
        /// </summary>
        private byte[] mUncheckedRadioButtonImage;

        private readonly DocumentBuilder mBuilder;
        private readonly DocumentFormatter mDocumentFormatter;
        private int mHtmlOleControlCounter;
    }
}
