// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 18/04/2011 by Alexey Titov

using Aspose.Words.Drawing.Core.Dml.Fills;
using Aspose.Words.Tests.Unified;
using NUnit.Framework;

namespace Aspose.Words.Tests.Dml.Fills
{
    [TestFixture]
    public class TestDmlBlip
    {
        [Test]
        public void Clone()
        {
            DmlBlip blip = new DmlBlip();
            //Arrange
            byte[] imageBytes = new byte[] {0, 1, 2};
            blip.EmbedImage = imageBytes;
            blip.ImageLink = "linked";
            blip.CompressionState = DmlCompressionState.HighQualityPrint;
            //Act
            DmlBlip result = blip.Clone();
            //Assert
            Assert.That(result.EmbedImage, Is.EqualTo(imageBytes));
            Assert.That(result.ImageLink, Is.EqualTo("linked"));
            Assert.That(result.CompressionState, Is.EqualTo(DmlCompressionState.HighQualityPrint));
        }

        [Test]
        public void GetImage_ImageNotDefined_NullReturned()
        {
            DmlBlip blip = new DmlBlip();
            // Arrange
            // Act
            byte[] result = blip.GetImageBytes();
            // Assert
            Assert.That(result, Is.Null);
        }

        [Test]
        public void GetImage_EmbededImage_EmbededImageReturned()
        {
            DmlBlip blip = new DmlBlip();
            // Arrange
            byte[] image = ArrayUtil.EmptyByteArray;
            // Act
            blip.EmbedImage = image;
            byte[] result = blip.GetImageBytes();
            // Assert
            Assert.That(result, Is.EqualTo(image));
        }

        [Test]
        public void GetImage_LinkedImage_ImageReturned()
        {
            DmlBlip blip = new DmlBlip();
            // Arrange
            Document doc = new Document();
            blip.Document = doc;
            blip.Document.ResourceLoadingCallback = new UserProvidedDataHandler();
            blip.ImageLink = "rid1";
            byte[] image = new byte[1];
            image[0] = 192;
            // Act
            byte[] result = blip.GetImageBytes();
            // Assert
            Assert.That(result.Length, Is.EqualTo(5)); 
        }

        [Test]
        public void GetImage_EmbededAndLinkedImageDefined_EmbededImageReturned()
        {
            DmlBlip blip = new DmlBlip();
            // Arrange
            byte[] image = new byte[1];
            image[0] = 192;
            // Act
            blip.EmbedImage = image;
            byte[] result = blip.GetImageBytes();
            // Assert
            Assert.That(result[0], Is.EqualTo(192));
        }


        [Test]
        public void GetImage_ExceptionOccuredDuringReading_NullReturned()
        {
            DmlBlip blip = new DmlBlip();
            // Arrange
            // Act
            byte[] result = blip.GetImageBytes();
            // Assert
            Assert.That(result, Is.Null);
        }
    }
}
