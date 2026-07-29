// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 08/10/2015 by Alexey Morozov

using Aspose.Words.Drawing.Ole;
using Aspose.Words.RW.Ole;

namespace Aspose.Words.Forms2
{
    /// <summary>
    /// The ToggleButton control indicates a state, such as Yes/No, or a mode, such as On/Off.
    /// </summary>
    internal class ToggleButtonControl : MorphDataControl
    {
        /// <summary>
        /// Ctor.
        /// </summary>
        internal ToggleButtonControl(string name) : base(name)
        {
            DisplayStyle = DisplayStyle.Toggle;
            Size = mDefaultSize;
        }

        /// <summary>
        /// Gets type of Forms 2.0 control.
        /// </summary>
        public override Forms2OleControlType Type
        {
            get { return Forms2OleControlType.ToggleButton; }
        }

        /// <summary>
        /// Gets ClsidCacheIndex.
        /// </summary>
        internal override ClsidCacheIndex ClsidCacheIndex
        {
            get { return ClsidCacheIndex.ToggleButton; }
        }

        /// <summary>
        /// Gets or sets a state of the control.
        /// </summary>
        internal NullableBool Checked
        {
            get { return OleUtil.ToNullableBool(Value); }
            set { Value = OleUtil.ToString(value); }
        }

        /// <summary>
        /// Gets a Clsid of the embedded object.
        /// </summary>
        protected override string ClsidVirtual
        {
            get { return ToggleButtonControlClsid; }
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

        private readonly OleSize mDefaultSize = OleSize.FromPoints(36, 40);
    }
}
