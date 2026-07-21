// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 11/07/2005 by Roman Korchagin

using System;
using System.IO;
using System.Text;
using System.Drawing;
using System.Reflection;
using System.Collections.Generic;
using CodePorting.Translator.Cs2Cpp;
using Aspose.Ss;
using Aspose.Common;
using Aspose.Collections;
using Aspose.JavaAttributes;
using Aspose.Words.Vba;
using Aspose.Words.Lists;
using Aspose.Words.Fonts;
using Aspose.Words.Tables;
using Aspose.Words.Fields;
using Aspose.Words.Saving;
using Aspose.Words.Markup;
using Aspose.Words.Framesets;
using Aspose.Words.Themes;
using Aspose.Words.Loading;
using Aspose.Words.Drawing;
using Aspose.Words.Progress;
using Aspose.Words.Settings;
using Aspose.Words.Revisions;
using Aspose.Words.Validation;
using Aspose.Words.Properties;
using Aspose.Words.RW.Factories;
using Aspose.Words.BuildingBlocks;
using Aspose.Words.WebExtensions;
using Aspose.Words.DigitalSignatures;
using Aspose.Words.Notes;
using SaveOptions=Aspose.Words.Saving.SaveOptions;
#if NETSTANDARD
using Graphics = SkiaSharp.SKCanvas;
#else
using System.Drawing.Printing;
using System.Web;
#endif

namespace Aspose.Words
{
#if NETSTANDARD // alexnosk: I am not sure how to be with xml comments where <see cref> is used, for now use preprocessor.
    /// <summary>
    /// Represents a Word document.
    /// <para>To learn more, visit the <a href="https://docs.aspose.com/words/net/working-with-document/">Working with Document</a> documentation article.</para>
    /// </summary>
    /// <remarks>
    /// <p>The <see cref="Document"/> is a central object in the Aspose.Words library.</p>
    ///
    /// <p>To load an existing document in any of the <see cref="LoadFormat"/> formats, pass a file name
    /// or a stream into one of the <see cref="Document"/> constructors. To create a blank document, call the
    /// constructor without parameters.</p>
    ///
    /// <p>Use one of the Save method overloads to save the document in any of the
    /// <see cref="SaveFormat"/> formats.</p>
    ///
    /// <para>To draw document pages directly onto a <b>Graphics</b> object use
    /// <see cref="RenderToScale"/> or <see cref="RenderToSize"/> method.</para>
    ///
    /// <p><see cref="MailMerge"/> is the Aspose.Words's reporting engine that allows to populate
    /// reports designed in Microsoft Word with data from various data sources quickly and easily.
    /// The data can be from a <ms>DataSet, DataTable, DataView, IDataReader</ms>
    /// <java>java.sql.ResultSet</java> or an array of values.
    /// <b>MailMerge</b> will go through the records found in the data source and insert them into
    /// mail merge fields in the document growing it as necessary.</p>
    ///
    /// <p><see cref="Document"/> stores document-wide information such as <see cref="DocumentBase.Styles"/>,
    /// <see cref="BuiltInDocumentProperties"/>, <see cref="CustomDocumentProperties"/>, lists and macros.
    /// Most of these objects are accessible via the corresponding properties of the <see cref="Document"/>.</p>
    ///
    /// <p>The <see cref="Document"/> is a root node of a tree that contains all other nodes of the document.
    /// The tree is a Composite design pattern and in many ways similar to XmlDocument.
    /// The content of the document can be manipulated freely programmatically:</p>
    /// <list type="bullet">
    /// <item>The nodes of the document can be accessed via typed collections, for example <see cref="Sections"/>,
    /// <see cref="ParagraphCollection"/> etc.</item>
    /// <item>The nodes of the document can be selected by their node type using
    /// <see cref="CompositeNode.GetChildNodes(Words.NodeType, bool)"/>
    /// or using an XPath query with <see cref="CompositeNode.SelectNodes"/> or <see cref="CompositeNode.SelectSingleNode"/>.</item>
    /// <item>Content nodes can be added or removed from anywhere in the document using
    /// <see cref="CompositeNode.InsertBefore{T}(T, Node)"/>, <see cref="CompositeNode.InsertAfter{T}(T, Node)"/>,
    /// <see cref="CompositeNode.RemoveChild{T}"/> and other
    /// methods provided by the base class <see cref="CompositeNode"/>.</item>
    /// <item>The formatting attributes of each node can be changed via the properties of that node.</item>
    /// </list>
    ///
    /// <p>Consider using <see cref="DocumentBuilder"/> that simplifies the task of programmatically creating
    /// or populating the document tree.</p>
    ///
    /// <p>The <see cref="Document"/> can contain only <see cref="Section"/> objects.</p>
    ///
    /// <p>In Microsoft Word, a valid document needs to have at least one section.</p>
    /// </remarks>
#elif CPP_DOC // Documentation reference to excluded code. Some para were excluded.
    /// <summary>
    /// Represents a Word document.
    /// <para>To learn more, visit the <a href="https://docs.aspose.com/words/net/working-with-document/">Working with Document</a> documentation article.</para>
    /// </summary>
    /// <remarks>
    /// <p>The <b>Document</b> is a central object in the Aspose.Words library.</p>
    ///
    /// <p>To load an existing document in any of the <see cref="LoadFormat"/> formats, pass a file name
    /// or a stream into one of the <b>Document</b> constructors. To create a blank document, call the
    /// constructor without parameters.</p>
    ///
    /// <p>Use one of the Save method overloads to save the document in any of the
    /// <see cref="SaveFormat"/> formats.</p>
    ///
    /// <p><b>Document</b> stores document-wide information such as <see cref="DocumentBase.Styles"/>,
    /// <see cref="BuiltInDocumentProperties"/>, <see cref="CustomDocumentProperties"/>, lists and macros.
    /// Most of these objects are accessible via the corresponding properties of the <b>Document</b>.</p>
    ///
    /// <p>The <b>Document</b> is a root node of a tree that contains all other nodes of the document.
    /// The tree is a Composite design pattern and in many ways similar to XmlDocument.
    /// The content of the document can be manipulated freely programmatically:</p>
    /// <list type="bullet">
    /// <item>The nodes of the document can be accessed via typed collections, for example <see cref="Sections"/>,
    /// <see cref="ParagraphCollection"/> etc.</item>
    /// <item>The nodes of the document can be selected by their node type using
    /// <see cref="CompositeNode.GetChildNodes(Words.NodeType, bool)"/>
    /// or using an XPath query with <see cref="CompositeNode.SelectNodes"/> or <see cref="CompositeNode.SelectSingleNode"/>.</item>
    /// <item>Content nodes can be added or removed from anywhere in the document using
    /// <see cref="CompositeNode.InsertBefore{T}(T, Node)"/>, <see cref="CompositeNode.InsertAfter{T}(T, Node)"/>,
    /// <see cref="CompositeNode.RemoveChild{T}"/> and other
    /// methods provided by the base class <see cref="CompositeNode"/>.</item>
    /// <item>The formatting attributes of each node can be changed via the properties of that node.</item>
    /// </list>
    ///
    /// <p>Consider using <see cref="DocumentBuilder"/> that simplifies the task of programmatically creating
    /// or populating the document tree.</p>
    ///
    /// <p>The <b>Document</b> can contain only <see cref="Section"/> objects.</p>
    ///
    /// <p>In Microsoft Word, a valid document needs to have at least one section.</p>
    /// </remarks>
#else
    /// <summary>
    /// Represents a Word document.
    /// <para>To learn more, visit the <a href="https://docs.aspose.com/words/net/working-with-document/">Working with Document</a> documentation article.</para>
    /// </summary>
    /// <remarks>
    /// <p>The <see cref="Document"/> is a central object in the Aspose.Words library.</p>
    ///
    /// <p>To load an existing document in any of the <see cref="LoadFormat"/> formats, pass a file name
    /// or a stream into one of the <see cref="Document"/> constructors. To create a blank document, call the
    /// constructor without parameters.</p>
    ///
    /// <p>Use one of the Save method overloads to save the document in any of the
    /// <see cref="SaveFormat"/> formats.</p>
    ///
    /// <p><see cref="Document"/> stores document-wide information such as <see cref="DocumentBase.Styles"/>,
    /// <see cref="BuiltInDocumentProperties"/>, <see cref="CustomDocumentProperties"/>, lists and macros.
    /// Most of these objects are accessible via the corresponding properties of the <see cref="Document"/>.</p>
    ///
    /// <p>The <see cref="Document"/> is a root node of a tree that contains all other nodes of the document.
    /// The tree is a Composite design pattern and in many ways similar to XmlDocument.
    /// The content of the document can be manipulated freely programmatically:</p>
    /// <list type="bullet">
    /// <item>The nodes of the document can be accessed via typed collections, for example <see cref="Sections"/>,
    /// <see cref="ParagraphCollection"/> etc.</item>
    /// <item>The nodes of the document can be selected by their node type using
    /// <see cref="CompositeNode.GetChildNodes(Words.NodeType, bool)"/>
    /// or using an XPath query with <see cref="CompositeNode.SelectNodes"/> or <see cref="CompositeNode.SelectSingleNode"/>.</item>
    /// <item>Content nodes can be added or removed from anywhere in the document using
    /// <see cref="CompositeNode.InsertBefore{T}(T, Node)"/>, <see cref="CompositeNode.InsertAfter{T}(T, Node)"/>,
    /// <see cref="CompositeNode.RemoveChild{T}"/> and other
    /// methods provided by the base class <see cref="CompositeNode"/>.</item>
    /// <item>The formatting attributes of each node can be changed via the properties of that node.</item>
    /// </list>
    ///
    /// <p>Consider using <see cref="DocumentBuilder"/> that simplifies the task of programmatically creating
    /// or populating the document tree.</p>
    ///
    /// <p>The <see cref="Document"/> can contain only <see cref="Section"/> objects.</p>
    ///
    /// <p>In Microsoft Word, a valid document needs to have at least one section.</p>
    /// </remarks>
#endif
    [JavaGenericArguments("DocumentBase<Node>")]
    public class Document : DocumentBase, ISectionAttrSource, IWatermarkProvider
    {
        /// <summary>
        /// Static ctor.
        /// </summary>
        static Document()
        {
            // Register encodings supported in the desktop .NET Framework but not in .NET Standard.
            EncodingUtil.RegisterEncodings();
        }

        /// <summary>
        /// Ctor that allows to create a completely empty document node or load a blank document.
        /// </summary>
        internal Document(DocumentCtorMode documentCtorMode) : this(documentCtorMode, null)
        {
        }

        /// <summary>
        /// Ctor that allows to create a completely empty document node or load a blank document.
        /// </summary>
        internal Document(DocumentCtorMode documentCtorMode, LoadOptions loadOptions)
        {
            switch (documentCtorMode)
            {
                case DocumentCtorMode.BlankDocumentNode:
                {
                    LoadBlank(null);
                    SetLocaleDefaultsForNewDocument();
                    break;
                }

                case DocumentCtorMode.EmptyDocumentNode:
                {
                    if (loadOptions != null)
                        PostLoadTasks(loadOptions);

                    break;
                }

                default:
                    break;
            }
        }


        /// <summary>
        /// Opens an existing document from a stream.
        /// This ctor for internal use only, it is forbidden to use public ctors inside code to avoid extra credit billings.
        /// </summary>
        /// <param name="stream">Stream where to load the document from.</param>
        /// <param name="loadOptions">Additional options to use when loading a document. Can be <c>null</c>.</param>
        /// <param name="increaseCredit">Increase credit if it is needed (for example when <see cref="PlainTextDocument"/> document is created)</param>
        internal Document(Stream stream, LoadOptions loadOptions, bool increaseCredit) : this(DocumentCtorMode.EmptyDocumentNode)
        {
            if (stream == null)
                throw new ArgumentNullException("stream");

            Load(stream, loadOptions);

            // FOSS
        }

        /// <overloads>Creates or loads a document.</overloads>
        /// <summary>
        /// Creates a blank Word document.
        /// </summary>
        /// <remarks>
        /// <p>A blank document is retrieved from resources, and by default, the resulting document looks more like created by <see cref="MsWordVersion.Word2007"/>.
        /// This blank document contains a default fonts table, minimal default styles, and latent styles.</p>
        /// <p><see cref="CompatibilityOptions.OptimizeFor"/> method can be used to optimize the document contents as well as default Aspose.Words behavior to a particular version of MS Word.</p>
        /// <p>The document paper size is Letter by default. If you want to change page setup, use
        /// <see cref="Section.PageSetup"/>.</p>
        /// <p>After creation, you can use <see cref="DocumentBuilder"/> to add document content easily.</p>
        /// </remarks>
        public Document() : this(DocumentCtorMode.BlankDocumentNode)
        {
            // Please NOTE - It is forbidden to use public ctors inside code to avoid extra credit billings.
            // Use internal Document ctors instead.

            // FOSS
        }

