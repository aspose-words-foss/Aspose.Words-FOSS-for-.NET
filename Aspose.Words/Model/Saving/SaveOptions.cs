// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 05/07/2010 by Roman Korchagin

using System;
using System.IO;
using Aspose.Common;
using Aspose.Words.Fonts;
using Aspose.Words.Properties;

namespace Aspose.Words.Saving
{
    /// <summary>
    /// This is an abstract base class for classes that allow the user to specify additional
    /// options when saving a document into a particular format.
    /// <para>To learn more, visit the <a href="https://docs.aspose.com/words/net/specify-save-options/">Specify Save Options</a> documentation article.</para>
    /// </summary>
    /// <remarks>
    /// An instance of the <see cref="SaveOptions"/> class or any derived class is passed to the stream <see cref="Document.Save(Stream,SaveOptions)"/>
    /// or string <see cref="Document.Save(string,SaveOptions)"/> overloads for the user to define custom options when saving a document.
    /// </remarks>
    public abstract class SaveOptions
    {
        /// <summary>
        /// </summary>
        protected SaveOptions()
        {
        }

        /// <summary>
        /// Sets options we use for unit testing.
        /// </summary>
        internal virtual void SetTestMode()
        {
            IsTestMode = true;
            // Pretty format makes it easier to visually compare differences with gold.
            PrettyFormat = true;
            // Do not write generator name, otherwise all golds will have to change with every release.
            ExportGeneratorName = false;
            // Do not write rsid table in test mode to avoid accept too many golds.
            WriteRsidTable = false;
            // Do not update lastprinted property in test mode to avoid accept too many golds.
            UpdateLastPrintedProperty = false;
            // Do not use builtin theme in test mode to avoid accept too many golds (1K+).
            SetBuiltInThemeIfNull = false;
            // Do not write w14:docId in test mode to avoid accept too many golds.
            WriteW14DocId = false;
        }

        /// <summary>
        /// The property helps to branch production and test code.
        ///
        /// Some gold tests demand simplified repeatable output: fixed (not current) date, time, guid, etc.
        /// So we have to branch between test and production code in such cases: test brunch should initialize
        /// the field by fixed value, but production brunch - by the current, non-fixed one.
        ///
        /// But we can't do the branching using DEBUG or TEST preprocessor constants that wrap just few lines of code:
        /// this construction is non-autoportable and causes tricky bugs in java.
        ///
        /// Note: SetTestMode() sets the property to true. TestUtil.Save() sets the property to true too.
        /// </summary>
        internal bool IsTestMode { get; set; }

        /// <summary>
        /// Specifies the format in which the document will be saved if this save options object is used.
        /// </summary>
        public abstract SaveFormat SaveFormat
        {
            get;
            set;
        }

        /// <summary>
        /// When saving to HTML and document splitting is requested we need to slightly change logic:
        /// we allow unspecified file name or stream. So callback interface will give them on demand.
        /// </summary>
        internal virtual bool IsMultipleMainPartsAllowed
        {
            get { return false; }
        }

        /// <summary>
        /// The property must return true if this is a flow document format. False if this is a fixed page (layout and rendering) format.
        /// Most or the formats are flow formats. So let's override in some derived classed for layout-oriented formats.
        /// </summary>
        internal virtual bool IsFlowFormat
        {
            get { return true; }
        }

        /// <summary>
        /// When <c>true</c>, causes the name and version of Aspose.Words to be embedded into produced files.
        /// Default value is <c>true</c>.
        /// </summary>
        /// <dev>
        /// Set to <c>false</c> when testing against gold files so don't have different output when version number changes.
        /// </dev>
        public bool ExportGeneratorName { get; set; } = true;

        /// <summary>
        /// Specifies the folder for temporary files used when saving to a DOC or DOCX file.
        /// By default, this property is <c>null</c> and no temporary files are used.
        /// </summary>
        /// <remarks>
        /// <para>When Aspose.Words saves a document, it needs to create temporary internal structures. By default,
        /// these internal structures are created in memory and the memory usage spikes for a short period while
        /// the document is being saved. When saving is complete, the memory is freed and reclaimed by the garbage collector.</para>
        ///
        /// <para>Specifying a temporary folder using <see cref="TempFolder"/> will cause Aspose.Words to keep the internal structures in
        /// temporary files instead of memory. It reduces the memory usage during saving, but will decrease the save performance.</para>
        ///
        /// <para>The folder must exist and be writable, otherwise an exception will be thrown.</para>
        ///
        /// <para>Aspose.Words automatically deletes all temporary files when saving is complete.</para>
        /// </remarks>
        /// <exception cref="OutOfMemoryException">Throw if you are saving a very large document (thousands of pages) and/or processing many documents at the same time.
        /// The memory spike during saving can be significant enough to cause the exception.</exception>
        public string TempFolder { set; get; }

