// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 08/10/2015 by Alexey Morozov

using System;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.IO;
using Aspose.Ss;
using Aspose.Words.Forms2;
using Aspose.Words.RW.Ole;
using Aspose.Words.RW.Ole.Ole2;

namespace Aspose.Words.Drawing.Ole
{
    /// <summary>
    /// Represents Microsoft Forms 2.0 OLE control.
    /// <para>To learn more, visit the <a href="https://docs.aspose.com/words/net/working-with-ole-objects/">Working with Ole Objects</a> documentation article.</para>
    /// </summary>
    public abstract class Forms2OleControl : OleControl
    {
        /// <summary>
        /// Creates Forms2OleControl object with a specified name.
        /// </summary>
        internal Forms2OleControl(string name, Forms2Pr pr = null) : base(name)
        {
            Pr = (pr != null) ? pr : new Forms2Pr();
        }

        /// <summary>
        /// Creates instance of the <see cref="Forms2OleControl"/> object of a specified type and name.
        /// </summary>
        internal static Forms2OleControl Create(Forms2OleControlType type, string name)
        {
            switch (type)
            {
                case Forms2OleControlType.OptionButton:
                    return new OptionButtonControl(name);
                case Forms2OleControlType.Label:
                    return new LabelControl(name);
                case Forms2OleControlType.Textbox:
                    return new TextBoxControl(name);
                case Forms2OleControlType.CheckBox:
                    return new CheckBoxControl(name);
                case Forms2OleControlType.ToggleButton:
                    return new ToggleButtonControl(name);
                case Forms2OleControlType.SpinButton:
                    return new SpinButtonControl(name);
                case Forms2OleControlType.ComboBox:
                    return new ComboBoxControl(name);
                case Forms2OleControlType.Frame:
                    return new FrameControl(name);
                case Forms2OleControlType.MultiPage:
                    return new MultiPageControl(name);
                case Forms2OleControlType.CommandButton:
                    return new CommandButtonControl(name);
                case Forms2OleControlType.Image:
                    return new ImageControl(name);
                case Forms2OleControlType.ScrollBar:
                    return new ScrollBarControl(name);
                case Forms2OleControlType.TabStrip:
                    return new TabStripControl(name);
                case Forms2OleControlType.ListBox:
                    return new ListBoxControl(name);
                case Forms2OleControlType.Form:
                    return new FormControl(name);
                default:
                    throw new InvalidOperationException(string.Format("Unexpected Forms2OleControl type: {0}", type));
            }
        }

        /// <summary>
        /// Reads the control from a storage.
        /// </summary>
        internal override void Read(MemoryStorage storage)
        {
            MemoryStream contentsStream = storage.GetStreamZeroPositioned("contents");
            // WORDSNET-18787 Check ActiveX control has content stream.
            if (contentsStream != null)
                Read(new BinaryReader(contentsStream));
        }

        /// <summary>
        /// Writes the control to a storage.
        /// </summary>
        internal override void Write(MemoryStorage storage)
        {
            MemoryStream stream = new MemoryStream();
            Write(new BinaryWriter(stream));
            storage["contents"] = stream;
        }

        /// <summary>
        /// Returns ProgId by a specified type.
        /// </summary>
        internal static string GetProgId(string clsid)
        {
            Forms2OleControlType type = ClsidToType(clsid);
            return GetProgId(type);
        }

        /// <summary>
        /// Returns ProgId by a specified type.
        /// </summary>
        private static string GetProgId(Forms2OleControlType type)
        {
            return string.Format("Forms.{0}.1", OleUtil.ToString(type));
        }

        /// <summary>
        /// Converts a control type to a Clsid.
        /// </summary>
        private static Forms2OleControlType ClsidToType(string clsid)
        {
            switch (clsid.ToLower())
            {
                case OptionButtonControlClsid: return Forms2OleControlType.OptionButton;
                case LabelControlClsid: return Forms2OleControlType.Label;
                case TextBoxControlClsid: return Forms2OleControlType.Textbox;
                case CheckBoxControlClsid: return Forms2OleControlType.CheckBox;
                case ToggleButtonControlClsid: return Forms2OleControlType.ToggleButton;
                case SpinButtonControlClsid: return Forms2OleControlType.SpinButton;
                case ComboBoxControlClsid: return Forms2OleControlType.ComboBox;
                case FrameControlClsid: return Forms2OleControlType.Frame;
                case MultiPageControlClsid: return Forms2OleControlType.MultiPage;
                case CommandButtonControlClsid: return Forms2OleControlType.CommandButton;
                case ImageControlClsid: return Forms2OleControlType.Image;
                case ScrollBarControlClsid: return Forms2OleControlType.ScrollBar;
                case TabStripControlClsid: return Forms2OleControlType.TabStrip;
                case ListBoxControlClsid: return Forms2OleControlType.ListBox;
                case FormControlClsid: return Forms2OleControlType.Form;
                default:
                    throw new InvalidOperationException(string.Format("Unexpected CLSID: {0}", clsid));
            }
        }

