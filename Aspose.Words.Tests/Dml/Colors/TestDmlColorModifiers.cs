// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 22/02/2011 by Alexey Titov

using Aspose.Drawing;
using Aspose.Words.Drawing.Core.Dml.Colors.Modifiers;
using Aspose.Words.Drawing.Core.Dml.Common;
using NUnit.Framework;

namespace Aspose.Words.Tests.Dml.Colors
{
    [TestFixture]
    public class TestDmlColorModifiers
    {
        [Test]
        public void Modify_DmlAlpha_ModifiedCorrectly()
        {
            DrColor result = ModifyColor(new DmlAlpha(), 0.40, gColor);
            //Assert
            Assert.That(result.A, Is.EqualTo(102.0));
        }

        [Test]
        public void Modify_DmlAlphaModulation_ModifiedCorrectly()
        {
            DrColor result = ModifyColor(new DmlAlphaModulation(), 2, gColor);
            //Assert
            Assert.That(result.A, Is.EqualTo(50.0));
        }

        [Test]
        public void Modify_DmlAlphaOffset_ModifiedCorrectly()
        {
            DrColor result = ModifyColor(new DmlAlphaOffset(), -gColor.A / 255.0, gColor);
            //Assert
            Assert.That(result.A, Is.EqualTo(0.0));
        }

        [Test]
        public void Modify_DmlShade_ModifiedCorrectly()
        {
            DrColor result = ModifyColor(new DmlShade(), 0.25, gColor);
            //Assert
            Assert.That(result, Is.EqualTo(new DrColor(gColor.A, 0x32, 0x4e, 0x6a)));
        }

        [Test]
        public void Modify_DmlTint_ModifiedCorrectly()
        {
            DrColor result = ModifyColor(new DmlTint(), 0.3, gColor);
            //Assert
            Assert.That(result, Is.EqualTo(new DrColor(gColor.A, 0xdf, 0xe6, 0xf0)));
        }

        [Test]
        public void Modify_DmlBlue_ModifiedCorrectly()
        {
            DrColor result = ModifyColor(new DmlBlue(), 0.4, gColor);
            //Assert
            Assert.That(result.B, Is.EqualTo(170.0));
        }

        [Test]
        public void Modify_DmlBlueModulation_ModifiedCorrectly()
        {
            DrColor result = ModifyColor(new DmlBlueModulation(), 0.5, gColor);
            //Assert
            Assert.That(result.B, Is.EqualTo(146.0));
        }

        [Test]
        public void Modify_DmlBlueOffset_ModifiedCorrectly()
        {
            DrColor result = ModifyColor(new DmlBlueOffset(), -gColor.B / 255.0, gColor);
            //Assert
            Assert.That(result.B, Is.EqualTo(0.0));
        }

        [Test]
        public void Modify_DmlRed_ModifiedCorrectly()
        {
            DrColor result = ModifyColor(new DmlRed(), 0.4, gColor);
            //Assert
            Assert.That(result.R, Is.EqualTo(170.0));
        }

        [Test]
        public void Modify_DmlRedModulation_ModifiedCorrectly()
        {
            DrColor result = ModifyColor(new DmlRedModulation(), 0.5, gColor);
            //Assert
            Assert.That(result.R, Is.EqualTo(71.0));
        }

        [Test]
        public void Modify_DmlRedOffset_ModifiedCorrectly()
        {
            DrColor result = ModifyColor(new DmlRedOffset(), -gColor.R / 255.0, gColor);
            //Assert
            Assert.That(result.R, Is.EqualTo(0.0));
        }

        [Test]
        public void Modify_DmlGreen_ModifiedCorrectly()
        {
            DrColor result = ModifyColor(new DmlGreen(), 0.4, gColor);
            //Assert
            Assert.That(result.G, Is.EqualTo(170.0));
        }

        [Test]
        public void Modify_DmlGreenModulation_ModifiedCorrectly()
        {
            DrColor result = ModifyColor(new DmlGreenModulation(), 0.5, gColor);
            //Assert
            Assert.That(result.G, Is.EqualTo(109));
        }

        [Test]
        public void Modify_DmlGreenOffset_ModifiedCorrectly()
        {
            DrColor result = ModifyColor(new DmlGreenOffset(), -gColor.G / 255.0, gColor);
            //Assert
            Assert.That(result.G, Is.EqualTo(0.0));
        }

