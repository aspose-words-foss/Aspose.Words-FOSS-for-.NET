// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 10/02/2020 by Alexey Noskov

#if NETSTANDARD
using Aspose.Images.Pal;
using Aspose.TestFx;
using NUnit.Framework;
using System.IO;

namespace Aspose.Tests.Xamarin
{
    [TestFixture]
    public class TestTiffReader
    {
        [Test, TestCaseSource("LibTiffTestCases")]
        public void TestTiffLibTiffPic(string input)
        {
            TestTiffCore("Images/Tiff/libtiffpic/" + input);
        }

        [Test, TestCaseSource("LibTiffDepthTestCases")]
        public void TestTiffLibTiffPicDepth(string input)
        {
            TestTiffCore("Images/Tiff/libtiffpic/depth/" + input);
        }

        [TestCase("Images/Tiff/density.tiff")]
        [TestCase("Images/Tiff/example2.tiff")]
        [TestCase("Images/Tiff/example3.tiff")]
        [TestCase("Images/Tiff/example4.tiff")]
        [TestCase("Images/Tiff/glow.tiff")]
        [TestCase("Images/Tiff/ortex.tiff")]
        [TestCase("Images/Tiff/sample.tif")]
        [TestCase("Images/Tiff/Trp88.tiff")]
        public void TestTiff(string input)
        {
            TestTiffCore(input);
        }

        [TestCase("Rendering/Aps/TestImages/TestImage Format16bppArgb1555.tif")]
        [TestCase("Rendering/Aps/TestImages/TestImage Format16bppRgb555.tif")]
        [TestCase("Rendering/Aps/TestImages/TestImage Format16bppRgb565.tif")]
        [TestCase("Rendering/Aps/TestImages/TestImage Format24bppRgb.tif")]
        [TestCase("Rendering/Aps/TestImages/TestImage Format32bppArgb.tif")]
        [TestCase("Rendering/Aps/TestImages/TestImage Format32bppPArgb.tif")]
        [TestCase("Rendering/Aps/TestImages/TestImage Format32bppRgb.tif")]
        [TestCase("Rendering/Aps/TestImages/TestImage Format48bppRgb.tif")]
        [TestCase("Rendering/Aps/TestImages/TestImage Format64bppArgb.tif")]
        [TestCase("Rendering/Aps/TestImages/TestImage FormaTif96bit_CMYK_NoCompression_SWOP.tif")]
        [TestCase("Rendering/Aps/TestImages/TestImage FormatTif24bit_CMYK_LZW.tif")]
        [TestCase("Rendering/Aps/TestImages/TestImage FormatTif64bit_LZW_AdobeRGB.tif")]
        [TestCase("Rendering/Aps/UnsupportedImages/TestImage Format24Bit_Lab.tif")]
        [TestCase("Rendering/Aps/UnsupportedImages/TestImage FormatIndexed256.tif")]
        [TestCase("Rendering/Aps/UnsupportedImages/TestImage FormatTif48bit_Lab.tif")]
        [TestCase("Rendering/Aps/UnsupportedImages/TestImage FormatTif64bit_Lab.tif")]
        public void TestTiffAW(string input)
        {
            TestTiffCore(input);
        }

        [Test]
        public void Test18428()
        {
            TestTiffCore("Images/Tiff/Test18428.tiff");
        }

        /// <summary>
        /// WORDSNET-20321 Aspose.Words.Document constructor hangs for ODT under netcoreapp3.1
        /// </summary>
        [Test]
        public void Test20321()
        {
            TestTiffCore("Images/Tiff/Test20321.tif");
        }

        /// <summary>
        /// WORDSNET-25200 DocumentBuilder throws an exception when inserting a Tiff image under NetStandart
        /// </summary>
        [Test]
        public void Test25200()
        {
            TestTiffCore("Images/Tiff/Test25200.tif");
        }

        private static readonly string[] LibTiffTestCases =
        {
            "caspian.tif", "cramps.tif", "cramps-tile.tif", "dscf0013.tif", "fax2d.tif", "g3test.tif", "jello.tif",
            "jim___ah.tif", "jim___cg.tif", "jim___dg.tif", "jim___gg.tif", "ladoga.tif", "off_l16.tif", "off_luv24.tif",
            "off_luv32.tif", "oxford.tif", "pc260001.tif", "quad-jpeg.tif", "quad-lzw.tif", "quad-tile.tif", "smallliz.tif",
            "strike.tif", "text.tif", "ycbcr-cat.tif", "zackthecat.tif"
        };

        private static readonly string[] LibTiffDepthTestCases =
        {
            "flower-minisblack-02.tif", "flower-minisblack-04.tif", "flower-minisblack-06.tif", "flower-minisblack-08.tif",
            "flower-minisblack-10.tif", "flower-minisblack-12.tif", "flower-minisblack-14.tif", "flower-minisblack-16.tif",
            "flower-minisblack-24.tif", "flower-minisblack-32.tif", "flower-palette-02.tif", "flower-palette-04.tif",
            "flower-palette-08.tif", "flower-palette-16.tif", "flower-rgb-contig-02.tif", "flower-rgb-contig-04.tif",
            "flower-rgb-contig-08.tif", "flower-rgb-contig-10.tif", "flower-rgb-contig-12.tif", "flower-rgb-contig-14.tif",
            "flower-rgb-contig-16.tif", "flower-rgb-contig-24.tif", "flower-rgb-contig-32.tif", "flower-rgb-planar-02.tif",
            "flower-rgb-planar-04.tif", "flower-rgb-planar-08.tif", "flower-rgb-planar-10.tif", "flower-rgb-planar-12.tif",
            "flower-rgb-planar-14.tif", "flower-rgb-planar-16.tif", "flower-rgb-planar-24.tif", "flower-rgb-planar-32.tif",
            "flower-separated-contig-08.tif", "flower-separated-contig-16.tif", "flower-separated-planar-08.tif",
            "flower-separated-planar-16.tif"
        };

#region TestCore

        private static void TestTiffCore(string input)
        {
            string tiffFileName = Path.GetFileNameWithoutExtension(input);
            string testIn = TestFxUtil.BuildTestFileName(input);
            string testFileDir = Path.GetDirectoryName(testIn);
            string testOut = TestFxUtil.BuildOutFileName(Path.Combine(testFileDir, string.Format("TestTiff_{0}", tiffFileName)), "", ".png");
            string testGold = TestFxUtil.BuildGoldFileName(Path.Combine(testFileDir, string.Format("TestTiff_{0}", tiffFileName)), "", ".png");

            TestFxUtil.EnsureDirectoryForFileExists(testOut);

            using (FileStream inputFS = File.OpenRead(testIn))
            using (BitmapPal bmp = new BitmapPal(inputFS))
            using (FileStream outputFS = File.Create(testOut))
                bmp.SavePng(outputFS);

            // Do not show original Tiff image, because some of them are not valid for windows and out of memory error is thrown.
            TestRenderingUtil.VerifyGraphics(testOut, testGold, null);
        }
#endregion
    }
}
#endif
