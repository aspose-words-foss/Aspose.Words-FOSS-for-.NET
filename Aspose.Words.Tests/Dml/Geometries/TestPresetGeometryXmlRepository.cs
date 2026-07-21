// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 18/11/2010 by Alexey Titov

using Aspose.Words.Drawing.Core.Dml.Geometries;
using NUnit.Framework;

namespace Aspose.Words.Tests.Dml.Geometries
{
    [TestFixture]
    public class TestPresetGeometryXmlRepository
    {
        [Test]
        public void GetPresetGeometryXml_NotExistingPreset_NullReturned()
        {
            // Act
            string result = gRepository.GetPresetGeometryXml("abra-kadabra");
            // Assert
            Assert.That(result, Is.Null);
        }

        [Test]
        public void GetPresetGeometryXml_FirstPreset_XmlReturned()
        {
            // Act
            string result = gRepository.GetPresetGeometryXml("accentBorderCallout1");
            // Assert
            Assert.That(result.StartsWith("<accentBorderCallout1>"), Is.True);
            Assert.That(result.EndsWith("</accentBorderCallout1>"), Is.True);
        }

        [Test]
        public void GetPresetGeometryXml_LastPreset_XmlReturned()
        {
            // Act
            string result = gRepository.GetPresetGeometryXml("wedgeRoundRectCallout");
            // Assert
            Assert.That(result.StartsWith("<wedgeRoundRectCallout>"), Is.True);
            Assert.That(result.EndsWith("</wedgeRoundRectCallout>"), Is.True);
        }

        private static readonly PresetGeometryXmlRepository gRepository = new PresetGeometryXmlRepository();
    }
}