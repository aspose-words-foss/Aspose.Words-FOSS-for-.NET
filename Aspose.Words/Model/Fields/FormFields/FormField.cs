// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 29/03/2004 by Roman Korchagin
using System;
using System.Diagnostics.CodeAnalysis;
using Aspose.Common;

namespace Aspose.Words.Fields
{
    /// <summary>
    /// Represents a single form field.
    /// <para>To learn more, visit the <a href="https://docs.aspose.com/words/net/working-with-form-fields/">Working with Form Fields</a> documentation article.</para>
    /// </summary>
    /// <remarks>
    /// <p>Microsoft Word provides the following form fields: checkbox, text input and dropdown (combobox).</p>
    ///
    /// <p><see cref="FormField"/> is an inline-node and can only be a child of <see cref="Paragraph"/>.</p>
    ///
    /// <p><see cref="FormField"/> is represented in a document by a special character and
    /// positioned as a character within a line of text.</p>
    ///
    /// <p>A complete form field in a Word document is a complex structure represented by several
    /// nodes: field start, field code such as FORMTEXT, form field data, field separator,
    /// field result, field end and a bookmark. To programmatically create form fields in a Word document use
    /// <see cref="DocumentBuilder.InsertCheckBox(string,bool,int)"/>,
    /// <see cref="DocumentBuilder.InsertTextInput"/> and
    /// <see cref="DocumentBuilder.InsertComboBox"/> which
    /// make sure all of the form field nodes are created in a correct order and in a suitable state.</p>
    /// </remarks>
    public class FormField : SpecialChar, IRunAttrSource
    {
        /// <summary>
        /// This is how the form field is represented in the model:
        /// FieldChar - start
        /// BookmarkStart
        /// Run - field code such as " FORMTEXT "
        /// FormField - contains ffdata
        /// FieldChar - separator
        /// Run - could be several, contain field value
        /// FieldChar - field end
        /// BookmarkEnd
        ///
        /// I also find this layout in some cases:
        /// BookmarkStart
        /// FieldChar - start
        /// Run - field code
        /// FormField
        /// FieldChar - separator
        /// Run - field value
        /// FieldChar - field end
        /// BookmarkEnd
        ///
        /// A text input form field that has a formular inside it (calculation) looks like this:
        /// FieldStart - form field
        /// Run - " FORMTEXT "
        ///     FieldStart - formula
        ///     Run - "=100+1000"
        ///     FieldSeparator - formula
        ///     Run - "1100"    - this is not formatted
        ///     FieldEnd - formula
        /// FormField - ffdata
        /// FieldChar - form field
        /// Run - "1,100" - formatted according to the form field format.
        /// FieldEnd - form field
        ///
        /// I could probably auto correct this and make it so bookmark always encompasses all field for example.
        /// </summary>
        internal FormField(DocumentBase doc, FormFieldPr formFieldPr, RunPr runPr) : base(doc, ControlChar.PictureChar, runPr)
        {
            FormFieldPr = formFieldPr;
        }

        /// <summary>
        /// Sets run attribute.
        /// </summary>
        /// <remarks>
        /// WORDSNET-522 We re-declare the <see cref="IRunAttrSource.SetRunAttr"/> method to provide the ability
        /// to set the same properties for the FormField, as well as for all field value Inlines.
        /// </remarks>
        void IRunAttrSource.SetRunAttr(int key, object value)
        {
            RunPr.SetAttr(key, value);

            if (Field == null)
                return;

            foreach (Node node in Field.GetFieldResultRange())
            {
                Debug.Assert(node != this);

                if (node == this)
                    continue;

                IRunAttrSource runAttrSource = node as IRunAttrSource;
                if (runAttrSource == null)
                    continue;

                runAttrSource.SetRunAttr(key, value);
            }
        }

        internal override Node Clone(bool isCloneChildren, INodeCloningListener cloningListener)
        {
            FormField lhs = (FormField)base.Clone(isCloneChildren, cloningListener);
            lhs.FormFieldPr = FormFieldPr.Clone();
            lhs.mFieldCache = null;
            return lhs;
        }

