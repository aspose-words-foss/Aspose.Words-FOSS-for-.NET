// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 10/16/2014 by Alexey Noskov

namespace Aspose.Words.Drawing.Core
{
    /// <summary>
    /// Represents the common properties of DrawingML and Shape that are required for rendering of their text box content.
    /// Interface is created for unification of text boxes layout and abstraction from underlying technology used by
    /// graphics object.
    /// </summary>
    internal interface ITextBox
    {
        TextBoxWrapMode TextBoxWrapMode_ITextBox { get; }
        LayoutFlow TextboxLayoutFlow_ITextBox { get; }
        bool HasVerticalTextFlow_ITextBox { get; }
        ShapeMarkupLanguage MarkupLanguage_ITextBox { get; }

        /// <summary>
        /// Gets the sum of horizontal margins.
        /// </summary>
        /// <remarks>
        /// It is used to calculate how much space is left for text box contents in a container of a certain size.
        /// </remarks>
        float GetHorizontalMargins_ITextBox();
    }
}
