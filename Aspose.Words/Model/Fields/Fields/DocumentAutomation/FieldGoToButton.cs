// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 07/11/2011 by Dmitry Vorobyev

namespace Aspose.Words.Fields
{
    /// <summary>
    /// Implements the GOTOBUTTON field.
    /// <para>To learn more, visit the <a href="https://docs.aspose.com/words/net/working-with-fields/">Working with Fields</a> documentation article.</para>
    /// </summary>
    /// <remarks>
    /// Inserts a jump command, such that when it is activated, the insertion point of the document is
    /// moved to the specified location.
    /// </remarks>
    public class FieldGoToButton : Field
    {
        internal override NodeRange GetFakeResult()
        {
            FieldArgument text = FieldCodeCache.GetArgument(DisplayTextArgumentIndex);

            return FieldButtonFakeResultBuilder.BuildFieldButtonFakeResult(text);
        }

        /// <summary>
        /// Gets or sets the name of a bookmark, a page number, or some other item to jump to.
        /// </summary>
        public string Location
        {
            get { return FieldCodeCache.GetArgumentAsString(LocationArgumentIndex); }
            set { FieldCodeCache.SetArgument(LocationArgumentIndex, value);}
        }

        /// <summary>
        /// Gets or sets the text of the "button" that appears in the document, such that it can be selected to activate the jump.
        /// </summary>
        public string DisplayText
        {
            get { return FieldCodeCache.GetArgumentAsString(DisplayTextArgumentIndex); }
            set { FieldCodeCache.SetArgument(DisplayTextArgumentIndex, value); }
        }

        [CodePorting.Translator.Cs2Cpp.CppConstexpr]
        private const int LocationArgumentIndex = 0;
        [CodePorting.Translator.Cs2Cpp.CppConstexpr]
        private const int DisplayTextArgumentIndex = 1;
    }
}
