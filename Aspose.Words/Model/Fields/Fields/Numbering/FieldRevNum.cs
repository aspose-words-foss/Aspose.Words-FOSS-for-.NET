// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 30/10/2011 by Dmitry Vorobyev

using Aspose.Words.Fields.Expressions;

namespace Aspose.Words.Fields
{
    /// <summary>
    /// Implements the REVNUM field.
    /// <para>To learn more, visit the <a href="https://docs.aspose.com/words/net/working-with-fields/">Working with Fields</a> documentation article.</para>
    /// </summary>
    /// <remarks>
    /// Retrieves the document's revision number, as recorded in the <b>Revision</b> property of the
    /// built-in document properties.
    /// </remarks>
    public class FieldRevNum : Field
    {
        internal override FieldUpdateAction UpdateCore()
        {
            Int32Constant value = GetResultInternal(FetchDocument());
            return new FieldUpdateActionApplyResult(this, value);
        }

        private static Int32Constant GetResultInternal(Document document)
        {
            return new Int32Constant(document.BuiltInDocumentProperties.RevisionNumber);
        }

        internal static IFieldInfoResultProvider FieldInfoResultProvider = new RevNumInfoResultProvider();

        private class RevNumInfoResultProvider : IFieldInfoResultProvider
        {
            Constant IFieldInfoResultProvider.GetResult(Document document, IFieldCode fieldCode)
            {
                return GetResultInternal(document);
            }
        }
    }
}
