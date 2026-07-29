// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 07/10/2016 by Alexander Zhiltsov

using NUnit.Framework;

namespace Aspose.Words.Tests.Model
{
    /// <summary>
    /// Tests the <see cref="DocumentBase.People"/> collection and the <see cref="PersonInternal"/> type which objects
    /// it stores.
    /// </summary>
    [TestFixture]
    public class TestPeople
    {
        [TestFixtureSetUp]
        public void SetUp()
        {
            TestUtil.SetUpTests();
        }

        /// <summary>
        /// Tests cloning of <see cref="PersonCollectionInternal"/>.
        /// </summary>
        [Test]
        public void TestCloning()
        {
            Document source = new Document();
            PersonInternal person = source.People.Add("John", ContactIdentityProvider.ActiveDirectory, "1");

            Document cloned = source.Clone();
            Assert.That(cloned.People.Count, Is.EqualTo(1));

            // Change object of the source document and check that new values are not applied to
            // the object of the cloned document.
            person.Author = "Jack";
            person.IdentityProvider = ContactIdentityProvider.WindowsLiveId;
            person.UserId = "000";

            PersonInternal clonedPerson = cloned.People[0];
            Assert.That(clonedPerson.Author, Is.EqualTo("John"));
            Assert.That(clonedPerson.IdentityProvider, Is.EqualTo(ContactIdentityProvider.ActiveDirectory));
            Assert.That(clonedPerson.UserId, Is.EqualTo("1"));
        }


        /// <summary>
        /// Checks properties of the <see cref="PersonInternal"/> object.
        /// </summary>
        private static void CheckPerson(PersonInternal person, string expectedAuthor, ContactIdentityProvider expectedProvider,
            string expectedId)
        {
            Assert.That(person.Author, Is.EqualTo(expectedAuthor));
            Assert.That(person.IdentityProvider, Is.EqualTo(expectedProvider));
            Assert.That(person.UserId, Is.EqualTo(expectedId));
        }

        // FOSS: TestWarningOnSavingToOldFormat removed. It asserted the "people part not supported" data-loss
        // warning produced when saving to ECMA-375/DOC/RTF/WML - all removed formats. Markdown/Text don't
        // raise that particular warning, so the scenario can't be reproduced in FOSS.

    }
}
