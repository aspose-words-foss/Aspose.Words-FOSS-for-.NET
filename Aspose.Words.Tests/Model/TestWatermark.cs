// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 12/04/2020 by Artem Ptitsin

using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;
using Aspose.Collections.Generic;
using Aspose.Images;
using Aspose.Images.Pal;
using Aspose.Words.Drawing;
using Aspose.Words.Saving;
using NUnit.Framework;

namespace Aspose.Words.Tests.Model
{
    /// <summary>
    /// Tests the watermark.
    /// </summary>
    [TestFixture]
    public class TestWatermark
    {
        [TestFixtureSetUp]
        public void SetUp()
        {
            TestUtil.SetUpTests();
        }

        /// <summary>
        /// WORDSNET-4879 Add a helper class for working with a document watermark.
        /// <see cref="Document.Watermark"/>
        /// </summary>
        [TestCase(@"Model\Shape\Watermark\Test4879A.docx")]
        [TestCase(@"Model\Shape\Watermark\Test4879B.docx")]
        public void Test4879(string pathFile)
        {
            Document doc = TestUtil.Open(pathFile);

            doc.Watermark.SetText("Test1");
            doc.Watermark.SetText("Test2");

            foreach (HeaderFooter headerFooter in doc.FirstSection.HeadersFooters)
            {
                if (headerFooter.IsHeader)
                {
                    Assert.That(headerFooter.Shapes.Count, Is.EqualTo(1));
                    Shape watermark = headerFooter.Shapes[0];
                    Assert.That(watermark.IsWatermark, Is.True);
                    Assert.That(watermark.TextPath.Text, Is.EqualTo("Test2"));
                    Assert.That(watermark.ParentParagraph, Is.EqualTo(headerFooter.FirstParagraph));
                }
            }

            doc.Watermark.Remove();

            foreach (HeaderFooter headerFooter in doc.FirstSection.HeadersFooters)
            {
                if (headerFooter.IsHeader)
                    Assert.That(headerFooter.Shapes.Count, Is.EqualTo(0));
            }
        }

        /// <summary>
        /// Relates to WORDSNET-4879 Checks that the watermark is correctly detected.
        /// </summary>
        [TestCase(@"Model\Shape\Watermark\Test4879IsWatermarkA.docx", 3, 0)]
        [TestCase(@"Model\Shape\Watermark\Test4879IsWatermarkB.docx", 3, 0)]
        [TestCase(@"Model\Shape\Watermark\Test4879IsWatermarkD.docx", 13, 10)]
        public void Test4879IsWatermark(string pathFile, int countShapes, int countWatermarks)
        {
            Document doc = TestUtil.Open(pathFile);
            List<Shape> shapes = doc.GetChildNodes(NodeType.Shape, true).ToList<Shape>();

            Assert.That(shapes.Count, Is.EqualTo(countShapes));

            int countRealWatermark = 0;
            foreach (Shape shape in shapes)
            {
                if (shape.IsWatermark)
                {
                    countRealWatermark++;
                    AssertIsWatermark(shape);
                }
            }

            Assert.That(countRealWatermark, Is.EqualTo(countWatermarks));
        }

        /// <summary>
        /// Relates to WORDSNET-4879 Checks that the watermark type is correctly detected.
        /// </summary>
        [TestCase(@"Model\Shape\Watermark\Test4879DefineWatermarkTypeA.docx", WatermarkType.None)]
        [TestCase(@"Model\Shape\Watermark\Test4879DefineWatermarkTypeB.docx", WatermarkType.None)]
        [TestCase(@"Model\Shape\Watermark\Test4879DefineWatermarkTypeC.docx", WatermarkType.Text)]
        [TestCase(@"Model\Shape\Watermark\Test4879DefineWatermarkTypeD.docx", WatermarkType.Text)]
        [TestCase(@"Model\Shape\Watermark\Test4879DefineWatermarkTypeE.docx", WatermarkType.Image)]
        [TestCase(@"Model\Shape\Watermark\Test4879DefineWatermarkTypeF.docx", WatermarkType.Text)]
        public void Test4879DefineWatermarkType(string path, WatermarkType type)
        {
            Document doc = TestUtil.Open(path);
            Assert.That(doc.Watermark.Type, Is.EqualTo(type));
        }

