// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 25/12/2008 by Roman Korchagin

using Aspose.JavaAttributes;

namespace Aspose.Words.Saving
{
    /// <summary>
    /// Implement this interface if you want to control how Aspose.Words saves images when 
    /// saving a document to HTML. May be used by other formats.
    /// </summary>
    [CodePorting.Translator.Cs2Cpp.CppVirtualInheritance("System.Object")]
    public interface IImageSavingCallback
    {
        /// <summary>
        /// Called when Aspose.Words saves an image to HTML.
        /// </summary>
        [JavaThrows(true)]
        void ImageSaving(ImageSavingArgs args);
    }
}
