// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 26/10/2016 by Edward Voronov

using Aspose.Words.Fields;
using NUnit.Framework;

namespace Aspose.Words.Tests.Fields
{
    /// <summary>
    /// Tests how the user information fields work.
    /// </summary>
    [TestFixture]
    public class TestUserInformation : TestFieldsBase
    {
        private static readonly UserInformation gCurrentUser;

        static TestUserInformation()
        {
            gCurrentUser = new UserInformation();
            gCurrentUser.Name = "Richard A. Drake";
            gCurrentUser.Initials = "RAD";
            gCurrentUser.Address = "1112 Travis Street\rPort St Lucie, FL 33452";
        }

        [SetUp]
        public void SetupDefaultUser()
        {
            UserInformation.DefaultUser.Name = "John D. Hardy";
            UserInformation.DefaultUser.Initials = "JDH";
            UserInformation.DefaultUser.Address = "2058 Coulter Lane\rRichmond, VA 23223";
        }

        [TearDown]
        public void ClearDefaultUser()
        {
            UserInformation.DefaultUser.Name = null;
            UserInformation.DefaultUser.Initials = null;
            UserInformation.DefaultUser.Address = null;
        }


        /// <summary>
        /// Tests how the USERINITIALS field is updated.
        /// </summary>
        [Test]
        public void TestUserInitials()
        {
            Document document = TestUtil.Open(@"Fields\UserInformation\TestUserInitials.docx");
            document.FieldOptions.CurrentUser = gCurrentUser;

            document.UpdateFields();

            TestUtil.SaveCheckGold(document, @"Fields\UserInformation\TestUserInitials.docx");
        }

        /// <summary>
        /// Tests how the USERADDRESS field is updated.
        /// </summary>
        [Test]
        public void TestUserAddress()
        {
            Document document = TestUtil.Open(@"Fields\UserInformation\TestUserAddress.docx");
            document.FieldOptions.CurrentUser = gCurrentUser;

            document.UpdateFields();

            TestUtil.SaveCheckGold(document, @"Fields\UserInformation\TestUserAddress.docx");
        }

        /// <summary>
        /// Tests how the USERNAME field is updated.
        /// </summary>
        [Test]
        public void TestDefaultUserName()
        {
            DocumentBuilder builder = new DocumentBuilder();
            Field field = builder.InsertField("USERNAME");

            field.Update();

            Assert.That(field.Result, Is.EqualTo("John D. Hardy"));
        }

        /// <summary>
        /// Tests how the USERINITIALS field is updated.
        /// </summary>
        [Test]
        public void TestDefaultUserInitials()
        {
            DocumentBuilder builder = new DocumentBuilder();
            Field field = builder.InsertField("USERINITIALS");

            field.Update();

            Assert.That(field.Result, Is.EqualTo("JDH"));
        }

        /// <summary>
        /// Tests how the USERADDRESS field is updated.
        /// </summary>
        [Test]
        public void TestDefaultUserAddress()
        {
            DocumentBuilder builder = new DocumentBuilder();
            Field field = builder.InsertField("USERADDRESS");

            field.Update();

            Assert.That(field.Result, Is.EqualTo("2058 Coulter Lane\rRichmond, VA 23223"));
        }
    }
}