        /// <summary>
        /// Relates to WORDSNET-4879 Checks the addition of a watermark to a new document.
        /// </summary>
        [Test]
        public void Test4879NewDocument()
        {
            Document doc = new Document();
            doc.Watermark.SetText("Test");

            int countHeaders = 0;
            foreach (HeaderFooter headerFooter in doc.FirstSection.HeadersFooters)
            {
                if (headerFooter.IsHeader)
                {
                    countHeaders++;
                    Shape watermark = headerFooter.Shapes[0];
                    Assert.That(watermark.IsWatermark, Is.True);
                    Assert.That(watermark.ParentParagraph, Is.EqualTo(headerFooter.FirstParagraph));
                }
            }

            Assert.That(countHeaders, Is.EqualTo(3));
        }

        /// <summary>
        /// Relates to WORDSNET-4879 Checks the addition of a watermark to a new document
        /// with several sections of different types.
        /// </summary>
        [Test]
        public void Test4879NewDocumentSeveralSections()
        {
            DocumentBuilder builder = new DocumentBuilder();
            builder.InsertBreak(BreakType.SectionBreakNewPage);
            builder.InsertBreak(BreakType.SectionBreakEvenPage);
            builder.InsertBreak(BreakType.SectionBreakOddPage);
            builder.InsertBreak(BreakType.SectionBreakNewColumn);
            builder.InsertBreak(BreakType.SectionBreakContinuous);

            Document doc = builder.Document;
            doc.Watermark.SetText("Test");

            int countHeaders = 0;
            foreach (HeaderFooter headersFooter in doc.FirstSection.HeadersFooters)
            {
                if (headersFooter.IsHeader)
                {
                    countHeaders++;
                    Assert.That(headersFooter.Shapes[0].IsWatermark, Is.True);
                }
            }

            Assert.That(countHeaders, Is.EqualTo(3));
        }

        /// <summary>
        /// Relates to WORDSNET-4879 Checks that all watermarks are removed.
        /// </summary>
        [TestCase(@"Model\Shape\Watermark\Test4879RemoveWatermarkA.docx", 10, 13)]
        [TestCase(@"Model\Shape\Watermark\Test4879RemoveWatermarkB.docx", 1, 4)]
        public void Test4879RemoveWatermark(string path, int countWatermarks, int countShapes)
        {
            Document doc = TestUtil.Open(path);
            List<Shape> shapes = doc.GetChildNodes(NodeType.Shape, true).ToList<Shape>();

            Assert.That(shapes.Count, Is.EqualTo(countShapes));
            int countWatermark = 0;
            foreach (Shape shape in shapes)
            {
                if (shape.IsWatermark)
                {
                    AssertIsWatermark(shape);
                    countWatermark++;
                }
            }

            Assert.That(countWatermark, Is.EqualTo(countWatermarks));
            doc.Watermark.Remove();

            shapes = doc.GetChildNodes(NodeType.Shape, true).ToList<Shape>();
            Assert.That(shapes.Count, Is.EqualTo(countShapes - countWatermarks));
        }

        /// <summary>
        /// Relates to WORDSNET-4879 Checks the default text watermark properties.
        /// </summary>
        [Test]
        public void Test4879TextWatermarkDefaults()
        {
            Document doc = new Document();
            doc.Watermark.SetText("Test");
            Shape watermark = doc.FirstSection.HeadersFooters[HeaderFooterType.HeaderFirst].Shapes[0];
            CheckDefaultTextWatermark(watermark);
        }