        /// <summary>
        /// Returns value of the <see cref="VariousPropertiesBits"/> field at a specified bit positions.
        /// </summary>
        private bool GetVariablePropertiesBitsField(VariousPropertiesBits bits)
        {
            return ((VariousPropertiesBits & bits) != 0);
        }

        /// <summary>
        /// Sets the <see cref="VariousPropertiesBits"/> field at a specified bit positions to a specified value.
        /// </summary>
        private void SetVariablePropertiesBitsField(VariousPropertiesBits bits, bool value)
        {
            if (value)
                VariousPropertiesBits |= bits;
            else
                VariousPropertiesBits &= ~bits;
        }

        /// <summary>
        /// Gets or sets a Caption property of the control.
        /// Default value is an empty string.
        /// </summary>
        public string Caption
        {
            get { return (string)Pr.FetchAttr(Forms2Attr.Caption); }
            set { Pr.SetAttr(Forms2Attr.Caption, value); }
        }

        /// <summary>
        /// Gets underlying Value property which often represents control state.
        /// For example checked option button has '1' value while unchecked has '0'.
        /// Default value is an empty string.
        /// </summary>
        public string Value
        {
            get { return (string)Pr.FetchAttr(Forms2Attr.Value); }
            internal set { Pr.SetAttr(Forms2Attr.Value, value); }
        }

        /// <summary>
        /// Returns <c>true</c> if control is in enabled state.
        /// </summary>
        public bool Enabled
        {
            get { return GetVariablePropertiesBitsField(VariousPropertiesBits.Enabled); }
            internal set { SetVariablePropertiesBitsField(VariousPropertiesBits.Enabled, value); }
        }

        internal bool WordWrap
        {
            get { return GetVariablePropertiesBitsField(VariousPropertiesBits.WordWrap); }
            set { SetVariablePropertiesBitsField(VariousPropertiesBits.WordWrap, value); }
        }

        internal bool Multiline
        {
            get { return GetVariablePropertiesBitsField(VariousPropertiesBits.MultiLine); }
            set { SetVariablePropertiesBitsField(VariousPropertiesBits.MultiLine, value); }
        }

        internal bool CaptionAlignmentLeft
        {
            get { return GetVariablePropertiesBitsField(VariousPropertiesBits.Alignment); }
            set { SetVariablePropertiesBitsField(VariousPropertiesBits.Alignment, value); }
        }

        internal bool SelectionMargin
        {
            get { return GetVariablePropertiesBitsField(VariousPropertiesBits.SelectionMargin); }
            set { SetVariablePropertiesBitsField(VariousPropertiesBits.SelectionMargin, value); }
        }

        /// <summary>
        /// Gets collection of immediate child controls.
        /// </summary>
        /// <remarks>Returns <c>null</c> if this control can not have children.</remarks>
        public virtual Forms2OleControlCollection ChildNodes
        {
            get { return null; }
        }

        /// <summary>
        /// Gets type of Forms 2.0 control.
        /// </summary>
        [SuppressMessage("Microsoft.Naming", "CA1721:PropertyNamesShouldNotMatchGetMethods",
            Justification = "Public API, as designed.")]
        public abstract Forms2OleControlType Type { get; }

        /// <summary>
        /// Gets or sets a string that specifies a group of mutually exclusive controls.
        /// The default value is an empty string.
        /// </summary>
        public string GroupName
        {
            get { return (string)Pr.FetchAttr(Forms2Attr.GroupName); }
            set
            {
                ArgumentUtil.CheckNotNull(value, "GroupName");
                Pr.SetAttr(Forms2Attr.GroupName, value);
            }
        }

        /// <summary>
        /// Gets ClsidCacheIndex.
        /// </summary>
        internal abstract ClsidCacheIndex ClsidCacheIndex { get; }

