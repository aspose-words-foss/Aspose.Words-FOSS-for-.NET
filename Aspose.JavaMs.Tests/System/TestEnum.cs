// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 28/10/2011 by Konstantin Sidorenko
// 2015/12/28 by Anatoliy Sidorenko

using System;
using NUnit.Framework;

namespace Aspose.JavaMs.Tests.System
{
    [TestFixture]
    public class TestEnum
    {
        [Test]
        public void TestGetName()
        {
            Assert.That(Enum.GetName(typeof(DayOfWeek), DayOfWeek.Sunday), Is.EqualTo("Sunday"));
            Assert.That(Enum.GetName(typeof(DayOfWeek), DayOfWeek.Monday), Is.EqualTo("Monday"));
            Assert.That(Enum.GetName(typeof(DayOfWeek), DayOfWeek.Tuesday), Is.EqualTo("Tuesday"));
            Assert.That(Enum.GetName(typeof(DayOfWeek), DayOfWeek.Wednesday), Is.EqualTo("Wednesday"));
            Assert.That(Enum.GetName(typeof(DayOfWeek), DayOfWeek.Thursday), Is.EqualTo("Thursday"));
            Assert.That(Enum.GetName(typeof(DayOfWeek), DayOfWeek.Friday), Is.EqualTo("Friday"));
            Assert.That(Enum.GetName(typeof(DayOfWeek), DayOfWeek.Saturday), Is.EqualTo("Saturday"));
        }
    }
}