        /// <summary>
        /// Relates to WORDSNET-4879 Checks the auto size font of the text watermark at different page sizes.
        /// </summary>
        [TestCase(Orientation.Portrait, WatermarkLayout.Diagonal, 351.42, 259.52)] // Defaults
        [TestCase(Orientation.Landscape, WatermarkLayout.Diagonal, 380.7, 281.15)]
        [TestCase(Orientation.Portrait, WatermarkLayout.Horizontal, 432, 319.03)]
        [TestCase(Orientation.Landscape, WatermarkLayout.Horizontal, 612, 451.97)]
        public void Test4879TextWatermarkAutoSize(Orientation orientation, WatermarkLayout layout, double width,
            double height)
        {
            Document doc = new Document();
            doc.FirstSection.PageSetup.Orientation = orientation;
            TextWatermarkOptions options = new TextWatermarkOptions();
            options.Layout = layout;

            doc.Watermark.SetText("Test", options);
            Shape watermark = doc.FirstSection.HeadersFooters[HeaderFooterType.HeaderFirst].Shapes[0];

            Assert.That(new[] { watermark.Width, watermark.Height }, Is.EqualTo(new[] { width, height }));
        }

        /// <summary>
        /// Relates to WORDSNET-4879 Checks the different size fonts of the text watermark.
        /// </summary>
        [TestCase(0, 351.42, 259.52)] // Auto.
        [TestCase(1, 1.74, 1.29)] // Min value.
        [TestCase(1638, 2850.12, 2104.83)] // Max value.
        [TestCase(-1, 0, 0)]
        [TestCase(1639, 0, 0)]
        [TestCase(54, 93.96, 69.39)]
        [TestCase(55, 95.7, 70.68)]
        public void Test4879TextWatermarkSize(float size, double width, double height)
        {
            try
            {
                Document doc = new Document();
                TextWatermarkOptions options = new TextWatermarkOptions();
                options.FontSize = size;

                if ((size == -1) || (size == 1639))
                    Assert.Fail();

                doc.Watermark.SetText("Test", options);
                Shape watermark = doc.FirstSection.HeadersFooters[HeaderFooterType.HeaderFirst].Shapes[0];

                Assert.That(new[] { watermark.Width, watermark.Height }, Is.EqualTo(new[] { width, height }));
            }
            catch (ArgumentOutOfRangeException)
            {
                if ((size != -1) && (size != 1639))
                    Assert.Fail();
            }
        }

        /// <summary>
        /// Relates to WORDSNET-4879 Checks the different text values of the text watermark.
        /// </summary>
        [TestCase(null)]
        [TestCase("")]
        [TestCase("   ")]
        [TestCase("   Test   ")]
        [TestCase("Test")]
        public void Test4879TextWatermarkText(string text)
        {
            CheckWatermarkText(text);
        }

        /// <summary>
        /// Relates to WORDSNET-4879 Checks the upper bound of the allowed length of the watermark text.
        /// </summary>
        [TestCase(199)]
        [TestCase(200)]
        [TestCase(201)]
        public void Test4879TextWatermarkUpperBoundTextLength(int countChars)
        {
            CheckWatermarkText(GenerateStringSpecificLength(countChars));
        }

        /// <summary>
        /// Relates to WORDSNET-4879 Checks the opacity of a text watermark
        /// when the semitrasparent property was set to false.
        /// </summary>
        [Test]
        public void Test4879TextWatermarkIsSemitrasparentFalse()
        {
            Document doc = new Document();
            TextWatermarkOptions options = new TextWatermarkOptions();
            options.IsSemitrasparent = false;

            doc.Watermark.SetText("Test", options);
            Shape watermark = doc.FirstSection.HeadersFooters[HeaderFooterType.HeaderFirst].Shapes[0];

            Assert.That(watermark.Fill.Opacity, Is.EqualTo(1));
        }

