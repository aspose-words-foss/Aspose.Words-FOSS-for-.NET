// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 09/11/2011 by Dmitry Vorobyev

using Aspose.Words.Fields.Expressions;

namespace Aspose.Words.Fields
{
    /// <summary>
    /// Implements the LASTSAVEDBY field.
    /// <para>To learn more, visit the <a href="https://docs.aspose.com/words/net/working-with-fields/">Working with Fields</a> documentation article.</para>
    /// </summary>
    /// <remarks>
    /// Retrieves the name of the user who last modified and saved the current document, as recorded in the <b>LastModifiedBy</b>
    /// property of the built-in document properties.
    /// </remarks>
    public class FieldLastSavedBy : Field
    {
        internal override FieldUpdateAction UpdateCore()
        {
            return new FieldUpdateActionApplyResult(this, GetResultInternal(FetchDocument()));
        }

        private static string GetResultInternal(Document document)
        {
            return document.BuiltInDocumentProperties.LastSavedBy;
        }

        internal static IFieldInfoResultProvider FieldInfoResultProvider = new LastSavedByInfoResultProvider();

        private class LastSavedByInfoResultProvider : IFieldInfoResultProvider
        {
            Constant IFieldInfoResultProvider.GetResult(Document document, IFieldCode fieldCode)
            {
                return new StringConstant(GetResultInternal(document));
            }
        }
    }
}