        /// <summary>
        /// Opens an existing document from a file. Automatically detects the file format.
        /// </summary>
        /// <param name="fileName">File name of the document to open.</param>
        /// <include file='..\Docs\Text.xml' path='Topics/Topic[@name="Document.LoadFileExceptions"]/*'/>
        public Document([CppForceStringParam] string fileName)
            : this(fileName, null)
        {
            // Please NOTE - It is forbidden to use public ctors inside code to avoid extra credit billings.
            // Use internal Document ctors instead.
        }

        /// <summary>
        /// Opens an existing document from a file. Allows to specify additional options such as an encryption password.
        /// </summary>
        /// <param name="fileName">File name of the document to open.</param>
        /// <param name="loadOptions">Additional options to use when loading a document. Can be <c>null</c>.</param>
        /// <include file='..\Docs\Text.xml' path='Topics/Topic[@name="Document.LoadFileExceptions"]/*'/>
        public Document(string fileName, LoadOptions loadOptions)
            : this(DocumentCtorMode.EmptyDocumentNode)
        {
            // Please NOTE - It is forbidden to use public ctors inside code to avoid extra credit billings.
            // Use internal Document ctors instead.

            ArgumentUtil.CheckHasChars(fileName, "fileName");

            mOriginalFileName = fileName;
            mBaseUri = GetBaseUriFromFileName(fileName);

            IResourceLoadingCallback resourceLoadingCallback = (loadOptions != null)
                ? loadOptions.ResourceLoadingCallback
                : null;

            Stream stream = OpenDocumentStream(fileName, resourceLoadingCallback);
            if (stream == null)
                return;

            using (stream)
                Load(stream, loadOptions);

            // FOSS

            AttachMappedCustomXmlUpdater();
        }

        private static string GetBaseUriFromFileName(string fileName)
        {
            bool hasFileScheme = UriUtil.HasFileScheme(fileName);

            if (hasFileScheme)
            {
                fileName = UriUtil.RemoveFileSchemePrefix(fileName);
            }

            // WORDSNET-26735 Correctly extract base URI in case when a document is loaded from a URI.
            if (UriUtil.IsHrefWithScheme(fileName))
            {
                return UriUtil.GetDirectoryHrefWithScheme(fileName);
            }

            string directoryName = Path.GetDirectoryName(fileName);

            if (hasFileScheme)
            {
                directoryName = UriUtil.AddFileSchemePrefix(directoryName);
            }

            return directoryName;
        }

        private static string GetBaseUriFromLoadOptions(LoadOptions loadOptions, string originalFileName)
        {
            if (!StringUtil.HasChars(loadOptions.BaseUri))
            {
                return string.Empty;
            }

            if (StringUtil.HasChars(originalFileName))
            {
                if (UriUtil.HasFileScheme(originalFileName))
                {
                    originalFileName = UriUtil.RemoveFileSchemePrefix(originalFileName);
                }

                // WORDSNET-26575 If a document is loaded from a URI and base URI specified in load options
                // doesn't have a scheme and document's URI has a scheme, we append that scheme to base URI.
                if (UriUtil.IsHrefWithScheme(originalFileName) && !UriUtil.IsHrefWithScheme(loadOptions.BaseUri))
                {
                    string scheme = UriUtil.GetScheme(originalFileName);
                    return scheme + "://" + loadOptions.BaseUri;
                }
            }

            return loadOptions.BaseUri;
        }

        /// <summary>
        /// Opens an existing document from a stream. Automatically detects the file format.
        /// </summary>
        /// <include file='..\Docs\Text.xml' path='Topics/Topic[@name="Document.OpenStreamCommon"]/*'/>
        /// <param name="stream">Stream where to load the document from.</param>
        /// <include file='..\Docs\Text.xml' path='Topics/Topic[@name="Document.LoadStreamExceptions"]/*'/>
        ///
        /// <javaName>Document(java.io.InputStream stream)</javaName>
        // JAVA: the first public api change map will be used: Stream -> java.io.InputStream
        public Document([CppIOStreamWrapper(IOStreamType.IStream)]Stream stream) : this(stream, null)
        {
            // Please NOTE - It is forbidden to use public ctors inside code to avoid extra credit billings.
            // Use internal Document ctors instead.
        }

        /// <summary>
        /// Opens an existing document from a stream. Allows to specify additional options such as an encryption password.
        /// </summary>
        /// <include file='..\Docs\Text.xml' path='Topics/Topic[@name="Document.OpenStreamCommon"]/*'/>
        /// <param name="stream">The stream where to load the document from.</param>
        /// <param name="loadOptions">Additional options to use when loading a document. Can be <c>null</c>.</param>
        /// <include file='..\Docs\Text.xml' path='Topics/Topic[@name="Document.LoadStreamExceptions"]/*'/>
        ///
        /// <javaName>Document(java.io.InputStream stream, com.aspose.words.LoadOptions loadOptions)</javaName>
        // JAVA: the first public api change map will be used: Stream -> java.io.InputStream
        public Document([CppIOStreamWrapper(IOStreamType.IStream)]Stream stream, LoadOptions loadOptions) : this(DocumentCtorMode.EmptyDocumentNode)
        {
            // Please NOTE - It is forbidden to use public ctors inside code to avoid extra credit billings.
            // Use internal Document ctors instead.

            if (stream == null)
                throw new ArgumentNullException("stream");

            Load(stream, loadOptions);

            AttachMappedCustomXmlUpdater();

            // FOSS
        }

        /// <summary>
        /// Gets or sets the full path of the template attached to the document.
        /// </summary>
        /// <remarks>
        /// <p>Empty string means the document is attached to the Normal template.</p>
        /// <seealso cref="Properties.BuiltInDocumentProperties.Template"/>
        /// </remarks>
        /// <exception cref="ArgumentNullException">Throws if you attempt to set to a <c>null</c> value.</exception>
        public string AttachedTemplate
        {
            get { return DocPr.AttachedTemplate; }
            set
            {
                ArgumentUtil.CheckNotNull(value, "value");
                DocPr.AttachedTemplate = value;
            }
        }

        /// <summary>
        /// Gets or sets a flag indicating whether the styles in the document are updated to match the styles in the
        /// attached template each time the document is opened in MS Word.
        /// </summary>
        public bool AutomaticallyUpdateStyles
        {
            get { return DocPr.LinkStyles; }
            set { DocPr.LinkStyles = value; }
        }

        /// <summary>
        /// Specifies whether to turn on the gray shading on form fields.
        /// </summary>
        public bool ShadeFormData
        {
            get { return !DocPr.DoNotShadeFormData; }
            set { DocPr.DoNotShadeFormData = !value; }
        }

        /// <summary>
        /// True if changes are tracked when this document is edited in Microsoft Word.
        /// </summary>
        /// <remarks>
        /// <p>Setting this option only instructs Microsoft Word whether the track changes
        /// is turned on or off. This property has no effect on changes to the document that you make
        /// programmatically via Aspose.Words.</p>
        /// <p>If you want to automatically track changes as they are made programmatically by Aspose.Words
        /// to this document use the <see cref="Document.StartTrackRevisions(string, DateTime)" /> method.</p>
        /// </remarks>
        public bool TrackRevisions
        {
            get { return DocPr.TrackRevisions; }
            set { DocPr.TrackRevisions = value; }
        }

        /// <summary>
        /// Specifies whether to display grammar errors in this document.
        /// </summary>
        public bool ShowGrammaticalErrors
        {
            get { return !DocPr.HideGrammaticalErrors; }
            set { DocPr.HideGrammaticalErrors = !value; }
        }

        /// <summary>
        /// Specifies whether to display spelling errors in this document.
        /// </summary>
        public bool ShowSpellingErrors
        {
            get { return !DocPr.HideSpellingErrors; }
            set { DocPr.HideSpellingErrors = !value; }
        }

        /// <summary>
        /// Returns <c>true</c> if the document has been checked for spelling.
        /// </summary>
        /// <remarks>
        /// To recheck the spelling in the document, set this property to <c>false</c>.
        /// </remarks>
        public bool SpellingChecked
        {
            get { return DocPr.ProofStateSpelling != ProofState.None; }
            set
            {
                DocPr.ProofStateSpelling = value
                    ? ProofState.Clean
                    : ProofState.None;
            }
        }

        /// <summary>
        /// Returns <c>true</c> if the document has been checked for grammar.
        /// </summary>
        /// <remarks>
        /// To recheck the grammar in the document, set this property to <c>false</c>.
        /// </remarks>
        public bool GrammarChecked
        {
            get { return DocPr.ProofStateGrammar != ProofState.None; }
            set
            {
                DocPr.ProofStateGrammar = value
                    ? ProofState.Clean
                    : ProofState.None;
            }
        }

        /// <summary>
        /// Specifies whether kerning applies to both Latin text and punctuation.
        /// </summary>
        public bool PunctuationKerning
        {
            get { return DocPr.PunctuationKerning; }
            set { DocPr.PunctuationKerning = value; }
        }

        /// <summary>
        /// Returns <see cref="NodeType.Document"/>.
        /// </summary>
        public override NodeType NodeType
        {
            get { return NodeType.Document; }
        }

        /// <summary>
        /// Returns a collection that represents all the built-in document properties of the document.
        /// </summary>
        public BuiltInDocumentProperties BuiltInDocumentProperties
        {
            get { return mBuiltInDocumentProperties; }
        }

        /// <summary>
        /// Returns a collection that represents a list of task pane add-ins.
        /// </summary>
        public TaskPaneCollection WebExtensionTaskPanes
        {
            get { return mWebExtensionTaskPanes; }
        }

        /// <summary>
        /// Returns a collection that represents all the custom document properties of the document.
        /// </summary>
        public CustomDocumentProperties CustomDocumentProperties
        {
            get
            {
                if (mCustomDocumentProperties == null)
                    mCustomDocumentProperties = new CustomDocumentProperties(this);

                return mCustomDocumentProperties;
            }
        }

        /// <summary>
        /// Gets the currently active document protection type.
        /// </summary>
        /// <remarks>
        /// <p>This property allows to retrieve the currently set document protection type.
        /// To change the document protection type use the <see cref="Protect(Aspose.Words.ProtectionType,string)"/>
        /// and <see cref="Unprotect()"/> methods.</p>
        ///
        /// <p>When a document is protected, the user can make only limited changes,
        /// such as adding annotations, making revisions, or completing a form.</p>
        ///
        /// <para>Note that document protection is different from write protection.
        /// Write protection is specified using the <see cref="WriteProtection"/></para>
        ///
        /// <seealso cref="Protect(Aspose.Words.ProtectionType,string)"/>
        /// <seealso cref="Unprotect()"/>
        /// <seealso cref="WriteProtection"/>
        /// </remarks>
        public ProtectionType ProtectionType
        {
            get { return DocPr.DocumentProtection.EnforcedProtectionType; }
        }

        /// <summary>
        /// Exposes document protection settings for testing purposes.
        /// </summary>
        internal DocumentProtection DocumentProtection
        {
            get { return DocPr.DocumentProtection; }
        }

        /// <summary>
        /// Returns a collection that represents all sections in the document.
        /// </summary>
        public SectionCollection Sections
        {
            get
            {
                if (mSectionsCache == null)
                    mSectionsCache = new SectionCollection(this);
                return mSectionsCache;
            }
        }

        /// <summary>
        /// Gets the first section in the document.
        /// </summary>
        /// <remarks>
        /// Returns <c>null</c> if there are no sections.
        /// </remarks>
        public Section FirstSection
        {
            get { return (Section)GetChild(NodeType.Section, 0, false); }
        }

        /// <summary>
        /// Gets the last section in the document.
        /// </summary>
        /// <remarks>
        /// Returns <c>null</c> if there are no sections.
        /// </remarks>
        public Section LastSection
        {
            get { return (Section)GetChild(NodeType.Section, -1, false); }
        }

        /// <summary>
        /// Provides options to control how the document is displayed in Microsoft Word.
        /// </summary>
        public ViewOptions ViewOptions
        {
            get { return DocPr.ViewOptions; }
        }

        /// <summary>
        /// Provides access to the document write protection options.
        /// </summary>
        public WriteProtection WriteProtection
        {
            get { return DocPr.WriteProtection; }
        }

        /// <summary>
        /// Provides access to document compatibility options (that is, the user preferences entered on the <b>Compatibility</b>
        /// tab of the <b>Options</b> dialog in Word).
        /// </summary>
        public CompatibilityOptions CompatibilityOptions
        {
            get { return DocPr.CompatibilityOptions; }
        }

        /// <summary>
        /// Gets or sets the object that contains all of the mail merge information for a document.
        /// </summary>
        /// <remarks>
        /// <para>You can use this object to specify a mail merge data source for a document and this information
        /// (along with the available data fields) will appear in Microsoft Word when the user opens this document.
        /// Or you can use this object to query mail merge settings that the user has specified in Microsoft Word
        /// for this document.</para>
        ///
        /// <para>This object is never <c>null</c>.</para>
        /// </remarks>
        public MailMergeSettings MailMergeSettings
        {
            get { return DocPr.MailMergeSettings; }
            set
            {
                ArgumentUtil.CheckNotNull(value, "value");
                DocPr.MailMergeSettings = value;
            }
        }

        /// <summary>
        /// Provides access to document hyphenation options.
        /// </summary>
        public HyphenationOptions HyphenationOptions
        {
            get { return DocPr.HyphenationOptions; }
        }

