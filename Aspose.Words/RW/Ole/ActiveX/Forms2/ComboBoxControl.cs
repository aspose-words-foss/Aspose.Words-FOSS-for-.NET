// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 08/10/2015 by Alexey Morozov

using Aspose.Words.Drawing.Ole;

namespace Aspose.Words.Forms2
{
    /// <summary>
    /// The ComboBox control combines a <see cref="TextBoxControl"/> with a <see cref="ListBoxControl" />
    /// to create a drop-down list box.
    /// </summary>
    internal class ComboBoxControl : MorphDataControl
    {
        /// <summary>
        /// Ctor.
        /// </summary>
        internal ComboBoxControl(string name) : base(name)
        {
            DisplayStyle = DisplayStyle.Combo;
            Size = mDefaultSize;
        }

        /// <summary>
        /// Gets type of Forms 2.0 control.
        /// </summary>
        public override Forms2OleControlType Type
        {
            get { return Forms2OleControlType.ComboBox; }
        }

        /// <summary>
        /// Gets ClsidCacheIndex.
        /// </summary>
        internal override ClsidCacheIndex ClsidCacheIndex
        {
            get { return ClsidCacheIndex.ComboBox; }
        }

        /// <summary>
        /// Gets selected value of the ComboBoxControl.
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
            get { return ComboBoxControlClsid; }
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

        internal ShowDropButtonWhen ShowDropButtonWhen
        {
            get { return (ShowDropButtonWhen)Pr.FetchAttr(Forms2Attr.ShowDropButtonWhen); }
            set { Pr.SetAttr(Forms2Attr.ShowDropButtonWhen, value); }
        }

        internal DropButtonStyle DropButtonStyle
        {
            get { return (DropButtonStyle)Pr.FetchAttr(Forms2Attr.DropButtonStyle); }
            set { Pr.SetAttr(Forms2Attr.DropButtonStyle, value); }
        }

        private readonly OleSize mDefaultSize = OleSize.FromPoints(72, 18);
    }
}