        /// <summary>
        /// Accepts a visitor.
        /// </summary>
        /// <remarks>
        /// <p>Calls <see cref="DocumentVisitor.VisitFormField"/>.</p>
        /// <p>For more info see the Visitor design pattern.</p>
        /// </remarks>
        /// <param name="visitor">The visitor that will visit the node.</param>
        /// <returns><c>false</c> if the visitor requested the enumeration to stop.</returns>
        public override bool Accept(DocumentVisitor visitor)
        {
            return VisitorActionToBool(visitor.VisitFormField(this));
        }

        /// <summary>
        /// Removes the complete form field, not just the form field special character.
        /// </summary>
        /// <remarks>
        /// If there is a bookmark associated with the form field, the bookmark is not removed.
        /// </remarks>
        public void RemoveField()
        {
            // If bookmark start is after the field start, then move the bookmark start to be before
            // the field start. This is to avoid deleting the bookmark start when deleting the field.
            if (Field == null)
                return;

            BookmarkStart bookmarkStart = Field.Start.NextSibling as BookmarkStart;
            if (bookmarkStart != null)
            {
                bookmarkStart.Remove();
                // because per OOXML spec one type of field (fldSimple) can be child of Sdt.
                Field.Start.FirstNonMarkupParentNode.InsertBefore(bookmarkStart, Field.Start);
            }

            Field.Remove();
        }

        /// <summary>
        /// Applies the text format specified in <see cref="TextInputFormat"/> and stores the value in <see cref="Result"/>.
        /// </summary>
        /// <param name="newValue">Can be a string, number or a <b>DateTime</b> object.</param>
        /// <remarks>The <see cref="TextInputDefault"/> value is applied if <paramref name="newValue"/> is <c>null</c>.</remarks>
        public void SetTextInputValue(object newValue)
        {
            if (Type != FieldType.FieldFormTextInput)
                throw new InvalidOperationException("The form field is not a text form field.");

            if (SetNullTextFormFieldResult(newValue))
                return;

            switch (TextInputType)
            {
                case TextFormFieldType.Regular:
                {
                    CharCase charCase = StringToCharCase(TextInputFormat);
                    Result = StringUtil.FormatCharCase((string)newValue, charCase);
                    break;
                }
                case TextFormFieldType.Number:
                {
                    FieldOptions fieldOptions = FetchDocument().FieldOptions;
                    NumberFormattingOptions formattingOptions =
                        fieldOptions.GetNumberFormattingOptions() |
                        NumberFormattingOptions.IgnoreUnmatchedDigitPlaceholder |
                        NumberFormattingOptions.IsMultiplyPercent;
                    Result = FormatterPal.NumberToStrMSWord(Convert.ToDouble(newValue), TextInputFormat, formattingOptions);
                    break;
                }
                case TextFormFieldType.Date:
                {
                    Result = WordUtil.FormatDateTime((DateTime)newValue, TextInputFormat);
                    break;
                }
                case TextFormFieldType.CurrentDate:
                case TextFormFieldType.CurrentTime:
                case TextFormFieldType.Calculated:
                    throw new InvalidOperationException("Cannot set the form field value because it is calculated.");
                default:
                    throw new InvalidOperationException(UnknownFormFieldType);
            }
        }

