// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 30/08/2011 by Dmitry Vorobyev

using Aspose.Words.Fields.Expressions;

namespace Aspose.Words.Fields
{
    /// <summary>
    /// Implements the EDITTIME field.
    /// <para>To learn more, visit the <a href="https://docs.aspose.com/words/net/working-with-fields/">Working with Fields</a> documentation article.</para>
    /// </summary>
    /// <remarks>
    /// Retrieves the total editing time, in minutes, since the document was created.
    /// </remarks>
    public class FieldEditTime : Field
    {
        internal override FieldUpdateAction UpdateCore()
        {
            Document document = FetchDocument();
            Int32Constant value = GetResultInternal(document);
            return new FieldUpdateActionApplyResult(this, value);
        }

        private static Int32Constant GetResultInternal(Document document)
        {
            return new Int32Constant(document.BuiltInDocumentProperties.TotalEditingTime);
        }

        internal static IFieldInfoResultProvider FieldInfoResultProvider = new EditTimeInfoResultProvider();

        private class EditTimeInfoResultProvider : IFieldInfoResultProvider
        {
            Constant IFieldInfoResultProvider.GetResult(Document document, IFieldCode fieldCode)
            {
                return GetResultInternal(document);
            }
        }
    }
}
