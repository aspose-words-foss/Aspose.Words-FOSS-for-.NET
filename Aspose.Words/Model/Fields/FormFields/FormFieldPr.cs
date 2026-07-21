// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 10/02/2008 by Roman Korchagin

namespace Aspose.Words.Fields
{
    /// <summary>
    /// Form field properties (ffData).
    /// Handles all form field types.
    /// </summary>
    internal class FormFieldPr : AttrCollection
    {
        protected override AttrCollection GetDefaults()
        {
            return gDefaults;
        }

        internal string Name
        {
            get { return (string)FetchAttr(FormFieldAttr.Name); }
            set
            {
                ArgumentUtil.CheckNotNull(value, "value");
                SetAttr(FormFieldAttr.Name, Truncate(value, MaxNameLength));
            }
        }

        internal bool Enabled
        {
            get { return (bool)FetchAttr(FormFieldAttr.Enabled); }
            set { SetAttr(FormFieldAttr.Enabled, value); }
        }

        internal bool CalcOnExit
        {
            get { return (bool)FetchAttr(FormFieldAttr.CalcOnExit); }
            set { SetAttr(FormFieldAttr.CalcOnExit, value); }
        }

        internal string EntryMacro
        {
            get { return (string)FetchAttr(FormFieldAttr.EntryMacro); }
            set
            {
                ArgumentUtil.CheckNotNull(value, "value");
                SetAttr(FormFieldAttr.EntryMacro, Truncate(value, MaxMacroNameLength));
            }
        }

        internal string ExitMacro
        {
            get { return (string)FetchAttr(FormFieldAttr.ExitMacro); }
            set
            {
                ArgumentUtil.CheckNotNull(value, "value");
                SetAttr(FormFieldAttr.ExitMacro, Truncate(value, MaxMacroNameLength));
            }
        }

        internal bool OwnHelpText
        {
            get { return (bool)FetchAttr(FormFieldAttr.OwnHelpText); }
            set { SetAttr(FormFieldAttr.OwnHelpText, value); }
        }

        internal string HelpText
        {
            get { return (string)FetchAttr(FormFieldAttr.HelpText); }
            set
            {
                ArgumentUtil.CheckNotNull(value, "value");
                SetAttr(FormFieldAttr.HelpText, Truncate(value, MaxHelpTextLength));
            }
        }

        internal bool OwnStatusText
        {
            get { return (bool)FetchAttr(FormFieldAttr.OwnStatusText); }
            set { SetAttr(FormFieldAttr.OwnStatusText, value); }
        }

        internal string StatusText
        {
            get { return (string)FetchAttr(FormFieldAttr.StatusText); }
            set
            {
                ArgumentUtil.CheckNotNull(value, "value");
                SetAttr(FormFieldAttr.StatusText, Truncate(value, MaxStatusTextLength));
            }
        }

        internal int TextInputMaxLength
        {
            get { return (int)FetchAttr(FormFieldAttr.TextInputMaxLength); }
            set { SetAttr(FormFieldAttr.TextInputMaxLength, value); }
        }

        internal TextFormFieldType TextInputType
        {
            get { return (TextFormFieldType)FetchAttr(FormFieldAttr.TextInputType); }
            set { SetAttr(FormFieldAttr.TextInputType, value); }
        }

        internal string TextInputDefault
        {
            get { return (string)FetchAttr(FormFieldAttr.TextInputDefault); }
            set
            {
                ArgumentUtil.CheckNotNull(value, "value");
                SetAttr(FormFieldAttr.TextInputDefault, Truncate(value, MaxTextInputDefaultLength));
            }
        }

        internal string TextInputFormat
        {
            get { return (string)FetchAttr(FormFieldAttr.TextInputFormat); }
            set
            {
                ArgumentUtil.CheckNotNull(value, "value");
                SetAttr(FormFieldAttr.TextInputFormat, Truncate(value, MaxTextInputFormatLength));
            }
        }

        internal bool CheckBoxSizeAuto
        {
            get { return (bool)FetchAttr(FormFieldAttr.CheckBoxSizeAuto); }
            set { SetAttr(FormFieldAttr.CheckBoxSizeAuto, value); }
        }

        internal int CheckBoxSizeHalfPoints
        {
            get { return (int)FetchAttr(FormFieldAttr.CheckBoxSizeHalfPoints); }
            set { SetAttr(FormFieldAttr.CheckBoxSizeHalfPoints, value); }
        }

