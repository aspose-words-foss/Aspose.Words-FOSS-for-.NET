// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 08/10/2015 by Alexey Morozov

using Aspose.Words.Drawing.Ole;

namespace Aspose.Words.Forms2
{
    /// <summary>
    /// The ListBox control displays a list of one or more items of text from which a user can choose.
    /// </summary>
    internal class ListBoxControl : MorphDataControl
    {
        /// <summary>
        /// Ctor.
        /// </summary>
        internal ListBoxControl(string name) : base(name)
        {
            DisplayStyle = DisplayStyle.List;
            Size = mDefaultSize;
        }

        /// <summary>
        /// Gets type of Forms 2.0 control.
        /// </summary>
        public override Forms2OleControlType Type
        {
            get { return Forms2OleControlType.ListBox; }
        }

        /// <summary>
        /// Gets ClsidCacheIndex.
        /// </summary>
        internal override ClsidCacheIndex ClsidCacheIndex
        {
            get { return ClsidCacheIndex.ListBox; }
        }

        /// <summary>
        /// Gets selected value of the ListBox.
        /// </summary>
        internal string SelectedValue
        {
            get { return (string)Pr.FetchAttr(Forms2Attr.Value); }
            set { Pr.SetAttr(Forms2Attr.Value, value); }
        }

        /// <summary>
        /// Gets a Clsid of the embedded object.
        /// </summary>
        protected override string ClsidVirtual
        {
            get { return ListBoxControlClsid; }
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

        internal ListStyle ListStyle
        {
            get { return (ListStyle)Pr.FetchAttr(Forms2Attr.ListStyle); }
            set { Pr.SetAttr(Forms2Attr.ListStyle, value); }
        }

        internal MultiSelect MultiSelect
        {
            get { return (MultiSelect)Pr.FetchAttr(Forms2Attr.MultiSelect); }
            set { Pr.SetAttr(Forms2Attr.MultiSelect, value); }
        }

        private readonly OleSize mDefaultSize = OleSize.FromPoints(72, 72);
    }
}
