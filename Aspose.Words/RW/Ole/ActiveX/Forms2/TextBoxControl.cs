// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 08/10/2015 by Alexey Morozov

using Aspose.Words.Forms2;

namespace Aspose.Words.Drawing.Ole
{
    /// <summary>
    /// The TextBox control displays text from an organized set of data or user input.
    /// </summary>
    public class TextBoxControl : MorphDataControl
    {
        /// <summary>
        /// Ctor.
        /// </summary>
        internal TextBoxControl(string name) : base(name)
        {
            Size = mDefaultSize;
        }

        /// <summary>
        /// Gets type of Forms 2.0 control.
        /// </summary>
        public override Forms2OleControlType Type
        {
            get { return Forms2OleControlType.Textbox; }
        }

        /// <summary>
        /// Gets or sets a text of the control.
        /// </summary>
        public string Text
        {
            get { return Value; }
            set { Value = value; }
        }

        /// <summary>
        /// Gets ClsidCacheIndex.
        /// </summary>
        internal override ClsidCacheIndex ClsidCacheIndex
        {
            get { return ClsidCacheIndex.TextBox; }
        }

        internal string FontName
        {
            get { return (string)Pr.FetchAttr(Forms2Attr.FontName); }
            set { Pr.SetAttr(Forms2Attr.FontName, value); }
        }

        internal uint FontHeight
        {
            get { return (uint)Pr.FetchAttr(Forms2Attr.FontHeight); }
            set { Pr.SetAttr(Forms2Attr.FontHeight, value); }
        }

        internal FontEffects FontEffects
        {
            get { return (FontEffects)Pr.FetchAttr(Forms2Attr.FontEffects); }
            set { Pr.SetAttr(Forms2Attr.FontEffects, value); }
        }

        internal ParagraphAlign ParagraphAlign
        {
            get { return (ParagraphAlign)Pr.FetchAttr(Forms2Attr.ParagraphAlign); }
            set { Pr.SetAttr(Forms2Attr.ParagraphAlign, value); }
        }

        /// <summary>
        /// Gets a Clsid of the embedded object.
        /// </summary>
        protected override string ClsidVirtual
        {
            get { return TextBoxControlClsid; }
        }

        private readonly OleSize mDefaultSize = OleSize.FromPoints(72, 18);
    }
}
