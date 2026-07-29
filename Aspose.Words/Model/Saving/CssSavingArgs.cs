// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 12/09/2012 by Alexey Butalov

using System.IO;
using CodePorting.Translator.Cs2Cpp;

namespace Aspose.Words.Saving
{
    /// <summary>
    /// Provides data for the <see cref="ICssSavingCallback.CssSaving"/> event.
    /// <para>To learn more, visit the <a href="https://docs.aspose.com/words/net/save-a-document/">Save a Document</a> documentation article.</para>
    /// </summary>
    /// <remarks>
    /// <para>By default, when Aspose.Words saves a document to HTML, it saves CSS information inline
    /// (as a value of the <b>style</b> attribute on every element).
    /// </para>
    /// <para><see cref="CssSavingArgs"/> allows to save CSS information into file by providing your own stream object.</para>
    /// <para>To save CSS into stream, use the <see cref="CssStream"/> property.</para>
    /// <para>To suppress saving CSS into a file and embedding to HTML document use the <see cref="IsExportNeeded"/> property.</para>
    /// </remarks>
    public class CssSavingArgs
    {
        /// <summary>
        /// Ctor. Should be internal. Client code won't create this object.
        /// </summary>
        internal CssSavingArgs(Document document)
        {
            Debug.Assert(document != null);
            mDocument = document;
        }

        /// <summary>
        /// Gets the document object that is currently being saved.
        /// </summary>
        public Document Document
        {
            get { return mDocument; }
        }

        /// <summary>
        /// Specifies whether Aspose.Words should keep the stream open or close it after saving an CSS information.
        /// </summary>
        /// <remarks>
        /// <para>Default is <c>false</c> and Aspose.Words will close the stream you provided
        /// in the <see cref="CssStream"/> property after writing an CSS information into it.
        /// Specify <c>true</c> to keep the stream open.</para>
        /// <seealso cref="CssStream"/>
        /// </remarks>
        public bool KeepCssStreamOpen
        {
            get { return mKeepCssStreamOpen; }
            set { mKeepCssStreamOpen = value; }
        }

        /// <summary>
        /// Allows to specify the stream where the CSS information will be saved to.
        /// </summary>
        /// <remarks>
        /// <para>This property allows you to save CSS information to a stream.</para>
        /// <para>The default value is <c>null</c>. This property doesn't suppress saving CSS information to a file or
        ///  embedding to HTML document. To suppress exporting CSS use the <see cref="IsExportNeeded"/> property.</para>
        /// 
        /// <para>Using <see cref="ICssSavingCallback" /> you cannot substitute CSS with 
        /// another. It is intended only for saving CSS to a stream.</para>
        /// 
        /// <seealso cref="KeepCssStreamOpen"/>
        /// </remarks>
        /// <javaName>CssStream(java.io.OutputStream)</javaName>
#if PLAIN_JAVA
        // Replaces the Stream with java.io.OutputStream in Java public API.
        public java.io.OutputStream getCssStream() { return mCssStream; }
        public void setCssStream(java.io.OutputStream value) { mCssStream = value; }
        private java.io.OutputStream mCssStream;
#else
        [CppIOStreamWrapper(IOStreamType.OStream)]
        public Stream CssStream
        {
            get { return mCssStream; }
            set { mCssStream = value; }
        }

        private Stream mCssStream;
#endif

        /// <summary>
        /// Allows to specify whether the CSS will be exported to file and embedded to HTML document. Default is <c>true</c>.
        /// When this property is <c>false</c>, the CSS information will not be saved to a CSS file and will not be embedded to HTML document.
        /// </summary>
        public bool IsExportNeeded
        {
            get { return mIsExportNeeded; }
            set { mIsExportNeeded = value; }
        }

        /// <summary>
        /// Exists to make calling code autoportable.
        /// </summary>
        internal bool HasUserStream
        {
            get { return mCssStream != null; }
        }

        /// <summary>
        /// Exists to make calling code autoportable.
        /// </summary>
        internal UserStreamWrapper CreateUserStreamWrapper()
        {
            return new UserStreamWrapper(mCssStream, mKeepCssStreamOpen);
        }

        private readonly Document mDocument;
        private bool mKeepCssStreamOpen;
        private bool mIsExportNeeded = true;
    }
}
