// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 25/12/2008 by Roman Korchagin
using System;
using System.IO;
using Aspose.Words.Drawing;
using CodePorting.Translator.Cs2Cpp;

namespace Aspose.Words.Saving
{
    /// <summary>
    /// Provides data for the <see cref="IImageSavingCallback.ImageSaving"/> event.
    /// <para>To learn more, visit the <a href="https://docs.aspose.com/words/net/save-a-document/">Save a Document</a> documentation article.</para>
    /// </summary>
    /// <remarks>
    /// <para>By default, when Aspose.Words saves a document to HTML, it saves each image into 
    /// a separate file. Aspose.Words uses the document file name and a unique number to generate unique file name
    /// for each image found in the document.</para>
    /// 
    /// <para><see cref="ImageSavingArgs"/> allows to redefine how image file names are generated or to 
    /// completely circumvent saving of images into files by providing your own stream objects.</para>
    /// 
    /// <para>To apply your own logic for generating image file names use the 
    /// <see cref="ImageFileName"/>, <see cref="CurrentShape"/> and <see cref="IsImageAvailable"/> 
    /// properties.</para>
    /// 
    /// <para>To save images into streams instead of files, use the <see cref="ImageStream"/> property.</para>
    /// </remarks>
    public class ImageSavingArgs
    {
        /// <summary>
        /// Ctor. Should be internal. Client code won't create this object.
        /// </summary>
        internal ImageSavingArgs(ShapeBase shape, bool isImageAvailable, string dstFileName)
        {
            mShape = shape;
            mIsImageAvailable = isImageAvailable;
            mImageFileName = dstFileName;
        }

        /// <summary>
        /// Gets the document object that is currently being saved.
        /// </summary>
        public Document Document
        {
            get { return mShape.FetchDocument(); }
        }

        /// <summary>
        /// Gets the <see cref="Aspose.Words.Drawing.ShapeBase"/> object corresponding to the shape or group shape 
        /// that is about to be saved.
        /// </summary>
        /// <remarks>
        /// 
        /// <para><see cref="IImageSavingCallback" /> can be fired while saving either a shape or a group shape. 
        /// That's why the property has <see cref="ShapeBase" /> type. You can check whether it's a group shape comparing 
        /// <see cref="ShapeBase.ShapeType" /> with <see cref="ShapeType.Group" /> or by casting it to one of derived classes: 
        /// <see cref="Shape" /> or <see cref="GroupShape" />.</para>
        /// 
        /// <para>Aspose.Words uses the document file name and a unique number to generate unique file name 
        /// for each image found in the document. You can use the <see cref="CurrentShape"/> property to generate 
        /// a "better" file name by examining shape properties such as <see cref="ImageData.Title"/> 
        /// (Shape only), <see cref="ImageData.SourceFullName"/> (Shape only) 
        /// and <see cref="ShapeBase.Name"/>. Of course you can build file names using any other properties or criteria 
        /// but note that subsidiary file names must be unique within the export operation.</para>
        /// 
        /// <para>Some images in the document can be unavailable. To check image availability 
        /// use the <see cref="IsImageAvailable"/> property.</para>
        /// </remarks>
        public ShapeBase CurrentShape
        {
            get { return mShape; }
        }

        /// <summary>
        /// Returns <c>true</c> if the current image is available for export.
        /// </summary>
        /// <remarks>
        /// <para>Some images in the document can be unavailable, for example, because the image
        /// is linked and the link is inaccessible or does not point to a valid image. 
        /// In this case Aspose.Words exports an icon with a red cross. This property returns 
        /// <c>true</c> if the original image is available; returns <c>false</c> if the original 
        /// image is not available and a "no image" icon will be offered for save.</para>
        /// 
        /// <para>When saving a group shape or a shape that doesn't require any image this property 
        /// is always <c>true</c>.</para>
        /// </remarks>
        /// <seealso cref="CurrentShape"/>
        public bool IsImageAvailable
        {
            get { return mIsImageAvailable; }
        }

