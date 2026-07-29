// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 30/10/2011 by Dmitry Vorobyev

namespace Aspose.Words.Fields
{
    /// <summary>
    /// Implements the PRINT field.
    /// <para>To learn more, visit the <a href="https://docs.aspose.com/words/net/working-with-fields/">Working with Fields</a> documentation article.</para>
    /// </summary>
    /// <remarks>
    /// An instruction to send the printer-specific control code characters to the selected printer
    /// when the document is printed.
    /// </remarks>
    public class FieldPrint : Field, IFieldCodeTokenInfoProvider
    {
        /// <summary>
        /// Gets or sets the printer-specific control code characters or PostScript instructions.
        /// </summary>
        public string PrinterInstructions
        {
            get { return FieldCodeCache.GetArgumentAsString(PrinterInstructionsArgumentIndex); }
            set { FieldCodeCache.SetArgument(PrinterInstructionsArgumentIndex, value); }
        }

        /// <summary>
        /// Gets or sets the drawing rectangle that the PostScript instructions operate on.
        /// </summary>
        public string PostScriptGroup
        {
            get { return FieldCodeCache.GetSwitchArgumentAsString(PostScriptGroupSwitch); }
            set { FieldCodeCache.SetSwitch(PostScriptGroupSwitch, value); }
        }

        FieldSwitchType IFieldCodeTokenInfoProvider.GetSwitchType(string switchName)
        {
            switch (switchName)
            {
                case PostScriptGroupSwitch:
                    return FieldSwitchType.HasArgument;
                default:
                    return FieldSwitchType.Unknown;
            }
        }

        [CodePorting.Translator.Cs2Cpp.CppConstexpr]
        private const int PrinterInstructionsArgumentIndex = 0;

        private const string PostScriptGroupSwitch = "\\p";
    }
}