        /// <summary>
        /// Returns <c>true</c> if the document has any tracked changes.
        /// </summary>
        /// <remarks>
        /// This property is a shortcut for comparing <see cref="RevisionCollection.Count"/> to zero.
        /// </remarks>
        public bool HasRevisions
        {
            get { return Revisions.Count != 0; }
        }

        /// <summary>
        /// Returns <c>true</c> if the document has a VBA project (macros).
        /// </summary>
        /// <seealso cref="RemoveMacros"/>
        public bool HasMacros
        {
            get { return (mVbaProject != null); }
        }

        /// <summary>
        /// Provides access to the document watermark.
        /// </summary>
        public Watermark Watermark
        {
            get
            {
                if (mWatermark == null)
                    mWatermark = new Watermark(this, this);

                return mWatermark;
            }
        }

        /// <summary>
        /// Returns <c>true</c> if the document has toolbar or command customizations.
        /// </summary>
        internal bool HasCustomizations
        {
            get { return (HasAttachedToolbars || HasAllocatedCommands || HasKeyMaps); }
        }

        internal bool HasAttachedToolbars
        {
            get { return (mAttachedToolbars != null) && (mAttachedToolbars.Length > 0); }
        }

        internal bool HasAllocatedCommands
        {
            get { return (mAllocatedCommands != null) && (mAllocatedCommands.Count > 0); }
        }

        internal bool HasKeyMaps
        {
            get { return (mKeyMaps != null) && (mKeyMaps.Count > 0); }
        }

        /// <summary>
        /// Gets the number of document versions that was stored in the DOC document.
        /// </summary>
        /// <remarks>
        /// <p>Versions in Microsoft Word are accessed via the File/Versions menu. Microsoft Word supports
        /// versions only for DOC files.</p>
        ///
        /// <p>This property allows to detect if there were document versions stored in this document
        /// before it was opened in Aspose.Words. Aspose.Words provides no other support for document versions.
        /// If you save this document using Aspose.Words, the document will be saved without versions.</p>
        /// </remarks>
        public int VersionsCount
        {
            get { return DocPr.VersionsCount; }
        }

        /// <summary>
        /// Gets or sets the interval (in points) between the default tab stops.
        /// </summary>
        /// <seealso cref="TabStopCollection"/><seealso cref="TabStop"/>
        public double DefaultTabStop
        {
            get { return ConvertUtilCore.TwipToPoint(DocPr.DefaultTabStop); }
            set
            {
                // According to 2.15.1.24 (defaultTabStop) of the specification, the default tab stop should be a positive
                // whole number. But it looks like MS Word also supports and uses zero as default tab stop.
                if (value < 0)
                    throw new ArgumentOutOfRangeException("value");

                DocPr.DefaultTabStop = ConvertUtilCore.PointToTwip(value);
            }
        }

        /// <summary>
        /// Gets the <see cref="Theme"/> object for this document.
        /// </summary>
        public Theme Theme
        {
            get
            {
                InitializeThemeIfNeeded();
                return mTheme;
            }
        }

        private void InitializeThemeIfNeeded()
        {
                if(GetThemeInternal() == null)
                    SetThemeInternal(Theme.BuiltInTheme.Clone());
        }

        internal override Theme GetThemeInternal()
        {
            return mTheme;
        }

        internal void SetThemeInternal(Theme theme)
        {
            mTheme = theme;
            // WORDSNET-15915 Attach document to update colors when theme is changed.
            if(mTheme != null)
            mTheme.Attach(this);
        }

        /// <summary>
        /// Gets or sets the collection of Custom XML Data Storage Parts.
        /// </summary>
        /// <remarks>
        /// <para>Aspose.Words loads and saves Custom XML Parts into OOXML and DOC documents only.</para>
        ///
        /// <para>This property cannot be <c>null</c>.</para>
        ///
        /// <seealso cref="CustomXmlPart"/>
        /// </remarks>
        public CustomXmlPartCollection CustomXmlParts
        {
            get { return mCustomXmlParts; }
            set
            {
                ArgumentUtil.CheckNotNull(value, "customXmlParts");
                mCustomXmlParts = value;
            }
        }

        /// <summary>
        /// Gets or sets the collection of custom parts (arbitrary content) that are linked to the OOXML package using "unknown relationships".
        /// </summary>
        /// <remarks>
        /// <para>Do not confuse these custom parts with Custom XML Data. If you need to access Custom XML parts,
        /// use the <see cref="CustomXmlParts"/> property.</para>
        ///
        /// <para>This collection contains OOXML parts whose parent is the OOXML package and they targets are of an "unknown relationship".
        /// For more information see <see cref="CustomPart"/>.</para>
        ///
        /// <para>Aspose.Words loads and saves custom parts into OOXML documents only.</para>
        ///
        /// <para>This property cannot be <c>null</c>.</para>
        ///
        /// <seealso cref="CustomPart"/>
        /// </remarks>
        public CustomPartCollection PackageCustomParts
        {
            get { return mPackageCustomParts; }
            set
            {
                ArgumentUtil.CheckNotNull(value, "packageCustomParts");
                mPackageCustomParts = value;
            }
        }

        internal byte[] DropdownStrings
        {
            get { return mDropdownStrings; }
            set { mDropdownStrings = value; }
        }

        /// <summary>
        /// Corresponds to the CompObj stream sometimes present in the document. Can be <c>null</c>.
        /// </summary>
        internal MemoryStream CompObj
        {
            get { return mCompObj; }
            set { mCompObj = value; }
        }

        /// <summary>
        /// MsoEnvelope data. See [MS-OSHARED] section 2.3.8.1.
        /// </summary>
        /// <remarks>
        /// Customer just wants this data to be preserved so don't waste time to parse it for a while.
        /// </remarks>
        internal byte[] MsoEnvelope
        {
            get { return mMsoEnvelope; }
            set { mMsoEnvelope = value; }
        }

        /// <summary>
        /// Contains the binary data with the custom toolbars.
        /// Can be null.
        /// </summary>
        internal byte[] AttachedToolbars
        {
            get { return mAttachedToolbars; }
            set { mAttachedToolbars = value; }
        }

        /// <summary>
        /// Contains <see cref="AllocatedCommand"/> objects.
        /// The order is important because it matches the order in the binary DOC.
        /// Do not delete or insert items here unless you are loading a document.
        /// Can be null.
        /// </summary>
        internal IList<AllocatedCommand> AllocatedCommands
        {
            get { return mAllocatedCommands; }
            set { mAllocatedCommands = value; }
        }

        /// <summary>
        /// Contains <see cref="KeyMap"/> objects.
        /// Can be null.
        /// </summary>
        internal IList<KeyMap> KeyMaps
        {
            get { return mKeyMaps; }
            set { mKeyMaps = value; }
        }

        /// <summary>
        /// Gets or sets the flags that specify what document events are active.
        /// When an event is turned on, the corresponding macro must exist in the VBA project.
        /// </summary>
        internal VbaDocumentEvents VbaDocumentEvents
        {
            get { return mVbaDocumentEvents; }
            set { mVbaDocumentEvents = value; }
        }

        /// <summary>
        /// Returns the collection of variables added to a document or template.
        /// </summary>
        public new VariableCollection Variables
        {
            get { return base.Variables; }
        }

        /// <summary>
        /// Gets or sets the glossary document within this document or template. A glossary document is a storage
        /// for AutoText, AutoCorrect and Building Block entries defined in a document.
        /// </summary>
        /// <remarks>
        /// <para>This property returns <c>null</c> if the document does not have a glossary document.</para>
        /// <para>You can add a glossary document to a document by creating a
        /// <see cref="Aspose.Words.BuildingBlocks.GlossaryDocument"/> object and assigning to this property.</para>
        ///
        /// <seealso cref="Aspose.Words.BuildingBlocks.GlossaryDocument"/>
        /// </remarks>
        public GlossaryDocument GlossaryDocument
        {
            get { return mGlossary; }
            set
            {
                mGlossary = value;
                Document mainDocument = this;
                mGlossary.MainDocument = mainDocument;
                mGlossary = CodePorting.Translator.Cs2Cpp.MemoryManagement.ExtendLifetime(mGlossary, mainDocument);
            }
        }

        /// <summary>
        /// Base URI of the document.
        /// </summary>
        /// <remarks>
        /// <p>The <b>BaseUri</b> property is used to resolve relative URI of images when importing or
        /// inserting HTML and also when exporting to HTML or PDF.</p>
        /// <p>Base URI can have optional protocol, optional host name, optional port and required path parts.</p>
        /// <p>If the document was created blank or loaded from a stream, then <b>BaseUri</b> will have the <c>null</c> value.</p>
        /// <p>If the document was loaded from a local file, then <b>BaseUri</b> will store the name of the folder
        /// that contains the file.</p>
        /// </remarks>
        internal string BaseUri
        {
            get { return mBaseUri; }
            set { mBaseUri = value; }
        }

        /// <summary>
        /// Gets the original file name of the document.
        /// </summary>
        /// <remarks>
        /// <p>Returns <c>null</c> if the document was loaded from a stream or created blank.</p>
        /// </remarks>
        public string OriginalFileName
        {
            get { return mOriginalFileName; }
        }

        internal string SavedFileName
        {
            get { return mSavedFileName; }
        }

        /// <summary>
        /// Gets the format of the original document that was loaded into this object.
        /// </summary>
        /// <remarks>
        /// <para>If you created a new blank document, returns the <see cref="LoadFormat.Doc"/> value.</para>
        /// </remarks>
        public LoadFormat OriginalLoadFormat
        {
            get { return mOriginalLoadFormat; }
        }

        /// <summary>
        /// Gets the OOXML compliance version determined from the loaded document content.
        /// Makes sense only for OOXML documents.
        /// </summary>
        /// <remarks>
        /// <para>If you created a new blank document or load non OOXML document
        /// returns the <see cref="OoxmlCompliance.Ecma376_2006"/> value.</para>
        /// </remarks>
        public OoxmlCompliance Compliance
        {
            get
            {
                if (ComplianceInfo == null)
                    return OoxmlCompliance.Ecma376_2006;

                switch (ComplianceInfo.Compliance)
                {
                    case OoxmlComplianceCore.Ecma376:
                        return OoxmlCompliance.Ecma376_2006;
                    case OoxmlComplianceCore.IsoTransitional:
                        return OoxmlCompliance.Iso29500_2008_Transitional;
                    case OoxmlComplianceCore.IsoStrict:
                        return OoxmlCompliance.Iso29500_2008_Strict;
                    default:
                        return OoxmlCompliance.Ecma376_2006;
                }
            }
        }

        /// <summary>
        /// Gets the collection of digital signatures for this document and their validation results.
        /// </summary>
        /// <remarks>
        /// <para>This collection contains digital signatures that were loaded from the original document.
        /// These digital signatures will not be saved when you save this <see cref="Aspose.Words.Document"/> object
        /// into a file or stream because saving or converting will produce a document that is different from the
        /// original and the original digital signatures will no longer be valid.</para>
        ///
        /// <para>This collection is never <c>null</c>. If the document is not signed, it will contain zero elements.</para>
        /// </remarks>
        public DigitalSignatureCollection DigitalSignatures
        {
            get { return mDigitalSignatures; }
        }

        /// <summary>
        /// Gets or sets document font settings.
        /// </summary>
        /// <remarks>
        /// <para>This property allows to specify font settings per document. If set to <c>null</c>, default static font settings
        /// <see cref="Aspose.Words.Fonts.FontSettings.DefaultInstance"/> will be used.</para>
        ///
        /// <para>The default value is <c>null</c>.</para>
        /// </remarks>
        public FontSettings FontSettings
        {
            get { return mFontSettings; }
            set { mFontSettings = value; }
        }

        /// <summary>
        /// Returns the effective font settings instance.
        /// </summary>
        internal FontSettings EffectiveFontSettings
        {
            get { return mFontSettings != null ? mFontSettings : FontSettings.DefaultInstance; }
        }

        /// <summary>
        /// Helps to resolve fonts in this document.
        /// </summary>
        internal override DocumentFontProvider FontProvider
        {
            get
            {
                if(mFontProvider == null)
                    mFontProvider = new DocumentFontProvider(this);
                return mFontProvider;
            }
        }

        /// <summary>
        /// Gets the <see cref="Bibliography"/> object that represents the list of sources available in the document.
        /// </summary>
        public Bibliography.Bibliography Bibliography
        {
            get
            {
                return mBibliography ?? (mBibliography = new Bibliography.Bibliography(this));
            }
        }

        /// <summary>
        /// Returns a <see cref="Frameset"/> instance if this document represents a frames page.
        /// </summary>
        /// <remarks>
        /// If the document is not framed, the property has the <c>null</c> value.
        /// </remarks>
        /// <dev>
        /// Not sure that it is good decision. The main idea that this Frame object represents outermost frame i.e document itself and it
        /// implements proposed public API method Frame.NewAt(NewAtType) which splits this frame into two frames vertically or horizontally.
        /// In details, this frame becomes frameset and two new child frames are created, one is clone of this frameset and second is new one.
        /// Next, each frame can be split again and so on. This behavior is close to MS Word (it has buttons "New frame at top", "New frame at left", etc).
        /// This API guarantees that we will have correct frames tree before saving document. In contrary, if user will has ordinal API like new Frameset, new Frame, AddFrame
        /// we should check that frames tree is correct (there is no frameset with less than two child for example and so on) upon saving.
        ///
        /// This idea has several disadvantages though. For example, Document.Frame members is meaningless if document is not framed document and it can confuse user.
        /// </dev>
        public Frameset Frameset
        {
            get { return mFrame; }
            internal set { mFrame = value; }
        }