        /// <summary>
        /// Relates to WORDSNET-4879 Checks the default image watermark properties.
        /// </summary>
        [Test]
        public void Test4879ImageWatermarkDefaults()
        {
            const string pathImage = @"Model\Shape\Watermark\Test4879WatermarkImage.png";
            Document doc = new Document();
            doc.Watermark.SetImage(BitmapPal.LoadNativeImage(TestUtil.GetInTestDataPath(pathImage)));

            Shape watermark = doc.FirstSection.HeadersFooters[HeaderFooterType.HeaderFirst].Shapes[0];

            Assert.That(System.Math.Round(watermark.ImageData.Contrast, 2, MidpointRounding.AwayFromZero), Is.EqualTo(0.15));
            Assert.That(System.Math.Round(watermark.ImageData.Brightness, 2, MidpointRounding.AwayFromZero), Is.EqualTo(0.85));
            Assert.That(watermark.Width, Is.EqualTo(432));
            Assert.That(watermark.Height, Is.EqualTo(432));

            CheckStandardWatermarkAttributes(watermark, "WordPictureWatermark");
        }

        /// <summary>
        /// Relates to WORDSNET-4879 Checks the different scales of the image watermark.
        /// Relates to WORDSJAVA-2386 Different scales of the image watermark.
        /// </summary>
        [TestCase(0, 432, 432)] // Default values - Auto.
        [TestCase(65.5, 4127.73, 4127.73)] // Max values.
        [TestCase(0.01, 0.63, 0.63)] // Min values.
        [TestCase(1, 63.02, 63.02)]
        [TestCase(-1, 432, 432)]
        [TestCase(65.51, 432, 432)]
        public void Test4879ImageWatermarkScale(double scale, double width, double height)
        {
            const string pathImage = @"Model\Shape\Watermark\Test4879WatermarkImage.png";
            Document doc = new Document();
            ImageWatermarkOptions options = new ImageWatermarkOptions();

            try
            {
                options.Scale = scale;

                if ((scale == -1) || (scale == 65.51))
                    Assert.Fail();

                doc.Watermark.SetImage(TestUtil.GetInTestDataPath(pathImage), options);
                Shape watermark = doc.FirstSection.HeadersFooters[HeaderFooterType.HeaderFirst].Shapes[0];

                double widthDiff = System.Math.Abs(width - watermark.Width);
                double heightDiff = System.Math.Abs(height - watermark.Height);

                Assert.That(widthDiff, Is.LessThan(1.0));
                Assert.That(heightDiff, Is.LessThan(1.0));
            }
            catch (ArgumentOutOfRangeException)
            {
                if ((scale != -1) && (scale != 65.51))
                    Assert.Fail();
            }
        }

        /// <summary>
        /// Relates to WORDSNET-4879 Checks the properties of a image watermark
        /// when the washout property was set to false.
        /// </summary>
        [Test]
        public void Test4879ImageWatermarkIsWashoutFalse()
        {
            const string pathImage = @"Model\Shape\Watermark\Test4879WatermarkImage.png";
            Document doc = new Document();
            ImageWatermarkOptions options = new ImageWatermarkOptions();
            options.IsWashout = false;

            doc.Watermark.SetImage(BitmapPal.LoadNativeImage(TestUtil.GetInTestDataPath(pathImage)), options);

            Shape watermark = doc.FirstSection.HeadersFooters[HeaderFooterType.HeaderFirst].Shapes[0];

            Assert.That(watermark.ImageData.Contrast, Is.EqualTo(0.5));
            Assert.That(watermark.ImageData.Brightness, Is.EqualTo(0.5));
        }

        /// <summary>
        /// Relates to WORDSNET-4879 Checks that image cannot be null.
        /// </summary>
        [Test]
        public void Test4879ImageWatermarkImageNull()
        {
            try
            {
                new Document().Watermark.SetImage(null);
                Assert.Fail();
            }
            catch (ArgumentNullException) { }
        }

