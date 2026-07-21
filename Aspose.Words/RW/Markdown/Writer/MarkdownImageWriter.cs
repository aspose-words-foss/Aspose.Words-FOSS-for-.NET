// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 02/07/2020 by Mikhail Nepreteamov

using System.IO;
using Aspose.Words.Drawing;
using Aspose.Words.Drawing.Core;
using Aspose.Words.RW.MarkupLanguage.Writer;
using Aspose.Words.Saving;

namespace Aspose.Words.RW.Markdown.Writer
{
    /// <summary>
    /// The class responsible for writing Images into markdown.
    /// </summary>
    internal class MarkdownImageWriter
    {
        /// <summary>
        /// Ctor.
        /// </summary>
        internal MarkdownImageWriter(string fileName, MarkdownSaveOptions saveOptions, MarkdownLinkDefinitionWriter linkDefinitionWriter)
        {
            mSaveOptions = saveOptions;
            mLinkDefinitionWriter = linkDefinitionWriter;

            mImageSavingCallback = (mSaveOptions.ResourceSavingCallback != null)
                ? new SavingCallbackProxy(mSaveOptions.ResourceSavingCallback, mSaveOptions.ImageSavingCallback)
                : mSaveOptions.ImageSavingCallback;

            ImageResourceWriter imageResourceWriter = new ImageResourceWriter(
                fileName,
                null,
                mSaveOptions.ImagesFolder,
                mSaveOptions.ImagesFolderAlias,
                string.Empty,
                false,
                mImageSavingCallback);

            // FOSS
        }

        /// <summary>
        /// Processes a specified Shape.
        /// </summary>
        internal bool OnShape(ShapeBase shapeBase)
        {
            Debug.Assert(shapeBase != null);

            if (!MarkupLanguageShapeWriter.IsShapeRenderable(shapeBase))
                return false;

            mShape = shapeBase;

            // FOSS
            return false;
        }

        /// <summary>
        /// Processes a specified Shape.
        /// </summary>
        internal bool OnShape(Shape shape)
        {
            Debug.Assert(shape != null);

            if (!MarkupLanguageShapeWriter.IsShapeRenderable(shape))
                return false;

            // WORDSNET-22787 We don't want to get NoImage as an output, just leave inaccessible SourceFullName unchanged.
            if ((shape.Document.ResourceLoadingCallback == null) && !IsImageAccessible(shape))
            {
                mShape = shape;

                WarningUtil.Warn(
                    shape.Document.WarningCallback,
                    WarningType.DataLossCategory,
                    WarningSource.Image,
                    string.Format("The image is not available: {0}.", shape.ImageData.SourceFullName));

                mShapeDestination = shape.ImageData.SourceFullName;
                mLinkDefinitionWriter.OnShape(mShape, mShapeDestination);

                return true;
            }

            return OnShape((ShapeBase)shape);
        }

        /// <summary>
        /// Returns true, if image in a specified shape is accessible.
        /// </summary>
        /// <remarks>
        /// The image can be inaccessible due to it is defined as a link only
        /// and the link is not accessible.
        /// </remarks>
        private static bool IsImageAccessible(Shape shape)
        {
            if (shape.ImageData == null)
                return false;

            if (!shape.ImageData.IsLinkOnly)
                return true;

            Document doc = shape.Document as Document;
            if (doc != null)
            {
                try
                {
                    using (Stream stream = ImageDataUtil.OpenStreamUnsafe(shape.ImageData.SourceFullName, doc))
                        return (stream.Length != 0);
                }
                catch
                {
                    // Mute exception
                }
            }

            return false;
        }