        /// <summary>
        /// Specifies whether to include textboxes, footnotes and endnotes in word count statistics.
        /// </summary>
        public bool IncludeTextboxesFootnotesEndnotesInStat
        {
            get { return !DocPr.DoNotIncludeSubDocsInStats; }
            set { DocPr.DoNotIncludeSubDocsInStats = !value; }
        }

        internal int GetNextTocEntryBookmarkIndex()
        {
            return mTocEntryBookmarkIndex++;
        }

        internal override Node Clone(bool isCloneChildren, INodeCloningListener cloningListener)
        {
            Document lhs = (Document)base.Clone(isCloneChildren, cloningListener);

            // Clone only objects that can be changed during the document lifetime.
            lhs.mBuiltInDocumentProperties = (BuiltInDocumentProperties)mBuiltInDocumentProperties.Clone();

            lhs.mCustomDocumentProperties = (CustomDocumentProperties)CustomDocumentProperties.Clone();
            lhs.mCustomDocumentProperties.SetDocument(lhs);

            if (mGlossary != null)
                lhs.GlossaryDocument = (GlossaryDocument)mGlossary.Clone(true, cloningListener);

            if (mTheme != null)
                lhs.mTheme = mTheme.Clone();

            if (ComplianceInfo != null)
                lhs.ComplianceInfo = ComplianceInfo.Clone();

            lhs.mCustomXmlParts = mCustomXmlParts.Clone();
            lhs.mPackageCustomParts = mPackageCustomParts.Clone();

            if (mBibliography != null)
                lhs.mBibliography = mBibliography.Clone(lhs);

            // WORDSNET-28870 We need to create a copy of CompObj to save a cloned document in a thread-safe manner.
            if (mCompObj != null)
            {
                lhs.mCompObj = new MemoryStream();
                mCompObj.WriteTo(lhs.mCompObj);
            }

            // These objects we do not change therefore we do not deep copy them.
            //
            // DigitalSignatures
            // Frames
            //
            // DropdownStrings
            // Cmds
            // CompObj
            //
            // AttachedToolbars
            // AllocatedCommands
            //
            // OriginalFileName and BaseUri are immutable and don't need to be copied.

            if (mVbaProject != null)
                lhs.mVbaProject = mVbaProject.Clone();

            // Facade objects are reset to null.
            lhs.mSectionsCache = null;
            lhs.mFootnoteOptionsCache = null;
            lhs.mEndnoteOptionsCache = null;
            lhs.mRevisionsCache = null;
#if DEBUG
            // DocumentFontProvider.RequestedFontsCache is required to verify fonts used inside the document.
            // Please see TestFontsUtil.VerifyFonts method.
            if (lhs.mFontProvider != null)
                lhs.mFontProvider = mFontProvider.Clone();
#else
            lhs.mFontProvider = null;
#endif

            lhs.mFieldNumListLabels = null;

            if (mEditSession != null)
                lhs.mEditSession = new EditSession(mEditSession.Author, mEditSession.DateTime);

            // SDT nodes are cloned before cloning custom XML parts, so we need reassign parts here.
            lhs.ResolveSdtXmlMapping();

            lhs.DocPr.Rsids = DocPr.Rsids.Clone();

            if (lhs.mMappedCustomXmlUpdater != null)
                lhs.AttachMappedCustomXmlUpdater();

            return lhs;
        }

        /// <summary>
        /// Performs a deep copy of the <see cref="Document"/>.
        /// </summary>
        /// <returns>The cloned document.</returns>
        /// <dev>Kept to remain compatible with the old API.</dev>
        public Document Clone()
        {
            Document newDoc =  (Document)Clone(true);
            return newDoc;
        }

        /// <summary>
        /// Reassigns custom XML parts in SDT XML mapping.
        /// </summary>
        private void ResolveSdtXmlMapping()
        {
            NodeCollection sdts = GetChildNodes(NodeType.StructuredDocumentTag, true);
            foreach (StructuredDocumentTag sdt in sdts)
                sdt.XmlMapping.ResolveCustomXmlPart();
        }

        /// <include file='..\Docs\Text.xml' path='Topics/Topic[@name="Node.Accept"]/*'/>
        /// <remarks>
        /// Calls <see cref="DocumentVisitor.VisitDocumentStart"/>, then calls <see cref="Node.Accept"/> for all child nodes of the document
        /// and calls <see cref="DocumentVisitor.VisitDocumentEnd"/> at the end.
        /// </remarks>
        public override bool Accept(DocumentVisitor visitor)
        {
            return AcceptCore(visitor);
        }

        /// <summary>
        /// Accepts a visitor for visiting the start of the document.
        /// </summary>
        /// <param name="visitor">The document visitor.</param>
        /// <returns>The action to be taken by the visitor.</returns>
        public override VisitorAction AcceptStart(DocumentVisitor visitor)
        {
            return visitor.VisitDocumentStart(this);
        }

        /// <summary>
        /// Accepts a visitor for visiting the end of the document.
        /// </summary>
        /// <param name="visitor">The document visitor.</param>
        /// <returns>The action to be taken by the visitor.</returns>
        public override VisitorAction AcceptEnd(DocumentVisitor visitor)
        {
            return visitor.VisitDocumentEnd(this);
        }

        /// <summary>
        /// Appends the specified document to the end of this document.
        /// </summary>
        /// <param name="srcDoc">The document to append.</param>
        /// <param name="importFormatMode">Specifies how to merge style formatting that clashes.</param>
        public void AppendDocument(Document srcDoc, ImportFormatMode importFormatMode)
        {
            AppendDocument(srcDoc, importFormatMode, null);
        }

        /// <summary>
        /// Appends the specified document to the end of this document.
        /// </summary>
        /// <param name="srcDoc">The document to append.</param>
        /// <param name="importFormatMode">Specifies how to merge style formatting that clashes.</param>
        /// <param name="importFormatOptions">Allows to specify options that affect formatting of a result document.</param>
        public void AppendDocument(Document srcDoc, ImportFormatMode importFormatMode, ImportFormatOptions importFormatOptions)
        {
            // WORDSNET-22103 Reworked to use DocumentInserter instead of pure NodeImporter for appending documents.
            DocumentInserter documentInserter = new DocumentInserter(this);
            documentInserter.AppendDocument(srcDoc, importFormatMode, importFormatOptions);
        }

        /// <summary>
        /// Merges the specified documents to the end of this document.
        /// </summary>
        /// <remarks>
        /// Tries to keep the documents as they looked before the merge.
        /// </remarks>
        internal void MergeDocuments(params Document[] srcDocs)
        {
            DocumentMerger.MergeDocuments(this, srcDocs);
        }

        /// <overloads>Saves the document.</overloads>
        /// <summary>
        /// Saves the document to a file. Automatically determines the save format from the extension.
        /// </summary>
        /// <param name="fileName">The name for the document. If a document with the
        /// specified file name already exists, the existing document is overwritten.</param>
        /// <returns>Additional information that you can optionally use.</returns>
        public SaveOutputParameters Save(string fileName)
        {
            return Save(fileName, null);
        }

        /// <summary>
        /// Saves the document to a file in the specified format.
        /// </summary>
        /// <param name="fileName">The name for the document. If a document with the
        /// specified file name already exists, the existing document is overwritten.</param>
        /// <param name="saveFormat">The format in which to save the document.</param>
        /// <returns>Additional information that you can optionally use.</returns>
        public SaveOutputParameters Save(string fileName, SaveFormat saveFormat)
        {
            return Save(fileName, SaveOptions.CreateSaveOptions(saveFormat));
        }

        /// <summary>
        /// Saves the document to a file using the specified save options.
        /// </summary>
        /// <param name="fileName">The name for the document. If a document with the
        /// specified file name already exists, the existing document is overwritten.</param>
        /// <param name="saveOptions">Specifies the options that control how the document is saved. Can be <c>null</c>.</param>
        /// <returns>Additional information that you can optionally use.</returns>
        public SaveOutputParameters Save(string fileName, SaveOptions saveOptions)
        {
            if (saveOptions != null && !saveOptions.IsMultipleMainPartsAllowed)
                ArgumentUtil.CheckHasChars(fileName, "fileName");

            return Save(null, fileName, saveOptions);
        }

        /// <summary>
        /// Saves the document to a stream or file using the specified save options.
        /// </summary>
        /// <param name="stream">Stream where to save the document.
        /// Can be <c>null</c>.</param>
        /// <param name="fileName">The name for the document. If a document with the
        /// specified file name already exists, the existing document is overwritten.
        /// Can be <c>null</c>.</param>
        /// <param name="saveOptions">Specifies the options that control how the document is saved. Can be <c>null</c>.</param>
        /// <returns>Additional information that you can optionally use.</returns>
        private SaveOutputParameters Save(Stream stream, string fileName, SaveOptions saveOptions)
        {
            if (saveOptions == null)
            {
                saveOptions = (fileName == null)
                    ? new OoxmlSaveOptions()
                    : SaveOptions.CreateSaveOptions(fileName);
            }

            SaveInfo saveInfo = new SaveInfo(this, stream, fileName, saveOptions);

            // FOSS

            DocumentValidator validator = null;
            if (saveOptions.IsFlowFormat)
            {
                // We call validator here only when saving into flow formats because for
                // fixed page formats the validator has already been called by the layout engine.
                validator = new DocumentValidator();
                validator.Execute(saveInfo);

                saveInfo.ShapeToConvertedOfficeMath = validator.ShapeToConvertedOfficeMath;
            }

            SaveOutputParameters result;
            if (!saveOptions.IsMultipleMainPartsAllowed)
            {
                // "Classic" mode with one main output file.
                if (stream == null && !StringUtil.HasChars(fileName))
                    throw new ArgumentException("The arguments Stream and fileName cannot be null or empty string.");

                if (stream == null)
                {
                    string directory = Path.GetDirectoryName(fileName);
                    if (StringUtil.HasChars(directory) && !Directory.Exists(directory))
                    {
                        Directory.CreateDirectory(directory);
                    }

                    // FOSS
                    {
                        using (stream = File.Create(fileName))
                        {
                            saveInfo.Stream = stream;
                            result = SaveCore(saveInfo);
                        }
                    }
                }
                else
                {
                    result = SaveCore(saveInfo);
                }
            }
            else
            {
                result = SaveCore(saveInfo);
            }

            if (validator != null)
                validator.Revert();

            return result;
        }

        private void SetupLoadWarningCallback(LoadOptions loadOptions)
        {
            if (loadOptions == null)
                return;

            // INSP DD This code deserves a comment.
            if (loadOptions.WarningCallback != null)
                WarningCallback = loadOptions.WarningCallback;
            else if (WarningCallback != null)
                loadOptions.WarningCallback = WarningCallback;
        }

        /// <summary>
        /// Saves the document to a stream using the specified format.
        /// </summary>
        /// <param name="stream">Stream where to save the document.</param>
        /// <param name="saveFormat">The format in which to save the document.</param>
        /// <returns>Additional information that you can optionally use.</returns>
        /// <javaName>void save(java.io.OutputStream outputStream, int saveFormat)</javaName>
#if PLAIN_JAVA
        //JAVA-added public wrapper for internalized member
        public SaveOutputParameters save(java.io.OutputStream stream, /*SaveFormat*/int saveFormat) throws Exception
        {
            MemoryStream tempStream = new MemoryStream();
            SaveOutputParameters result = save(tempStream, saveFormat);
            tempStream.setPosition(0);
            com.aspose.ms.java.IO.JavaOnlyStreamUtil.copyStream(tempStream, stream);
            return result;
        }
#endif
        [JavaInternal]
        public SaveOutputParameters Save([CppIOStreamWrapper(IOStreamType.OStream)]Stream stream, SaveFormat saveFormat)
        {
            return Save(stream, SaveOptions.CreateSaveOptions(saveFormat));
        }

        /// <summary>
        /// Saves the document to a stream using the specified save options.
        /// </summary>
        /// <param name="stream">Stream where to save the document.</param>
        /// <param name="saveOptions">Specifies the options that control how the document is saved. Can be <c>null</c>.
        /// If this is <c>null</c>, the document will be saved in the binary DOC format.</param>
        /// <returns>Additional information that you can optionally use.</returns>
        /// <javaName>void save(java.io.OutputStream outputStream, com.aspose.words.SaveOptions saveOptions)</javaName>
#if PLAIN_JAVA
        //JAVA-added public wrapper for internalized member
        public SaveOutputParameters save(java.io.OutputStream stream, SaveOptions saveOptions) throws Exception
        {
            MemoryStream tempStream = new MemoryStream();
            SaveOutputParameters result = save(tempStream, saveOptions);
            tempStream.setPosition(0);
            com.aspose.ms.java.IO.JavaOnlyStreamUtil.copyStream(tempStream, stream);
            return result;
        }
#endif
        [JavaInternal]
        public SaveOutputParameters Save([CppIOStreamWrapper(IOStreamType.OStream)]Stream stream, SaveOptions saveOptions)
        {
            if (!saveOptions.IsMultipleMainPartsAllowed && stream == null)
                throw new ArgumentNullException("stream");

            return Save(stream, null, saveOptions);
        }