        /// <summary>
        /// Relates to WORDSNET-4879 Checks that if set null instead of <see cref="ImageWatermarkOptions"/> or
        /// <see cref="TextWatermarkOptions"/>, the parameters will be by default.
        /// </summary>
        [Test]
        public void Test4879OptionsIsNull()
        {
            const string pathImage = @"Model\Shape\Watermark\Test4879WatermarkImage.png";
            Document doc = new Document();

            doc.Watermark.SetText("Test", null);
            Shape shape = doc.FirstSection.HeadersFooters[HeaderFooterType.HeaderPrimary].Shapes[0];
            CheckDefaultTextWatermark(shape);

            doc.Watermark.SetImage(BitmapPal.LoadNativeImage(TestUtil.GetInTestDataPath(pathImage)), null);
            shape = doc.FirstSection.HeadersFooters[HeaderFooterType.HeaderPrimary].Shapes[0];
            CheckDefaultImageWatermark(shape);
        }

        /// <summary>
        /// WORDSNET-23231 Shape.Id is duplicated for watermarks inserted by Aspose.Words.
        /// AW clones and inserts watermark shape to headers with original name and identifier.
        /// Need update shape names ans id's to fix the problem.
        /// </summary>
        [TestCase(true)]
        [TestCase(false)]
        public void Test23231(bool text)
        {
            Document doc = new Document();

            if (text)
                doc.Watermark.SetText("Watermark");
            else
#if NETSTANDARD
                doc.Watermark.SetImage(SkiaSharp.SKBitmap.Decode(ImageUtil.GetNoImageBytes()));
#elif CPLUSPLUS
                doc.Watermark.SetImage(Image.FromStream(ImageUtil.GetNoImageStream()));
#else
                doc.Watermark.SetImage(new BitmapPal(ImageUtil.GetNoImageBytes()).NativeImage);
#endif

            HashSetGeneric<int> shapeIds = new HashSetGeneric<int>();
            foreach(Section sect in doc.Sections)
            {
                foreach(HeaderFooter hf in sect.HeadersFooters)
                {
                    Assert.That(hf.Shapes.Count, Is.EqualTo(1));
                    Shape shape = hf.Shapes[0];

                    AssertIsWatermark(shape);

                    if (shapeIds.Contains(shape.Id))
                        throw new Exception("duplicate shape id.");

                    shapeIds.Add(shape.Id);

                    string name = shape.HasImage ? Watermark.ImageNamePrefix : Watermark.TextNamePrefix;
                    name += shape.Id.ToString();

                    Assert.That(shape.Name, Is.EqualTo(name));
                }
            }
        }

        /// <summary>
        /// WORDSNET-24516 NullReferenceException is thrown when check watermark type in the document without header/footer.
        /// Added resilience for the document without header/footer.
        /// </summary>
        [Test]
        public void Test24516()
        {
            Document doc = new Document();
            Assert.That(doc.Watermark.Type, Is.EqualTo(WatermarkType.None));
        }

        /// <summary>
        /// WORDSNET-24431 Watermark is displayed over the text
        /// Now the <see cref="ShapeBase.BehindText"/> property of a watermark shape is set to 'true' like MS Word does.
        /// </summary>
        [Test]
        public void Test24431()
        {
            Document doc = new Document();

            doc.Watermark.SetText(new string('i', 200));

            DocumentBuilder builder = new DocumentBuilder(doc);
            builder.MoveToHeaderFooter(HeaderFooterType.HeaderPrimary);
            builder.Writeln(new string('i', 2000));
            builder.MoveToHeaderFooter(HeaderFooterType.FooterPrimary);
            builder.Writeln(new string('i', 2000));
            builder.MoveToSection(0);
            builder.Writeln(new string('i', 2000));

            Shape shape = doc.FirstSection.HeadersFooters[HeaderFooterType.HeaderPrimary].Shapes[0];
            Assert.That(shape.BehindText, Is.True);

            TestUtil.SaveCheckGoldExportOnly(doc, @"Model\Shape\Watermark\Test24431.docx");
        }


        // FOSS: Test27938 (set watermark image from an SVG stream) removed — SVG image
        // decoding is part of the removed image-rendering subsystem, so loading the .svg
        // watermark throws CantCreateBitmapException. Raster (PNG) image watermarks are
        // still covered by Test4879ImageWatermarkDefaults / Test4879ImageWatermarkScale.

