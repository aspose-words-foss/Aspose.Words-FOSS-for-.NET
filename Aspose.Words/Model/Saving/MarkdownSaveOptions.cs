// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 15/04/2019 by Ilya Navrotskiy

using System;
using Aspose.Words.RW;

namespace Aspose.Words.Saving
{
    /// <summary>
    /// Class to specify additional options when saving a document into the <see cref="Aspose.Words.SaveFormat.Markdown"/> format.
    /// <para>To learn more, visit the <a href="https://docs.aspose.com/words/net/specify-save-options/">Specify Save Options</a> documentation article.</para>
    /// </summary>
    public class MarkdownSaveOptions : TxtSaveOptionsBase
    {
        /// <summary>
        /// Initializes a new instance of this class that can be used to save a document
        /// in the <see cref="Aspose.Words.SaveFormat.Markdown"/> format.
        /// </summary>
        public MarkdownSaveOptions()
        {
            mSaveImageOptions.ScaleImageToShapeSize = true;
        }

        /// <summary>
        /// Specifies the format in which the document will be saved if this save options object is used.
        /// Can only be <see cref="Aspose.Words.SaveFormat.Markdown"/>.
        /// </summary>
        public override SaveFormat SaveFormat
        {
            get { return SaveFormat.Markdown; }
            set
            {
                if (value != SaveFormat.Markdown)
                    throw new ArgumentException("An invalid SaveFormat for this options type was chosen.");
            }
        }

        /// <summary>
        /// Gets or sets a value that specifies how to align contents in tables
        /// when exporting into the <see cref="Aspose.Words.SaveFormat.Markdown"/> format.
        /// The default value is <see cref="Aspose.Words.Saving.TableContentAlignment.Auto"/>.
        /// </summary>
        public TableContentAlignment TableContentAlignment { get; set; }

        /// <summary>
        /// Specifies the physical folder where images are saved when exporting a document to
        /// the <see cref="Aspose.Words.SaveFormat.Markdown"/> format. Default is an empty string.
        /// </summary>
        /// <remarks>
        /// <para>
        /// When you save a <see cref="Document"/> in <see cref="Aspose.Words.SaveFormat.Markdown"/> format,
        /// Aspose.Words needs to save all images embedded in the document as standalone files.
        /// <see cref="ImagesFolder"/> allows you to specify where the images will be saved.
        /// </para>
        /// <para>
        /// If you save a document into a file and provide a file name, Aspose.Words, by default, saves the images in
        /// the same folder where the document file is saved. Use <see cref="ImagesFolder"/> to override this behavior.
        /// </para>
        /// <para>
        /// If you save a document into a stream, Aspose.Words does not have a folder
        /// where to save the images, but still needs to save the images somewhere. In this case,
        /// you need to specify an accessible folder in the <see cref="ImagesFolder"/> property.
        /// </para>
        /// <para>
        /// If the folder specified by <see cref="ImagesFolder"/> doesn't exist, it will be created automatically.
        /// </para>
        /// </remarks>
        public string ImagesFolder
        {
            get { return mImagesFolder; }
            set
            {
                ArgumentUtil.CheckNotNull(value, "ImagesFolder");
                mImagesFolder = value;
            }
        }

        /// <summary>
        /// Specifies the name of the folder used to construct image URIs written into a document.
        /// Default is an empty string.
        /// </summary>
        /// <remarks>
        /// <para>When you save a <see cref="Document"/> in <see cref="Aspose.Words.SaveFormat.Markdown"/> format,
        /// Aspose.Words needs to save all images embedded in the document as standalone files.
        /// <see cref="ImagesFolder"/> allows you to specify where the images will be saved and
        /// <see cref="ImagesFolderAlias"/> allows to specify how the image URIs will be constructed.</para>
        ///
        /// <para>If <see cref="ImagesFolderAlias"/> is not an empty string, then the image URI written
        /// to Markdown will be <i>ImagesFolderAlias + &lt;image file name&gt;</i>.</para>
        ///
        /// <para>If <see cref="ImagesFolderAlias"/> is an empty string, then the image URI written
        /// to Markdown will be <i>ImagesFolder + &lt;image file name&gt;</i>.</para>
        ///
        /// <para>If <see cref="ImagesFolderAlias"/> is set to '.' (dot), then the image file name
        /// will be written to Markdown without path regardless of other options.</para>
        ///
        /// <seealso cref="ImagesFolder"/>
        /// <seealso cref="ImageSavingCallback"/>
        /// </remarks>
        public string ImagesFolderAlias
        {
            get { return mImagesFolderAlias; }
            set
            {
                ArgumentUtil.CheckNotNull(value, "ImagesFolderAlias");
                mImagesFolderAlias = value;
            }
        }

