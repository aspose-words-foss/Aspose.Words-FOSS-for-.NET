// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 10/05/2011 by Alexey Titov

using System.IO;
using Aspose.Common;
using Aspose.Images;
using Aspose.IO;
using Aspose.Words.Loading;
using Convert = System.Convert;

namespace Aspose.Words.Drawing.Core
{
    /// <summary>
    /// Helper class containing common methods used to draw image.
    /// </summary>
    internal class ImageDataUtil
    {
        /// <summary>
        /// Load image bytes using ResourceLoadingCallback. 
        /// </summary>
        internal static byte[] LoadImageBytes(string fileName, DocumentBase doc)
        {
            byte[] imageBytes = null;

            if ((doc != null) && (doc.ResourceLoadingCallback != null))
            {
                ResourceLoadingArgs args = new ResourceLoadingArgs("", fileName, ResourceType.Image);
                switch (doc.ResourceLoadingCallback.ResourceLoading(args))
                {
                    case ResourceLoadingAction.UserProvided:
                        imageBytes = args.GetData();
                        break;
                    case ResourceLoadingAction.Default:
                        imageBytes = LoadImageBytesCore(fileName, doc);
                        break;
                    case ResourceLoadingAction.Skip:
                        // If customer would like to skip loading external image, we should show NoImage cross. 
                        imageBytes = ImageUtil.GetNoImageBytes();
                        break;
                    default:
                        break;
                }
            }
            else
            {
                imageBytes = LoadImageBytesCore(fileName, doc);
            }

            return imageBytes;
        }

        /// <summary>
        /// Opens image stream using ResourceLoadingCallback. 
        /// </summary>
        internal static Stream OpenStream(string fileName, DocumentBase doc)
        {
            byte[] imageBytes = LoadImageBytes(fileName, doc);

            return new MemoryStream(imageBytes);
        }

        /// <summary>
        /// Opens stream and copy it to byte array.
        /// Mute exception if any errors are occurred during stream reading.
        /// </summary>
        private static byte[] LoadImageBytesCore(string fileName, DocumentBase doc)
        {
            // WORDSNET-9480 The problem occurred because ImageData.ToByteArray method throws when it cannot read an image bytes.
            // Made code resilient. No-image is returned in case of some problems.
            byte[] imageBytes = null;
            try
            {
                using (Stream stream = OpenStreamUnsafe(fileName, doc as Document))
                    imageBytes = StreamUtil.CopyStreamToByteArray(stream);
            }
            catch
            {
                // Mute exception
            }
            return imageBytes;
        }

        /// <summary>
        /// Opens stream resolving relative paths via the current document (BaseUri of). 
        /// This code won't work properly once we implement loading documents from http etc. 
        /// <see cref="Document.MapFileName" /> and this method should be revised in this case.
        /// Consider using UriUtil.IsAbsoluteHref and UriUtil.CombineHref.
        /// </summary>
        public static Stream OpenStreamUnsafe(string href, Document document)
        {
            if ((document != null) && !UriUtil.IsHrefWithScheme(href))
                href = document.MapFileName(UriUtil.UnescapeHref(href));

            return SystemPal.OpenStreamFromHref(href);
        }

        /// <summary>
        /// Calculate textbox IDs.
        /// </summary>
        public static int CalcTextIds(int value)
        {
            return Convert.ToInt32((value + 1) << 16);
        }
    }
}
