// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 16/08/2004 by Roman Korchagin

using System.IO;
using Aspose.Common;
using Aspose.Words.Drawing;
using Aspose.Words.Saving;

namespace Aspose.Words.RW.MarkupLanguage.Writer
{
    /// <summary>
    /// Helps to write images from the document model into most of the non-MS markup language formats.
    /// </summary>
    internal class ImageResourceWriter : ResourceWriterBase
    {
        /// <summary>
        /// Call this before building the document.
        /// </summary>
        /// <param name="fileName">Pass the name of the file you want to save to.
        /// Pass null if saving into a stream. Used to extract path and base name for images.</param>
        /// <param name="subsidiaryContentPartCollector">Can be null. If not null, then we collect all subsidiary
        /// parts to be written separately or externally.</param>
        /// <param name="exportImagesFolder">ExportImagesFolder.</param>
        /// <param name="exportImagesFolderAlias">ExportImagesFolderAlias.</param>
        /// <param name="messageWhenNoResourceFolder">Message that is put into exception when we cannot write 
        /// files because we have no clearly specified output.</param>
        /// <param name="isMakeResourceFolderAbsolute"></param>
        /// <param name="imageSavingCallback"></param>
        internal ImageResourceWriter(
            string fileName,
            SubsidiaryContentPartCollector subsidiaryContentPartCollector,
            string exportImagesFolder,
            string exportImagesFolderAlias,
            string messageWhenNoResourceFolder,
            bool isMakeResourceFolderAbsolute,
            IImageSavingCallback imageSavingCallback)
            : base(fileName, subsidiaryContentPartCollector, exportImagesFolder, exportImagesFolderAlias,
                messageWhenNoResourceFolder, false, isMakeResourceFolderAbsolute)
        {
            mImageSavingCallback = imageSavingCallback;
        }

        internal string WriteExternalImage(
            ShapeBase shape,
            byte[] imageBytes,
            int offset,
            int length,
            FileFormat fileFormat,
            bool isImageAvailable)
        {
            string fileExtension = FileFormatCore.ToExt(fileFormat);
            string shortFileName = GetNextImageFileName(fileExtension);
            string fullFileName = GetFullFileName(shortFileName);

            UserStreamWrapper userStreamWrapper = null;
            if (mImageSavingCallback != null)
            {
                ImageSavingArgs args = new ImageSavingArgs(shape, isImageAvailable, shortFileName);
                mImageSavingCallback.ImageSaving(args);
                shortFileName = args.ImageFileName;
                fullFileName = GetFullFileName(shortFileName);

                if (args.HasUserStream)
                    userStreamWrapper = args.CreateUserStreamWrapper();
            }

            if (SubsidiaryContentPartCollector != null)
            {
                // Collect it, don't write files.
                Debug.Assert(Path.GetFileName(shortFileName) == shortFileName);
                SubsidiaryContentPartCollector.AddPart(new BinaryContentPart(
                    FileFormatCore.ToContentType(fileFormat),
                    shortFileName,
                    imageBytes,
                    offset,
                    length));
            }
            else if (userStreamWrapper != null)
            {
                Stream userStream = userStreamWrapper.BeginUserStream();
                userStream.Write(imageBytes, offset, length);
            }
            else
            {
                CreateFolderIfNeeded();
                using (FileStream fileStream = File.Create(fullFileName))
                {
                    fileStream.Write(imageBytes, offset, length);
                }
            }

            if (userStreamWrapper != null)
                userStreamWrapper.EndUserStream();

            return MapResourceFileName(fullFileName);
        }

        /// <summary>
        /// Calls the image saving callback to notify the user that a shape is about to be saved.
        /// </summary>
        internal void NotifyCallback(
            ShapeBase shape,
            FileFormat fileFormat,
            bool isImageAvailable)
        {
            if (mImageSavingCallback == null)
            {
                return;
            }

            string fileExtension = FileFormatCore.ToExt(fileFormat);
            string shortFileName = GetNextImageFileName(fileExtension);
            ImageSavingArgs args = new ImageSavingArgs(shape, isImageAvailable, shortFileName);
            mImageSavingCallback.ImageSaving(args);

            if (args.HasUserStream)
            {
                // Make sure we close the user-provided stream if we are told to do so.
                UserStreamWrapper userStreamWrapper = args.CreateUserStreamWrapper();
                userStreamWrapper.EndUserStream();
            }
        }

        /// <summary>
        /// Generates short (without path) image file name and increments inner counter.
        /// </summary>
        private string GetNextImageFileName(string imageType)
        {
            ++mImageNum;
            return string.Format(
                "{0}.{1}.{2}",
                (SubsidiaryContentPartCollector != null) ? "image" : ResourceBaseName,
                FormatterPal.IntToStrD3(mImageNum),
                imageType);
        }

        /// <summary>
        /// A user-provided callback that is called for each image written.
        /// </summary>
        private readonly IImageSavingCallback mImageSavingCallback;

        /// <summary>
        /// Current image number, used in generation of the unique image file name.
        /// </summary>
        private int mImageNum;
    }
}
