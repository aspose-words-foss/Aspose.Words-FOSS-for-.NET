// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 31/01/2024 by Denis Panov

#if NETSTANDARD
using System;
using System.Drawing.Imaging;
using System.IO;
using Aspose.Images;
using Aspose.Images.Pal;
using Aspose.Images.Pal.Graphics.Decoder.Tiff;
using Aspose.TestFx;
using NUnit.Framework;
using SkiaSharp;

namespace Aspose.Tests.Xamarin
{
    [TestFixture]
    public class TestTiffWriter
    {
        [TestCase(TiffCompressionCore.None)]
        [TestCase(TiffCompressionCore.Rle)]
        [TestCase(TiffCompressionCore.Ccitt3)]
        [TestCase(TiffCompressionCore.Ccitt4)]
        [TestCase(TiffCompressionCore.Lzw)]
        public void TestTiffThreshold(TiffCompressionCore compression)
        {
            TestTiffCore("Images/Tiff/glow.tiff", compression, ImageBinarizationMethodCore.Threshold);
        }

        [TestCase(TiffCompressionCore.None)]
        [TestCase(TiffCompressionCore.Rle)]
        [TestCase(TiffCompressionCore.Ccitt3)]
        [TestCase(TiffCompressionCore.Ccitt4)]
        [TestCase(TiffCompressionCore.Lzw)]
        public void TestTiffDithering(TiffCompressionCore compression)
        {
            TestTiffCore("Images/Tiff/glow.tiff", compression, ImageBinarizationMethodCore.FloydSteinbergDithering);
        }

        [TestCase("SimpleNone", TiffCompressionCore.None, "Images/Tiff/example2.tiff", "Images/Tiff/example3.tiff")]
        [TestCase("SimpleRle", TiffCompressionCore.Rle, "Images/Tiff/ortex.tiff", "Images/Tiff/sample.tif")]
        [TestCase("SimpleCcitt", TiffCompressionCore.Ccitt3, "Images/AWReference.gif")]
        [TestCase("SimpleLzw", TiffCompressionCore.Lzw, "Images/w3test.png", "Images/AWReference.gif", "Images/CMYK/TestCmykImage.jpeg")]
        public void TestTiffMultipage(string testName, TiffCompressionCore compression, params string[] pageFiles)
        {
            TestTiffMultipageCore(testName, compression, ImageBinarizationMethodCore.Threshold, pageFiles);
        }

        [TestCase("Images/SkiaColorType/Alpha8.png", TiffCompressionCore.Lzw, ImageBinarizationMethodCore.Threshold)]
        [TestCase("Images/SkiaColorType/Alpha8.png", TiffCompressionCore.Ccitt3, ImageBinarizationMethodCore.FloydSteinbergDithering)]
        [TestCase("Images/SkiaColorType/Gray8.png", TiffCompressionCore.None, ImageBinarizationMethodCore.Threshold)]
        [TestCase("Images/SkiaColorType/Argb4444.png", TiffCompressionCore.Rle, ImageBinarizationMethodCore.FloydSteinbergDithering)]
        [TestCase("Images/SkiaColorType/Rgb565.png", TiffCompressionCore.Ccitt4, ImageBinarizationMethodCore.Threshold)]
        [TestCase("Images/SkiaColorType/Rgb888x.png", TiffCompressionCore.Lzw, ImageBinarizationMethodCore.FloydSteinbergDithering)]
        [TestCase("Images/SkiaColorType/Rgb101010x.png", TiffCompressionCore.Ccitt3, ImageBinarizationMethodCore.Threshold)]
        [TestCase("Images/SkiaColorType/Rgba1010102.png", TiffCompressionCore.Lzw, ImageBinarizationMethodCore.Threshold)]
        [TestCase("Images/SkiaColorType/RgbaF16.png", TiffCompressionCore.None, ImageBinarizationMethodCore.Threshold)]
        public void TestTiffColorType(string testName, TiffCompressionCore comp, ImageBinarizationMethodCore binMethod)
        {
            TestTiffCore(testName, comp, binMethod);
        }

        #region TestCore

