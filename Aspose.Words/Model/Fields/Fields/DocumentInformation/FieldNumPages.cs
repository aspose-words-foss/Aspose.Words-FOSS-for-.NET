// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 09/11/2011 by Dmitry Vorobyev

using Aspose.Words.Fields.Expressions;

namespace Aspose.Words.Fields
{
    /// <summary>
    /// Implements the NUMPAGES field.
    /// <para>To learn more, visit the <a href="https://docs.aspose.com/words/net/working-with-fields/">Working with Fields</a> documentation article.</para>
    /// </summary>
    /// <remarks>
    /// Retrieves the number of pages in the current document, as recorded in the <b>Pages</b> property of the
    /// built-in document properties.
    /// </remarks>
    public class FieldNumPages : Field
    {
        // Actually updated by the layout engine.

        internal static IFieldInfoResultProvider FieldInfoResultProvider = new NumPagesInfoResultProvider();

        private class NumPagesInfoResultProvider : IFieldInfoResultProvider
        {
            Constant IFieldInfoResultProvider.GetResult(Document document, IFieldCode fieldCode)
            {
                // FOSS
                return new Int32Constant(0);
            }
        }
    }
}
