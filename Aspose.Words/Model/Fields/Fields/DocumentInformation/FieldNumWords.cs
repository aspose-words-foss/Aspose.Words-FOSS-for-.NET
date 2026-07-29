// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 09/11/2011 by Dmitry Vorobyev

using Aspose.Words.Fields.Expressions;

namespace Aspose.Words.Fields
{
    /// <summary>
    /// Implements the NUMWORDS field.
    /// <para>To learn more, visit the <a href="https://docs.aspose.com/words/net/working-with-fields/">Working with Fields</a> documentation article.</para>
    /// </summary>
    /// <remarks>
    /// Retrieves the number of words in the current document, as recorded in the <b>Words</b> property of the
    /// built-in document properties.
    /// </remarks>
    public class FieldNumWords : Field
    {
        internal override FieldUpdateAction UpdateCore()
        {
            Int32Constant value = GetResultInternal(FetchDocument());
            return new FieldUpdateActionApplyResult(this, value);
        }

        private static Int32Constant GetResultInternal(Document document)
        {
            return new Int32Constant(document.BuiltInDocumentProperties.Words);
        }

        internal static IFieldInfoResultProvider FieldInfoResultProvider = new NumWordsInfoResultProvider();

        private class NumWordsInfoResultProvider : IFieldInfoResultProvider
        {
            Constant IFieldInfoResultProvider.GetResult(Document document, IFieldCode fieldCode)
            {
                return GetResultInternal(document);
            }
        }
    }
}
