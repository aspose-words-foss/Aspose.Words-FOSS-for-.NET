// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 27/01/2016 by Denis Darkin

using System.IO;
using Aspose.Common;
using Aspose.Words.Loading;
using Aspose.Words.Properties;
using CodePorting.Translator.Cs2Cpp;

namespace Aspose.Words
{
    /// <summary>
    /// Allows to extract plain-text representation of the document's content.
    /// <para>To learn more, visit the <a href="https://docs.aspose.com/words/net/working-with-text-document/">Working with Text Document</a> documentation article.</para>
    /// </summary>
    public class PlainTextDocument
    {
        /// <summary>
        /// Creates a plain text document from a file. Automatically detects the file format.
        /// </summary>
        /// <param name="fileName">Name of the file to extract the text from.</param>
        /// <include file='..\Docs\Text.xml' path='Topics/Topic[@name="Document.LoadFileExceptions"]/*'/>
        public PlainTextDocument(string fileName)
        {
            LoadOptions options = new LoadOptions(LoadFormat.Auto, "", "");
            options.LoadMode = LoadMode.RawText;

            using (Stream stream = SystemPal.OpenStreamFromHref(fileName))
                PopulateData(new Document(stream, options, true));
        }

        /// <summary>
        /// Creates a plain text document from a file. Allows to specify additional options such as an encryption password.
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <param name="fileName">Name of the file to extract the text from.</param>
        /// <param name="loadOptions">Additional options to use when loading a document. Can be <c>null</c>.</param>
        /// <include file='..\Docs\Text.xml' path='Topics/Topic[@name="Document.LoadFileExceptions"]/*'/>
        public PlainTextDocument(string fileName, LoadOptions loadOptions)
        {
            LoadOptions options = (loadOptions == null)
                ? new LoadOptions()
                : loadOptions.Clone();
            options.LoadMode = LoadMode.RawText;

            using (Stream stream = SystemPal.OpenStreamFromHref(fileName))
                PopulateData(new Document(stream, options, true));
        }

        /// <summary>
        /// Creates a plain text document from a stream. Automatically detects the file format.
        /// </summary>
        /// <include file='..\Docs\Text.xml' path='Topics/Topic[@name="Document.OpenStreamCommon"]/*'/>
        /// <param name="stream">The stream where to extract the text from.</param>
        /// <include file='..\Docs\Text.xml' path='Topics/Topic[@name="Document.LoadStreamExceptions"]/*'/>
        /// 
        /// <javaName>PlainTextDocument(java.io.InputStream stream)</javaName>
        // JAVA: the first public api change map will be used: Stream -> java.io.InputStream
        // WORDSJAVA-25686 - Loading from InputStream always load into memory first 
        public PlainTextDocument([CppIOStreamWrapper(IOStreamType.IStream)]Stream stream)
        {
            LoadOptions options = new LoadOptions(LoadFormat.Auto, "", "");
            options.LoadMode = LoadMode.RawText;
            PopulateData(new Document(stream, options, true));
        }

        /// <summary>
        /// Creates a plain text document from a stream. Allows to specify additional options such as an encryption password.
        /// </summary>
        /// <include file='..\Docs\Text.xml' path='Topics/Topic[@name="Document.OpenStreamCommon"]/*'/>
        /// <param name="stream">The stream where to extract the text from.</param>
        /// <param name="loadOptions">Additional options to use when loading a document. Can be <c>null</c>.</param>
        /// <include file='..\Docs\Text.xml' path='Topics/Topic[@name="Document.LoadStreamExceptions"]/*'/>
        /// 
        /// <javaName>PlainTextDocument(java.io.InputStream stream, com.aspose.words.LoadOptions loadOptions)</javaName>
        // JAVA: the first public api change map will be used: Stream -> java.io.InputStream
        // WORDSJAVA-25686 - Loading from InputStream always load into memory first 
        public PlainTextDocument([CppIOStreamWrapper(IOStreamType.IStream)]Stream stream, LoadOptions loadOptions)
        {
            LoadOptions options = (loadOptions == null)
                ? new LoadOptions()
                : loadOptions.Clone();
            options.LoadMode = LoadMode.RawText;
            PopulateData(new Document(stream, options, true));
        }

        private void PopulateData(Document container)
        {
            mText = container.GetText();
            mBuiltInDocumentProperties = container.BuiltInDocumentProperties;
            mCustomProperties = container.CustomDocumentProperties;
        }

        /// <summary>
        /// Gets textual content of the document concatenated as a string.
        /// </summary>
        public string Text
        {
            get { return mText; }
        }

        /// <summary>
        /// Gets <see cref="BuiltInDocumentProperties"/> of the document.
        /// </summary>
        public BuiltInDocumentProperties BuiltInDocumentProperties
        {
            get { return mBuiltInDocumentProperties; }
        }

        /// <summary>
        /// Gets <see cref="CustomDocumentProperties"/> of the document.
        /// </summary>
        public CustomDocumentProperties CustomDocumentProperties
        {
            get { return mCustomProperties; }
        }

        private string mText;
        private BuiltInDocumentProperties mBuiltInDocumentProperties;
        private CustomDocumentProperties mCustomProperties;
    }
}
