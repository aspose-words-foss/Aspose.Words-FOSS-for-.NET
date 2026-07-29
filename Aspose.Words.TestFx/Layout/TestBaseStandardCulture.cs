// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 09/08/2018 by Denis Shvydkiy

using Aspose.Common;
using NUnit.Framework;

namespace Aspose.Words.Tests.Layout
{
    /// <summary>
    ///  The base class for the Layout tests.
    /// </summary>
    /// <remarks>C++ porting workaround. This class explicitly moved from Aspose.Words.Tests.Layout to here
    /// because both Tests.Layout and Tests.Rendering depend on this class,
    /// but C++ porter doesn't allow dependency between two test projects.</remarks>
    public abstract class TestBaseStandardCulture
    {
        /// <summary>
        /// Performs a one time setup of the test environment.
        /// </summary>
        [TestFixtureSetUp]
        public void OneTimeSetUp()
        {
            TestUtil.SetUpTests();
        }

        /// <summary>
        /// Sets the standard culture before every test in the derived class.
        /// </summary>
        /// <remarks>
        /// Parsing of numbers and dates is performed using the current culture and this is by design.
        /// So we have to select the standard culture before we run a test in order to pass on all
        /// development machines that can have different cultures selected.
        /// </remarks>
        [SetUp]
        public void SetStandardCulture()
        {
            SystemPal.SaveCulture();
            SystemPal.SetStandardCulture();
        }

        /// <summary>
        /// Reverts the culture to the original one after every test in the derived class.
        /// </summary>
        [TearDown]
        public void RevertToOldCulture()
        {
            SystemPal.RestoreCulture();
        }
    }
}
