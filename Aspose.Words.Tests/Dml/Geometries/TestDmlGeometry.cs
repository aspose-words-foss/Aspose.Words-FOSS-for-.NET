// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 18/11/2010 by Alexey Titov

using Aspose.Words.Drawing.Core.Dml.Geometries;
using Aspose.Words.Drawing.Core.Dml.Path;
using NUnit.Framework;

namespace Aspose.Words.Tests.Dml.Geometries
{
    [TestFixture]
    public class TestDmlGeometry
    {
        [Test]
        public void AddPath_Null_NothingAdded()
        {
            DmlGeometry geometry = new DmlGeometry();
            // Act
            geometry.AddPath(null);
            // Assert
            Assert.That(geometry.Paths.Count, Is.EqualTo(0));
        }

        [Test]
        public void AddPath_Path_PathAdded()
        {

            // Arrange
            DmlGeometry geometry = new DmlGeometry();
            DmlPath path = new DmlPath();
            // Act
            geometry.AddPath(path);
            // Assert
            Assert.That(geometry.Paths.Count, Is.EqualTo(1));
            Assert.That(geometry.Paths[0], Is.EqualTo(path));
        }

    }
}