        /// <summary>
        /// Gets a boolean value indicating either the Forms2 OleControl is composite.
        /// </summary>
        internal virtual bool IsComposite
        {
            get { return false; }
        }

        /// <summary>
        /// Gets a clipboard format of the control.
        /// </summary>
        internal override ClipboardFormat ClipboardFormat
        {
            get { return gClipboardFormat; }
        }

        /// <summary>
        /// Gets or sets a MousePointer that specifies the type of icon displayed as the mouse pointer for the control.
        /// </summary>
        internal MousePointer MousePointer
        {
            get { return (MousePointer)Pr.FetchAttr(Forms2Attr.MousePointer); }
            set { Pr.SetAttr(Forms2Attr.MousePointer, value); }
        }

        /// <summary>
        /// Gets or sets bytes that specifies a custom icon to display as the mouse pointer for the control, which is used
        /// when the value of the <see cref="MousePointer"/> property is set to <see cref="Forms2.MousePointer.Custom"/>.
        /// </summary>
        internal byte[] MouseIcon
        {
            get { return (byte[])Pr.FetchAttr(Forms2Attr.MouseIcon); }
            set { Pr.SetAttr(Forms2Attr.MouseIcon, value); }
        }

        /// <summary>
        /// Gets or sets bytes that specifies the picture to display on a control.
        /// </summary>
        internal byte[] Picture
        {
            get { return (byte[])Pr.FetchAttr(Forms2Attr.Picture); }
            set { Pr.SetAttr(Forms2Attr.Picture, value); }
        }

        /// <summary>
        /// Gets a string that is associated with a control and that contains data entered by the user.
        /// </summary>
        internal string Tag
        {
            get { return (string)Pr.FetchAttr(Forms2Attr.Tag); }
        }

        /// <summary>
        /// Gets a string that specifies the tooltip for the control.
        /// </summary>
        internal string Tooltip
        {
            get { return (string)Pr.FetchAttr(Forms2Attr.Tooltips); }
        }

        /// <summary>
        /// Gets a string that specifies the license key of a control.
        /// </summary>
        internal string RuntimeLicKey
        {
            get { return (string)Pr.FetchAttr(Forms2Attr.RuntimeLicKey); }
        }

        /// <summary>
        /// Gets a string that specifies a cell in a worksheet that sets the Value property of a control when the
        /// control is loaded and to which the new value of the Value property is stored after it changes in the control.
        /// </summary>
        internal string ControlSource
        {
            get { return (string)Pr.FetchAttr(Forms2Attr.ControlSource); }
        }

        /// <summary>
        /// Gets a string that specifies the source for the list of values in a ComboBox or ListBox that is embedded
        /// in a form.
        /// </summary>
        internal string RowSource
        {
            get { return (string)Pr.FetchAttr(Forms2Attr.RowSource); }
        }

        /// <summary>
        /// Gets or sets a boolean value that specifies whether the control automatically resizes
        /// to display its entire contents.
        /// </summary>
        internal bool AutoSize
        {
            get { return (bool)Pr.FetchAttr(Forms2Attr.AutoSize); }
            set {Pr.SetAttr(Forms2Attr.AutoSize, value);}
        }

        /// <summary>
        /// Gets or sets a foreground color of the control.
        /// The default value depends on a type of the control.
        /// </summary>
        public Color ForeColor
        {
            get { return ForeColorInternal.Color; }
            set { ForeColorInternal = OleColor.FromColor(value); }
        }

        /// <summary>
        /// Gets or sets a background color of the control.
        /// The default value depends on a type of the control.
        /// </summary>
        public Color BackColor
        {
            get { return BackColorInternal.Color; }
            set { BackColorInternal = OleColor.FromColor(value); }
        }

        /// <summary>
        /// Gets or sets a background style for the control.
        /// </summary>
        internal BackStyle BackStyle
        {
            get
            {
                return (GetVariablePropertiesBitsField(VariousPropertiesBits.BackStyle))
                    ? BackStyle.Opaque
                    : BackStyle.Transparent;
            }
            set
            {
                SetVariablePropertiesBitsField(VariousPropertiesBits.BackStyle, (value == BackStyle.Opaque));
            }
        }

        /// <summary>
        /// Gets or sets a color of border of the control.
        /// The default value depends on a type of the control.
        /// </summary>
        internal Color BorderColor
        {
            get { return BorderColorInternal.Color; }
            set { BorderColorInternal = OleColor.FromColor(value); }
        }