        /// <summary>
        /// WORDSNET-28260 Exception is occurred in Document.Watermark.Type property.
        /// Added resilience for the completely empty document without any sections.
        /// </summary>
        [Test]
        public void Test28260()
        {
            Document doc = new Document();
            doc.Sections.Clear();
            Assert.That(doc.Watermark.Type, Is.EqualTo(WatermarkType.None));
        }

        private static void AssertIsWatermark(Shape shape)
        {
            if (shape.MarkupLanguage != ShapeMarkupLanguage.Vml)
                Assert.Fail();

            bool isWatermark = (shape.Name.Contains("PowerPlusWaterMarkObject") && shape.IsWordArt)
                               || (shape.Name.Contains("WordPictureWatermark") && shape.IsImage);

            if (isWatermark)
            {
                HeaderFooter headerFooter = shape.GetStoryAncestor(NodeType.HeaderFooter) as HeaderFooter;
                isWatermark = (headerFooter != null) && headerFooter.IsHeader;
            }

            Assert.That(isWatermark, Is.True);
        }

        private static void CheckStandardWatermarkAttributes(Shape watermark, string requiredNamePart)
        {
            Assert.That(watermark.RelativeHorizontalPosition, Is.EqualTo(RelativeHorizontalPosition.Margin));
            Assert.That(watermark.RelativeVerticalPosition, Is.EqualTo(RelativeVerticalPosition.Margin));
            Assert.That(watermark.WrapType, Is.EqualTo(WrapType.None));
            Assert.That(watermark.VerticalAlignment, Is.EqualTo(VerticalAlignment.Center));
            Assert.That(watermark.HorizontalAlignment, Is.EqualTo(HorizontalAlignment.Center));

            Assert.That(watermark.Name.Contains(requiredNamePart), Is.True);
        }

        private static string GenerateStringSpecificLength(int countChars)
        {
            const char ch = 'a';
            StringBuilder builder = new StringBuilder(countChars);
            builder.Append(ch, countChars);
            return builder.ToString();
        }

        private static void CheckWatermarkText(string text)
        {
            try
            {
                Document doc = new Document();
                doc.Watermark.SetText(text);

                if ((text == null) || (text == string.Empty) || (text.Trim().Length == 0) || (text.Length > 200))
                    Assert.Fail();
            }
            catch (ArgumentOutOfRangeException)
            {
                if ((text != string.Empty) && (text.Trim().Length != 0) && (text.Length < 200))
                    Assert.Fail();
            }
            catch (ArgumentNullException)
            {
                if (text != null)
                    Assert.Fail();
            }
        }

        private static void CheckDefaultTextWatermark(Shape watermark)
        {
            Assert.That(watermark.Fill.Opacity, Is.EqualTo(0.5));
            Assert.That(watermark.Fill.Color.ToArgb(), Is.EqualTo(Color.Silver.ToArgb()));
            Assert.That(watermark.StrokeColor.ToArgb(), Is.EqualTo(Color.Silver.ToArgb()));
            Assert.That(watermark.TextPath.FontFamily, Is.EqualTo("Calibri"));
            Assert.That(watermark.Rotation, Is.EqualTo(315));
            Assert.That(new[] { watermark.Width, watermark.Height }, Is.EqualTo(new[] { 351.42, 259.52 }));

            CheckStandardWatermarkAttributes(watermark, "PowerPlusWaterMarkObject");
        }

        private static void CheckDefaultImageWatermark(Shape watermark)
        {
            Assert.That(System.Math.Round(watermark.ImageData.Contrast, 2, MidpointRounding.AwayFromZero), Is.EqualTo(0.15));
            Assert.That(System.Math.Round(watermark.ImageData.Brightness, 2, MidpointRounding.AwayFromZero), Is.EqualTo(0.85));
            Assert.That(watermark.Width, Is.EqualTo(432));
            Assert.That(watermark.Height, Is.EqualTo(432));

            CheckStandardWatermarkAttributes(watermark, "WordPictureWatermark");
        }
    }
}