        private bool SetNullTextFormFieldResult(object newValue)
        {
            if (newValue != null)
                return false;

            switch (TextInputType)
            {
                case TextFormFieldType.Regular:
                case TextFormFieldType.Number:
                case TextFormFieldType.Date:
                    Result = null;
                    return true;
                case TextFormFieldType.CurrentDate:
                case TextFormFieldType.CurrentTime:
                case TextFormFieldType.Calculated:
                    return false;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private static CharCase StringToCharCase(string charCase)
        {
            charCase = charCase.ToLower();
            switch (charCase)
            {
                case "uppercase":
                    return CharCase.Upper;
                case "lowercase":
                    return CharCase.Lower;
                case "first capital":
                    return CharCase.FirstCap;
                case "title case":
                    return CharCase.Caps;
                default:
                    return CharCase.Default;
            }
        }

        /// <summary>
        /// Gets the start of a field that contains this form field node.
        /// </summary>
        private FieldStart GetFieldStart()
        {
            // The implementation of this method should be in sync with FieldStart.FormField implementation.

            int level = 0;
            DocumentPosition position = DocumentPosition.CreatePositionBefore(this);

            while (true)
            {
                Node node = position.Node;

                switch (node.NodeType)
                {
                    case NodeType.FieldStart:
                        if (level == 0)
                            return (FieldStart)node;

                        level--;
                        break;

                    case NodeType.FieldEnd:
                        level++;
                        break;

                    default:
                        // Do nothing.
                        break;
                }

                if (!position.Move(null, false, true, true, false, false))
                    return null;
            }
        }

        /// <summary>
        /// Copies the properties from the old field value <see cref="Inline"/> to the <see cref="FormField"/> and to the all field value Inlines.
        /// </summary>
        /// <remarks>
        /// WORDSNET-522 We need to copy the properties from the old field value <see cref="Inline"/> to the <see cref="FormField"/>
        /// and to the new field value Inlines, because the old field value Inlines was excluded from the node tree.
        /// </remarks>
        private void CopyPropertiesToFormField(Inline oldFieldValueInline)
        {
            if (oldFieldValueInline == null)
                return;

            RunPr = oldFieldValueInline.RunPr.Clone();

            if (Field == null)
                return;

            foreach (Node node in Field.GetFieldResultRange())
            {
                Debug.Assert(node != this);

                if (node == this)
                    continue;

                Inline inline = node as Inline;
                if (inline == null)
                    continue;

                inline.RunPr = oldFieldValueInline.RunPr.Clone();
            }
        }

        /// <summary>
        /// Returns <see cref="NodeType.FormField"/>.
        /// </summary>
        public override NodeType NodeType
        {
            get { return NodeType.FormField; }
        }

        /// <summary>
        /// Gets or sets the form field name.
        /// </summary>
        /// <remarks>
        /// Microsoft Word allows strings with at most 20 characters.
        /// </remarks>
        public string Name
        {
            get { return FormFieldPr.Name; }
            set
            {
                ArgumentUtil.CheckNotNull(value, "value");

                FormFieldPr.Name = value;

                BookmarkStart bmkStart = BookmarkStart;
                if (bmkStart != null)
                {
                    BookmarkEnd bmkEnd =
                        BookmarkFinder.FindBookmarkEnd(bmkStart.GetTopmostAncestor(), bmkStart.Name, bmkStart);
                    if (bmkEnd != null)
                        bmkStart.Bookmark.Name = FormFieldPr.Name;
                }
            }
        }

        /// <summary>
        /// Returns the form field type.
        /// </summary>
        [SuppressMessage("Microsoft.Naming", "CA1721:PropertyNamesShouldNotMatchGetMethods",
            Justification = "Public API, as designed.")]
        public FieldType Type
        {
            get
            {
                return (Field != null) ? Field.Type : FieldType.FieldNone;
            }
        }

        /// <summary>
        /// Gets the form field type that we use for writing into some formats.
        /// </summary>
        internal FormFieldType FormFieldType
        {
            get
            {
                switch (Type)
                {
                    case FieldType.FieldFormTextInput:
                        return FormFieldType.TextInput;
                    case FieldType.FieldFormCheckBox:
                        return FormFieldType.CheckBox;
                    case FieldType.FieldFormDropDown:
                        return FormFieldType.DropDown;
                    default:
                        throw new InvalidOperationException(UnknownFormFieldType);
                }
            }
        }

        /// <summary>
        /// Gets the field object that contains this form field.
        /// </summary>
        internal Field Field
        {
            get
            {
                if (mFieldCache == null)
                {
                    FieldStart fieldStart = GetFieldStart();
                    if (fieldStart != null)
                        mFieldCache = fieldStart.GetField();
                }

                return mFieldCache;
            }
        }

        /// <summary>
        /// Gets or sets a string that represents the result of this form field.
        /// </summary>
        /// <remarks>
        /// <p>For a text form field the result is the text that is in the field.</p>
        /// <p>For a checkbox form field the result can be "1" or "0" to indicate checked or unchecked.</p>
        /// <p>For a dropdown form field the result is the string selected in the dropdown.</p>
        ///
        /// <p>Setting <see cref="Result"/> for a text form field does not apply the text format
        /// specified in <see cref="TextInputFormat"/>. If you want to set a value and apply the
        /// format, use the <see cref="SetTextInputValue"/> method.</p>
        /// <p>For a text form field the <see cref="TextInputDefault"/> value is applied
        /// if <paramref name="value"/> is <c>null</c>.</p>
        /// </remarks>
        public string Result
        {
            get
            {
                // Must do different things depending on the form field type.
                switch (Type)
                {
                    case FieldType.FieldFormTextInput:
                    {
                        if (Field == null)
                            return "";

                        string result = Field.GetResult(true);
                        return (result == DefaultTextInputValue) ? "" : result;
                    }
                    case FieldType.FieldFormCheckBox:
                        return Checked ? "1" : "0";
                    case FieldType.FieldFormDropDown:
                        return DropDownValue;
                    default:
                        throw new InvalidOperationException(UnknownFormFieldType);
                }
            }
            set
            {
                switch (Type)
                {
                    case FieldType.FieldFormTextInput:
                    {
                        // WORDSNET-522 We need to remember the old field value first Inline, because it will be excluded
                        // from the node tree and the new field value Inlines will be inserted.
                        Inline oldFieldValueFirstInline = FieldValueFirstInline;

                        Field.Result = value ?? TextInputDefault;

                        // WORDSNET-522 We need to copy the properties from the old field value first Inline
                        // to the FormField and to the new field value Inlines.
                        if (oldFieldValueFirstInline != null)
                            CopyPropertiesToFormField(oldFieldValueFirstInline);

                        break;
                    }
                    case FieldType.FieldFormCheckBox:
                        ArgumentUtil.CheckNotNull(value, "value");

                        // RK It is okay to parse like this because we want to throw an exception to the user if he gives us some rubbish.
                        Checked = (FormatterPal.ParseInt(value) != 0);
                        break;
                    case FieldType.FieldFormDropDown:
                        ArgumentUtil.CheckNotNull(value, "value");

                        DropDownValue = value;
                        break;
                    default:
                        throw new InvalidOperationException(UnknownFormFieldType);
                }
            }
        }

        /// <summary>
        /// Returns or sets the text that's displayed in the status bar when a form field has the focus.
        /// </summary>
        /// <remarks>
        /// <p>If the <see cref="OwnStatus"/> property is set to <c>true</c>, the <see cref="StatusText"/> property specifies the status bar text.
        /// If the <see cref="OwnStatus"/> property is set to <c>false</c>, the <see cref="StatusText"/> property specifies the name of an AutoText
        /// entry that contains status bar text for the form field.</p>
        /// <p> Microsoft Word allows strings with at most 138 characters.</p>
        /// </remarks>
        public string StatusText
        {
            get { return FormFieldPr.StatusText; }
            set { FormFieldPr.StatusText = value; }
        }

        /// <summary>
        /// Specifies the source of the text that's displayed in the status bar when a form field has the focus.
        /// </summary>
        /// <remarks>
        /// <p>If <c>true</c>, the text specified by the <see cref="StatusText"/> property is displayed.
        /// If <c>false</c>, the text of the AutoText entry specified by the <see cref="StatusText"/> property is displayed.</p>
        /// </remarks>
        public bool OwnStatus
        {
            get { return FormFieldPr.OwnStatusText; }
            set { FormFieldPr.OwnStatusText = value; }
        }

        /// <summary>
        /// Returns or sets the text that's displayed in a message box when the form field has the focus and the user presses F1.
        /// </summary>
        /// <remarks>
        /// <p>If the <see cref="OwnHelp"/> property is set to <c>true</c>, <see cref="HelpText"/> specifies the text string value.
        /// If <see cref="OwnHelp"/> is set to <c>false</c>, <see cref="HelpText"/> specifies the name of an AutoText entry that contains help
        /// text for the form field.</p>
        /// <p>Microsoft Word allows strings with at most 255 characters.</p>
        /// </remarks>
        public string HelpText
        {
            get { return FormFieldPr.HelpText; }
            set { FormFieldPr.HelpText = value; }
        }

        /// <summary>
        /// Specifies the source of the text that's displayed in a message box when a form field has the focus and the user presses F1.
        /// </summary>
        /// <remarks>
        /// <p>If <c>true</c>, the text specified by the <see cref="HelpText"/> property is displayed.
        /// If <c>false</c>, the text in the AutoText entry specified by the <see cref="HelpText"/> property is displayed.</p>
        /// </remarks>
        public bool OwnHelp
        {
            get { return FormFieldPr.OwnHelpText; }
            set { FormFieldPr.OwnHelpText = value; }
        }

        /// <summary>
        /// True if references to the specified form field are automatically updated whenever the field is exited.
        /// </summary>
        /// <remarks>
        /// <p>Setting <see cref="CalculateOnExit"/> only affects the behavior of the form field when
        /// the document is opened in Microsoft Word. Aspose.Words never updates references
        /// to the form field.</p>
        /// </remarks>
        public bool CalculateOnExit
        {
            get { return FormFieldPr.CalcOnExit; }
            set { FormFieldPr.CalcOnExit = value; }
        }

        /// <summary>
        /// Returns or sets an entry macro name for the form field.
        /// </summary>
        /// <remarks>
        /// <p>The entry macro runs when the form field gets the focus in Microsoft Word.</p>
        /// <p>Microsoft Word allows strings with at most 32 characters.</p>
        /// </remarks>
        public string EntryMacro
        {
            get { return FormFieldPr.EntryMacro; }
            set { FormFieldPr.EntryMacro = value; }
        }

        /// <summary>
        /// Returns or sets an exit macro name for the form field.
        /// </summary>
        /// <remarks>
        /// <p>The exit macro runs when the form field loses the focus in Microsoft Word.</p>
        /// <p>Microsoft Word allows strings with at most 32 characters.</p>
        /// </remarks>
        public string ExitMacro
        {
            get { return FormFieldPr.ExitMacro; }
            set { FormFieldPr.ExitMacro = value; }
        }

        /// <summary>
        /// True if a form field is enabled.
        /// </summary>
        /// <remarks>
        /// <p>If a form field is enabled, its contents can be changed as the form is filled in.</p>
        /// </remarks>
        public bool Enabled
        {
            get { return FormFieldPr.Enabled; }
            set { FormFieldPr.Enabled = value; }
        }

        /// <summary>
        /// Returns or sets the text formatting for a text form field.
        /// </summary>
        /// <remarks>
        /// <p>If the text form field contains regular text, then valid format strings are
        /// "", "UPPERCASE", "LOWERCASE", "FIRST CAPITAL" and "TITLE CASE". The strings
        /// are case-insensitive.</p>
        /// <p>If the text form field contains a number or a date/time value, then valid
        /// format strings are number or date and time format strings.</p>
        /// <p>Microsoft Word allows strings with at most 64 characters.</p>
        /// </remarks>
        public string TextInputFormat
        {
            get { return FormFieldPr.TextInputFormat; }
            set { FormFieldPr.TextInputFormat = value; }
        }

        /// <summary>
        /// Gets or sets the type of a text form field.
        /// </summary>
        public TextFormFieldType TextInputType
        {
            get { return FormFieldPr.TextInputType; }
            set { FormFieldPr.TextInputType = value; }
        }

        /// <summary>
        /// Gets or sets the default string or a calculation expression of a text form field.
        /// </summary>
        /// <remarks>
        ///
        /// <para>The meaning of this property depends on the value of the <see cref="TextInputType"/> property.</para>
        ///
        /// <para>When <see cref="TextInputType"/> is <see cref="TextFormFieldType.Regular"/> or
        /// <see cref="TextFormFieldType.Number"/>, this string specifies the default string for the text form field.
        /// This string is the content that Microsoft Word will display in the document when the form field is empty.</para>
        ///
        /// <para>When <see cref="TextInputType"/> is <see cref="TextFormFieldType.Calculated"/>, then this string holds
        /// the expression to be calculated. The expression needs to be a formula valid according to Microsoft Word formula field
        /// requirements. When you set a new expression using this property, Aspose.Words calculates the formula result
        /// automatically and inserts it into the form field.</para>
        ///
        /// <para>Microsoft Word allows strings with at most 255 characters.</para>
        /// </remarks>
        public string TextInputDefault
        {
            get { return FormFieldPr.TextInputDefault; }
            set
            {
                FormFieldPr.TextInputDefault = value;

                if (TextInputType == TextFormFieldType.Calculated)
                {
                    // We are dealing with something like this and we need to remove and insert a completely new inner formula field.
                    //
                    // FieldStart - form field
                    // BookmarkStart - optional
                    // Run - " FORMTEXT "
                    //     FieldStart - formula
                    //     Run - "=100+1000"
                    //     FieldSeparator - formula
                    //     Run - "1100"    - this is not formatted
                    //     FieldEnd - formula
                    // FormField - ffdata (this)
                    // FieldChar - form field
                    // Run - "1,100" - formatted according to the form field format.
                    // FieldEnd - form field

                    // Jump to the form field code so we can have the range of the formula field and delete it.
                    Node startNode = Field.Start.NextSiblingOfType(NodeType.Run);
                    NodeRemover.Remove(startNode, false, this, false);

                    // Insert the new inner formula field, no need to update just yet.
                    DocumentBuilder builder = new DocumentBuilder(FetchDocument());
                    builder.MoveTo(this);
                    builder.InsertField(value, "");

                    // Update this field, it will update the inner formula first.
                    Field.Update();
                }
            }
        }

        /// <summary>
        /// Maximum length for the text field. Zero when the length is not limited.
        /// </summary>
        public int MaxLength
        {
            get { return FormFieldPr.TextInputMaxLength; }
            set { FormFieldPr.TextInputMaxLength = value; }
        }

        /// <summary>
        /// Provides access to the items of a dropdown form field.
        /// </summary>
        /// <remarks>
        /// <p>Microsoft Word allows maximum 25 items in a dropdown form field.</p>
        /// </remarks>
        public DropDownItemCollection DropDownItems
        {
            get { return FormFieldPr.DropDownItems; }
        }

        /// <summary>
        /// Gets or sets the index specifying the currently selected item in a dropdown form field.
        /// </summary>
        public int DropDownSelectedIndex
        {
            get
            {
                // RK This resolves to default value if needed.
                if (FormFieldPr.HasDropDownResult)
                    return FormFieldPr.DropDownResult;
                else
                    return FormFieldPr.DropDownDefault;
            }
            set
            {
                FormFieldPr.DropDownResult = value;
            }
        }

        /// <summary>
        /// Gets/sets string selected in the dropdown.
        /// If you specify a string that is not in the dropdown items, nothing happens.
        /// The comparison is case insensitive.
        /// </summary>
        internal string DropDownValue
        {
            get
            {
                int index = DropDownSelectedIndex;
                if ((index < 0) || (index >= DropDownItems.Count))
                    return "";

                return DropDownItems[index];
            }
            set
            {
                for (int i = 0; i < DropDownItems.Count; ++i)
                {
                    if (StringUtil.EqualsIgnoreCase(DropDownItems[i], value))
                    {
                        DropDownSelectedIndex = i;
                        return;
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets the checked status of the check box form field.
        /// Default value for this property is <c>false</c>.
        /// </summary>
        /// <remarks>
        /// <p>Applicable for a check box form field only.</p>
        /// </remarks>
        public bool Checked
        {
            get
            {
                // RK This resolves to default value if needed.
                if (FormFieldPr.HasCheckBoxChecked)
                    return FormFieldPr.CheckBoxChecked;
                else
                    return FormFieldPr.CheckBoxDefault;
            }
            set
            {
                FormFieldPr.CheckBoxChecked = value;
            }
        }

        /// <summary>
        /// Gets or sets the default value of the check box form field.
        /// Default value for this property is <c>false</c>.
        /// </summary>
        /// <remarks>
        /// <p>Applicable for a check box form field only.</p>
        /// </remarks>
        public bool Default
        {
            get
            {
                return FormFieldPr.CheckBoxDefault;
            }
            set
            {
                FormFieldPr.CheckBoxDefault = value;
            }
        }

        /// <summary>
        /// Gets or sets the boolean value that indicates whether the size of the textbox is automatic or specified explicitly.
        /// </summary>
        /// <remarks>
        /// <p>Applicable for a check box form field only.</p>
        /// <seealso cref="CheckBoxSize"/>
        /// </remarks>
        public bool IsCheckBoxExactSize
        {
            get { return !FormFieldPr.CheckBoxSizeAuto; }
            set { FormFieldPr.CheckBoxSizeAuto = !value; }
        }

        /// <summary>
        /// Gets or sets the size of the checkbox in points. Has effect only when <see cref="IsCheckBoxExactSize"/> is <c>true</c>.
        /// </summary>
        /// <remarks>
        /// <p>Applicable for a check box form field only.</p>
        /// <seealso cref="IsCheckBoxExactSize"/>
        /// </remarks>
        public double CheckBoxSize
        {
            get { return ConvertUtilCore.HalfPointToPoint(FormFieldPr.CheckBoxSizeHalfPoints); }
            set { FormFieldPr.CheckBoxSizeHalfPoints = ConvertUtilCore.PointToHalfPoint(value); }
        }

        /// <summary>
        /// Gets the bookmark start associated with this form field.
        /// </summary>
        internal BookmarkStart BookmarkStart
        {
            get
            {
                if (Field == null)
                    return null;

                // Normally bookmark start is immediately after field start.
                BookmarkStart bookmarkStart = Field.Start.NextSibling as BookmarkStart;
                if (bookmarkStart != null)
                    return bookmarkStart;

                // But sometimes it can be immediately before ...
                bookmarkStart = Field.Start.PreviousSibling as BookmarkStart;
                if (bookmarkStart != null)
                    return bookmarkStart;

                // ... or immediately before and above.
                return ResolveBookmarkStartBeforeParent(Field.Start);
            }
        }

        private Inline FieldValueFirstInline
        {
            get
            {
                if (Field == null)
                    return null;

                foreach (Node node in Field.GetFieldResultRange())
                {
                    Debug.Assert(node != this);

                    Inline inline = node as Inline;
                    if (inline != null)
                        return inline;
                }

                return null;
            }
        }

        private static BookmarkStart ResolveBookmarkStartBeforeParent(FieldStart fieldStart)
        {
            if (fieldStart.ParentNode == null)
                return null;

            if (!fieldStart.IsFirstChild)
                return null;

            return fieldStart.ParentNode.PreviousSibling as BookmarkStart;
        }

        /// <summary>
        /// Provides direct access to the form field properties (ffData).
        /// </summary>
        internal FormFieldPr FormFieldPr { get; private set; }

        private Field mFieldCache;

        /// <summary>
        /// When there is no default there will be maximum 5 circles displayed. Just mimic what MS Word is doing.
        /// </summary>
        internal static readonly string DefaultTextInputValue = new string(ControlChar.DefaultTextInputChar, 5);

        private const string UnknownFormFieldType = "Unknown form field type.";
    }
}
