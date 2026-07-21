// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 05/03/2013 by Alexey Noskov

using System;
using System.IO;
using CodePorting.Translator.Cs2Cpp;

namespace Aspose.Words.Saving
{
    /// <summary>
    /// Provides data for the <see cref="IResourceSavingCallback.ResourceSaving"/> event.
    /// <para>To learn more, visit the <a href="https://docs.aspose.com/words/net/save-a-document/">Save a Document</a> documentation article.</para>
    /// </summary>
    /// <remarks>
    /// <para>By default, when Aspose.Words saves a document to fixed page HTML, SVG or Markdown, it saves each resource into
    /// a separate file. Aspose.Words uses the document file name and a unique number to generate unique file name
    /// for each resource found in the document.</para>
    ///
    /// <para><see cref="ResourceSavingArgs"/> allows to redefine how resource file names are generated or to
    /// completely circumvent saving of resources into files by providing your own stream objects.</para>
    ///
    /// <para>To apply your own logic for generating resource file names use the
    /// <see cref="ResourceFileName"/> property.</para>
    ///
    /// <para>To save resources into streams instead of files, use the <see cref="ResourceStream"/> property.</para>
    /// </remarks>
    public class ResourceSavingArgs
    {
        /// <summary>
        /// Ctor. Should be internal. Client code won't create this object.
        /// </summary>
        internal ResourceSavingArgs(Document doc, string resourceFileName, string resourceFileUri)
        {
            Debug.Assert(StringUtil.HasChars(resourceFileName));
            Debug.Assert(resourceFileUri != null);

            mDoc = doc;
            mResourceFileName = resourceFileName;
            mResourceFileUri = resourceFileUri;
        }

        /// <summary>
        /// Gets the document object that is currently being saved.
        /// </summary>
        public Document Document
        {
            get { return mDoc; }
        }

        #if !CPP_DOC
        /// <summary>
        /// Gets or sets the file name (without path) where the resource will be saved to.
        /// </summary>
        /// <remarks>
        /// <para>This property allows you to redefine how the resource file names are generated
        /// during export to fixed page HTML, SVG or Markdown.</para>
        ///
        /// <para>When the event is fired, this property contains the file name that was generated
        /// by Aspose.Words. You can change the value of this property to save the resource into a
        /// different file. Note that file names must be unique.</para>
        ///
        /// <p>Aspose.Words automatically generates a unique file name for every resource when
        /// exporting to fixed page HTML, SVG or Markdown format. How the resource file name is generated
        /// depends on whether you save the document to a file or to a stream.</p>
        ///
        /// <p>When saving a document to a file, the generated resource file name looks like
        /// <i>&lt;document base file name&gt;.&lt;image number&gt;.&lt;extension&gt;</i>.</p>
        ///
        /// <p>When saving a document to a stream, the generated resource file name looks like
        /// <i>Aspose.Words.&lt;document guid&gt;.&lt;image number&gt;.&lt;extension&gt;</i>.</p>
        ///
        /// </remarks>
        #else
        /// <summary>
        /// Gets or sets the file name (without path) where the resource will be saved to.
        /// </summary>
        /// <remarks>
        /// <para>This property allows you to redefine how the resource file names are generated
        /// during export to fixed page HTML or SVG.</para>
        ///
        /// <para>When the event is fired, this property contains the file name that was generated
        /// by Aspose.Words. You can change the value of this property to save the resource into a
        /// different file. Note that file names must be unique.</para>
        ///
        /// <p>Aspose.Words automatically generates a unique file name for every resource when
        /// exporting to fixed page HTML or SVG format. How the resource file name is generated
        /// depends on whether you save the document to a file or to a stream.</p>
        ///
        /// <p>When saving a document to a file, the generated resource file name looks like
        /// <i>&lt;document base file name&gt;.&lt;image number&gt;.&lt;extension&gt;</i>.</p>
        ///
        /// <p>When saving a document to a stream, the generated resource file name looks like
        /// <i>Aspose.Words.&lt;document guid&gt;.&lt;image number&gt;.&lt;extension&gt;</i>.</p>
        ///
        /// <para><see cref="ResourceFileName"/> must contain only the file name without the path.
        /// Aspose.Words determines the path for saving and the value of the <c>src</c> attribute for writing
        /// to fixed page HTML or SVG using the document file name</para>
        ///
        /// <seealso cref="ResourceStream"/>
        /// </remarks>
        #endif
        public string ResourceFileName
        {
            get { return mResourceFileName; }
            set
            {
                ArgumentUtil.CheckHasChars(value, "ResourceFileName");
                if (Path.GetFileName(value) != value)
                    throw new ArgumentException("ResourceFileName must be a file name without path.");
                mResourceFileName = value;
            }
        }