        /// <summary>
        /// Allows to control how images are saved when a document is saved to
        /// <see cref="Aspose.Words.SaveFormat.Markdown"/> format.
        /// </summary>
        public IImageSavingCallback ImageSavingCallback
        {
            get { return mSaveImageOptions.ImageSavingCallback; }
            set { mSaveImageOptions.ImageSavingCallback = value; }
        }

        /// <summary>
        /// Allows to control how resources are saved when a document is exported to
        /// <see cref="Aspose.Words.SaveFormat.Markdown"/> format.
        /// </summary>
        /// <remarks>
        /// Note, there is only one type of resources in Markdown. These are images.
        /// When you specify both <see cref="ImageSavingCallback"/> and <see cref="ResourceSavingCallback"/>,
        /// then first is called <see cref="ResourceSavingCallback"/>. However, note it is not necessary to have both
        /// implementations, as <see cref="ImageSavingArgs"/> is actually a subset of <see cref="ResourceSavingArgs"/>.
        /// </remarks>
        public IResourceSavingCallback ResourceSavingCallback { get; set; }

        /// <summary>
        /// Specifies whether images are saved in Base64 format to the output file.
        /// Default value is <c>false</c>.
        /// </summary>
        /// <remarks>
        /// <para>When this property is set to <c>true</c> images data are exported
        /// directly into the <b>img</b> elements and separate files are not created.</para>
        /// </remarks>
        public bool ExportImagesAsBase64
        {
            get { return mSaveImageOptions.ToBase64; }
            set { mSaveImageOptions.ToBase64 = value; }
        }

        /// <summary>
        /// Specifies how list items will be written to the output file.
        /// Default value is <see cref="MarkdownListExportMode.MarkdownSyntax"/>.
        /// </summary>
        /// <remarks>
        /// <para>When this property is set to <see cref="MarkdownListExportMode.PlainText"/> all list labels are
        /// updated using <see cref="Document.UpdateListLabels"/> and exported with their actual values. Such lists
        /// can be non-compatible with Markdown format and will be recognized as plain text upon importing in this case.</para>
        /// <para>When this property is set to <see cref="MarkdownListExportMode.MarkdownSyntax"/>, writer tries to export
        /// list items in manner that allows to numerate list items in automatic mode by Markdown.</para>
        /// </remarks>
        public MarkdownListExportMode ListExportMode { get; set; }

        /// <summary>
        /// Gets or sets a boolean value indicating either to export underline
        /// text formatting as sequence of two plus characters "++".
        /// The default value is <c>false</c>.
        /// </summary>
        public bool ExportUnderlineFormatting { get; set; }

        /// <summary>
        /// Specifies how links will be written to the output file.
        /// Default value is <see cref="MarkdownLinkExportMode.Auto"/>.
        /// </summary>
        public MarkdownLinkExportMode LinkExportMode { get; set; }

        /// <summary>
        /// Specifies how OfficeMath will be written to the output file.
        /// Default value is <see cref="MarkdownOfficeMathExportMode.Text"/>.
        /// </summary>
        public MarkdownOfficeMathExportMode OfficeMathExportMode { get; set; }

        /// <summary>
        /// Allows to specify the elements to be exported to Markdown as raw HTML.
        /// Default value is <see cref="MarkdownExportAsHtml.None"/>.
        /// </summary>
        public MarkdownExportAsHtml ExportAsHtml { get; set; }

        /// <summary>
        /// Specifies the output resolution for images when exporting to Markdown.
        /// Default is <c>96 dpi</c>.
        /// </summary>
        public int ImageResolution
        {
            get { return mSaveImageOptions.Resolution; }
            set
            {
                ArgumentUtil.CheckPositive(value, "ImageResolution");
                mSaveImageOptions.Resolution = value;
            }
        }

        /// <summary>
        /// Specifies how to export empty paragraphs to Markdown.
        /// Default value is <see cref="MarkdownEmptyParagraphExportMode.EmptyLine"/>.
        /// </summary>
        public MarkdownEmptyParagraphExportMode EmptyParagraphExportMode { get; set; }

        /// <summary>
        /// Specifies how line breaks will be written to the output file.
        /// Default value is <see cref="MarkdownLineBreakExportMode.Backslash"/>.
        /// </summary>
        internal MarkdownLineBreakExportMode LineBreakExportMode { get; set; }

        /// <summary>
        /// Gets SaveImageOptions object.
        /// </summary>
        internal SaveImageOptions SaveImageOptions
        {
            get
            {
                mSaveImageOptions.UseAntiAliasing = UseAntiAliasing;
                return mSaveImageOptions;
            }
        }

        /// <summary>
        /// <see cref="Aspose.Words.SaveFormat.Markdown"/> image saving options.
        /// </summary>
        private readonly SaveImageOptions mSaveImageOptions = new SaveImageOptions();

        /// <summary>
        /// <see cref="Aspose.Words.SaveFormat.Markdown"/> export options.
        /// </summary>
        private string mImagesFolder = string.Empty;
        private string mImagesFolderAlias = string.Empty;
    }
}