        /// <summary>
        /// Gets a string that is ready for writing into markdown.
        /// </summary>
        internal string Text
        {
            get
            {
                string referenceLabel = mLinkDefinitionWriter.GetReferenceString(mShape, mShapeDestination);
                string title = (!string.IsNullOrEmpty(Title))
                    ? string.Format(" \"{0}\"", Title)
                    : string.Empty;

                // Local and remote paths including ImageFolder shall be correct URI paths.
                string uri = mSaveOptions.ExportImagesAsBase64
                    ? mShapeDestination
                    : UriUtil.HrefToUri(mShapeDestination);

                // Exported as Inline if the corresponding option is set or if the reference link could not have been resolved.
                bool isExportAsInline = (mSaveOptions.LinkExportMode == MarkdownLinkExportMode.Inline) ||
                                        (!mLinkDefinitionWriter.HasCollapsedLinkDefinition(mShapeDestination, Title));

                // WORDSNET-28748 Replace some characters with their hexadecimal references in Alt text.
                string altText = MarkdownUtil.ReplaceWithHexRef(mShape.AlternativeText, "\r\n");

                string resultText = string.Format("{0}{1}{2}{3}{4}{5}{6}",
                    ImageDescriptionBlock.OpeningDelimiter,
                    altText, ImageDescriptionBlock.ClosingDelimiter,
                    LinkDestinationBlock.OpeningDelimiter, uri, title, LinkDestinationBlock.ClosingDelimiter);

                if (!isExportAsInline)
                {
                    // Exported as a shortcut if referenceLabel is empty or matches the alternative text of the current Shape.
                    bool isExportAsShortcut = (altText == string.Empty) ||
                                              ((referenceLabel != string.Empty) &&
                                               (referenceLabel.Substring(1, referenceLabel.Length - 2) ==
                                                mShape.AlternativeText));
                    resultText = isExportAsShortcut
                        ? string.Format("!{0}", referenceLabel)
                        : string.Format("{0}{1}{2}{3}", ImageDescriptionBlock.OpeningDelimiter, altText,
                            ImageDescriptionBlock.ClosingDelimiter, referenceLabel);
                }

                return resultText;
            }
        }

        /// <summary>
        /// Gets the current shape title.
        /// </summary>
        private string Title
        {
            get
            {
                Shape shape = mShape as Shape;
                return shape != null
                    ? shape.ImageData.Title
                    : mShape.Title;
            }
        }

        /// <summary>
        /// The writer of the link definitions.
        /// </summary>
        private readonly MarkdownLinkDefinitionWriter mLinkDefinitionWriter;

        /// <summary>
        /// Save options of the source document.
        /// </summary>
        private readonly MarkdownSaveOptions mSaveOptions;

        /// <summary>
        /// The shape.
        /// </summary>
        private ShapeBase mShape;

        /// <summary>
        /// The shape image destination.
        /// </summary>
        private string mShapeDestination;

        /// <summary>
        /// Image saving callback is either <see cref="MarkdownSaveOptions.ImageSavingCallback"/>,
        /// or <see cref="SavingCallbackProxy"/>.
        /// </summary>
        private readonly IImageSavingCallback mImageSavingCallback;

        /// <summary>
        /// The class to proxy <see cref="mResourceSavingCallback"/>.
        /// </summary>
        /// <remarks>
        /// There are only images can be written as external resources in Markdown.
        /// However, defining <see cref="MarkdownSaveOptions.ResourceSavingCallback"/> allows you additionally
        /// to change <see cref="ResourceSavingArgs.ResourceFileUri"/>. So, you can change actual saved file name
        /// to something, like ![](/some/custom/uri).
        /// </remarks>
        private class SavingCallbackProxy : IImageSavingCallback
        {
            internal SavingCallbackProxy(
                IResourceSavingCallback resourceSavingCallback,
                IImageSavingCallback imageSavingCallback)
            {
                mResourceSavingCallback = resourceSavingCallback;
                mImageSavingCallback = imageSavingCallback;
            }

            /// <summary>
            /// Called when Aspose.Words saves an image to Markdown.
            /// </summary>
            public void ImageSaving(ImageSavingArgs args)
            {
                // Process ResourceSavingCallback.
                if (mResourceSavingCallback != null)
                {
                    mArgs = new ResourceSavingArgs(args.Document, args.ImageFileName, "");
                    mResourceSavingCallback.ResourceSaving(mArgs);

                    args.ImageFileName = mArgs.ResourceFileName;
#if PLAIN_JAVA
                    args.setImageStream(mArgs.getResourceStream());
#else
                    args.ImageStream = mArgs.ResourceStream;
#endif
                    args.KeepImageStreamOpen = mArgs.KeepResourceStreamOpen;
                }

                // Process ImageSavingCallback.
                if (mImageSavingCallback != null)
                    mImageSavingCallback.ImageSaving(args);
            }

            /// <summary>
            /// Indicates whether the user has provided a custom URI for the resource file.
            /// </summary>
            internal bool HasUserResourceFileUri
            {
                get { return mArgs.HasUserResourceFileUri; }
            }

            /// <summary>
            /// Gets or sets the uniform resource identifier (URI) used to reference the resource file from the document.
            /// </summary>
            internal string ResourceFileUri
            {
                get { return mArgs.ResourceFileUri; }
            }

            private ResourceSavingArgs mArgs;

            private readonly IResourceSavingCallback mResourceSavingCallback;
            private readonly IImageSavingCallback mImageSavingCallback;
        }
    }
}