        // RK There is no HttpResponse in .NET Framework Client Profile.
#if !NETSTANDARD
        /// <summary>
        /// Sends the document to the client browser.
        /// </summary>
        /// <remarks>
        /// <para>Internally, this method saves to a memory stream first and then copies to the response stream
        /// because the response stream does not support seek.</para>
        /// </remarks>
        /// <param name="response">Response object where to save the document.</param>
        /// <param name="fileName">The name for the document that will appear at the client browser.
        /// The name should not contain path.</param>
        /// <param name="contentDisposition">A <see cref="ContentDisposition"/> value that
        /// specifies how the document is presented at the client browser.</param>
        /// <param name="saveOptions">Specifies the options that control how the document is saved. Can be <c>null</c>.</param>
        /// <returns>Additional information that you can optionally use.</returns>
        /// <msonly>Remove this from Java public API.</msonly>
        [JavaDelete("We do not provide saving document to a response stream in Java.")]
        [CodePorting.Translator.Cs2Cpp.CppSkipEntity("We do not provide saving document to a response stream in C++.")]
        public SaveOutputParameters Save(HttpResponse response, string fileName, ContentDisposition contentDisposition, SaveOptions saveOptions)
        {
            if (response == null)
                throw new ArgumentNullException("response");

            ArgumentUtil.CheckHasChars(fileName, "fileName");

            // Let's check against both forward and back slash.
            if (fileName.IndexOfAny(gDirectoryDelimiters) >= 0)
                throw new ArgumentException("Cannot contain path separators.", "fileName");

            if (saveOptions == null)
                saveOptions = SaveOptions.CreateSaveOptions(fileName);

            if (saveOptions.IsMultipleMainPartsAllowed)
                throw new InvalidOperationException(
                    "Document cannot be written to HTTP response in this mode. " +
                    "Please choose another output format or don't request document splitting.");

            response.ClearContent();

            MemoryStream memStream = new MemoryStream();
            SaveOutputParameters result = Save(memStream, null, saveOptions);
            response.ContentType = result.ContentType;

            string disposition = (contentDisposition == ContentDisposition.Attachment) ? "attachment" : "inline";

            // WORDSNET-13135 If file name contains comma (,), "Google chrome" shows the following error:
            // "Duplicate headers received from server ERR_RESPONSE_HEADERS_MULTIPLE_CONTENT_DISPOSITION"
            // The both ('filename' and 'filename*') formats need apply for "Content-Disposition" header to avoid this error.
            string extNameFormat = HttResponseUtil.EscapeMimeParameterValue(fileName, true);
            string oldNameFormat = HttResponseUtil.EscapeMimeParameterValue(fileName, false);
            string headerValue = string.Format("{0}; filename*=UTF-8''{1}; filename={2}", disposition, extNameFormat, oldNameFormat);

            WriteDocToHttResponse(response, memStream.GetBuffer(), (int)memStream.Length, "Content-Disposition", headerValue);

            return result;
        }

        private static readonly char[] gDirectoryDelimiters = new char[] { '/', '\\' };

        /// <summary>
        /// Attach header and send document to client through response object.
        /// </summary>
        /// <param name="response">Response object.</param>
        /// <param name="docData">Presentation of document in bytes.</param>
        /// <param name="dataLength">Length of document presentation in bytes.</param>
        /// <param name="headerName">Header name.</param>
        /// <param name="headerValue">Header value.</param>
        [JavaDelete("We do not provide saving document to a response stream in Java.")]
        protected virtual void WriteDocToHttResponse(HttpResponse response, byte[] docData, int dataLength, string headerName, string headerValue)
        {
            response.AddHeader(headerName, headerValue);
            response.OutputStream.Write(docData, 0, dataLength);
        }
#endif

        /// <summary>
        /// If the document contains no sections, creates one section with one paragraph.
        /// </summary>
        public void EnsureMinimum()
        {
            Styles.EnsureMinimum();

            // Ensure a section.
            Section section = FirstSection;
            if (section == null)
                section = (Section)AppendChild(new Section(this));

            section.EnsureMinimum();
        }

        /// <summary>
        /// Accepts all tracked changes in the document.
        /// </summary>
        /// <remarks>This method is a shortcut for <see cref="RevisionCollection.AcceptAll"/>.</remarks>
        public void AcceptAllRevisions()
        {
            Revisions.AcceptAll();
        }

        /// <overloads>Protects the document from changes.</overloads>
        /// <summary>
        /// Protects the document from changes without changing the existing password or assigns a random password.
        /// </summary>
        /// <remarks>
        /// <p>When a document is protected, the user can make only limited changes,
        /// such as adding annotations, making revisions, or completing a form.</p>
        ///
        /// <p>When you protect a document, and the document already has a protection password,
        /// the existing protection password is not changed.</p>
        ///
        /// <p>When you protect a document, and the document does not have a protection password,
        /// this method assigns a random password that makes it impossible to unprotect the document
        /// in Microsoft Word, but you still can unprotect the document in Aspose.Words as it does not
        /// require a password when unprotecting.</p>
        /// </remarks>
        /// <param name="type">Specifies the protection type for the document.</param>
        public void Protect(ProtectionType type)
        {
            DocPr.DocumentProtection.ProtectOld(type);
            RemoveSectionUnlock(type);
            AutoUpdateTrackRevisions();
        }

        /// <summary>
        /// Protects the document from changes and optionally sets a protection password.
        /// </summary>
        /// <remarks>
        /// <p>When a document is protected, the user can make only limited changes,
        /// such as adding annotations, making revisions, or completing a form.</p>
        ///
        /// <para>Note that document protection is different from write protection.
        /// Write protection is specified using the <see cref="WriteProtection"/>.</para>
        /// </remarks>
        /// <param name="type">Specifies the protection type for the document.</param>
        /// <param name="password">The password to protect the document with.
        /// Specify <c>null</c> or empty string if you want to protect the document without a password.</param>
        public void Protect(ProtectionType type, string password)
        {
            DocPr.DocumentProtection.Protect(type, password);
            RemoveSectionUnlock(type);
            AutoUpdateTrackRevisions();
        }

        /// <overloads>Removes protection from the document.</overloads>
        /// <summary>
        /// Removes protection from the document regardless of the password.
        /// </summary>
        /// <remarks>
        /// <para>This method unprotects the document even if it has a protection password.</para>
        ///
        /// <para>Note that document protection is different from write protection.
        /// Write protection is specified using the <see cref="WriteProtection"/>.</para>
        /// </remarks>
        public void Unprotect()
        {
            DocPr.DocumentProtection.Unprotect();
            AutoUpdateTrackRevisions();
        }

        /// <summary>
        /// Removes protection from the document if a correct password is specified.
        /// </summary>
        /// <remarks>
        /// <para>This method unprotects the document only if a correct password is specified.</para>
        ///
        /// <para>Note that document protection is different from write protection.
        /// Write protection is specified using the <see cref="WriteProtection"/>.</para>
        /// </remarks>
        /// <param name="password">The password to unprotect the document with.</param>
        /// <returns><c>true</c> if a correct password was specified and the document was unprotected.</returns>
        public bool Unprotect(string password)
        {
            bool isPasswordValid = DocPr.DocumentProtection.ValidatePassword(password);
            if (isPasswordValid)
                Unprotect();
            return isPasswordValid;
        }

        /// <summary>
        /// WORDSNET-5515 When turning on revisions only protection, MS Word also automatically turns on track revisions,
        /// so do we now. Otherwise it was possible to freely edit the document.
        /// </summary>
        private void AutoUpdateTrackRevisions()
        {
            // Do not turn off tracking if it already is turned on.
            if(!TrackRevisions)
                TrackRevisions = (ProtectionType == ProtectionType.AllowOnlyRevisions);
        }

        /// <summary>
        /// Implements an earlier approach to table column widths re-calculation that has known issues.
        /// </summary>
        /// <remarks>
        /// The method is deprecated and it will be removed in a few releases.
        /// </remarks>
        [Obsolete("Obsolete, column widths are re-calculated automatically before saving.")] // 2022-03-11 WORDSNET-23539
        public void UpdateTableLayout()
        {
            // WORDSNET-749 Word seems to store incorrect width values if the preferred widths
            // of the cells are percents. In this case, we should recalculate the widths.
            NodeCollection tables = new NodeCollection(this, NodeType.Table, true);
            foreach (Table table in tables)
            {
                // Only invoke topmost tables because nested tables are handled by their parent tables.
                if (!table.IsNested)
                    table.UpdateLayout();
            }
        }

        /// <summary>
        /// Updates list labels for all list items in the document.
        /// </summary>
        /// <remarks>
        /// <para>This method updates list label properties such as <see cref="ListLabel.LabelValue"/> and
        /// <see cref="ListLabel.LabelString"/> for each <see cref="Paragraph.ListLabel"/> object in the document.</para>
        /// <para>Also, this method is sometimes implicitly called when updating fields in the document. This is required
        /// because some fields that may reference list numbers (such as TOC or REF) need them be up-to-date.</para>
        /// </remarks>
        public void UpdateListLabels()
        {
            ListLabelUpdater.UpdateListLabels(this);
        }

        /// <summary>
        /// Updates the <see cref="Footnote.ActualReferenceMark"/> property of all footnotes and endnotes in the document.
        /// </summary>
        /// <remarks>
        /// Updating fields (<see cref="Document.UpdateFields"/>) may be necessary to get the correct result.
        /// </remarks>
        public void UpdateActualReferenceMarks()
        {
            FootnoteIndexUpdater.Execute(this);
        }

        /// <summary>
        /// Removes all macros (the VBA project) as well as toolbars and command customizations from the document.
        /// </summary>
        /// <remarks>
        /// <p>By removing all macros from a document you can ensure the document contains no macro viruses.</p>
        /// </remarks>
        public void RemoveMacros()
        {
            // RK We always had this behavior for removing toolbar and command customizations too,
            // but now we parse customizations separately we can provide a separate method to users if needed.
            mAttachedToolbars = null;
            mAllocatedCommands = null;
            mKeyMaps = null;

            mVbaProject = null;
            mVbaDocumentEvents = 0;
        }

        /// <summary>
        /// Updates the values of fields in the whole document.
        /// </summary>
        /// <remarks>
        /// <include file='..\Docs\Text.xml' path='Topics/Topic[@name="UpdateFields"]/*'/>
        ///
        /// <para>Use the <see cref="NormalizeFieldTypes"/> method before fields updating if there were document changes that affected field types.</para>
        /// <para>To update fields in a specific part of the document use <see cref="Range.UpdateFields"/>.</para>
        /// </remarks>
        public void UpdateFields()
        {
            // Field engine tracking is quite complex task so postpone it till later and
            // suspend tracking during this operation.
            using (new SuspendTrackRevisionsDocument(this))
                using (new SuspendMappedCustomXmlUpdateDocument(this))
            {
                Range.UpdateFields();
            }
        }

        /// <summary>
        /// Unlinks fields in the whole document.
        /// </summary>
        /// <remarks>
        /// <para>Replaces all the fields in the whole document with their most recent results.</para>
        /// <para>To unlink fields in a specific part of the document use <see cref="Range.UnlinkFields"/>.</para>
        /// </remarks>
        public void UnlinkFields()
        {
            using (new SuspendTrackRevisionsDocument(this))
                Range.UnlinkFields();
        }

        /// <summary>
        /// Changes field type values <see cref="FieldChar.FieldType"/> of <see cref="FieldStart"/>, <see cref="FieldSeparator"/>, <see cref="FieldEnd"/>
        /// in the whole document so that they correspond to the field types contained in the field codes.
        /// </summary>
        /// <remarks>
        /// <para>Use this method after document changes that affect field types.</para>
        /// <para>To change field type values in a specific part of the document use <see cref="Range.NormalizeFieldTypes"/>.</para>
        /// </remarks>
        public void NormalizeFieldTypes()
        {
            Range.NormalizeFieldTypes();
        }

        /// <summary>
        /// Joins runs with same formatting in all paragraphs of the document.
        /// </summary>
        /// <remarks>
        /// <p>This is an optimization method. Some documents contain adjacent runs with same formatting.
        /// Usually this occurs if a document was intensively edited manually.
        /// You can reduce the document size and speed up further processing by joining these runs.</p>
        ///
        /// <p>The operation checks every <see cref="Paragraph"/> node in the document for adjacent <see cref="Run"/>
        /// nodes having identical properties. It ignores unique identifiers used to track editing sessions of run
        /// creation and modification. First run in every joining sequence accumulates all text. Remaining
        /// runs are deleted from the document.</p>
        /// </remarks>
        /// <returns>Number of joins performed. When <b>N</b> adjacent runs are being joined they count as <b>N - 1</b> joins.</returns>
        public int JoinRunsWithSameFormatting()
        {
            // WORDSNET-27660 Suspend XML update during this run join.
            using (new SuspendMappedCustomXmlUpdateDocument(this))
            {
            NodeCollection paragraphs = GetChildNodes(NodeType.Paragraph, true);
            StringBuilder sb = new StringBuilder(1024);
            int joinCount = 0;

            foreach (Paragraph para in paragraphs)
                joinCount += para.JoinRunsWithSameFormatting(sb);

            return joinCount;
        }
        }

