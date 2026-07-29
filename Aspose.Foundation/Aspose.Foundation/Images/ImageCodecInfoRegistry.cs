// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 29/07/2011 by Roman Korchagin

#if !NETSTANDARD

using System.Drawing.Imaging;
using Aspose.Collections;
using Aspose.JavaAttributes;

namespace Aspose.Images
{
    [JavaDelete("Not needed on Java.")]
    public static class ImageCodecInfoRegistry
    {
        /// <summary>
        /// Gets supported codec information for the specified image format.
        /// </summary>
        public static ImageCodecInfo GetEncoderInfo(ImageFormat format)
        {
            return gImageCodecInfos[format.Guid];
        }

        /// <summary>
        /// Static ctor.
        /// </summary>
        static ImageCodecInfoRegistry()
        {
            // Do this once to avoid doing this for every image save.
            foreach (ImageCodecInfo imageCodecInfo in ImageCodecInfo.GetImageEncoders())
                gImageCodecInfos[imageCodecInfo.FormatID] = imageCodecInfo;
        }

        /// <summary>
        /// The key is ImageFormat.Guid and the value is ImageCodecInfo.
        /// </summary>
        private static readonly GuidToObjDictionary<ImageCodecInfo> gImageCodecInfos = 
            new GuidToObjDictionary<ImageCodecInfo>();
    }
}
#endif
