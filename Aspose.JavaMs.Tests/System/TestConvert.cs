// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 24/01/2007 by Konstantin Sidorenko
// 22/12/2015 by Anatoliy Sidorenko

using System;
using System.Runtime.InteropServices;
using NUnit.Framework;
using Org.BouncyCastle.Math;

namespace Aspose.JavaMs.Tests.System
{
    [TestFixture]
    public class TestConvert
    {
        [Test]
        public void TestConvertToInt32()
        {
            Assert.That(0, Is.EqualTo(Convert.ToInt32(null)));
            Assert.That(8, Is.EqualTo(Convert.ToInt32((byte)8)));
            Assert.That(9, Is.EqualTo(Convert.ToInt32((short)9)));
            Assert.That(10, Is.EqualTo(Convert.ToInt32(10)));

            Assert.That(32, Is.EqualTo(Convert.ToInt32(32L)));
            Assert.That(8, Is.EqualTo(Convert.ToInt32(8.2F)));
            Assert.That(8, Is.EqualTo(Convert.ToInt32(8.2)));

            Assert.That(333, Is.EqualTo(Convert.ToInt32("333")));
            Assert.That(1, Is.EqualTo(Convert.ToInt32(true)));
            Assert.That(0, Is.EqualTo(Convert.ToInt32(false)));

            //simple ordinal (non-value) enums
            Assert.That(0, Is.EqualTo(Convert.ToInt32(DayOfWeek.Sunday)));
            Assert.That(1, Is.EqualTo(Convert.ToInt32(DayOfWeek.Monday)));

            //former value enums
            Assert.That(1, Is.EqualTo(Convert.ToInt32(VarEnum.VT_NULL)));
            Assert.That(64, Is.EqualTo(Convert.ToInt32(VarEnum.VT_FILETIME)));
        }

        [Test]
        public void TestConvertToInt32Overflow()
        {
            long mBigPositive = (long)int.MaxValue << 2;
            long mBigNegative = (long)int.MinValue << 2;

            int countTried = 0;
            int countCatched = 0;

            try { countTried++; Convert.ToInt32(mBigPositive); }
            catch (OverflowException) { countCatched++; }

            try { countTried++; Convert.ToInt32(mBigNegative); }
            catch (OverflowException) { countCatched++; }

            try { countTried++; Convert.ToInt32((float)mBigPositive); }
            catch (OverflowException) { countCatched++; }

            try { countTried++; Convert.ToInt32((float)mBigNegative); }
            catch (OverflowException) { countCatched++; }

            try { countTried++; Convert.ToInt32((double)mBigPositive); }
            catch (OverflowException) { countCatched++; }

            try { countTried++; Convert.ToInt32((double)mBigNegative); }
            catch (OverflowException) { countCatched++; }


            try { countTried++; Convert.ToInt32((decimal)mBigPositive); }
            catch (OverflowException) { countCatched++; }

            try { countTried++; Convert.ToInt32((decimal)mBigNegative); }
            catch (OverflowException) { countCatched++; }

            try { countTried++; Convert.ToInt32(BigInteger.ValueOf(mBigPositive)); }
            catch (InvalidCastException) { countCatched++; }

            try { countTried++; Convert.ToInt32(BigInteger.ValueOf(mBigNegative)); }
            catch (InvalidCastException) { countCatched++; }

            // Java throws NumberFormatException both for overflow and bad format:

            try { countTried++; Convert.ToInt32("333.3"); }
            catch (FormatException) { countCatched++; }

            try { countTried++; Convert.ToInt32(mBigPositive.ToString()); }
#if JAVA
            catch (FormatException) { countCatched++; }
#else
            catch (OverflowException) { countCatched++; }
#endif

            try { countTried++; Convert.ToInt32(mBigNegative.ToString()); }
#if JAVA
            catch (FormatException) { countCatched++; }
#else
            catch (OverflowException) { countCatched++; }
#endif

            Assert.That(countTried, Is.EqualTo(countCatched));
        }
    }
}