        #if !CPP_DOC
        /// <summary>
        /// Gets or sets the uniform resource identifier (URI) used to reference the resource file from the document.
        /// </summary>
        /// <remarks>
        /// <para>This property allows you to change URIs of resource files exported to fixed page HTML,
        /// SVG or Markdown documents.</para>
        ///
        /// <para>Aspose.Words automatically generates an URI for every resource file during export to fixed page HTML,
        /// SVG or Markdown format. The generated URIs reference resource files saved by Aspose.Words. However, the URIs
        /// can be incorrect if resource files are to be moved to other location or if resource files are saved to streams.
        /// This property allows to correct URIs in these cases.</para>
        ///
        /// <para>When the event is fired, this property contains the URI that was generated
        /// by Aspose.Words. You can change the value of this property to provide a custom URI for the resource file.</para>
        ///
        /// <see cref="MarkdownSaveOptions.ImagesFolder"/>
        /// <see cref="MarkdownSaveOptions.ImagesFolderAlias"/>
        /// </remarks>
        #else
        /// <summary>
        /// Gets or sets the uniform resource identifier (URI) used to reference the resource file from the document.
        /// </summary>
        /// <remarks>
        /// <para>This property allows you to change URIs of resource files exported to fixed page HTML or SVG documents.</para>
        ///
        /// <para>Aspose.Words automatically generates an URI for every resource file during export to fixed page HTML
        /// or SVG format. The generated URIs reference resource files saved by Aspose.Words. However, the URIs can be
        /// incorrect if resource files are to be moved to other location or if resource files are saved to streams.
        /// This property allows to correct URIs in these cases.</para>
        ///
        /// <para>When the event is fired, this property contains the URI that was generated
        /// by Aspose.Words. You can change the value of this property to provide a custom URI for the resource file.</para>
        /// </remarks>
        #endif
        public string ResourceFileUri
        {
            get { return mResourceFileUri; }
            set
            {
                ArgumentUtil.CheckHasChars(value, "ResourceFileUri");
                mResourceFileUri = value;
                mHasUserResourceFileUri = true;
            }
        }

        /// <summary>
        /// Indicates whether the user has provided a custom URI for the resource file.
        /// </summary>
        internal bool HasUserResourceFileUri
        {
            get { return mHasUserResourceFileUri; }
        }

        /// <summary>
        /// Specifies whether Aspose.Words should keep the stream open or close it after saving a resource.
        /// </summary>
        /// <remarks>
        /// <para>Default is <c>false</c> and Aspose.Words will close the stream you provided
        /// in the <see cref="ResourceStream"/> property after writing a resource into it.
        /// Specify <c>true</c> to keep the stream open.</para>
        ///
        /// <seealso cref="ResourceStream"/>
        /// </remarks>
        public bool KeepResourceStreamOpen
        {
            get { return mKeepResourceStreamOpen; }
            set { mKeepResourceStreamOpen = value; }
        }

        /// <summary>
        /// Allows to specify the stream where the resource will be saved to.
        /// </summary>
        /// <remarks>
        /// <para>This property allows you to save resources to streams instead of files.</para>
        ///
        /// <para>The default value is <c>null</c>. When this property is <c>null</c>, the resource
        /// will be saved to a file specified in the <see cref="ResourceFileName"/> property.</para>
        ///
        /// <para>Using <see cref="IResourceSavingCallback" /> you cannot substitute one resource with
        /// another. It is intended only for control over location where to save resources.</para>
        ///
        /// <seealso cref="ResourceFileName"/>
        /// <seealso cref="KeepResourceStreamOpen"/>
        /// </remarks>
        /// <javaName>ResourceStream(java.io.OutputStream)</javaName>
#if PLAIN_JAVA
        // Replaces the Stream with java.io.OutputStream in Java public API.
        public java.io.OutputStream getResourceStream() { return mResourceStream; }
        public void setResourceStream(java.io.OutputStream value) { mResourceStream = value; }
        private java.io.OutputStream mResourceStream;
#else
        [CppIOStreamWrapper(IOStreamType.OStream)]
        public Stream ResourceStream
        {
            get { return mResourceStream; }
            set { mResourceStream = value; }
        }
        private Stream mResourceStream;
#endif

        /// <summary>
        /// Exists to make calling code autoportable.
        /// </summary>
        internal bool HasUserStream
        {
            get { return mResourceStream != null; }
        }

        /// <summary>
        /// Exists to make calling code autoportable.
        /// </summary>
        internal UserStreamWrapper CreateUserStreamWrapper()
        {
            return new UserStreamWrapper(mResourceStream, mKeepResourceStreamOpen);
        }

        private string mResourceFileName;
        private string mResourceFileUri;
        private bool mHasUserResourceFileUri;
        private bool mKeepResourceStreamOpen;
        private readonly Document mDoc;
    }
}
