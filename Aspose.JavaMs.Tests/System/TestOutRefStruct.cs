// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 14/02/2018 by Anatoly Sidorenko

using System.Drawing;
using NUnit.Framework;

namespace Aspose.JavaMs.Tests.System
{
    [TestFixture]
    public class TestOutRefStruct
    {
        [Test]
        public void TestStructOutRefCtor()
        {
            Point p = new Point(1, 2);
            PointF pf = new PointF(1.0f, 2.0f);
            SizeF sf = new SizeF();
            Size s = new Size();

            MethodRef(out sf, out s, ref p, ref pf);

            Assert.That(2.0f, Is.EqualTo(sf.Height));
            Assert.That(1.0f, Is.EqualTo(sf.Width));

            Assert.That(2, Is.EqualTo(s.Height));
            Assert.That(1, Is.EqualTo(s.Width));

            Assert.That(2.0f, Is.EqualTo(pf.X));
            Assert.That(1.0f, Is.EqualTo(pf.Y));

            Assert.That(2, Is.EqualTo(p.X));
            Assert.That(1, Is.EqualTo(p.Y));
        }

        [Test]
        public void TestStructOutRefProperty()
        {
            Point p = new Point(1, 2);
            PointF pf = new PointF(3.0f, 2.0f);
            SizeF sf = new SizeF(0.1f, 0.2f);
            Size s = new Size(4, 5);

            MethodRef2(p, ref sf, ref s, ref p, ref pf);

            Assert.That(2.0f, Is.EqualTo(sf.Height));
            Assert.That(1.0f, Is.EqualTo(sf.Width));

            Assert.That(2, Is.EqualTo(s.Height));
            Assert.That(1, Is.EqualTo(s.Width));

            Assert.That(1.0f, Is.EqualTo(pf.X));
            Assert.That(2.0f, Is.EqualTo(pf.Y));

            Assert.That(2, Is.EqualTo(p.X));
            Assert.That(1, Is.EqualTo(p.Y));
        }
        
        private void MethodRef(out SizeF outSizeF, out Size outSize, ref Point refPoint, ref PointF refPointf)
        {
            PointF pfj = refPointf;
            refPointf = new PointF(pfj.Y, pfj.X);
            Point pj = refPoint;
            refPoint = new Point(pj.Y, pj.X);
            outSizeF = new SizeF(pfj);
            outSize = new Size(pj);
        }
        
        private void MethodRef2(Point point, ref SizeF refSizeF, ref Size refSize, ref Point refPoint, ref PointF refPointf)
        {
            refPoint.X = point.Y;
            refPoint.Y = point.X;
            refPointf.X = point.X;
            refPointf.Y = point.Y;
            refSize.Width = point.X;
            refSize.Height = point.Y;
            refSizeF.Width = point.X;
            refSizeF.Height = point.Y;
        }
    }
}
