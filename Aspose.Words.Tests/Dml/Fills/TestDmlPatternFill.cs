// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 18/04/2011 by Alexey Titov

using System.Drawing.Drawing2D;
using Aspose.Drawing;
using Aspose.Words.Drawing;
using Aspose.Words.Drawing.Core.Dml.Colors;
using Aspose.Words.Drawing.Core.Dml.Fills;
using Aspose.Words.Drawing.Core.Dml.Styles;
using NUnit.Framework;

namespace Aspose.Words.Tests.Dml.Fills
{
    [TestFixture]
    public class TestDmlPatternFill
    {
        [Test]
        public void ApplyStyleColor_StyleColorAppliedToAnyPlaceholder()
        {
            DmlPatternFill fill = new DmlPatternFill();
            //Arrange
            fill.ForegroundColor = new DmlPlaceholderColor();
            fill.BackgroundColor = new DmlPlaceholderColor();
            DmlSystemColor color = new DmlSystemColor();
            //Act
            fill.ApplyStyleColor(color);
            //Assert
            Assert.That(((DmlPlaceholderColor)fill.BackgroundColor).StyleColor, Is.EqualTo(color));
            Assert.That(((DmlPlaceholderColor)fill.ForegroundColor).StyleColor, Is.EqualTo(color));
        }

    }
}