        /// <summary>
        /// Gets or sets the file name (without path) where the image will be saved to.
        /// </summary>
        /// <remarks>
        /// <para>This property allows you to redefine how the image file names are generated
        /// during export to HTML.</para>
        /// 
        /// <para>When the event is fired, this property contains the file name that was generated 
        /// by Aspose.Words. You can change the value of this property to save the image into a 
        /// different file. Note that file names must be unique.</para>
        /// 
        /// <p>Aspose.Words automatically generates a unique file name for every embedded image when 
        /// exporting to HTML format. How the image file name is generated 
        /// depends on whether you save the document to a file or to a stream.</p>
        /// 
        /// <p>When saving a document to a file, the generated image file name looks like 
        /// <i>&lt;document base file name&gt;.&lt;image number&gt;.&lt;extension&gt;</i>.</p>
        /// 
        /// <p>When saving a document to a stream, the generated image file name looks like 
        /// <i>Aspose.Words.&lt;document guid&gt;.&lt;image number&gt;.&lt;extension&gt;</i>.</p>
        /// 
        /// <para><see cref="ImageFileName"/> must contain only the file name without the path.
        /// Aspose.Words determines the path for saving and the value of the <c>src</c> attribute for writing 
        /// to HTML using the document file name, the <see cref="HtmlSaveOptions.ImagesFolder"/> and
        /// <see cref="HtmlSaveOptions.ImagesFolderAlias"/> properties.</para>
        /// 
        /// <seealso cref="CurrentShape"/>
        /// <seealso cref="IsImageAvailable"/>
        /// <seealso cref="ImageStream"/>
        /// <seealso cref="HtmlSaveOptions.ImagesFolder"/>
        /// <seealso cref="HtmlSaveOptions.ImagesFolderAlias"/>
        /// </remarks>
        public string ImageFileName
        {
            get { return mImageFileName; }
            set
            {
                ArgumentUtil.CheckHasChars(value, "ImageFileName");
                if (Path.GetFileName(value) != value)
                    throw new ArgumentException("ImageFileName must be a file name without path.");
                mImageFileName = value;
            }
        }

        /// <summary>
        /// Specifies whether Aspose.Words should keep the stream open or close it after saving an image.
        /// </summary>
        /// <remarks>
        /// <para>Default is <c>false</c> and Aspose.Words will close the stream you provided
        /// in the <see cref="ImageStream"/> property after writing an image into it.
        /// Specify <c>true</c> to keep the stream open.</para>
        /// 
        /// <seealso cref="ImageStream"/>
        /// </remarks>
        public bool KeepImageStreamOpen
        {
            get { return mKeepImageStreamOpen; }
            set { mKeepImageStreamOpen = value; }
        }

        /// <summary>
        /// Allows to specify the stream where the image will be saved to.
        /// </summary>
        /// <remarks>
        /// <para>This property allows you to save images to streams instead of files during HTML.</para>
        /// 
        /// <para>The default value is <c>null</c>. When this property is <c>null</c>, the image 
        /// will be saved to a file specified in the <see cref="ImageFileName"/> property.</para>
        /// 
        /// <para>Using <see cref="IImageSavingCallback" /> you cannot substitute one image with 
        /// another. It is intended only for control over location where to save images.</para>
        /// 
        /// <seealso cref="ImageFileName"/>
        /// <seealso cref="KeepImageStreamOpen"/>
        /// </remarks>
        /// <javaName>ImageStream(java.io.OutputStream)</javaName>
#if PLAIN_JAVA
        // Manually replace System.IO.Stream with java.io.OutputStream in Java public API.
        // We can't use public API autosubstitution here since we need OutputStream inside.
        public java.io.OutputStream getImageStream() { return mImageStream; }
        public void setImageStream(java.io.OutputStream value) { mImageStream = value; }
        private java.io.OutputStream mImageStream;
#else
        [CppIOStreamWrapper(IOStreamType.OStream)]
        public Stream ImageStream
        {
            get { return mImageStream; }
            set { mImageStream = value; }
        }
        private Stream mImageStream;
#endif

        /// <summary>
        /// Exists to make calling code autoportable.
        /// </summary>
        internal bool HasUserStream
        {
            get { return mImageStream != null; }
        }

        /// <summary>
        /// Exists to make calling code autoportable.
        /// </summary>
        internal UserStreamWrapper CreateUserStreamWrapper()
        {
            return new UserStreamWrapper(mImageStream, mKeepImageStreamOpen);
        }

        private readonly ShapeBase mShape;
        private readonly bool mIsImageAvailable;
        private string mImageFileName;
        private bool mKeepImageStreamOpen;
    }
}