        /// <summary>
        /// Converts formatting specified in table styles into direct formatting on tables in the document.
        /// </summary>
        /// <remarks>
        /// <para>This method exists because this version of Aspose.Words provides only limited support for
        /// table styles (see below). This method might be useful when you load a DOCX or WordprocessingML
        /// document that contains tables formatted with table styles and you need to query formatting of
        /// tables, cells, paragraphs or text.</para>
        ///
        /// <para>This version of Aspose.Words provides limited support for table styles as follows:</para>
        /// <list type="bullet">
        ///
        /// <item>Table styles defined in DOCX or WordprocessingML documents are preserved as table styles
        /// when saving the document as DOCX or WordprocessingML.</item>
        ///
        /// <item>Table styles defined in DOCX or WordprocessingML documents are automatically converted
        /// to direct formatting on tables when saving the document into any other format,
        /// rendering or printing.</item>
        ///
        /// <item>Table styles defined in DOC documents are preserved as table styles when
        /// saving the document as DOC only.</item>
        /// </list>
        /// </remarks>
        public void ExpandTableStylesToDirectFormatting()
        {
            TableFormattingExpander expander = new TableFormattingExpander();
            foreach (Table table in GetChildNodes(NodeType.Table, true))
                expander.Expand(table);
        }

        /// <summary>
        /// Cleans unused styles and lists from the document.
        /// </summary>
        public void Cleanup()
        {
            CleanupOptions options = new CleanupOptions();
            Cleanup(options);
        }

        /// <summary>
        /// Cleans unused styles and lists from the document depending on given <see cref="CleanupOptions" />.
        /// </summary>
        public void Cleanup(CleanupOptions options)
        {
            DocumentCleaner.Execute(this, options);

            if (GlossaryDocument != null)
                DocumentCleaner.Execute(GlossaryDocument, options);
        }

        /// <summary>
        /// Removes external XML schema references from this document.
        /// </summary>
        public void RemoveExternalSchemaReferences()
        {
            XmlNamespaces.Clear();
            XmlSchemaReferences.Clear();
        }

        /// <summary>
        /// Starts automatically marking all further changes you make to the document programmatically as revision changes.
        /// </summary>
        /// <remarks>
        /// <p>If you call this method and then make some changes to the document programmatically,
        /// save the document and later open the document in MS Word you will see these changes as revisions.</p>
        ///
        /// <p>Currently Aspose.Words supports tracking of node insertions and deletions only. Formatting changes are not
        /// recorded as revisions.</p>
        ///
        /// <p>Automatic tracking of changes is supported both when modifying this document through node manipulations
        ///  as well as when using <see cref="DocumentBuilder" /></p>
        ///
        /// <p>This method does not change the <see cref="Document.TrackRevisions" /> option and does not use its value
        /// for the purposes of revision tracking.</p>
        /// </remarks>
        /// <param name="author">Initials of the author to use for revisions.</param>
        /// <param name="dateTime">The date and time to use for revisions.</param>
        /// <seealso cref="StopTrackRevisions"/>
        public void StartTrackRevisions(string author, DateTime dateTime)
        {
            mTrackRevisionsEnabled = true;
            mEditSession = new EditSession(author, dateTime);
        }

        /// <summary>
        /// Starts automatically marking all further changes you make to the document programmatically as revision changes.
        /// </summary>
        /// <remarks>
        /// <p>If you call this method and then make some changes to the document programmatically,
        /// save the document and later open the document in MS Word you will see these changes as revisions.</p>
        ///
        /// <p>Currently Aspose.Words supports tracking of node insertions and deletions only. Formatting changes are not
        /// recorded as revisions.</p>
        ///
        /// <p>Automatic tracking of changes is supported both when modifying this document through node manipulations
        ///  as well as when using <see cref="DocumentBuilder" /></p>
        ///
        /// <p>This method does not change the <see cref="Document.TrackRevisions" /> option and does not use its value
        /// for the purposes of revision tracking.</p>
        /// </remarks>
        /// <param name="author">Initials of the author to use for revisions.</param>
        /// <seealso cref="StopTrackRevisions"/>
        public void StartTrackRevisions(string author)
        {
            StartTrackRevisions(author, DateTime.Now);
        }

        /// <summary>
        /// Stops automatic marking of document changes as revisions.
        /// </summary>
        /// <seealso cref="StartTrackRevisions(string,DateTime)"/>
        public void StopTrackRevisions()
        {
            mTrackRevisionsEnabled = false;
        }

        /// <summary>
        /// Copies styles from the specified template to a document.
        /// </summary>
        /// <remarks>
        /// When styles are copied from a template to a document,
        /// like-named styles in the document are redefined to match the style descriptions in the template.
        /// Unique styles from the template are copied to the document. Unique styles in the document remain intact.
        /// </remarks>
        /// <dev>
        /// AM. This is experimental method for WORDSNET-16783
        /// Customer trying to copy all styles but we don't fully understand his requirements.
        /// He wants preserve style hierarchy and this is not possible if copy styles one by one as
        /// we resolve style in destination collection and update links to based and linked styles
        /// accordingly.
        /// </dev>
        public void CopyStylesFromTemplate(string template)
        {
            // WORDSNET-20391 This resource is defined by user, there is no need to use IResourceLoadingCallback.
            using (Stream stream = SystemPal.OpenStreamFromHref(template))
            {
                Document templateDoc = new Document(stream, null, false);
                CopyStylesFromTemplate(templateDoc);
            }
        }

        /// <summary>
        /// Copies styles from the specified template to a document.
        /// </summary>
        /// <remarks>
        /// When styles are copied from a template to a document,
        /// like-named styles in the document are redefined to match the style descriptions in the template.
        /// Unique styles from the template are copied to the document. Unique styles in the document remain intact.
        /// </remarks>
        public void CopyStylesFromTemplate(Document template)
        {
            Styles.CopyStylesFromTemplate(template.Styles);
        }

        /// <summary>
        /// A collection of <see cref="XmlNamespace"/> objects that represents the entire collection of schemas in the Schema Library.
        /// </summary>
        internal XmlNamespaceCollection XmlNamespaces
        {
            get { return DocPr.XmlNamespaces; }
        }

        /// <summary>
        /// A collection of XMLSchemaReference objects that represent the unique namespaces that are attached to a document.
        /// </summary>
        internal XmlSchemaReferenceCollection XmlSchemaReferences
        {
            get { return DocPr.XmlSchemaReferences; }
        }

        /// <summary>
        /// Current edit session.
        /// </summary>
        internal EditSession EditSession
        {
            get { return mEditSession; }
            set { mEditSession = value; }
        }

        /// <summary>
        /// Maps filename to the base folder (folder of the original document file name).
        /// </summary>
        /// <param name="fileName">Can be relative or absolute path. When this is
        /// absolute path, then it is returned without change.
        /// This also works okay when the file name is a UNC path.</param>
        internal string MapFileName(string fileName)
        {
            string baseFolder = (mBaseUri != null) ? mBaseUri : "";
            //Path.Combine returns fileName without changes if it is an absolute path.
            return Path.GetFullPath(Path.Combine(baseFolder, fileName));
        }

        /// <summary>
        /// Can only add Section nodes to Document.
        /// </summary>
        internal override bool CanInsert(Node newChild)
        {
            return (newChild.NodeType == NodeType.Section);
        }

        /// <summary>
        /// Detects file format and loads the document from a stream.
        /// </summary>
        internal void Load(Stream stream, LoadOptions loadOptions)
        {
            Debug.Assert(stream != null);

            // andrnosk: WORDSNET-8678 Some of customers are complaining because AW cannot load zero size file
            // (Empty docx-files can be created by right-clicking and chose -> New Microsoft Word Document),
            // that is why if the stream is empty we have to load blank document and return.
            if (stream.Length <= 0)
            {
                LoadBlank(loadOptions);
                SetLocaleDefaultsForNewDocument();
                return;
            }

            // If the user gives us no load options, use default.
            if (loadOptions == null)
                loadOptions = new LoadOptions();

            // Setup warning callback.
            if (loadOptions.WarningCallback != null)
                WarningCallback = loadOptions.WarningCallback;

            if (loadOptions.LoadFormat != LoadFormat.Auto)
            {
                // The user has specified the format explicitly. Probably he is hoping to win some time by
                // avoiding auto detection. We do what he wants and attempt to load the format explicitly.
                // But the user might be fooled by the document's extension and specify wrong format
                // and loading will fail. It is too hard to explain to the user he is wrong, so we just
                // silence all exceptions if this attempt fails and perform a proper auto detection.
                long savedPos = stream.Position;
                try
                {
                    LoadCore(stream, loadOptions);
                    return;
                }
                catch
                {
                    // Silence all exceptions. Let the auto-detection proceed.

                    // Original LoadOption object must not be changed during loading.
                    loadOptions = loadOptions.Clone();
                    loadOptions.LoadFormat = LoadFormat.Auto;
                    stream.Position = savedPos;

                    // WORDSNET-22014 Remove loaded nodes to avoid content duplication.
                    ResetState();
                }
            }

            // Load with auto detection.
            LoadCore(stream, loadOptions);
        }

        private void LoadCore(Stream stream, LoadOptions loadOptions)
        {
            Debug.Assert(stream != null);
            Debug.Assert(loadOptions != null);

            try
            {
                // Auto-recover from one frequent user's mistake - attempt to load from a memory stream
                // that was written to just before. Its position will be at the end, reset to start.
                if (stream.Position == stream.Length)
                    stream.Position = 0;

                // The lines copy the data we need from loadOptions into this document instance.
                SetupLoadWarningCallback(loadOptions);

                // FOSS

                // andrnosk: WORDSNET-927 Use ResourceLoadingCallback specified in LoadOptions.
                ResourceLoadingCallback = loadOptions.ResourceLoadingCallback;
                FontSettings = loadOptions.FontSettings;

                string baseUriFromLoadOptions = GetBaseUriFromLoadOptions(loadOptions, mOriginalFileName);
                if (StringUtil.HasChars(baseUriFromLoadOptions))
                {
                    mBaseUri = baseUriFromLoadOptions;
                }

                // Perform file format detection if required.
                FileFormatInfo fileFormatInfo = null;
                if (loadOptions.LoadFormat == LoadFormat.Auto)
                {
                    FileFormatDetector detector = new FileFormatDetector();
                    fileFormatInfo = detector.Detect(stream, loadOptions.Encoding);

                    // WORDSNET-26415 Fallback into WordDocument stream to get actual document content.
                    if (fileFormatInfo.EmbeddedInWordDocument)
                    {
                        FileSystem fs = new FileSystem(stream);
                        stream = (Stream)fs.Root["WordDocument"];
                    }

                    // WORDSNET-24408 Fallback unknown format to TXT depending on input file extension.
                    // WORDSNET-24982, 25109. Word does not fallback to TXT for ZIP-archives.
                    // WORDSNET-27211 Don't fallback to TXT when load format specified explicitly.
                    if ((fileFormatInfo.LoadFormat == LoadFormat.Unknown) && !ZipUtilPal.IsZipFile(stream) &&
                        (mOriginalLoadFormat == LoadFormat.Auto))
                    {
                        // WORDSNET-24603, 24609. Fallback unknown format to TXT for all streams, but not only FileStream.
                        FileStream fileStream = stream as FileStream;
                        string ext = (fileStream != null)
                            ? Path.GetExtension(fileStream.Name).ToLower()
                            : string.Empty;

                        // IN. Actually, .sxw format is opened successfully with Word,
                        // and just cannot be opened in LibreOffice. But I left it for backward compatibility,
                        // as we have corresponding test for that in TestOdtContentReader.TestOpenOfficeWriter().
                        if ((ext != ".docx") && (ext != ".odt") && (ext != ".sxw"))
                        {
                            fileFormatInfo.SetLoadFormat(LoadFormat.Text);
                            fileFormatInfo.SetEncoding(detector.Encoding);
                        }
                    }

                    mOriginalLoadFormat = fileFormatInfo.LoadFormat;
                }
                else
                {
                    // WORDSNET-27211 We here because customer set load format explicitly. Let's just use it.
                    mOriginalLoadFormat = loadOptions.LoadFormat;
                }

                ProgressUtils.WarnWhenProgressUnsupported(
                    mOriginalLoadFormat, loadOptions.WarningCallback, loadOptions.ProgressCallback);

                // FOSS

                IDocumentReader documentReader = ReaderFactory.CreateReader(stream, loadOptions,
                    mOriginalLoadFormat, fileFormatInfo, this);

                using (new SuspendMappedCustomXmlUpdateDocument(this))
                {

                    if (documentReader.IsEncrypted)
                    {
                        Stream decryptedStream = documentReader.Decrypt();
                        // The easiest way to proceed here is to recurse.
                        LoadCore(decryptedStream, loadOptions);
                        // Return, to avoid executing the common code below second time.
                        return;
                    }

                    // FOSS

                    documentReader.Read();

                    if (FileFormatUtil.IsPageSetupApplicable(mOriginalLoadFormat) &&
                        (loadOptions.LanguagePreferences.DefaultEditingLanguage != EditingLanguage.EnglishUS))
                        FirstSection.SetPaperSizeByLanguage(loadOptions.LanguagePreferences.DefaultEditingLanguage);

                    // FOSS

                    PostLoadTasks(loadOptions);

                    if (loadOptions.UpdateDirtyFields)
                        FieldUpdater.UpdateDirtyFields(this);
                }
            }
            catch (Exception e)
            {
                // Cancellation was requested from the progress callback.
                CancellationException cancellationException = e as CancellationException;
                if ((cancellationException != null) && (cancellationException.InnerException != null))
                    throw (Exception)cancellationException.InnerException;

#if CPLUSPLUS
                // Workaround for WORDSCPP-622
                FileFormatUtil.ConvertAndRethrowLoadException(e);
#else
                throw FileFormatUtil.ConvertLoadException(e);
#endif
            }
        }