        /// <summary>
        /// Gets or sets a value determining whether OLE controls presentation image will be updated.
        /// </summary>
        /// <dev>
        /// Actually it looks that MS Word always updates presentation image for OLE controls but
        /// rendering team is not sure that our OLE control rendering feature is ready to do the same.
        ///
        /// This is the reason we make it optional and disable it by default.
        /// </dev>
        public bool UpdateOleControlImages { get; set; }

        /// <summary>
        /// When <c>true</c>, pretty formats output where applicable.
        /// Default value is <c>false</c>.
        /// </summary>
        /// <remarks>
        /// <p>Set to <c>true</c> to make HTML, MHTML, EPUB, WordML, RTF, DOCX and ODT output human readable.
        /// Useful for testing or debugging.</p>
        /// </remarks>
        public bool PrettyFormat { get; set; }

        /// <summary>
        /// Gets or sets a value determining whether or not to use anti-aliasing for rendering.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The default value is <c>false</c>. When this value is set to <c>true</c> anti-aliasing is
        /// used for rendering.
        /// </para>
        /// <para>This property is used when the document is exported to the following formats:
        /// <see cref="SaveFormat.Tiff"/>, <see cref="SaveFormat.Png"/>, <see cref="SaveFormat.Bmp"/>,
        /// <see cref="SaveFormat.Jpeg"/>, <see cref="SaveFormat.Emf"/>. When the document is exported to the
        /// <see cref="SaveFormat.Html"/>, <see cref="SaveFormat.Mhtml"/>,
        /// <see cref="SaveFormat.Epub"/>, <see cref="SaveFormat.Azw3"/>
        /// or <see cref="SaveFormat.Mobi"/> formats this option is used for raster images.</para>
        /// </remarks>
        public bool UseAntiAliasing { get; set; }

        /// <summary>
        /// Gets or sets a value determining whether or not to use high quality (i.e. slow) rendering algorithms.
        /// </summary>
        /// <remarks>
        /// The default value is <c>false</c>.
        /// <para>This property is used when the document is exported to image formats:
        /// <see cref="SaveFormat.Tiff"/>, <see cref="SaveFormat.Png"/>, <see cref="SaveFormat.Bmp"/>,
        /// <see cref="SaveFormat.Jpeg"/>, <see cref="SaveFormat.Emf"/>.</para>
        /// </remarks>
        public bool UseHighQualityRendering { get; set; } = false;

        /// <summary>
        /// Gets or sets a value determining how DrawingML shapes are rendered.
        /// </summary>
        /// <remarks>
        /// The default value is <see cref="DmlRenderingMode.Fallback"/>.
        /// <para>This property is used when the document is exported to fixed page formats.</para>
        /// </remarks>
        public DmlRenderingMode DmlRenderingMode
        {
            get { return mDmlRenderingMode; }
            set { mDmlRenderingMode = value; }
        }
        private DmlRenderingMode mDmlRenderingMode = DmlRenderingMode.DrawingML;

        /// <summary>
        /// Gets or sets a value determining how DrawingML effects are rendered.
        /// </summary>
        /// <remarks>
        /// The default value is <see cref="DmlEffectsRenderingMode.Simplified"/>.
        /// <para>This property is used when the document is exported to fixed page formats.</para>
        /// </remarks>
        public virtual DmlEffectsRenderingMode DmlEffectsRenderingMode
        {
            get { return mDmlEffectsRenderingMode; }
            set { mDmlEffectsRenderingMode = value; }
        }
        private DmlEffectsRenderingMode mDmlEffectsRenderingMode = DmlEffectsRenderingMode.Simplified;

        /// <summary>
        /// Gets or sets a value determining how ink (InkML) objects are rendered.
        /// </summary>
        /// <remarks>
        /// The default value is <see cref="ImlRenderingMode.InkML"/>.
        /// <para>This property is used when the document is exported to fixed page formats.</para>
        /// </remarks>
        public ImlRenderingMode ImlRenderingMode
        {
            get { return mImlRenderingMode; }
            set { mImlRenderingMode = value; }
        }
        private ImlRenderingMode mImlRenderingMode = ImlRenderingMode.InkML;

        /// <summary>
        /// Gets or sets path to default template (including filename).
        /// Default value for this property is <b>empty string</b><ms> (<see cref="string.Empty"/>)</ms>.
        /// </summary>
        /// <remarks>If specified, this path is used to load template when <see cref="Document.AutomaticallyUpdateStyles" /> is <c>true</c>,
        ///  but <see cref="Document.AttachedTemplate" /> is empty.</remarks>
        public string DefaultTemplate
        {
            get { return mDefaultTemplate; }
            set { mDefaultTemplate = value; }
        }
        private string mDefaultTemplate = string.Empty;

