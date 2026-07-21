// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 08/10/2015 by Alexey Morozov

using Aspose.Words.Forms2;
using Aspose.Words.RW.Ole;

namespace Aspose.Words.Drawing.Ole
{
    /// <summary>
    /// The CheckBox control toggles a value.
    /// </summary>
    /// <remarks>
    /// It has three possible states: selected, cleared, and neither selected nor cleared,
    /// meaning a combination of on and off states.
    /// </remarks>
    public class CheckBoxControl : MorphDataControl
    {
        /// <summary>
        /// Ctor.
        /// </summary>
        internal CheckBoxControl(string name) : base(name)
        {
            DisplayStyle = DisplayStyle.CheckBox;
            Size = mDefaultSize;
        }

        /// <summary>
        /// Ctor.
        /// </summary>
        internal CheckBoxControl(string name, string caption, NullableBool checkedState) : this(name)
        {
            Caption = caption;
            CheckedInternal = checkedState;
        }

        /// <summary>
        /// Gets type of Forms 2.0 control.
        /// </summary>
        public override Forms2OleControlType Type
        {
            get { return Forms2OleControlType.CheckBox; }
        }

        /// <summary>
        /// Gets or sets a boolean value indicating either this <see cref="CheckBoxControl"/> is checked or not.
        /// The default value is <c>false</c>.
        /// </summary>
        public bool Checked
        {
            get { return (OleUtil.ToNullableBool(Value) == NullableBool.True); }
            set { Value = OleUtil.ToString(value ? NullableBool.True : NullableBool.False); }
        }

        /// <summary>
        /// Gets ClsidCacheIndex.
        /// </summary>
        internal override ClsidCacheIndex ClsidCacheIndex
        {
            get { return ClsidCacheIndex.CheckBox; }
        }

        /// <summary>
        /// Gets or sets a state of the control.
        /// </summary>
        internal NullableBool CheckedInternal
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
            get { return CheckBoxControlClsid; }
        }

        private readonly OleSize mDefaultSize = OleSize.FromPoints(108, 18);
    }
}