        private static void TestTiffCore(string input, TiffCompressionCore compression, ImageBinarizationMethodCore binarizationMethod)
        {
            string tiffFileName = Path.GetFileNameWithoutExtension(input);
            string testIn = TestFxUtil.BuildTestFileName(input);
            string suffix = GetSuffix(compression, binarizationMethod);

            string testOut = TestFxUtil.BuildOutFileName(string.Format("Images/Tiff/TestTiff_{0}_{1}", tiffFileName, suffix), "", ".tif");
            string testGold = TestFxUtil.BuildGoldFileName(string.Format("Images/Tiff/TestTiff_{0}_{1}", tiffFileName, suffix), "", ".tif");

            TestFxUtil.EnsureDirectoryForFileExists(testOut);

            using (FileStream inputFS = File.OpenRead(testIn))
            using (BitmapPal bmp = new BitmapPal(inputFS))
            {
                using (FileStream outputFS = File.Create(testOut))
                using (TiffWriterPal tiffWriter = new TiffWriterPal())
                    tiffWriter.SaveFirstFrame(outputFS, compression, binarizationMethod, 0, PixelFormat.Format32bppArgb, bmp, false);
            }

            // Do not show original Tiff image, because some of them are not valid for windows and out of memory error is thrown.
            TestRenderingUtil.VerifyGraphics(testOut, testGold, null);
        }

        private static void TestTiffMultipageCore(
            string testName,
            TiffCompressionCore compression,
            ImageBinarizationMethodCore binarizationMethod,
            string[] pageFiles)
        {
            string testOut = TestFxUtil.BuildOutFileName(string.Format("Images/Tiff/TestMultipageTiff_{0}", testName), "", ".tif");

            CreateMultipageTiff(testOut, compression, binarizationMethod, pageFiles);

            VerifyMultipageTiffGraphics(testOut, testName, pageFiles.Length);
        }

        private static string GetSuffix(TiffCompressionCore compression, ImageBinarizationMethodCore binarizationMethod)
        {
            string suffix = Enum.GetName(typeof(TiffCompressionCore), compression);
            suffix += binarizationMethod == ImageBinarizationMethodCore.Threshold ? "_Threshold" : "_Dithering";
            return suffix;
        }

        private static void VerifyMultipageTiffGraphics(string tiffFile, string testName, int pageCountExpected)
        {
            string testFileDir = Path.GetDirectoryName(tiffFile);

            using (FileStream tiffFS = File.OpenRead(tiffFile))
            using (TiffDecoder tiffDecoder = new TiffDecoder(tiffFS))
            {
                Assert.That(tiffDecoder.PageCount, Is.EqualTo(pageCountExpected));

                for (short i = 0; i < tiffDecoder.PageCount; i++)
                {
                    string testOutPage = TestFxUtil.BuildOutFileName(string.Format("Images/Tiff/TestMultipageTiff_{0}", testName), " " + i, ".png");
                    string testGoldPage = TestFxUtil.BuildGoldFileName(string.Format("Images/Tiff/TestMultipageTiff_{0}", testName), " " + i, ".png");

                    using (FileStream outPage = File.Create(testOutPage))
                    using (SKBitmap page = tiffDecoder.GetNativeBitmap(i))
                        page.Encode(outPage, SKEncodedImageFormat.Png, 96);

                    TestRenderingUtil.VerifyGraphics(testOutPage, testGoldPage, null);
                }
            }
        }

        private static void CreateMultipageTiff(string tiffFile,
            TiffCompressionCore compression,
            ImageBinarizationMethodCore binarizationMethod,
            string[] pageFiles)
        {
            TestFxUtil.EnsureDirectoryForFileExists(tiffFile);

            using (FileStream outputFS = File.Create(tiffFile))
            using (TiffWriterPal tiffWriter = new TiffWriterPal())
            for (int i = 0; i < pageFiles.Length; i++)
            {
                using (FileStream inputFS = File.OpenRead(TestFxUtil.BuildTestFileName(pageFiles[i])))
                using (BitmapPal bmp = new BitmapPal(inputFS))
                {
                    if (i == 0)
                        tiffWriter.SaveFirstFrame(outputFS, compression, binarizationMethod, 0, PixelFormat.Format32bppArgb, bmp, false);
                    else
                        tiffWriter.SaveIntermediateFrame(bmp);

                    tiffWriter.FlushMultiframe();
                }
            }
        }
        #endregion
    }
}
#endif