        /// <summary>
        /// Gets or sets a type of border used by the control.
        /// </summary>
        internal BorderStyle BorderStyle
        {
            get { return (BorderStyle)Pr.FetchAttr(Forms2Attr.BorderStyle); }
            set { Pr.SetAttr(Forms2Attr.BorderStyle, value); }
        }

        internal SpecialEffect SpecialEffect
        {
            get { return (SpecialEffect)Pr.FetchAttr(Forms2Attr.SpecialEffect); }
            set { Pr.SetAttr(Forms2Attr.SpecialEffect, value); }
        }

        /// <summary>
        /// Gets or sets a size, in HIMETRIC units, of the control.
        /// </summary>
        internal OleSize Size
        {
            get { return (OleSize)Pr.FetchAttr(Forms2Attr.Size); }
            set { Pr.SetAttr(Forms2Attr.Size, value);}
        }

        /// <summary>
        /// Gets or sets a OlePosition that specifies the location of the top-left corner of an embedded control on a form,
        /// relative to the top-left corner of the LogicalSize of the form.
        /// The default value is (0, 0), which specifies that the top-left corner of the embedded control is at
        /// the top-left corner of the form.
        /// </summary>
        internal OlePosition SitePosition
        {
            get { return (OlePosition)Pr.FetchAttr(Forms2Attr.SitePosition); }
            set { Pr.SetAttr(Forms2Attr.SitePosition, value); }
        }


        /// <summary>
        /// Gets or sets a width of the control in points.
        /// </summary>
        /// <dev>
        /// Actually, Forms2OleControl keeps this value as integer, measured in HIMETRIC internally.
        /// See <see cref="OleSize"/> and <seealso cref="OlePosition"/> for details. So, the value of this property is
        /// converted from Points to HIMETRICs in setter and vise versa in getter. Note, such conversion can lead to mismatch
        /// in values of setter and getter due to rounding errors. Cannot completely understand rounding algorithm of
        /// Word in GUI and also cannot find it in VBA. So, for a moment, just round up it with 2 digits precision.
        /// </dev>
        public double Width
        {
            get { return System.Math.Round(ConvertUtilCore.HimetricToPoint(Size.Width), 2); }
            set { Size = new OleSize(ConvertUtilCore.PointToHimetricInt(value), Size.Height); }
        }

        /// <summary>
        /// Gets or sets a height of the control in points.
        /// </summary>
        /// <dev>
        /// See additional explanation in <see cref="Width"/> property.
        /// </dev>
        public double Height
        {
            get { return System.Math.Round(ConvertUtilCore.HimetricToPoint(Size.Height), 2); }
            set { Size = new OleSize(Size.Width, ConvertUtilCore.PointToHimetricInt(value)); }
        }

        /// <summary>
        /// Gets or sets a top-coordinate of a top-left corner of the embedded control on a form,
        /// relative to the top-left corner of the LogicalSize of the form.
        /// Measured in Points, with rounding up to 2 digits.
        /// The default value is 0.
        /// </summary>
        /// <dev>
        /// See additional explanation in <see cref="Width"/> property.
        /// </dev>
        internal double Top
        {
            get { return System.Math.Round(ConvertUtilCore.HimetricToPoint(SitePosition.Top), 2); }
            set { SitePosition = new OlePosition(ConvertUtilCore.PointToHimetricInt(value), SitePosition.Left); }
        }

        /// <summary>
        /// Gets or sets a left-coordinate of a top-left corner of the embedded control on a form,
        /// relative to the top-left corner of the LogicalSize of the form.
        /// Measured in Points, with rounding up to 2 digits.
        /// The default value is 0.
        /// </summary>
        /// <dev>
        /// See additional explanation in <see cref="Width"/> property.
        /// </dev>
        internal double Left
        {
            get { return System.Math.Round(ConvertUtilCore.HimetricToPoint(SitePosition.Left), 2); }
            set { SitePosition = new OlePosition(SitePosition.Top, ConvertUtilCore.PointToHimetricInt(value)); }
        }

        /// <summary>
        /// Gets or sets a unique identifier for an embedded control on a form.
        /// </summary>
        internal int IdInternal
        {
            get { return (int)Pr.FetchAttr(Forms2Attr.ID); }
            set { Pr.SetAttr(Forms2Attr.ID, value); }
        }

