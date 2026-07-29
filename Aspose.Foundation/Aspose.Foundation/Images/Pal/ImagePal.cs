// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 20/12/2015 by Alexander Zhiltsov
#if !NETSTANDARD
using System;
using System.Drawing;
using System.IO;
using Aspose.JavaAttributes;

namespace Aspose.Images.Pal
{
    /// <summary>
    /// Base class for wrappers of native image objects.
    /// </summary>
    [JavaManual("Manual porting by design.")]
    public class ImagePal : IDisposable
    {
        /// <summary>
        /// It is not possible to create objects of this type.
        /// </summary>
        protected ImagePal()
        {
        }

        /// <summary>
        /// This implementation is a streamlined version of the FxCop rule
        /// http://msdn.microsoft.com/en-us/library/ms244737(VS.80).aspx
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            // The method below does nothing for this class because this class doesn't have a destructor, 
            // but a derived class can have this one.
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Sets empty image and releases all resources associated with the object.
        /// </summary>
        public void Close()
        {
            Dispose(true);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing && (mNativeImage != null))
            {
                // Dispose managed resources that implement IDisposable here. 
                mNativeImage.Dispose();
                mNativeImage = null;
            }
        }

        /// <summary>
        /// Sets the native image object that this wrapper encapsulates.
        /// </summary>
        protected void SetNativeImage(Image value)
        {
            mNativeImage = value;
        }

        /// <summary>
        /// Gets the native image object that this wrapper encapsulates.
        /// </summary>
        internal Image NativeImage
        {
            get { return mNativeImage; }
        }

        /// <summary>
        /// Returns the original format of the image.
        /// </summary>
        public FileFormat ImageType
        {
            get { return mImageType; }
            protected set { mImageType = value; }
        }

        /// <summary>
        /// Returns width of the image.
        /// </summary>
        public int Width
        {
            get { return mNativeImage.Width; }
        }

        /// <summary>
        /// Returns height of the image.
        /// </summary>
        public int Height
        {
            get { return mNativeImage.Height; }
        }

        /// <summary>
        /// Returns the resolution that is resilient to Windows desktop resolution.
        /// </summary>
        public float HorizontalResolution
        {
            get { return mIsOriginalResolutionZero ? ImageConstants.StandardResolution : mNativeImage.HorizontalResolution; }
        }

        /// <summary>
        /// Returns the resolution that is resilient to Windows desktop resolution.
        /// </summary>
        public float VerticalResolution
        {
            get { return mIsOriginalResolutionZero ? ImageConstants.StandardResolution : mNativeImage.VerticalResolution; }
        }

        /// <summary>
        /// Used during initialization.
        /// </summary>
        protected void SetIsOriginalResolutionZero(byte[] imageBytes)
        {
            // Normally, this class handles bitmap images only, but a textured brush can have a metafile
            // as its texture and when converting such brush to a native image, .NET plays its dirty trick again and
            // sets the bitmap resolution to the current desktop resolution. So we need to treat metafiles as having
            // zero resolution.
            using (MemoryStream stream = new MemoryStream(imageBytes))
                SetIsOriginalResolutionZero(stream);
        }

        /// <summary>
        /// Used during initialization.
        /// </summary>
        protected void SetIsOriginalResolutionZero(Stream imageStream)
        {
            FileFormat imageType = ImageUtil.GetImageType(imageStream);

            mIsOriginalResolutionZero =
                ImageUtil.IsMetafile(imageType) || ImageUtil.GetImageSize(imageStream, imageType).IsOriginalResolutionZero;
        }

        /// <summary>
        /// RK It can happen when the resolution returned from the original image bytes is different
        /// from the resolution returned by the GDI+ for an image that was loaded from those bytes.
        /// 
        /// This could happen for example, when the resolution was not specified in the original image bytes 
        /// and we default to 96dpi. But creating a GDI+ image from such bytes will set image resolution to 
        /// the current desktop resolution and it could be 120dpi for example. This will cause XPS and possibly
        /// other tests to fail because the output will be dependent on the desktop resolution.
        /// 
        /// So here we detect that situation and change the bitmap resolution to what we expect it to be,
        /// but to properly set the bitmap resolution we actually have to create a new bitmap.
        /// This method disposes the original bitmap.
        /// </summary>
        protected void SetStandardResolutionIfOriginalResolutionWasZero()
        {
            if (mIsOriginalResolutionZero)
            {
                Bitmap newBitmap = new Bitmap(mNativeImage);
                newBitmap.SetResolution(ImageConstants.StandardResolution, ImageConstants.StandardResolution);
                mNativeImage.Dispose();
                mNativeImage = newBitmap;
                mIsOriginalResolutionZero = false;
            }
        }

        private Image mNativeImage;
        private FileFormat mImageType;
        private bool mIsOriginalResolutionZero;
    }
}
#endif
