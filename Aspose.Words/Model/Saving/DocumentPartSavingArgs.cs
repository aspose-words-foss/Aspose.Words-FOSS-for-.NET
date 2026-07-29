// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 31/07/2010 by Viktor Sazhaev
using System;
using System.IO;
using Aspose.JavaAttributes;
using CodePorting.Translator.Cs2Cpp;

namespace Aspose.Words.Saving
{
    /// <summary>
    /// Provides data for the <see cref="IDocumentPartSavingCallback.DocumentPartSaving"/> callback.
    /// <para>To learn more, visit the <a href="https://docs.aspose.com/words/net/save-a-document/">Save a Document</a> documentation article.</para>
    /// </summary>
    /// <remarks>
    /// <p>When Aspose.Words saves a document to HTML or related formats and <see cref="HtmlSaveOptions.DocumentSplitCriteria"/> 
    /// is specified, the document is split into parts and by default, each document part is saved into a separate file.</p>
    /// 
    /// <p>Class <see cref="DocumentPartSavingArgs"/> allows you to control how each document part will be saved. 
    /// It allows to redefine how file names are generated or to completely circumvent saving of document parts into 
    /// files by providing your own stream objects.</p>
    /// 
    /// <p>To save document parts into streams instead of files, use the <see cref="DocumentPartStream"/> property.</p>
    /// </remarks>
    public class DocumentPartSavingArgs
    {
        /// <summary>
        /// Ctor. Should be internal. Client code won't create this object.
        /// </summary>
        internal DocumentPartSavingArgs(Document document, string fileName)
        {
            mDocument = document;
            mDocumentPartFileName = fileName;
        }

        /// <summary>
        /// Gets the document object that is being saved.
        /// </summary>
        public Document Document
        {
            get { return mDocument; }
        }

        /// <summary>
        /// Gets or sets the file name (without path) where the document part will be saved to.
        /// </summary>
        /// <remarks>
        /// <p>This property allows you to redefine how the document part file names are generated
        /// during export to HTML or EPUB.</p>
        /// 
        /// <p>When the callback is invoked, this property contains the file name that was generated 
        /// by Aspose.Words. You can change the value of this property to save the document part into a 
        /// different file. Note that the file name for each part must be unique.</p>
        /// 
        /// <p><see cref="DocumentPartFileName"/> must contain only the file name without the path. 
        /// Aspose.Words determines the path for saving using the document file name. If output document 
        /// file name was not specified, for instance when saving to a stream, this file name is used only 
        /// for referencing document parts. The same is true when saving to EPUB format.</p>
        /// 
        /// <seealso cref="DocumentPartStream"/>
        /// </remarks>
        public string DocumentPartFileName
        {
            get { return mDocumentPartFileName; }
            set
            {
                ArgumentUtil.CheckHasChars(value, "DocumentPartFileName");
                if (Path.GetFileName(value) != value)
                    throw new ArgumentException("DocumentPartFileName must be a file name without path.");
                mDocumentPartFileName = value;
            }
        }

        /// <summary>
        /// Specifies whether Aspose.Words should keep the stream open or close it after saving a document part.
        /// </summary>
        /// <remarks>
        /// <p>Default is <c>false</c> and Aspose.Words will close the stream you provided
        /// in the <see cref="DocumentPartStream"/> property after writing a document part into it.
        /// Specify <c>true</c> to keep the stream open. Please note that the main output stream 
        /// provided in the call to <see cref="Aspose.Words.Document.Save(Stream,SaveFormat)"/> or 
        /// <see cref="Aspose.Words.Document.Save(Stream,SaveOptions)"/> will never be closed by Aspose.Words 
        /// even if <see cref="KeepDocumentPartStreamOpen"/> is set to <c>false</c>.</p>
        /// 
        /// <seealso cref="DocumentPartStream"/>
        /// </remarks>
        public bool KeepDocumentPartStreamOpen
        {
            get { return mKeepDocumentPartStreamOpen; }
            set { mKeepDocumentPartStreamOpen = value; }
        }

        /// <summary>
        /// Allows to specify the stream where the document part will be saved to.
        /// </summary>
        /// <remarks>
        /// <p>This property allows you to save document parts to streams instead of files during HTML export.</p>
        /// 
        /// <p>The default value is <c>null</c>. When this property is <c>null</c>, the document part 
        /// will be saved to a file specified in the <see cref="DocumentPartFileName"/> property.</p>
        /// 
        /// <p>When saving to a stream in HTML format is requested by <see cref="Aspose.Words.Document.Save(Stream,SaveFormat)"/> 
        /// or <see cref="Aspose.Words.Document.Save(Stream,SaveOptions)"/> and first document part is about to be saved, 
        /// Aspose.Words suggests here the main output stream initially passed by the caller.</p>
        /// 
        /// <p>When saving to EPUB format that is a container format based on HTML, <see cref="DocumentPartStream"/> cannot 
        /// be specified because all subsidiary parts will be encapsulated into a single output package.</p>
        /// 
        /// <seealso cref="KeepDocumentPartStreamOpen"/>
        /// </remarks>
        /// <javaName>DocumentPartStream(java.io.OutputStream)</javaName>
        [JavaUseSecondApiChangeMap]
        [CppIOStreamWrapper(IOStreamType.OStream)]
        public Stream DocumentPartStream
        {
            get { return mDocumentPartStream; }
            set { mDocumentPartStream = value; }
        }
        private Stream mDocumentPartStream;

        /// <summary>
        /// Exists to make calling code autoportable.
        /// </summary>
        internal bool HasUserStream
        {
            get { return mDocumentPartStream != null; }
        }

        /// <summary>
        /// Exists to make calling code autoportable.
        /// </summary>
        internal UserStreamWrapper CreateUserStreamWrapper()
        {
            return new UserStreamWrapper(mDocumentPartStream, mKeepDocumentPartStreamOpen);
        }

        private readonly Document mDocument;
        private string mDocumentPartFileName;
        private bool mKeepDocumentPartStreamOpen;
    }
}
