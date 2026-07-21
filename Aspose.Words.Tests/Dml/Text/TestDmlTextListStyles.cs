// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 10/07/2011 by Alexey Titov

using Aspose.Words.Drawing.Core.Dml.Common;
using Aspose.Words.Drawing.Core.Dml.Text;
using NUnit.Framework;

namespace Aspose.Words.Tests.Dml.Text
{
    [TestFixture]
    public class TestDmlTextListStyles
    {
        [Test]
        public void ParagraphDefaultPropsDefined_DefinedValuesReturned()
        {
            // Arrange
            DmlTextListStyles styles = new DmlTextListStyles();
            styles.DefaultParagraphProperties.TextIdentation = 777;
            // Act
            // Assert
            Assert.That(styles.ListLevel9Style.TextIdentation, Is.EqualTo(777));
        }

        [Test]
        public void ParagraphPropsDefinedOnTopLevel_DefinedValuesReturned()
        {
            // Arrange
            DmlTextListStyles styles = new DmlTextListStyles();
            styles.ListLevel1Style.TextIdentation = 777;
            // Act
            // Assert
            Assert.That(styles.ListLevel9Style.TextIdentation, Is.EqualTo(777));
        }

        [Test]
        public void ParagraphPropsNotDefined_DefaultValuesRetuned()
        {
            // Arrange
            DmlTextListStyles styles = new DmlTextListStyles();
            // Act
            // Assert
            object expected =
                DmlParagraphPropertiesDefaults.Instance.GetProperty((int)DmlParagraphPropertiesIds.TextIdentation);
            Assert.That(styles.ListLevel9Style.TextIdentation, Is.EqualTo(expected));
        }

        [Test]
        public void ParagraphPropsOverriden_OverridenValuesReturned()
        {
            // Arrange
            DmlTextListStyles styles = new DmlTextListStyles();
            styles.DefaultParagraphProperties.TextIdentation = 777;
            styles.ListLevel9Style.TextIdentation = 666;
            // Act
            // Assert
            Assert.That(styles.ListLevel9Style.TextIdentation, Is.EqualTo(666));
        }

        [Test]
        public void RunDefaultPropsDefined_DefinedValuesReturned()
        {
            // Arrange
            DmlTextListStyles styles = new DmlTextListStyles();
            styles.DefaultParagraphProperties.DefaultRunProperties.FontSize = new DmlTextPoints(11111);
            // Act
            // Assert
            Assert.That(styles.ListLevel9Style.DefaultRunProperties.FontSize, Is.EqualTo(new DmlTextPoints(11111)));
        }

        [Test]
        public void RunPropsDefinedOnTopLevel_DefinedValuesReturned()
        {
            // Arrange
            DmlTextListStyles styles = new DmlTextListStyles();
            styles.ListLevel1Style.DefaultRunProperties.FontSize = new DmlTextPoints(11111);
            // Act
            // Assert
            Assert.That(styles.ListLevel9Style.DefaultRunProperties.FontSize, Is.EqualTo(new DmlTextPoints(11111)));
        }

        [Test]
        public void RunPropsNotDefined_DefaultValuesRetuned()
        {
            // Arrange
            DmlTextListStyles styles = new DmlTextListStyles();
            // Act
            // Assert
            object expected = DmlRunPropertiesDefaults.Instance.GetProperty((int)DmlRunPropertiesIds.Baseline);
            Assert.That(styles.ListLevel9Style.DefaultRunProperties.Baseline, Is.EqualTo(expected));
        }

        [Test]
        public void RunPropsOverriden_OverridenValuesReturned()
        {
            // Arrange
            DmlTextListStyles styles = new DmlTextListStyles();
            styles.DefaultParagraphProperties.DefaultRunProperties.FontSize = new DmlTextPoints(11111);
            styles.ListLevel9Style.DefaultRunProperties.FontSize = new DmlTextPoints(123);
            // Act
            // Assert
            Assert.That(styles.ListLevel9Style.DefaultRunProperties.FontSize, Is.EqualTo(new DmlTextPoints(123)));
        }
    }
}