// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 08/11/2011 by Dmitry Vorobyev

using Aspose.Words.Fields.Expressions;
using Aspose.Words.Properties;

namespace Aspose.Words.Fields
{
    /// <summary>
    /// Implements the AUTHOR field.
    /// <para>To learn more, visit the <a href="https://docs.aspose.com/words/net/working-with-fields/">Working with Fields</a> documentation article.</para>
    /// </summary>
    /// <remarks>
    /// Retrieves, and optionally sets, the document author's name, as recorded in the <b>Author</b> property of the
    /// built-in document properties.
    /// </remarks>
    public class FieldAuthor : Field
    {
        internal override FieldUpdateAction UpdateCore()
        {
            Document document = FetchDocument();

            string authorName = GetResultInternal(document, FieldCodeCache);

            return new FieldUpdateActionApplyResult(this, authorName);
        }

        private static string GetResultInternal(Document document, IFieldCode fieldCode)
        {
            string authorName = GetAuthorName(document, fieldCode);

            if (authorName != null)
                SaveAuthorNameInDocumentProperties(document, authorName);

            return authorName;
        }

        private static void SaveAuthorNameInDocumentProperties(Document document, string authorName)
        {
            BuiltInDocumentProperties properties = document.BuiltInDocumentProperties;
            properties.Author = authorName;
        }

        private static string GetAuthorName(Document document, IFieldCode fieldCode)
        {
            // We can get author's name from 3 source: fieldcode, built-in document properties and from field options.
            // below illustrates the preparation from these sources in order of priority
            string authorFromFieldCode = fieldCode.GetArgumentAsString(AuthorNameArgumentIndex);
            if (authorFromFieldCode != null)
                return authorFromFieldCode;

            BuiltInDocumentProperties properties = document.BuiltInDocumentProperties;
            if (StringUtil.HasChars(properties.Author))
                return properties.Author;

            return document.FieldOptions.DefaultDocumentAuthor;
        }

        /// <summary>
        /// Gets or sets the document author's name.
        /// </summary>
        public string AuthorName
        {
            get { return FieldCodeCache.GetArgumentAsString(AuthorNameArgumentIndex); }
            set { FieldCodeCache.SetArgument(AuthorNameArgumentIndex, value); }
        }

        [CodePorting.Translator.Cs2Cpp.CppConstexpr]
        private const int AuthorNameArgumentIndex = 0;

        internal static IFieldInfoResultProvider FieldInfoResultProvider = new AuthorInfoResultProvider();

        private class AuthorInfoResultProvider : IFieldInfoResultProvider
        {
            Constant IFieldInfoResultProvider.GetResult(Document document, IFieldCode fieldCode)
            {
                return new StringConstant(GetResultInternal(document, fieldCode));
            }
        }
    }
}