        /// <summary>
        /// Gets or sets a value determining if fields of certain types should be updated before saving the document to a fixed page format.
        /// Default value for this property is <c>true</c>.
        /// </summary>
        /// <remarks>
        /// Allows to specify whether to mimic or not MS Word behavior.
        /// </remarks>
        public bool UpdateFields { get; set; } = true;

        /// <summary>
        /// Gets or sets a value determining whether the <see cref="BuiltInDocumentProperties.LastSavedTime" /> property is updated before saving.
        /// </summary>
        public bool UpdateLastSavedTimeProperty { get; set; }

        /// <summary>
        /// Gets or sets a value determining whether the <see cref="BuiltInDocumentProperties.LastPrinted" /> property is updated before saving.
        /// </summary>
        public bool UpdateLastPrintedProperty { get; set; }

        /// <summary>
        /// Gets or sets a value determining whether the <see cref="BuiltInDocumentProperties.CreatedTime" /> property is updated before saving.
        /// Default value is <c>false</c>;
        /// </summary>
        public bool UpdateCreatedTimeProperty { get; set; } = false;

        /// <summary>
        /// Gets or sets value determining if memory optimization should be performed before saving the document.
        /// Default value for this property is <c>false</c>.
        /// </summary>
        /// <remarks>
        /// Setting this option to <c>true</c> can significantly decrease memory consumption while saving large documents at the cost of slower saving time.
        /// </remarks>
        /// <dev>
        /// So far only attribute interning procedure is bound to this property but I think we can make more memory optimization in the future.
        /// That's why I decided to use more general property name.
        /// </dev>
        public bool MemoryOptimization { get; set; } = false;

        /// <summary>
        /// Determines whether the font attributes will be changed according to the character code being used.
        /// </summary>
        public bool UpdateAmbiguousTextFont { get; set; } = false;

        /// <overloads>Creates a save options object of a correct class.</overloads>
        /// <summary>
        /// Creates a save options object of a class suitable for the specified save format.
        /// </summary>
        /// <param name="saveFormat">The save format for which to create a save options object.</param>
        /// <returns>An object of a class that derives from <see cref="SaveOptions"/>.</returns>
        public static SaveOptions CreateSaveOptions(SaveFormat saveFormat)
        {
            switch (saveFormat)
            {
                case SaveFormat.Doc:
                case SaveFormat.Dot:
                    throw new NotSupportedException("FOSS");
                case SaveFormat.Docx:
                case SaveFormat.Docm:
                case SaveFormat.Dotx:
                case SaveFormat.Dotm:
                case SaveFormat.FlatOpc:
                case SaveFormat.FlatOpcMacroEnabled:
                case SaveFormat.FlatOpcTemplate:
                case SaveFormat.FlatOpcTemplateMacroEnabled:
                    return new OoxmlSaveOptions(saveFormat);
                case SaveFormat.Rtf:
                    throw new NotSupportedException("FOSS");
                case SaveFormat.WordML:
                    return new WordML2003SaveOptions();
                case SaveFormat.Pdf:
                    throw new NotSupportedException("FOSS");
                case SaveFormat.Ps:
                    throw new NotSupportedException("FOSS");
                case SaveFormat.Pcl:
                    throw new NotSupportedException("FOSS");
                case SaveFormat.Xps:
                    throw new NotSupportedException("FOSS");
                case SaveFormat.OpenXps:
                    throw new NotSupportedException("FOSS");
                case SaveFormat.XamlFixed:
                    throw new NotSupportedException("FOSS");
                case SaveFormat.Svg:
                    throw new NotSupportedException("FOSS");
                case SaveFormat.HtmlFixed:
                    throw new NotSupportedException("FOSS");
                case SaveFormat.Html:
                case SaveFormat.Mhtml:
                case SaveFormat.Epub:
                case SaveFormat.Azw3:
                case SaveFormat.Mobi:
                    return new HtmlSaveOptions(saveFormat);
                case SaveFormat.Odt:
                case SaveFormat.Ott:
                    throw new NotSupportedException("FOSS");
                case SaveFormat.Text:
                    return new TxtSaveOptions();
                case SaveFormat.XamlFlow:
                    throw new NotSupportedException("FOSS");
                case SaveFormat.XamlFlowPack:
                    throw new NotSupportedException("FOSS");
                case SaveFormat.Markdown:
                    return new MarkdownSaveOptions();
                case SaveFormat.Emf:
                case SaveFormat.Tiff:
                case SaveFormat.Png:
                case SaveFormat.Bmp:
                case SaveFormat.Jpeg:
                case SaveFormat.Gif:
                case SaveFormat.Eps:
#if NETSTANDARD || NET462_OR_GREATER || JAVA
                case SaveFormat.WebP:
#endif
                    throw new NotSupportedException("FOSS");
                case SaveFormat.Xlsx:
                    throw new NotSupportedException("FOSS");
                case SaveFormat.Docling:
                    throw new NotSupportedException("FOSS");

                default:
                    throw new ArgumentException("Invalid save format requested.");
            }
        }