        /// <summary>
        /// Performs post-loading useful tasks such as untangling bookmarks.
        /// </summary>
        private void PostLoadTasks(LoadOptions loadOptions)
        {
            DocumentPostLoader postLoader = new DocumentPostLoader();
            postLoader.Execute(this, loadOptions, mOriginalLoadFormat);
        }

        /// <summary>
        /// Opens a document stream from the given file path or URI. Uses <see cref="IResourceLoadingCallback"/>,
        /// if specified. Can return <c>null</c>.
        /// </summary>
        /// <remarks>
        /// This overload uses <see cref="IResourceLoadingCallback"/> only if the given file path or URI represents
        /// a href with scheme.
        /// </remarks>
        internal static Stream OpenDocumentStream(string pathOrUri, IResourceLoadingCallback resourceLoadingCallback)
        {
            return OpenDocumentStream(pathOrUri, resourceLoadingCallback, true);
        }

        /// <summary>
        /// Opens a document stream from the given file path or URI. Uses <see cref="IResourceLoadingCallback"/>,
        /// if specified. Can return <c>null</c>.
        /// </summary>
        /// <remarks>
        /// This overload can optionally use <see cref="IResourceLoadingCallback"/> every time it is present or only if
        /// the given file path or URI represents a href with scheme.
        /// </remarks>
        internal static Stream OpenDocumentStream(
            string pathOrUri,
            IResourceLoadingCallback resourceLoadingCallback,
            bool useCallbackIfHrefWithSchemeOnly)
        {
            if (!useCallbackIfHrefWithSchemeOnly || UriUtil.IsHrefWithScheme(pathOrUri))
            {
                if (resourceLoadingCallback != null)
                {
                    ResourceLoadingArgs args = new ResourceLoadingArgs("", pathOrUri, ResourceType.Document);
                    switch (resourceLoadingCallback.ResourceLoading(args))
                    {
                        case ResourceLoadingAction.Default:
                            return SystemPal.OpenStreamFromHref(pathOrUri);
                        case ResourceLoadingAction.Skip:
                            return null;
                        case ResourceLoadingAction.UserProvided:
                            return !args.IsDataEmpty
                                ? new MemoryStream(args.GetData())
                                : null;
                        default:
                            Debug.Fail("Never invoked!");
                            return null;
                    }
                }

                return SystemPal.OpenStreamFromHref(pathOrUri);
            }

            return File.OpenRead(pathOrUri);
        }

        /// <summary>
        /// Loads a blank document from the embedded resource.
        ///
        /// We load a blank document from a resource so the resulting document looks more like created by MS Word.
        /// The blank document resource includes a default fonts table, minimum default styles and also latent styles.
        /// </summary>
        internal void LoadBlank(LoadOptions origLoadOptions)
        {
            Debug.Assert(!HasChildNodes);
            const string resourceName = "Aspose.Words.Resources.Blank.docx";
            using (Stream stream = ResourceUtil.FetchResourceStream(resourceName))
            {
                // alexnosk WORDSNET-19906 Specify LoadFormat.Docx explicitly to skip file format detection to minimize blank document creation time.
                // Since we are sure Blank.docx is not encrypted, it is safe to specify it explicitly.
                LoadOptions blankDocLoadOptions = new LoadOptions();
                blankDocLoadOptions.LoadFormat = LoadFormat.Docx;
                blankDocLoadOptions.IsLoadingBlankDocument = true;

                // WORDSNET-23836 Notify about progress when blank document is loading.
                blankDocLoadOptions.ProgressCallback = (origLoadOptions != null) ? origLoadOptions.ProgressCallback : null;

                Load(stream, blankDocLoadOptions);
            }

            // Make sure the new document does not have some rubbish in the properties.
            BuiltInDocumentProperties.Author = "";
            BuiltInDocumentProperties.Company = "";
            BuiltInDocumentProperties.CreatedTime = DateTime.MinValue;
            BuiltInDocumentProperties.LastSavedBy = "";
            BuiltInDocumentProperties.LastSavedTime = DateTime.MinValue;
            // WORDSNET-969 Platform-specific Creator tag in PDF tests causes separate golds for Java. Use Family instead of Product.
            BuiltInDocumentProperties.NameOfApplication = AssemblyConstants.Family;
            BuiltInDocumentProperties.RevisionNumber = 1;

            // Blank.docx contains some locale identifiers assigned to the styles that we don't want, remove them.
            Styles.RemoveLocaleIdsFromStyles();

            // WORDSNET-6478 We should set this props to false, because we load Doc file, where this prop is true.
            DocPr.CompatibilityOptions.UICompat97To2003 = false;

            // WORDSNET-16234 Resource file blank.doc has this property set to true, so reset it to false by design.
            // Note, we should reset it in two places so they are synchronized.
            FontInfos.EmbedSystemFonts = false;
            DocPr.EmbedSystemFonts = false;
        }

        /// <summary>
        /// Save implementation.
        /// </summary>
        /// <param name="saveInfo"></param>
        private SaveOutputParameters SaveCore(SaveInfo saveInfo)
        {
            SaveOptions saveOptions = saveInfo.SaveOptions;
            Debug.Assert(saveOptions != null);

            // FOSS
            Debug.Assert((saveInfo.Stream != null) || saveOptions.IsMultipleMainPartsAllowed);

            ProgressUtils.WarnWhenProgressUnsupported(saveInfo.SaveFormat, WarningCallback, saveOptions.ProgressCallback);

            mSavedFileName = saveInfo.FileName;

            // WORDSNET-19842 Use UTC time to update properties.
            if (saveOptions.UpdateLastSavedTimeProperty)
                BuiltInDocumentProperties.LastSavedTime = DateTimeUtil.ToUniversalTime(DateTimeUtil.GetNow());

            if (saveOptions.UpdateLastPrintedProperty)
                BuiltInDocumentProperties.LastPrinted = DateTimeUtil.ToUniversalTime(DateTimeUtil.GetNow());

            if (saveOptions.UpdateCreatedTimeProperty)
                BuiltInDocumentProperties.CreatedTime = DateTimeUtil.ToUniversalTime(DateTimeUtil.GetNow());

            if (saveOptions.SetBuiltInThemeIfNull)
            {
                // WORDSNET-6886 Use built-in theme if theme is null.
                InitializeThemeIfNeeded();
            }

            if (!saveOptions.IsFlowFormat && saveOptions.UpdateFields)
                UpdateFieldsBeforeSave();

            if (mBibliography != null)
                mBibliography.Save();

            IDocumentWriter writer = WriterFactory.CreateWriter(saveOptions.SaveFormat);

            // FOSS

            SaveOutputParameters result = writer.SaveToStream(saveInfo);

            return result;
        }

        private void UpdateFieldsBeforeSave()
        {
            using (new SuspendTrackRevisionsDocument(this))
            {
                FieldUpdater.UpdateFieldsBeforeSave(this);
            }
        }

        /// <summary>
        /// Suspends revision tracking.
        /// </summary>
        internal override void SuspendTrackRevisions(SuspendedRevisionTypes revisionTypes)
        {
            switch (revisionTypes)
            {
                case SuspendedRevisionTypes.All:
                    mRevisionTrackingLockCount++;
                    break;
                case SuspendedRevisionTypes.Move:
                    mMoveRevisionTrackingLockCount++;
                    break;
                default:
                    throw new InvalidOperationException();
            }
        }

        /// <summary>
        /// Resumes revision tracking.
        /// </summary>
        internal override void ResumeTrackRevisions(SuspendedRevisionTypes revisionTypes)
        {
            switch (revisionTypes)
            {
                case SuspendedRevisionTypes.All:
                    mRevisionTrackingLockCount--;
                    break;
                case SuspendedRevisionTypes.Move:
                    mMoveRevisionTrackingLockCount--;
                    break;
                default:
                    throw new InvalidOperationException();
            }
        }

        /// <summary>
        /// Updates styles from attached template or path provided in load options.
        /// </summary>
        internal void UpdateStyles(SaveOptions so)
        {
            string path = StringUtil.HasChars(so.DefaultTemplate) ? so.DefaultTemplate : DocPr.AttachedTemplate;

            if (!StringUtil.HasChars(path))
            {
                WarningUtil.Warn(WarningCallback, WarningType.DataLoss,
                    WarningSource.Validator, WarningStrings.EmptyTemplatePath);
                return;
            }

            Document template = null;
            try
            {
                // Resiliently open template to update styles from.
                using (Stream stream = OpenDocumentStream(path, ResourceLoadingCallback))
                {
                    FileFormatDetector detector = new FileFormatDetector();
                    FileFormatInfo fi = detector.Detect(stream);

                    // Log warning when template type is not allowed.
                    if (!fi.IsDocumentTemplate)
                        throw new InvalidOperationException();

                    template = new Document(stream, null, false);
                }
            }
            catch
            {
                WarningUtil.Warn(WarningCallback, WarningType.DataLoss,
                                 WarningSource.Validator, WarningStrings.FailedTemplateLoad, path);
            }

            if (template != null)
                Styles.UpdateFromTemplate(template);
        }

        /// <summary>
        /// Indicates that node changes should be marked instead of real deletion/insertion.
        /// </summary>
        internal override bool IsTrackRevisionsEnabled
        {
            get { return (mRevisionTrackingLockCount == 0) && mTrackRevisionsEnabled; }
        }

        /// <summary>
        /// Indicates whether node movements are tracked as move revisions. If it is <c>false</c>, the movements are
        /// represented as <see cref="RevisionType.Deletion"/> and <see cref="RevisionType.Insertion"/> revisions.
        /// </summary>
        internal override bool IsMoveRevisionsTracked
        {
            get { return IsTrackRevisionsEnabled && (mMoveRevisionTrackingLockCount == 0); }
        }

        /// <summary>
        /// Returns Node by its id, used for debugging.
        /// </summary>
        /// <param name="id">Node string id</param>
        /// <returns></returns>
        internal Node GetNodeById(string id)
        {
            string[] path = id.Split('.');
            Array.Reverse(path);
            CompositeNode node = this;

            for (int i = 0; i < path.Length - 1; i++)
                node = (CompositeNode)node.GetChildNodes(NodeType.Any, false)[Convert.ToInt32(path[i])];

            return node.GetChildNodes(NodeType.Any, false)[Convert.ToInt32(path[path.Length - 1])];
        }

        /// <summary>
        /// Is created on demand, makes sense only for OOXML documents. For other documents it is <c>null</c>.
        /// </summary>
        internal OoxmlComplianceInfo ComplianceInfo { get; set; }

        /// <summary>
        /// Sets defaults, that depend on current locale, to all sections.
        /// </summary>
        private void SetLocaleDefaultsForNewDocument()
        {
            foreach (Section section in Sections)
                section.SectPr.SetLocaleDefaultsForNewDocument();
        }

        /// <summary>
        /// Removes Unlock from the single-sectioned document.
        /// </summary>
        private void RemoveSectionUnlock(ProtectionType type)
        {
            if (type != ProtectionType.AllowOnlyFormFields)
                return;

            // MSW unconditionally removes Unlocked attribute for single section document only.
            if (Sections.Count == 1)
                FirstSection.SectPr.Remove(SectAttr.Unlocked);
        }

        /// <summary>
        /// Returns value indicating whether the document has cover page or not.
        /// </summary>
        internal bool HasCoverPage()
        {
            NodeCollection stds = GetChildNodes(NodeType.StructuredDocumentTag, true);
            foreach (StructuredDocumentTag std in stds)
            {
                if (!std.IsSdtDocPart)
                    continue;

                if (std.ControlProperties == null)
                    continue;

                if (std.BuildingBlockGallery != StructuredDocumentTag.CoverPageBuildingBlockGallery)
                    continue;

                return true;
            }

            return false;
        }

#region Rendering