        /// <summary>
        /// Gets or sets the size, in bytes, of an embedded control that is persisted to the Object stream of a Form.
        /// </summary>
        internal uint ObjectStreamSize
        {
            get { return (uint)Pr.FetchAttr(Forms2Attr.ObjectStreamSize); }
            set { Pr.SetAttr(Forms2Attr.ObjectStreamSize, value); }
        }

        /// <summary>
        /// Get or set a Unicode character that specifies the accelerator key for the control.
        /// The default value is 0x0000, no accelerator.
        /// </summary>
        internal char Accelerator
        {
            get { return (char)Pr.FetchAttr(Forms2Attr.Accelerator); }
            set { Pr.SetAttr(Forms2Attr.Accelerator, value); }
        }

        /// <summary>
        /// Gets or sets a boolean value that specifies whether data in the control is locked for editing.
        /// The default value is <c>false</c>.
        /// </summary>
        internal bool Locked
        {
            get { return GetVariablePropertiesBitsField(VariousPropertiesBits.Locked); }
            set { SetVariablePropertiesBitsField(VariousPropertiesBits.Locked, value); }
        }

        /// <summary>
        /// Gets or sets a boolean value that specifies whether the control takes the focus when clicked.
        /// The default value is <c>true</c>.
        /// </summary>
        internal bool TakeFocusOnClick
        {
            get { return (bool)Pr.FetchAttr(Forms2Attr.TakeFocusOnClick); }
            set { Pr.SetAttr(Forms2Attr.TakeFocusOnClick, value); }
        }

        /// <summary>
        /// Gets a UserType of the control.
        /// </summary>
        protected override string UserType
        {
            get { return string.Format("Microsoft Forms 2.0 {0}", OleUtil.ToString(Type)); }
        }

        /// <summary>
        /// Gets a ProgId of the control.
        /// </summary>
        protected override string ProgId
        {
            get { return GetProgId(Type); }
        }

        /// <summary>
        /// Gets or sets a background color of the control.
        /// The default value depends on a type of the control.
        /// </summary>
        private OleColor BackColorInternal
        {
            get { return (OleColor)Pr.FetchAttr(Forms2Attr.BackgroundColor); }
            set { Pr.SetAttr(Forms2Attr.BackgroundColor, value); }
        }

        /// <summary>
        /// Gets or sets a foreground color of the control.
        /// The default value depends on a type of the control.
        /// </summary>
        private OleColor ForeColorInternal
        {
            get { return (OleColor)Pr.FetchAttr(Forms2Attr.ForegroundColor); }
            set { Pr.SetAttr(Forms2Attr.ForegroundColor, value); }
        }

        /// <summary>
        /// Gets or sets a color of border of the control.
        /// The default value depends on a type of the control.
        /// </summary>
        private OleColor BorderColorInternal
        {
            get { return (OleColor)Pr.FetchAttr(Forms2Attr.BorderColor); }
            set { Pr.SetAttr(Forms2Attr.BorderColor, value); }
        }

        /// <summary>
        /// Gets or sets a VariousPropertiesBits field.
        /// </summary>
        private VariousPropertiesBits VariousPropertiesBits
        {
            get { return (VariousPropertiesBits)Pr.FetchAttr(Forms2Attr.VariousPropertyBits); }
            set { Pr.SetAttr(Forms2Attr.VariousPropertyBits, value); }
        }

        /// <summary>
        /// The collection of properties of the control.
        /// </summary>
        internal readonly Forms2Pr Pr;

        /// <summary>
        /// StdPicture GUID as specified in [MS-OFORMS], 2.4.8 GuidAndPicture.
        /// </summary>
        internal static readonly Guid ClsidStdPicture = new Guid("0be35204-8f91-11ce-9de3-00aa004bb851");

        /// <summary>
        /// StdPicture preamble as specified in [MS-OFORMS], 2.4.13 StdPicture.
        /// </summary>
        internal const int StdPicturePreamble = 0x0000746C;

        /// <summary>
        /// The version of Forms2OleControls.
        /// </summary>
        internal const short Forms2Version = 0x0200;

        /// <summary>
        /// Specifies a ClipboardFormat of the Forms 2.0 controls.
        /// </summary>
        private static readonly ClipboardFormat gClipboardFormat = new ClipboardFormat("Embedded Object");
    }
}

