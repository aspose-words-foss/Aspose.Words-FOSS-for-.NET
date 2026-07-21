// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 30/10/2011 by Dmitry Vorobyev

namespace Aspose.Words.Fields
{
    /// <summary>
    /// Implements the FORMDROPDOWN field.
    /// <para>To learn more, visit the <a href="https://docs.aspose.com/words/net/working-with-fields/">Working with Fields</a> documentation article.</para>
    /// </summary>
    /// <remarks>
    /// Inserts a drop-down list style form field.
    /// </remarks>
    public class FieldFormDropDown : Field
    {
        internal override NodeRange GetFakeResult()
        {
            // MF There is no way to get layout options here.
            // The one defined at the document level could be overridden by layout (e.g. in tests)
            // So this code assumes that layout will not invoke it if IsDontWrapDropDownFormFields is True.

            // No form field - no result.
            FormField formField = Start.FormField;
            if (formField == null)
                return null;

            Document document = FetchDocument();
            IFieldRunPrProvider provider = new FieldFakeResultRunPrProvider(this);
            const bool isRtlEmbedding = false; // Be consistent with FieldFakeResultAppender.

            // FORMDROPDOWN items are always single-line.
            return FieldTextHelper.GetSingleLineTextRunRangeBidiAware(formField.Result, document, provider, isRtlEmbedding);
        }
    }
}
