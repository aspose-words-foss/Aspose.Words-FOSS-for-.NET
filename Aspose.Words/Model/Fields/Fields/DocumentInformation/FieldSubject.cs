// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 08/11/2011 by Dmitry Vorobyev

using Aspose.Words.Fields.Expressions;
using Aspose.Words.Properties;

namespace Aspose.Words.Fields
{
    /// <summary>
    /// Implements the SUBJECT field.
    /// <para>To learn more, visit the <a href="https://docs.aspose.com/words/net/working-with-fields/">Working with Fields</a> documentation article.</para>
    /// </summary>
    /// <remarks>
    /// Retrieves, and optionally sets, the document's subject, as recorded in the <b>Subject</b> property of the
    /// built-in document properties.
    /// </remarks>
    public class FieldSubject : Field
    {
        internal override FieldUpdateAction UpdateCore()
        {
            return new FieldUpdateActionApplyResult(this, GetResultFieldSubject(FetchDocument(), FieldCodeCache));
        }

        private static string GetResultFieldSubject(Document document, IFieldCode fieldCode)
        {
            BuiltInDocumentProperties properties = document.BuiltInDocumentProperties;

            string text = fieldCode.GetArgumentAsString(TextArgumentIndex);
            if (text != null)
                properties.Subject = text;

            return properties.Subject;
        }

        /// <summary>
        /// Gets or sets the text of the subject.
        /// </summary>
        public string Text
        {
            get { return FieldCodeCache.GetArgumentAsString(TextArgumentIndex); }
            set { FieldCodeCache.SetArgument(TextArgumentIndex, value); }
        }

        [CodePorting.Translator.Cs2Cpp.CppConstexpr]
        private const int TextArgumentIndex = 0;

        internal static IFieldInfoResultProvider FieldInfoResultProvider = new SubjectInfoResultProvider();

        private class SubjectInfoResultProvider : IFieldInfoResultProvider
        {
            Constant IFieldInfoResultProvider.GetResult(Document document, IFieldCode fieldCode)
            {
                return new StringConstant(GetResultFieldSubject(document, fieldCode));
            }
        }
    }
}
