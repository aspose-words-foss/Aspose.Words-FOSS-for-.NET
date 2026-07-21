// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 08/10/2015 by Alexey Morozov

using Aspose.Words.Forms2;
using Aspose.Words.RW.Ole;

namespace Aspose.Words.Drawing.Ole
{
    /// <summary>
    /// The OptionButton control enables a single choice in a limited set of mutually exclusive choices.
    /// </summary>
    public class OptionButtonControl : MorphDataControl
    {
        /// <summary>
        /// Ctor.
        /// </summary>
        internal OptionButtonControl(string name) : base(name)
        {
            DisplayStyle = DisplayStyle.OptionButton;
            Size = mDefaultSize;
        }

        /// <summary>
        /// Gets type of Forms 2.0 control.
        /// </summary>
        public override Forms2OleControlType Type
        {
            get { return Forms2OleControlType.OptionButton; }
        }

        /// <summary>
        /// Gets or sets a boolean value indicating either this <see cref="OptionButtonControl"/> is selected or not.
        /// </summary>
        /// <remarks>
        /// Note, this property allows you to select multiple items in a group of <see cref="OptionButtonControl"/>
        /// with the same <see cref="Forms2OleControl.GroupName"/>. It is up to you to take care of deselecting a previously
        /// selected item when you make this <see cref="OptionButtonControl"/> selected.
        /// </remarks>
        public bool Selected
        {
            get { return (OleUtil.ToNullableBool(Value) == NullableBool.True); }
            set { Value = OleUtil.ToString(value? NullableBool.True : NullableBool.False); }
        }

        /// <summary>
        /// Gets ClsidCacheIndex.
        /// </summary>
        internal override ClsidCacheIndex ClsidCacheIndex
        {
            get { return ClsidCacheIndex.OptionButton; }
        }

        /// <summary>
        /// Gets or sets a selected state of the control.
        /// </summary>
        internal NullableBool SelectedInternal
        {
            get { return OleUtil.ToNullableBool(Value); }
            set { Value = OleUtil.ToString(value); }
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
            get { return OptionButtonControlClsid; }
        }

        private readonly OleSize mDefaultSize = OleSize.FromPoints(108, 18);
    }
}
