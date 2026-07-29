// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 10/10/2010 by Alexey Titov

using Aspose.Words.Drawing.Core.Dml.Path;
using NUnit.Framework;

namespace Aspose.Words.Tests.Dml.Path
{
    [TestFixture]
    public class TestDmlPath
    {
        [Test]
        public void AddPathPart_Null_NothingAdded()
        {
            DmlPath path = CreatePath();
            // Act
            path.AddPathPart(null);
            // Assert
            Assert.That(path.PathParts.Count, Is.EqualTo(0));
        }

        [Test]
        public void AddPathPart_PathPart_PathPartAdded()
        {
            DmlPath path = CreatePath();
            // Arrange
            DmlClose pathPart = new DmlClose();
            // Act
            path.AddPathPart(pathPart);
            // Assert
            Assert.That(path.PathParts.Count, Is.EqualTo(1));
            Assert.That(path.PathParts[0], Is.EqualTo(pathPart));
        }

        private static DmlPath CreatePath()
        {
            DmlPath path = new DmlPath();
            path.Height = 10;
            path.Width = 20;
            
            return path;
        }
    }
}