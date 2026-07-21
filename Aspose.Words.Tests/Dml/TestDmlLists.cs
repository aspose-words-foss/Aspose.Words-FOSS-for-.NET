// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 02/27/2015 by Alexey Noskov

using NUnit.Framework;

namespace Aspose.Words.Tests.Dml
{
    [TestFixture]
    public class TestDmlLists
    {
        [SetUp]
        public void SetUp()
        {
            TestUtil.SetUpTests();
        }

        [Test]
        public void TestDmlListsBulletTypes()
        {
            TestUtil.OpenSaveOpen(@"Model\DrawingML\TextBox\TestDmlListsBulletTypes.docx");
        }

        [Test]
        public void TestDmlListsLevels()
        {
            TestUtil.OpenSaveOpen(@"Model\DrawingML\TextBox\TestDmlListsLevels.docx");
        }

        [Test]
        public void TestDmlListsBulletColor()
        {
            TestUtil.OpenSaveOpen(@"Model\DrawingML\TextBox\TestDmlListsBulletColor.docx");
        }

        [Test]
        public void TestDmlListsBulletFont()
        {
            TestUtil.OpenSaveOpen(@"Model\DrawingML\TextBox\TestDmlListsBulletFont.docx");
        }

        [Test]
        public void TestDmlListsBulletSize()
        {
            TestUtil.OpenSaveOpen(@"Model\DrawingML\TextBox\TestDmlListsBulletSize.docx");
        }
    }
}
