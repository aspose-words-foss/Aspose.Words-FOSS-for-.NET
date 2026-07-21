// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 08/10/2015 by Dmitry Bormashov

using Aspose.Words.Drawing.Core.Dml.Geometries;
using NUnit.Framework;

namespace Aspose.Words.Tests.Dml.Geometries
{
    [TestFixture]
    public class TestPresetTextWarpXmlRepository
    {
        [Test]
        public void GetPresetTextWarpXml_NotExistingPreset_NullReturned()
        {
            // Act
            string result = gRepository.GetPresetTextWarpXml("abra-kadabra");
            // Assert
            Assert.That(result, Is.Null);
        }

        [Test]
        public void GetPresetTextWarpXml_FirstPreset_XmlReturned()
        {
            // Act
            string result = gRepository.GetPresetTextWarpXml("textArchDown");
            // Assert
            Assert.That(result.StartsWith("<textArchDown>"), Is.True);
            Assert.That(result.EndsWith("</textArchDown>"), Is.True);
        }

        [Test]
        public void GetPresetTextWarpXml_LastPreset_XmlReturned()
        {
            // Act
            string result = gRepository.GetPresetTextWarpXml("textWave4");
            // Assert
            Assert.That(result.StartsWith("<textWave4>"), Is.True);
            Assert.That(result.EndsWith("</textWave4>"), Is.True);
        }

        private static readonly PresetTextWarpXmlRepository gRepository = new PresetTextWarpXmlRepository();
    }
}