        /// <summary>
        /// Creates a save options object of a class suitable for the file extension specified in the given file name.
        /// </summary>
        /// <param name="fileName">The extension of this file name determines the class of the save options object to create.</param>
        /// <returns>An object of a class that derives from <see cref="SaveOptions"/>.</returns>
        public static SaveOptions CreateSaveOptions(string fileName)
        {
            // Internal methods are used instead of FileFormatUtil.ExtensionToSaveFormat
            // to get SaveFormat.Unknown instead of an exception.
            return CreateSaveOptions(FileFormatUtil.ToSaveFormat(FileFormatCore.FromExt(Path.GetExtension(fileName))));
        }

        /// <summary>
        /// Gets or sets a value determining how 3D effects are rendered.
        /// </summary>
        /// <remarks>
        /// The default value is <see cref="Dml3DEffectsRenderingMode.Basic"/>.
        /// </remarks>
        public Dml3DEffectsRenderingMode Dml3DEffectsRenderingMode
        {
            get { return mDml3DEffectsRenderingMode; }
            set { mDml3DEffectsRenderingMode = value; }
        }
        private Dml3DEffectsRenderingMode mDml3DEffectsRenderingMode = Dml3DEffectsRenderingMode.Basic;

        /// <summary>
        /// Called during saving a document and accepts data about saving progress.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Progress is reported when saving to <see cref="SaveFormat.Docx"/>, <see cref="SaveFormat.FlatOpc"/>,
        /// <see cref="SaveFormat.Docm"/>, <see cref="SaveFormat.Dotm"/>, <see cref="SaveFormat.Dotx"/>,
        /// <see cref="SaveFormat.Doc"/>, <see cref="SaveFormat.Dot"/>,
        /// <see cref="SaveFormat.Html"/>, <see cref="SaveFormat.Mhtml"/>, <see cref="SaveFormat.Epub"/>,
        /// <see cref="SaveFormat.XamlFlow"/>, or <see cref="SaveFormat.XamlFlowPack"/>.
        /// </para>
        /// </remarks>
        public IDocumentSavingCallback ProgressCallback { get; set; }

        /// <summary>
        /// Gets or sets a boolean value indicating whether to allow embedding fonts with PostScript outlines
        /// when embedding TrueType fonts in a document upon it is saved.
        /// The default value is <c>false</c>.
        /// </summary>
        /// <remarks>
        /// <para>Note, Word does not embed PostScript fonts, but can open documents with embedded fonts of this type.</para>
        /// <para>This option only works when <see cref="FontInfoCollection.EmbedTrueTypeFonts"/> of the
        /// <see cref="DocumentBase.FontInfos"/> property is set to <c>true</c>.</para>
        /// </remarks>
        public bool AllowEmbeddingPostScriptFonts { get; set; }

        /// <summary>
        /// Gets or sets custom local time zone used for date/time fields.
        /// </summary>
        /// <remarks>
        /// <para>This option is available in either .Net framework starting from 3.5 version or .Net Standard.</para>
        /// <para>By default, Aspose.Words uses system local time zone when writes date/time fields, this option allows to set custom value.</para>
        /// </remarks>
        /// <dev>
        /// So far this option used in SDT content updating only. Please see WORDSNET-20863 for details.
        /// We can use it for other document date/time values later.
        /// </dev>
        public TimeZoneInfo CustomTimeZoneInfo
        {
            get { return mCustomTimeZoneInfo; }
            set { mCustomTimeZoneInfo = value; }
        }
        private TimeZoneInfo mCustomTimeZoneInfo = TimeZoneInfo.Local;

        /// <summary>
        /// Determines whether build-in theme should be written if there is no theme in the document.
        /// Default value is true.
        /// </summary>
        internal bool SetBuiltInThemeIfNull { get; set; } = true;

        /// <summary>
        /// Determines whether rsid table should be written. Default value is true.
        /// </summary>
        internal bool WriteRsidTable { get; set; } = true;

        /// <summary>
        /// Determines whether w14:docId should be written in settings part. Default value is true.
        /// </summary>
        internal bool WriteW14DocId { get; set; } = true;

        /// <summary>
        /// Controls whether we need to update XmlMapping before saving.
        /// </summary>
        internal bool UpdateXmlMapping { get; set; } = true;
    }
}
