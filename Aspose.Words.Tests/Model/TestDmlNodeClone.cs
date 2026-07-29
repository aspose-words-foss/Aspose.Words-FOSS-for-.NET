// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 29/07/2014 by Andrey Noskov

using System.Collections.Generic;
using Aspose.Words.Drawing;
using Aspose.Words.Drawing.Core.Dml;
using Aspose.Words.Drawing.Core.Dml.Common;
using Aspose.Words.Drawing.Core.Dml.Effects;
using Aspose.Words.Drawing.Core.Dml.Fills;
using Aspose.Words.Drawing.Core.Dml.Scene3D;
using NUnit.Framework;

namespace Aspose.Words.Tests.Model
{
    /// <summary>
    /// Test DmlNode cloning.
    /// </summary>
    [TestFixture]
    public class TestDmlNodeClone
    {
        [TestFixtureSetUp]
        public void SetUp()
        {
            TestUtil.SetUpTests();
        }

        [Test]
        public void TestDmlNodesClone()
        {
            DmlPicture dmlPicture = new DmlPicture();
            DmlPicture clonedPic = (DmlPicture)dmlPicture.Clone(true, null);
            Assert.That(clonedPic, IsNot.SameAs(dmlPicture));

            DmlShape dmlShape = new DmlShape(DmlNodeType.Shape);
            DmlShape clonedShape = (DmlShape)dmlShape.Clone(true, null);
            Assert.That(clonedShape, IsNot.SameAs(dmlShape));

            DmlLockedCanvas dmlLockedCanvas = new DmlLockedCanvas(DmlNodeType.LockedCanvas);
            DmlLockedCanvas clonedLoc = (DmlLockedCanvas)dmlLockedCanvas.Clone(true, null);
            Assert.That(clonedLoc, IsNot.SameAs(dmlLockedCanvas));

            DmlGroupShape dmlGroupShape = new DmlGroupShape(DmlNodeType.GroupShape);
            DmlGroupShape clonedGroupShape = (DmlGroupShape)dmlGroupShape.Clone(true, null);
            Assert.That(clonedGroupShape, IsNot.SameAs(dmlGroupShape));

            DmlBlipFill dmlBlipFill = new DmlBlipFill();
            dmlBlipFill.Blip = new DmlBlip();
            dmlBlipFill.BlipFillMode = new DmlBlipFillTile();
            DmlBlipFill clonedBlipFill = (DmlBlipFill)dmlBlipFill.Clone();
            Assert.That(clonedBlipFill, IsNot.SameAs(dmlBlipFill));
        }

        [Test]
        public void TestDmlRotation3DClone()
        {
            DmlRotation3D rotation3D = new DmlRotation3D();

            rotation3D.Latitude = DmlAngle.FromDegrees(50);
            rotation3D.Longitude = DmlAngle.FromDegrees(100);
            rotation3D.Revolution = DmlAngle.FromDegrees(150);

            DmlRotation3D cloned = rotation3D.Clone();

            Assert.That(cloned, IsNot.SameAs(rotation3D));

            Assert.That(cloned.Latitude.ValueInDegrees, Is.EqualTo(50));
            Assert.That(cloned.Longitude.ValueInDegrees, Is.EqualTo(100));
            Assert.That(cloned.Revolution.ValueInDegrees, Is.EqualTo(150));
        }

        [Test]
        public void TestDmlEffectsClone()
        {
            DmlAlphaBiLevelEffect effect = new DmlAlphaBiLevelEffect();
            
            // Change something in the original object.
            effect.Threshold = 40.0d;
            
            DmlAlphaBiLevelEffect cloned = (DmlAlphaBiLevelEffect)effect.Clone();

            effect.Threshold = 20.0d;
            
            // Check values in the original and cloned objects.
            Assert.That(cloned.Threshold, Is.EqualTo(40.0d));
            Assert.That(effect.Threshold, Is.EqualTo(20.0d));

            Assert.That(cloned, IsNot.SameAs(effect));
        }

        [Test]
        public void TestDmlBlipFillClone()
        {
            DmlBlipFill blipFill = new DmlBlipFill();
            blipFill.RotateWithShape = true;
            blipFill.Blip = new DmlBlip();
            blipFill.Blip.ImageLink = "TestLinkedPictureReference";

            IList<DmlEffect> effects = new List<DmlEffect>();
            DmlColorChangeEffect dmlColor = new DmlColorChangeEffect();
            dmlColor.ConsiderAlphaValues = true;
            effects.Add(dmlColor);
            effects.Add(new DmlAlphaInverseEffect());

            blipFill.Blip.Effects = effects;

            DmlBlipFill cloned = (DmlBlipFill)blipFill.Clone();

            blipFill.RotateWithShape = false;
            Assert.That(cloned.Blip.Effects.Count, Is.EqualTo(2));

            // Check values in the original and cloned objects.
            Assert.That(cloned.RotateWithShape, Is.True);
            Assert.That(blipFill.RotateWithShape, Is.False);
            Assert.That(cloned.Blip, IsNot.SameAs(blipFill.Blip));
            Assert.That(cloned.Blip.Effects, IsNot.SameAs(blipFill.Blip.Effects));
            Assert.That(cloned, IsNot.SameAs(blipFill));
        }

    }
}