        /// <summary>
        /// A cache for graphics context.
        /// While rendering complex 3D effects using <see cref="Dml3DEffectsRenderingMode.Advanced"/>,
        /// the graphics context is taken from this cache, if it is not here, a new one is created.
        /// For 1 document, we must have 1 graphic context.
        /// </summary>
        internal StringToObjDictionary<object> OpenGLGraphicsContextCache
        {
            get
            {
                if (mOpenGLGraphicsContextCache == null)
                    mOpenGLGraphicsContextCache = new StringToObjDictionary<object>();

                return mOpenGLGraphicsContextCache;
            }
        }

        /// <summary>
        /// Gets a collection of revisions (tracked changes) that exist in this document.
        /// </summary>
        /// <remarks>
        /// <para>The returned collection is a "live" collection, which means if you remove parts of a document that contain
        /// revisions, the deleted revisions will automatically disappear from this collection.</para>
        /// </remarks>
        public RevisionCollection Revisions
        {
            get
            {
                if (mRevisionsCache == null)
                    mRevisionsCache = new RevisionCollection(this);

                return mRevisionsCache;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to work with the original or revised version of a document.
        /// </summary>
        /// <remarks>
        /// The default value is <b><see cref="Aspose.Words.RevisionsView.Original"/></b>.
        /// </remarks>
        /// <dev>
        /// Currently this option is supported for <see cref="ListLabel.LabelString"/>,
        /// <see cref="ListFormat.ListLevel"/> and <see cref="ListFormat.ListLevelNumber"/> only.
        /// </dev>
        public RevisionsView RevisionsView
        {
            get { return mRevisionsView; }
            set { mRevisionsView = value; }
        }

        /// <summary>
        /// Gets or sets the character spacing adjustment of a document.
        /// </summary>
        public JustificationMode JustificationMode
        {
            get { return DocPr.CharacterSpacingType; }
            set { DocPr.CharacterSpacingType = value; }
        }

        /// <inheritdoc />
        void IWatermarkProvider.Add(Shape watermark)
        {
            // If a section has no headers, it will be linked with the headers of the first section.
            // So, the first section should have headers, and other sections may not have headers.
            FirstSection.AddWatermark(watermark, true);
            for (int i = 1; i < Sections.Count; i++)
                Sections[i].AddWatermark(watermark, false);

            // FOSS
        }

        /// <inheritdoc />
        Shape IWatermarkProvider.Get()
        {
            // WORDSNET-28260 Resilience against completely empty document.
            if (FirstSection == null)
                return null;

            // WORDSNET-24516 Resilience when primary header is missing.
            HeaderFooter headerPrimary = FirstSection.HeadersFooters[HeaderFooterType.HeaderPrimary];
            if (headerPrimary == null)
                return null;

            ShapeCollection shapes = headerPrimary.Shapes;
            foreach (Shape shape in shapes)
            {
                if (shape.IsWatermark)
                    return shape;
            }

            return null;
        }

        /// <inheritdoc />
        void IWatermarkProvider.Remove()
        {
            foreach (Section section in Sections)
            {
                section.RemoveWatermark();
                // FOSS
            }
        }

        #endregion Rendering

        /// <summary>
        /// Attaches an instance of <see cref="MappedCustomXmlUpdater"/> as a node changing callback.
        /// </summary>
        private void AttachMappedCustomXmlUpdater()
        {
            mMappedCustomXmlUpdater = new MappedCustomXmlUpdater(this);
            AddInternalNodeChangingCallback(mMappedCustomXmlUpdater);
        }

        #region Footnote and Endnote Options

        /// <summary>
        /// Provides options that control numbering and positioning of footnotes in this document.
        /// </summary>
        public FootnoteOptions FootnoteOptions
        {
            get
            {
                if (mFootnoteOptionsCache == null)
                    mFootnoteOptionsCache = new FootnoteOptions(this);
                return mFootnoteOptionsCache;
            }
        }

        /// <summary>
        /// Provides options that control numbering and positioning of endnotes in this document.
        /// </summary>
        public EndnoteOptions EndnoteOptions
        {
            get
            {
                if (mEndnoteOptionsCache == null)
                    mEndnoteOptionsCache = new EndnoteOptions(this);
                return mEndnoteOptionsCache;
            }
        }

        /// <summary>
        /// We implement ISectionAttrSource to expose footnote and endnote options in the public API nicely.
        /// These options are ugly in MS Word because global options in docpr are for Word97 documents only.
        /// Options for newer documents are in properties of each section.
        ///
        /// The scenarios we want are as follows:
        ///
        /// 1. Opening and saving without modification preserves all document-wide and section-wide
        /// footnote and endnote options always.
        ///
        /// 2. Opening a document and quering document-wide properties returns document-wide properties.
        /// Modifying document-wide properties modifies both document-wide and properties in each section.
        /// At least this seems to be what MS Word is doing.
        /// </summary>
        object ISectionAttrSource.GetDirectSectionAttr(int key)
        {
            return DocPr.FootnotePr.GetDirectSectionAttr(key);
        }

        object ISectionAttrSource.GetDirectSectionAttr(int key, RevisionsView revisionsView)
        {
            return DocPr.FootnotePr.GetDirectSectionAttr(key, revisionsView);
        }

        object ISectionAttrSource.FetchInheritedSectionAttr(int key)
        {
            return SectPr.FetchDefaultAttr(key);
        }

        object ISectionAttrSource.FetchSectionAttr(int key)
        {
            object value = ((ISectionAttrSource)this).GetDirectSectionAttr(key);
            return (value != null) ? value : ((ISectionAttrSource)this).FetchInheritedSectionAttr(key);
        }

        object ISectionAttrSource.FetchSectionAttr(int key, RevisionsView revisionsView)
        {
            object value = ((ISectionAttrSource)this).GetDirectSectionAttr(key, revisionsView);
            return (value != null) ? value : ((ISectionAttrSource)this).FetchInheritedSectionAttr(key);
        }

        void ISectionAttrSource.SetSectionAttr(int key, object value)
        {
            DocPr.FootnotePr.SetSectionAttr(key, value);

            foreach (Section section in Sections)
                ((ISectionAttrSource)section).SetSectionAttr(key, value);
        }

        void ISectionAttrSource.SetSectionAttr(int key, object value, RevisionsView revisionsView)
        {
            DocPr.FootnotePr.SetAttr(key, value, revisionsView);

            foreach (Section section in Sections)
                ((ISectionAttrSource)section).SetSectionAttr(key, value, revisionsView);
        }

        void ISectionAttrSource.ClearSectionAttrs()
        {
            DocPr.FootnotePr.ClearSectionAttrs();

            foreach (Section section in Sections)
                ((ISectionAttrSource)section).ClearSectionAttrs();
        }

#endregion Footnote and Endnote Options

#region Fields

        /// <summary>
        /// Gets a <see cref="Fields.FieldOptions"/> object that represents options to control field handling in the document.
        /// </summary>
        public FieldOptions FieldOptions
        {
            get
            {
                if (mFieldOptionsCache == null)
                    mFieldOptionsCache = new FieldOptions(this);
                return mFieldOptionsCache;
            }
        }

        /// <summary>
        /// Gets or sets a flag indicating that Microsoft Word will remove all user information from comments, revisions and
        /// document properties upon saving the document.
        /// </summary>
        public bool RemovePersonalInformation
        {
            get { return DocPr.RemovePersonalInformation; }
            set { DocPr.RemovePersonalInformation = value; }
        }

        /// <summary>
        /// Gets or sets a <see cref="VbaProject" />.
        /// </summary>
        public VbaProject VbaProject
        {
            get { return mVbaProject; }
            set { mVbaProject = value; }
        }

        internal void ReadVbaProject(MemoryStorage storage, byte[] signature)
        {
            if (storage != null)
            {
                mVbaProject = new VbaProject(storage);
                mVbaProject.Signature = signature;
            }
        }

        /// <summary>
        /// Gets previously set <see cref="FieldNumListLabel"/> instance for the given field start.
        /// </summary>
        /// <param name="fieldStart"></param>
        /// <returns></returns>
        internal FieldNumListLabel GetFieldNumListLabel(FieldStart fieldStart)
        {
            if (mFieldNumListLabels == null)
                return null;

            FieldNumListLabel value = mFieldNumListLabels.GetValueOrNull(fieldStart);
            return value;
        }

        /// <summary>
        /// Sets the specified <see cref="FieldNumListLabel"/> instance for the given field start.
        /// </summary>
        /// <param name="fieldStart"></param>
        /// <param name="label"></param>
        internal void SetFieldNumListLabel(FieldStart fieldStart, FieldNumListLabel label)
        {
            // Create on the first demand.
            if (mFieldNumListLabels == null)
                mFieldNumListLabels = new Dictionary<FieldStart, FieldNumListLabel>();

            mFieldNumListLabels[fieldStart] = label;
        }

        /// <summary>
        /// Forgets all of the previously set <see cref="FieldNumListLabel"/> instances for every field start
        /// within the document.
        /// </summary>
        internal void ClearFieldNumListLabels()
        {
            mFieldNumListLabels = null;
        }

        internal DateTime CurrentDateTimeCache
        {
            get
            {
                if (mCurrentDateTimeCache == DateTime.MinValue)
                    mCurrentDateTimeCache = DateTimeUtil.GetNow();

                return mCurrentDateTimeCache;
            }
        }

        internal FieldUpdateProgressContext FieldUpdateProgressContext { get; set; }
#endregion

        // RK It is interesting to note that if you clone a document and attempt to save both documents
        // using different threads, it could screw up because some of these unparsed structures are
        // memory streams and they have position, etc that are not safe for multithreading.
        // The risk of this is small, but if this ever happens, I need to store this data in a better way.

        // Word document fields
        private BuiltInDocumentProperties mBuiltInDocumentProperties = new BuiltInDocumentProperties();
        private CustomDocumentProperties mCustomDocumentProperties;
        /// <summary>
        /// Can be null.
        /// </summary>
        private GlossaryDocument mGlossary;
        /// <summary>
        /// Can be null.
        /// </summary>
        private Theme mTheme;
        private CustomXmlPartCollection mCustomXmlParts = new CustomXmlPartCollection();
        private CustomPartCollection mPackageCustomParts = new CustomPartCollection();

        /// <summary>
        /// AM. I avoided to use late creation of signature collection to simplify reader's code.
        /// </summary>
        private readonly DigitalSignatureCollection mDigitalSignatures = new DigitalSignatureCollection();

        /// <summary>
        /// Represents root frame object of this document. Can be null.
        /// </summary>
        private Frameset mFrame;

        // Some unparsed data.
        private byte[] mDropdownStrings;
        private MemoryStream mCompObj;
        private byte[] mMsoEnvelope;

        // MS Word UI customization related.
        private byte[] mAttachedToolbars;
        private IList<AllocatedCommand> mAllocatedCommands;
        private IList<KeyMap> mKeyMaps;

        // VBA related.
        private VbaDocumentEvents mVbaDocumentEvents;
        private VbaProject mVbaProject;

        /// <summary>
        /// The original name of the file from which the document was loaded. Can be null.
        /// </summary>
        private readonly string mOriginalFileName;
        private string mSavedFileName;
        private LoadFormat mOriginalLoadFormat;
        /// <summary>
        /// Supposed to be the folder where the document is stored. Can be null.
        /// </summary>
        private string mBaseUri;

        /// <summary>
        /// Facades, created on demand.
        /// </summary>
        private SectionCollection mSectionsCache;
        private StringToObjDictionary<object> mOpenGLGraphicsContextCache;
        private FootnoteOptions mFootnoteOptionsCache;
        private EndnoteOptions mEndnoteOptionsCache;
        private RevisionCollection mRevisionsCache;
        private Dictionary<FieldStart, FieldNumListLabel> mFieldNumListLabels;
        private int mTocEntryBookmarkIndex = 256000000;
        private FieldOptions mFieldOptionsCache;

        /// <summary>
        /// It seems that in DOC (and in the model) there is an 8 byte prefix before the signature.
        /// 4 bytes length of the signature without the prefix.
        /// 4 bytes - the value 8. Not sure what it is. Maybe length of the prefix.
        /// </summary>
        [CodePorting.Translator.Cs2Cpp.CppConstexpr]
        internal const int VbaSignaturePrefixLength = 8;

        // Shows how long writing to fixed page format takes.
        // This is needed for performance tests.
        internal long FixedPageFormatStoringMilliseconds = 0;

        private bool mTrackRevisionsEnabled;
        private int mRevisionTrackingLockCount;
        private int mMoveRevisionTrackingLockCount;
        private EditSession mEditSession;

        // Initialize DateTime for Java.
        private DateTime mCurrentDateTimeCache = DateTime.MinValue;

        private FontSettings mFontSettings;
        private DocumentFontProvider mFontProvider;

        private RevisionsView mRevisionsView = RevisionsView.Original;

        private readonly TaskPaneCollection mWebExtensionTaskPanes = new TaskPaneCollection();

        private Watermark mWatermark;

        private MappedCustomXmlUpdater mMappedCustomXmlUpdater;
        private Bibliography.Bibliography mBibliography;
    }
}
