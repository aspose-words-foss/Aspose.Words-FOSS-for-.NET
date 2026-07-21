// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 22/11/2006 by Roman Korchagin

using Aspose.Words.Saving;

namespace Aspose.Words.RW
{
    /// <summary>
    /// Options used when exporting images.
    /// Used internally in HTML, SVG and XAML export modules.
    /// </summary>
    internal class SaveImageOptions
    {
        /// <summary>
        /// Creates a clone of the current <see cref="SaveImageOptions"/> class.
        /// </summary>
        internal SaveImageOptions Clone()
        {
            return (SaveImageOptions)MemberwiseClone();
        }

        /// <summary>
        /// Indicates the format in which metafiles are to be saved.
        /// </summary>
        internal HtmlMetafileFormat MetafileFormat;

        /// <summary>
        /// Causes all images to be scaled to the shape size.
        /// </summary>
        internal bool ScaleImageToShapeSize;

        /// <summary>
        /// If metafiles are converted to raster or images are scaled to shape size,
        /// this is the output resolution for the images.
        /// </summary>
        internal int Resolution = DefaultResolution;

        /// <summary>
        /// Causes all images to be saved in Base64 format.
        /// </summary>
        internal bool ToBase64;

        /// <summary>
        /// When saving allows override standard behavior.
        /// See <see cref="ImageSavingArgs"/>.
        /// </summary>
        internal IImageSavingCallback ImageSavingCallback;

        /// <summary>
        /// Image's zoom factor.
        /// </summary>
        internal float Scale = 1.0f;

        /// <summary>
        /// Gets or sets a value determining whether or not to use anti-aliasing for rendering.
        /// </summary>
        internal bool UseAntiAliasing;

        /// <summary>
        /// Causes all shapes to be saved as SVG markup.
        /// </summary>
        internal bool ShapesAsSvg;

        /// <summary>
        /// Default output resolution for the images.
        /// </summary>
        internal const int DefaultResolution = (int)ImageConstants.StandardResolution;
    }
}