        internal bool CheckBoxDefault
        {
            get { return (bool)FetchAttr(FormFieldAttr.CheckBoxDefault); }
            set { SetAttr(FormFieldAttr.CheckBoxDefault, value); }
        }

        /// <summary>
        /// Gets the explicit checked value.
        /// </summary>
        internal bool CheckBoxChecked
        {
            get { return (bool)FetchAttr(FormFieldAttr.CheckBoxChecked); }
            set { SetAttr(FormFieldAttr.CheckBoxChecked, value); }
        }

        internal bool HasCheckBoxChecked
        {
            get { return Contains(FormFieldAttr.CheckBoxChecked); }
        }

        internal int DropDownDefault
        {
            get { return (int)FetchAttr(FormFieldAttr.DropDownDefault); }
            set { SetAttr(FormFieldAttr.DropDownDefault, value); }
        }

        internal int DropDownResult
        {
            get { return (int)FetchAttr(FormFieldAttr.DropDownResult); }
            set { SetAttr(FormFieldAttr.DropDownResult, value); }
        }

        internal bool HasDropDownResult
        {
            get { return Contains(FormFieldAttr.DropDownResult); }
        }

        /// <summary>
        /// Gets a collection of drop down items.
        /// Automatically creates an empty collection if it does not exist.
        /// </summary>
        internal DropDownItemCollection DropDownItems
        {
            get
            {
                DropDownItemCollection items = (DropDownItemCollection)FetchAttr(FormFieldAttr.DropDownItems);
                if (items == null)
                {
                    items = new DropDownItemCollection();
                    SetAttr(FormFieldAttr.DropDownItems, items);
                }

                return items;
            }
        }

        /// <summary>
        /// RK Word earlier than 2003 crashes if we specify a string longer than allowed so lets silently truncate.
        /// </summary>
        private static string Truncate(string value, int maxLength)
        {
            if (value.Length > maxLength)
                return value.Substring(0, maxLength);
            else
                return value;
        }

        private static readonly FormFieldPr gDefaults = BuildDefaults();

        private static FormFieldPr BuildDefaults()
        {
            FormFieldPr defaults = new FormFieldPr();

            defaults.Add(FormFieldAttr.CalcOnExit, false);
            defaults.Add(FormFieldAttr.CheckBoxChecked, false);
            defaults.Add(FormFieldAttr.CheckBoxDefault, false);
            defaults.Add(FormFieldAttr.CheckBoxSizeHalfPoints, 20);
            defaults.Add(FormFieldAttr.CheckBoxSizeAuto, true);
            defaults.Add(FormFieldAttr.DropDownDefault, 0);
            defaults.Add(FormFieldAttr.DropDownItems, null);
            defaults.Add(FormFieldAttr.DropDownResult, 0);
            defaults.Add(FormFieldAttr.Enabled, true);
            defaults.Add(FormFieldAttr.EntryMacro, "");
            defaults.Add(FormFieldAttr.ExitMacro, "");
            defaults.Add(FormFieldAttr.HelpText, "");
            defaults.Add(FormFieldAttr.OwnHelpText, false);
            defaults.Add(FormFieldAttr.Name, "");
            defaults.Add(FormFieldAttr.StatusText, "");
            defaults.Add(FormFieldAttr.OwnStatusText, false);
            defaults.Add(FormFieldAttr.TextInputDefault, "");
            defaults.Add(FormFieldAttr.TextInputFormat, "");
            defaults.Add(FormFieldAttr.TextInputMaxLength, 0);
            defaults.Add(FormFieldAttr.TextInputType, TextFormFieldType.Regular);

            return defaults;
        }

        [CodePorting.Translator.Cs2Cpp.CppConstexpr]
        internal const int MaxNameLength = 20;
        [CodePorting.Translator.Cs2Cpp.CppConstexpr]
        internal const int MaxTextInputDefaultLength = 255;
        [CodePorting.Translator.Cs2Cpp.CppConstexpr]
        internal const int MaxTextInputFormatLength = 64;
        [CodePorting.Translator.Cs2Cpp.CppConstexpr]
        internal const int MaxStatusTextLength = 138;
        [CodePorting.Translator.Cs2Cpp.CppConstexpr]
        internal const int MaxHelpTextLength = 255;
        [CodePorting.Translator.Cs2Cpp.CppConstexpr]
        internal const int MaxMacroNameLength = 32;
    }
}
