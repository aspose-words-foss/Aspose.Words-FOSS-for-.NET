// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 16/02/2005 by Roman Korchagin

namespace Aspose.Words.Fields
{
    /// <summary>
    /// Implements the MACROBUTTON field.
    /// <para>To learn more, visit the <a href="https://docs.aspose.com/words/net/working-with-fields/">Working with Fields</a> documentation article.</para>
    /// </summary>
    /// <remarks>
    /// <p>Allows a macro or command to be run.</p>
    /// <p>In Aspose.Words this field can also act as a merge field.</p>
    /// </remarks>
    public class FieldMacroButton : Field, IMergeFieldSurrogate
    {
        internal override NodeRange GetFakeResult()
        {
            FieldArgument text = FieldCodeCache.GetArgument(DisplayTextArgumentIndex);

            return FieldButtonFakeResultBuilder.BuildFieldButtonFakeResult(text);
        }

        string IMergeFieldSurrogate.GetMergeFieldName()
        {
            return DisplayText;
        }

        bool IMergeFieldSurrogate.CanWorkAsMergeField()
        {
            return false;
        }

        bool IMergeFieldSurrogate.IsMergeValueRequired()
        {
            return false;
        }

        /// <summary>
        /// Gets or sets the name of the macro or command to run.
        /// </summary>
        public string MacroName
        {
            get { return FieldCodeCache.GetArgumentAsString(MacroNameArgumentIndex); }
            set { FieldCodeCache.SetArgument(MacroNameArgumentIndex, value); }
        }

        /// <summary>
        /// Gets or sets the text to appear as the "button" that is selected to run the macro or command.
        /// </summary>
        public string DisplayText
        {
            get { return FieldCodeCache.GetArgumentAsString(DisplayTextArgumentIndex); }
            set { FieldCodeCache.SetArgument(DisplayTextArgumentIndex, value);}
        }

        [CodePorting.Translator.Cs2Cpp.CppConstexpr]
        private const int MacroNameArgumentIndex = 0;

        [CodePorting.Translator.Cs2Cpp.CppConstexpr]
        private const int DisplayTextArgumentIndex = 1;
    }
}