        [Test]
        public void Modify_DmlInverse_ModifiedCorrectly()
        {
            //Arrange
            DmlInverse modificator = new DmlInverse();
            //Act
            DrColor result = modificator.Modify(gColor);
            //Assert
            DrColor expected = new DrColor(
                gColor.A,
                255 - gColor.R,
                255 - gColor.G,
                255 - gColor.B);
            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void Modify_DmlComplement_ModifiedCorrectly()
        {
            //Arrange
            DmlComplement modificator = new DmlComplement();
            //Act
            DrColor result = modificator.Modify(gColor);
            //Assert
            DrColor expected = DrColor.FromArgb(0xC7, 0x96, 0x64);
            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void Modify_DmlHue_ModifiedCorrectly()
        {
            //Arrange
            DmlHue modificator = new DmlHue();
            modificator.Value = DmlAngle.FromDegrees(45);
            //Act
            DrColor result = modificator.Modify(gColor);
            //Assert
            DrColor expected = new DrColor(gColor.A, 199, 175, 100);
            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void Modify_DmlHueOffset_ModifiedCorrectly()
        {
            //Arrange
            DmlHueOffset modificator = new DmlHueOffset();
            modificator.Value = DmlAngle.FromDegrees(-110);
            //Act
            DrColor result = modificator.Modify(gColor);
            //Assert
            DrColor expected = new DrColor(gColor.A, 133, 199, 100);
            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void Modify_DmlHueModulation_ModifiedCorrectly()
        {
            DrColor result = ModifyColor(new DmlHueModulation(), 0.5, gColor);
            //Assert
            Assert.That(result, Is.EqualTo(new DrColor(gColor.A, 125, 199, 100)));
        }

        [Test]
        public void Modify_DmlLuminance_ModifiedCorrectly()
        {
            DrColor result = ModifyColor(new DmlLuminance(), 0.25, gColor);
            //Assert
            Assert.That(result, Is.EqualTo(new DrColor(gColor.A, 33, 63, 94)));
        }

        [Test]
        public void Modify_DmlLuminanceModulation_ModifiedCorrectly()
        {
            DrColor result = ModifyColor(new DmlLuminanceModulation(), 0.25, gColor);
            //Assert
            Assert.That(result, Is.EqualTo(new DrColor(gColor.A, 19, 37, 55)));
        }

        [Test]
        public void Modify_DmlLuminanceOffset_ModifiedCorrectly()
        {
            DrColor result = ModifyColor(new DmlLuminanceOffset(), -0.09, gColor);
            //Assert
            Assert.That(result, Is.EqualTo(new DrColor(gColor.A, 66, 127, 187)));
        }

        [Test]
        public void Modify_DmlSaturation_ModifiedCorrectly()
        {
            DrColor result = ModifyColor(new DmlSaturation(), 0.25, gColor);
            //Assert
            Assert.That(result, Is.EqualTo(new DrColor(gColor.A, 123, 149, 176)));
        }

        [Test]
        public void Modify_DmlSaturationModulation_ModifiedCorrectly()
        {
            DrColor result = ModifyColor(new DmlSaturationModulation(), 0.5, gColor);
            //Assert
            Assert.That(result, Is.EqualTo(new DrColor(gColor.A, 125, 149, 175)));
        }

        /// <summary>
        /// WORDSNET-20638 Tests if the saturation modulation modifier is applied correctly.
        /// </summary>
        [Test]
        public void Modify_DmlSaturationModulation_A()
        {
            //Arrange
            DrColor[] sourceColors = new DrColor[]
            {
                new DrColor(68, 114, 196), // kleinBlueColor
                new DrColor(237, 125, 49), // cadmiumOrangeColor
                new DrColor(165, 165, 165), // quickSilverColor
                new DrColor(255, 192, 0), //amberColor
                new DrColor(230, 77, 25), //trinidadColor
                new DrColor(112, 173, 71) //rybGreenColor
            };

            DrColor[] expectedColors = new DrColor[]
            {
                new DrColor(4, 95, 255),
                new DrColor(255, 106, 0),
                new DrColor(165, 165, 165),
                new DrColor(255, 255, 0),
                new DrColor(255, 26, 0),
                new DrColor(101, 224, 19)
            };

            double[] satModValues = new double[] { 2 };

            //Assert
            AssertArrayOfModifedColors(new DmlSaturationModulation(), satModValues, sourceColors, expectedColors);
        }

        [Test]
        public void Modify_DmlSaturationOffset_ModifiedCorrectly()
        {
            DrColor result = ModifyColor(new DmlSaturationOffset(), -0.08, gColor);
            //Assert
            Assert.That(result, Is.EqualTo(new DrColor(gColor.A, 108, 149, 191)));
        }

        /// <summary>
        /// WORDSNET-20719 Tests if the saturation offset is correctly applied for <see cref="DrColor"/> with zero saturation.
        /// </summary>
        [Test]
        public void Modify_DmlSaturationOffset_ZeroSaturation()
        {
            // Color with zero saturation and lightness less than 50 (25%).
            DrColor capeCodeColor = new DrColor(64, 64, 64);
            // Color with zero saturation and lightness greater than 50 (64.7%).
            DrColor quickSilverColor = new DrColor(165, 165, 165);
            DrColor[] sourceColors = new DrColor[] { capeCodeColor, quickSilverColor };
            double[] satOffValues = new double[] { 0.5, 1.0, 1.5, 2.0 };

            DrColor[] expectedColors = new DrColor[]
            {
                // CapeCodeColor modifications.
                new DrColor (96, 32, 0),
                new DrColor (128, 0, 0),
                new DrColor (160, 0, 0),
                new DrColor (192, 0, 0),

                // QuickSilverColor modifications.
                new DrColor (210, 120, 0),
                new DrColor (254, 75, 0),
                new DrColor (255, 30, 0),
                new DrColor (255, 0, 0),
            };

            //Assert
            AssertArrayOfModifedColors(new DmlSaturationOffset(), satOffValues, sourceColors, expectedColors);
        }

        [Test]
        public void Modify_DmlGray_ModifiedCorrectly()
        {
            //Arrange
            DmlGray modificator = new DmlGray();
            //Act
            DrColor result = modificator.Modify(gColor);
            //Assert
            DrColor expected = new DrColor(gColor.A, 140, 140, 140);
            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void Modify_DmlGamma_ModifiedCorrectly()
        {
            //Arrange
            DmlGamma modificator = new DmlGamma();
            //Act
            DrColor result = modificator.Modify(gColor);
            //Assert
            DrColor expected = new DrColor(gColor.A, 0xA8, 0xCA, 0xE5);
            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void Modify_DmlInverseGamma_ModifiedCorrectly()
        {
            //Arrange
            DmlInverseGamma modificator = new DmlInverseGamma();
            //Act
            DrColor result = modificator.Modify(gColor);
            //Assert
            DrColor expected = new DrColor(gColor.A, 0x20, 0x4E, 0x93);
            Assert.That(result, Is.EqualTo(expected));
        }

        /// <summary>
        /// Compares the array <see cref = "DrColor"/> with the applied modifiers with the expected array <see cref = "DrColor"/>.
        /// </summary>
        /// <param name="modType">The specified modifier</param>
        /// <param name="modValues">The array of the modifier values</param>
        /// <param name="sourceColors">The array of source <see cref="DrColor"/></param>
        /// <param name="expectedColors">The array of expected <see cref="DrColor"/></param>
        private static void AssertArrayOfModifedColors(
            DmlPercentageBasedColorModifier modType,
            double[] modValues,
            DrColor[] sourceColors,
            DrColor[] expectedColors)
        {
            int indexOfExpectedColor = 0;

            for (int i = 0; i < sourceColors.Length; i++)
            {
                for (int j = 0; j < modValues.Length; j++)
                {
                    double modificatorValue = modValues[j];
                    DrColor result = ModifyColor(modType, modificatorValue, sourceColors[i]);
                    DrColor expectedColor = expectedColors[indexOfExpectedColor];
                    Assert.That(result, Is.EqualTo(expectedColor), string.Format("Expected ({0},{1},{2}) but was ({3},{4},{5}) ",
                       expectedColor.R, expectedColor.G, expectedColor.B, result.R, result.G, result.B));
                    indexOfExpectedColor++;
                }
            }
        }

        private static DrColor ModifyColor(DmlPercentageBasedColorModifier modifier, double value, DrColor color)
        {
            //Arrange
            modifier.Value = value;
            //Act
            return modifier.Modify(color);
        }

        private static readonly DrColor gColor = new DrColor(25, 100, 150, 200);
    }
}
